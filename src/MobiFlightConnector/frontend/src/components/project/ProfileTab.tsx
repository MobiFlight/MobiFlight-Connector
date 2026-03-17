import { ConfigFile } from "@/types"
import { VariantProps } from "class-variance-authority"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandFileContextMenu } from "@/types/commands"
import { Button } from "@/components/ui/button"
import { buttonVariants } from "@/components/ui/variants"
import { forwardRef, useCallback, useEffect, useRef, useState } from "react"
import { cn } from "@/lib/utils"
import { InlineEditLabel, InlineEditLabelRef } from "../InlineEditLabel"
import { useDroppable } from "@dnd-kit/core"
import ProfileTabContextMenu from "@/components/project/ProfileTab/ProfileTabContextMenu"

export interface ProfileTabProps extends VariantProps<typeof buttonVariants> {
  file: ConfigFile
  index: number
  selectActiveFile: (index: number) => void
  resizeCallback?: () => void
}

export const ProfileTab = forwardRef<HTMLDivElement, ProfileTabProps>(
  (
    {
      file,
      index,
      variant,
      selectActiveFile: onSelectActiveFile,
      resizeCallback,
    }: ProfileTabProps,
    ref,
  ) => {
    const { publish } = publishOnMessageExchange()
    const inlineEditRef = useRef<InlineEditLabelRef>(null)
    const buttonRef = useRef<HTMLButtonElement>(null)

    const label = file.Label ?? file.FileName
    const [optimisticLabel, setOptimisticLabel] = useState(label)

    const isActiveTab = variant === "tabActive"

    useEffect(() => {
      setOptimisticLabel(label)
    }, [label])

    const onSave = (newLabel: string) => {
      setOptimisticLabel(newLabel)
      publish({
        key: "CommandFileContextMenu",
        payload: {
          action: "rename",
          index: index,
          file: {
            ...file,
            Label: newLabel,
          },
        },
      } as CommandFileContextMenu)
    }

    const groupHoverStyle =
      variant === "tabActive"
        ? "group-hover:bg-primary group-hover:text-primary-foreground"
        : "group-hover:bg-accent group-hover:text-accent-foreground"

    // this ensures correct text color
    // when we are in edit mode
    // but hover outside of the input field, e.g. the menu button
    const groupHoverInputStyle = "group-hover:text-foreground"

    const { setNodeRef } = useDroppable({
      id: `file-button-${index}`,
      data: {
        type: `tab`,
        index: index,
      },
    })

    const maxInputWidth = "max-w-60 xl:max-w-70 3xl:max-w-80"
    const labelClassName = `truncate transition-all duration-300 ease-in-out ${maxInputWidth}`

    const combinedRef = useCallback(
      (node: HTMLDivElement | null) => {
        setNodeRef(node)
        if (typeof ref === "function") ref(node)
        else if (ref) ref.current = node
      },
      [ref, setNodeRef],
    )

    useEffect(() => {
      const resizeObserver = new ResizeObserver(() => {
        resizeCallback?.()
      })

      const button = buttonRef.current
      if (button) {
        resizeObserver.observe(button)
      }

      return () => {
        if (button) {
          resizeObserver.unobserve(button)
        }
      }
    }, [resizeCallback])

    return (
      <div
        className={`group flex flex-row justify-center border-b`}
        ref={combinedRef}
        role="tab"
        aria-selected={isActiveTab}
        title={optimisticLabel}
      >
        <Button
          ref={buttonRef}
          variant={variant}
          value={optimisticLabel}
          className={cn(
            groupHoverStyle,
            "rounded-r-none rounded-b-none border-r-0",
            maxInputWidth,
          )}
          onClick={() => onSelectActiveFile(index)}
        >
          <InlineEditLabel
            labelClassName={labelClassName}
            inputClassName={groupHoverInputStyle}
            ref={inlineEditRef}
            value={optimisticLabel}
            onSave={onSave}
            disabled={!isActiveTab}
          />
        </Button>
        <ProfileTabContextMenu
          variant={variant}
          groupHoverStyle={groupHoverStyle}
          index={index}
          file={file}
          inlineEditRef={inlineEditRef}
        />
      </div>
    )
  },
)

ProfileTab.displayName = "ProfileTab"
