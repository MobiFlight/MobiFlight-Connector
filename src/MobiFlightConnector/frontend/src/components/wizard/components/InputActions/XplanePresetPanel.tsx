import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Switch } from "@/components/ui/switch"
import { PresetListItem } from "@/components/wizard/components/PresetListItem"
import { fetchHubHopPresets, filterPresetByText } from "@/lib/configWizard"
import { useProjectStore } from "@/stores/projectStore"
import { Preset, XplanePreset } from "@/types/preset"
import { AircraftInfo } from "@/types/project"
import { IconX } from "@tabler/icons-react"
import { useQuery } from "@tanstack/react-query"
import { useVirtualizer } from "@tanstack/react-virtual"
import {
  useCallback,
  useDeferredValue,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react"
import { useTranslation } from "react-i18next"

export type XplanePresetPanelProps = {
  variant: "input" | "output"
  selectedPath: string | null
  setSelectedPreset: (preset: XplanePreset | Preset | null) => void
}

// Outside the component so it isn't a fresh value on every render (it is a
// dependency of scrollActiveProjectIntoView below).
const SCROLL_INTO_VIEW_TIMEOUT = 800

const XplanePresetPanel = ({
  variant,
  selectedPath,
  setSelectedPreset,
}: XplanePresetPanelProps) => {
  const { t } = useTranslation()
  const { project } = useProjectStore()
  const [favoritesOnly, setFavoritesOnly] = useState(true)

  const viewportRef = useRef<HTMLDivElement | null>(null)
  const scrollTimeoutRef = useRef<number | null>(null)

  const cancelScrollIntoView = () => {
    if (scrollTimeoutRef.current !== null) {
      window.clearTimeout(scrollTimeoutRef.current)
      scrollTimeoutRef.current = null
    }
  }

  // Give PresetListItem a stable callback identity so React.memo on it is
  // actually effective, without requiring callers to memoize setSelectedPreset.
  const setSelectedPresetRef = useRef(setSelectedPreset)
  useEffect(() => {
    setSelectedPresetRef.current = setSelectedPreset
  })
  const handleSelectPreset = useCallback(
    (preset: Preset | XplanePreset | null) =>
      setSelectedPresetRef.current(preset),
    [],
  )

  const validPresetTypes = useMemo(
    () =>
      variant === "input"
        ? ["input", "inputoutput", "input (potentiometer)"]
        : ["output", "inputoutput"],
    [variant],
  )

  const { data: presets = [] } = useQuery({
    queryKey: ["xplane-presets"],
    queryFn: () => fetchHubHopPresets("xplane") as Promise<XplanePreset[]>,
    staleTime: Infinity,
  })

  const validPresets = useMemo(() => {
    const aircraftList = project?.Aircraft
    const hasAircraft = (aircraftList?.length ?? 0) > 0
    const projectAircraftFilter = (p: XplanePreset) =>
      hasAircraft
        ? aircraftList!.some(
            (a: AircraftInfo) => a.Name === p.aircraft && a.Vendor === p.vendor,
          )
        : true
    return presets.filter(
      (p: XplanePreset) =>
        validPresetTypes.includes(p.presetType.toLowerCase()) &&
        (favoritesOnly ? projectAircraftFilter(p) : true),
    )
  }, [presets, validPresetTypes, favoritesOnly, project?.Aircraft])

  const selectedPreset = validPresets.find((p) => p.code === selectedPath)

  const [filter, setFilter] = useState({
    vendor: selectedPreset?.vendor || "",
    aircraft: selectedPreset?.aircraft || "",
    system: selectedPreset?.system || "",
    search: "",
  })

  const deferredSearch = useDeferredValue(filter.search)

  const filteredPresets = useMemo(
    () =>
      validPresets.filter(
        (p) =>
          (filter.vendor ? p.vendor === filter.vendor : true) &&
          (filter.aircraft ? p.aircraft === filter.aircraft : true) &&
          (filter.system ? p.system === filter.system : true) &&
          filterPresetByText(p, deferredSearch),
      ),
    [validPresets, filter.vendor, filter.aircraft, filter.system, deferredSearch],
  )

  const filteredCategoryPresets = useMemo(
    () =>
      validPresets.filter(
        (p) =>
          (filter.vendor ? p.vendor === filter.vendor : true) &&
          (filter.aircraft ? p.aircraft === filter.aircraft : true) &&
          filterPresetByText(p, deferredSearch),
      ),
    [validPresets, filter.vendor, filter.aircraft, deferredSearch],
  )

  const filteredVendorPresets = useMemo(
    () =>
      validPresets.filter(
        (p) =>
          (filter.system ? p.system === filter.system : true) &&
          (filter.aircraft ? p.aircraft === filter.aircraft : true) &&
          filterPresetByText(p, deferredSearch),
      ),
    [validPresets, filter.system, filter.aircraft, deferredSearch],
  )

  const filteredAircraftPresets = useMemo(
    () =>
      validPresets.filter(
        (p) =>
          (filter.vendor ? p.vendor === filter.vendor : true) &&
          (filter.system ? p.system === filter.system : true) &&
          filterPresetByText(p, deferredSearch),
      ),
    [validPresets, filter.vendor, filter.system, deferredSearch],
  )

  const categories = useMemo(
    () =>
      [
        ...new Set([
          ...filteredCategoryPresets.map((p) => p.system),
          ...(filter.system ? [filter.system] : []),
        ]),
      ].sort(),
    [filteredCategoryPresets, filter.system],
  )
  const aircraft = useMemo(
    () =>
      [
        ...new Set([
          ...filteredAircraftPresets.map((p) => p.aircraft),
          ...(filter.aircraft ? [filter.aircraft] : []),
        ]),
      ].sort(),
    [filteredAircraftPresets, filter.aircraft],
  )
  const vendors = useMemo(
    () =>
      [
        ...new Set([
          ...filteredVendorPresets.map((p) => p.vendor),
          ...(filter.vendor ? [filter.vendor] : []),
        ]),
      ].sort(),
    [filteredVendorPresets, filter.vendor],
  )

  const selectedIndex = useMemo(
    () => filteredPresets.findIndex((p) => p.code === selectedPath),
    [filteredPresets, selectedPath],
  )

  const getPresetKey = useCallback(
    (index: number) => filteredPresets[index].id,
    [filteredPresets],
  )

  const rowVirtualizer = useVirtualizer({
    count: filteredPresets.length,
    getScrollElement: () => viewportRef.current,
    // Initial estimate only; measureElement refines it once a row mounts.
    // Matches a single-line row: 20px title, 12px description, 2px borders,
    // 8px vertical padding and the 4px gap on the wrapper.
    estimateSize: () => 48,
    overscan: 8,
    getItemKey: getPresetKey,
  })

  const scrollActiveProjectIntoView = useCallback(() => {
    if (selectedIndex < 0) return
    cancelScrollIntoView()
    scrollTimeoutRef.current = window.setTimeout(() => {
      rowVirtualizer.scrollToIndex(selectedIndex, { align: "center" })
      scrollTimeoutRef.current = null
    }, SCROLL_INTO_VIEW_TIMEOUT)
  }, [selectedIndex, rowVirtualizer])

  useEffect(() => {
    scrollActiveProjectIntoView()
    return cancelScrollIntoView
  }, [scrollActiveProjectIntoView])

  return (
    <Card className="grow">
      <CardContent className="flex flex-col gap-4 pt-4">
        <div className="flex flex-row items-end justify-between">
          <div className="flex flex-col">
            <div className="text-lg font-semibold">
              {t("Dialog.InputConfigWizard.InputActions.Common.Preset.Title")}
            </div>
            <div className="text-muted-foreground text-sm">
              {t(
                "Dialog.InputConfigWizard.InputActions.Common.Preset.Description",
              )}
            </div>
          </div>
          <div className="flex flex-row items-center gap-2">
            <Switch
              checked={favoritesOnly}
              onCheckedChange={(checked) => setFavoritesOnly(checked)}
            ></Switch>
            <Label>
              {t(
                "Dialog.InputConfigWizard.InputActions.Common.Preset.ProjectAircraftOnly",
              )}
            </Label>
          </div>
        </div>
        <div className="flex flex-col gap-1">
          <div className="grid grid-cols-4 gap-2">
            <Input
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.FilterPresets",
              )}
              value={filter.search}
              onChange={(e) =>
                setFilter((prev) => ({ ...prev, search: e.target.value }))
              }
            />
            <ComboBox
              align="start"
              widthClass="flex-1"
              selected={filter?.vendor}
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.FilterByVendor",
              )}
              getLabel={(item) => item}
              getValue={(item) => item}
              items={vendors}
              isSelected={(item) => item === filter?.vendor}
              setSelected={(item) => {
                setFilter((prev) => ({ ...prev, vendor: item || "" }))
              }}
              searchPlaceholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.SearchVendors",
              )}
            />
            <ComboBox
              align="start"
              widthClass="flex-1"
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.FilterByAircraft",
              )}
              getLabel={(item) => item}
              getValue={(item) => item}
              items={aircraft}
              selected={filter?.aircraft}
              isSelected={(item) => item === filter?.aircraft}
              setSelected={(item) => {
                setFilter((prev) => ({ ...prev, aircraft: item || "" }))
              }}
              searchPlaceholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.SearchAircraft",
              )}
            />
            <ComboBox
              align="start"
              widthClass="flex-1"
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.FilterBySystem",
              )}
              getLabel={(item) => `${item}`}
              getValue={(item) => item}
              items={categories}
              selected={filter?.system}
              isSelected={(item) => item === filter?.system}
              setSelected={(item) => {
                setFilter((prev) => ({ ...prev, system: item || "" }))
              }}
              searchPlaceholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.SearchSystems",
              )}
            />
          </div>
          <div className="flex flex-row items-center justify-between">
            <div role="status" className="px-2 py-2 text-sm">
              {t("Dialog.InputConfigWizard.InputActions.Common.PresetsFound", {
                count: filteredPresets.length,
              })}
            </div>
            {(filter.aircraft ||
              filter.vendor ||
              filter.system ||
              filter.search) && (
              <Button
                size={"sm"}
                className="w-fit px-2 py-0"
                variant="ghost"
                onClick={() =>
                  setFilter({
                    vendor: "",
                    aircraft: "",
                    system: "",
                    search: "",
                  })
                }
              >
                <IconX />
                <span className="py-0 text-sm">
                  {t("Dialog.General.ResetFilters")}
                </span>
              </Button>
            )}
          </div>
        </div>
        <div className="-mt-3 flex flex-col gap-2">
          <ScrollArea
            className="h-56"
            viewportRef={viewportRef}
            onMouseEnter={cancelScrollIntoView}
            onMouseLeave={scrollActiveProjectIntoView}
          >
            <div
              role="list"
              className="relative w-full"
              style={{ height: `${rowVirtualizer.getTotalSize()}px` }}
            >
              {rowVirtualizer.getVirtualItems().map((virtualRow) => {
                const preset = filteredPresets[virtualRow.index]
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
                      isSelected={preset.code === selectedPath}
                      setSelectedPreset={handleSelectPreset}
                    />
                  </div>
                )
              })}
            </div>
          </ScrollArea>
        </div>
      </CardContent>
    </Card>
  )
}

export default XplanePresetPanel
