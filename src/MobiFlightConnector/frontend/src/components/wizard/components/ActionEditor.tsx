import { Card, CardContent } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import ActionTypeComboBox from "@/components/wizard/components/ActionTypeComboBox"
import CopyPasteActionPanel from "@/components/wizard/components/CopyPasteActionPanel"
import EventIdInputActionPanel from "@/components/wizard/components/InputActions/EventIdInputActionPanel"
import FsuipcOffsetInputActionPanel from "@/components/wizard/components/InputActions/FsuipcOffsetInputActionPanel"
import JeehellInputActionPanel from "@/components/wizard/components/InputActions/JeehellInputActionPanel"
import KeyboardInputActionPanel from "@/components/wizard/components/InputActions/KeyboardInputActionPanel"
import LuaMacroInputActionPanel from "@/components/wizard/components/InputActions/LuaMacroInputActionPanel"
import MsfsInputActionPanel from "@/components/wizard/components/InputActions/MsfsInputActionPanel"
import ProSimInputActionPanel from "@/components/wizard/components/InputActions/ProSimInputActionPanel"
import RetriggerPanel from "@/components/wizard/components/InputActions/RetriggerPanel"
import { VariablePanel } from "@/components/wizard/components/InputActions/VariablePanel"
import VJoyInputActionPanel from "@/components/wizard/components/InputActions/VJoyInputActionPanel"
import XplaneInputActionPanel from "@/components/wizard/components/InputActions/XplaneInputActionPanel"
import { ActionTypeOptions } from "@/lib/configWizard"
import {
  Action,
  EventIdInputAction,
  FsuipcOffsetInputAction,
  JeehellInputAction,
  KeyInputAction,
  LuaMacroInputAction,
  MobiFlightVariableAction,
  MsfsInputAction,
  PmdgEventIdInputAction,
  ProSimInputAction,
  VJoyInputAction,
  XplaneInputAction,
} from "@/types/config"

export interface ActionEditorProps {
  action?: Action
  onActionChange: (item: Action) => void
}

const ActionEditor = ({ action, onActionChange }: ActionEditorProps) => {
  const selectedActionType = action
    ? ActionTypeOptions.find((option) => option.value === action.Type)
    : undefined

  return (
    <Card data-testid="action-editor">
      <CardContent className="pt-4">
        <div className="flex flex-col gap-4">
          <div className="flex flex-row justify-between items-end">
            <ActionTypeComboBox
              selectedActionType={selectedActionType}
              setSelectedActionType={(option) => {
                if (option) {
                  onActionChange({ ...action, Type: option.value })
                }
              }}
            />
            <CopyPasteActionPanel
              action={action}
              onActionChange={(newAction) => {
                onActionChange(newAction)
              }}
            />
          </div>
          <Separator />
          {selectedActionType?.value === "MSFS2020CustomInputAction" && (
            <MsfsInputActionPanel
              config={action ? (action as MsfsInputAction) : null}
              onConfigChange={(config) => onActionChange(config)}
            />
          )}
          {selectedActionType?.value === "XplaneInputAction" && (
            <XplaneInputActionPanel
              config={action as XplaneInputAction}
              onConfigChange={(c) => onActionChange(c)}
            />
          )}
          {selectedActionType?.value === "VariableInputAction" && (
            <VariablePanel
              currentVariable={
                action
                  ? (action as MobiFlightVariableAction).Variable
                  : undefined
              }
              onVariableChange={(variable) =>
                onActionChange({
                  ...(action as MobiFlightVariableAction),
                  Variable: variable,
                } as MobiFlightVariableAction)
              }
            />
          )}

          {selectedActionType?.value === "RetriggerInputAction" && (
            <RetriggerPanel />
          )}

          {selectedActionType?.value === "VJoyInputAction" && (
            <VJoyInputActionPanel
              config={action ? (action as VJoyInputAction) : null}
              setConfig={(config) =>
                onActionChange({
                  ...(action as VJoyInputAction),
                  ...config,
                } as VJoyInputAction)
              }
            />
          )}

          {selectedActionType?.value === "KeyInputAction" && (
            <KeyboardInputActionPanel
              config={action ? (action as KeyInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as KeyInputAction),
                  ...config,
                } as KeyInputAction)
              }
            />
          )}

          {selectedActionType?.value === "FsuipcOffsetInputAction" && (
            <FsuipcOffsetInputActionPanel
              config={action ? (action as FsuipcOffsetInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as FsuipcOffsetInputAction),
                  ...config,
                } as FsuipcOffsetInputAction)
              }
            />
          )}

          {selectedActionType?.value === "ProSimInputAction" && (
            <ProSimInputActionPanel
              config={action ? (action as ProSimInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as ProSimInputAction),
                  ...config,
                } as ProSimInputAction)
              }
            />
          )}

          {selectedActionType?.value === "LuaMacroInputAction" && (
            <LuaMacroInputActionPanel
              config={action ? (action as LuaMacroInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as LuaMacroInputAction),
                  ...config,
                } as LuaMacroInputAction)
              }
            />
          )}

          {selectedActionType?.value === "JeehellInputAction" && (
            <JeehellInputActionPanel
              config={action ? (action as JeehellInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as JeehellInputAction),
                  ...config,
                } as JeehellInputAction)
              }
            />
          )}

          {selectedActionType?.value === "EventIdInputAction" && (
            <EventIdInputActionPanel
              variant="default"
              config={action ? (action as EventIdInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as EventIdInputAction),
                  ...config,
                } as EventIdInputAction)
              }
            />
          )}

          {selectedActionType?.value === "PmdgEventIdInputAction" && (
            <EventIdInputActionPanel
              variant="pmdg"
              config={action ? (action as PmdgEventIdInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as PmdgEventIdInputAction),
                  ...config,
                } as PmdgEventIdInputAction)
              }
            />
          )}
        </div>
      </CardContent>
    </Card>
  )
}
export default ActionEditor
