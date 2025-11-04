import type {Modifier} from '@dnd-kit/core';
import {getEventCoordinates} from '@dnd-kit/utilities';

export const snapToCursor: Modifier = ({
  activatorEvent,
  draggingNodeRect,
  transform,
}) => {
  if (draggingNodeRect && activatorEvent) {
    const activatorCoordinates = getEventCoordinates(activatorEvent);

    if (!activatorCoordinates) {
      return transform;
    }

    const offsetY = activatorCoordinates.y - draggingNodeRect.top;
    const offsetX = activatorCoordinates.x - draggingNodeRect.left;

    return {
      ...transform,
      x: transform.x + offsetX,
      y: transform.y + offsetY - draggingNodeRect.height / 2,
    };
  }

  return transform;
};