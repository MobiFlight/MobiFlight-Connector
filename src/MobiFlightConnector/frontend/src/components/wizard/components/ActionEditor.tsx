import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
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
  ButtonHoldOptions,
  ButtonLongReleaseOptions,
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
  event: { variant: string; event: string }
  buttonOptions?: Partial<ButtonHoldOptions> & Partial<ButtonLongReleaseOptions>
  action: Action | null
  onActionChange: (
    item: Action | null,
    buttonOptions?: Partial<ButtonHoldOptions> &
      Partial<ButtonLongReleaseOptions>,
  ) => void
}
export interface ActionSummaryProps {
  action?: Action
}
export const ActionSummary = ({ action }: ActionSummaryProps) => {
  const { t } = useTranslation()
  if (!action)
    return <span className="text-muted-foreground text-sm">No Action.</span>
  const selectedActionType = action
    ? ActionTypeOptions.find((option) => option.value === action.Type)
    : undefined
  const typeOption = ActionTypeOptions.find(
    (option) => option.value === action.Type,
  )
  const actionTypeLabel = typeOption ? typeOption.value : action.Type
  return (
    <div className="flex grow flex-row items-center gap-8">
      <div className="flex w-32 flex-col gap-1">
        <Label>
          {t("Dialog.InputConfigWizard.InputActions.Common.ActionLabel")}:
        </Label>
        <Badge
          variant={"outline"}
          className="w-fit"
          title={t(
            `Dialog.InputConfigWizard.ActionType.Options.${actionTypeLabel}.label`,
          )}
        >
          {t(
            `Dialog.InputConfigWizard.ActionType.Options.${actionTypeLabel}.short`,
          )}
        </Badge>
      </div>
      {selectedActionType?.value === "MSFS2020CustomInputAction" && (
        <MsfsInputActionPanel
          variant="summary"
          config={action ? (action as MsfsInputAction) : null}
          onConfigChange={() => {}}
        />
      )}
      {selectedActionType?.value === "XplaneInputAction" && (
        <XplaneInputActionPanel
          variant="summary"
          config={action as XplaneInputAction}
          onConfigChange={() => {}}
        />
      )}
      {selectedActionType?.value === "VariableInputAction" && (
        <VariablePanel
          variant="summary"
          currentVariable={
            action ? (action as MobiFlightVariableAction).Variable : undefined
          }
          onVariableChange={() => {}}
        />
      )}
      {selectedActionType?.value === "RetriggerInputAction" && (
        <RetriggerPanel variant="summary" />
      )}
      {selectedActionType?.value === "VJoyInputAction" && (
        <VJoyInputActionPanel
          variant="summary"
          config={action ? (action as VJoyInputAction) : null}
          setConfig={() => {}}
        />
      )}
      {selectedActionType?.value === "KeyInputAction" && (
        <KeyboardInputActionPanel
          variant="summary"
          config={action ? (action as KeyInputAction) : null}
          onConfigChange={() => {}}
        />
      )}
      {selectedActionType?.value === "FsuipcOffsetInputAction" && (
        <FsuipcOffsetInputActionPanel
          variant="summary"
          config={action ? (action as FsuipcOffsetInputAction) : null}
          onConfigChange={() => {}}
        />
      )}
      {selectedActionType?.value === "ProSimInputAction" && (
        <ProSimInputActionPanel
          variant="summary"
          config={action ? (action as ProSimInputAction) : null}
          onConfigChange={() => {}}
        />
      )}
      {selectedActionType?.value === "LuaMacroInputAction" && (
        <LuaMacroInputActionPanel
          variant="summary"
          config={action ? (action as LuaMacroInputAction) : null}
          onConfigChange={() => {}}
        />
      )}
      {selectedActionType?.value === "JeehellInputAction" && (
        <JeehellInputActionPanel
          variant="summary"
          config={action ? (action as JeehellInputAction) : null}
          onConfigChange={() => {}}
        />
      )}
      {selectedActionType?.value === "EventIdInputAction" && (
        <EventIdInputActionPanel
          variant="summary"
          options="default"
          config={action ? (action as EventIdInputAction) : null}
          onConfigChange={() => {}}
        />
      )}
      {selectedActionType?.value === "PmdgEventIdInputAction" && (
        <EventIdInputActionPanel
          variant="summary"
          options="pmdg"
          config={action ? (action as PmdgEventIdInputAction) : null}
          onConfigChange={() => {}}
        />
      )}
    </div>
  )
}

