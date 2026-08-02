const textInputTypes = new Set([
  "email",
  "number",
  "password",
  "search",
  "tel",
  "text",
  "url",
])

export type TextContextMenuTargetState =
  | {
      kind: "input"
      inputType: string
      disabled: boolean
      readOnly: boolean
    }
  | { kind: "textarea"; disabled: boolean; readOnly: boolean }
  | { kind: "contenteditable" }
  | { kind: "other" }

export function allowsNativeTextContextMenu(
  target: TextContextMenuTargetState,
): boolean {
  switch (target.kind) {
    case "input":
      // Read-only text still needs native Copy and Select All. Chromium disables
      // mutating commands such as Cut and Paste for us.
      return !target.disabled && textInputTypes.has(target.inputType)
    case "textarea":
      return !target.disabled
    case "contenteditable":
      return true
    default:
      return false
  }
}

function describeContextMenuTarget(
  target: EventTarget | null,
): TextContextMenuTargetState {
  if (target instanceof HTMLInputElement) {
    return {
      kind: "input",
      inputType: target.type,
      disabled: target.disabled,
      readOnly: target.readOnly,
    }
  }

  if (target instanceof HTMLTextAreaElement) {
    return {
      kind: "textarea",
      disabled: target.disabled,
      readOnly: target.readOnly,
    }
  }

  const element =
    target instanceof HTMLElement
      ? target
      : target instanceof Element
        ? target.parentElement
        : null

  if (element?.isContentEditable) {
    return { kind: "contenteditable" }
  }

  return { kind: "other" }
}

export function isNativeTextContextMenuTarget(
  target: EventTarget | null,
): boolean {
  return allowsNativeTextContextMenu(describeContextMenuTarget(target))
}

export function enforceTextContextMenuPolicy(event: MouseEvent) {
  const target = event.composedPath()[0] ?? event.target

  if (!isNativeTextContextMenuTarget(target)) {
    event.preventDefault()
  }
}
