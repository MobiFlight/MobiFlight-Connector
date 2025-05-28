import {
  IconBrandDiscordFilled,
  IconBrandYoutubeFilled,
  IconHeartDollar,
} from "@tabler/icons-react"
import { Button } from "./ui/button"

export const CommunityMenu = () => {
  return (
    <div className="flex flex-row gap-2">
      <Button className="bg-pink-700 px-2 py-1 text-white">
        <IconHeartDollar className="fill-white stroke-white text-white" />
        Support
      </Button>
      <Button className="px-2 py-1" variant={"outline"}>
        <IconBrandDiscordFilled className="fill-indigo-800 stroke-indigo-800" />
        Discord
      </Button>
      <Button className="h-10 px-2 py-1" variant={"outline"}>
        <IconBrandYoutubeFilled className="fill-red-700 stroke-red-700" />
        YouTube
      </Button>
      <Button className="h-10 px-2 py-1" variant={"outline"}>
        <IconBrandDiscordFilled className="fill-orange-400 stroke-orange-400" />
        HubHop
      </Button>
    </div>
  )
}
