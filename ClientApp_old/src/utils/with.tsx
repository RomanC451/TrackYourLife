import { DateOnly } from "./date";

const withDate = <P extends object>(
  WrappedComponent: React.ComponentType<P>,
  date: DateOnly,
): React.FC<Omit<P, "date">> => {
  return (props) => <WrappedComponent {...(props as P)} date={date} />;
};

export default withDate;

export const withOnSuccess = <P extends { onSuccess: () => void }>(
  WrappedComponent: React.ComponentType<P>,
  onSuccess: () => void,
): React.FC<Omit<P, "onSuccess">> => {
  return (props) => (
    <WrappedComponent {...(props as P)} onSuccess={onSuccess} />
  );
};

export const withProp = <P extends Record<string, any>, K extends keyof P>(
  WrappedComponent: React.ComponentType<P>,
  propName: K,
  propValue: P[K],
): React.FC<Omit<P, K>> => {
  return (props) => (
    <WrappedComponent {...(props as P)} {...{ [propName]: propValue }} />
  );
};

export const withProps = <P extends Record<string, any>, K extends Partial<P>>(
  WrappedComponent: React.ComponentType<P>,
  injectedProps: K,
): React.FC<Omit<P, keyof K>> => {
  return (props) => <WrappedComponent {...(props as P)} {...injectedProps} />;
};
