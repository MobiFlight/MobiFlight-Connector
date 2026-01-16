import { ExecutionState } from "../src/types/messages"
import { test, expect } from "./fixtures"

test.describe("Project view tests", () => {
  test("Confirm empty project view content and actions", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()

    await expect(
      page.getByText("Create your first project to get started"),
    ).toBeVisible()

    const recentProjectFilter = page.getByTestId("recent-projects-filter-bar")
    await expect(recentProjectFilter).not.toBeVisible()

    const createProjectButton = page.getByRole("button", { name: "Project" })
    await expect(createProjectButton).toBeVisible()
    await expect(createProjectButton).toHaveCount(1)
  })

  test("Confirm project view with test data has correct content", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps({
      Name: "Test Project",
      Sim: "msfs",
      ControllerBindings: [
        {
          BoundController: "ProtoBoard-v2/ SN-3F1-FDD",
          OriginalController: "ProtoBoard-v2/ SN-3F1-FDD",
          Status: "Match",
        },
        {
          BoundController: null,
          OriginalController: "MobiFlight Board / SN-12345",
          Status: "Missing",
        },
        {
          BoundController: "Alpha Flight Controls / JS-67890",
          OriginalController: "Alpha Flight Controls / JS-67891",
          Status: "AutoBind",
        },
        {
          BoundController:
            "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000",
          OriginalController:
            "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000",
          Status: "Match",
        },
        {
          BoundController: "miniCOCKPIT miniFCU/ SN-E98-277",
          OriginalController: "miniCOCKPIT miniFCU/ SN-E98-277",
          Status: "Match",
        },
      ],
    })

    const currentProjectCard = page.getByTestId("project-card")
    await expect(currentProjectCard).toBeVisible()
    await expect(
      currentProjectCard.getByRole("heading", { name: "Test Project" }),
    ).toBeVisible()

    const controllerIcons = currentProjectCard
      .getByTestId("controller-icons")
      .getByTestId("controller-icon")
    await expect(controllerIcons).toHaveCount(5)
    await expect(controllerIcons.nth(0)).toHaveAttribute(
      "title",
      "miniCOCKPIT miniFCU - Controller connected",
    )
    await expect(controllerIcons.nth(1)).toHaveAttribute(
      "title",
      "Bravo Throttle Quadrant - Controller connected",
    )
    await expect(controllerIcons.nth(2)).toHaveAttribute(
      "title",
      "ProtoBoard-v2 - Controller connected",
    )
    await expect(controllerIcons.nth(3)).toHaveAttribute(
      "title",
      "Alpha Flight Controls - Auto-bound controller",
    )
    await expect(controllerIcons.nth(4)).toHaveAttribute(
      "title",
      "MobiFlight Board - Controller missing",
    )

    const bindingIssueIcon = currentProjectCard.getByTestId(
      "controller-binding-issue-icon",
    )

    await expect(bindingIssueIcon).toBeVisible()
    await expect(bindingIssueIcon).toHaveAttribute(
      "title",
      "Click to view controller binding issues",
    )

    const dialog = page.getByRole("dialog", { name: "Controller Bindings" })
    await expect(dialog).not.toBeVisible()
    await bindingIssueIcon.click()
    await expect(dialog).toBeVisible()
  })

  test("Confirm project shows more indicator correctly", async ({
    dashboardPage,
    page,
  }) => {
    const controllerBindings = [
      {
        BoundController: "ProtoBoard-v2/ SN-3F1-FDD",
        OriginalController: "ProtoBoard-v2/ SN-3F1-FDD",
        Status: "Match",
      },
      {
        BoundController: null,
        OriginalController: "MobiFlight Board / SN-12345",
        Status: "Missing",
      },
      {
        BoundController: "Alpha Flight Controls / JS-67890",
        OriginalController: "Alpha Flight Controls / JS-67891",
        Status: "AutoBind",
      },
      {
        BoundController:
          "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000",
        OriginalController:
          "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000",
        Status: "Match",
      },
      {
        BoundController: "miniCOCKPIT miniFCU/ SN-E98-277",
        OriginalController: "miniCOCKPIT miniFCU/ SN-E98-277",
        Status: "Match",
      },
      {
        BoundController: null,
        Status: "Missing",
        OriginalController:
          "Behringer X-Touch Mini / MI-b0875190-3b89-11ed-8007-444553540001",
      },
      {
        BoundController:
          "Alpha Flight Controls / JS-b0875190-3b89-11ed-8007-444553540000",
        Status: "RequiresManualBind",
        OriginalController:
          "Alpha Flight Controls / JS-b0875190-3b89-11ed-8007-444553540000",
      }
    ]

    const sixBindingsWithMatch = controllerBindings.slice(0, 6).map((cb) => ({...cb, Status: 'Match'}))
    const sevenBindingsWithMatch = controllerBindings.slice(0, 7).map((cb) => ({...cb, Status: 'Match'}))
    const sixBindingsWithError = controllerBindings.slice(0, 6).map((cb) => ({...cb, Status: 'RequiresManualBind'}))
    const currentProjectCard = page.getByTestId("project-card")
    
    const bindingIssueIcon = currentProjectCard.getByTestId(
      "controller-binding-issue-icon",
    )
    const moreControllersIndicator = currentProjectCard.getByTestId("more-controllers-indicator")

    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps({
      Name: "Test Project",
      Sim: "msfs",
      ControllerBindings: sixBindingsWithMatch,
    })

    await expect(bindingIssueIcon).not.toBeVisible()
    await expect(moreControllersIndicator).not.toBeVisible()

    await dashboardPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps({
      Name: "Test Project",
      Sim: "msfs",
      ControllerBindings: sevenBindingsWithMatch,
    })

    await expect(bindingIssueIcon).not.toBeVisible()
    await expect(moreControllersIndicator).toBeVisible()
    await expect(moreControllersIndicator.getByText("+1")).toBeVisible()

    await dashboardPage.mobiFlightPage.initWithTestDataAndSpecificProjectProps({
      Name: "Test Project",
      Sim: "msfs",
      ControllerBindings: sixBindingsWithError,
    })

    await expect(bindingIssueIcon).toBeVisible()
    await expect(moreControllersIndicator).toBeVisible()
    await expect(moreControllersIndicator.getByText("+1")).toBeVisible()
  })

  test("Navigate to project view", async ({ dashboardPage, page }) => {
    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.initWithTestData()
    const currentProjectCard = page.getByTestId("project-card")

    // Verify we navigate to config route
    await currentProjectCard.getByRole("button").nth(0).click()
    await expect(page).toHaveURL(/.*\/config((\/|\?).*)?/)
  })

  test("Start and Stop config execution", async ({
    dashboardPage,
    configListPage,
    page,
  }) => {
    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.initWithTestData()
    await dashboardPage.mobiFlightPage.trackCommand("CommandProjectToolbar")
    const currentProjectCard = page.getByTestId("project-card")

    // Verify correct start and stop messages
    await currentProjectCard
      .getByTestId("project-card-start-stop-button")
      .click()
    let postedCommands = await dashboardPage.mobiFlightPage.getTrackedCommands()
    let lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandProjectToolbar")
    expect(lastCommand.payload.action).toEqual("run")

    // Simulate that the config is running
    await configListPage.updateExecutionState({
      IsRunning: true,
      IsTesting: false,
      RunAvailable: false,
      TestAvailable: false,
    } as ExecutionState)

    await currentProjectCard
      .getByTestId("project-card-start-stop-button")
      .click()

    postedCommands = await dashboardPage.mobiFlightPage.getTrackedCommands()
    lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandProjectToolbar")
    expect(lastCommand.payload.action).toEqual("stop")
  })
})

