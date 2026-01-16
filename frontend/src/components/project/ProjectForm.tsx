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
import { Checkbox } from "@/components/ui/checkbox"
import { useState } from "react"
import { ProjectFeatures, ProjectInfo } from "@/types/project"
import { useLocation } from "react-router"
import { useTranslation } from "react-i18next"
import { cn } from "@/lib/utils"

type ProjectFormProps = {
  project: ProjectInfo
  isOpen: boolean
  onOpenChange: (open: boolean) => void
  onSave: (values: {
    Name: string
    Sim: string
    Features: ProjectFeatures
  }) => void
}

const ProjectForm = ({
  project,
  isOpen,
  onOpenChange,
  onSave,
}: ProjectFormProps) => {
  const [name, setName] = useState(project?.Name ?? "")
  const [simulator, setSimulator] = useState<string>(project?.Sim ?? "msfs")
  const [useFsuipc, setUseFsuipc] = useState(project?.Features?.FSUIPC ?? false)
  const [useProsim, setUseProsim] = useState(project?.Features?.ProSim ?? false)
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
        className="max-h-[90vh] overflow-y-auto sm:max-w-[600px]"
        onKeyDown={handleFormKeyDown}
      >
        <DialogHeader>
          <DialogTitle className="text-2xl">
            {isEdit
              ? t("Project.Form.Title.Edit")
              : t("Project.Form.Title.New")}
          </DialogTitle>
          <DialogDescription className="text-md">
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
          <div>
            <Label className="text-base font-semibold">
              {t("Project.Form.Simulator.Label")}
            </Label>
            <p className="text-muted-foreground text-sm">
              {t("Project.Form.Simulator.HelpText")}
            </p>
          </div>
          {/* Flight Simulator Selection */}

          <div className="flex flex-row gap-4">
            {["msfs", "xplane", "p3d", "fsx"].map((sim) => (
              <div
                role="radio"
                aria-checked={simulator === sim}
                key={sim}
                className={cn(
                  "inline-block h-48 flex-1 cursor-pointer rounded-lg transition-all duration-200 hover:scale-110",
                  simulator === sim
                    ? "drop-shadow-primary/50 ring-primary ring-3 drop-shadow-lg"
                    : "opacity-50 hover:opacity-100",
                )}
                onClick={() => {
                  setUseFsuipc(false)
                  setUseProsim(false)
                  setSimulator(sim)
                }}
              >
                <img
                  src={`/sim/${sim.toLowerCase()}.jpg`}
                  alt={sim}
                  className="h-full w-full rounded-lg object-cover"
                />
              </div>
            ))}
          </div>
          {/* Simulator name */}
          <div className="flex flex-col">
            {t(`Project.Simulator.${simulator}`)}
          </div>
          <div className="flex h-24 flex-col gap-2">
            {(simulator === "msfs" || simulator === "p3d") && (
              <div className="flex h-24 flex-col gap-2">
                <p className="text-muted-foreground text-sm">
                  {t("Project.Form.Simulator.Feature")}
                </p>
                {/* FSUIPC Option (only for MSFS) */}
                {simulator === "msfs" && (
                  <div className="flex items-center space-x-2 pl-2">
                    <Checkbox
                      id="fsuipc"
                      checked={useFsuipc}
                      onCheckedChange={(checked) =>
                        setUseFsuipc(Boolean(checked))
                      }
                    />
                    <Label htmlFor="fsuipc" className="font-normal">
                      {t("Project.Form.Simulator.UseFsuipc")}
                    </Label>
                  </div>
                )}

                {/* ProSim Option (MSFS & P3D) */}
                <div className="flex items-center space-x-2 pl-2">
                  <Checkbox
                    id="prosim"
                    checked={useProsim}
                    onCheckedChange={(checked) =>
                      setUseProsim(Boolean(checked))
                    }
                  />
                  <Label htmlFor="prosim" className="font-normal">
                    {t("Project.Form.Simulator.UseProSim")}
                  </Label>
                </div>
              </div>
            )}
          </div>
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline" type="button">
              {t("Project.Dialog.General.Cancel")}
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
