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
import { useCallback, useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next"
import LoadingSpinner from "../LoadingSpinner"

export type XplanePresetPanelProps = {
  variant: "input" | "output"
  isLoading: boolean
  selectedPath: string | null
  setSelectedPreset: (preset: XplanePreset | Preset | null) => void
}
const XplanePresetPanel = ({
  variant,
  isLoading,
  selectedPath,
  setSelectedPreset,
}: XplanePresetPanelProps) => {
  const { t } = useTranslation()
  const { project } = useProjectStore()
  const [favoritesOnly, setFavoritesOnly] = useState(true)

  const SCROLL_INTO_VIEW_TIMEOUT = 800
  const refActiveElement = useRef<HTMLDivElement | null>(null)
  const scrollTimeoutRef = useRef<number | null>(null)

  const cancelScrollIntoView = () => {
    if (scrollTimeoutRef.current !== null) {
      window.clearTimeout(scrollTimeoutRef.current)
      scrollTimeoutRef.current = null
    }
  }

  const scrollActiveProjectIntoView = useCallback(() => {
    if (refActiveElement.current) {
      cancelScrollIntoView()
      scrollTimeoutRef.current = window.setTimeout(() => {
        refActiveElement.current?.scrollIntoView({
          behavior: "smooth",
          block: "nearest",
        })
        scrollTimeoutRef.current = null
      }, SCROLL_INTO_VIEW_TIMEOUT)
    }
  }, [refActiveElement, scrollTimeoutRef])

  const validPresetTypes =
    variant === "input"
      ? ["input", "inputoutput", "input (potentiometer)"]
      : ["output", "inputoutput"]

  const { data: presets = [] } = useQuery({
    queryKey: ["xplane-presets"],
    queryFn: () => fetchHubHopPresets("xplane") as Promise<XplanePreset[]>,
    staleTime: Infinity,
  })

  const projectAircraftFilter = (p: XplanePreset) =>
    (project?.Aircraft?.length ?? 0) > 0
      ? project!.Aircraft!.some(
          (a: AircraftInfo) => a.Name === p.aircraft && a.Vendor === p.vendor,
        )
      : true

  const validPresets = presets.filter(
    (p: XplanePreset) =>
      validPresetTypes.includes(p.presetType.toLowerCase()) &&
      (favoritesOnly ? projectAircraftFilter(p) : true),
  )

  const selectedPreset = validPresets.find((p) => p.code === selectedPath)

  const [filter, setFilter] = useState({
    vendor: selectedPreset?.vendor || "",
    aircraft: selectedPreset?.aircraft || "",
    system: selectedPreset?.system || "",
    search: "",
  })

  const filteredPresets = validPresets.filter(
    (p) =>
      (filter.vendor ? p.vendor === filter.vendor : true) &&
      (filter.aircraft ? p.aircraft === filter.aircraft : true) &&
      (filter.system ? p.system === filter.system : true) &&
      filterPresetByText(p, filter.search),
  )

  const filteredCategoryPresets = validPresets.filter(
    (p) =>
      (filter.vendor ? p.vendor === filter.vendor : true) &&
      (filter.aircraft ? p.aircraft === filter.aircraft : true) &&
      filterPresetByText(p, filter.search),
  )

  const filteredVendorPresets = validPresets.filter(
    (p) =>
      (filter.system ? p.system === filter.system : true) &&
      (filter.aircraft ? p.aircraft === filter.aircraft : true) &&
      filterPresetByText(p, filter.search),
  )

  const filteredAircraftPresets = validPresets.filter(
    (p) =>
      (filter.vendor ? p.vendor === filter.vendor : true) &&
      (filter.system ? p.system === filter.system : true) &&
      filterPresetByText(p, filter.search),
  )

  const categories = [
    ...new Set([
      ...filteredCategoryPresets.map((p) => p.system),
      ...(filter.system ? [filter.system] : []),
    ]),
  ].sort()
  const aircraft = [
    ...new Set([
      ...filteredAircraftPresets.map((p) => p.aircraft),
      ...(filter.aircraft ? [filter.aircraft] : []),
    ]),
  ].sort()
  const vendors = [
    ...new Set([
      ...filteredVendorPresets.map((p) => p.vendor),
      ...(filter.vendor ? [filter.vendor] : []),
    ]),
  ].sort()

  useEffect(() => {
    if (!refActiveElement.current) return
    scrollActiveProjectIntoView()
  }, [refActiveElement, scrollActiveProjectIntoView])

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
          {isLoading ? <LoadingSpinner /> : (
          <ScrollArea
            className="h-56"
            onMouseEnter={cancelScrollIntoView}
            onMouseLeave={scrollActiveProjectIntoView}
          >
            <div className="flex flex-col gap-1" role="list">
              {filteredPresets.map((preset) => (
                <PresetListItem
                  ref={preset.code === selectedPath ? refActiveElement : null}
                  key={preset.code}
                  preset={preset}
                  isSelected={preset.code === selectedPath}
                  setSelectedPreset={setSelectedPreset}
                />
              ))}
            </div>
          </ScrollArea>
        )}
        </div>
      </CardContent>
    </Card>
  )
}

export default XplanePresetPanel
