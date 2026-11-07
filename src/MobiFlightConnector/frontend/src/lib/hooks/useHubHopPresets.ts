import { fetchHubHopPresets } from "@/lib/configWizard"
import { PresetSim } from "@/types/preset"
import { useQuery } from "@tanstack/react-query"

export const useHubHopPresets = (sim: PresetSim) => {
  return useQuery({
    queryKey: [`${sim}-presets`],
    queryFn: () => fetchHubHopPresets(sim),
    // presets don't change at runtime; HubHopState drives invalidation
    staleTime: Infinity,
  })
}