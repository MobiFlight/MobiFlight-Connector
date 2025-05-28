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

export const MainMenu = () => {
  return (
    <Menubar>
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
              <MenubarItem>Dummy project 1</MenubarItem>
              <MenubarItem>Dummy project 2</MenubarItem>
              <MenubarItem>Dummy project 3</MenubarItem>
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
    </Menubar>
  )
}
