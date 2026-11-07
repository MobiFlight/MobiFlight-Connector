export type SectionHeadlineProps = {
  title: string
  description: string
  description2?: string
}
const SectionHeadline = ({ title, description, description2 }: SectionHeadlineProps) => {
  return (
    <div className="flex flex-col">
      <div className="text-lg font-semibold">{title}</div>
      <div className="text-muted-foreground text-sm">{description}</div>
      {description2 && (
        <div className="text-muted-foreground text-sm">{description2}</div>
      )}
    </div>
  )
}
export default SectionHeadline
