interface ConditionalWrapperProps {
  condition: boolean;
  wrapper: (children: React.ReactNode) => React.ReactNode;
  children: React.ReactNode;
}
const ConditionalWrapper = (props: ConditionalWrapperProps) => {
  const { condition, wrapper, children } = props;
  return condition ? wrapper(children) : children;
}

export default ConditionalWrapper;