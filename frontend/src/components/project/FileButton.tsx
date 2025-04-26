import { ConfigFile } from "@/types"
import { VariantProps } from "class-variance-authority"
import {
  IconChevronDown,
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

  const { publish } = publishOnMessageExchange()
  const [ isEditing, setIsEditing ] = useState(false)
  const [ label, setLabel ] = useState(file.Label ?? file.FileName)
  const inputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    if (isEditing && inputRef.current) {
      console.log("focusing")
      setTimeout(() => { inputRef?.current?.focus() }, 200)
    }
  }, [isEditing, label])

  return (
    <div className="flex justify-center">
      <Button
        variant={variant}
        value={file.FileName}
        className="h-8 rounded-r-none border-r-0"
        onClick={() => onSelectActiveFile(index)}
        onDoubleClick={(e) => {
          e.stopPropagation()
          setIsEditing(true)
        }}
      >
        { !isEditing 
          ? 
          (label)
          :
          <Input
            ref={inputRef}
            className="w-auto bg-transparent border-none h-6 focus-visible:ring-0 focus-visible:border-none focus-visible:ring-offset-0 rounded-0"
            value={label}
            onChange={(e) => {
              setLabel(e.target.value)
            }}
            onBlur={() => {
              setIsEditing(false)
            }}
            
            onKeyDown={(e) => {
              if (e.key === "Escape") {
                setIsEditing(false)
                setLabel(file.Label ?? file.FileName)
              }
              if (e.key === "Enter") {
                setIsEditing(false)
                setLabel(e.currentTarget.value)
              }
            }}
          />
        }
      </Button>
      <div className="relative">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant={variant} className="h-8 w-8 rounded-l-none p-0">
              <span className="sr-only">Open menu</span>
              <IconChevronDown className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem
              onClick={() => {
                setIsEditing(true)
              }}
            >
              <IconEdit />
              Rename
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={() => {
                publish({
                  key: "CommandFileContextMenu",
                  payload: {
                    action: "remove",
                    file: file,
                  },
                } as CommandFileContextMenu)
              }}
            >
              <IconTrash />
              Remove
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={() => {
                publish({
                  key: "CommandFileContextMenu",
                  payload: {
                    action: "export",
                    file: file,
                  },
                } as CommandFileContextMenu)
              }}
            >
              <IconFileExport />
              Save
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}

export default FileButton
