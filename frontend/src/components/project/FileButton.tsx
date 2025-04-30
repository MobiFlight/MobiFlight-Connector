import { ConfigFile } from "@/types"
import { VariantProps } from "class-variance-authority"
import {
  IconDotsVertical,
  IconEdit,
  IconFileExport,
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
import { Input } from "../ui/input"
import { useEffect, useRef, useState } from "react"
import { cn } from "@/lib/utils"
import { useTranslation } from "react-i18next"

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
  const [ isEditing, setIsEditing ] = useState(false)
  const [ label, setLabel ] = useState(file.Label ?? file.FileName)
  const inputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    if (isEditing && inputRef.current) {
      setTimeout(() => { inputRef?.current?.focus() }, 300)
    }
  }, [isEditing])

  useEffect(() => {
    setLabel(file.Label ?? file.FileName)
  }, [file.Label, file.FileName])

  const groupHoverStyle = variant === "default" ? "group-hover:bg-primary/90" : "group-hover:bg-accent group-hover:text-accent-foreground"

  return (
    <div className="flex justify-center group" role="tab">
      <Button
        variant={variant}
        value={file.FileName}
        className="rounded-r-none border-r-0 rounded-b-none border-b-0"
        onClick={() => onSelectActiveFile(index)}
        onDoubleClick={(e) => {
          e.stopPropagation()
          e.preventDefault()
          setIsEditing(true)
        }}
      >
        { !isEditing 
          ? 
          (label)
          :
          <Input
            ref={inputRef}
            className="bg-transparent border-none h-6 focus-visible:ring-0 focus-visible:border-none focus-visible:ring-offset-0 rounded-0"
            value={label}
            onChange={(e) => {
              setLabel(e.target.value)
            }}
            onBlur={() => {
              setIsEditing(false)
            }}
            
            onKeyDown={(e) => {
              e.stopPropagation()

              if (e.key === "Escape") {
                setIsEditing(false)
                setLabel(file.Label ?? file.FileName)
              }
              if (e.key === "Enter") {
                setIsEditing(false)
                setLabel(e.currentTarget.value)
                publish({
                  key: "CommandFileContextMenu",
                  payload: {
                    action: "rename",
                    index: index,
                    file: {
                      ...file,
                      Label: e.currentTarget.value,
                    }
                  },
                } as CommandFileContextMenu)
              }
            }}
          />
        }
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
                setIsEditing(true)
              }}
            >
              <IconEdit />
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
            <DropdownMenuItem
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
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}

export default FileButton
