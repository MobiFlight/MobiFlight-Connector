import { useSettingsStore } from "@/stores/settingsStore"
import {
  Menubar,
  MenubarContent,
  MenubarItem,
  MenubarMenu,
  MenubarSeparator,
  MenubarShortcut,
  MenubarSub,
  MenubarSubContent,
  MenubarSubTrigger,
  MenubarTrigger,
} from "./ui/menubar"
import { CommunityMenu } from "./CommunityMenu"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandMainMenuPayload } from "@/types/commands"
import DarkModeToggle from "./DarkModeToggle"
import { useProjectStore } from "@/stores/projectStore"

export const MainMenu = () => {
  const { settings } = useSettingsStore()
  const { hasChanged } = useProjectStore()
  const { publish } = publishOnMessageExchange()
  const handleMenuItemClick = (payload: CommandMainMenuPayload) => {
    publish({
      key: "CommandMainMenu",
      payload: payload,
    })
  }
  return (
    <Menubar className="justify-between bg-muted/20">
      <div className="flex items-center">
        <MenubarMenu>
          <MenubarTrigger>File</MenubarTrigger>
          <MenubarContent>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "file.new" })}
            >
              New<MenubarShortcut>Ctrl+N</MenubarShortcut>
            </MenubarItem>
            <MenubarSeparator />
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "file.open" })}
            >
              Open...<MenubarShortcut>Ctrl+O</MenubarShortcut>
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "file.save" })}
              disabled={!hasChanged}
            >
              Save<MenubarShortcut>Ctrl+S</MenubarShortcut>
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "file.saveas" })}
            >
              Save as...<MenubarShortcut>Ctrl+Shift+S</MenubarShortcut>
            </MenubarItem>
            <MenubarSeparator />
            <MenubarSub>
              <MenubarSubTrigger>Recent projects</MenubarSubTrigger>
              <MenubarSubContent>
                {settings && settings.RecentFiles.length > 0 ? (
                  settings.RecentFiles.map((file, index) => (
                    <MenubarItem
                      key={index}
                      onSelect={() =>
                        handleMenuItemClick({
                          action: "file.recent",
                          index: index,
                        })
                      }
                    >
                      {file}
                    </MenubarItem>
                  ))
                ) : (
                  <MenubarItem disabled>No recent projects</MenubarItem>
                )}
              </MenubarSubContent>
            </MenubarSub>
            <MenubarSeparator />
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "file.exit" })}
            >
              Exit<MenubarShortcut>Ctrl+Q</MenubarShortcut>
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
        <MenubarMenu>
          <MenubarTrigger>Extras</MenubarTrigger>
          <MenubarContent>
            <MenubarSub>
              <MenubarSubTrigger>HubHop</MenubarSubTrigger>
              <MenubarSubContent>
                <MenubarItem
                  onSelect={() =>
                    handleMenuItemClick({ action: "extras.hubhop.download" })
                  }
                >
                  Download latest presets
                </MenubarItem>
              </MenubarSubContent>
            </MenubarSub>
            <MenubarSub>
              <MenubarSubTrigger>Microsoft Flight Simulator</MenubarSubTrigger>
              <MenubarSubContent>
                <MenubarItem
                  onSelect={() =>
                    handleMenuItemClick({ action: "extras.msfs.reinstall" })
                  }
                >
                  Re-install WASM Module
                </MenubarItem>
                <MenubarItem>Open Community Folder</MenubarItem>
              </MenubarSubContent>
            </MenubarSub>
            <MenubarItem
              onSelect={() =>
                handleMenuItemClick({ action: "extras.copylogs" })
              }
            >
              Copy logs to clipboard
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "extras.serials" })}
            >
              Manage orphaned serials
            </MenubarItem>
            <MenubarSeparator />
            <MenubarItem
              onSelect={() =>
                handleMenuItemClick({ action: "extras.settings" })
              }
            >
              Settings
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
        <MenubarMenu>
          <MenubarTrigger>Help</MenubarTrigger>
          <MenubarContent>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.docs" })}
            >
              Documentation<MenubarShortcut>F1</MenubarShortcut>
            </MenubarItem>
            <MenubarItem
              onSelect={() =>
                handleMenuItemClick({ action: "help.checkforupdate" })
              }
            >
              Check for update
            </MenubarItem>
            <MenubarSeparator />
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.discord" })}
            >
              Visit Discord server
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.hubhop" })}
            >
              Visit HubHop website
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.youtube" })}
            >
              Visit YouTube channel
            </MenubarItem>
            <MenubarSeparator />
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.about" })}
            >
              About
            </MenubarItem>
            <MenubarItem
              onSelect={() =>
                handleMenuItemClick({ action: "help.releasenotes" })
              }
            >
              Release notes
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
      </div>
      <div className="flex items-center gap-8 px-2">
        <CommunityMenu />
        <DarkModeToggle />
      </div>
    </Menubar>
  )
}
