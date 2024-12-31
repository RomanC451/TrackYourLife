// import { memo, useMemo } from "react";
// import { useToggle } from "usehooks-ts";
// import useStoreLoadingStateToContext, {
//   LoadingContextProvider,
//   useLoadingState,
// } from "./contexts/LoadingContext";
// import FoodSearch from "./features/health/components/foodSearch/FoodSearch";
// import useAddIngredientMutation from "./features/health/mutations/recipes/useAddIngredientMutation";
// import { FoodDto } from "./services/openapi";

// type testProps = {};

// function Test({}: testProps): JSX.Element {
//   return (
//     <LoadingContextProvider>
//       <MemoisedTestComponent1 />
//     </LoadingContextProvider>
//   );
// }

// function AddFoodDialog({ food }: { food: FoodDto }): JSX.Element {
//   return <div>{food.name}</div>;
// }

// function AddFoodButton({
//   food,
//   onSuccess,
// }: {
//   food: FoodDto;
//   onSuccess: () => void;
// }): JSX.Element {
//   const muttation = useAddIngredientMutation();
//   return <button>Add Food</button>;
// }

// // type SearchWithModalProps = {
// //   AddFoodButton: React.ComponentType<{ food: FoodDto; className?: string }>;
// //   AddFoodDialog: React.ComponentType<{ food: FoodDto; onSuccess: () => void }>;
// //   placeHolder?: string;
// // };

// // function FoodSearch({
// //   AddFoodButton,
// //   AddFoodDialog,
// //   placeHolder,
// // }: SearchWithModalProps) {
// //   const [resultsTableOpened, setResultsTableOpened] = useState(false);

// //   return <Input></Input>;
// // }

// function TestComponent1(): JSX.Element {
//   const isLoading = useLoadingState();

//   console.log("isLoadingContext", isLoading);

//   const items = useMemo(() => Array.from({ length: 5 }), []);

//   return (
//     <div className="flex-col">
//       <p>{isLoading ? "loading" : "not loading"}</p>
//       <FoodSearch
//         AddFoodButton={AddFoodDialog}
//         AddFoodDialog={AddFoodButton}
//         placeHolder="Search for ingredients..."
//       />

//       <MemoisedTestComponent1o2 />
//       {items.map((_, i) => (
//         <MemoisedTestComponent1o3 key={i} index={i} />
//       ))}
//     </div>
//   );
// }

// const MemoisedTestComponent1 = memo(TestComponent1);

// function TestComponent1o2(): JSX.Element {
//   const [state, setState] = useToggle(true);
//   useStoreLoadingStateToContext("test1", state);

//   return (
//     <div>
//       <button
//         className={!state ? "text-red-400" : "text-green-400"}
//         onClick={setState}
//       >
//         setState1
//       </button>
//     </div>
//   );
// }

// const MemoisedTestComponent1o2 = memo(TestComponent1o2);

// function TestComponent1o3({ index }: { index: number }): JSX.Element {
//   const [state, setState] = useToggle(true);
//   useStoreLoadingStateToContext(`test2-${index}`, state);
//   console.log(`test2-${index}`);
//   return (
//     <div>
//       <button
//         className={!state ? "text-red-400" : "text-green-400"}
//         onClick={setState}
//       >
//         setState2
//       </button>
//     </div>
//   );
// }

// const MemoisedTestComponent1o3 = memo(TestComponent1o3);

// export default Test;
