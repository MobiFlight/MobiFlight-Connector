import { ProjectInfo } from "@/types/project"
import ProjectListItem from "./ProjectListItem"
import { Button } from "../ui/button"
import { ScrollArea } from "../ui/scroll-area"
import { useSearchParams } from "react-router"
import { Card, CardContent } from "@/components/ui/card"
import { useRef } from "react"
import { ProjectCreateButton } from "@/components/project/ProjectCreateButton"
import { cn } from "@/lib/utils"
import ProjectListFilter from "@/components/project/ProjectListFilter"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"

export type ProjectListProps = {
  className?: string
  summarys: ProjectInfo[]
  activeProject?: ProjectInfo
  onSelect: (project: ProjectInfo) => void
}

const ProjectList = ({
  className,
  summarys,
  activeProject,
  onSelect,
}: ProjectListProps) => {
  const refActiveElement = useRef<HTMLDivElement | null>(null)

  const { publish } = publishOnMessageExchange()

  const [searchParams, setSearchParams] = useSearchParams()
  const activeFilter = searchParams.get("projects_filter") || "all"
  const activeTextFilter = searchParams.get("projects_text") || ""

  const resetAllFilters = () => {
    setSearchParams({})
  }

  const scrollActiveProjectIntoView = () => {
    if (refActiveElement.current) {
      window.setTimeout(() => {
        refActiveElement.current?.scrollIntoView({
          behavior: "smooth",
          block: "nearest",
        })
      }, 500)
    }
  }

  const filteredSummarys = summarys
    .filter((project) => {
      if (activeFilter === "all") return true
      if (activeFilter === "msfs") return project.Sim === "msfs"
      if (activeFilter === "xplane") return project.Sim === "xplane"
      return true
    })
    .filter((project) => {
      if (!activeTextFilter) return true
      return project.Name.toLowerCase().includes(activeTextFilter.toLowerCase())
    })

  const onListItemRemove = (index: number) => {
    publish({
      key: "CommandMainMenu",
      payload: {
        action: "virtual.recent.remove",
        index: index,
      },
    })
  }

  return (
    <div className={cn(`flex grow flex-col gap-4`, className)}>
      <ProjectListFilter />
      {summarys.length > 0 ? (
        <div className="relative flex flex-0 grow flex-col">
          <ScrollArea
            className="grow pr-2 transition-all duration-300"
            onMouseLeave={scrollActiveProjectIntoView}
          >
            <div
              className="group/projectlist flex w-[calc(100%)] flex-row flex-wrap gap-4"
              data-testid="recent-projects-list"
            >
              {filteredSummarys.length > 0 ? (
                filteredSummarys.map((project, index) => {
                  const isActive = activeProject?.FilePath === project.FilePath
                  const refActive = isActive ? { ref: refActiveElement } : {}
                  return (
                    <ProjectListItem
                      {...refActive}
                      key={`${project.Name}-${index}`}
                      summary={project}
                      className={`w-[calc(100%-1rem)] 2xl:w-[calc(50%-1rem)] 2xl:max-w-[calc(50%-1rem)]`}
                      active={isActive}
                      onClick={() => {
                        if (isActive) return
                        onSelect(project)
                      }}
                      onClickRemove={() => onListItemRemove(index)}
                    />
                  )
                })
              ) : (
                <Card className="w-full">
                  <CardContent className="flex flex-col items-center justify-center gap-4 pt-4">
                    <div className="text-muted-foreground">
                      Current filter doesn't match any projects.
                    </div>
                    <Button className="h-8" onClick={() => resetAllFilters()}>
                      Clear Filter
                    </Button>
                  </CardContent>
                </Card>
              )}
            </div>
          </ScrollArea>
        </div>
      ) : (
        <Card className="w-full">
          <CardContent className="flex flex-col items-center justify-center gap-4 pt-4">
            <div className="text-muted-foreground">
              Create your first project to get started!
            </div>
            <ProjectCreateButton />
          </CardContent>
        </Card>
      )}
    </div>
  )
}

export default ProjectList
