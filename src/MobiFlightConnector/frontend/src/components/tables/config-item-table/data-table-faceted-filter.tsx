import { Column } from "@tanstack/react-table"

import FacetedFilterOptions, {
  FacetedFilterOption,
} from "@/components/FacetedFilterOptions"

interface DataTableFacetedFilterProps<TData, TValue> {
  disabled?: boolean
  column?: Column<TData, TValue>
  title?: string
  options: FacetedFilterOption[]
}

/**
 * Maps a TanStack Table column onto `FacetedFilterOptions`: the column
 * supplies the facet counts and owns the selection, while the menu itself
 * stays unaware that a table exists.
 */
export function DataTableFacetedFilter<TData, TValue>({
  disabled = false,
  column,
  title,
  options,
}: DataTableFacetedFilterProps<TData, TValue>) {
  // TODO: Fix this to work with React Compiler, changes done to table columns does not trigger a re-render
  "use no memo"

  return (
    <FacetedFilterOptions
      disabled={disabled}
      title={title}
      options={options}
      facets={column?.getFacetedUniqueValues()}
      values={(column?.getFilterValue() as string[]) ?? []}
      onValuesChange={(values) =>
        column?.setFilterValue(values.length ? values : undefined)
      }
    />
  )
}
