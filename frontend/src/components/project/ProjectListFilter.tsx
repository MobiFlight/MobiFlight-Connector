import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { ComponentProps } from "react"
import { useTranslation } from "react-i18next"
import { useSearchParams } from "react-router"

export type ProjectListFilterProps = ComponentProps<"div">

const ProjectListFilter = (props: ProjectListFilterProps) => {
  const [searchParams, setSearchParams] = useSearchParams()
  const activeFilter = searchParams.get("projects_filter") || "all"
  const activeTextFilter = searchParams.get("projects_text") || ""
  const { t } = useTranslation()

  const handleFilterChange = (filter: string) => {
    setSearchParams({
      projects_filter: filter,
      projects_text: activeTextFilter,
    })
  }

  return (
    <div
      className="flex flex-row gap-2"
      data-testid="recent-projects-filter-bar"
      {...props}
    >
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
  )
}

export default ProjectListFilter
