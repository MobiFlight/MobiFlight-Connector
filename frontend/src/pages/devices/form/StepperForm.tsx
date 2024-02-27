import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import { IDeviceElement, IDeviceItem } from "@/types"
import { StepperProfilesPresets } from "@/types/deviceElements.d"
import { IconHelp } from "@tabler/icons-react"
import { useEffect } from "react"

type StepperFormProps = {
  device: IDeviceItem
  element: IDeviceElement
  setElement: (element: IDeviceElement) => void
}

const StepperForm = (props: StepperFormProps) => {
  const ProfilePresets = StepperProfilesPresets
  const { element, device, setElement } = props
  const Pin1 = element.ConfigData["Pin1"]
  const Pin2 = element.ConfigData["Pin2"]
  const Pin3 = element.ConfigData["Pin3"]
  const Pin4 = element.ConfigData["Pin4"]
  const AutoHome = element.ConfigData["AutoHome"] === "True"
  const PinHome = element.ConfigData["PinHome"]
  const Mode = element.ConfigData["Mode"]
  const Backlash = element.ConfigData["Backlash"]
  const Profile = element.ConfigData["Profile"]
  const freePins = device.Pins.filter(
    (pin) =>
      !pin.Used ||
      pin.Pin === parseInt(element.ConfigData["Pin1"]) ||
      pin.Pin === parseInt(element.ConfigData["Pin2"]) ||
      pin.Pin === parseInt(element.ConfigData["Pin3"]) ||
      pin.Pin === parseInt(element.ConfigData["Pin4"])
  )

  const ProfileOptions = ProfilePresets.map((p) => ({
    value: p.value.id,
    label: p.label,
  }))

  const ModeOptions = [
    { value: "0", label: "Half-step mode" },
    { value: "1", label: "Full-step mode" },
    { value: "2", label: "Driver mode" },
  ]

  // find a free pin for our AutoHome
  useEffect(() => {
    if (
      AutoHome &&
      freePins.find((p) => p.Pin.toString() == PinHome) === undefined
    ) {
      setElement({
        ...element,
        ConfigData: {
          ...element.ConfigData,
          PinHome: freePins[0].Pin.toString(),
        },
      })
    }
  }, [AutoHome])

  return (
    <>
      <div className="flex flex-col gap-8">
        <div className="w-1/2">
          <Label>Name</Label>
          <Input
            className="w-1/2"
            name="Name"
            value={element.Name}
            onChange={(e) => setElement({ ...element, Name: e.target.value })}
          />
        </div>
        <div className="flex flex-row gap-4 items-end">
          <div>
            <Label>Profile</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[360px]"
                options={ProfileOptions}
                value={Profile}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, Profile: value },
                  })
                }
              />
            </div>
          </div>
          <div className="cursor-pointer stroke-primary text-primary  hover:stroke-blue-400  hover:text-blue-400">
            <a
              onClick={() => {}}
              className="flex flex-row mb-2 gap-2 items-center"
            >
              <IconHelp></IconHelp>
              <p>Help me choose the right profile</p>
            </a>
          </div>
        </div>
        <div className="flex flex-row gap-4">
          <div>
            <Label>Pin1</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[120px]"
                options={freePins.map((pin) => ({
                  value: pin.Pin.toString(),
                  label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                }))}
                value={Pin1}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, Pin1: value },
                  })
                }
              />
            </div>
          </div>
          <div>
            <Label>Pin2</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[120px]"
                options={freePins.map((pin) => ({
                  value: pin.Pin.toString(),
                  label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                }))}
                value={Pin2}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, Pin2: value },
                  })
                }
              />
            </div>
          </div>
          {/* Pin3 and Pin4 are optional, so we only show them if they are used */}
          {Mode !== "2" && (
            <div>
              <Label>Pin3</Label>
              <div className="w-1/3">
                <ComboBox
                  className="w-[120px]"
                  options={freePins.map((pin) => ({
                    value: pin.Pin.toString(),
                    label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                  }))}
                  value={Pin3}
                  onSelect={(value) =>
                    setElement({
                      ...element,
                      ConfigData: { ...element.ConfigData, Pin3: value },
                    })
                  }
                />
              </div>
            </div>
          )}
          {Mode !== "2" && (
            <div>
              <Label>Pin4</Label>
              <div className="w-1/3">
                <ComboBox
                  className="w-[120px]"
                  options={freePins.map((pin) => ({
                    value: pin.Pin.toString(),
                    label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                  }))}
                  value={Pin4}
                  onSelect={(value) =>
                    setElement({
                      ...element,
                      ConfigData: { ...element.ConfigData, Pin4: value },
                    })
                  }
                />
              </div>
            </div>
          )}
        </div>
        <div className="flex flex-row gap-4 items-center h-12">
          <div className="flex flex-row items-center gap-4">
            <Switch
              checked={AutoHome}
              onCheckedChange={(_) =>
                setElement({
                  ...element,
                  ConfigData: {
                    ...element.ConfigData,
                    AutoHome: !AutoHome ? "True" : "False",
                  },
                })
              }
            />
            <Label>Use Auto Home</Label>
          </div>
          {AutoHome && (
            <div className="flex flex-col">
              <div className="w-1/3">
                <ComboBox
                  className="w-[120px]"
                  options={freePins.map((pin) => ({
                    value: pin.Pin.toString(),
                    label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                  }))}
                  value={PinHome}
                  onSelect={(value) =>
                    setElement({
                      ...element,
                      ConfigData: { ...element.ConfigData, PinHome: value },
                    })
                  }
                />
              </div>
            </div>
          )}
        </div>

        {Profile === "255" && (
          <>
            <div className="flex flex-row gap-4 items-end">
              <div>
                <Label>Mode</Label>
                <div className="w-1/3">
                  <ComboBox
                    className="w-[240px]"
                    options={ModeOptions}
                    value={Mode}
                    onSelect={(value) =>
                      setElement({
                        ...element,
                        ConfigData: { ...element.ConfigData, Mode: value },
                      })
                    }
                  />
                </div>
              </div>
              <div className="cursor-pointer stroke-primary text-primary  hover:stroke-blue-400  hover:text-blue-400">
                <a
                  onClick={() => {}}
                  className="flex flex-row mb-2 gap-2 items-center"
                >
                  <IconHelp></IconHelp>
                  <p>Help me choose the right mode</p>
                </a>
              </div>
            </div>
            <div className="flex flex-row gap-4 items-end">
              <div>
                <Label>Backlash</Label>
                <div className="w-1/3">
                  <Input
                    name="Backlash"
                    value={Backlash}
                    onChange={(e) =>
                      setElement({
                        ...element,
                        ConfigData: {
                          ...element.ConfigData,
                          Backlash: e.target.value,
                        },
                      })
                    }
                  />
                </div>
              </div>
            </div>
          </>
        )}
      </div>
    </>
  )
}

export default StepperForm
