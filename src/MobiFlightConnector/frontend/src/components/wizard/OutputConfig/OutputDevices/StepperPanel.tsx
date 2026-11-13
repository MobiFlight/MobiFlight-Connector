import DeviceIcon from "@/components/icons/DeviceIcon"
import { StepperConfig } from "@/types/config"
import { DeviceReference, Controller } from "@/types/controller"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import { Switch } from "@/components/ui/switch"
import OutputDeviceComboBox from "@/components/wizard/OutputConfig/OutputDevices/OutputDeviceComboBox"
import { Button } from "@/components/ui/button"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import WizardSection from "@/components/wizard/common/WizardSection"
import { useTranslation } from "react-i18next"
import {
  IconHome,
  IconPlayerPlay,
  IconPlayerTrackNext,
  IconPlayerTrackNextFilled,
  IconPlayerTrackPrev,
  IconPlayerTrackPrevFilled,
} from "@tabler/icons-react"
import { useState } from "react"
import { Toolbar } from "radix-ui"
import messageExchange from "@/lib/messageExchange"

export interface StepperCalibrationProps {
  controller?: Partial<Controller>
  config: StepperConfig
}

const StepperCalibration = ({
  controller,
  config,
}: StepperCalibrationProps) => {
  const { publish } = messageExchange
  const AVAILABLE_STEPS = ["-50", "-10", "-1", "+1", "+10", "+50"]
  const ICON_MAP = {
    "-50": IconPlayerTrackPrevFilled,
    "-10": IconPlayerTrackPrev,
    "-1": IconPlayerPlay,
    "+1": IconPlayerPlay,
    "+10": IconPlayerTrackNext,
    "+50": IconPlayerTrackNextFilled,
  } as Record<string, React.ComponentType<{ className?: string }>>

  const { t } = useTranslation("wizard")
  const [activeStepCount, setActiveStepCount] = useState<number>(1)
  const calibrate = (step: number) => {
    setActiveStepCount(step)
    move(step)
  }

  const setHome = () => {
    publish({
      key: "CommandStepperCalibration",
      payload: { controller: controller, stepper: config, action: "home" },
    })
  }

  const move = (step: number) => {
    publish({
      key: "CommandStepperCalibration",
      payload: {
        controller: controller,
        stepper: config,
        action: "move",
        steps: step,
      },
    })
  }

  const handleKeyInput = (
    e: React.KeyboardEvent<HTMLButtonElement>,
    step: number,
  ) => {
    switch (e.code) {
      case "Enter":
        e.preventDefault()
        e.stopPropagation()
        setHome()
        break
      case "Space":
        e.preventDefault()
        e.stopPropagation()
        calibrate(step)
        break
    }
  }

  return (
    <div className="flex flex-col gap-2">
      <div className="flex flex-row justify-between gap-4">
        <Toolbar.Root className="flex flex-row gap-2">
          {AVAILABLE_STEPS.map((step) => {
            const stepValue = parseInt(step)
            const Icon = ICON_MAP[step.toString()]
            return (
              <Toolbar.Button key={step} asChild>
                <Button
                  variant={activeStepCount == stepValue ? "default" : "outline"}
                  onClick={() => calibrate(stepValue)}
                  onFocus={() => setActiveStepCount(stepValue)}
                  onKeyDown={(e) => handleKeyInput(e, stepValue)}
                >
                  <Icon className={stepValue == -1 ? "rotate-180" : ""} />
                  {step}
                </Button>
              </Toolbar.Button>
            )
          })}
        </Toolbar.Root>
        <div>
          <Button variant={"secondary"} onClick={setHome}>
            <IconHome />
            {t("ConfigWizard.Device.Stepper.Calibrate.SetHome")}
          </Button>
        </div>
      </div>
      <div className="flex flex-row items-center gap-2">
        <p className="text-muted-foreground text-sm">
          {t("ConfigWizard.Device.Stepper.Calibrate.Hint")}
        </p>
      </div>
    </div>
  )
}

export interface StepperPanelProps {
  variant: "summary" | "details"
  config: StepperConfig
  controller?: Partial<Controller>
  // only used by the details variant
  devices?: DeviceReference[]
  onChange?: (config: StepperConfig) => void
}

type StepperNumberField =
  | "InputRev"
  | "OutputRev"
  | "TestValue"
  | "Acceleration"
  | "Speed"

