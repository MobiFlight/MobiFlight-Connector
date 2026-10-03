import { useState } from "react"
import { useNavigate } from "react-router"
import SettingsDialog from "../settings/SettingsDialog"

const SettingsModal = () => {


  const navigate = useNavigate()

  const [open, setOpen] = useState(true)

  return (
    <SettingsDialog
      isOpen={open}
      onOpenChange={(open: boolean) => {
        setOpen(open)
        navigate(-1)
      }}
    />
  )
}
export default SettingsModal
