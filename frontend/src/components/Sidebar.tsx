import { NavLink, useParams } from 'react-router-dom';
import { DarkModeToggle } from './mobiflight/darkmode-toggle'
import { IconDeviceGamepad2, IconList, IconListSearch, IconSettings } from '@tabler/icons-react';
import { Button } from './ui/button';
import { cn } from '@/lib/utils';

type SidebarProps = {
  className?: string
}

const Sidebar = (props: SidebarProps) => {
  const params = useParams()
  const activeClassNameProps = "dark:bg-gray-700"

  return (
    <div className={cn(`bg-primary dark:bg-slate-800 min-w-16 w-16 flex flex-col justify-between items-center pb-8 z-50`, props.className)}>
      <div className='flex flex-col items-center gap-8 pt-24'>
        <NavLink to={`/projects/${params.id}/configs`}>
          <Button variant="outline" className={cn(activeClassNameProps, `hover:border hover:border-gray-500 w-12 h-12`)} size="icon">
            <IconList></IconList>
          </Button>
        </NavLink>
        <NavLink to={`/devices`}>
          <Button variant="outline" className={cn(activeClassNameProps, `hover:border hover:border-gray-500 w-12 h-12`)} size="icon">
            <IconDeviceGamepad2></IconDeviceGamepad2>
          </Button>
        </NavLink>

      </div>
      <div className='flex flex-col items-center gap-8'>
        <NavLink to="/log">
          <Button variant="outline" className={cn(activeClassNameProps, `hover:border hover:border-gray-500 w-12 h-12`)} size="icon">
            <IconListSearch></IconListSearch>
          </Button>
        </NavLink>
        <NavLink to="/settings">
          <Button variant="outline" className={cn(activeClassNameProps, `hover:border hover:border-gray-500 w-12 h-12`)} size="icon">
            <IconSettings></IconSettings>
          </Button>
        </NavLink>
        <DarkModeToggle className="hover:border hover:border-gray-500"></DarkModeToggle>
      </div>
    </div>
  )
}

export default Sidebar