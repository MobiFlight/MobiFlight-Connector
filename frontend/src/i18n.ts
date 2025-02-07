import i18n from "i18next"
import Backend from "i18next-http-backend"
import LanguageDetector from "@/lib/languageDetector"
import { initReactI18next } from "react-i18next"

export default i18n
  .use(LanguageDetector)
  .use(Backend)
  .use(initReactI18next)
  .init({
    fallbackLng: "en",
    debug: true,
  })
