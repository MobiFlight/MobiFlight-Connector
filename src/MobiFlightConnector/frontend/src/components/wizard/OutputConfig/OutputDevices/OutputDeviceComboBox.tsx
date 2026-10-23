import ComboBox from "@/components/ComboBox"
import { BaseDevice, OutputDeviceType } from "@/types/controller"

export interface OutputDeviceComboBoxProps {
  devices: BaseDevice[]
  deviceType: OutputDeviceType
  selectedName: string
  onChange: (name: string) => void
}

const OutputDeviceComboBox = ({
  devices,
  deviceType,
  selectedName,
  onChange,
}: OutputDeviceComboBoxProps) => {
  // Keep the configured device visible
  // even if it is currently not available on the controller
  const deviceOptions: BaseDevice[] =
    selectedName && !devices.some((device) => device.Name === selectedName)
      ? [
          ...devices,
          { Name: selectedName, Label: selectedName, Type: deviceType },
        ]
      : devices

  const selectedDevice = deviceOptions.find(
    (device) => device.Name === selectedName,
  )

  return (
    <ComboBox
      widthClass="w-56"
      align="start"
      items={deviceOptions}
      getLabel={(device) => device.Label ?? device.Name}
      getValue={(device) => device.Name}
      isSelected={(device, selected) => device.Name === selected?.Name}
      selected={selectedDevice}
      setSelected={(device) => onChange(device?.Name ?? "")}
    />
  )
}

export default OutputDeviceComboBox
