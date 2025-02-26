import { IconMoon, IconSun } from '@tabler/icons-react'
import { useTheme } from './theme-provider'

const DarkModeToggle = () => {  
  const {theme, setTheme} = useTheme()
  
  return (
    theme === "dark" ?
      <IconMoon className='cursor-pointer' onClick={() => setTheme("light")} />
      :
      <IconSun className='cursor-pointer' onClick={() => setTheme("dark")} />
  )

}

export default DarkModeToggle
