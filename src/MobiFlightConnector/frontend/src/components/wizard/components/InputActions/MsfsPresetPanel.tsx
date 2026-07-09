import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Switch } from "@/components/ui/switch"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { cn } from "@/lib/utils"
import { useProjectStore } from "@/stores/projectStore"
import { AircraftInfo } from "@/types/project"
import { IconX } from "@tabler/icons-react"
import { useQuery } from "@tanstack/react-query"
import { forwardRef, useCallback, useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next"

export type Preset = {
  id: string
  vendor: string
  aircraft: string
  system: string
  label: string
  description: string
  code: string
  version: number
  status: string
  createdDate: string
  updatedBy?: string
  reported?: number
  score?: number
  presetType: "Input" | "Output" | "Input (Potentiometer)"
}

type PresetItemProps = {
  key: string
  preset: Preset
  isSelected: boolean
  setSelectedPreset: (preset: Preset | null) => void
}

const MsfsPresetItem = forwardRef<HTMLDivElement, PresetItemProps>(
  ({ key, preset, isSelected, setSelectedPreset }: PresetItemProps, ref) => {
    return (
      <div
        ref={ref}
        role="listitem"
        key={key}
        className={cn(
          "bg-background hover:bg-accent/50 flex cursor-pointer flex-row justify-between gap-2 px-2 pt-0.5 pb-1.5 rounded-md border-2 border-background",
          isSelected && "bg-primary/20 border-primary border-2",
        )}
        onClick={() => setSelectedPreset(preset)}
      >
        <div className="flex flex-col">
          <div className="text-sm font-semibold">{preset.label}</div>
          <div className="text-muted-foreground text-xs/3">
            {preset.description ?? "-"}
          </div>
        </div>
        <div className="flex flex-col justify-items-end px-2">
          <div className="text-right text-sm/5 font-semibold">
            {preset.system}
          </div>
          <div className="text-muted-foreground text-right text-xs/3">
            {preset.aircraft}
          </div>
        </div>
      </div>
    )
  },
)

export type MsfsPresetPanelProps = {
  variant: "input" | "output"
  selectedPresetId: string | null
  setSelectedPreset: (preset: Preset | null) => void
}

const MsfsPresetPanel = ({
  variant,
  selectedPresetId,
  setSelectedPreset,
}: MsfsPresetPanelProps) => {
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
    variant === "input" ? ["input", "input (potentiometer)"] : ["output"]

  const { data: presets = [] /*, isLoading */ } = useQuery({
    queryKey: ["msfs-presets"],
    queryFn: () => fetchHubHopPresets("msfs"),
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })

  const projectAircraftFilter = (p: Preset) =>
    (project?.Aircraft?.length ?? 0) > 0
      ? project!.Aircraft!.some(
          (a: AircraftInfo) => a.Name === p.aircraft && a.Vendor === p.vendor,
        )
      : true

  const validPresets = presets.filter(
    (p: Preset) =>
      validPresetTypes.includes(p.presetType.toLowerCase()) &&
      (favoritesOnly ? projectAircraftFilter(p) : true),
  )

  const selectedPreset = validPresets.find((p) => p.id === selectedPresetId)

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
      p.label.toLowerCase().includes(filter.search.toLowerCase()),
  )

  useEffect(() => {
    if (!refActiveElement.current) return
    scrollActiveProjectIntoView()
  }, [refActiveElement, scrollActiveProjectIntoView])

  const categories = [...new Set(filteredPresets.map((p) => p.system))].sort()
  const aircraft = [...new Set(filteredPresets.map((p) => p.aircraft))].sort()
  const vendors = [...new Set(filteredPresets.map((p) => p.vendor))].sort()

  return (
    <div className="flex flex-col gap-4 rounded-lg border p-4 px-6 pb-2 shadow">
      <div className="flex flex-row items-end justify-between">
        <div className="flex flex-col">
          <div className="text-lg font-semibold">Step 2 - Select preset</div>
          <div className="text-muted-foreground text-sm">
            Choose a preset from our preset library. Use filters to narrow down
            your search.
          </div>
        </div>
        <div className="flex flex-row items-center gap-2">
          <Switch
            checked={favoritesOnly}
            onCheckedChange={(checked) => setFavoritesOnly(checked)}
          ></Switch>
          <Label>Project aircraft only</Label>
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
      </div>
      <div className="flex flex-col gap-2">
        <ScrollArea
          className="h-56"
          onMouseEnter={cancelScrollIntoView}
          onMouseLeave={scrollActiveProjectIntoView}
        >
          <div className="flex flex-col gap-1" role="list">
            {filteredPresets.map((preset) => (
              <MsfsPresetItem
                ref={preset.id === selectedPresetId ? refActiveElement : null}
                key={preset.id}
                preset={preset}
                isSelected={preset.id === selectedPresetId}
                setSelectedPreset={setSelectedPreset}
              />
            ))}
          </div>
        </ScrollArea>
        <div className="flex flex-row items-center justify-between">
        <div role="status" className="px-2 text-sm">
          {t("Dialog.InputConfigWizard.InputActions.Common.PresetsFound", {
            count: filteredPresets.length,
          })}
        </div>
        <Button
          size={"sm"}
          className="w-fit px-2 py-1"
          variant="ghost"
          onClick={() =>
            setFilter({ vendor: "", aircraft: "", system: "", search: "" })
          }
        >
          <IconX />
          <span className="text-sm">{t("Dialog.General.ResetFilters")}</span>
        </Button>
      </div>
      </div>
    </div>
  )
}
export default MsfsPresetPanel
