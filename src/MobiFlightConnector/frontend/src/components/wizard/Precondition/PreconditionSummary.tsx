import { Badge } from "@/components/ui/badge"
import { preconditionVariants } from "@/components/wizard/variants"
import { IConfigItem, Precondition } from "@/types/config"
import { IconPlus, IconTallymark2 } from "@tabler/icons-react"

type PreconditionSummaryProps = {
  preconditions: Precondition[] // Replace with actual type of preconditions
  outputConfigs: IConfigItem[] // Add this prop to receive the config names
  maxDisplayCount: number
}

const PreconditionSummary = ({
  preconditions,
  outputConfigs,
  maxDisplayCount,
}: PreconditionSummaryProps) => {
  const preconditionIds = preconditions
    .filter((precondition) => precondition.Ref !== undefined)
    .map((precondition) => precondition.Ref) as string[]

  const preconditionConfigs =
    outputConfigs
      .filter((item) => preconditionIds.includes(item.GUID))
      .map((item) => ({ ref: item.GUID, name: item.Name })) || []

  return (
    <div className="text-muted-foreground flex flex-row gap-2 py-2">
      {preconditions.slice(0, maxDisplayCount).map((precondition, index) => {
        const label =
          preconditionConfigs.find((config) => config.ref === precondition.Ref)
            ?.name ?? precondition.Ref

        const isLast =
          index === preconditions.slice(0, maxDisplayCount).length - 1

        const variantStyle = preconditionVariants[precondition.Type]

        return (
          <div className="flex flex-row items-center gap-2" key={index}>
            <Badge
              variant="outline"
              className={`px-4 ${variantStyle} flex flex-row items-center gap-1 rounded`}
            >
              <span className="max-w-30 truncate text-sm whitespace-nowrap">
                {label}
              </span>
              <span className="text-sm">{precondition.Operand}</span>
              <span className="text-sm">{precondition.Value}</span>
            </Badge>
            {!isLast && (
              <Badge variant="secondary" className="h-full px-2">
                {precondition.Logic == "and" ? (
                  <IconPlus size={10} />
                ) : (
                  <IconTallymark2 size={10} />
                )}
              </Badge>
            )}
          </div>
        )
      })}
      {preconditions.length > maxDisplayCount && (
        <Badge variant="outline" className="px-4">
          <span className="text-sm whitespace-nowrap">
            +{preconditions.length - maxDisplayCount} more
          </span>
        </Badge>
      )}
    </div>
  )
}
export default PreconditionSummary
