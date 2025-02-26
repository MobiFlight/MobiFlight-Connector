import { IconMoon, IconSun } from "@tabler/icons-react"
import { useTheme } from "./theme-provider"

const DarkModeToggle = () => {
  const { setTheme } = useTheme()

  return (
    <div className="flex items-center relative">
      <IconMoon
        className="cursor-pointer rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100"
        onClick={() => setTheme("light")}
      />
      <IconSun
        className="cursor-pointer absolute rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0"
        onClick={() => setTheme("dark")}
      />
    </div>
  )
}

export default DarkModeToggle
