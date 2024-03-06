import { NavLink, useParams } from "react-router-dom"
import { DarkModeToggle } from "./mobiflight/darkmode-toggle"
import {
  IconDeviceGamepad2,
  IconList,
  IconListSearch,
  IconSettings,
} from "@tabler/icons-react"
import { Button } from "./ui/button"
import { cn } from "@/lib/utils"

type SidebarProps = {
  className?: string
}

const Sidebar = (props: SidebarProps) => {
  const params = useParams()
  const normalClassNameProps =
    "border-none hover:bg-gray-200 w-12 h-12 group/side"
  const activeClassNameProps =
    "dark:bg-gray-700 border-none bg-primary stroke-white"

  return (
    <div
      className={cn(
        `z-50 flex w-16 min-w-16 flex-col items-center justify-between bg-zinc-400 pb-8 dark:bg-slate-800`,
        props.className,
      )}
    >
      <div className="flex flex-col items-center gap-8 pt-24">
        <NavLink
          to={`/devices`}
          children={(nav) => (
            <Button
              variant="outline"
              className={
                nav.isActive
                  ? `${activeClassNameProps} ${normalClassNameProps}`
                  : normalClassNameProps
              }
              size="icon"
            >
              <IconDeviceGamepad2
                className={
                  nav.isActive
                    ? `stroke-white group-hover/side:stroke-blue-900`
                    : "group-hover/side:stroke-blue-900"
                }
              ></IconDeviceGamepad2>
            </Button>
          )}
        ></NavLink>
        <NavLink
          to={`/projects/${params.id}/configs`}
          children={(nav) => (
            <Button
              variant="outline"
              className={
                nav.isActive
                  ? `${activeClassNameProps} ${normalClassNameProps}`
                  : normalClassNameProps
              }
              size="icon"
            >
              <IconList
                className={
                  nav.isActive
                    ? `stroke-white group-hover/side:stroke-blue-900`
                    : "group-hover/side:stroke-blue-900"
                }
              ></IconList>
            </Button>
          )}
        ></NavLink>
      </div>
      <div className="flex flex-col items-center gap-8">
        <NavLink
          to="/log"
          children={(nav) => (
            <Button
              variant="outline"
              className={
                nav.isActive
                  ? `${activeClassNameProps} ${normalClassNameProps}`
                  : normalClassNameProps
              }
              size="icon"
            >
              <IconListSearch
                className={
                  nav.isActive
                    ? `stroke-white group-hover/side:stroke-blue-900`
                    : "group-hover/side:stroke-blue-900"
                }
              ></IconListSearch>
            </Button>
          )}
        ></NavLink>
        <NavLink
          to="/settings"
          children={(nav) => (
            <Button
              variant="outline"
              className={
                nav.isActive
                  ? `${activeClassNameProps} ${normalClassNameProps}`
                  : normalClassNameProps
              }
              size="icon"
            >
              <IconSettings
                className={
                  nav.isActive
                    ? `stroke-white group-hover/side:stroke-blue-900`
                    : "group-hover/side:stroke-blue-900"
                }
              ></IconSettings>
            </Button>
          )}
        ></NavLink>
        <DarkModeToggle className="hover:border hover:border-gray-500"></DarkModeToggle>
      </div>
    </div>
  )
}

export default Sidebar
