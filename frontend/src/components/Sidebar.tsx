import { useParams } from 'react-router-dom';
import { DarkModeToggle } from './mobiflight/darkmode-toggle'
import { IconDeviceGamepad2, IconList, IconSettings } from '@tabler/icons-react';
import { Button } from './ui/button';

type SidebarProps = {
  className?: string
}

const Sidebar = (props: SidebarProps) => {
  const params = useParams()


  return (
    <div className={`${props.className} bg-primary dark:bg-slate-800 w-16 flex flex-col justify-between items-center pb-8 z-50`}>
      <div className='flex flex-col items-center gap-8 pt-24'>
        <Button variant="outline" className='w-12 h-12' size="icon">
          <IconList></IconList>
        </Button>
        <Button variant="outline" className='w-12 h-12' size="icon">
          <IconDeviceGamepad2></IconDeviceGamepad2>
        </Button>
        
      </div>
      <div className='flex flex-col items-center gap-8'>
      <Button variant="outline" className='w-12 h-12' size="icon">
          <IconSettings></IconSettings>
        </Button>
      <DarkModeToggle></DarkModeToggle>
      </div>
    </div>
  )
}

export default Sidebar