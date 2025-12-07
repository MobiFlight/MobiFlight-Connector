import i18n from "i18next"
import Backend, { HttpBackendOptions } from "i18next-http-backend"
import LanguageDetector from "@/lib/languageDetector"
import { initReactI18next } from "react-i18next"

export default i18n
  .use(LanguageDetector)
  .use(Backend)
  .use(initReactI18next)
  .init<HttpBackendOptions>({
    fallbackLng: "en",
    ns: ["translation", "feed"],
    debug: true,
    maxRetries: 2,
    retryTimeout: 100,
    load: "languageOnly"
  })
