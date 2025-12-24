import { cn } from "@/lib/utils"
import { ProjectInfo } from "@/types/project"
import { IconStarFilled, IconStar } from "@tabler/icons-react"
import { HtmlHTMLAttributes } from "react"

export type ProjectFavStarProps = HtmlHTMLAttributes<HTMLDivElement> & {
  summary: ProjectInfo
  variant?: "default" | "small"
}

const ProjectFavStar = ({
  summary,
  variant = "default",
}: ProjectFavStarProps) => {
  const sizeVariant = variant === "small" ? "h-6 w-6 p-0.5" : "h-8 w-8 p-1"

  return (
    <div
      role="button"
      className={cn(
        `flex items-center justify-center rounded-full bg-white shadow-md`,
        sizeVariant,
      )}
    >
      {summary.Favorite ? (
        <IconStarFilled className="fill-amber-400 stroke-none" />
      ) : (
        <IconStar className="stroke-muted-foreground" />
      )}
    </div>
  )
}

export default ProjectFavStar
