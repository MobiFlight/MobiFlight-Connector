import { ProfileTab } from "./ProfileTab"
import { AddProfileTabMenu } from "./ProfileTab/AddProfileTabMenu"
import { Button } from "../ui/button"
import {
  IconChevronLeft,
  IconChevronRight,
  IconMinusVertical,
} from "@tabler/icons-react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { useProjectStore } from "@/stores/projectStore"
import { useCallback, useEffect, useRef, useState } from "react"
import { useTranslation } from "react-i18next"
import ExecutionToolbar from "../ExecutionToolbar"
import ProjectNameLabel from "./ProjectNameLabel"
import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"
import { useNavigate } from "react-router"
import { Dialog, DialogTitle } from "@radix-ui/react-dialog"
import { DialogContent, DialogHeader } from "@/components/ui/dialog"
import { useWindowSize } from "@/lib/hooks/useWindowSize"
import { useOverflowDetector } from "@/lib/hooks/useOverflowDetector"
import { cn } from "@/lib/utils"

const ProjectPanel = () => {
  const SCROLL_TAB_INTO_VIEW_DELAY_MS = 1500
  const SCROLL_OFFSET = 150

  const overflowRef = useRef<HTMLDivElement | null>(null)

  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()
  const navigate = useNavigate()
  const [isDialogOpen, setIsDialogOpen] = useState(false)
  const { width, height } = useWindowSize()
  const { overflow, checkOverflow } = useOverflowDetector(overflowRef)

  // Track previous width and height
  const prevWindowSizeRef = useRef({ width, height })

  const {
    activeConfigFileIndex,
    setActiveConfigFileIndex,
    project,
    setProject,
    hasChanged,
  } = useProjectStore()

  const configFiles = project?.ConfigFiles ?? []

  useEffect(() => {
    if (project === null) return

    if (project.ConfigFiles === null || project.ConfigFiles.length === 0) return

    if (activeConfigFileIndex >= project.ConfigFiles.length) {
      setActiveConfigFileIndex(project.ConfigFiles.length - 1)
      return
    }
  }, [project, activeConfigFileIndex, setActiveConfigFileIndex])

  const selectActiveFile = useCallback(
    (index: number) => {
      setActiveConfigFileIndex(index)
    },
    [setActiveConfigFileIndex],
  )

  useEffect(() => {
    publishOnMessageExchange().publish({
      key: "CommandActiveConfigFile",
      payload: {
        index: activeConfigFileIndex,
      },
    })
  }, [activeConfigFileIndex])

  useEffect(() => {
    checkOverflow()
  }, [activeConfigFileIndex, checkOverflow])

  const scrollIntoViewTimeoutRef = useRef<NodeJS.Timeout | null>(null)
  const scrollIntervalRef = useRef<NodeJS.Timeout | null>(null)

  const scrollActiveProfileTabIntoView = useCallback(() => {
    if (activeConfigFileIndex === -1) return

    activeProfileTabRef.current?.scrollIntoView({
      behavior: "smooth",
      block: "nearest",
    })
  }, [activeConfigFileIndex])

  const resetScrollActiveProfileTabIntoView = useCallback(() => {
    if (scrollIntoViewTimeoutRef.current == null) return
    clearTimeout(scrollIntoViewTimeoutRef.current)
    scrollIntoViewTimeoutRef.current = null
  }, [])

  const scrollActiveProfileTabIntoViewWithDelay = useCallback(() => {
    if (!activeProfileTabRef.current) return

    resetScrollActiveProfileTabIntoView()

    scrollIntoViewTimeoutRef.current = setTimeout(
      scrollActiveProfileTabIntoView,
      SCROLL_TAB_INTO_VIEW_DELAY_MS,
    )
  }, [scrollActiveProfileTabIntoView, resetScrollActiveProfileTabIntoView])

  useEffect(() => {
    // scroll tab into view
    // when activeConfigFileIndex changes
    resetScrollActiveProfileTabIntoView()
    scrollActiveProfileTabIntoView()
  }, [
    activeConfigFileIndex,
    scrollActiveProfileTabIntoView,
    resetScrollActiveProfileTabIntoView,
  ])

  const handleMouseWheel = (event: React.WheelEvent) => {
    if (overflowRef.current === null) return
    const scrollContainer = overflowRef.current
    if (!scrollContainer) return

    event.stopPropagation()
    const newScrollLeft = scrollContainer.scrollLeft + event.deltaY
    scrollContainer.scrollLeft = newScrollLeft
  }

  useEffect(() => {
    if (
      prevWindowSizeRef.current.width === width &&
      prevWindowSizeRef.current.height === height
    )
      return
    prevWindowSizeRef.current = { width, height }
    // scroll tab into view
    // when window size changes
    scrollActiveProfileTabIntoViewWithDelay()
  }, [width, height, scrollActiveProfileTabIntoViewWithDelay])

  const addConfigFile = () => {
    publishOnMessageExchange().publish({
      key: "CommandAddConfigFile",
      payload: {
        type: "create",
      },
    })

    setTimeout(() => setActiveConfigFileIndex(configFiles.length), 200)
  }

  const mergeConfigFile = () => {
    publish({
      key: "CommandAddConfigFile",
      payload: {
        type: "merge",
      },
    })

    setTimeout(() => setActiveConfigFileIndex(configFiles.length), 200)
  }

  const { dragState } = useConfigItemDragContext()

  const saveChanges = () => {
    // Implement save logic here
    setIsDialogOpen(false)
    publish({
      key: "CommandMainMenu",
      payload: {
        action: "file.save",
      },
    })
    navigate("/home")
  }

  const discardChanges = () => {
    // Implement discard logic here
    setIsDialogOpen(false)

    // Discard a newly created project
    if (project?.FilePath == null) {
      publish({
        key: "CommandDiscardChanges",
        payload: {
          project: project,
        },
      })
      setProject(null)
    } else {
      console.log("Re-opening project to discard changes", project.FilePath)
      publish({
        key: "CommandMainMenu",
        payload: {
          action: "file.recent",
          options: {
            project: project,
          },
        },
      })
    }
    navigate("/home")
  }

  // Hover timer ref
  const hoverTimeoutRef = useRef<NodeJS.Timeout | null>(null)
  const activeProfileTabRef = useRef<HTMLDivElement | null>(null)

  useEffect(() => {
    if (
      dragState?.ui.hoveredTabIndex !== undefined &&
      dragState?.ui.hoveredTabIndex !== -1
    ) {
      hoverTimeoutRef.current = setTimeout(() => {
        selectActiveFile(dragState?.ui.hoveredTabIndex)
      }, 600)
    } else {
      if (hoverTimeoutRef.current) {
        clearTimeout(hoverTimeoutRef.current)
        hoverTimeoutRef.current = null
      }
    }
  }, [dragState?.ui.hoveredTabIndex, selectActiveFile])

  // clear timers on unmount
  useEffect(() => {
    return () => {
      if (scrollIntoViewTimeoutRef.current) {
        clearTimeout(scrollIntoViewTimeoutRef.current)
        scrollIntoViewTimeoutRef.current = null
      }
      if (hoverTimeoutRef.current) {
        clearTimeout(hoverTimeoutRef.current)
        hoverTimeoutRef.current = null
      }
      if (scrollIntervalRef.current) {
        clearInterval(scrollIntervalRef.current)
        scrollIntervalRef.current = null
      }
    }
  }, [])

  const scrollTabs = (direction: "left" | "right") => () => {
    const scrollContainer = overflowRef.current
    if (scrollContainer === null) return

    const scrollAmount = direction === "left" ? -SCROLL_OFFSET : SCROLL_OFFSET
    scrollContainer.scrollBy({ left: scrollAmount, behavior: "smooth" })
  }

  return (
    <div
      className="flex h-11 flex-row gap-0 pt-1 pr-2 pb-0 pl-0"
      data-testid="project-panel"
      onMouseEnter={resetScrollActiveProfileTabIntoView}
      onMouseLeave={scrollActiveProfileTabIntoViewWithDelay}
    >
      <div className="border-muted-foreground/50 flex flex-row items-center gap-2 border-b border-solid px-2">
        <IconChevronLeft
          role="button"
          onClick={() => {
            if (hasChanged) {
              // Prevent navigation if there are unsaved changes
              setIsDialogOpen(true)
              return
            }
            navigate("/home")
          }}
        />
      </div>

      <div className="border-muted-foreground/50 flex flex-row items-center border-0 border-b px-0 pr-2">
        <ProjectNameLabel />
        <IconMinusVertical className="stroke-muted-foreground/50" />
        <ExecutionToolbar />
      </div>

      <div className="border-muted-foreground/50 flex w-9 flex-row items-center justify-end gap-1 border-b px-0 pr-2">
        <Button
          title={t("Project.Tabs.ScrollLeft")}
          variant="ghost"
          className={cn(
            "h-7 p-0 transition-opacity duration-300",
            "group/scroll-left",
            overflow.any ? "w-7 opacity-100" : "scale-0 opacity-0",
          )}
          onClick={scrollTabs("left")}
          data-testid="tab-scroll-left"
        >
          <span className="sr-only">{t("Project.Tabs.ScrollLeft")}</span>
          <IconChevronLeft className="stroke-muted-foreground/50 group-hover/scroll-left:stroke-foreground" />
        </Button>
      </div>

      <div className="relative h-full min-h-10 grow" role="tablist">
        <div
          className="no-scrollbar absolute inset-0 overflow-x-auto overflow-y-hidden"
          ref={overflowRef}
          onWheel={handleMouseWheel}
        >
          <div className="flex h-full flex-row">
            {configFiles?.map((file, index) => {
              return (
                <ProfileTab
                  key={index}
                  ref={
                    index === activeConfigFileIndex ? activeProfileTabRef : null
                  }
                  variant={
                    index === activeConfigFileIndex
                      ? "tabActive"
                      : dragState?.ui.hoveredTabIndex === index
                        ? "tabDragging"
                        : "tabDefault"
                  }
                  file={file}
                  index={index}
                  selectActiveFile={selectActiveFile}
                  resizeCallback={checkOverflow}
                />
              )
            })}
            {!overflow.right && (
              <AddProfileTabMenu
                data-testid="add-profile-tab-menu-regular"
                onAddConfigFile={addConfigFile}
                onMergeConfigFile={mergeConfigFile}
                onMouseEnter={resetScrollActiveProfileTabIntoView}
                onMouseLeave={scrollActiveProfileTabIntoViewWithDelay}
              />
            )}
            <div className="border-muted-foreground/50 grow border-b"></div>
          </div>
        </div>
        {/* Left shadow */}
        {overflow.left && (
          <div
            data-testid="tab-overflow-indicator-left"
            className="from-foreground/20 dark:from-background/50 pointer-events-none absolute top-0 bottom-0 left-0 z-20 w-2 rounded-tl-sm bg-linear-to-r to-transparent pb-1 dark:bottom-0.5 dark:w-3"
          />
        )}
        {/* Right shadow */}
        {overflow.right && (
          <div
            data-testid="tab-overflow-indicator-right"
            className="from-foreground/20 dark:from-background/50 pointer-events-none absolute top-0 right-0 bottom-0 z-200 w-2 bg-linear-to-l to-transparent pb-1"
          />
        )}
      </div>
      {overflow.right && (
        <AddProfileTabMenu
          data-testid="add-profile-tab-menu-overflow"
          onAddConfigFile={addConfigFile}
          onMergeConfigFile={mergeConfigFile}
          onMouseEnter={resetScrollActiveProfileTabIntoView}
          onMouseLeave={scrollActiveProfileTabIntoViewWithDelay}
        />
      )}
      <div
        className={cn(
          "border-muted-foreground/50 flex flex-row items-center justify-start gap-1 border-b px-0 transition-transform duration-300",
          overflow.any ? "w-8" : "w-0",
        )}
      >
        <Button
          title={t("Project.Tabs.ScrollRight")}
          variant="ghost"
          className={cn(
            "h-7 w-7 p-0 transition-opacity duration-300",
            "group/scroll-right",
            overflow.any ? "w-7 opacity-100" : "scale-0 opacity-0",
          )}
          onClick={scrollTabs("right")}
          data-testid="tab-scroll-right"
        >
          <span className="sr-only">{t("Project.Tabs.ScrollRight")}</span>
          <IconChevronRight className="stroke-muted-foreground/50 group-hover/scroll-right:stroke-foreground" />
        </Button>
      </div>
      <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
        <DialogContent>
          <DialogHeader className="sr-only">
            <DialogTitle>{t("Project.UnsavedChanges.Title")}</DialogTitle>
          </DialogHeader>
          <div>{t("Project.UnsavedChanges.Description")}</div>
          <div className="flex flex-row justify-end gap-4">
            <Button variant="ghost" onClick={discardChanges}>
              {t("Project.UnsavedChanges.Discard")}
            </Button>
            <Button onClick={saveChanges}>
              {t("Project.UnsavedChanges.Save")}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  )
}

export default ProjectPanel
