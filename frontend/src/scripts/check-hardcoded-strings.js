import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';
import {
    shouldIgnoreFile,
    detectI18nUsage,
    extractHardcodedStrings
} from './modules/i18n-detection-rules.js';

// Resolve __dirname for ES modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Main configuration (non-detection related)
const config = {
    // Directories to scan
    scanDirs: [
        path.join(__dirname, '../components'),
        path.join(__dirname, '../pages'),
        path.join(__dirname, '../lib')
    ],
    // File extensions to check
    extensions: ['.tsx', '.ts', '.jsx', '.js']
};

/**
 * Analyze a file for hardcoded strings
 */
function analyzeFile(content, filePath) {
    const hardcodedStrings = extractHardcodedStrings(content);
    const usesI18n = detectI18nUsage(content);
    
    return {
        hardcodedStrings,
        usesI18n,
        fileHasTranslations: usesI18n
    };
}

/**
 * Scan a directory recursively for files to analyze
 */
function scanDirectory(dirPath, results = []) {
    if (!fs.existsSync(dirPath)) {
        console.warn(`Directory not found: ${dirPath}`);
        return results;
    }
    
    const items = fs.readdirSync(dirPath);
    
    items.forEach(item => {
        const fullPath = path.join(dirPath, item);
        const stat = fs.statSync(fullPath);
        
        if (stat.isDirectory() && !shouldIgnoreFile(fullPath)) {
            scanDirectory(fullPath, results);
        } else if (stat.isFile() && !shouldIgnoreFile(fullPath)) {
            const ext = path.extname(fullPath);
            if (config.extensions.includes(ext)) {
                try {
                    const content = fs.readFileSync(fullPath, 'utf8');
                    const analysis = analyzeFile(content, fullPath);
                    
                    if (analysis.hardcodedStrings.length > 0) {
                        results.push({
                            file: path.relative(process.cwd(), fullPath),
                            ...analysis
                        });
                    }
                } catch (error) {
                    console.error(`Error reading file ${fullPath}:`, error.message);
                }
            }
        }
    });
    
    return results;
}

/**
 * Generate a detailed report of findings
 */
function generateReport(results) {
    const totalFiles = results.length;
    const totalStrings = results.reduce((sum, file) => sum + file.hardcodedStrings.length, 0);
    const filesWithI18n = results.filter(file => file.usesI18n).length;
    
    console.log('### I18n Compliance Report\n');
    console.log(`**Total files with hardcoded strings:** ${totalFiles}`);
    console.log(`**Total hardcoded strings found:** ${totalStrings}`);
    console.log(`**Files using i18n:** ${filesWithI18n}/${totalFiles}`);
    console.log();
    
    if (results.length === 0) {
        console.log('✅ No hardcoded strings detected!');
        return;
    }
    
    // Sort by number of issues (most problematic files first)
    results.sort((a, b) => b.hardcodedStrings.length - a.hardcodedStrings.length);
    
    results.forEach(file => {
        console.log(`\n📄 **${file.file}** ${file.usesI18n ? '(uses i18n)' : '(no i18n detected)'}`);
        console.log(`   Issues: ${file.hardcodedStrings.length}`);
        
        file.hardcodedStrings.forEach(issue => {
            console.log(`   - Line ${issue.line}: "${issue.string}"`);
            console.log(`     Context: ${issue.context}`);
        });
    });
}

/**
 * Generate GitHub comment format for CI/CD integration
 */
function generateGitHubComment(results) {
    const totalFiles = results.length;
    const totalStrings = results.reduce((sum, file) => sum + file.hardcodedStrings.length, 0);
    
    if (totalStrings === 0) {
        return '✅ **I18n Compliance Check Passed** - No hardcoded strings detected!';
    }
    
    let comment = '### 🌐 I18n Compliance Issues Detected\n\n';
    comment += `- **${totalFiles}** files contain hardcoded strings\n`;
    comment += `- **${totalStrings}** hardcoded strings found\n\n`;
    
    comment += '<details>\n<summary>Click to see details</summary>\n\n';
    
    results.slice(0, 10).forEach(file => { // Limit to first 10 files to avoid huge comments
        comment += `**${file.file}** (${file.hardcodedStrings.length} issues)\n`;
        file.hardcodedStrings.slice(0, 5).forEach(issue => { // Limit to first 5 issues per file
            comment += `- Line ${issue.line}: \`"${issue.string}"\`\n`;
        });
        if (file.hardcodedStrings.length > 5) {
            comment += `- ... and ${file.hardcodedStrings.length - 5} more\n`;
        }
        comment += '\n';
    });
    
    if (totalFiles > 10) {
        comment += `... and ${totalFiles - 10} more files\n\n`;
    }
    
    comment += '</details>\n\n';
    comment += '💡 **Tip**: Use the `t()` function from `react-i18next` to make strings translatable.';
    
    return comment;
}

/**
 * Main function to run the i18n compliance check
 */
async function checkI18nCompliance() {
    console.log('🔍 Scanning for hardcoded strings...\n');
    
    let allResults = [];
    
    config.scanDirs.forEach(dir => {
        if (fs.existsSync(dir)) {
            console.log(`Scanning: ${path.relative(process.cwd(), dir)}`);
            const results = scanDirectory(dir);
            allResults = allResults.concat(results);
        }
    });
    
    console.log('\n' + '='.repeat(50));
    generateReport(allResults);
    
    // Generate GitHub comment if requested
    if (process.argv.includes('--github')) {
        console.log('\n' + '='.repeat(50));
        console.log('GitHub Workflow Comment:');
        console.log(generateGitHubComment(allResults));
    }
    
    // Exit with error code if issues found (for CI/CD)
    if (allResults.length > 0 && process.argv.includes('--strict')) {
        process.exit(1);
    }
}

// Run the script
checkI18nCompliance();