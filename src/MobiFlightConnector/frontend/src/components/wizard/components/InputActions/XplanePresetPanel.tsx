import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { IconX } from "@tabler/icons-react"
import { useQuery } from "@tanstack/react-query"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export type XplanePreset = {
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
  presetType: "input" | "output" | "inputoutput" | "potentiometer"
  codeType: "DataRef" | "Command"
}

export type XplanePresetPanelProps = {
  variant: "input" | "output"
  selectedPath: string | null
  onPresetSelect: (preset: XplanePreset) => void
}

const XplanePresetPanel = ({
  variant,
  selectedPath,
  onPresetSelect,
}: XplanePresetPanelProps) => {
  const { t } = useTranslation()
  const validPresetTypes =
    variant === "input"
      ? ["input", "inputoutput", "potentiometer"]
      : ["output", "inputoutput"]

  const { data: presets = [] } = useQuery({
    queryKey: ["xplane-presets"],
    queryFn: () =>
      fetch("/presets/xplane_hubhop_presets.json")
        .then((r) => r.json())
        .then((presets) =>
          presets.filter((p: XplanePreset) =>
            validPresetTypes.includes(p.presetType.toLowerCase()),
          ),
        ) as Promise<XplanePreset[]>,
    staleTime: Infinity,
  })

  const selectedPreset = presets.find((p) => p.code === selectedPath)

  const [filter, setFilter] = useState({
    vendor: selectedPreset?.vendor || "",
    aircraft: selectedPreset?.aircraft || "",
    system: selectedPreset?.system || "",
    search: "",
  })

  const filteredPresets = presets.filter(
    (p) =>
      (filter.vendor ? p.vendor === filter.vendor : true) &&
      (filter.aircraft ? p.aircraft === filter.aircraft : true) &&
      (filter.system ? p.system === filter.system : true) &&
      p.label.toLowerCase().includes(filter.search.toLowerCase()),
  )

  const categories = [...new Set(filteredPresets.map((p) => p.system))]
  const aircraft = [...new Set(filteredPresets.map((p) => p.aircraft))]
  const vendors = [...new Set(filteredPresets.map((p) => p.vendor))]

  return (
    <div className="flex flex-col gap-4">
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
          widthClass="flex-1"
          items={vendors}
          selected={filter.vendor}
          placeholder={t(
            "Dialog.InputConfigWizard.InputActions.Common.FilterByVendor",
          )}
          getLabel={(item) => item}
          getValue={(item) => item}
          isSelected={(item) => item === filter.vendor}
          setSelected={(item) =>
            setFilter((prev) => ({ ...prev, vendor: item || "" }))
          }
          searchPlaceholder={t(
            "Dialog.InputConfigWizard.InputActions.Common.SearchVendors",
          )}
        />
        <ComboBox
          widthClass="flex-1"
          items={aircraft}
          selected={filter.aircraft}
          placeholder={t(
            "Dialog.InputConfigWizard.InputActions.Common.FilterByAircraft",
          )}
          getLabel={(item) => item}
          getValue={(item) => item}
          isSelected={(item) => item === filter.aircraft}
          setSelected={(item) =>
            setFilter((prev) => ({ ...prev, aircraft: item || "" }))
          }
          searchPlaceholder={t(
            "Dialog.InputConfigWizard.InputActions.Common.SearchAircraft",
          )}
        />
        <ComboBox
          widthClass="flex-1"
          items={categories}
          selected={filter.system}
          placeholder={t(
            "Dialog.InputConfigWizard.InputActions.Common.FilterBySystem",
          )}
          getLabel={(item) => item}
          getValue={(item) => item}
          isSelected={(item) => item === filter.system}
          setSelected={(item) =>
            setFilter((prev) => ({ ...prev, system: item || "" }))
          }
          searchPlaceholder={t(
            "Dialog.InputConfigWizard.InputActions.Common.SearchSystems",
          )}
        />
      </div>
      <div className="grid grid-cols-4 items-center gap-2">
        <div className="col-span-2">
          <ComboBox
            items={filteredPresets}
            selected={selectedPreset}
            placeholder={t(
              "Dialog.InputConfigWizard.InputActions.Common.SelectPreset",
            )}
            getLabel={(item) => item.label}
            getValue={(item) => item.id}
            isSelected={(item) => item.id === selectedPreset?.id}
            setSelected={(item) => {
              if (item) onPresetSelect(item)
            }}
            searchPlaceholder={t(
              "Dialog.InputConfigWizard.InputActions.Common.SearchPresets",
            )}
            widthClass="w-full"
            variant="nofilter"
          />
        </div>
        <div role="status" className="px-2 text-sm">
          {t("Dialog.InputConfigWizard.InputActions.Common.PresetsFound", {
            count: filteredPresets.length,
          })}
        </div>
        <div>
          <Button
            size={"sm"}
            variant="ghost"
            onClick={() =>
              setFilter({ vendor: "", aircraft: "", system: "", search: "" })
            }
          >
            <IconX />
            {t("Dialog.General.ResetFilters")}
          </Button>
        </div>
      </div>
      <div className="flex flex-col gap-2">
        <Label htmlFor="description">
          {t("Dialog.InputConfigWizard.InputActions.Common.DescriptionLabel")}
        </Label>
        <div id="description" className="rounded border p-2 text-sm">
          {selectedPreset?.description
            ? selectedPreset?.description
            : t(
                "Dialog.InputConfigWizard.InputActions.Common.NoDescriptionAvailable",
              )}
        </div>
      </div>
    </div>
  )
}

export default XplanePresetPanel
