import ConfirmationDialog from "@/components/ConfirmationDialog"
import ProjectCard from "@/components/project/ProjectCard"
import { ProjectCreateButton } from "@/components/project/ProjectCreateButton"
import ProjectList from "@/components/project/ProjectList"
import LoaderOverlay from "@/components/tables/config-item-table/LoaderOverlay"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { useAsynchronous } from "@/lib/hooks/useAsynchronous"
import useMessageExchange from "@/lib/hooks/useMessageExchange"
import { useProjectStore } from "@/stores/projectStore"
import { useRecentProjects } from "@/stores/settingsStore"
import { CommandMainMenu } from "@/types/commands"
import { ProjectInfo } from "@/types/project"
import { useCallback, useState } from "react"
import { useTranslation } from "react-i18next"

const ProjectMainCard = () => {
  const { t } = useTranslation()
  const { publish } = useMessageExchange()
  const { recentProjects } = useRecentProjects()
  const { project, hasChanged, saveStatus, setSaveStatus } = useProjectStore()
  const activeProject = project

  const [isDialogOpen, setIsDialogOpen] = useState(false)
  const [pendingProject, setPendingProject] = useState<ProjectInfo | null>(null)

  const { waitForSaveStatus } = useAsynchronous()

  const loadProject = useCallback(
    (project: ProjectInfo) => {
      publish({
        key: "CommandMainMenu",
        payload: {
          action: "file.recent",
          options: {
            project: project,
          },
        },
      } as CommandMainMenu)
    },
    [publish],
  )

  const handleSaveChanges = async () => {
    // set frontend to saving state
    // this will block the UI from further interactions
    // until the save is complete in the backend,
    // which is indicated by the saveStatus changing
    setSaveStatus("saving")

    // close the dialog
    setIsDialogOpen(false)

    // trigger save command in backend
    publish({
      key: "CommandMainMenu",
      payload: {
        action: "file.save",
      },
    } as CommandMainMenu)

    // wait for save to complete
    waitForSaveStatus().then((result) => {
      // if save was successful,
      // only then go on and load the pending project
      if (result === "success" && pendingProject) {
        loadProject(pendingProject)
      }

      // always clear pending project
      setPendingProject(null)
    })
  }

  const handleDiscardChanges = () => {
    setIsDialogOpen(false)

    if (pendingProject) {
      loadProject(pendingProject)
      setPendingProject(null)
    }
  }

  const confirmLoadProject = (project: ProjectInfo) => {
    if (hasChanged) {
      // display confirmation dialog
      setPendingProject(project)
      setIsDialogOpen(true)
      return
    }
    loadProject(project)
  }

  const showRecentProjects = recentProjects.length > 0

  return (
    <Card
      className="border-shadow-none flex grow flex-col border-none shadow-none"
      data-testid="project-main-card"
    >
      <CardHeader className="">
        <div className="flex flex-row items-center justify-between">
          <div className="flex flex-col gap-2">
            <CardTitle>
              <h2>{t("Project.Card.Main.Title")}</h2>
            </CardTitle>
            <CardDescription>
              {t("Project.Card.Main.Description")}
            </CardDescription>
          </div>
          {showRecentProjects && <ProjectCreateButton />}
        </div>
      </CardHeader>
      <CardContent className="flex grow flex-col">
        {showRecentProjects ? (
          <div className="flex grow flex-row gap-8">
            <div className="flex min-w-96 flex-col gap-4">
              <div>
                {activeProject ? (
                  <h3 className="text-lg font-semibold">
                    {t("Project.Card.Main.CurrentProject")}
                  </h3>
                ) : (
                  <h3 className="text-lg font-semibold">
                    {t("Project.Card.Main.NoActiveProject")}
                  </h3>
                )}
              </div>
              {activeProject ? (
                <ProjectCard
                  summary={activeProject}
                  className="w-96 max-w-96"
                />
              ) : (
                <div className="border-primary/25 bg-card space-y-2 rounded-xl border p-4 shadow-md transition-all duration-200 ease-in-out hover:shadow-lg">
                  <div className="flex flex-col gap-4">
                    <div className="text-muted-foreground flex flex-row items-center justify-items-center gap-2">
                      {t("Project.Card.Main.NoActiveProject")}
                    </div>
                    <ProjectCreateButton />
                  </div>
                </div>
              )}
            </div>
            <div className="flex grow flex-col gap-4">
              <div className="">
                <h3 className="text-lg font-semibold">
                  {t("Project.Card.Main.AllProjects")}
                </h3>
              </div>
              <ProjectList
                className="grow"
                summarys={recentProjects}
                activeProject={activeProject as ProjectInfo}
                onSelect={(project) => confirmLoadProject(project)}
              />
            </div>
          </div>
        ) : (
          <Card className="w-full">
            <CardContent className="flex flex-col items-center justify-center gap-4 pt-4">
              <div className="text-muted-foreground">
                {t("Project.Card.Main.CreateFirstProject")}
              </div>
              <ProjectCreateButton />
            </CardContent>
          </Card>
        )}
        <ConfirmationDialog
          open={isDialogOpen}
          onOpenChange={setIsDialogOpen}
          saveChanges={handleSaveChanges}
          discardChanges={handleDiscardChanges}
        />
        <LoaderOverlay
          message={t("General.Overlay.SavingChanges")}
          open={saveStatus === "saving"}
        />
      </CardContent>
    </Card>
  )
}

export default ProjectMainCard
