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
import { IconArrowBack, IconEdit } from "@tabler/icons-react"
import { useQuery } from "@tanstack/react-query"
import { RefObject, useState } from "react"
import { useTranslation } from "react-i18next"
import { Separator } from "@/components/ui/separator"
import { useLocation, useNavigate, useSearchParams } from "react-router"
import { ScrollArea } from "@/components/ui/scroll-area"

type AircraftItemProps = {
  aircraft: AircraftInfoWithStats
  checked: boolean
  onChecked: (aircraft: AircraftInfo) => void
} & React.HTMLAttributes<HTMLDivElement>

const AircraftItem = ({
  aircraft,
  checked,
  onChecked,
  ...props
}: AircraftItemProps) => {
  return (
    <div
      className={`hover:bg-accent flex cursor-pointer flex-row items-center gap-4 rounded-md border px-4 py-1 ${checked && "border-primary"}`}
      onClick={() => onChecked(aircraft)}
      {...props}
    >
      <Checkbox checked={checked}></Checkbox>
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
      <DrawerContent
        data-testid="project-aircraft-drawer"
        className="pb-8 data-[vaul-drawer-direction=right]:w-5/6 data-[vaul-drawer-direction=right]:sm:max-w-5/6"
      >
        <DrawerHeader className="p-2">
          <DrawerTitle className="sr-only">
            {t("Project.Form.Aircraft.Dialog.Title")}
          </DrawerTitle>
          <DrawerClose className="text-primary flex flex-row items-center gap-2 underline-offset-4 hover:underline">
            <IconArrowBack size={16} />
            {t("Dialog.General.GoBack")}
          </DrawerClose>
        </DrawerHeader>
        <div className="flex grow flex-col gap-2 px-4">
          <div className="text-md text-muted-foreground">
            {t("Project.Form.Aircraft.Dialog.Description")}
          </div>
          <div className="flex flex-row items-end justify-between">
            <div className="text-md font-bold">
              {t("Project.Form.Aircraft.Dialog.SelectedAircraft.Title")}
            </div>
            <div className="flex flex-row items-center gap-2">
              <div className="text-muted-foreground pr-4 text-sm">
                {t("Project.Form.Aircraft.Dialog.SelectedAircraft.Count", {
                  count: selectedAircraftWithStats.length,
                })}
              </div>
            </div>
          </div>
          <div
            className={cn(
              `text-md relative flex max-h-13 grow flex-col gap-1`,
              selectedAircraftWithStats.length === 2 && "max-h-28",
              selectedAircraftWithStats.length === 3 && "max-h-43",
              selectedAircraftWithStats.length === 4 && "max-h-58",
              selectedAircraftWithStats.length >= 5 && "max-h-60",
            )}
          >
            {selectedAircraftWithStats.length === 0 ? (
              <div className="text-muted-foreground p-3 text-sm">
                {t("Project.Form.Aircraft.Dialog.SelectedAircraft.None")}
              </div>
            ) : (
              <ScrollArea className="grow">
                <div
                  className="flex flex-col gap-2 pb-1"
                  role="listbox"
                  aria-label={t(
                    "Project.Form.Aircraft.Dialog.SelectedAircraft.Title",
                  )}
                >
                  {selectedAircraftWithStats.map((ac) => (
                    <AircraftItem
                      role="option"
                      key={`selected-${ac.Vendor}-${ac.Name}`}
                      aircraft={ac}
                      checked={true}
                      onChecked={removeAircraft}
                    />
                  ))}
                </div>
              </ScrollArea>
            )}
          </div>
          <Separator className="mt-4 mb-2" />
          <div className="flex flex-col gap-2">
            <div className="flex flex-row items-end justify-between">
              <div className="text-md font-bold">
                {t("Project.Form.Aircraft.Dialog.AvailableAircraft.Title")}
              </div>
              <div className="flex flex-row items-center gap-2">
                <div className="text-muted-foreground pr-4 text-sm">
                  {t("Project.Form.Aircraft.Dialog.AvailableAircraft.Count", {
                    count: availableAircraft.length,
                  })}
                </div>
              </div>
            </div>
            <Input
              id="filter"
              placeholder={t("Project.Form.Aircraft.Dialog.SearchPlaceholder")}
              className="mb-2"
              value={filter}
              onChange={(e) => setFilter(e.target.value)}
            />
          </div>
          <div className="text-md flex grow">
            <ScrollArea className="grow">
              {availableAircraft.length === 0 ? (
                <div className="text-muted-foreground p-3 text-sm">
                  {t("Project.Form.Aircraft.Dialog.AvailableAircraft.None")}
                </div>
              ) : (
                <div
                  className="flex flex-col gap-2 pb-1"
                  role="listbox"
                  aria-label={t(
                    "Project.Form.Aircraft.Dialog.AvailableAircraft.Title",
                  )}
                >
                  {availableAircraft.map((ac) => (
                    <AircraftItem
                      role="option"
                      key={`available-${ac.Vendor}-${ac.Name}`}
                      aircraft={ac}
                      checked={ac.selected}
                      onChecked={addAircraft}
                    />
                  ))}
                </div>
              )}
            </ScrollArea>
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
  const { t } = useTranslation()
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
        <Label className="text-base font-semibold">
          {t("Project.Form.Aircraft.Label")}
        </Label>
        <Badge variant={"default"}>{t("General.NewFeature")}</Badge>
      </div>
      <div className="text-muted-foreground text-sm">
        {t("Project.Form.Aircraft.Description")}
      </div>
      {["msfs", "xplane"].includes(variant) ? (
        <>
          <div className="flex flex-row items-center gap-4">
            {selectedAircraft.length === 0 ? (
              <Badge
                className="text-muted-foreground text-sm"
                variant={"secondary"}
              >
                {t("Project.Form.Aircraft.Dialog.SelectedAircraft.None")}
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
              <div className="sr-only">
                {t("Project.Form.Aircraft.EditList")}
              </div>
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
          {t("Project.Form.Aircraft.NotSupported")}
        </div>
      )}
    </div>
  )
}
export default ProjectAircraft
