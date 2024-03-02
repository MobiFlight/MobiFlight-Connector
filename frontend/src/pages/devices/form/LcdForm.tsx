import { ComboBox } from "@/components/mobiflight/ComboBox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { IDeviceElement, IDeviceItem } from "@/types"
import { IconHelp } from "@tabler/icons-react"

type LcdFormProps = {
  device: IDeviceItem
  element: IDeviceElement
  setElement: (element: IDeviceElement) => void
}

const LcdForm = (props: LcdFormProps) => {
  const { element, setElement } = props
  const Address = element.ConfigData["Address"]
  const Rows = element.ConfigData["Lines"]
  const Columns = element.ConfigData["Columns"]
  const availableAdresses = [
    { value: "0x20", label: "0x20" },
    { value: "0x21", label: "0x21" },
    { value: "0x22", label: "0x22" },
    { value: "0x23", label: "0x23" },
    { value: "0x24", label: "0x24" },
    { value: "0x25", label: "0x25" },
    { value: "0x26", label: "0x26" },
    { value: "0x27", label: "0x27" },
    { value: "0x38", label: "0x38" },
    { value: "0x39", label: "0x39" },
    { value: "0x3A", label: "0x3A" },
    { value: "0x3B", label: "0x3B" },
    { value: "0x3C", label: "0x3C" },
    { value: "0x3D", label: "0x3D" },
    { value: "0x3E", label: "0x3E" },
    { value: "0x3F", label: "0x3F" },
  ]

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
        <div className="flex flex-row gap-4">
          <div className="flex flex-col gap-2">
            <Label>Address</Label>
            <div className="w-1/3">
              <ComboBox
                className="w-24"
                options={availableAdresses}
                value={Address}
                onSelect={(value) =>
                  setElement({
                    ...element,
                    ConfigData: { ...element.ConfigData, Address: value },
                  })
                }
              />
            </div>
          </div>
          
          <div className="flex flex-col gap-2 w-16">
            <Label>Rows</Label>
            <Input
              name="Rows"
              value={Rows}
              onChange={(e) =>
                setElement({
                  ...element,
                  ConfigData: {
                    ...element.ConfigData,
                    Rows: e.target.value,
                  },
                })
              }
            />
          </div>
          <div className="flex flex-col gap-2 w-16">
            <Label>Columns</Label>
            <Input
              name="Columns"
              value={Columns}
              onChange={(e) =>
                setElement({
                  ...element,
                  ConfigData: {
                    ...element.ConfigData,
                    Columns: e.target.value,
                  },
                })
              }
            />
          </div>
          <div className="mt-8 cursor-pointer stroke-primary text-primary  hover:stroke-blue-400  hover:text-blue-400">
            <a
              onClick={() => {}}
              className="flex flex-row mb-2 gap-2 items-center"
            >
              <IconHelp></IconHelp>
              <p>Help me choose the right values</p>
            </a>
          </div>
        </div>
      </div>
    </>
  )
}

export default LcdForm
