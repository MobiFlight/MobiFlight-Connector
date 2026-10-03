import { ScrollArea } from "@/components/ui/scroll-area"
import { PresetListItem } from "@/components/wizard/components/PresetListItem"
import { Preset, XplanePreset } from "@/types/preset"
import { useVirtualizer } from "@tanstack/react-virtual"
import { useCallback, useEffect, useRef } from "react"

const SCROLL_INTO_VIEW_TIMEOUT = 800

type VirtualPresetListProps = {
  presets: Preset[]
  selectedId: string | null
  getPresetId: (preset: Preset) => string
  setSelectedPreset: (preset: Preset | XplanePreset | null) => void
}

export const VirtualPresetList = ({
  presets,
  selectedId,
  getPresetId,
  setSelectedPreset,
}: VirtualPresetListProps) => {
  "use no memo"

  const viewportRef = useRef<HTMLDivElement | null>(null)
  const scrollTimeoutRef = useRef<number | null>(null)

  const getItemKey = useCallback(
    (index: number) => (presets[index] ? getPresetId(presets[index]) : index),
    [presets, getPresetId],
  )

  const rowVirtualizer = useVirtualizer({
    count: presets.length,
    getScrollElement: () => viewportRef.current,
    estimateSize: () => 48,
    overscan: 8,
    getItemKey,
  })

  const cancelScrollIntoView = () => {
    if (scrollTimeoutRef.current !== null) {
      window.clearTimeout(scrollTimeoutRef.current)
      scrollTimeoutRef.current = null
    }
  }

  const scrollSelectedIntoView = () => {
    cancelScrollIntoView()
    if (!selectedId) return
    const index = presets.findIndex((p) => getPresetId(p) === selectedId)
    if (index < 0) return
    scrollTimeoutRef.current = window.setTimeout(() => {
      // smooth scrolling is not supported by the virtualizer when rows are
      // measured dynamically
      rowVirtualizer.scrollToIndex(index, { align: "auto" })
      scrollTimeoutRef.current = null
    }, SCROLL_INTO_VIEW_TIMEOUT)
  }

  useEffect(() => {
    scrollSelectedIntoView()
    return cancelScrollIntoView
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  return (
    <ScrollArea
      className="h-56"
      viewportRef={viewportRef}
      onMouseEnter={cancelScrollIntoView}
      onMouseLeave={scrollSelectedIntoView}
    >
      <div
        role="list"
        className="relative w-full"
        style={{ height: `${rowVirtualizer.getTotalSize()}px` }}
      >
        {rowVirtualizer.getVirtualItems().map((virtualRow) => {
          const preset = presets[virtualRow.index]
          return (
            <div
              key={virtualRow.key}
              ref={rowVirtualizer.measureElement}
              data-index={virtualRow.index}
              className="absolute top-0 left-0 w-full pb-1"
              style={{ transform: `translateY(${virtualRow.start}px)` }}
            >
              <PresetListItem
                preset={preset}
                isSelected={getPresetId(preset) === selectedId}
                setSelectedPreset={setSelectedPreset}
              />
            </div>
          )
        })}
      </div>
    </ScrollArea>
  )
}
