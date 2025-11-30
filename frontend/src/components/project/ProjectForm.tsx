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
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import { Checkbox } from "@/components/ui/checkbox"
import { useState } from "react"
import { ProjectInfo } from "@/types/project"
import { useLocation } from "react-router"
import { useTranslation } from "react-i18next"

type ProjectFormProps = {
  project: ProjectInfo
  isOpen: boolean
  onOpenChange: (open: boolean) => void
  onSave: (values: { Name: string; Sim: string; UseFsuipc: boolean }) => void
}

const ProjectForm = ({
  project,
  isOpen,
  onOpenChange,
  onSave,
}: ProjectFormProps) => {
  const [name, setName] = useState(project?.Name ?? "")
  const [simulator, setSimulator] = useState<string>(project?.Sim ?? "msfs")
  const [useFsuipc, setUseFsuipc] = useState(project?.UseFsuipc ?? false)
  const [hasError, setHasError] = useState(false)

  const location = useLocation()
  const isEdit = location.state?.mode === "edit"

  const { t } = useTranslation()

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (name.trim() === "") {
      setHasError(true)
      return
    }
    setHasError(false)
    console.log("Saving")
    onSave({
      Name: name,
      Sim: simulator,
      UseFsuipc: useFsuipc,
    })
  }

  const showErrorMessage = hasError && name.length === 0

  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] overflow-y-auto sm:max-w-[600px]">
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
              <p className="text-sm text-red-500" data-testid="form-project-name-error">
                {t("Project.Form.Name.Error.Required")}
              </p>
            )}{" "}
            {/* Show error */}
          </div>

          {/* Flight Simulator Selection */}
          <div className="grid gap-3">
            <Label className="text-base font-semibold">
              {t("Project.Form.Simulator.Label")}
            </Label>
            <p className="text-muted-foreground text-sm">
              {t("Project.Form.Simulator.HelpText")}
            </p>
            <RadioGroup
              value={simulator}
              onValueChange={(value) => {
                setSimulator(value)
                setUseFsuipc(false) // Reset FSUIPC when changing simulator
              }}
            >
              <div className="flex items-center space-x-2">
                <RadioGroupItem value="msfs" id="msfs" />
                <Label htmlFor="msfs" className="font-normal">
                  {t("Project.Simulator.msfs")}
                </Label>
              </div>
              {/* FSUIPC Option (only for MSFS) */}
              {simulator === "msfs" && (
                <div className="flex items-center space-x-2 pl-6">
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
              <div className="flex items-center space-x-2">
                <RadioGroupItem value="xplane" id="xplane" />
                <Label htmlFor="xplane" className="font-normal">
                  {t("Project.Simulator.xplane")}
                </Label>
              </div>
              <div className="flex items-center space-x-2">
                <RadioGroupItem value="p3d" id="p3d" />
                <Label htmlFor="p3d" className="font-normal">
                  {t("Project.Simulator.p3d")}
                </Label>
              </div>
              <div className="flex items-center space-x-2">
                <RadioGroupItem value="fsx" id="fsx" />
                <Label htmlFor="fsx" className="font-normal">
                  {t("Project.Simulator.fsx")}
                </Label>
              </div>
            </RadioGroup>
          </div>
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline" type="button">
              { t("Project.Form.Buttons.Cancel") }
            </Button>
          </DialogClose>
          <Button onClick={handleSubmit}>
            {isEdit ? t("Project.Form.Buttons.Update") : t("Project.Form.Buttons.Create")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export default ProjectForm
