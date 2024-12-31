function BooleanSpan({ state }: { state: boolean }) {
  return (
    <span className={state ? "text-green-500" : "text-red-500"}>
      {state ? "true" : "false"}
    </span>
  );
}

export default BooleanSpan;
