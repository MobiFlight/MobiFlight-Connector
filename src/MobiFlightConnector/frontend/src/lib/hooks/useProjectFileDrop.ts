import { useEffect } from "react"
import { useTranslation } from "react-i18next"
import { toast } from "@/components/ui/ToastWrapper"
import { CommandMainMenu } from "@/types/commands"

// Same file types as the File > Open dialog
export const PROJECT_FILE_EXTENSIONS = [".mfproj", ".mcc", ".aic"]

export const isProjectFile = (fileName: string) =>
  PROJECT_FILE_EXTENSIONS.some((extension) =>
    fileName.toLowerCase().endsWith(extension),
  )

const hasFiles = (event: DragEvent) =>
  event.dataTransfer?.types.includes("Files") ?? false

const isModalOpen = () =>
  document.querySelector('[role="dialog"], [role="alertdialog"]') !== null

// Opens a project file that is dropped onto the window, like File > Open.
// Without this, WebView2 navigates away from the app to show the dropped file.
// Pages that must not open projects (e.g. startup) pass openFiles: false and
// only swallow the drop. Drops are also ignored while a modal is open.
export const useProjectFileDrop = ({ openFiles = true } = {}) => {
  const { t } = useTranslation()

  useEffect(() => {
    const handleDragOver = (event: DragEvent) => {
      if (!hasFiles(event)) return
      // Cancelling dragover is what allows the drop
      event.preventDefault()
      event.dataTransfer!.dropEffect = "copy"
    }

    const handleDrop = (event: DragEvent) => {
      if (!hasFiles(event)) return
      event.preventDefault()

      if (!openFiles || isModalOpen()) return

      const projectFile = Array.from(event.dataTransfer!.files).find((file) =>
        isProjectFile(file.name),
      )

      if (!projectFile) {
        toast({
          id: "project-file-drop-unsupported",
          title: t("Notifications.ProjectFileDropUnsupported.Title"),
          description: t(
            "Notifications.ProjectFileDropUnsupported.Description",
            { extensions: PROJECT_FILE_EXTENSIONS.join(", ") },
          ),
        })
        return
      }

      // The browser doesn't know the file's path, so the file is passed
      // to the host, which adds the path to the message.
      const message: CommandMainMenu = {
        key: "CommandMainMenu",
        payload: { action: "file.open" },
      }
      window.chrome?.webview?.postMessageWithAdditionalObjects?.(message, [
        projectFile,
      ])
    }

    window.addEventListener("dragover", handleDragOver)
    window.addEventListener("drop", handleDrop)
    return () => {
      window.removeEventListener("dragover", handleDragOver)
      window.removeEventListener("drop", handleDrop)
    }
  }, [t, openFiles])
}
