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
import { fetchHubHopPresets } from "@/lib/configWizard"
import { cn } from "@/lib/utils"
import { AircraftInfo, SimulatorType } from "@/types/project"
import { IconArrowBack, IconEdit, IconX } from "@tabler/icons-react"
import { useQuery } from "@tanstack/react-query"
import { RefObject, useState } from "react"
import { useTranslation } from "react-i18next"
import { Separator } from "@/components/ui/separator"
import { useLocation, useNavigate, useSearchParams } from "react-router"

const AircraftItem = ({
  aircraft,
  checked,
  onChecked,
}: {
  aircraft: AircraftInfoWithStats
  checked: boolean
  onChecked: (aircraft: AircraftInfo) => void
}) => {
  return (
    <div
      className={`hover:bg-accent flex cursor-pointer flex-row items-center gap-4 rounded-md border px-4 py-1 ${checked && "border-primary"}`}
      onClick={() => onChecked(aircraft)}
    >
      <Checkbox
        checked={checked}
        onCheckedChange={() => onChecked(aircraft)}
      ></Checkbox>
      <div className="flex grow flex-col font-medium">
        <div className="grow font-medium">
          {aircraft.Name ?? "Unknown Aircraft"}
        </div>
        <div className="text-muted-foreground text-xs">
          {aircraft.Vendor ?? "Unknown Vendor"}
        </div>
      </div>
      {aircraft.Count > 0 && (
        <div className="w-32 text-right text-sm font-medium">
          {aircraft.Count} Presets
        </div>
      )}
    </div>
  )
}

type AircraftStats = {
  Count: number
  Input: boolean
  Output: boolean
  Potentiometer: boolean
}

type AircraftInfoWithStats = AircraftInfo & AircraftStats

export interface ProjectAircraftProps {
  selectedAircraft: AircraftInfo[]
  setSelectedAircraft: (aircraft: AircraftInfo[]) => void
  variant: SimulatorType
  drawerContainer?: RefObject<HTMLDivElement | null>
}

interface ProjectAircraftDrawerProps {
  variant: "msfs" | "xplane"
  selectedAircraft: AircraftInfo[]
  setSelectedAircraft: (aircraft: AircraftInfo[]) => void
  drawerContainer?: RefObject<HTMLDivElement | null>
  drawerOpen: boolean
  setDrawerOpen: (open: boolean) => void
}
const ProjectAircraftDrawer = ({
  variant,
  selectedAircraft,
  setSelectedAircraft,
  drawerContainer,
  drawerOpen,
  setDrawerOpen,
}: ProjectAircraftDrawerProps) => {
  const { t } = useTranslation()

  const [filter, setFilter] = useState("")

  const { data: presets = [] /*, isLoading */ } = useQuery({
    queryKey: [`${variant}-presets`],
    queryFn: () => fetchHubHopPresets(variant),
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })

  const aircarftStatsMap = new Map<string, AircraftInfoWithStats>()

  presets.forEach((p) => {
    const key = `${p.vendor}###${p.aircraft}`
    const existing = aircarftStatsMap.get(key)

    if (existing) {
      existing.Count += 1
      existing.Input = existing.Input || p.presetType === "input"
      existing.Output = existing.Output || p.presetType === "output"
      existing.Potentiometer =
        existing.Potentiometer || p.presetType === "potentiometer"
      return
    }
    aircarftStatsMap.set(key, {
      Vendor: p.vendor,
      Name: p.aircraft,
      Count: 1,
      Input: p.presetType === "input",
      Output: p.presetType === "output",
      Potentiometer: p.presetType === "potentiometer",
    })
  })

  const allAircraftWithStats = [...aircarftStatsMap.values()]
  const availableAircraft = allAircraftWithStats
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

  const selectedAircraftWithStats = selectedAircraft.map((ac) => {
    return (
      allAircraftWithStats.find(
        (a) => a.Name === ac.Name && a.Vendor === ac.Vendor,
      ) ?? {
        ...ac,
        Count: 0,
        Input: false,
        Output: false,
        Potentiometer: false,
      }
    )
  })

  return (
    <Drawer
      container={drawerContainer?.current || undefined}
      direction="right"
      open={drawerOpen}
      onClose={() => setDrawerOpen(false)}
    >
      <DrawerContent className="pb-8 data-[vaul-drawer-direction=right]:w-5/6 data-[vaul-drawer-direction=right]:sm:max-w-5/6">
        <DrawerHeader className="p-2">
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
          <div className="flex flex-row items-end justify-between">
            <div className="text-md font-bold">Selected Aircraft</div>
            <div className="flex flex-row items-center gap-2">
              <div className="text-muted-foreground pr-4 text-sm">
                {selectedAircraftWithStats.length} selected
              </div>
            </div>
          </div>
          <div
            className={cn(
              `text-md flex flex-col gap-1`,
              selectedAircraftWithStats.length >= 4 &&
                "min-h-50 overflow-y-auto shadow-inner",
            )}
          >
            {selectedAircraftWithStats.length === 0 ? (
              <div className="text-muted-foreground text-sm">
                No aircraft defined.
              </div>
            ) : (
              <div className="flex flex-col gap-2">
                {selectedAircraftWithStats.map((ac, index) => (
                  <AircraftItem
                    key={`${index}`}
                    aircraft={ac}
                    checked={true}
                    onChecked={removeAircraft}
                  />
                ))}
              </div>
            )}
          </div>
          <Separator className="my-4"/>
          <div className="flex flex-col gap-2 pr-3">
            <div className="flex flex-row items-end justify-between">
              <div className="text-md font-bold">Available Aircraft</div>
              <div className="flex flex-row items-center gap-2">
                <div className="text-muted-foreground pr-4 text-sm">
                  {availableAircraft.length} available
                </div>
              </div>
            </div>
            <Input
              id="filter"
              placeholder="Search..."
              className="mb-2"
              value={filter}
              onChange={(e) => setFilter(e.target.value)}
            />
          </div>
          <div className="text-md flex min-h-32 flex-col gap-1 overflow-y-auto shadow-inner">
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
  variant,
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
    <div className="flex flex-col gap-2">
      <div className="flex flex-row items-center gap-2">
        <Label className="text-base font-semibold">Aircraft</Label>
        <Badge variant={"default"}>New</Badge>
      </div>
      <div className="text-muted-foreground text-sm">
        Define one or more aircraft that are enabled for this project. Options
        will be filtered based on the defined aircraft.
      </div>
      {["msfs", "xplane"].includes(variant) ? (
        <>
          <div className="flex flex-row items-center gap-4">
            {selectedAircraft.length === 0 ? (
              <Badge
                className="text-muted-foreground text-sm"
                variant={"secondary"}
              >
                No aircraft defined.
              </Badge>
            ) : (
              <div className="flex flex-row flex-wrap gap-2">
                {selectedAircraft.map((ac, index) => (
                  <Badge key={`${index}`} className="font-medium">
                    {ac.Vendor ?? "Unknown Vendor"} -{" "}
                    {ac.Name ?? "Unknown Aircraft"}
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
          {detailView && (
            <ProjectAircraftDrawer
              variant={variant as "msfs" | "xplane"}
              selectedAircraft={selectedAircraft}
              setSelectedAircraft={setSelectedAircraft}
              drawerOpen={drawerOpen}
              setDrawerOpen={closeDetailView}
              drawerContainer={drawerContainer}
            />
          )}
        </>
      ) : (
        <div className="bg-accent/20 h-8 rounded-md border p-1 text-center text-sm">
          Aircraft selection is not supported for this sim.
        </div>
      )}
    </div>
  )
}
export default ProjectAircraft
