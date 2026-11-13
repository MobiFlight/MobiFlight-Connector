import { ConfigItemType } from "@/types/config"
import { IconChevronRight } from "@tabler/icons-react"
import { Fragment } from "react"

const DrawerBreadcrumb = ({
  mode,
  crumbs,
}: {
  mode: ConfigItemType
  crumbs: string[]
}) => {
  return (
    <div className="flex flex-row items-center gap-2">
      {mode === "InputConfigItem" ? (
        <div className="bg-cfginput h-5 w-2"></div>
      ) : (
        <div className="bg-cfgoutput h-5 w-2"></div>
      )}
      {crumbs.map((crumb, index) => (
        <Fragment key={index}>
          <span className="text-xl">
            {crumb}
          </span>
          {index < crumbs.length - 1 && (
            <IconChevronRight className="-mx-1 mt-0.5 w-5" />
          )}
        </Fragment>
      ))}
    </div>
  )
}

export default DrawerBreadcrumb
