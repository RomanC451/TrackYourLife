import { DateOnly } from "./date";

const withDate = <P extends object>(
  WrappedComponent: React.ComponentType<P>,
  date: DateOnly,
): React.ComponentType<Omit<P, "date">> => {
  return (props) => <WrappedComponent {...(props as P)} date={date} />;
};

export default withDate;

export const withOnSuccess = <P extends { onSuccess: () => void }>(
  WrappedComponent: React.ComponentType<P>,
  onSuccess: () => void,
): React.ComponentType<Omit<P, "onSuccess">> => {
  return (props) => (
    <WrappedComponent {...(props as P)} onSuccess={onSuccess} />
  );
};

export const withProp = <P extends Record<string, unknown>, K extends keyof P>(
  WrappedComponent: React.ComponentType<P>,
  propName: K,
  propValue: P[K],
): React.ComponentType<Omit<P, K>> => {
  return (props) => (
    <WrappedComponent {...(props as P)} {...{ [propName]: propValue }} />
  );
};

export const withProps = <
  P extends Record<string, unknown>,
  K extends Partial<P>,
>(
  WrappedComponent: React.ComponentType<P>,
  injectedProps: K,
): React.ComponentType<Omit<P, keyof K>> => {
  return (props) => <WrappedComponent {...(props as P)} {...injectedProps} />;
};
