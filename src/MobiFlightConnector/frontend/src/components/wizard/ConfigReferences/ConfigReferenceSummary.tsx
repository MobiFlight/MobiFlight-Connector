import { Badge } from "@/components/ui/badge"
import { Separator } from "@/components/ui/separator"
import { configReferenceVariants } from "@/components/wizard/variants"
import { ConfigReference, IConfigItem } from "@/types/config"

type ConfigReferenceSummaryProps = {
  configReferences: ConfigReference[] // Replace with actual type of preconditions
  outputConfigs: IConfigItem[] // Add this prop to receive the config names
  maxDisplayCount: number
}

const ConfigReferenceSummary = ({
  configReferences,
  outputConfigs,
  maxDisplayCount,
}: ConfigReferenceSummaryProps) => {
  const configReferenceIds = configReferences
    .filter((configReference) => configReference.Ref !== undefined)
    .map((configReference) => configReference.Ref) as string[]

  const configReferenceConfigs =
    outputConfigs
      .filter((item) => configReferenceIds.includes(item.GUID))
      .map((item) => ({ ref: item.GUID, name: item.Name })) || []

  return (
    <div className="text-muted-foreground flex flex-row gap-2 pt-4">
      {configReferences
        .slice(0, maxDisplayCount)
        .map((configReference, index) => {
          const label =
            configReferenceConfigs.find(
              (config) => config.ref === configReference.Ref,
            )?.name ?? configReference.Ref
          const variantStyle = configReferenceVariants["default"]

          return (
            <div className="flex flex-row items-center" key={index}>
              <Badge
                variant="outline"
                className={`px-3 pr-3 py-1 ${variantStyle} flex flex-row`}
                title={label}
              >
                <span className="text-md rounded-full font-bold">{configReference.Placeholder}</span>
                <Separator orientation="vertical" className="mx-2 h-4 bg-gray-400" />
                <span className="max-w-30 truncate text-sm font-normal">{label}</span>
              </Badge>
            </div>
          )
        })}
      {configReferences.length > maxDisplayCount && (
        <Badge variant="outline" className="px-4">
          <span className="text-sm whitespace-nowrap">
            +{configReferences.length - maxDisplayCount} more
          </span>
        </Badge>
      )}
    </div>
  )
}
export default ConfigReferenceSummary
