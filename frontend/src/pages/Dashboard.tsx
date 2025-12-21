// import ControllerMainCard from "@/components/controllers/ControllerMainCard"
import CommunityMainCard from "@/components/community/CommunityMainCard"
import DashboardNav from "@/components/DashboardNav"
import ProjectMainCard from "@/components/project/ProjectMainCard"
import { useParams } from "react-router"

const Dashboard = () => {
  const params = useParams()

  const activeContent = params.content || "project"
  const isProjectActive = activeContent === "project"
  const isCommunityActive = activeContent === "community"

  return (
    <div className="flex h-full flex-col gap-2">
      <div className="flex grow flex-row gap-2">
        <div
          className={`flex w-full flex-col opacity-100 transition-all duration-300 xl:w-2/3 2xl:w-3/4 ${isProjectActive ? "" : "opacity-0 max-xl:hidden"}`}
        >
          <ProjectMainCard />
        </div>
        <div
          className={`w-full opacity-100 transition-all duration-300 xl:block xl:w-1/3 2xl:w-1/4 ${isCommunityActive ? "" : "opacity-0 max-xl:hidden"}`}
        >
          <CommunityMainCard />
        </div>
        {/* <div className="xl:col-span-2 2xl:col-span-2">      
        <ControllerMainCard />
      </div> */}
      </div>
      <DashboardNav
        isProjectActive={isProjectActive}
        isCommunityActive={isCommunityActive}
      />
    </div>
  )
}

export default Dashboard
