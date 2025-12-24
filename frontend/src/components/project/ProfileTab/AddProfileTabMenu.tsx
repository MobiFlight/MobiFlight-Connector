import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { IconFolderPlus, IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

interface AddProfileTabMenuProps extends React.HTMLAttributes<HTMLDivElement> {
  onAddConfigFile: () => void
  onMergeConfigFile: () => void
  onMouseEnter?: () => void
  onMouseLeave?: () => void
}

export const AddProfileTabMenu = ({
  onAddConfigFile,
  onMergeConfigFile,
  onMouseEnter,
  onMouseLeave,
  ...props
} : AddProfileTabMenuProps) => {
  const { t } = useTranslation()

  return (
    <div className="border-muted-foreground/50 border-b px-2" {...props}>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <div
            className="py-1"
            onMouseEnter={onMouseEnter}
            onMouseLeave={onMouseLeave}
          >
            <Button variant={"default"} className="h-8 px-2">
              <span className="sr-only">{t("General.Action.OpenMenu")}</span>
              <IconPlus />
            </Button>
          </div>
        </DropdownMenuTrigger>
        <DropdownMenuContent
          align="start"
          onMouseEnter={onMouseEnter}
          onMouseLeave={onMouseLeave}
        >
          <DropdownMenuItem onClick={onAddConfigFile}>
            <IconPlus />
            {t("Project.File.Action.New")}
          </DropdownMenuItem>
          <DropdownMenuItem onClick={onMergeConfigFile}>
            <IconFolderPlus />
            {t("Project.File.Action.Merge")}
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  )
}
