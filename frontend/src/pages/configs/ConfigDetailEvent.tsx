import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import {
  Dialog,
  DialogContent,
  DialogHeader
} from "@/components/ui/dialog"

import msfs2020 from "@/assets/sims/msfs2020-logo.png"
import mfLogo from "@/assets/mobiflight-logo-border.png"
import deviceInput from "@/assets/ui/hand-flipping-switch.png"

import { useState } from "react"
import { IconHelp } from "@tabler/icons-react"
import { useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { IConfigItem } from "@/types"

interface ConfigDetailEventViewProps {
  config: IConfigItem
  className?: string
  editMode: boolean
  onEnterEditMode: () => void
  onCancelEditMode: () => void
  onSaveEditMode: () => void
}

const ConfigDetailEventView = (props: ConfigDetailEventViewProps) => {
  const {
    editMode,
    className,
    onEnterEditMode,
    onCancelEditMode,
    onSaveEditMode,
  } = props
  const [dialogOpen, setDialogOpen] = useState(false)
  const navigate = useNavigate()

  return (
    <div className={cn(editMode ? "" : "", className)}>
      <Card>
        <CardHeader>
          <h3 className="text-xl">Event</h3>
          <p className="text-xs">Define device or simulator events</p>
        </CardHeader>
        <CardContent className="text-center">
          No settings defined.
        </CardContent>
        <CardFooter className="flex flex-row items-center gap-4">
          {!editMode&&<Button onClick={() => onEnterEditMode()}>Edit</Button>}
          {editMode&&<Button onClick={() => onSaveEditMode()}>Save</Button>}
          {editMode&&<Button variant={"ghost"} onClick={() => onCancelEditMode()}>Cancel</Button>}
          <IconHelp>Learn more</IconHelp>
        </CardFooter>
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

export default ConfigDetailEventView
