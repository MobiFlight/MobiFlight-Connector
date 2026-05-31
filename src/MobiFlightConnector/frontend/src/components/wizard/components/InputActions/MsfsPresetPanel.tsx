import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useQuery } from "@tanstack/react-query"
import { useState } from "react"

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
  presetType: "input" | "output" | "potentiometer"
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
  const validPresetTypes =
    variant === "input" ? ["input", "potentiometer"] : ["output"]
  // In MsfsPresetPanel (or a dedicated hook)
  const { data: presets = [] /*, isLoading */ } = useQuery({
    queryKey: ["msfs-presets"],
    queryFn: () =>
      fetch("/presets/msfs2020_hubhop_presets.json")
        .then((r) => r.json())
        .then((presets) =>
          presets.filter((p: Preset) =>
            validPresetTypes.includes(p.presetType.toLowerCase()),
          ),
        ) as Promise<Preset[]>,
    staleTime: Infinity, // presets don't change at runtime; HubHopState drives invalidation
  })

  const selectedPreset = presets.find((p) => p.id === selectedPresetId)

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
          selected={filter?.vendor}
          placeholder="Filter by vendor"
          getLabel={(item) => item}
          getValue={(item) => item}
          items={vendors}
          isSelected={(item) => item === filter?.vendor}
          setSelected={(item) => {
            setFilter((prev) => ({ ...prev, vendor: item || "" }))
          }}
          searchPlaceholder="Search vendors..."
        />
        <ComboBox
          placeholder="Filter by aircraft"
          getLabel={(item) => item}
          getValue={(item) => item}
          items={aircraft}
          selected={filter?.aircraft}
          isSelected={(item) => item === filter?.aircraft}
          setSelected={(item) => {
            setFilter((prev) => ({ ...prev, aircraft: item || "" }))
          }}
          searchPlaceholder="Search aircraft..."
        />
        <ComboBox
          placeholder="Filter by system"
          getLabel={(item) => item}
          getValue={(item) => item}
          items={categories}
          selected={filter?.system}
          isSelected={(item) => item === filter?.system}
          setSelected={(item) => {
            setFilter((prev) => ({ ...prev, system: item || "" }))
          }}
          searchPlaceholder="Search systems..."
        />
      </div>
      <div className="flex flex-row gap-4">
        <ComboBox
          selected={selectedPreset}
          placeholder="Select preset"
          getLabel={(item) => item.label}
          getValue={(item) => item.id}
          items={filteredPresets}
          isSelected={(item) => item.id === selectedPreset?.id}
          setSelected={(item) => {
            setSelectedPreset(item ? item : null)
          }}
          searchPlaceholder="Search presets..."
          widthClass="w-150"
        />
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
export default MsfsPresetPanel