test.describe("Project settings modal features", () => {
  test("Create new project in modal", async ({ dashboardPage, page }) => {
    const createProjectButton = page.getByRole("button", { name: "Project" })
    const createProjectDialog = page.getByRole("dialog", {
      name: "Create New Project",
    })
    const fsuipcCheckbox = createProjectDialog.getByLabel("FSUIPC")
    const prosimCheckbox = createProjectDialog.getByLabel("ProSim")
    const projectNameInput = createProjectDialog.getByLabel("Project Name")
    const createButton = createProjectDialog.getByRole("button", {
      name: "Create",
    })

    const projectOptions = [
      {
        name: "MSFS no FSUIPC",
        value: "msfs",
        simLabel: "Microsoft Flight Simulator",
        fsuipc: { click: false, use: false },
        prosim: { click: false, use: false },
      },
      {
        name: "MSFS with FSUIPC",
        value: "msfs",
        simLabel: "Microsoft Flight Simulator",
        fsuipc: { click: true, use: true },
        prosim: { click: false, use: false },
      },
      {
        name: "X-Plane",
        value: "xplane",
        simLabel: "X-Plane",
        fsuipc: { click: false, use: false },
        prosim: { click: false, use: false },
      },
      {
        name: "Prepar3D",
        value: "p3d",
        simLabel: "Prepar3D",
        fsuipc: { click: false, use: true },
        prosim: { click: false, use: false },
      },
      {
        name: "FSX / FS2004",
        value: "fsx",
        simLabel: "FSX / FS2004",
        fsuipc: { click: false, use: true },
        prosim: { click: false, use: false },
      },
      {
        name: "MSFS with ProSim",
        value: "msfs",
        simLabel: "Microsoft Flight Simulator",
        fsuipc: { click: false, use: false },
        prosim: { click: true, use: true },
      },
    ]

    for (const option of projectOptions) {
      await dashboardPage.gotoPage()
      await dashboardPage.mobiFlightPage.trackCommand("CommandMainMenu")

      await createProjectButton.click()

      await projectNameInput.fill(option.name)

      const simOption = createProjectDialog.getByRole("radio", {
        name: option.value,
      })

      await simOption.click()
      await expect(createProjectDialog.getByText(option.simLabel)).toBeVisible()
      if (option.fsuipc.click) {
        await fsuipcCheckbox.check()
      }

      if (option.prosim.click) {
        await prosimCheckbox.check()
      }

      await createButton.click()
      await expect(createProjectDialog).not.toBeVisible()
      const postedCommands =
        await dashboardPage.mobiFlightPage.getTrackedCommands()
      const lastCommand = postedCommands!.pop()

      expect(lastCommand.key).toEqual("CommandMainMenu")
      expect(lastCommand.payload.action).toEqual("file.new")
      expect(lastCommand.payload.options.project.Name).toEqual(option.name)
      expect(lastCommand.payload.options.project.Sim).toEqual(option.value)
      expect(lastCommand.payload.options.project.Features.FSUIPC).toEqual(
        option.fsuipc.use,
      )
      expect(lastCommand.payload.options.project.Features.ProSim).toEqual(
        option.prosim.use,
      )
    }
  })

  test("Reset feature checkboxes when changing simulator", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()
    const createProjectButton = page.getByRole("button", { name: "Project" })
    const createProjectDialog = page.getByRole("dialog", {
      name: "Create New Project",
    })
    const fsuipcCheckbox = createProjectDialog.getByLabel("FSUIPC")
    const prosimCheckbox = createProjectDialog.getByLabel("ProSim")
    const projectNameInput = createProjectDialog.getByLabel("Project Name")

    await createProjectButton.click()
    await projectNameInput.fill("Test Reset Features")

    // Select MSFS and enable both features
    const msfsOption = createProjectDialog.getByRole("radio", {
      name: "msfs",
    })
    await msfsOption.click()
    await fsuipcCheckbox.check()
    await prosimCheckbox.check()

    expect(await fsuipcCheckbox.isChecked()).toBeTruthy()
    expect(await prosimCheckbox.isChecked()).toBeTruthy()

    // Change to X-Plane and verify both features are disabled
    const p3dOption = createProjectDialog.getByRole("radio", {
      name: "p3d",
    })
    await p3dOption.click()
    expect(await prosimCheckbox.isChecked()).toBeFalsy()

    // Change back to MSFS and verify both features are disabled
    await msfsOption.click()
    expect(await fsuipcCheckbox.isChecked()).toBeFalsy()
    expect(await prosimCheckbox.isChecked()).toBeFalsy()
  })

  test("Dont allow new project without a name", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()

    const createProjectButton = page.getByRole("button", { name: "Project" })
    const createProjectDialog = page.getByRole("dialog", {
      name: "Create New Project",
    })
    const projectNameInput = createProjectDialog.getByLabel("Project Name")
    const createButton = createProjectDialog.getByRole("button", {
      name: "Create",
    })

    await createProjectButton.click()
    await projectNameInput.fill("")
    await createButton.click()

    // The dialog is still open
    await expect(createProjectDialog).toBeVisible()

    // Error message is shown
    const errorMessage = createProjectDialog.getByTestId(
      "form-project-name-error",
    )
    await expect(errorMessage).toBeVisible()

    await projectNameInput.fill("Valid Name")
    await expect(errorMessage).not.toBeVisible()

    // Now the form can submit and dialog is closed
    await createButton.click()
    await expect(createProjectDialog).not.toBeVisible()
  })

  test("Edit current project settings", async ({ dashboardPage, page }) => {
    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.initWithTestData()

    const currentProjectCard = page.getByTestId("project-card")
    const projectContextMenu = currentProjectCard.getByRole("button", {
      name: "Open menu",
    })
    await projectContextMenu.click()

    const settingsMenuItem = page.getByRole("menuitem", { name: "Settings" })
    await expect(settingsMenuItem).toBeVisible()
    settingsMenuItem.click()

    const editProjectDialog = page.getByRole("dialog", {
      name: "Edit Project",
    })
    await expect(editProjectDialog).toBeVisible()
  })

  test("Using [space] and [del] work when on top of config list view", async ({
    configListPage,
    page,
  }) => {
    // https://github.com/MobiFlight/MobiFlight-Connector/issues/2448
    await configListPage.gotoPage()
    await configListPage.mobiFlightPage.initWithTestData()

    const FileMenu = page
      .getByRole("menubar")
      .getByRole("menuitem", { name: "File" })
    await expect(FileMenu).toBeVisible()
    await FileMenu.click()
    const NewMenuItem = page.getByRole("menuitem", { name: "New" })
    await expect(NewMenuItem).toBeVisible()
    await NewMenuItem.click()

    const createProjectDialog = page.getByRole("dialog", {
      name: "Create New Project",
    })
    await expect(createProjectDialog).toBeVisible()

    const projectNameInput = createProjectDialog.getByLabel("Project Name")
    await projectNameInput.fill("Test Project With Space")

    await expect(projectNameInput).toHaveValue("Test Project With Space")
    await projectNameInput.press("Backspace")
    await projectNameInput.press("Backspace")
    await projectNameInput.press("Backspace")
    await projectNameInput.press("Backspace")
    await projectNameInput.press("Backspace")

    await expect(projectNameInput).toHaveValue("Test Project With ")
  })
})

