import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { useProjectStore } from "@/stores/projectStore"
import { Precondition } from "@/types/config"
import { IconEdit, IconPlus, IconTallymark2 } from "@tabler/icons-react"

export type PreconditionPanelProps = {
  preconditions: Precondition[] // Replace with actual type of preconditions
  variant: "summary" | "details"
  openDetailsPanel: () => void
}

const PreconditionPanel = ({
  preconditions,
  variant,
  openDetailsPanel,
}: PreconditionPanelProps) => {
  const { project, activeConfigFileIndex } = useProjectStore()
  const maxDisplayCount = 2

  const preconditionIds = preconditions
    .filter((precondition) => precondition.Ref !== undefined)
    .map((precondition) => precondition.Ref) as string[]

  const preconditionConfigs =
    project?.ConfigFiles[activeConfigFileIndex].ConfigItems.filter((item) =>
      preconditionIds.includes(item.GUID),
    ).map((item) => ({ ref: item.GUID, name: item.Name })) || []

  return variant === "summary" ? (
    <Card>
      <CardContent className="flex flex-col gap-2 pt-4">
        <div className="text-lg font-semibold">Preconditions (optional)</div>
        {preconditions.length > 0 ? (
          <div className="flex flex-col gap-2">
            <div className="text-muted-foreground flex flex-row gap-2 py-2">
              {preconditions
                .slice(0, maxDisplayCount)
                .map((precondition, index) => {
                  const label =
                    preconditionConfigs.find(
                      (config) => config.ref === precondition.Ref,
                    )?.name ?? precondition.Ref

                  const color = {
                    variable: "border-orange-400",
                    config: "border-blue-400",
                  } as Record<string, string>

                  const isLast = index === preconditions.slice(0, maxDisplayCount).length - 1 

                  return (
                    <>
                      <Badge
                        key={index}
                        variant="outline"
                        className={`px-4 ${color[precondition.Type]} flex flex-row items-center gap-1`}
                      >
                        <span className="max-w-30 truncate text-sm whitespace-nowrap">
                          {label}
                        </span>
                        <span className="text-sm">{precondition.Operand}</span>
                        <span className="text-sm">{precondition.Value}</span>
                      </Badge>
                      {!isLast && (
                        <Badge variant="secondary" className="px-2">
                          {precondition.Logic == "and" ? (
                            <IconPlus size={10} />
                          ) : (
                            <IconTallymark2 size={10} />
                          )}
                        </Badge>
                      )}
                    </>
                  )
                })}
              {preconditions.length > maxDisplayCount && (
                <Badge variant="outline" className="px-4">
                  <span className="text-sm">
                    +{preconditions.length - maxDisplayCount} more
                  </span>
                </Badge>
              )}
            </div>
            <Button variant="outline" onClick={openDetailsPanel}>
              <IconEdit className="" />
              Preconditions
            </Button>
          </div>
        ) : (
          <div className="flex flex-col gap-2">
            <div className="text-muted-foreground text-sm">
              The preconditions define conditions that must be met before the
              action can be executed.
            </div>
            <Button variant="outline" onClick={openDetailsPanel}>
              <IconPlus className="" />
              Preconditions
            </Button>
          </div>
        )}
      </CardContent>
    </Card>
  ) : (
    <div className="flex flex-col gap-4">
      <div className="text-lg font-semibold">Preconditions</div>
      <div className="text-muted-foreground text-sm">
        The preconditions define conditions that must be met before the action
        can be executed.
      </div>
    </div>
  )
}
export default PreconditionPanel
