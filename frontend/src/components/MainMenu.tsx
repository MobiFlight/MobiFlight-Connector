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
import { useProjectModal } from "@/lib/hooks/useProjectModal"
import { useTranslation } from "react-i18next"
import { useModal } from "@/lib/hooks/useModal"

export const MainMenu = () => {
  const { t } = useTranslation()
  const { settings } = useSettingsStore()
  const { hasChanged } = useProjectStore()
  const { publish } = publishOnMessageExchange()
  const handleMenuItemClick = (payload: CommandMainMenuPayload) => {
    publish({
      key: "CommandMainMenu",
      payload: payload,
    })
  }

  const { showOverlay: showProjectOverlay } = useProjectModal()
  const { showOverlay: showModalOverlay } = useModal()

  return (
    <Menubar className="bg-muted/20 justify-between">
      <div className="flex items-center">
        <MenubarMenu>
          <MenubarTrigger>File</MenubarTrigger>
          <MenubarContent>
            <MenubarItem
              onSelect={() => {
                showProjectOverlay({ mode: "create" })
              }}
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
                          options: {
                            filePath: file,
                          },
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
          <MenubarTrigger>{t("MainMenu.View.Label")}</MenubarTrigger>
          <MenubarContent>
            <MenubarItem
              onSelect={() =>
                handleMenuItemClick({ action: "view.zoom.reset" })
              }
            >
              {t("MainMenu.View.Zoom.Reset")}
              <MenubarShortcut>
                {t("MainMenu.View.Zoom.Shortcut.Reset")}
              </MenubarShortcut>
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "view.zoom.in" })}
            >
              {t("MainMenu.View.Zoom.In")}
              <MenubarShortcut>
                {t("MainMenu.View.Zoom.Shortcut.In")}
              </MenubarShortcut>
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "view.zoom.out" })}
            >
              {t("MainMenu.View.Zoom.Out")}
              <MenubarShortcut>
                {t("MainMenu.View.Zoom.Shortcut.Out")}
              </MenubarShortcut>
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
              onSelect={() => showModalOverlay({ route: "/bindings" })}
            >
              {t("MainMenu.Extras.ControllerBindings")}
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
