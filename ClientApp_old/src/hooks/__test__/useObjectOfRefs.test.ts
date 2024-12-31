// import React from "react";
// import { renderHook } from "@testing-library/react";

// import useObjectOfRefs from "../useObjectOfRefs";

// describe("useObjectOfRefs tests", () => {
//   test("renders without crashing", () => {
//     renderHook(() => useObjectOfRefs({}));
//   });

//   test("rendering the hook with out passing the data argument should return an empty object", () => {
//     const { result } = renderHook(() => useObjectOfRefs({}));
//     const [refsObject, _] = result.current;
//     expect(refsObject).toEqual({});
//   });

//   test("Returned object of refs should have desired keys passed as arguments", () => {
//     const { result } = renderHook(({ data }) => useObjectOfRefs(data), {
//       initialProps: { data: { email: "", password: "" } },
//     });
//     const [refsObject, _] = result.current;
//     expect(Object.keys(refsObject)).toEqual(["email", "password"]);
//   });

//   test("handleChange should change the value of proper element from the refsObject", () => {
//     const { result } = renderHook(({ data }) => useObjectOfRefs(data), {
//       initialProps: { data: { email: "" } },
//     });
//     const [refsObject, handleChange] = result.current;
//     expect(refsObject["email"].current).toBe("");

//     const event = {
//       target: { name: "email", value: "dycjh@example.com" },
//     } as React.ChangeEvent<HTMLInputElement>;

//     handleChange(event);
//     expect(refsObject["email"].current).toBe("dycjh@example.com");
//   });
// });
