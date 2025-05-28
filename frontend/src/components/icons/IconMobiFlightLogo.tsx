import { cn } from "@/lib/utils"

type IconMobiFlightLogoProps = {
  className?: string
}

const IconMobiFlightLogo = (props: IconMobiFlightLogoProps) => {
  const className = cn("stroke-current fill-current ", props.className)

  return (
    <svg
      className={className}
      id="SVGRoot"
      width="24"
      height="24"
      version="1.1"
      viewBox="0 0 177.16 177.16"
    >
      <g>
        <rect width="177.16" height="177.16" fill="#3366ca"/>
        <path
          d="m0 0v177.16h177.16v-177.16zm9.2809 9.2809h158.6v21.428h-34.087l-28.608 115.58h30.526l11.826-47.676h20.344v69.103h-158.6v-21.428h1.1384l20.296-81.868-4.635 81.868h23.014l36.118-81.868-20.296 81.868h27.647l28.608-115.58h-42.031l-29.726 67.903 4.1547-67.903h-42.19l-2.0972 8.4743zm149.13 45.667h9.4767v20.547h-14.59z"
          stroke="none"
          fill="#ffffff"
        />
      </g>
    </svg>
  )
}

export default IconMobiFlightLogo
