import { describe, it, expect } from "vitest"
import fs from "fs"
import path from "path"
import { fileURLToPath } from "url"

// Guards against translation edits breaking interpolation or 
// markup at runtime, e.g. a placeholder like {{count}} getting 
// translated.

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const localesDir = path.resolve(__dirname, "../../public/locales")

const PLACEHOLDER_RE = /\{\{[^}]+\}\}/g
const TAG_RE = /<\/?[a-zA-Z0-9]+>/g

function flatten(obj: unknown, prefix = ""): Record<string, string> {
  const out: Record<string, string> = {}
  if (typeof obj !== "object" || obj === null) return out
  for (const [key, value] of Object.entries(obj as Record<string, unknown>)) {
    const fullKey = prefix ? `${prefix}.${key}` : key
    if (typeof value === "object" && value !== null) {
      Object.assign(out, flatten(value, fullKey))
    } else if (typeof value === "string") {
      out[fullKey] = value
    }
  }
  return out
}

function extractSorted(value: string, re: RegExp): string[] {
  return [...(value.match(re) ?? [])].sort()
}

const languages = fs
  .readdirSync(localesDir)
  .filter((entry) => fs.statSync(path.join(localesDir, entry)).isDirectory() && entry !== "en")

const enTranslations = flatten(
  JSON.parse(fs.readFileSync(path.join(localesDir, "en/translation.json"), "utf8")),
)

describe.each(languages)("translation.json (%s)", (lang) => {
  const filePath = path.join(localesDir, lang, "translation.json")

  it("exists and is valid JSON", () => {
    expect(fs.existsSync(filePath)).toBe(true)
    expect(() => JSON.parse(fs.readFileSync(filePath, "utf8"))).not.toThrow()
  })

  if (!fs.existsSync(filePath)) return

  const translations = flatten(JSON.parse(fs.readFileSync(filePath, "utf8")))

  // Non-core languages can lag behind at times, so missing keys are not checked
  // here. The translation coverage table in the PR shows coverage percentage.
  
  it("has no extra keys not present in en", () => {
    const extra = Object.keys(translations).filter((key) => !(key in enTranslations))
    expect(extra).toEqual([])
  })

  it("keeps interpolation placeholders (e.g. {{count}}) identical to en", () => {
    const mismatches: string[] = []
    for (const [key, enValue] of Object.entries(enTranslations)) {
      const localizedValue = translations[key]
      if (localizedValue === undefined) continue
      const enPlaceholders = extractSorted(enValue, PLACEHOLDER_RE)
      const localizedPlaceholders = extractSorted(localizedValue, PLACEHOLDER_RE)
      if (JSON.stringify(enPlaceholders) !== JSON.stringify(localizedPlaceholders)) {
        mismatches.push(`${key}: en=${JSON.stringify(enPlaceholders)} ${lang}=${JSON.stringify(localizedPlaceholders)}`)
      }
    }
    expect(mismatches).toEqual([])
  })

  it("keeps pseudo-HTML tags (e.g. <badge>, <span>, <1>) identical to en", () => {
    const mismatches: string[] = []
    for (const [key, enValue] of Object.entries(enTranslations)) {
      const localizedValue = translations[key]
      if (localizedValue === undefined) continue
      const enTags = extractSorted(enValue, TAG_RE)
      const localizedTags = extractSorted(localizedValue, TAG_RE)
      if (JSON.stringify(enTags) !== JSON.stringify(localizedTags)) {
        mismatches.push(`${key}: en=${JSON.stringify(enTags)} ${lang}=${JSON.stringify(localizedTags)}`)
      }
    }
    expect(mismatches).toEqual([])
  })
})
