import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import {
  Drawer,
  DrawerClose,
  DrawerContent,
  DrawerHeader,
  DrawerTitle,
} from "@/components/ui/drawer"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Preset } from "@/components/wizard/components/InputActions/MsfsPresetPanel"
import { fetchHubHopPresets } from "@/lib/configWizard"
import { AircraftInfo } from "@/types/project"
import { IconArrowBack, IconEdit } from "@tabler/icons-react"
import { useQuery } from "@tanstack/react-query"
import { RefObject, useState } from "react"
import { useTranslation } from "react-i18next"
import { useLocation, useNavigate, useSearchParams } from "react-router"

const AircraftItem = ({
  aircraft,
  checked,
  onChecked,
}: {
  aircraft: AircraftInfo
  checked: boolean
  onChecked: (aircraft: AircraftInfo) => void
}) => {
  return (
    <div className="flex flex-row items-center gap-4 rounded-md border p-2">
      <Checkbox
        checked={checked}
        onCheckedChange={() => onChecked(aircraft)}
      ></Checkbox>
      <span className="font-medium w-56">{aircraft.Vendor ?? "Unknown Vendor"}</span>
      <span className="font-medium">{aircraft.Name ?? "Unknown Aircraft"}</span>
    </div>
  )
}

export interface ProjectAircraftProps {
  selectedAircraft: AircraftInfo[]
  setSelectedAircraft: (aircraft: AircraftInfo[]) => void
  variant?: "summary" | "form"
  drawerContainer?: RefObject<HTMLDivElement | null>
}

interface ProjectAircraftDrawerProps {
  selectedAircraft: AircraftInfo[]
  setSelectedAircraft: (aircraft: AircraftInfo[]) => void
  drawerContainer?: RefObject<HTMLDivElement | null>
  drawerOpen: boolean
  setDrawerOpen: (open: boolean) => void
}
const ProjectAircraftDrawer = ({
  selectedAircraft,
  setSelectedAircraft,
  drawerContainer,
  drawerOpen,
  setDrawerOpen,
}: ProjectAircraftDrawerProps) => {
  const { t } = useTranslation()

  const [filter, setFilter] = useState("")

  const { data: presets = [] /*, isLoading */ } = useQuery({
    queryKey: ["msfs-presets"],
    queryFn: () => fetchHubHopPresets("msfs"),
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })

  const availableAircraft = [
    ...new Set(presets.map((p: Preset) => `${p.vendor}###${p.aircraft}`)),
  ]
    .map((uniqueAircraft) => {
      const [Vendor, Name] = uniqueAircraft.split("###")
      return { Vendor, Name }
    })
    .map((ac) => ({
      selected: selectedAircraft.some(
        (a) => a.Name === ac.Name && a.Vendor === ac.Vendor,
      ),
      ...ac,
    }))
    .filter(
      (ac) =>
        !ac.selected &&
        (ac.Name?.toLowerCase().includes(filter.toLowerCase()) ||
        ac.Vendor?.toLowerCase().includes(filter.toLowerCase())),
    )
    .sort((a, b) => a.Name?.localeCompare(b.Name || "") || 0)
    .sort((a, b) => a.Vendor?.localeCompare(b.Vendor || "") || 0)

  const addAircraft = (aircraft: AircraftInfo) => {
    console.log("Adding aircraft", aircraft)
    const prev = selectedAircraft
    prev.push(aircraft)
    setSelectedAircraft([...prev])
  }

  const removeAircraft = (aircraft: AircraftInfo) => {
    const prev = selectedAircraft.filter(
      (a) => a.Name !== aircraft.Name || a.Vendor !== aircraft.Vendor,
    )
    setSelectedAircraft([...prev])
  }

  return (
    <Drawer
      container={drawerContainer?.current || undefined}
      direction="right"
      open={drawerOpen}
      onClose={() => setDrawerOpen(false)}
    >
      <DrawerContent className="data-[vaul-drawer-direction=right]:w-5/6 data-[vaul-drawer-direction=right]:sm:max-w-5/6 pb-8">
        <DrawerHeader>
          <DrawerTitle className="sr-only">
            {t("Dialog.InputConfigWizard.DrawerTitle")}
          </DrawerTitle>
          <DrawerClose className="flex flex-row">
            <Button variant="link">
              <IconArrowBack />
              {t("Dialog.InputConfigWizard.GoBack")}
            </Button>
          </DrawerClose>
        </DrawerHeader>
        <div className="flex flex-col gap-2 overflow-y-auto px-4">
          <div className="text-md text-muted-foreground">
            Define one more aircraft that should be enabled for this project.
          </div>
          <div className="text-lg font-bold">Selected Aircraft</div>
          {selectedAircraft.length === 0 ? (
            <div className="text-muted-foreground text-sm">
              No aircraft defined.
            </div>
          ) : (
            <div className="flex flex-col gap-2">
              {selectedAircraft.map((ac, index) => (
                <AircraftItem
                  key={`${index}`}
                  aircraft={ac}
                  checked={true}
                  onChecked={removeAircraft}
                />
              ))}
            </div>
          )}
          <div className="text-md">Available Aircraft</div>
          <Input
            placeholder="Search..."
            className="mb-2"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
          />
          <div className="text-md overflow-y-auto flex flex-col gap-1">
            {availableAircraft.length === 0 ? (
              <div className="text-muted-foreground text-sm">
                No aircraft matches current filter.
              </div>
            ) : (
              availableAircraft.map((ac) => (
                <AircraftItem
                  key={`${ac.Vendor}-${ac.Name}`}
                  aircraft={ac}
                  checked={ac.selected}
                  onChecked={addAircraft}
                />
              ))
            )}
          </div>
        </div>
      </DrawerContent>
    </Drawer>
  )
}

