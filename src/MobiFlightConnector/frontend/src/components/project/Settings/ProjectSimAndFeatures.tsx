import { Checkbox } from "@/components/ui/checkbox"
import { Label } from "@/components/ui/label"
import { cn } from "@/lib/utils"
import { ProjectFeatures, ProjectInfo } from "@/types/project"
import { useTranslation } from "react-i18next"
type ProjectSimAndFeaturesProps = {
  simSettings: Partial<ProjectInfo>
  onChange: (values: Partial<ProjectInfo>) => void
}
const ProjectSimAndFeatures = ({
  simSettings,
  onChange,
}: ProjectSimAndFeaturesProps) => {
  const { t } = useTranslation()
  const useFsuipc = simSettings.Features?.FSUIPC ?? false
  const useProsim = simSettings.Features?.ProSim ?? false
  return (
    <>
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
            aria-checked={simSettings.Sim === sim}
            key={sim}
            className={cn(
              "inline-block h-48 flex-1 cursor-pointer rounded-lg transition-all duration-200 hover:scale-110",
              simSettings.Sim === sim
                ? "drop-shadow-primary/50 ring-primary ring-3 drop-shadow-lg"
                : "opacity-50 hover:opacity-100",
            )}
            onClick={() => {
              onChange({
                Sim: sim,
                Features: {
                  FSUIPC: false,
                  ProSim: false,
                },
              } as Partial<ProjectInfo>)
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
      <div className="flex flex-col">{t(`Project.Simulator.${simSettings.Sim}`)}</div>
      <div className="flex flex-col gap-2">
        {(simSettings.Sim === "msfs" || simSettings.Sim === "p3d") && (
          <div className="flex flex-col gap-2">
            <p className="text-muted-foreground text-sm">
              {t("Project.Form.Simulator.Feature")}
            </p>
            {/* FSUIPC Option (only for MSFS) */}
            {simSettings.Sim === "msfs" && (
              <div className="flex items-center space-x-2 pl-2">
                <Checkbox
                  id="fsuipc"
                  checked={useFsuipc}
                  onCheckedChange={(checked) => onChange({
                    ...simSettings,
                    Features: {
                      ...simSettings.Features,
                      FSUIPC: Boolean(checked),
                    } as ProjectFeatures,
                  })}
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
                onCheckedChange={(checked) => onChange({
                  ...simSettings,
                  Features: {
                    ...simSettings.Features,
                    ProSim: Boolean(checked),
                  } as ProjectFeatures,
                })}
              />
              <Label htmlFor="prosim" className="font-normal">
                {t("Project.Form.Simulator.UseProSim")}
              </Label>
            </div>
          </div>
        )}
      </div>
    </>
  )
}
export default ProjectSimAndFeatures