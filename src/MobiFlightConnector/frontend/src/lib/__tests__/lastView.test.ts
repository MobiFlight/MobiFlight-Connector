import { describe, it, expect } from "vitest"
import { getMainViewFromPath, getStartupRoute } from "../lastView"
import type { Project } from "@/types"

describe("lastView - getMainViewFromPath", () => {
  it("should return dashboard for the home routes", () => {
    // Arrange
    const paths = ["/home", "/home/projects"]

    // Act
    const views = paths.map(getMainViewFromPath)

    // Assert
    expect(views).toEqual(["dashboard", "dashboard"])
  })

  it("should return config for the config list routes", () => {
    // Arrange
    const paths = ["/config", "/config/some-config-id"]

    // Act
    const views = paths.map(getMainViewFromPath)

    // Assert
    expect(views).toEqual(["config", "config"])
  })

  it("should return null for all other routes", () => {
    // Arrange
    const paths = ["/", "/start", "/index.html", "/auth/login", "/homepage"]

    // Act
    const views = paths.map(getMainViewFromPath)

    // Assert
    expect(views).toEqual([null, null, null, null, null])
  })
})

describe("lastView - getStartupRoute", () => {
  const loadedProject = {
    Name: "Test Project",
    FilePath: "C:\\Projects\\Test.mfproj",
  } as Project

  const unsavedProject = {
    Name: "",
    FilePath: "",
  } as Project

  it("should return the config list if it was the last view and a project got loaded", () => {
    // Act
    const route = getStartupRoute("config", loadedProject)

    // Assert
    expect(route).toBe("/config")
  })

  it("should return the dashboard if it was the last view", () => {
    // Act
    const route = getStartupRoute("dashboard", loadedProject)

    // Assert
    expect(route).toBe("/home")
  })

  it("should return the dashboard if no view was remembered yet", () => {
    // Act
    const route = getStartupRoute(null, loadedProject)

    // Assert
    expect(route).toBe("/home")
  })

  it("should return the dashboard if no project file got loaded", () => {
    // Act
    const routeWithoutProject = getStartupRoute("config", null)
    const routeWithUnsavedProject = getStartupRoute("config", unsavedProject)

    // Assert
    expect(routeWithoutProject).toBe("/home")
    expect(routeWithUnsavedProject).toBe("/home")
  })
})