const ActionPanel = ({
  action,
  onActionChange,
}: {
  action: Action | null
  onActionChange: (action: Action) => void
}) => {
  if (!action) return null

  switch (action.Type) {
    case "MSFS2020CustomInputAction":
      return (
        <MsfsInputActionPanel
          variant="details"
          config={action ? (action as MsfsInputAction) : null}
          onConfigChange={(config) => onActionChange(config)}
        />
      )
    case "XplaneInputAction":
      return (
        <XplaneInputActionPanel
          variant="details"
          config={action ? (action as XplaneInputAction) : null}
          onConfigChange={(c) => onActionChange(c)}
        />
      )
    case "VariableInputAction":
      return (
        <VariablePanel
          variant="details"
          currentVariable={
            action ? (action as MobiFlightVariableAction).Variable : undefined
          }
          onVariableChange={(variable) =>
            onActionChange({
              ...(action as MobiFlightVariableAction),
              Variable: variable,
            } as MobiFlightVariableAction)
          }
        />
      )
    case "RetriggerInputAction":
      return <RetriggerPanel variant="details" />

    case "VJoyInputAction":
      return (
        <VJoyInputActionPanel
          variant="details"
          config={action ? (action as VJoyInputAction) : null}
          setConfig={(config) =>
            onActionChange({
              ...(action as VJoyInputAction),
              ...config,
            } as VJoyInputAction)
          }
        />
      )

    case "KeyInputAction":
      return (
        <KeyboardInputActionPanel
          variant="details"
          config={action ? (action as KeyInputAction) : null}
          onConfigChange={(config) =>
            onActionChange({
              ...(action as KeyInputAction),
              ...config,
            } as KeyInputAction)
          }
        />
      )

    case "FsuipcOffsetInputAction":
      return (
        <FsuipcOffsetInputActionPanel
          variant="details"
          config={action ? (action as FsuipcOffsetInputAction) : null}
          onConfigChange={(config) =>
            onActionChange({
              ...(action as FsuipcOffsetInputAction),
              ...config,
            } as FsuipcOffsetInputAction)
          }
        />
      )
    case "ProSimInputAction":
      return (
        <ProSimInputActionPanel
          variant="details"
          config={action ? (action as ProSimInputAction) : null}
          onConfigChange={(config) =>
            onActionChange({
              ...(action as ProSimInputAction),
              ...config,
            } as ProSimInputAction)
          }
        />
      )
    case "LuaMacroInputAction":
      return (
        <LuaMacroInputActionPanel
          variant="details"
          config={action ? (action as LuaMacroInputAction) : null}
          onConfigChange={(config) =>
            onActionChange({
              ...(action as LuaMacroInputAction),
              ...config,
            } as LuaMacroInputAction)
          }
        />
      )
    case "JeehellInputAction":
      return (
        <JeehellInputActionPanel
          variant="details"
          config={action ? (action as JeehellInputAction) : null}
          onConfigChange={(config) =>
            onActionChange({
              ...(action as JeehellInputAction),
              ...config,
            } as JeehellInputAction)
          }
        />
      )
    case "EventIdInputAction":
      return (
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
        />
      )
    case "PmdgEventIdInputAction":
      return (
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
        />
      )
  }
}

