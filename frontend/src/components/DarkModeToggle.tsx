import { useTheme } from "@/lib/hooks/useTheme"
import { IconMoon, IconSun } from "@tabler/icons-react"

const DarkModeToggle = () => {
  const { setTheme } = useTheme()

  return (
    <div className="hidden md:flex items-center relative">
      <IconMoon
        role="button"
        aria-label="Toggle light mode"
        className="cursor-pointer rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100"
        onClick={() => setTheme("light")}
        />
      <IconSun
        role="button"
        aria-label="Toggle dark mode"
        className="cursor-pointer absolute rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0"
        onClick={() => setTheme("dark")}
      />
    </div>
  )
}

export default DarkModeToggle