const numberFields: {
  field: StepperNumberField
  labelKey: string
  defaultProfile?: true
  profileField?: string
}[] = [
  {
    field: "InputRev",
    labelKey: "ConfigWizard.Device.Stepper.InputRevolution",
  },
  {
    field: "OutputRev",
    labelKey: "ConfigWizard.Device.Stepper.OutputRevolution",
    defaultProfile: true,
    profileField: "StepsPerRevolution",
  },
  { field: "TestValue", labelKey: "ConfigWizard.Device.Stepper.TestValue" },
  {
    field: "Acceleration",
    labelKey: "ConfigWizard.Device.Stepper.Acceleration",
    defaultProfile: true,
  },
  {
    field: "Speed",
    labelKey: "ConfigWizard.Device.Stepper.Speed",
    defaultProfile: true,
  },
]

const StepperPanel = ({
  config,
  variant,
  controller,
  devices = [],
  onChange,
}: StepperPanelProps) => {
  const { t } = useTranslation("wizard")
  const currentDevice = devices.find((device) => device.Name === config.Address)

  if (variant == "details") {
    return (
      <>
        <WizardSection>
          <SectionHeadline
            title={t("ConfigWizard.Device.Stepper.Title")}
            description={t("ConfigWizard.Device.Stepper.Description")}
          />
          <div className="flex flex-col gap-2">
            <Label>{t("ConfigWizard.Device.Stepper.Label")}</Label>
            <OutputDeviceComboBox
              devices={devices}
              deviceType="Stepper"
              selectedName={config.Address}
              onChange={(address) =>
                onChange?.({ ...config, Address: address, Name: address })
              }
            />
          </div>
          <div className="grid grid-cols-3 gap-4">
            {numberFields.map(
              ({ field, labelKey, defaultProfile, profileField }) => {
                const profileKey = profileField ?? (field.toString() as string)
                const profileValue = (
                  currentDevice?.Capabilities?.Profile as Record<
                    string,
                    number | boolean
                  >
                )?.[profileKey]
                const fieldValue = config[field]
                const hasNonDefaultValue = defaultProfile
                  ? profileValue !== undefined && profileValue !== fieldValue
                  : false
                return (
                  <div key={field} className="flex flex-col gap-2">
                    <Label htmlFor={`stepper-${field}`}>{t(labelKey)}</Label>
                    <Input
                      id={`stepper-${field}`}
                      type="number"
                      value={config[field]}
                      onChange={(e) =>
                        onChange?.({
                          ...config,
                          [field]: Number(e.target.value),
                        })
                      }
                    />
                    {hasNonDefaultValue && (
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() =>
                          onChange?.({ ...config, [field]: profileValue })
                        }
                      >
                        {t("ConfigWizard.Device.Stepper.Reset")}
                      </Button>
                    )}
                  </div>
                )
              },
            )}
          </div>
          <div className="flex flex-row items-center gap-2">
            <Switch
              id="stepper-compass-mode"
              checked={config.CompassMode}
              onCheckedChange={(checked) =>
                onChange?.({ ...config, CompassMode: !!checked })
              }
            />
            <Label htmlFor="stepper-compass-mode">
              {t("ConfigWizard.Device.Stepper.CompassMode")}
            </Label>
          </div>
        </WizardSection>
        {config.Address && (
          <WizardSection>
            <SectionHeadline
              title={t("ConfigWizard.Device.Stepper.Calibrate.Title")}
              description={t(
                "ConfigWizard.Device.Stepper.Calibrate.Description",
              )}
            />
            <StepperCalibration controller={controller} config={config} />
          </WizardSection>
        )}
      </>
    )
  }

  return (
    variant == "summary" && (
      <div className="flex grow flex-row items-center justify-between gap-2">
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Controller")}</Label>
          {controller?.Name}
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.DisplayType")}</Label>
          <span className="flex flex-row gap-2">
            <DeviceIcon variant="Stepper" />{" "}
            {t("ConfigWizard.Device.Stepper.Label")}
          </span>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.General.Device")}</Label>
          <Badge variant="secondary">{config.Name}</Badge>
        </div>
        <div className="flex flex-col gap-1">
          <Label>{t("ConfigWizard.Device.Stepper.Steps")}</Label>
          <Badge variant="secondary">
            {config.InputRev} / {config.OutputRev}
          </Badge>
        </div>
        {config.CompassMode && (
          <div className="flex flex-col gap-1">
            <Label>{t("ConfigWizard.General.Options")}</Label>
            <Badge variant="secondary">
              {t("ConfigWizard.Device.Stepper.CompassMode")}
            </Badge>
          </div>
        )}
      </div>
    )
  )
}

export default StepperPanel
