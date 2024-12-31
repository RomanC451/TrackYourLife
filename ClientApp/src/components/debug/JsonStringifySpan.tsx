function JsonStringifySpan({ object }: { object: unknown }) {
  return (
    <span className="text-wrap">
      <pre>{JSON.stringify(object, null, 2)} </pre>
    </span>
  );
}

export default JsonStringifySpan;
