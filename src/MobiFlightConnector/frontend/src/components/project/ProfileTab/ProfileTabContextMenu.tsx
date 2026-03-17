import { InlineEditLabelRef } from "@/components/InlineEditLabel"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { buttonVariants } from "@/components/ui/variants"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { cn } from "@/lib/utils"
import { ConfigFile } from "@/types"
import { CommandFileContextMenu } from "@/types/commands"
import { IconDotsVertical, IconPencil, IconTrash } from "@tabler/icons-react"
import { VariantProps } from "class-variance-authority"
import { RefObject } from "react"
import { useTranslation } from "react-i18next"

export interface ProfileTabContextMenuProps
  extends VariantProps<typeof buttonVariants> {
  index: number
  file: ConfigFile | null
  groupHoverStyle: string
  inlineEditRef: RefObject<InlineEditLabelRef | null>
}

const ProfileTabContextMenu = ({
  variant,
  groupHoverStyle,
  index,
  file,
  inlineEditRef,
}: ProfileTabContextMenuProps) => {
  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()

  return (
    <div className="relative">
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant={variant}
            className={cn(
              groupHoverStyle,
              "w-8 rounded-l-none rounded-b-none border-l-0 p-0 pb-0",
            )}
          >
            <span className="sr-only">{t("General.Action.OpenMenu")}</span>
            <IconDotsVertical className="h-4 w-4" />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuItem
            onClick={() => {
              inlineEditRef.current?.startEditing()
            }}
          >
            <IconPencil />
            {t("Project.File.Action.Rename")}
          </DropdownMenuItem>
          <DropdownMenuItem
            onClick={() => {
              publish({
                key: "CommandFileContextMenu",
                payload: {
                  action: "remove",
                  index: index,
                  file: file,
                },
              } as CommandFileContextMenu)
            }}
          >
            <IconTrash />
            {t("Project.File.Action.Remove")}
          </DropdownMenuItem>
          {/* <DropdownMenuItem
              onClick={() => {
                publish({
                  key: "CommandFileContextMenu",
                  payload: {
                    action: "export",
                    index: index,
                    file: file,
                  },
                } as CommandFileContextMenu)
              }}
            >
              <IconFileExport />
              {t("Project.File.Action.Export")}
            </DropdownMenuItem> */}
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  )
}

export default ProfileTabContextMenu
