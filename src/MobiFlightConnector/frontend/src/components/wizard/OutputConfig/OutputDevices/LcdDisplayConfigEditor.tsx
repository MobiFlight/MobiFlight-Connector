import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import { cn } from "@/lib/utils"
import { useConfigItemStateStore } from "@/stores/configItemStateStore"
import { useProjectStore } from "@/stores/projectStore"
import { ConfigReference, LcdDisplayConfig } from "@/types/config"
import { DeviceReference } from "@/types/controller"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export type LcdEditorView = "template" | "visualization"

// the placeholder for the value of this config
const OWN_VALUE_PLACEHOLDER = "$"

// one colour per value: "$" first, then the config references in order
const COLORS = [
  "bg-sky-500/60",
  "bg-amber-500/60",
  "bg-emerald-500/60",
  "bg-fuchsia-500/60",
  "bg-rose-500/60",
  "bg-violet-500/60",
  "bg-teal-500/60",
  "bg-orange-500/60",
]

type LcdValue = {
  placeholder: string
  name: string
  value: string
  color: string
}

// every line gets exactly `cols` characters, the grid exactly `rows` lines
const toGrid = (lines: string[], rows: number, cols: number) =>
  Array.from({ length: rows }, (_, row) =>
    (lines[row] ?? "").padEnd(cols).slice(0, cols),
  )

const setChar = (line: string, col: number, char: string) =>
  line.slice(0, col) + char + line.slice(col + 1)

const fill = (line: string, from: number, to: number, char: string) =>
  line.slice(0, from) + char.repeat(to - from + 1) + line.slice(to + 1)

type LcdCellProps = {
  char: string
  color?: string
  isCaret: boolean
  isSelected: boolean
  onMouseDown: () => void
  onMouseEnter: () => void
  testId: string
}

const LcdCell = ({
  char,
  color,
  isCaret,
  isSelected,
  onMouseDown,
  onMouseEnter,
  testId,
}: LcdCellProps) => (
  <div
    data-testid={testId}
    className={cn(
      "flex h-8 w-6 cursor-text items-center justify-center rounded-xs border border-sky-900 bg-sky-950 font-mono text-lg text-sky-100 select-none",
      color,
      isSelected && "ring-2 ring-white/70",
      isCaret && "border-b-3 border-b-white",
    )}
    onMouseDown={onMouseDown}
    onMouseEnter={onMouseEnter}
  >
    {char}
  </div>
)

type ValueRowProps = {
  value: LcdValue
  onClick: () => void
}

const ValueRow = ({ value, onClick }: ValueRowProps) => (
  <button
    type="button"
    className="hover:bg-accent flex flex-row items-center gap-3 rounded-md border px-3 py-1.5 text-left text-sm"
    onClick={onClick}
  >
    <span
      className={cn(
        "flex h-6 w-6 items-center justify-center rounded-xs font-mono",
        value.color,
      )}
    >
      {value.placeholder}
    </span>
    <span className="grow">{value.name}</span>
    <span className="text-muted-foreground font-mono">{value.value}</span>
  </button>
)

export type LcdDisplayConfigEditorProps = {
  config: LcdDisplayConfig
  device?: DeviceReference
  configRefs: ConfigReference[]
  // the GUID of the config item, used to show its live value
  configId: string
  // the display text rendered by the backend, one string per row
  previewLines: string[] | null
  view: LcdEditorView
  onViewChange: (view: LcdEditorView) => void
  onChange: (config: LcdDisplayConfig) => void
}

