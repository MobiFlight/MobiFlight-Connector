import { IconLoader2 } from "@tabler/icons-react"

//loading spinner component to be used in the wizard when fetching presets
const LoadingSpinner = () => {
  return (
    <div className="flex items-center justify-center p-6" data-testid="loading-spinner">
      <IconLoader2 className="text-primary h-8 w-8 animate-spin" />
    </div>
  )
}

export default LoadingSpinner