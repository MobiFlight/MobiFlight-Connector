import { MouseSensor, MouseSensorOptions } from '@dnd-kit/core';

class CustomMouseSensor extends MouseSensor {
  static activators = [
    {
      eventName: 'onMouseDown' as const,
      handler: (
        {nativeEvent: event}: { nativeEvent: MouseEvent },
        {onActivation}: MouseSensorOptions
      ) => {
        // Only activate on primary mouse button (left click)
        if (event.button !== 0) {
          return false;
        }

        // Don't start drag if modifier keys are pressed
        if (event.ctrlKey || event.shiftKey || event.metaKey) {
          return false;
        }

        onActivation?.({event});

        return true;
      },
    },
  ];
}

export { CustomMouseSensor };