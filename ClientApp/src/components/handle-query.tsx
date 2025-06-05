import { UseQueryResult } from "@tanstack/react-query";

function handleQuery<T>(
  query: UseQueryResult<T, Error>,
  success: (data: T) => React.ReactNode,
) {
  if (query.isPending) {
    return <div>Loading...</div>;
  }

  if (query.isError) {
    return <div>Error: {query.error.message}</div>;
  }

  return success(query.data);
}

export default handleQuery;
