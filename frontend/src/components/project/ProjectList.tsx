import { ProjectInfo } from "@/types/project"
import ProjectListItem from "./ProjectListItem"
import { Button } from "../ui/button"
import { ScrollArea } from "../ui/scroll-area"
import { useSearchParams } from "react-router"
import { Input } from "@/components/ui/input"
import { Card, CardContent } from "@/components/ui/card"
import { useRef } from "react"
import { ProjectCreateButton } from "@/components/project/ProjectCreateButton"
import { useTranslation } from "react-i18next"

export type ProjectListProps = {
  summarys: ProjectInfo[]
  activeProject?: ProjectInfo
  onSelect: (project: ProjectInfo) => void
}

const ProjectList = ({
  summarys,
  activeProject,
  onSelect,
}: ProjectListProps) => {
  const [searchParams, setSearchParams] = useSearchParams()
  const { t } = useTranslation()
  const activeFilter = searchParams.get("projects_filter") || "all"
  const activeTextFilter = searchParams.get("projects_text") || ""
  const refActiveElement = useRef<HTMLDivElement | null>(null)

  const resetAllFilters = () => {
    setSearchParams({})
  }

  const handleFilterChange = (filter: string) => {
    setSearchParams({
      projects_filter: filter,
      projects_text: activeTextFilter,
    })
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

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-row gap-2" data-testid="recent-projects-filter-bar">
        <Input
          placeholder={t("Project.Card.Filter.Search.Placeholder")}
          className="h-8 w-36 transition-all duration-500 md:w-56"
          value={activeTextFilter}
          onChange={(e) =>
            setSearchParams({
              projects_text: e.target.value,
              projects_filter: activeFilter,
            })
          }
        ></Input>
        <Button
          className="h-8 px-3 text-sm"
          variant={activeFilter === "all" ? "default" : "outline"}
          onClick={() => handleFilterChange("all")}
        >
          All
        </Button>
        <Button
          className="h-8 px-3 text-sm"
          variant={activeFilter === "msfs" ? "default" : "outline"}
          onClick={() => handleFilterChange("msfs")}
        >
          Microsoft
        </Button>
        <Button
          className="h-8 px-3 text-sm"
          variant={activeFilter === "xplane" ? "default" : "outline"}
          onClick={() => handleFilterChange("xplane")}
        >
          X-Plane
        </Button>
      </div>
      {summarys.length > 0 ? (
        <ScrollArea
          className="transition-all h-112 vlg:h-180 vxl:h-260 pr-2 duration-300"
          onMouseLeave={scrollActiveProjectIntoView}
        >
          <div className="group/projectlist flex w-[calc(100%)] flex-row flex-wrap gap-4" data-testid="recent-projects-list">
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
