import { Input } from "@/components/ui/input"
import { Interpolation } from "@/types/modifier"
import { Switch } from "@/components/ui/switch"
import { Button } from "@/components/ui/button"
import {
  IconChevronDown,
  IconGripVertical,
  IconPlus,
  IconTrash,
} from "@tabler/icons-react"

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible"
import { useState } from "react"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"

type InterpolationPanelProps = {
  variant: "summary" | "editor"
  modifier: Interpolation
  onChange: (updated: Interpolation) => void
  onDelete: () => void
}

const InterpolationPanel = ({
  variant,
  modifier,
  onChange,
  onDelete,
}: InterpolationPanelProps) => {
  const [open, setOpen] = useState(false)
  const rangeStart = Object.keys(modifier.Values)
  const interpolationValues = rangeStart.map((start) => ({
    start: parseInt(start),
    end: modifier.Values[parseInt(start)],
  }))

  const convertToRecord = (updateValues: { start: number; end: number }[]) => {
    return updateValues.reduce(
      (acc, { start, end }) => {
        acc[start] = end
        return acc
      },
      {} as Record<number, number>,
    )
  }

  const addMapping = () => {
    const lastMapping =
      interpolationValues.length > 0
        ? interpolationValues[interpolationValues.length - 1]
        : { start: 0, end: 0 }
    const newMapping = {
      start: lastMapping.start * 2,
      end: lastMapping.end * 2,
    }
    const updatedValues = [...interpolationValues, newMapping]
    onChange({ ...modifier, Values: convertToRecord(updatedValues) })
  }

  const removeMapping = (index: number) => {
    const updatedValues = interpolationValues.filter((_, i) => i !== index)
    onChange({ ...modifier, Values: convertToRecord(updatedValues) })
  }

  return variant === "summary" ? (
    <Badge className="bg-sky-500">Interpolation</Badge>
  ) : (
    <div className="flex flex-col gap-2 rounded-md border px-1 py-0.5">
      <Collapsible
        open={open}
        onOpenChange={setOpen}
        className="flex flex-col gap-2"
      >
        <div className="flex flex-row items-center gap-4">
          <IconGripVertical className="stroke-2" />
          <Switch
            id="active"
            checked={modifier.Active}
            onCheckedChange={(checked) =>
              onChange({ ...modifier, Active: checked })
            }
          />
          <CollapsibleTrigger className="flex grow flex-row items-center justify-between">
            <div className="text-md px-2 font-semibold">Interpolation</div>
            <Button onClick={() => {}} size={"sm"} variant="ghost">
              <IconChevronDown />
            </Button>
          </CollapsibleTrigger>
          <Button onClick={onDelete} size={"sm"} variant="ghost">
            <IconTrash />
          </Button>
        </div>
        <CollapsibleContent className="data-[state=closed]:animate-collapsible-up data-[state=open]:animate-collapsible-down overflow-hidden flex flex-col gap-4 border-t pt-2 pr-12 pl-12 pb-2">
          <div className="text-muted-foreground text-sm">
            Define mappings between input and output values. Values outside the
            range are clamped.
          </div>
          <div>
            <div className="text-md font-semibold">Mappings</div>
            <Table className="">
              <TableHeader>
                <TableRow>
                  <TableHead>From</TableHead>
                  <TableHead>To</TableHead>
                  <TableHead>Action</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {interpolationValues.map((range, index) => {
                  const { start, end } = range
                  return (
                    <TableRow key={index}>
                      <TableCell className="px-2 py-1">
                        <Input
                          id="start"
                          value={start ?? ""}
                          onChange={(e) =>
                            onChange({
                              ...modifier,
                              Values: convertToRecord(
                                interpolationValues.map((v, i) =>
                                  i === index
                                    ? {
                                        start: parseInt(e.target.value),
                                        end: v.end,
                                      }
                                    : v,
                                ),
                              ),
                            })
                          }
                        />
                      </TableCell>
                      <TableCell className="px-2 py-1">
                        <Input
                          id="end"
                          value={end ?? ""}
                          onChange={(e) =>
                            onChange({
                              ...modifier,
                              Values: convertToRecord(
                                interpolationValues.map((v, i) =>
                                  i === index
                                    ? {
                                        start: v.start,
                                        end: parseInt(e.target.value),
                                      }
                                    : v,
                                ),
                              ),
                            })
                          }
                        />
                      </TableCell>
                      <TableCell className="px-2 py-1">
                        <Button
                          onClick={() => removeMapping(index)}
                          size={"sm"}
                          variant="ghost"
                        >
                          <IconTrash />
                        </Button>
                      </TableCell>
                    </TableRow>
                  )
                })}
              </TableBody>
            </Table>
          </div>
          <div className="pl-2">
            <Button variant="outline" size="sm" onClick={addMapping}>
              <IconPlus />
              Add mapping
            </Button>
          </div>
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
export default InterpolationPanel
