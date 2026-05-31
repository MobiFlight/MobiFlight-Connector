import { Badge } from "@/components/ui/badge"
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
    <div className="text-muted-foreground flex flex-row gap-2 py-2">
      {configReferences
        .slice(0, maxDisplayCount)
        .map((configReference, index) => {
          const label =
            configReferenceConfigs.find(
              (config) => config.ref === configReference.Ref,
            )?.name ?? configReference.Ref
          const variantStyle = configReferenceVariants["default"]
          
          return (
            <div className="flex flex-row items-center gap-2" key={index}>
              <Badge
                variant="outline"
                className={`px-4 ${variantStyle} flex flex-row items-center justify-center gap-1 rounded min-w-10`}
                title={label}
              >
                <span className="text-sm">{configReference.Placeholder}</span>
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
