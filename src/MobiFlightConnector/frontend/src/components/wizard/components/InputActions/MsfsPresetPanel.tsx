import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
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
}

export type MsfsPresetPanelProps = {
  selectedPresetId: string | null
  setSelectedPreset: (preset: Preset | null) => void
}

const MsfsPresetPanel = ({
  selectedPresetId,
  setSelectedPreset,
}: MsfsPresetPanelProps) => {
  // In MsfsPresetPanel (or a dedicated hook)
  const { data: presets = [], isLoading } = useQuery({
    queryKey: ["msfs-presets"],
    queryFn: () =>
      fetch("/presets/msfs2020_hubhop_presets.json").then((r) => r.json()) as Promise<Preset[]>,
    staleTime: Infinity, // presets don't change at runtime; HubHopState drives invalidation
  })

  const selectedPreset = presets.find((p) => p.id === selectedPresetId)

  const [filter, setFilter] = useState({
    vendor: selectedPreset?.vendor || "",
    aircraft: selectedPreset?.aircraft || "",
    system: selectedPreset?.system || "",
    search: "",
  })

  const filteredPresets = presets
    .filter((p) => (filter.vendor ? p.vendor === filter.vendor : true))
    .filter((p) => (filter.aircraft ? p.aircraft === filter.aircraft : true))
    .filter((p) => (filter.system ? p.system === filter.system : true))
    .filter((p) => p.label.toLowerCase().includes(filter.search.toLowerCase()))

  const categories = [...new Set(presets.map((p) => p.system))]
  const aircraft = [...new Set(presets.map((p) => p.aircraft))]
  const vendors = [...new Set(presets.map((p) => p.vendor))]

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
        <div>
          <div className="font-md">Selected Preset Details:</div>
          <div>
            Label:{" "}
            {selectedPresetId
              ? presets.find((p) => p.id === selectedPresetId)?.label
              : "None"}
          </div>
          <div>
            Vendor:{" "}
            {selectedPresetId
              ? presets.find((p) => p.id === selectedPresetId)?.vendor
              : "None"}
          </div>
          <div>
            Aircraft:{" "}
            {selectedPresetId
              ? presets.find((p) => p.id === selectedPresetId)?.aircraft
              : "None"}
          </div>
          <div>
            Category:{" "}
            {selectedPresetId
              ? presets.find((p) => p.id === selectedPresetId)?.system
              : "None"}
          </div>
          <div>
            Code:{" "}
            {selectedPresetId
              ? presets.find((p) => p.id === selectedPresetId)?.code
              : "None"}
          </div>
        </div>
      </div>
    </div>
  )
}
export default MsfsPresetPanel
