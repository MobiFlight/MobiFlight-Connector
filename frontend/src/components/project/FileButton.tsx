import { ConfigFile } from "@/types"
import { VariantProps } from "class-variance-authority"
import {
  IconDotsVertical,
  IconPencil,
  IconTrash,
} from "@tabler/icons-react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandFileContextMenu } from "@/types/commands"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { buttonVariants } from "@/components/ui/variants"
import { useEffect, useRef, useState } from "react"
import { cn } from "@/lib/utils"
import { useTranslation } from "react-i18next"
import { InlineEditLabel, InlineEditLabelRef } from "../InlineEditLabel"
import { useConfigItemDragContext } from "../providers/DragDropProvider"

export interface FileButtonProps extends VariantProps<typeof buttonVariants> {
  file: ConfigFile
  index: number
  selectActiveFile: (index: number) => void
}

const FileButton = ({
  file,
  index,
  variant,
  selectActiveFile: onSelectActiveFile,
}: FileButtonProps) => {

  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()
  const [ label, setLabel ] = useState(file.Label ?? file.FileName)
  const inlineEditRef = useRef<InlineEditLabelRef>(null)

  // Hover timer ref
  const hoverTimeoutRef = useRef<NodeJS.Timeout | null>(null)

  // Get drag state from context
  const { dragState } = useConfigItemDragContext()
  const isDragging = dragState?.isDragging ?? false

  useEffect(() => {
    setLabel(file.Label ?? file.FileName)
  }, [file.Label, file.FileName])

  const isActiveTab = variant === "tabActive"

  const onSave = (newLabel: string) => {
    publish({
      key: "CommandFileContextMenu",
      payload: {
        action: "rename",
        index: index,
        file: {
          ...file,
          Label: newLabel,
        }
      },
    } as CommandFileContextMenu)
  }
  const groupHoverStyle = variant === "tabActive" ? "group-hover:bg-primary group-hover:text-primary-foreground" : "group-hover:bg-accent group-hover:text-accent-foreground"

  const onTabMouseEnter = () => {
    if(!isDragging) return
    // Start timer to switch tabs after 800ms hover
    hoverTimeoutRef.current = setTimeout(() => {
      onSelectActiveFile(index)
    }, 800)
  }  

  const onTabMouseLeave = () => {
    if(!hoverTimeoutRef.current) return

    clearTimeout(hoverTimeoutRef.current)
    hoverTimeoutRef.current = null
  }
    


  return (
    <div className="flex justify-center group" 
         role="tab"
         onMouseEnter={onTabMouseEnter}
         onMouseLeave={onTabMouseLeave}
         >
      <Button
        variant={variant}
        value={file.FileName || ""}
        className={cn(groupHoverStyle, "rounded-r-none border-r-0 rounded-b-none border-b-0")}
        onClick={() => onSelectActiveFile(index)}
      >
        <InlineEditLabel 
          ref={inlineEditRef}
          value={label}
          onSave={onSave}
          disabled={!isActiveTab}
          inputClassName="pt-1"
        />
      </Button>
      <div className="relative">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant={variant} className={cn(groupHoverStyle, "w-8 rounded-l-none p-0 rounded-b-none pb-0 border-l-0 border-b-0")}>
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
    </div>
  )
}

export default FileButton
