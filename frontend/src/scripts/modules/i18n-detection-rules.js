/**
 * I18n Detection Rules Configuration
 * Separated from main logic for easier maintenance and testing
 */

export const I18N_DETECTION_RULES = {
    // Patterns that indicate i18n usage
    i18nPatterns: [
        /t\s*\(\s*["'`]([^"'`]+)["'`]/g, // t("key")
        /\{\s*t\s*\(\s*["'`]([^"'`]+)["'`]/g, // {t("key")}
        /useTranslation/g, // useTranslation hook
        /Trans\s+i18nKey=/g, // <Trans i18nKey="key">
    ],

    // Patterns to detect hardcoded strings
    hardcodedPatterns: [
        // JSX text content between tags (more specific)
        />([^<>{}\n\r\t\s][^<>{}]*[a-zA-Z][^<>{}]*)</g,
        // String literals in specific JSX attributes that should be translated
        /(?:title|placeholder|aria-label|alt|label)\s*=\s*["']([^"'{}]+)["']/g,
        // Button/Interactive element text (more specific)
        /<(?:Button|Label|span|div|p|h[1-6])[^>]*>\s*([^<>{}\n\r\t\s][^<>{}]*[a-zA-Z][^<>{}]*)\s*</g,
        // String literals that look like user-facing text (in quotes)
        /["']([A-Z][a-zA-Z\s]{3,}[.!?]?)["']/g,
    ],

    // Strings to ignore (common non-translatable strings)
    ignorePatterns: [
        // Basic patterns
        /^[0-9\s\-_.,;:!?()[\]{}/*+=<>@#$%^&|\\~`"']*$/, // Numbers, symbols, whitespace
        /^[a-f0-9-]{36}$/, // UUIDs
        /^[a-f0-9]{8,}$/, // Hex strings
        /^(px|em|rem|vh|vw|%|\d+)$/, // CSS values
        /^(true|false|null|undefined)$/, // Literals
        /^[A-Z_][A-Z0-9_]*$/, // Constants (ALL_CAPS)

        // URLs and paths
        /^(https?|ftp|file):\/\//, // URLs
        /^\w+@\w+\.\w+/, // Email addresses
        /^\/[^/]*/, // File paths starting with /
        /^\.[^.]*/, // File extensions
        /^[a-zA-Z0-9_-]+\.(jpg|jpeg|png|gif|svg|ico|css|js|ts|tsx|jsx|json|xml|html)$/i, // File names

        // HTTP and web
        /^(GET|POST|PUT|DELETE|PATCH|HEAD|OPTIONS)$/, // HTTP methods
        /^(text|application|image|audio|video)\//, // MIME types

        // CSS related
        /^[rgb|hsl|#][a-f0-9(),.\s%]*$/i, // Colors
        /^(flex|grid|block|inline|none|relative|absolute|fixed)$/, // CSS display values
        /^(left|right|center|top|bottom|middle)$/, // Alignment values
        /^className$/, // React props
        /^[\w-]+:[\w-]+$/, // CSS-like key:value

        // TypeScript and React specific patterns
        /^React\./i, // React namespaced types
        /^typeof\s+/i, // typeof expressions
        /ComponentPropsWithoutRef/i, // React type utilities
        /ElementRef/i, // React type utilities
        /forwardRef/i, // React forwardRef
        /^[A-Z][a-zA-Z]*Primitive/i, // Primitive component types
        /^[a-zA-Z]+\.[A-Z][a-zA-Z]*$/i, // Namespaced types like DropdownMenuPrimitive.Item

        // Naming patterns
        /^use[A-Z][a-zA-Z]*$/i, // Hook names
        /^[a-z][a-zA-Z]*Ref$/i, // Ref variable names
        /^[a-z][a-zA-Z]*Props$/i, // Props type names
        /^\w+Context$/i, // Context names
        /^\w+Provider$/i, // Provider names
        /^set[A-Z][a-zA-Z]*$/i, // Setter function names
        /^handle[A-Z][a-zA-Z]*$/i, // Handler function names
        /^on[A-Z][a-zA-Z]*$/i, // Event handler props
        /^is[A-Z][a-zA-Z]*$/i, // Boolean props
        /^has[A-Z][a-zA-Z]*$/i, // Boolean props
        /^[a-z]+[A-Z][a-zA-Z]*$/i, // camelCase identifiers
        /^[A-Z][a-zA-Z]*$/, // PascalCase identifiers (components, types)

        // Browser APIs and built-ins
        /^\w+\(\)$/i, // Function calls
        /^console\./i, // Console methods
        /^process\./i, // Process methods
        /^window\./i, // Window methods
        /^document\./i, // Document methods
        /^Math\./i, // Math methods
        /^JSON\./i, // JSON methods
        /^Object\./i, // Object methods
        /^Array\./i, // Array methods
        /^String\./i, // String methods
        /^Number\./i, // Number methods
        /^Date\./i, // Date methods
        /^RegExp\./i, // RegExp methods

        // Generic programming terms
        /^(key|value|index|length|size|count|id|name|type|class|style|data|config|options|params|args|result|error|success|fail|callback|promise|async|await|return|export|import|default|const|let|var|function|class|interface|enum|namespace)$/i,

        // Short strings and acronyms
        /^[a-z]{1,4}$/i, // Very short strings (likely variable names)
        /^[A-Z]{2,}$/i, // Acronyms
    ],

    // Context patterns that indicate the string is likely not user-facing
    contextIgnorePatterns: [
        /interface\s+\w+/i, // Interface definitions
        /type\s+\w+/i, // Type definitions
        /import\s+.*from/i, // Import statements
        /export\s+/i, // Export statements
        /React\.ElementRef<typeof/i, // React type definitions
        /React\.ComponentPropsWithoutRef</i, // React type definitions
        /forwardRef</i, // forwardRef usage
        /typeof\s+\w+/i, // typeof expressions
        /\w+Primitive\./i, // Primitive component references
        /as\s+\w+/i, // Type assertions
        /:\s*\w+/i, // Type annotations
        /extends\s+\w+/i, // Type extensions
        /implements\s+\w+/i, // Interface implementations
        /console\.(log|warn|error|info)/i, // Console statements
        /className\s*=/i, // CSS class assignments
        /style\s*=/i, // Style assignments
    ]
};

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
 * Detect if file uses i18n based on content
 */
export function detectI18nUsage(content) {
    return I18N_DETECTION_RULES.i18nPatterns.some(pattern => pattern.test(content));
}

/**
 * Extract hardcoded strings from content using configured patterns
 */
export function extractHardcodedStrings(content) {
    const foundStrings = [];
    const lines = content.split('\n');
    
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