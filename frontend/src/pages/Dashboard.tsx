// import ControllerMainCard from "@/components/controllers/ControllerMainCard"
import CommunityMainCard from "@/components/community/CommunityMainCard"
import ProjectMainCard from "@/components/project/ProjectMainCard"
import { Button } from "@/components/ui/button"
import { Link, useParams } from "react-router"

const Dashboard = () => {
  const params = useParams()

  const activeContent = params.content || "project"
  const isProjectActive = activeContent === "project"
  const isCommunityActive = activeContent === "community"

  return (
    <div className="flex h-full flex-col gap-2">
      <div className="flex grow flex-row gap-2">
        <div
          className={`w-full opacity-100 transition-all duration-300 lg:w-2/3 ${isProjectActive ? "" : "max-lg:hidden opacity-0"}`}
        >
          <ProjectMainCard />
        </div>
        <div
          className={`w-full opacity-100 transition-all duration-300 lg:block lg:w-1/3 ${isCommunityActive ? "" : "max-lg:hidden opacity-0"}`}
        >
          <CommunityMainCard />
        </div>
        {/* <div className="xl:col-span-2 2xl:col-span-2">      
        <ControllerMainCard />
      </div> */}
      </div>
      <div className="flex flex-row gap-0 px-4 lg:hidden">
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
    </div>
  )
}

export default Dashboard
