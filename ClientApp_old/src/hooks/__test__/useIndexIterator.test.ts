// import { renderHook } from "@testing-library/react";

// import useIndexIterator from "../useIndexIterator";

// test("Testing starting index, next and previos functions", () => {
//   const { result } = renderHook(() => useIndexIterator(10, 2));
//   const [index, next, prev] = result.current;
//   expect(index.current).toBe(2);
//   next();
//   expect(index.current).toBe(3);
//   prev();
//   expect(index.current).toBe(2);
// });

// test("Testing continous property enabled", () => {
//   const { result } = renderHook(() => useIndexIterator(4, 3));
//   const [index, next, prev] = result.current;
//   next();
//   expect(index.current).toBe(0);
//   prev();
//   expect(index.current).toBe(3);
// });

// test("Testing continous property disabled", () => {
//   const { result } = renderHook(() => useIndexIterator(4, 3, false));
//   const [index, next, prev] = result.current;
//   next();
//   expect(index.current).toBe(3);
//   prev();
//   prev();
//   prev();
//   expect(index.current).toBe(0);
//   prev();
//   expect(index.current).toBe(0);
// });

// test("Should trow error when starting index bigger than array length", () => {
//   expect(() => useIndexIterator(1, 3, false)).toThrow(
//     "Starting index must be less than array length"
//   );
// });
