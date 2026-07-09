import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { useProjectStore } from "@/stores/projectStore"
import { AircraftInfo } from "@/types/project"
import { IconX } from "@tabler/icons-react"
import { useQuery } from "@tanstack/react-query"
import { useState } from "react"
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

  const validPresets = presets.filter((p: Preset) =>
    validPresetTypes.includes(p.presetType.toLowerCase()) && favoritesOnly
      ? projectAircraftFilter(p)
      : true,
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

  const categories = [...new Set(filteredPresets.map((p) => p.system))].sort()
  const aircraft = [...new Set(filteredPresets.map((p) => p.aircraft))].sort()
  const vendors = [...new Set(filteredPresets.map((p) => p.vendor))].sort()

  return (
    <div className="flex flex-col gap-4 rounded-lg border p-4 px-6 shadow pb-2">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">
          Step 2 - Select preset
        </div>
        <div className="text-muted-foreground text-sm">
          Choose a preset from our preset library. Use filters to narrow down
          your search.
        </div>
      </div>

      <div className="grid grid-cols-4 gap-2">
        <div className="col-span-3">
          <Input
            placeholder={t(
              "Dialog.InputConfigWizard.InputActions.Common.FilterPresets",
            )}
            value={filter.search}
            onChange={(e) =>
              setFilter((prev) => ({ ...prev, search: e.target.value }))
            }
          />
        </div>
        <div className="flex flex-row items-center gap-2">
          <Switch
            checked={favoritesOnly}
            onCheckedChange={(checked) => setFavoritesOnly(checked)}
          ></Switch>
          <Label>Project aircraft only</Label>
        </div>
      </div>
      <div className="grid grid-cols-4 gap-2">
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
        <Button
          size={"sm"}
          className="w-fit"
          variant="ghost"
          onClick={() =>
            setFilter({ vendor: "", aircraft: "", system: "", search: "" })
          }
        >
          <IconX />
          <span className="text-sm">{t("Dialog.General.ResetFilters")}</span>
        </Button>
      </div>
      <div className="bg-accent/50 -mx-4 flex flex-col gap-4 rounded-lg p-4 shadow-inner ">
        <div className="grid grid-cols-4 items-center gap-2">
          <div className="col-span-3">
            <ComboBox
              align="start"
              items={filteredPresets}
              selected={selectedPreset}
              placeholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.SelectPreset",
              )}
              getLabel={(item) => item.label}
              getValue={(item) => item.id}
              isSelected={(item) => item.id === selectedPreset?.id}
              setSelected={(item) => {
                setSelectedPreset(item ? item : null)
              }}
              searchPlaceholder={t(
                "Dialog.InputConfigWizard.InputActions.Common.SearchPresets",
              )}
              widthClass="w-full"
            />
          </div>
          <div role="status" className="px-2 text-sm">
            {t("Dialog.InputConfigWizard.InputActions.Common.PresetsFound", {
              count: filteredPresets.length,
            })}
          </div>
        </div>
        <div className="flex flex-col gap-1">
          <Label htmlFor="description">
            {t("Dialog.InputConfigWizard.InputActions.Common.DescriptionLabel")}
          </Label>
          <div
            id="description"
            className="rounded border-2 border-dashed p-2 text-sm bg-background"
          >
            {selectedPreset?.description
              ? selectedPreset?.description
              : t(
                  "Dialog.InputConfigWizard.InputActions.Common.NoDescriptionAvailable",
                )}
          </div>
        </div>
      </div>
    </div>
  )
}
export default MsfsPresetPanel