const ProjectAircraft = ({
  selectedAircraft,
  setSelectedAircraft,
  drawerContainer,
}: ProjectAircraftProps) => {
  const navigate = useNavigate()
  const location = useLocation()
  const state = location.state as { backgroundLocation?: Location }

  const [searchParams] = useSearchParams()
  const [drawerOpen, setDrawerOpen] = useState(false)

  const detailView = searchParams.get("detail")
  const navigateToDetailView = (view: string) => {
    setDrawerOpen(true)
    navigate(`?detail=${view}`, { state })
  }

  const closeDetailView = (open: boolean) => {
    if (open) return
    setDrawerOpen(false)
    setTimeout(() => navigate(-1), 500)
  }

  return (
    <>
      <div className="flex flex-col gap-2">
        <div className="flex flex-row items-center gap-2">
          <Label className="text-base font-semibold">Aircraft</Label>
          <Badge variant={"default"}>New</Badge>
        </div>
        <div className="text-muted-foreground text-sm">
          Define one or more aircraft that are enabled for this project. Options
          will be filtered based on the defined aircraft.
        </div>
        <div className="flex flex-row items-center gap-4">
          {selectedAircraft.length === 0 ? (
            <div className="text-muted-foreground text-sm">
              No aircraft defined.
            </div>
          ) : (
            <div className="flex flex-row gap-2">
              {selectedAircraft.map((ac, index) => (
                <Badge key={`${index}`} className="font-medium">
                  {ac.Vendor ?? "Unknown Vendor"} - {ac.Name ?? "Unknown Aircraft"}
                </Badge>
              ))}
            </div>
          )}
          <Button
            variant="outline"
            size="sm"
            className="h-8 w-8"
            onClick={() => navigateToDetailView("aircraft")}
          >
            <IconEdit />
          </Button>
        </div>
      </div>
      {detailView && (
        <ProjectAircraftDrawer
          selectedAircraft={selectedAircraft}
          setSelectedAircraft={setSelectedAircraft}
          drawerOpen={drawerOpen}
          setDrawerOpen={closeDetailView}
          drawerContainer={drawerContainer}
        />
      )}
    </>
  )
}
export default ProjectAircraft
