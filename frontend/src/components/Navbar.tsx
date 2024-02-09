import { IconBrandDiscord, IconBrandGithub, IconBrandPaypal, IconBrandYoutube } from '@tabler/icons-react'
//import HubHopLogo from '@/assets/hubhop-logo.svg'
import HubHopIcon from './mobiflight/icons/HubHopIcon'
// import HubHopLogo from '@/assets/mobiflight-logo-border.png'

const Navbar = () => {
  return (
    <div className='flex flex-row h-12 bg-slate-600 justify-end items-center pr-12 gap-4'>
      <div className='bg-purple-800 py-2 px-2'>
        <IconBrandDiscord className='h-8 w-8 text-white' />
      </div>
      <div className='bg-red-700 py-2 px-2'>
        <IconBrandYoutube className='h-8 w-8 text-white' />
      </div>
      <div className='bg-black py-2 px-2'>
        <IconBrandGithub className='h-8 w-8 text-white' />
      </div>
      <div className='bg-orange-400 py-2 px-2'>
        <HubHopIcon className='h-8 w-8 text-white' />
      </div>
      <div className='bg-yellow-500 py-2 px-1 flex items-center gap-1 pr-2 italic text-white font-bold'>
        <IconBrandPaypal className='h-8 w-8 text-white' />
        <div>Donate</div>
      </div>
    </div>
  )
}

export default Navbar