export const LcdDisplayConfigEditor = ({
  config,
  device,
  configRefs,
  configId,
  previewLines,
  view,
  onViewChange,
  onChange,
}: LcdDisplayConfigEditorProps) => {
  const { t } = useTranslation("wizard")
  const rows = device?.Capabilities?.Rows ?? 2
  const cols = device?.Capabilities?.Columns ?? 16

  const [caret, setCaret] = useState({ row: 0, col: 0 })
  // column where a selection on the caret row started, null = nothing selected
  const [anchor, setAnchor] = useState<number | null>(null)
  const [dragStart, setDragStart] = useState<{
    row: number
    col: number
  } | null>(null)

  const grid = toGrid(config.Lines ?? [], rows, cols)
  const shownGrid =
    view === "visualization" && previewLines
      ? toGrid(previewLines, rows, cols)
      : grid

  const liveValues = useConfigItemStateStore((s) => s.values)
  const { project, activeConfigFileIndex } = useProjectStore()
  const configItems =
    project?.ConfigFiles[activeConfigFileIndex]?.ConfigItems ?? []

  const values: LcdValue[] = [
    {
      placeholder: OWN_VALUE_PLACEHOLDER,
      name: t("ConfigWizard.Device.LcdDisplay.Editor.ThisConfig"),
      value: liveValues[configId]?.Value ?? "",
      color: COLORS[0],
    },
    ...configRefs
      .filter((ref) => ref.Active)
      .map((ref, index) => ({
        placeholder: ref.Placeholder,
        name: configItems.find((c) => c.GUID === ref.Ref)?.Name ?? ref.Ref,
        value: liveValues[ref.Ref]?.Value ?? ref.TestValue,
        color: COLORS[(index + 1) % COLORS.length],
      })),
  ]
  const colorFor = (char: string) =>
    values.find((v) => v.placeholder === char)?.color

  const selection =
    anchor === null
      ? null
      : { from: Math.min(anchor, caret.col), to: Math.max(anchor, caret.col) }

  const updateLine = (row: number, line: string) => {
    const lines = grid.map((l, i) => (i === row ? line : l))
    onChange({ ...config, Lines: lines.map((l) => l.trimEnd()) })
  }

  const moveCaret = (row: number, col: number, keepSelection: boolean) => {
    const next = {
      row: Math.max(0, Math.min(rows - 1, row)),
      col: Math.max(0, Math.min(cols - 1, col)),
    }
    if (keepSelection && next.row === caret.row) setAnchor(anchor ?? caret.col)
    else setAnchor(null)
    setCaret(next)
  }

  // writes the character into the selection or into the caret cell
  const write = (char: string) => {
    if (selection) {
      updateLine(
        caret.row,
        fill(grid[caret.row], selection.from, selection.to, char),
      )
      moveCaret(caret.row, selection.to + 1, false)
    } else {
      updateLine(caret.row, setChar(grid[caret.row], caret.col, char))
      moveCaret(caret.row, caret.col + 1, false)
    }
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    const { row, col } = caret
    switch (e.key) {
      case "ArrowLeft":
        moveCaret(row, col - 1, e.shiftKey)
        break
      case "ArrowRight":
        moveCaret(row, col + 1, e.shiftKey)
        break
      case "ArrowUp":
        moveCaret(row - 1, col, false)
        break
      case "ArrowDown":
        moveCaret(row + 1, col, false)
        break
      case "Home":
        moveCaret(row, 0, e.shiftKey)
        break
      case "End":
        moveCaret(row, cols - 1, e.shiftKey)
        break
      case "Enter":
        moveCaret(row + 1, 0, false)
        break
      case "Backspace":
        if (selection) {
          updateLine(row, fill(grid[row], selection.from, selection.to, " "))
          moveCaret(row, selection.from, false)
        } else if (col > 0) {
          updateLine(row, setChar(grid[row], col - 1, " "))
          moveCaret(row, col - 1, false)
        }
        break
      case "Delete":
        if (selection) {
          updateLine(row, fill(grid[row], selection.from, selection.to, " "))
          moveCaret(row, selection.from, false)
        } else {
          updateLine(row, setChar(grid[row], col, " "))
        }
        break
      default:
        // a single printable character
        if (e.key.length === 1 && !e.ctrlKey && !e.metaKey) write(e.key)
        else return
    }
    e.preventDefault()
  }

  return (
    <div className="flex flex-col gap-4" data-testid="lcd-editor">
      <div className="flex flex-row items-center justify-between">
        <Label>{t("ConfigWizard.Device.LcdDisplay.Editor.Template")}</Label>
        <div className="flex flex-row items-center gap-2">
          <Switch
            id="lcd-editor-view"
            data-testid="lcd-editor-view"
            checked={view === "visualization"}
            onCheckedChange={(checked) =>
              onViewChange(checked ? "visualization" : "template")
            }
          />
          <Label htmlFor="lcd-editor-view">
            {t("ConfigWizard.Device.LcdDisplay.Editor.Visualization")}
          </Label>
        </div>
      </div>

      <div
        role="textbox"
        aria-label={t("ConfigWizard.Device.LcdDisplay.Editor.Template")}
        tabIndex={0}
        className="focus-visible:ring-ring inline-grid w-fit gap-0.5 rounded-md bg-slate-800 p-3 outline-none focus-visible:ring-2"
        style={{ gridTemplateColumns: `repeat(${cols}, auto)` }}
        onKeyDown={handleKeyDown}
        onMouseUp={() => setDragStart(null)}
        onMouseLeave={() => setDragStart(null)}
      >
        {shownGrid.map((line, row) =>
          Array.from(line).map((char, col) => (
            <LcdCell
              key={`${row}-${col}`}
              testId={`lcd-cell-${row}-${col}`}
              char={char}
              color={colorFor(grid[row][col])}
              isCaret={caret.row === row && caret.col === col}
              isSelected={
                caret.row === row &&
                selection !== null &&
                col >= selection.from &&
                col <= selection.to
              }
              onMouseDown={() => {
                moveCaret(row, col, false)
                setDragStart({ row, col })
              }}
              onMouseEnter={() => {
                if (dragStart?.row !== row) return
                setAnchor(dragStart.col)
                setCaret({ row, col })
              }}
            />
          )),
        )}
      </div>

      <div className="flex flex-col gap-2">
        <Label>{t("ConfigWizard.Device.LcdDisplay.Editor.Values")}</Label>
        {values.map((value) => (
          <ValueRow
            key={value.placeholder}
            value={value}
            onClick={() => write(value.placeholder)}
          />
        ))}
      </div>
    </div>
  )
}
