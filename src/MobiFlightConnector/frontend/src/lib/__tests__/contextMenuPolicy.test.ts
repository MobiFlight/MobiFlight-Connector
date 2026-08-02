import { describe, expect, it } from "vitest"
import { allowsNativeTextContextMenu } from "../contextMenuPolicy"

describe("allowsNativeTextContextMenu", () => {
  it("allows editable and read-only text inputs", () => {
    expect(
      allowsNativeTextContextMenu({
        kind: "input",
        inputType: "text",
        disabled: false,
        readOnly: false,
      }),
    ).toBe(true)
    expect(
      allowsNativeTextContextMenu({
        kind: "input",
        inputType: "text",
        disabled: false,
        readOnly: true,
      }),
    ).toBe(true)
  })

  it("allows text-like input types", () => {
    for (const inputType of [
      "email",
      "number",
      "password",
      "search",
      "tel",
      "url",
    ]) {
      expect(
        allowsNativeTextContextMenu({
          kind: "input",
          inputType,
          disabled: false,
          readOnly: false,
        }),
      ).toBe(true)
    }
  })

  it("rejects disabled and non-text inputs", () => {
    expect(
      allowsNativeTextContextMenu({
        kind: "input",
        inputType: "text",
        disabled: true,
        readOnly: false,
      }),
    ).toBe(false)
    expect(
      allowsNativeTextContextMenu({
        kind: "input",
        inputType: "checkbox",
        disabled: false,
        readOnly: false,
      }),
    ).toBe(false)
  })

  it("allows enabled and read-only textareas but rejects disabled textareas", () => {
    expect(
      allowsNativeTextContextMenu({
        kind: "textarea",
        disabled: false,
        readOnly: false,
      }),
    ).toBe(true)
    expect(
      allowsNativeTextContextMenu({
        kind: "textarea",
        disabled: false,
        readOnly: true,
      }),
    ).toBe(true)
    expect(
      allowsNativeTextContextMenu({
        kind: "textarea",
        disabled: true,
        readOnly: false,
      }),
    ).toBe(false)
  })

  it("allows contenteditable targets and rejects other page content", () => {
    expect(allowsNativeTextContextMenu({ kind: "contenteditable" })).toBe(true)
    expect(allowsNativeTextContextMenu({ kind: "other" })).toBe(false)
  })
})
