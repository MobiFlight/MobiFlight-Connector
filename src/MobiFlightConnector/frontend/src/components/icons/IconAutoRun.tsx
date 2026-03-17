import { cn } from "@/lib/utils"

type IconAutoRunProps = {
  className?: string
}

const IconAutoRun = (props: IconAutoRunProps) => {
  const className = cn("stroke-current fill-current ", props.className)

  return (
    <svg
      className={className}
      width="24"
      height="24"
      version="1.1"
      viewBox="0 0 24 24"
    >
      <path
        d="M5.63604 19.6823C4.80031 18.8466 4.13738 17.8544 3.68508 16.7625C3.23279 15.6706 3 14.5003 3 13.3184C3 12.1365 3.23279 10.9661 3.68508 9.87421C4.13738 8.78228 4.80031 7.79013 5.63604 6.9544C6.47177 6.11867 7.46392 5.45574 8.55585 5.00344C9.64778 4.55115 10.8181 4.31836 12 4.31836C13.1819 4.31836 14.3522 4.55115 15.4442 5.00344C16.5361 5.45574 17.5282 6.11867 18.364 6.9544C19.1997 7.79013 19.8626 8.78228 20.3149 9.87421C20.7672 10.9661 21 12.1365 21 13.3184C21 14.5003 20.7672 15.6706 20.3149 16.7625C19.8626 17.8544 19.1997 18.8466 18.364 19.6823M18.364 19.6823L18.633 17.3184M18.364 19.6823L20.5826 18.6361M9 17.3184V12.5184C9 10.8614 10.343 9.31836 12 9.31836C13.657 9.31836 15 10.8614 15 12.5184V17.3184M15 14.3184H9"
        strokeWidth="2"
        fill="none"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}

export default IconAutoRun
