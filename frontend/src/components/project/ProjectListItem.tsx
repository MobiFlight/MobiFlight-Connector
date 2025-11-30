import { ProjectInfo } from "@/types/project"
import { forwardRef, HtmlHTMLAttributes } from "react"
import { Badge } from "../ui/badge"
import { cn } from "@/lib/utils"
import { ProjectCardImage, ProjectCardTitle } from "./ProjectCard"
import ProjectFavStar from "./ProjectFavStar"
import { useTranslation } from "react-i18next"

export type ProjectListItemProps = HtmlHTMLAttributes<HTMLDivElement> & {
  summary: ProjectInfo
  active?: boolean
}

const ProjectListItem = forwardRef<HTMLDivElement, ProjectListItemProps>(
  ({ summary, className, active, ...props }, ref) => {
    const { t } = useTranslation()

    const activateStateClassName = active
      ? "bg-primary/20"
      : "opacity-75 group-hover/projectlist:opacity-100"
    const bgColor = summary.Sim ? "bg-primary" : "bg-muted-foreground"

    const simulatorLabel = summary.Sim
      ? t(`Project.Simulator.${summary.Sim.toLowerCase()}`)
      : t(`Project.Simulator.none`)

    return (
      <div
        data-testid ="project-list-item"
        className={cn(
          "group flex flex-row items-center justify-between gap-2 rounded-md p-2",
          "shadow-sm transition-all duration-200 ease-in-out hover:shadow-md",
          "hover:border-primary hover:bg-primary/10 cursor-pointer",
          activateStateClassName,
          className,
        )}
        ref={ref}
        {...props}
      >
        <div className="flex flex-row gap-4 w-full">
          <div className="relative shrink-0">
            <ProjectCardImage summary={summary} className="h-24 w-32" />
            <div className="absolute inset-0 flex items-start justify-start p-2">
              <ProjectFavStar summary={summary} variant="small" />
            </div>
          </div>

          <div className="flex flex-1 flex-col gap-2 w-1">
            <ProjectCardTitle summary={summary} variant="listitem" />
            <div className="flex flex-row items-end justify-between">
              <div className="flex flex-col gap-2">
                <div className="flex flex-row gap-1">
                  <Badge key={summary.Sim} className={bgColor}>
                    {simulatorLabel}
                  </Badge>
                </div>
              </div>
            </div>
            <div
              title={summary.FilePath}
              className="text-muted-foreground truncate text-sm"
            >
              {summary.FilePath}
            </div>
          </div>
        </div>
      </div>
    )
  },
)

export default ProjectListItem
