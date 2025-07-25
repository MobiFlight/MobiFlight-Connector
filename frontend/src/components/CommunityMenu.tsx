import {
  IconBrandDiscordFilled,
  IconBrandYoutubeFilled,
  IconHeartDollar,
} from "@tabler/icons-react"
import { Button } from "./ui/button"
import IconBrandHubHopLogo from "./icons/IconBrandHubHopLogo"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandMainMenuPayload } from "@/types/commands"

export const CommunityMenu = () => {
  const { publish } = publishOnMessageExchange()
  const handleMenuItemClick = (payload: CommandMainMenuPayload) => {
    publish({
      key: "CommandMainMenu",
      payload: payload,
    })
  }
  return (
    <div className="flex flex-row items-center gap-1 py-2 text-sm">
      <Button
        className="group h-8 gap-1 rounded-full bg-pink-600 py-1 pl-3 pr-4 text-white hover:bg-pink-400 dark:border dark:border-pink-600 dark:bg-transparent dark:text-pink-600 dark:hover:bg-pink-600 dark:hover:text-white [&_svg]:size-5"
        variant={"default"}
        onClick={() => handleMenuItemClick({ action: "help.donate" })}
      >
        <IconHeartDollar className="fill-none stroke-white text-white group-hover:stroke-white dark:stroke-pink-600" />
        Support us
      </Button>
      <Button
        className="h-8 gap-1 rounded-full px-4 py-1 [&_svg]:size-6"
        variant={"ghost"}
        onClick={() => handleMenuItemClick({ action: "help.discord" })}
      >
        <IconBrandDiscordFilled className="fill-indigo-800 stroke-indigo-800" />
        Discord
      </Button>
      <Button
        className="h-8 gap-1 rounded-full px-4 py-1 [&_svg]:size-6"
        variant={"ghost"}
        onClick={() => handleMenuItemClick({ action: "help.youtube" })}
      >
        <IconBrandYoutubeFilled className="fill-red-700 stroke-red-700" />
        YouTube
      </Button>
      <Button
        className="h-8 gap-1 rounded-full px-4 py-1 [&_svg]:size-6"
        variant={"ghost"}
        onClick={() => handleMenuItemClick({ action: "help.hubhop" })}
      >
        <IconBrandHubHopLogo className="fill-orange-400 stroke-orange-400" />
        HubHop
      </Button>
    </div>
  )
}
