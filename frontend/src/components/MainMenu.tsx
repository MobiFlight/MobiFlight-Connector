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
import UserMenuItem from "@/components/user/UserMenuItem"

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
          <MenubarTrigger>{t("MainMenu.File.Label")}</MenubarTrigger>
          <MenubarContent>
            <MenubarItem
              onSelect={() => {
                showProjectOverlay({ mode: "create" })
              }}
            >
              {t("MainMenu.File.New")}<MenubarShortcut>Ctrl+N</MenubarShortcut>
            </MenubarItem>
            <MenubarSeparator />
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "file.open" })}
            >
              {t("MainMenu.File.Open")}<MenubarShortcut>Ctrl+O</MenubarShortcut>
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "file.save" })}
              disabled={!hasChanged}
            >
              {t("MainMenu.File.Save")}<MenubarShortcut>Ctrl+S</MenubarShortcut>
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "file.saveas" })}
            >
              {t("MainMenu.File.SaveAs")}<MenubarShortcut>Ctrl+Shift+S</MenubarShortcut>
            </MenubarItem>
            <MenubarSeparator />
            <MenubarSub>
              <MenubarSubTrigger>{t("MainMenu.File.RecentProjects")}</MenubarSubTrigger>
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
                  <MenubarItem disabled>{t("MainMenu.File.RecentProjects.None")}</MenubarItem>
                )}
              </MenubarSubContent>
            </MenubarSub>
            <MenubarSeparator />
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "file.exit" })}
            >
              {t("MainMenu.File.Exit")}<MenubarShortcut>Ctrl+Q</MenubarShortcut>
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
          <MenubarTrigger>{ t("MainMenu.Extras.Label") }</MenubarTrigger>
          <MenubarContent>
            <MenubarSub>
              <MenubarSubTrigger>HubHop</MenubarSubTrigger>
              <MenubarSubContent>
                <MenubarItem
                  onSelect={() =>
                    handleMenuItemClick({ action: "extras.hubhop.download" })
                  }
                >
                  { t("MainMenu.Extras.HubHop.DownloadLatestPresets")}
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
                  { t("MainMenu.Extras.MSFS.ReinstallWASMModule") }
                </MenubarItem>
              </MenubarSubContent>
            </MenubarSub>
            <MenubarItem
              onSelect={() =>
                handleMenuItemClick({ action: "extras.copylogs" })
              }
            >
              { t("MainMenu.Extras.CopyLogs") }
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
              { t("MainMenu.Extras.Settings") }
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
        <MenubarMenu>
          <MenubarTrigger>{ t("MainMenu.Help.Label") }</MenubarTrigger>
          <MenubarContent>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.docs" })}
            >
              { t("MainMenu.Help.Documentation") }<MenubarShortcut>F1</MenubarShortcut>
            </MenubarItem>
            <MenubarItem
              onSelect={() =>
                handleMenuItemClick({ action: "help.checkforupdate" })
              }
            >
              { t("MainMenu.Help.CheckForUpdates") }
            </MenubarItem>
            <MenubarSeparator />
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.discord" })}
            >
              { t("MainMenu.Help.VisitDiscord") }
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.hubhop" })}
            >
              { t("MainMenu.Help.VisitHubHop") }
            </MenubarItem>
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.youtube" })}
            >
              { t("MainMenu.Help.VisitYouTube") }
            </MenubarItem>
            <MenubarSeparator />
            <MenubarItem
              onSelect={() => handleMenuItemClick({ action: "help.about" })}
            >
              { t("MainMenu.Help.About") }
            </MenubarItem>
            <MenubarItem
              onSelect={() =>
                handleMenuItemClick({ action: "help.releasenotes" })
              }
            >
              { t("MainMenu.Help.ReleaseNotes") }
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
      </div>
      <div className="flex items-center gap-4 px-2">
        <CommunityMenu />
        <div className="flex flex-row gap-1">
          <DarkModeToggle />
          <UserMenuItem />
        </div>
      </div>
    </Menubar>
  )
}
