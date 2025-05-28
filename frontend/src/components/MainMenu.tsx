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
import IconMobiFlightLogo from "./icons/IconMobiFlightLogo"
import { CommunityMenu } from "./CommunityMenu"

export const MainMenu = () => {
  const { settings } = useSettingsStore()
  return (
    <Menubar className="justify-between bg-muted/20">
      <div className="flex">
      <IconMobiFlightLogo />
      <MenubarMenu>
        <MenubarTrigger>File</MenubarTrigger>
        <MenubarContent>
          <MenubarItem>
            New<MenubarShortcut>Ctrl+N</MenubarShortcut>
          </MenubarItem>
          <MenubarSeparator />
          <MenubarItem>
            Open...<MenubarShortcut>Ctrl+O</MenubarShortcut>
          </MenubarItem>
          <MenubarItem>
            Save as...<MenubarShortcut>Ctrl+Shift+S</MenubarShortcut>
          </MenubarItem>
          <MenubarSeparator />
          <MenubarSub>
            <MenubarSubTrigger>Recent projects</MenubarSubTrigger>
            <MenubarSubContent>
              {settings && settings.RecentFiles.length > 0 ? (
                settings.RecentFiles.map((file, index) => (
                  <MenubarItem key={index}>{file}</MenubarItem>
                ))
              ) : (
                <MenubarItem disabled>No recent projects</MenubarItem>
              )}
            </MenubarSubContent>
          </MenubarSub>
          <MenubarSeparator />
          <MenubarItem>Exit</MenubarItem>
        </MenubarContent>
      </MenubarMenu>
      <MenubarMenu>
        <MenubarTrigger>Extras</MenubarTrigger>
        <MenubarContent>
          <MenubarSub>
            <MenubarSubTrigger>HubHop</MenubarSubTrigger>
            <MenubarSubContent>
              <MenubarItem>Download latest presets</MenubarItem>
            </MenubarSubContent>
          </MenubarSub>
          <MenubarSub>
            <MenubarSubTrigger>Microsoft Flight Simulator</MenubarSubTrigger>
            <MenubarSubContent>
              <MenubarItem>Re-install WASM Module</MenubarItem>
              <MenubarItem>Open Community Folder</MenubarItem>
            </MenubarSubContent>
          </MenubarSub>
          <MenubarItem>Copy logs to clipboard</MenubarItem>
          <MenubarItem>Manage orphaned serials</MenubarItem>
          <MenubarSeparator />
          <MenubarItem>Settings</MenubarItem>
        </MenubarContent>
      </MenubarMenu>
      <MenubarMenu>
        <MenubarTrigger>Help</MenubarTrigger>
        <MenubarContent>
          <MenubarItem>Documentation</MenubarItem>
          <MenubarItem>Check for update</MenubarItem>
          <MenubarSeparator />
          <MenubarItem>Visit Discord server</MenubarItem>
          <MenubarItem>Visit HubHop website</MenubarItem>
          <MenubarItem>Visit YouTube channel</MenubarItem>
          <MenubarSeparator />
          <MenubarItem>About</MenubarItem>
          <MenubarItem>Release notes</MenubarItem>
        </MenubarContent>
      </MenubarMenu>
      </div>
      <div>
      <CommunityMenu />
      </div>
    </Menubar>
  )
}
