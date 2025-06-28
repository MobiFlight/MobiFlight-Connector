import {
  IconBrandDiscordFilled,
  IconBrandYoutubeFilled,
  IconHeartDollar,
} from "@tabler/icons-react"
import { Button } from "./ui/button"
import IconBrandHubHopLogo from "./icons/IconBrandHubHopLogo"

export const CommunityMenu = () => {
  return (
    <div className="flex flex-row gap-1 py-2 text-sm items-center">
      <Button className="group bg-pink-600 hover:bg-pink-400 dark:hover:bg-pink-600 dark:bg-transparent dark:border-pink-600 dark:border dark:text-pink-600 text-white dark:hover:text-white pl-3 pr-4 py-1 h-8 [&_svg]:size-5 rounded-full gap-1" variant={"default"}>
        <IconHeartDollar className="fill-none stroke-white text-white dark:stroke-pink-600 group-hover:stroke-white " />
        Support us
      </Button>
      <Button className="px-4 py-1 [&_svg]:size-6 h-8 rounded-full gap-1" variant={"ghost"}>
        <IconBrandDiscordFilled className="fill-indigo-800 stroke-indigo-800" />
        Discord
      </Button>
      <Button className="px-4 py-1 [&_svg]:size-6 h-8 rounded-full gap-1" variant={"ghost"}>
        <IconBrandYoutubeFilled className="fill-red-700 stroke-red-700" />
        YouTube
      </Button>
      <Button className="px-4 py-1 [&_svg]:size-6 h-8 rounded-full gap-1" variant={"ghost"}>
        <IconBrandHubHopLogo className="fill-orange-400 stroke-orange-400" />
        HubHop
      </Button>
    </div>
  )
}
