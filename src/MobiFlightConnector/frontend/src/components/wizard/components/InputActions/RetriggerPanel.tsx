const RetriggerPanel = () => {
  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">Retrigger</div>
        <div className="text-muted-foreground text-sm">Use this input action to re-trigger all button states.</div>
        <div className="text-muted-foreground text-sm">This is helpful to sync your virtual cockpit to the switch and button states of your physical controllers.</div>
      </div>
    </div>
  )
}
export default RetriggerPanel
