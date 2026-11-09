import "i18next"
import translation from "../../public/locales/en/translation.json"
import wizard from "../../public/locales/en/wizard.json"

declare module "i18next" {
  interface CustomTypeOptions {
    defaultNS: "translation"
    fallbackNS: "translation"
    resources: {
      translation: typeof translation
      wizard: typeof wizard
    }
  }
}