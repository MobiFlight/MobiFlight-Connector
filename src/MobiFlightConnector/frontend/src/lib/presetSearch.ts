// Preset search and order, shared with MobiFlight Connector: keep this file free of
// project imports so it can be copied as is, and both give the same results.

export interface SearchablePreset {
  vendor?: string
  aircraft?: string
  label: string
  system?: string
  description?: string | null
  code?: string
  author?: string
  updatedBy?: string
}

// A word found in a heavier field makes a preset more relevant
const FIELD_WEIGHTS: readonly [keyof SearchablePreset, number][] = [
  ["label", 5],
  ["system", 4],
  ["description", 3],
  ["code", 2],
  ["author", 1],
  ["updatedBy", 1],
]
const MAX_WEIGHT = 5

// "Landing  ON" -> ["landing", "on"]
export const searchTerms = (text: string) =>
  text.toLowerCase().trim().split(/\s+/).filter(Boolean)

// Relevance in percent (0-100). A preset matches only if every term is found
// in one of its fields, in any order; partial words count. Each term adds the
// weight of the best field it is found in. 0 means no match.
export const presetRelevance = (
  preset: SearchablePreset,
  terms: string[],
): number => {
  if (terms.length === 0) return 100
  const fields = FIELD_WEIGHTS.map(
    ([field, weight]) => [preset[field]?.toLowerCase() ?? "", weight] as const,
  )
  const weights = terms.map((term) =>
    Math.max(
      0,
      ...fields
        .filter(([value]) => value.includes(term))
        .map(([, weight]) => weight),
    ),
  )
  if (weights.includes(0)) return 0
  const total = weights.reduce((sum, weight) => sum + weight, 0)
  return Math.round((total / (MAX_WEIGHT * terms.length)) * 100)
}

// Compares text ignoring case, with numbers in natural order ("A320" before "A1000").
// One shared collator: localeCompare with options is much slower on 35,000 presets.
export const compareText = new Intl.Collator("en", {
  sensitivity: "base",
  numeric: true,
}).compare

// Default preset order: vendor, aircraft, then name. Also orders presets of
// equal relevance, so the list doesn't depend on the order the server sends.
export const comparePresets = (a: SearchablePreset, b: SearchablePreset) =>
  compareText(a.vendor ?? "", b.vendor ?? "") ||
  compareText(a.aircraft ?? "", b.aircraft ?? "") ||
  compareText(a.label ?? "", b.label ?? "")
