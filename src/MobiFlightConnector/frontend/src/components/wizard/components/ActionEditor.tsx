import { Card, CardContent } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
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
import { useTranslation } from "react-i18next"

export interface ActionEditorProps {
  action: Action | null
  onActionChange: (item: Action) => void
}

export interface ActionSummaryProps {
  action?: Action
  onActionEdit: () => void
}

export const ActionSummary = ({ action, onActionEdit }: ActionSummaryProps) => {
  const { t } = useTranslation()
  if (!action)
    return <span className="text-muted-foreground text-sm">No Action.</span>

  const selectedActionType = action
    ? ActionTypeOptions.find((option) => option.value === action.Type)
    : undefined

  const typeOption = ActionTypeOptions.find(
    (option) => option.value === action.Type,
  )
  const actionTypeLavbel = typeOption ? typeOption.value : action.Type
  return (
    <div className="flex grow flex-row items-center gap-8">
      <div className="flex w-32 flex-col gap-1 truncate">
        <Label>Action:</Label>
        <span
          className="truncate"
          title={t(
            `Dialog.InputConfigWizard.ActionType.Options.${actionTypeLavbel}`,
          )}
        >
          {t(`Dialog.InputConfigWizard.ActionType.Options.${actionTypeLavbel}`)}
        </span>
      </div>
      {selectedActionType?.value === "MSFS2020CustomInputAction" && (
        <MsfsInputActionPanel
          variant="summary"
          config={action ? (action as MsfsInputAction) : null}
          onConfigChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}
      {selectedActionType?.value === "XplaneInputAction" && (
        <XplaneInputActionPanel
          variant="summary"
          config={action as XplaneInputAction}
          onConfigChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}
      {selectedActionType?.value === "VariableInputAction" && (
        <VariablePanel
          variant="summary"
          currentVariable={
            action ? (action as MobiFlightVariableAction).Variable : undefined
          }
          onVariableChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}

      {selectedActionType?.value === "RetriggerInputAction" && (
        <RetriggerPanel 
          variant="summary"
          onEditAction={() => onActionEdit()}
        />
      )}

      {selectedActionType?.value === "VJoyInputAction" && (
        <VJoyInputActionPanel
          variant="summary"
          config={action ? (action as VJoyInputAction) : null}
          setConfig={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}

      {selectedActionType?.value === "KeyInputAction" && (
        <KeyboardInputActionPanel
          variant="summary"
          config={action ? (action as KeyInputAction) : null}
          onConfigChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}

      {selectedActionType?.value === "FsuipcOffsetInputAction" && (
        <FsuipcOffsetInputActionPanel
          variant="summary"
          config={action ? (action as FsuipcOffsetInputAction) : null}
          onConfigChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}

      {selectedActionType?.value === "ProSimInputAction" && (
        <ProSimInputActionPanel
          variant="summary"
          config={action ? (action as ProSimInputAction) : null}
          onConfigChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}

      {selectedActionType?.value === "LuaMacroInputAction" && (
        <LuaMacroInputActionPanel
          variant="summary"
          config={action ? (action as LuaMacroInputAction) : null}
          onConfigChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}

      {selectedActionType?.value === "JeehellInputAction" && (
        <JeehellInputActionPanel
          variant="summary"
          config={action ? (action as JeehellInputAction) : null}
          onConfigChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}

      {selectedActionType?.value === "EventIdInputAction" && (
        <EventIdInputActionPanel
          variant="summary"
          options="default"
          config={action ? (action as EventIdInputAction) : null}
          onConfigChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}

      {selectedActionType?.value === "PmdgEventIdInputAction" && (
        <EventIdInputActionPanel
          variant="summary"
          options="pmdg"
          config={action ? (action as PmdgEventIdInputAction) : null}
          onConfigChange={() => {}}
          onEditAction={() => onActionEdit()}
        />
      )}
    </div>
  )
}

const ActionEditor = ({ action, onActionChange }: ActionEditorProps) => {
  const selectedActionType = action
    ? ActionTypeOptions.find((option) => option.value === action.Type)
    : undefined

  return (
    <Card data-testid="action-editor">
      <CardContent className="pt-4">
        <div className="flex flex-col gap-4">
          <div className="flex flex-row items-end justify-between">
            <ActionTypeComboBox
              selectedActionType={selectedActionType}
              setSelectedActionType={(option) => {
                onActionChange({ ...action, Type: option?.value ?? null })
              }}
            />
            <CopyPasteActionPanel
              action={action}
              onActionChange={(newAction) => {
                onActionChange(newAction)
              }}
            />
          </div>
          { selectedActionType?.value && <Separator /> }
          {selectedActionType?.value === "MSFS2020CustomInputAction" && (
            <MsfsInputActionPanel
              variant="details"
              config={action ? (action as MsfsInputAction) : null}
              onConfigChange={(config) => onActionChange(config)}
              onEditAction={() => {}}
            />
          )}
          {selectedActionType?.value === "XplaneInputAction" && (
            <XplaneInputActionPanel
              variant="details"
              config={action ? (action as XplaneInputAction) : null}
              onConfigChange={(c) => onActionChange(c)}
              onEditAction={() => {}}
            />
          )}
          {selectedActionType?.value === "VariableInputAction" && (
            <VariablePanel
              variant="details"
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
              onEditAction={() => {}}
            />
          )}

          {selectedActionType?.value === "RetriggerInputAction" && (
            <RetriggerPanel 
              variant="details"
              onEditAction={() => {}}
            />
          )}

          {selectedActionType?.value === "VJoyInputAction" && (
            <VJoyInputActionPanel
              variant="details"
              config={action ? (action as VJoyInputAction) : null}
              setConfig={(config) =>
                onActionChange({
                  ...(action as VJoyInputAction),
                  ...config,
                } as VJoyInputAction)
              }
              onEditAction={() => {}}
            />
          )}

          {selectedActionType?.value === "KeyInputAction" && (
            <KeyboardInputActionPanel
              variant="details"
              config={action ? (action as KeyInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as KeyInputAction),
                  ...config,
                } as KeyInputAction)
              }
              onEditAction={() => {}}
            />
          )}

          {selectedActionType?.value === "FsuipcOffsetInputAction" && (
            <FsuipcOffsetInputActionPanel
              variant="details"
              config={action ? (action as FsuipcOffsetInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as FsuipcOffsetInputAction),
                  ...config,
                } as FsuipcOffsetInputAction)
              }
              onEditAction={() => {}}
            />
          )}

          {selectedActionType?.value === "ProSimInputAction" && (
            <ProSimInputActionPanel
              variant="details"
              config={action ? (action as ProSimInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as ProSimInputAction),
                  ...config,
                } as ProSimInputAction)
              }
              onEditAction={() => {}}
            />
          )}

          {selectedActionType?.value === "LuaMacroInputAction" && (
            <LuaMacroInputActionPanel
              variant="details"
              config={action ? (action as LuaMacroInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as LuaMacroInputAction),
                  ...config,
                } as LuaMacroInputAction)
              }
              onEditAction={() => {}}
            />
          )}

          {selectedActionType?.value === "JeehellInputAction" && (
            <JeehellInputActionPanel
              variant="details"
              config={action ? (action as JeehellInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as JeehellInputAction),
                  ...config,
                } as JeehellInputAction)
              }
              onEditAction={() => {}}
            />
          )}

          {selectedActionType?.value === "EventIdInputAction" && (
            <EventIdInputActionPanel
              variant="details"
              options="default"
              config={action ? (action as EventIdInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as EventIdInputAction),
                  ...config,
                } as EventIdInputAction)
              }
              onEditAction={() => {}}
            />
          )}

          {selectedActionType?.value === "PmdgEventIdInputAction" && (
            <EventIdInputActionPanel
              variant="details"
              options="pmdg"
              config={action ? (action as PmdgEventIdInputAction) : null}
              onConfigChange={(config) =>
                onActionChange({
                  ...(action as PmdgEventIdInputAction),
                  ...config,
                } as PmdgEventIdInputAction)
              }
              onEditAction={() => {}}
            />
          )}
        </div>
      </CardContent>
    </Card>
  )
}
export default ActionEditor
