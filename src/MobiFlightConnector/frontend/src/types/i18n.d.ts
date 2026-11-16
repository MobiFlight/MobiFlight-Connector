import "i18next"

// Typed translation keys are disabled for now.
// Re-enable by adding `resources` (typeof the JSON files in public/locales/en)
// once dynamic keys are backed by proper domain types.
declare module "i18next" {
  interface CustomTypeOptions {
    defaultNS: "translation"
    fallbackNS: "translation"
  }
}
