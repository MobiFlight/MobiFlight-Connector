import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useQuery } from "@tanstack/react-query"
import { useState } from "react"

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
      <div className="grid grid-cols-4 gap-4">
        <Input
          placeholder="Filter presets"
          value={filter.search}
          onChange={(e) =>
            setFilter((prev) => ({ ...prev, search: e.target.value }))
          }
        />
        <ComboBox
          items={vendors}
          selected={filter.vendor}
          placeholder="Filter by vendor"
          getLabel={(item) => item}
          getValue={(item) => item}
          isSelected={(item) => item === filter.vendor}
          setSelected={(item) =>
            setFilter((prev) => ({ ...prev, vendor: item || "" }))
          }
          searchPlaceholder="Search vendors..."
        />
        <ComboBox
          items={aircraft}
          selected={filter.aircraft}
          placeholder="Filter by aircraft"
          getLabel={(item) => item}
          getValue={(item) => item}
          isSelected={(item) => item === filter.aircraft}
          setSelected={(item) =>
            setFilter((prev) => ({ ...prev, aircraft: item || "" }))
          }
          searchPlaceholder="Search aircraft..."
        />
        <ComboBox
          items={categories}
          selected={filter.system}
          placeholder="Filter by system"
          getLabel={(item) => item}
          getValue={(item) => item}
          isSelected={(item) => item === filter.system}
          setSelected={(item) =>
            setFilter((prev) => ({ ...prev, system: item || "" }))
          }
          searchPlaceholder="Search systems..."
        />
      </div>
      <div className="flex flex-row items-center gap-4">
        <ComboBox
          items={filteredPresets}
          selected={selectedPreset}
          placeholder="Select preset"
          getLabel={(item) => item.label}
          getValue={(item) => item.id}
          isSelected={(item) => item.id === selectedPreset?.id}
          setSelected={(item) => {
            if (item) onPresetSelect(item)
          }}
          searchPlaceholder="Search presets..."
          widthClass="w-150"
        />
        <div role="status" className="text-sm">{`${filteredPresets.length} preset(s) found`}</div>
      </div>
      <div className="flex flex-col gap-2">
        <Label htmlFor="description">Description:</Label>
        <div id="description" className="rounded border p-2 text-sm">
          {selectedPreset?.description
            ? selectedPreset?.description
            : "No description available"}
        </div>
      </div>
    </div>
  )
}

export default XplanePresetPanel
