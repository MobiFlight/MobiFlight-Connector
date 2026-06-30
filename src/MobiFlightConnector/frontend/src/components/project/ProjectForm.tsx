import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useRef, useState } from "react"
import {
  AircraftInfo,
  ProjectFeatures,
  ProjectInfo,
  SimulatorType,
} from "@/types/project"
import { useLocation } from "react-router"
import { useTranslation } from "react-i18next"
import ProjectAircraft from "@/components/project/Settings/ProjectAircraft"
import ProjectSimAndFeatures from "@/components/project/Settings/ProjectSimAndFeatures"

type ProjectFormProps = {
  project: ProjectInfo
  isOpen: boolean
  onOpenChange: (open: boolean) => void
  onSave: (values: {
    Name: string
    Sim: SimulatorType
    Features: ProjectFeatures
    Aircraft: AircraftInfo[]
  }) => void
}

const ProjectForm = ({
  project,
  isOpen,
  onOpenChange,
  onSave,
}: ProjectFormProps) => {
  const defaultAircraft = {
    msfs: [
      { Vendor: "Asobo", Name: "Generic" },
      { Vendor: "Microsoft", Name: "Generic" },
    ],
    xplane: [{ Vendor: "Laminar Research", Name: "Generic" }],
    p3d: [],
    fsx: [],
    none: [],
  } as Record<SimulatorType, AircraftInfo[]>

  const [name, setName] = useState(project?.Name ?? "")
  const [simulator, setSimulator] = useState<SimulatorType>(
    project?.Sim ?? "msfs",
  )
  const [useFsuipc, setUseFsuipc] = useState(project?.Features?.FSUIPC ?? false)
  const [useProsim, setUseProsim] = useState(project?.Features?.ProSim ?? false)
  const [aircraft, setAircraft] = useState<AircraftInfo[]>(
    project?.Aircraft ?? defaultAircraft[simulator],
  )

  const [hasError, setHasError] = useState(false)

  const location = useLocation()
  const isEdit = location.state?.mode === "edit"

  const { t } = useTranslation()

  const FsuipcOptionIsDefaultForSimulator = (simulator: string) => {
    return simulator === "fsx" || simulator === "p3d"
  }

  const ProSimFeatureIsSupportedBySimulator = (simulator: string) => {
    return simulator === "msfs" || simulator === "p3d"
  }

  const containerRef = useRef<HTMLDivElement>(null)

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const trimmedName = name.trim()

    if (trimmedName === "") {
      setHasError(true)
      return
    }
    setHasError(false)

    onSave({
      Name: trimmedName,
      Sim: simulator,
      Features: {
        FSUIPC: useFsuipc || FsuipcOptionIsDefaultForSimulator(simulator),
        ProSim: useProsim && ProSimFeatureIsSupportedBySimulator(simulator),
      },
      Aircraft: aircraft,
    })
  }

  const handleFormKeyDown = (e: React.KeyboardEvent) => {
    e.stopPropagation()
    if (e.key === "Enter") {
      handleSubmit(e as unknown as React.FormEvent)
    }
  }

  const showErrorMessage = hasError && name.length === 0

  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange}>
      <DialogContent
        className="max-h-[90vh] overflow-x-hidden overflow-y-auto sm:max-w-[600px]"
        onKeyDown={handleFormKeyDown}
        ref={containerRef}
      >
        <DialogHeader>
          <DialogTitle className="text-2xl">
            {isEdit
              ? t("Project.Form.Title.Edit")
              : t("Project.Form.Title.New")}
          </DialogTitle>
          <DialogDescription>
            {t("Project.Form.Description")}
          </DialogDescription>
        </DialogHeader>

        <div className="grid gap-6">
          {/* Project Name */}
          <div className="grid gap-2">
            <Label htmlFor="project-name" className="text-base font-semibold">
              {t("Project.Form.Name.Label")}
            </Label>
            <Input
              id="project-name"
              name="name"
              value={name}
              className={showErrorMessage ? "border-red-500" : ""}
              onChange={(e) => setName(e.target.value)}
              placeholder={t("Project.Form.Name.Placeholder")}
              aria-invalid={showErrorMessage ? "true" : "false"}
              required
            />
            {showErrorMessage && (
              <p
                className="text-sm text-red-500"
                data-testid="form-project-name-error"
              >
                {t("Project.Form.Name.Error.Required")}
              </p>
            )}{" "}
            {/* Show error */}
          </div>
          <ProjectSimAndFeatures
            simSettings={
              {
                Sim: simulator,
                Features: {
                  FSUIPC: useFsuipc,
                  ProSim: useProsim,
                },
              } as Partial<ProjectInfo>
            }
            onChange={(values) => {
              console.log(values)
              if (values.Sim) {
                setSimulator(values.Sim)
              }
              if (values.Features) {
                setUseFsuipc(values.Features.FSUIPC ?? false)
                setUseProsim(values.Features.ProSim ?? false)
              }
              // reset to default aircraft on switching simulator type
              setAircraft(defaultAircraft[values.Sim ?? "none"])
            }}
          />
          <ProjectAircraft
            variant={simulator}
            selectedAircraft={aircraft}
            setSelectedAircraft={setAircraft}
            drawerContainer={containerRef}
          />
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline" type="button">
              {t("Dialog.General.Cancel")}
            </Button>
          </DialogClose>
          <Button onClick={handleSubmit}>
            {isEdit
              ? t("Project.Form.Buttons.Update")
              : t("Project.Form.Buttons.Create")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export default ProjectForm
