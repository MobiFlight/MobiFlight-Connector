import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

// Resolve __dirname for ES modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Paths to the translation files
const localesDir = path.join(__dirname, '../../public/locales');
const enFilePath = path.join(localesDir, 'en/translation.json');

// Define core languages
const coreLanguages = ['en', 'de', 'es'];

// Helper function to recursively get all keys from an object
function getAllKeys(obj, prefix = '') {
    return Object.keys(obj).reduce((keys, key) => {
        const fullKey = prefix ? `${prefix}.${key}` : key;
        if (typeof obj[key] === 'object' && obj[key] !== null) {
            keys.push(...getAllKeys(obj[key], fullKey));
        } else {
            keys.push(fullKey);
        }
        return keys;
    }, []);
}

// Main function to check translations
async function checkTranslations() {
    if (!fs.existsSync(enFilePath)) {
        console.error(`English translation file not found at ${enFilePath}`);
        process.exit(1);
    }

    const enTranslations = JSON.parse(fs.readFileSync(enFilePath, 'utf8'));
    const enKeys = getAllKeys(enTranslations);

    const languages = fs.readdirSync(localesDir).filter(lang => lang !== 'en');

    const stats = [];
    let missingKeysForLanguage = null;

    languages.forEach(lang => {
        const langFilePath = path.join(localesDir, lang, 'translation.json');
        if (!fs.existsSync(langFilePath)) {
            console.warn(`Translation file for language "${lang}" not found.`);
            stats.push({ language: lang, missingKeys: enKeys.length, totalKeys: enKeys.length, percentageComplete: 0 });
            return;
        }

        const langTranslations = JSON.parse(fs.readFileSync(langFilePath, 'utf8'));
        const langKeys = getAllKeys(langTranslations);

        const missingKeys = enKeys.filter(key => !langKeys.includes(key));
        const percentageComplete = ((enKeys.length - missingKeys.length) / enKeys.length) * 100;

        stats.push({
            language: lang,
            missingKeys: missingKeys.length,
            totalKeys: enKeys.length,
            percentageComplete: percentageComplete.toFixed(2),
        });

        // If the user provided a language key, store the missing keys for that language
        const userLanguage = process.argv[2]?.toLowerCase();
        if (userLanguage && lang.toLowerCase() === userLanguage) {
            missingKeysForLanguage = missingKeys;
        }
    });

    // Output statistics in plain text
    console.log('Translation Statistics:');
    stats.forEach(stat => {
        const isCore = coreLanguages.includes(stat.language);
        console.log(
            `Language: ${stat.language.toUpperCase()} ${isCore ? '(Core)' : ''}\n` +
            `  - Percentage Complete: ${stat.percentageComplete}%\n` +
            `  - Missing Keys: ${stat.missingKeys}/${stat.totalKeys}\n`
        );
    });

    // Print missing keys for the specified language
    if (missingKeysForLanguage) {
        console.log(`\nMissing Keys for Language: ${process.argv[2].toUpperCase()}`);
        missingKeysForLanguage.forEach(key => console.log(`  - ${key}`));
    }

    // Generate GitHub workflow comment
    const githubComment = generateGitHubComment(stats);
    console.log('\nGitHub Workflow Comment:\n');
    console.log(githubComment);
}

// Function to generate a GitHub comment
function generateGitHubComment(stats) {
    let comment = '### Translation Statistics\n\n';
    comment += '| Language | Core Language | Percentage Complete | Missing Keys |\n';
    comment += '|----------|---------------|---------------------|--------------|\n';

    stats.forEach(stat => {
        const isCore = coreLanguages.includes(stat.language);
        comment += `| ${stat.language.toUpperCase()} | ${isCore ? '✅' : '❌'} | ${stat.percentageComplete}% | ${stat.missingKeys} |\n`;
    });

    return comment;
}

// Run the script
checkTranslations();