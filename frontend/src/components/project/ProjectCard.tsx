import { ProjectInfo } from "@/types/project"
import {
  IconChevronRight,
  IconDeviceGamepad2,
  IconDotsVertical,
  IconExclamationCircle,
  IconPlayerPlayFilled,
  IconPlayerStopFilled,
  IconQuestionMark,
  IconSettings,
} from "@tabler/icons-react"
import { Badge } from "@/components/ui/badge"
import { HtmlHTMLAttributes } from "react"
import { cn } from "@/lib/utils"
import { Button } from "../ui/button"
import TwoStateIcon from "../icons/TwoStateIcon"
import ProjectFavStar from "./ProjectFavStar"
import { useNavigate } from "react-router"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { useTranslation } from "react-i18next"
import {
  ProjectModalOptions,
  useProjectModal,
} from "@/lib/hooks/useProjectModal"
import { useExecutionStateStore } from "@/stores/executionStateStore"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import {
  CommandProjectToolbarPayload,
} from "@/types/commands"
import ControllerIcon from "@/components/project/ControllerIcon"
import { useModal } from "@/lib/hooks/useModal"

export type ProjectCardProps = HtmlHTMLAttributes<HTMLDivElement> & {
  summary: ProjectInfo
}
export const ProjectCardTitle = ({
  summary,
  variant,
}: {
  summary: ProjectInfo
  variant?: "default" | "listitem"
}) => {
  const navigate = useNavigate()
  const navigateToProject = () => {
    navigate(`/config`)
  }

  const variants = {
    default: {
      title: "text-xl font-medium truncate",
      button: "p-0 [&_svg]:size-8",
      icon: "h-8",
      options: { role: "button", onClick: navigateToProject },
    },
    listitem: {
      title: "text-lg font-semibold truncate",
      button: "h-6 p-1 [&_svg]:size-6 w-auto",
      icon: "h-6",
      options: {},
    },
  }

  const titleClassName = variants[variant || "default"].title
  const buttonClassName = variants[variant || "default"].button
  const iconClassName = variants[variant || "default"].icon
  const titleOptions = variants[variant || "default"].options

  return (
    <div
      {...titleOptions}
      className="flex flex-row items-center justify-between"
    >
      <div className="flex min-w-0 flex-row items-center justify-start gap-2">
        <h2 className={titleClassName}>{summary.Name}</h2>
      </div>
      {summary && (
        <Button
          variant="ghost"
          className={buttonClassName}
          onClick={navigateToProject}
        >
          <IconChevronRight className={cn("text-primary", iconClassName)} />
        </Button>
      )}
    </div>
  )
}

export const ProjectCardImage = ({
  summary,
  className,
}: HtmlHTMLAttributes<HTMLDivElement> & { summary: ProjectInfo }) => {
  const imageUrl = summary.Thumbnail || `/sim/${summary.Sim?.toLowerCase()}.jpg`

  return summary.Sim ? (
    <div className={cn("bg-accent rounded-lg", className)}>
      <img
        src={imageUrl}
        alt={summary.Name}
        className="h-full w-full rounded-lg object-cover"
      />
    </div>
  ) : (
    <div className={cn("bg-accent rounded-lg opacity-50", className)}>
      <div className="flex h-full w-full items-center justify-center rounded-lg bg-linear-to-br from-indigo-500 from-10% via-sky-500 via-30% to-emerald-500 to-90% dark:from-indigo-500/10 dark:via-sky-500/0 dark:via-70% dark:to-emerald-500/5">
        <IconQuestionMark className="text-background h-full w-full" />
      </div>
    </div>
  )
}

export const ProjectCardStartStopButton = ({
  className,
  ...props
}: HtmlHTMLAttributes<HTMLButtonElement>) => {
  const { publish } = publishOnMessageExchange()
  const { isRunning, isTesting } = useExecutionStateStore()

  const handleMenuItemClick = (payload: CommandProjectToolbarPayload) => {
    publish({
      key: "CommandProjectToolbar",
      payload: payload,
    })
  }

  return (
    <Button
      data-testid="project-card-start-stop-button"
      disabled={isTesting}
      variant="ghost"
      className={cn("text-md gap-1 p-1 [&_svg]:size-8", className)}
      onClick={() =>
        handleMenuItemClick({ action: !isRunning ? "run" : "stop" })
      }
      {...props}
    >
      <TwoStateIcon
        state={isRunning}
        primaryIcon={IconPlayerPlayFilled}
        secondaryIcon={IconPlayerStopFilled}
        primaryClassName={
          !isTesting
            ? "fill-green-600 stroke-green-600"
            : "fill-none stroke-2 stroke-muted-foreground"
        }
        secondaryClassName="fill-red-700 stroke-red-700"
      />
    </Button>
  )
}

