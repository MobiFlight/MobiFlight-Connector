import ComboBox from "@/components/ComboBox"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
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
  setSelectedPreset: (preset: XplanePreset | null) => void
}

const XplanePresetPanel = ({
  variant,
  selectedPath,
  setSelectedPreset,
}: XplanePresetPanelProps) => {
  const validPresetTypes =
    variant === "input" ? ["input", "potentiometer"] : ["output", "inputoutput"]
  // In XplanePresetPanel (or a dedicated hook)
  const { data: presets = [] /*, isLoading */ } = useQuery({
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
        <div className="text-md font-semibold">Description:</div>
        <div className="rounded border p-2">
          {selectedPath
            ? presets.find((p) => p.code === selectedPath)?.description
            : "None"}
        </div>
      </div>
      <ComboBox
        selected={selectedPreset?.codeType}
        placeholder="Define preset type"
        getLabel={(item) => item}
        getValue={(item) => item}
        items={["DataRef", "Command"]}
        isSelected={(item) => item === selectedPreset?.codeType}
        setSelected={(item) => {
          if (!selectedPreset) return
          setSelectedPreset(
            item
              ? ({ ...selectedPreset, codeType: item } as XplanePreset)
              : null,
          )
        }}
        searchPlaceholder="Preset type..."
        widthClass="w-48"
      />
      <div className="flex flex-col gap-2">
        <div className="text-md font-semibold">Code:</div>
        <Textarea
          value={
            selectedPath
              ? presets.find((p) => p.code === selectedPath)?.code
              : "None"
          }
        />
        <div>Supports input value (@) and placeholders ($, #, etc.)</div>
      </div>
    </div>
  )
}
export default XplanePresetPanel
