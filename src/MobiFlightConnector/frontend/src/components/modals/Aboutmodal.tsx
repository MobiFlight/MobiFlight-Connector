import { useState } from "react"
import { useNavigate } from "react-router"
import AboutDialog from "../about/AboutDialog"


const AboutModal = () => {


  const navigate = useNavigate()

  const [open, setOpen] = useState(true)

  return (
    <AboutDialog
      open={open}
      onOpenChange={(open: boolean) => {
        setOpen(open)
        navigate(-1)
      }}
    />
  )
}
export default AboutModal