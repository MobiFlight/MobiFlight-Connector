import { Interpolation } from "@/types/modifier"
import { Button } from "@/components/ui/button"
import { IconPlus, IconTrash } from "@tabler/icons-react"

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import Input from "@/components/Input"
import { validateNumberInput } from "@/lib/hooks/useDraftCommitInput"
import { Trans, useTranslation } from "react-i18next"
import { useState } from "react"

export const InterpolationPanelTrigger = ({
  modifier,
}: {
  modifier: Interpolation
}) => {
  const { t } = useTranslation()

  const rangeStart = Object.keys(modifier.Values)
  const interpolationValues = rangeStart.map((start) => ({
    start: parseInt(start),
    end: modifier.Values[parseInt(start)],
  }))

  const summaryInfo = {
    min: Math.min(...interpolationValues.map((v) => v.start)),
    max: Math.max(...interpolationValues.map((v) => v.end)),
    mappings: interpolationValues.length,
  }

  return (
    <div className="flex flex-row items-center gap-2">
      <div className="text-md w-32 px-2 text-left font-semibold">
        {t("Dialog.Modifiers.Type.Interpolation.Label")}
      </div>
      <div className="flex flex-row items-center gap-1 text-sm">
        <Trans
          i18nKey="Dialog.Modifiers.Type.Interpolation.Summary"
          components={{ badge: <Badge variant={"secondary"} /> }}
          values={{
            count: summaryInfo.mappings,
            min: summaryInfo.min,
            max: summaryInfo.max,
          }}
        />
      </div>
    </div>
  )
}

export const InterpolationPanelContent = ({
  modifier,
  onChange,
}: {
  modifier: Interpolation
  onChange: (updated: Interpolation) => void
}) => {
  const { t } = useTranslation()
  const rangeStart = Object.keys(modifier.Values)
  const [interpolationValues, setInterpolationValues] = useState<
    { start: number; end: number }[]
  >(
    rangeStart.map((start) => ({
      start: parseInt(start),
      end: modifier.Values[parseInt(start)],
    })),
  )

  const convertToRecord = (updateValues: { start: number; end: number }[]) => {
    return updateValues.reduce(
      (acc, { start, end }) => {
        acc[start] = end
        return acc
      },
      {} as Record<number, number>,
    )
  }

  const removeMapping = (index: number) => {
    const updatedValues = interpolationValues.filter((_, i) => i !== index)
    setInterpolationValues(updatedValues)
    onChange({ ...modifier, Values: convertToRecord(updatedValues) })
  }

  const updateFromValue = (value: number, index: number) => {
    const updatedValues = interpolationValues.map((v, i) =>
      i === index
        ? {
            start: value,
            end: v.end,
          }
        : v,
    )

    setInterpolationValues(updatedValues)
    onChange({
      ...modifier,
      Values: convertToRecord(updatedValues),
    })
  }

  const updateToValue = (value: number, index: number) => {
    const updatedValues = interpolationValues.map((v, i) =>
      i === index
        ? {
            start: v.start,
            end: value,
          }
        : v,
    )

    setInterpolationValues(updatedValues)
    onChange({
      ...modifier,
      Values: convertToRecord(updatedValues),
    })
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
    setInterpolationValues(updatedValues)
    onChange({ ...modifier, Values: convertToRecord(updatedValues) })
  }

  return (
    <>
      <div className="text-muted-foreground text-sm">
        {t("Dialog.Modifiers.Type.Interpolation.Description")}
      </div>
      <div>
        <div className="text-md font-semibold">
          {t("Dialog.Modifiers.Type.Interpolation.Mappings")}
        </div>
        <Table className="">
          <TableHeader>
            <TableRow>
              <TableHead>
                {t("Dialog.Modifiers.Type.Interpolation.From")}
              </TableHead>
              <TableHead>
                {t("Dialog.Modifiers.Type.Interpolation.To")}
              </TableHead>
              <TableHead>
                <span className="sr-only">
                  {t("Dialog.Modifiers.Type.Interpolation.Action")}
                </span>
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {interpolationValues.map((range, index) => {
              const { start: from, end: to } = range
              return (
                <TableRow key={index}>
                  <TableCell className="px-2 py-1">
                    <Input
                      id="from"
                      value={from as number}
                      onChange={(value) => updateFromValue(value, index)}
                      validateOnCommit={validateNumberInput}
                    />
                  </TableCell>
                  <TableCell className="px-2 py-1">
                    <Input
                      id="to"
                      value={to as number}
                      onChange={(value) => updateToValue(value, index)}
                      validateOnCommit={validateNumberInput}
                    />
                  </TableCell>
                  <TableCell className="px-2 py-1">
                    <Button
                      onClick={() => removeMapping(index)}
                      size={"sm"}
                      variant="ghost"
                      disabled={interpolationValues.length <= 2}
                    >
                      <IconTrash />
                      <span className="sr-only">
                        {t("Dialog.Modifiers.Type.Interpolation.Remove")}
                      </span>
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
          {t("Dialog.Modifiers.Type.Interpolation.Add")}
        </Button>
      </div>
    </>
  )
}
