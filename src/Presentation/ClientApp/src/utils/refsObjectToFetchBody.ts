/**
 * Converts an object of React refs to an object with their current values.
 *
 * @param refsObject - An object with keys as property names and values as React ref objects.
 * @returns An object with keys as property names and values as the current values of the React refs.
 */
function refsObjectToFetchBody(refsObject: {
  [key: string]: React.RefObject<number | string | boolean>;
}): { [key: string]: number | string | boolean } {
  const fetchBody: { [key: string]: number | string | boolean } = {};

  Object.keys(refsObject).forEach((key) => {
    fetchBody[key] = refsObject[key].current!;
  });

  return fetchBody;
}

export default refsObjectToFetchBody;
