import { Blink } from "@/types/modifier"
import { Button } from "@/components/ui/button"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Label } from "@/components/ui/label"
import Input from "@/components/Input"
import { validateNumberInput } from "@/lib/hooks/useDraftCommitInput"
import { Trans, useTranslation } from "react-i18next"
import { IconPlus, IconTrash } from "@tabler/icons-react"

const blinkValuesFromArray = (
  array: number[],
): { on: number; off: number }[] => {
  const blinkValues: { on: number; off: number }[] = []
  for (let i = 0; i < array.length; i += 2) {
    const on = array[i]
    const off = array[i + 1]
    blinkValues.push({
      on: on,
      off: off,
    })
  }
  return blinkValues
}

export const BlinkPanelTrigger = ({ modifier }: { modifier: Blink }) => {
  const { t } = useTranslation()
  const blinkValues: { on: number; off: number }[] = blinkValuesFromArray(
    modifier.OnOffSequence,
  )

  return (
    <div className="flex flex-row items-center gap-2">
      <div className="text-md w-32 px-2 text-left font-semibold">
        {t("Dialog.Modifiers.Type.Blink.Label")}
      </div>
      <div className="flex flex-row items-center gap-2 text-sm">
        <Trans
          i18nKey="Dialog.Modifiers.Type.Blink.Summary"
          values={{
            blinkValue: modifier.BlinkValue,
            on: blinkValues.length > 0 ? `${blinkValues[0].on}` : "",
            off: blinkValues.length > 0 ? `${blinkValues[0].off}` : "",
          }}
          components={{
            badge: <Badge variant={"secondary"} />,
            span: <span className="text-sm font-semibold" />,
          }}
        />
      </div>
    </div>
  )
}

export const BlinkPanelContent = ({
  modifier,
  onChange,
}: {
  modifier: Blink
  onChange: (updated: Blink) => void
}) => {
  const { t } = useTranslation()
  const blinkValues: { on: number; off: number }[] = blinkValuesFromArray(
    modifier.OnOffSequence,
  )

  const convertToFlatArray = (updateValues: { on: number; off: number }[]) => {
    const flatArray: number[] = []
    updateValues.forEach(({ on, off }) => {
      flatArray.push(on, off)
    })
    return flatArray
  }

  const addBlink = () => {
    const lastMapping =
      blinkValues.length > 0
        ? blinkValues[blinkValues.length - 1]
        : { on: 0, off: 0 }
    const newMapping = {
      on: lastMapping.on,
      off: lastMapping.off,
    }
    const updatedValues = [...blinkValues, newMapping]
    onChange({ ...modifier, OnOffSequence: convertToFlatArray(updatedValues) })
  }

  const deleteBlink = (index: number) => {
    const updatedValues = blinkValues.filter((_, i) => i !== index)
    onChange({ ...modifier, OnOffSequence: convertToFlatArray(updatedValues) })
  }

  return (
    <>
      <div className="text-muted-foreground text-sm">
        {t("Dialog.Modifiers.Type.Blink.Description")}
      </div>
      <div className="flex flex-col gap-1 pr-12">
        <Label htmlFor="alternate" className="text-md font-semibold">
          {t("Dialog.Modifiers.Type.Blink.AlternateValue")}
        </Label>
        <Input
          className="w-12"
          id="alternate"
          value={modifier.BlinkValue}
          onChange={(value) => onChange({ ...modifier, BlinkValue: value })}
        />
      </div>
      <div>
        <div className="text-md font-semibold">
          {t("Dialog.Modifiers.Type.Blink.BlinkSequence")}
        </div>
        <Table className="">
          <TableHeader>
            <TableRow>
              <TableHead>{t("Dialog.Modifiers.Type.Blink.On")}</TableHead>
              <TableHead>{t("Dialog.Modifiers.Type.Blink.Off")}</TableHead>
              <TableHead>
                <span className="sr-only">
                  {t("Dialog.Modifiers.Type.Blink.Action")}
                </span>
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {blinkValues.map((range, index) => {
              const { on, off } = range
              return (
                <TableRow key={index}>
                  <TableCell className="px-2 py-1">
                    <Input
                      id="on"
                      value={on ?? 500}
                      validateOnCommit={validateNumberInput}
                      onChange={(value) =>
                        onChange({
                          ...modifier,
                          OnOffSequence: convertToFlatArray(
                            blinkValues.map((v, i) =>
                              i === index
                                ? {
                                    on: value,
                                    off: v.off,
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
                      id="off"
                      value={off ?? 500}
                      validateOnCommit={validateNumberInput}
                      onChange={(value) =>
                        onChange({
                          ...modifier,
                          OnOffSequence: convertToFlatArray(
                            blinkValues.map((v, i) =>
                              i === index
                                ? {
                                    on: v.on,
                                    off: value,
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
                      variant="ghost"
                      size="sm"
                      onClick={() => {
                        deleteBlink(index)
                      }}
                      disabled={blinkValues.length === 1}
                    >
                      <IconTrash />
                      <span className="sr-only">
                        {t("Dialog.Modifiers.Type.Blink.Remove")}
                      </span>
                    </Button>
                  </TableCell>
                </TableRow>
              )
            })}
          </TableBody>
        </Table>
      </div>
      <Button variant="outline" size="sm" onClick={addBlink}>
        <IconPlus />
        {t("Dialog.Modifiers.Type.Blink.Add")}
      </Button>
    </>
  )
}