test.describe("Project list view tests", () => {
  test("Confirm project list view content", async ({ dashboardPage, page }) => {
    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.initWithTestData()

    const recentProjectsList = page.getByTestId("recent-projects-list")
    await expect(recentProjectsList).toBeVisible()

    const projectItems = recentProjectsList.getByTestId("project-list-item")
    await expect(projectItems).toHaveCount(27)

    const firstProject = projectItems.nth(0)
    await expect(firstProject).toBeVisible()

    await dashboardPage.mobiFlightPage.trackCommand("CommandMainMenu")
    await firstProject.click()

    const postedCommands =
      await dashboardPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandMainMenu")
    expect(lastCommand.payload.action).toEqual("file.recent")
    expect(lastCommand.payload.options.project).toEqual(
      dashboardPage.mobiFlightPage.getRecentProjects()[0],
    )

    // Verify we navigate to config route
    await firstProject.getByRole("button").nth(1).click()
    await expect(page).toHaveURL(/.*\/config((\/|\?).*)?/)
  })

  test("Filter project list view", async ({ dashboardPage, page }) => {
    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.initWithTestData()

    const recentProjectsList = page.getByTestId("recent-projects-list")
    const filterBar = page.getByTestId("recent-projects-filter-bar")
    const filterAllButton = filterBar.getByRole("button", { name: "All" })
    const filterMsfsButton = filterBar.getByRole("button", {
      name: "Microsoft",
    })
    const filterXplaneButton = filterBar.getByRole("button", {
      name: "X-Plane",
    })
    const filterInput = filterBar.getByPlaceholder("Filter projects...")
    const projectItems = recentProjectsList.getByTestId("project-list-item")

    await expect(recentProjectsList).toBeVisible()
    await expect(projectItems).toHaveCount(27)

    // Filter MSFS
    await filterMsfsButton.click()
    await expect(projectItems).toHaveCount(25)

    // Filter X-Plane
    await filterXplaneButton.click()
    await expect(projectItems).toHaveCount(1)

    // Filter All
    await filterAllButton.click()
    await expect(projectItems).toHaveCount(27)

    // Text filter
    await filterInput.fill("new pro")
    await expect(projectItems).toHaveCount(6)
    for (let i = 0; i < 6; i++) {
      await expect(projectItems.nth(i)).toContainText("New Project")
    }
  })

  test("Remove projects from recent list", async ({ dashboardPage, page }) => {
    await dashboardPage.gotoPage()
    await dashboardPage.mobiFlightPage.initWithTestData()
    await dashboardPage.mobiFlightPage.trackCommand("CommandMainMenu")

    const recentProjectsList = page.getByTestId("recent-projects-list")
    const projectItems = recentProjectsList.getByTestId("project-list-item")

    await expect(recentProjectsList).toBeVisible()
    await expect(projectItems).toHaveCount(27)

    const secondProject = projectItems.nth(1)
    const removeButton = secondProject.getByRole("button", { name: "Remove" })
    await expect(removeButton).toBeVisible()
    await removeButton.click()

    const postedCommands =
      await dashboardPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandMainMenu")
    expect(lastCommand.payload.action).toEqual("virtual.recent.remove")
    expect(lastCommand.payload.index).toEqual(1)
  })
})

