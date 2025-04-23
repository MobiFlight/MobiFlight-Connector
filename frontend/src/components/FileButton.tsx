import { ConfigFile } from "@/types"
import { Button } from "./ui/button"
import { VariantProps } from "class-variance-authority"
import { buttonVariants } from "./ui/variants"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "./ui/dropdown-menu"
import { IconChevronDown, IconEdit, IconFileExport, IconTrash } from "@tabler/icons-react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandFileContextMenu } from "@/types/commands"

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

  return (
    <div className="flex justify-center">
    <Button
      variant={variant}
      value={file.FileName}
      className="h-8 rounded-r-none border-r-0"
      onClick={() => onSelectActiveFile(index)}
    >
      {file.Label ?? file.FileName}
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
                publish({
                  key: "CommandFileContextMenu",
                  payload: {  
                    action: "rename",
                    file: file,
                  },
                } as CommandFileContextMenu)
              }}
            ><IconEdit/>Rename</DropdownMenuItem>
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
            ><IconTrash />Remove</DropdownMenuItem>
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
            ><IconFileExport />Export</DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
    </div>
    </div>
  )
}

export default FileButton
