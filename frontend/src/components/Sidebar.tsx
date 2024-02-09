import { DarkModeToggle } from './mobiflight/darkmode-toggle'

type SidebarProps = {
  className?: string
}

const Sidebar = (props: SidebarProps) => {
  return (
    <div className={`${props.className} bg-blue-950 w-16 flex flex-col justify-end items-center pb-8`}>
      <DarkModeToggle></DarkModeToggle>
    </div>
  )
}

export default Sidebar