test.describe("Community Feed tests", () => {
  test("Confirm default feed items", async ({ dashboardPage, page }) => {
    await dashboardPage.gotoPage()

    await expect(page.getByText("Community Feed")).toBeVisible()

    const feedFilter = page.getByTestId("community-feed-filter-bar")
    await expect(feedFilter).toBeVisible()

    const allFilterButton = page.getByRole("button", { name: "All" })
    await expect(allFilterButton).toBeVisible()
    await expect(allFilterButton).toHaveCount(1)

    const communityFilterButton = page.getByRole("button", {
      name: "Community",
    })
    await expect(communityFilterButton).toBeVisible()
    await expect(communityFilterButton).toHaveCount(1)

    const offersFilterButton = page.getByRole("button", { name: "Offers" })
    await expect(offersFilterButton).toBeVisible()
    await expect(offersFilterButton).toHaveCount(1)

    const eventsFilterButton = page.getByRole("button", { name: "Events" })
    await expect(eventsFilterButton).toBeVisible()
    await expect(eventsFilterButton).toHaveCount(1)

    const feedItems = page.getByTestId("community-feed-item")
    await expect(feedItems).toHaveCount(4)

    await communityFilterButton.click()
    await expect(feedItems).toHaveCount(2)

    await offersFilterButton.click()
    await expect(feedItems).toHaveCount(1)

    await eventsFilterButton.click()
    await expect(feedItems).toHaveCount(1)
  })

  test("Confirm button links are working correctly", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()
    const feedItems = page.getByTestId("community-feed-item")
    await expect(feedItems).toHaveCount(4)
    const offerItem = feedItems.nth(0)
    const offerButton = offerItem.getByRole("button", {
      name: "Support Us!",
      exact: true,
    })
    await expect(offerButton).toBeVisible()

    await dashboardPage.mobiFlightPage.trackCommand("CommandOpenLinkInBrowser")
    await offerButton.click()

    const postedCommands =
      await dashboardPage.mobiFlightPage.getTrackedCommands()
    const lastCommand = postedCommands!.pop()
    expect(lastCommand.key).toEqual("CommandOpenLinkInBrowser")
    expect(lastCommand.payload.url).toEqual("https://mobiflight.com/donate")
  })

  test("Confirm responsiveness small window size", async ({
    dashboardPage,
    page,
  }) => {
    await dashboardPage.gotoPage()
    await page.setViewportSize({ width: 500, height: 800 })

    const feedTitle = page.getByText("Community Feed")
    await expect(feedTitle).not.toBeVisible()

    const dashBoardNav = page.getByTestId("dashboard-nav")
    await expect(dashBoardNav).toBeVisible()

    const communityNavButton = dashBoardNav.getByRole("button", {
      name: "Community",
    })
    await expect(communityNavButton).toBeVisible()
    await expect(communityNavButton).toHaveCount(1)

    await communityNavButton.click()
    await expect(feedTitle).toBeVisible()
  })
})
