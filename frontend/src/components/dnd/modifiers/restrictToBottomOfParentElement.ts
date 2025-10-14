import type {ClientRect} from '@dnd-kit/core';
import type {Transform} from '@dnd-kit/utilities';
import type {Modifier} from '@dnd-kit/core';

function restrictToBoundingRect(
  transform: Transform,
  rect: ClientRect,
  boundingRect: ClientRect
): Transform {
  const value = {
    ...transform,
  };

  if (
    rect.bottom + transform.y >=
    boundingRect.top + boundingRect.height
  ) {
    value.y = boundingRect.top + boundingRect.height - rect.bottom;
  }

  if (rect.left + transform.x <= boundingRect.left) {
    value.x = boundingRect.left - rect.left;
  } else if (
    rect.right + transform.x >=
    boundingRect.left + boundingRect.width
  ) {
    value.x = boundingRect.left + boundingRect.width - rect.right;
  }

  return value;
}


export const restrictToBottomOfParentElement: Modifier = ({
  containerNodeRect,
  draggingNodeRect,
  transform,
}) => {
  if (!draggingNodeRect || !containerNodeRect) {
    return transform;
  }

  return restrictToBoundingRect(transform, draggingNodeRect, containerNodeRect);
};