import { IconBrandDiscord, IconBrandGithub, IconBrandPaypal, IconBrandYoutube } from '@tabler/icons-react'
//import HubHopLogo from '@/assets/hubhop-logo.svg'
import HubHopIcon from './mobiflight/icons/HubHopIcon'
import { Link } from 'react-router-dom'
// import HubHopLogo from '@/assets/mobiflight-logo-border.png'

const Navbar = () => {
  return (
    <div className='flex flex-row h-12 bg-slate-600 justify-end items-center pr-12 gap-4'>
      <div className='bg-purple-800 py-2 px-2'>
        <Link to="https://mobiflight.com/discord" target="_blank">
          <IconBrandDiscord className='h-8 w-8 text-white' />
        </Link>
      </div>
      <div className='bg-red-700 py-2 px-2'>
        <Link to="https://youtube.com/mobiflight" target="_blank">
          <IconBrandYoutube className='h-8 w-8 text-white' />
        </Link>
      </div>
      <div className='bg-black py-2 px-2'>
        <Link to="https://github.com/mobiflight" target="_blank">
          <IconBrandGithub className='h-8 w-8 text-white' />
        </Link>
      </div>
      <div className='bg-orange-400 py-2 px-2'>
        <Link to="https://hubhop.mobiflight.com" target="_blank">
          <HubHopIcon className='h-8 w-8 text-white' />
        </Link>
      </div>
      <Link to="https://paypal.me/mobiflight" target="_blank">
        <div className='bg-yellow-500 py-2 px-1 flex items-center gap-1 pr-2 italic text-white font-bold'>
          <IconBrandPaypal className='h-8 w-8 text-white' />
          <div>Donate</div>
        </div>
      </Link>
    </div>
  )
}

export default Navbar