const ProjectCard = ({
  summary,
  className,
  ...otherProps
}: ProjectCardProps) => {
  const maxControllersToShow = 6
  const { t } = useTranslation()
  const { showOverlay } = useProjectModal()
  const { showOverlay: showModalOverlay } = useModal()

  const handleEditSettings = () => {
    const options = { mode: "edit", project: summary } as ProjectModalOptions
    showOverlay(options)
  }

  
  const simulatorLabel = summary
    ? summary.Sim
      ? t(`Project.Simulator.${summary.Sim.toLowerCase()}`)
      : t(`Project.Simulator.none`)
    : t(`Project.Simulator.none`)

  const hasBindingIssues = summary.ControllerBindings?.some(
    (binding) =>
      binding.Status === "Missing" || binding.Status === "RequiresManualBind",
  )

  const adjustedMaxControllersToShow = hasBindingIssues
    ? maxControllersToShow - 1
    : maxControllersToShow

  const sortedControllerBindings = summary.ControllerBindings?.sort((a, b) => {
    const priority = {
      RequiresManualBind: 0,
      Missing: 1,
      AutoBind: 2,
      Match: 3,
    }
    return priority[a.Status] - priority[b.Status]
  })
    .slice(0, adjustedMaxControllersToShow)
    .reverse()

  const showMoreControllers =
    summary.ControllerBindings &&
    summary.ControllerBindings.length > adjustedMaxControllersToShow

  return (
    <div
      data-testid="project-card"
      {...otherProps}
      className={cn(
        "border-primary/25 bg-card space-y-2 rounded-xl border p-4 shadow-md transition-all duration-200 ease-in-out hover:shadow-lg",
        className,
      )}
    >
      <ProjectCardTitle summary={summary} />
      {summary ? (
        <div className="flex flex-col gap-4">
          <div className="relative">
            <ProjectCardImage summary={summary} className="h-84" />
            <div className="absolute inset-0 flex items-start justify-start p-4">
              <ProjectFavStar summary={summary} />
            </div>
          </div>
          <div className="flex flex-row">
            <div className="flex flex-1 flex-col gap-4">
              <div className="text-muted-foreground flex flex-row items-center justify-items-center gap-2">
                <Badge key={summary.Sim}>{simulatorLabel}</Badge>
              </div>
              <div className="flex h-11 flex-row items-center gap-0">
                <div
                  className="flex flex-row-reverse items-center -space-x-2 space-x-reverse transition-transform hover:space-x-0.5"
                  data-testid="controller-icons"
                >
                  {showMoreControllers && (
                    <div
                      className="text-foreground flex h-10 w-10 items-center justify-center rounded-full bg-none"
                      data-testid="more-controllers-indicator"
                    >
                      <span className="text-md font-bold">
                        +
                        {summary.ControllerBindings!.length -
                          adjustedMaxControllersToShow}
                      </span>
                    </div>
                  )}
                  {sortedControllerBindings?.map((controllerBinding, index) => {
                    const serial =
                      controllerBinding.BoundController ||
                      controllerBinding.OriginalController ||
                      ""
                    return (
                      controllerBinding.BoundController != "-" && (
                        <ControllerIcon
                          className="transition-all ease-in-out"
                          key={`${controllerBinding.BoundController}-${index}`}
                          serial={serial}
                          status={controllerBinding.Status}
                        />
                      )
                    )
                  })}
                  {hasBindingIssues && (
                    <div
                      className="relative mr-1"
                      role="button"
                      data-testid="controller-binding-issue-icon"
                      title={t("Project.Card.Main.BindingIssuesTooltip")}
                      onClick={() =>
                        showModalOverlay({ route: "/bindings" })
                      }
                    >
                      <IconExclamationCircle className="h-11 w-11 stroke-red-600" />
                    </div>
                  )}
                </div>
              </div>
            </div>
            <div className="flex flex-col items-end justify-between">
              <div className="relative">
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" className="h-8 w-4 px-2">
                      <span className="sr-only">
                        {t("General.Action.OpenMenu")}
                      </span>
                      <IconDotsVertical className="h-4 w-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem onClick={handleEditSettings}>
                      <IconSettings />
                      {t("Project.Toolbar.Settings")}
                    </DropdownMenuItem>
                    <DropdownMenuItem onClick={() => showModalOverlay({ route: "/bindings" })}>
                      <IconDeviceGamepad2 />
                      {t("MainMenu.Extras.ControllerBindings")}
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
              <div className="flex flex-row items-end">
                <ProjectCardStartStopButton />
              </div>
            </div>
          </div>
        </div>
      ) : (
        <div className="flex flex-col gap-4">
          <div>{t("Project.Card.Main.NoActiveProjectHint")}</div>
        </div>
      )}
    </div>
  )
}

export default ProjectCard
