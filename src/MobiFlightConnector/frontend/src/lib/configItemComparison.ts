import _ from "lodash"
import type { IConfigItem } from "@/types/config"

const normalizeForComparison = (
  item: IConfigItem | undefined,
): IConfigItem | undefined => {
  if (!item) return item

  return {
    ...item,
    Preconditions: item.Preconditions?.length ? item.Preconditions : undefined,
    ConfigRefs: item.ConfigRefs?.length ? item.ConfigRefs : undefined,
    Modifiers: item.Modifiers?.Items?.length ? item.Modifiers : undefined,
  }
}

export const areConfigItemsEquivalent = (
  first: IConfigItem | undefined,
  second: IConfigItem | undefined,
): boolean => {
  return _.isEqual(
    normalizeForComparison(first),
    normalizeForComparison(second),
  )
}
