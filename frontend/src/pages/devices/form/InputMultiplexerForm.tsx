import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { IDeviceElement, IDeviceItem } from "@/types"
import { IconArrowsLeftRight } from "@tabler/icons-react"
import React from "react"
import { useNavigate } from "react-router-dom"

type InputMultiplexerProps = {
  device: IDeviceItem
  element: IDeviceElement
  allowEditPinSx: boolean
  setElement: (element: IDeviceElement) => void
}

const InputMultiplexerForm = (props: InputMultiplexerProps) => {
  const navigate = useNavigate()
  const { element, device, setElement } = props
  const PinS0 = element.ConfigData["PinS0"]
  const PinS1 = element.ConfigData["PinS1"]
  const PinS2 = element.ConfigData["PinS2"]
  const PinS3 = element.ConfigData["PinS3"]
  const PinData = element.ConfigData["PinData"]
  const firstMultiplexer = device.Elements?.find(e=>e.Type === "InputMultiplexer")
  const allowEditPinSx = firstMultiplexer?.Id === element.Id ?? false
  const freePins = device.Pins.filter(
    (pin) =>
      !pin.Used ||
      pin.Pin === parseInt(element.ConfigData["PinS0"]) ||
      pin.Pin === parseInt(element.ConfigData["PinS1"]) ||
      pin.Pin === parseInt(element.ConfigData["PinS2"]) ||
      pin.Pin === parseInt(element.ConfigData["PinS3"]) ||
      pin.Pin === parseInt(element.ConfigData["PinData"])
  )

  return (
    <>
      <div className="flex flex-col gap-8">
        <div className="w-1/4">
          <Label>Name</Label>
          <Input
            name="Name"
            value={element.Name}
            onChange={(e) => setElement({ ...element, Name: e.target.value })}
          />
        </div>
        {!allowEditPinSx ? (
          <div className="flex flex-row gap-4 items-end">
            <div>
              <Label>Pin 0</Label>
              <div className="p-2 bg-gray-200 dark:bg-background rounded-sm border text-center">{PinS0}</div>
            </div>
            <div>
              <Label>Pin 1</Label>
              <div className="p-2 bg-gray-200 dark:bg-background rounded-sm border text-center">{PinS1}</div>
            </div>
            <div>
              <Label>Pin 2</Label>
              <div className="p-2 bg-gray-200 dark:bg-background rounded-sm border text-center">{PinS2}</div>
            </div>
            <div>
              <Label>Pin 3</Label>
              <div className="p-2 bg-gray-200 dark:bg-background rounded-sm border text-center">{PinS3}</div>
            </div>
            <Button variant={"ghost"} onClick={() => {
              navigate(`/devices/${device.Type}/${device.Id}/elements/${firstMultiplexer!.Id}`)
            }}>Edit shared settings</Button>
          </div>
        ) : (
          <div className="flex flex-row gap-4">
            <div>
              <Label>Pin 0</Label>
              <div className="w-1/3">
                <ComboBox
                  className="w-[120px]"
                  options={freePins.map((pin) => ({
                    value: pin.Pin.toString(),
                    label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                  }))}
                  value={PinS0}
                  onSelect={(value) =>
                    setElement({
                      ...element,
                      ConfigData: { ...element.ConfigData, PinS0: value },
                    })
                  }
                />
              </div>
            </div>
            <IconArrowsLeftRight
              className="stroke-primary mt-8 cursor-pointer hover:stroke-blue-300 transition-all"
              onClick={() => {
                setElement({
                  ...element,
                  ConfigData: {
                    ...element.ConfigData,
                    PinS0: PinS1,
                    PinS1: PinS0,
                  },
                })
              }}
            />
            <div>
              <Label>Pin 1</Label>
              <div className="w-1/3">
                <ComboBox
                  className="w-[120px]"
                  options={freePins.map((pin) => ({
                    value: pin.Pin.toString(),
                    label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                  }))}
                  value={PinS1}
                  onSelect={(value) =>
                    setElement({
                      ...element,
                      ConfigData: { ...element.ConfigData, PinS1: value },
                    })
                  }
                />
              </div>
            </div>
            <IconArrowsLeftRight
              className="stroke-primary mt-8 cursor-pointer hover:stroke-blue-300 transition-all"
              onClick={() => {
                setElement({
                  ...element,
                  ConfigData: {
                    ...element.ConfigData,
                    PinS1: PinS2,
                    PinS2: PinS1,
                  },
                })
              }}
            />
            <div>
              <Label>Pin 2</Label>
              <div className="w-1/3">
                <ComboBox
                  className="w-[120px]"
                  options={freePins.map((pin) => ({
                    value: pin.Pin.toString(),
                    label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                  }))}
                  value={PinS2}
                  onSelect={(value) =>
                    setElement({
                      ...element,
                      ConfigData: { ...element.ConfigData, PinS2: value },
                    })
                  }
                />
              </div>
            </div>
            <IconArrowsLeftRight
              className="stroke-primary mt-8 cursor-pointer hover:stroke-blue-300 transition-all"
              onClick={() => {
                setElement({
                  ...element,
                  ConfigData: {
                    ...element.ConfigData,
                    PinS2: PinS3,
                    PinS3: PinS2,
                  },
                })
              }}
            />
            <div>
              <Label>Pin 3</Label>
              <div className="w-1/3">
                <ComboBox
                  className="w-[120px]"
                  options={freePins.map((pin) => ({
                    value: pin.Pin.toString(),
                    label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                  }))}
                  value={PinS3}
                  onSelect={(value) =>
                    setElement({
                      ...element,
                      ConfigData: { ...element.ConfigData, PinS3: value },
                    })
                  }
                />
              </div>
            </div>
          </div>
        )}

        <div className="flex flex-row gap-4">
          <div>
            <Label>Data</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-[120px]"
                options={freePins.map((pin) => ({
                  value: pin.Pin.toString(),
                  label: `${pin.Pin}${pin.isPWM ? " (PWM)" : ""}`,
                }))}
                value={PinData}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, PinData: value },
                  })
                }
              />
            </div>
          </div>
        </div>
      </div>
    </>
  )
}

export default InputMultiplexerForm