const ButtonOnHoldOptions = ({
  event,
  buttonOptions,
  action,
  onActionChange,
}: ActionEditorProps) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-1 flex-col gap-1">
      <div className="flex flex-row items-center gap-2 [&_span]:text-sm">
        <span
          title={t(
            `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.options.holdDelay.description`,
          )}
        >
          {t(
            `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.options.holdDelay.label`,
          )}
        </span>
        <Input
          aria-label={t(
            `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.options.holdDelay.label`,
          )}
          className="w-16"
          value={buttonOptions?.HoldDelay}
          onChange={(e) => {
            const value = e.target.value
            onActionChange(action, {
              HoldDelay: value ? parseInt(value) : undefined,
              RepeatDelay: buttonOptions?.RepeatDelay,
            })
          }}
        />
        <span
          title={t(
            `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.options.repeatDelay.description`,
          )}
        >
          {t(
            `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.options.repeatDelay.label`,
          )}
        </span>
        <Input
          aria-label={t(
            `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.options.repeatDelay.label`,
          )}
          className="w-16"
          value={buttonOptions?.RepeatDelay}
          onChange={(e) => {
            const value = e.target.value
            onActionChange(action, {
              HoldDelay: buttonOptions?.HoldDelay,
              RepeatDelay: value ? parseInt(value) : undefined,
            })
          }}
        />
        <span>ms</span>
      </div>
    </div>
  )
}

const ButtonOnLongReleaseOptions = ({
  event,
  buttonOptions,
  action,
  onActionChange,
}: ActionEditorProps) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-1 flex-col gap-1">
      <div className="flex flex-row items-center gap-2 [&_span]:text-sm">
        <span
          title={t(
            `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.options.longReleaseDelay.description`,
          )}
        >
          {t(
            `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.options.longReleaseDelay.label`,
          )}
        </span>
        <Input
          aria-label={t(
            `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.options.longReleaseDelay.label`,
          )}
          className="w-16"
          onChange={(e) => {
            const value = e.target.value
            onActionChange(action, {
              LongReleaseDelay: value ? parseInt(value) : undefined,
            })
          }}
          value={buttonOptions?.LongReleaseDelay}
        />
        <span className="whitespace-nowrap">ms</span>
      </div>
    </div>
  )
}

const ActionEditor = ({
  event,
  buttonOptions,
  action,
  onActionChange,
}: ActionEditorProps) => {
  const { t } = useTranslation()
  const selectedActionType = action
    ? ActionTypeOptions.find((option) => option.value === action.Type)
    : undefined
  return (
    <div className="flex flex-col gap-4" data-testid="action-editor">
      <div className="flex flex-col gap-4 rounded-lg border px-6 py-4 shadow">
        <div className="flex flex-1 flex-col">
          <div className="text-lg font-semibold">
            {t(
              `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.label`,
            )}
          </div>
          <div className="text-muted-foreground text-sm">
            {t(
              `Dialog.InputConfigWizard.${event.variant}.Event.${event.event}.description`,
            )}
          </div>
        </div>
        {event.variant === "button" && event.event === "onHold" && (
          <ButtonOnHoldOptions
            event={event}
            buttonOptions={buttonOptions}
            action={action}
            onActionChange={onActionChange}
          />
        )}
        {event.variant === "button" && event.event === "onLongRelease" && (
          <ButtonOnLongReleaseOptions
            event={event}
            buttonOptions={buttonOptions}
            action={action}
            onActionChange={onActionChange}
          />
        )}
      </div>
      <Card>
        <CardContent className="pt-4">
          <div className="flex flex-col gap-4">
            <div className="flex flex-row items-end justify-between">
              <ActionTypeComboBox
                selectedActionType={selectedActionType}
                setSelectedActionType={(option) => {
                  onActionChange(
                    option ? { ...action, Type: option.value } : null,
                  )
                }}
              />
              <CopyPasteActionPanel
                action={action}
                onActionChange={(newAction) => {
                  onActionChange(newAction)
                }}
              />
            </div>
          </div>
        </CardContent>
      </Card>
      <ActionPanel onActionChange={onActionChange} action={action} />
    </div>
  )
}
export default ActionEditor
