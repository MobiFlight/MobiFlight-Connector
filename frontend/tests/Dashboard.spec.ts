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
      Controllers: [
        "ProtoBoard-v2/ SN-3F1-FDD",
        "MobiFlight Board / SN-12345",
        "Alpha Flight Controls / JS-67890",
        "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000",
        "miniCOCKPIT miniFCU/ SN-E98-277",
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
      "ProtoBoard-v2",
    )
    await expect(controllerIcons.nth(1)).toHaveAttribute(
      "title",
      "MobiFlight Board",
    )
    await expect(controllerIcons.nth(2)).toHaveAttribute(
      "title",
      "Alpha Flight Controls",
    )
    await expect(controllerIcons.nth(3)).toHaveAttribute(
      "title",
      "Bravo Throttle Quadrant",
    )
    await expect(controllerIcons.nth(4)).toHaveAttribute(
      "title",
      "miniCOCKPIT miniFCU",
    )
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
    const currentProjectCard = page.getByTestId("project-card")

    // Verify correct start and stop messages
    await currentProjectCard
      .getByTestId("project-card-start-stop-button")
      .click()
    await dashboardPage.mobiFlightPage.trackCommand("CommandProjectToolbar")
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

  test("Create new project", async ({ dashboardPage, page }) => {
    await dashboardPage.gotoPage()

    const createProjectButton = page.getByRole("button", { name: "Project" })
    const createProjectDialog = page.getByRole("dialog", {
      name: "Create New Project",
    })
    const fsuipcCheckbox = createProjectDialog.getByLabel("Use FSUIPC")
    const projectNameInput = createProjectDialog.getByLabel("Project Name")
    const createButton = createProjectDialog.getByRole("button", {
      name: "Create",
    })

    const projectOptions = [
      { name: "MSFS no FSUIPC", value: "msfs", useFsuipc: false },
      { name: "MSFS with FSUIPC", value: "msfs", useFsuipc: true },
      { name: "X-Plane", value: "xplane", useFsuipc: false },
      { name: "Prepar3D", value: "p3d", useFsuipc: false },
    ]
    
    for (const option of projectOptions) {
      await dashboardPage.mobiFlightPage.trackCommand("CommandMainMenu")
      
      await createProjectButton.click()
        
      await projectNameInput.fill(option.name)

      const simOptionLocator = `[role="radio"][value="${option.value}"]`
      const simOption = createProjectDialog.locator(simOptionLocator)

      await simOption.check()
      if (option.useFsuipc) {
        await fsuipcCheckbox.check()
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
      expect(lastCommand.payload.options.project.UseFsuipc).toEqual(
        option.useFsuipc,
      )

      await dashboardPage.gotoPage()
    }
  })

  test("Dont allow new project without a name", async ({ dashboardPage, page }) => {
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
    const errorMessage = createProjectDialog.getByTestId("form-project-name-error")
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
    const projectContextMenu = currentProjectCard.getByRole('button', { name: 'Open menu' })
    await projectContextMenu.click()

    const settingsMenuItem = page.getByRole("menuitem", { name: "Settings" })
    await expect(settingsMenuItem).toBeVisible()
    settingsMenuItem.click()

    const editProjectDialog = page.getByRole("dialog", {
      name: "Edit Project",
    })
    await expect(editProjectDialog).toBeVisible()
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
})
