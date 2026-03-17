import { LanguageDetectorModule } from "i18next"
const LanguageDetector: LanguageDetectorModule = {
  type: "languageDetector",
  detect: () => {
    /* return detected language */
    const userLanguage = navigator.language || "en"
    return userLanguage
  },
}

export default LanguageDetector
