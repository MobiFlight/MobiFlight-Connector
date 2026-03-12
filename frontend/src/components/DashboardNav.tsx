import { Button } from "@/components/ui/button"
import { Link } from "react-router"

export type DashboardNavProps = {
  isProjectActive: boolean
  isCommunityActive: boolean
}

const DashboardNav = ({
  isProjectActive,
  isCommunityActive,
}: DashboardNavProps) => {
  return (
    <div
      className="flex flex-row gap-0 px-4 xl:hidden"
      data-testid="dashboard-nav"
    >
      <Link to="/home/project">
        <Button
          variant={isProjectActive ? "default" : "outline"}
          className="rounded-r-none"
        >
          Project
        </Button>
      </Link>
      <Link to="/home/community">
        <Button
          variant={isCommunityActive ? "default" : "outline"}
          className="rounded-l-none"
        >
          Community
        </Button>
      </Link>
    </div>
  )
}

export default DashboardNav
