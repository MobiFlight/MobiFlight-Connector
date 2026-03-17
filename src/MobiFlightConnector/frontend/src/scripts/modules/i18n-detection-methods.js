import {
  I18N_DETECTION_RULES
} from "./i18n-detection-rules.js"
/**
 * Parse ignore blocks and line-level ignores from file content
 */
function parseIgnoreDirectives(content) {
    const lines = content.split('\n');
    const ignoredLines = new Set();
    const ignoredRanges = [];
    
    let inIgnoreBlock = false;
    let blockStartLine = -1;
    
    lines.forEach((line, index) => {
        const lineNumber = index + 1;
        
        // Check for line-level ignore comments
        const hasLineIgnore = I18N_DETECTION_RULES.ignoreComments.some(pattern => 
            pattern.test(line)
        );
        
        if (hasLineIgnore) {
            ignoredLines.add(lineNumber);
            return;
        }
        
        // Check for block start
        const hasBlockStart = I18N_DETECTION_RULES.blockIgnoreStart.some(pattern => 
            pattern.test(line)
        );
        
        if (hasBlockStart && !inIgnoreBlock) {
            inIgnoreBlock = true;
            blockStartLine = lineNumber;
            ignoredLines.add(lineNumber); // Also ignore the start line
            return;
        }
        
        // Check for block end
        const hasBlockEnd = I18N_DETECTION_RULES.blockIgnoreEnd.some(pattern => 
            pattern.test(line)
        );
        
        if (hasBlockEnd && inIgnoreBlock) {
            inIgnoreBlock = false;
            ignoredLines.add(lineNumber); // Also ignore the end line
            ignoredRanges.push({ start: blockStartLine, end: lineNumber });
            return;
        }
        
        // If we're in an ignore block, add this line to ignored
        if (inIgnoreBlock) {
            ignoredLines.add(lineNumber);
        }
    });
    
    return { ignoredLines, ignoredRanges };
}

/**
 * Check if a file should be ignored based on path patterns
 */
export function shouldIgnoreFile(filePath) {
    const ignorePatterns = [
        'test', 'spec', '.test.', '.spec.',
        'node_modules', '.git', 'dist', 'build',
        'storybook', 'stories',
        'types.ts', 'constants.ts', 'index.ts',
        '/ui/', // Ignore UI component library files
    ];
    
    return ignorePatterns.some(pattern => filePath.includes(pattern));
}

/**
 * Check if a string should be ignored based on content patterns
 */
export function shouldIgnoreString(str) {
    if (!str || str.length < 2) return true;
    return I18N_DETECTION_RULES.ignorePatterns.some(pattern => pattern.test(str));
}

/**
 * Check if the context suggests this is not user-facing text
 */
export function shouldIgnoreContext(context) {
    return I18N_DETECTION_RULES.contextIgnorePatterns.some(pattern => pattern.test(context));
}

/**
 * Check if a line should be ignored based on ignore directives
 */
export function shouldIgnoreLine(lineNumber, ignoredLines) {
    return ignoredLines.has(lineNumber);
}

/**
 * Detect if file uses i18n based on content
 */
export function detectI18nUsage(content) {
    return I18N_DETECTION_RULES.i18nPatterns.some(pattern => pattern.test(content));
}

/**
 * Extract hardcoded strings from content using configured patterns
 * Now respects @ignore directives and conditional contexts
 */
export function extractHardcodedStrings(content) {
    const foundStrings = [];
    const lines = content.split('\n');
    const { ignoredLines } = parseIgnoreDirectives(content);
    
    I18N_DETECTION_RULES.hardcodedPatterns.forEach(pattern => {
        let match;
        pattern.lastIndex = 0; // Reset regex state
        
        while ((match = pattern.exec(content)) !== null) {
            const foundString = match[1]?.trim();
            
            if (foundString && !shouldIgnoreString(foundString)) {
                // Find line number and context
                const beforeMatch = content.substring(0, match.index);
                const lineNumber = beforeMatch.split('\n').length;
                const lineContent = lines[lineNumber - 1]?.trim();
                
                // Check if this line should be ignored due to @ignore directives
                if (shouldIgnoreLine(lineNumber, ignoredLines)) {
                    continue;
                }
                
                // Check if context suggests this is not user-facing text
                if (!shouldIgnoreContext(lineContent)) {
                    foundStrings.push({
                        string: foundString,
                        line: lineNumber,
                        context: lineContent,
                        pattern: pattern.source
                    });
                }
            }
        }
    });
    
    return foundStrings;
}