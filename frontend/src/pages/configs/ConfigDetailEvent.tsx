import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import { Dialog, DialogContent, DialogHeader } from "@/components/ui/dialog"

import msfs2020 from "@/assets/sims/msfs2020-logo.png"
import mfLogo from "@/assets/mobiflight-logo-border.png"
import deviceInput from "@/assets/ui/hand-flipping-switch.png"

import { useState } from "react"
import { IconHelp } from "@tabler/icons-react"
import { useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { IConfigItem } from "@/types"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { IConfigEvent } from "@/types/config"
import SimConnectEvent from "./event/SimConnectEvent"
import MobiFlightVariable from "./event/MobiFlightVariable"
import XplaneEvent from "./event/Xplane"
import FSUIPC from "./event/FSUIPC"

interface ConfigDetailEventViewProps {
  config: IConfigItem
  className?: string
  editMode: boolean
  onEnterEditMode: () => void
  onCancelEditMode: () => void
  onSaveEditMode: () => void
}

const ConfigDetailEvent = (props: ConfigDetailEventViewProps) => {
  const {
    config,
    editMode,
    className,
    onEnterEditMode,
    onCancelEditMode,
    onSaveEditMode,
  } = props
  const [dialogOpen, setDialogOpen] = useState(false)
  const navigate = useNavigate()

  const [tempConfig, setTempConfig] = useState(config)

  const OutputEventOptions = {
    SIMCONNECT: "MSFS 2020",
    XPLANE: "X-Plane",
    VARIABLE: "MobiFlight",
    FSUIPC: "FSUIPC",
  }

  return (
    <div className={cn(editMode ? "" : "", className)}>
      <Card>
        <CardHeader>
          <h2 className="">
            Select one type of event from the set of options:
          </h2>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          <div className="flex flex-row items-center gap-4">
            <div className="w-16">Type:</div>
            <div>
              <Select
                onValueChange={(value) => {
                  setTempConfig({
                    ...config,
                    Event: {
                      Type: value as IConfigEvent["Type"],
                      Settings: config.Event.Settings,
                    },
                  })
                }}
                defaultValue={config.Event.Type}
              >
                <SelectTrigger className="w-[180px]">
                  <SelectValue placeholder={config.Event.Type} />
                </SelectTrigger>
                <SelectContent>
                  {Object.entries(OutputEventOptions).map(([key, value]) => (
                    <SelectItem key={key} value={key}>
                      {value}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
          <div>
            {tempConfig.Event.Type === "SIMCONNECT" && (
              <SimConnectEvent config={config}></SimConnectEvent>
            )}
            {tempConfig.Event.Type === "VARIABLE" && (
              <MobiFlightVariable></MobiFlightVariable>
            )}
            {tempConfig.Event.Type === "XPLANE" && <XplaneEvent></XplaneEvent>}
            {tempConfig.Event.Type === "FSUIPC" && <FSUIPC></FSUIPC>}
          </div>
        </CardContent>
        <CardFooter className="flex flex-row items-center gap-4"></CardFooter>
      </Card>
      <Dialog
        open={dialogOpen}
        onOpenChange={() => {
          navigate("../")
          setDialogOpen(false)
        }}
      >
        <DialogContent className="max-w-3xl">
          <DialogHeader className="flex flex-col gap-8">
            <div className="flex flex-col gap-2">
              <h2 className="text-2xl">
                What kind of event would you like to use?
              </h2>
              <div className="text-sm">
                You can use different event sources for the value which you want
                to use
              </div>
            </div>
            <ul className="grid grid-cols-3 gap-4">
              <li>
                <Card className="flex aspect-square cursor-pointer flex-col items-center gap-4 pt-4">
                  <img
                    src={deviceInput}
                    alt="Hand flipping the switch"
                    className="h-32 rounded-xl"
                  />
                  <div className="text-center text-xl">
                    <span className="font-bold">Device input</span>
                  </div>
                </Card>
              </li>
              <li>
                <Card className="flex aspect-square cursor-pointer flex-col items-center gap-4 pt-4">
                  <img
                    src={msfs2020}
                    alt="MSFS2020 Logo"
                    className="h-32 rounded-xl"
                  />
                  <div className="text-center text-xl">
                    <span className="font-bold">MSFS2020</span>
                    <br /> Simulator Value
                  </div>
                </Card>
              </li>
              <li>
                <Card
                  className="flex aspect-square cursor-pointer flex-col items-center gap-4 pt-4"
                  onClickCapture={() => {
                    navigate("../mobiflight")
                    setDialogOpen(false)
                  }}
                >
                  <img
                    src={mfLogo}
                    alt="MobiFlightLogo"
                    className="h-32 rounded-xl"
                  />
                  <div className="text-center text-xl">
                    <span className="font-bold">
                      MobiFlight
                      <br />
                    </span>
                    Variable
                  </div>
                </Card>
              </li>
            </ul>
            <div className="flex flex-row items-center gap-2 text-primary">
              <IconHelp /> What are the different options?
            </div>
          </DialogHeader>
        </DialogContent>
      </Dialog>
    </div>
  )
}

export default ConfigDetailEvent
