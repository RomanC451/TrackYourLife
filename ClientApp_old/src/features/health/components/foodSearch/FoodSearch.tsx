import React, { useEffect, useRef, useState } from "react";
import { Card } from "~/chadcn/ui/card";

import { debounce, TextField } from "@mui/material";

import { FoodDto } from "~/services/openapi";
import { withOnSuccess } from "~/utils/with";
import useFoodSearchQuery, {
  removeFoodSearchQuery,
} from "../../queries/useFoodSearchQuery";
import FoodList from "./FoodList";

type SearchWithModalProps = {
  AddFoodButton: React.ComponentType<{ food: FoodDto; className?: string }>;
  AddFoodDialog: React.ComponentType<{ food: FoodDto; onSuccess: () => void }>;
  placeHolder?: string;
};

function FoodSearch({
  AddFoodButton,
  AddFoodDialog,
  placeHolder,
}: SearchWithModalProps) {
  const [resultsTableOpened, setResultsTableOpened] = useState(false);

  const {
    searchQuery,
    searchValue,
    setSearchValue,
    error,
    resetError,

    isPending,
  } = useFoodSearchQuery();

  const textFieldRef = useRef<HTMLDivElement>(null);
  const cardRef = useRef<HTMLDivElement>(null);

  const LocalAddFoodDialog = withOnSuccess(AddFoodDialog, () => {
    setResultsTableOpened(false);
  });

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      const targetNode = event.target as Node;
      if (
        targetNode.nodeName !== "HTML" &&
        textFieldRef.current &&
        !textFieldRef.current.contains(targetNode) &&
        cardRef.current &&
        !cardRef.current.contains(targetNode)
      ) {
        setResultsTableOpened(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  return (
    <div className="h-max-[calc(90%-195px)] relative  flex w-full flex-col items-center gap-2">
      <TextField
        ref={textFieldRef}
        className="text-black"
        autoComplete={"false"}
        fullWidth
        label={placeHolder ?? "Search food..."}
        error={!!error}
        helperText={error}
        onChange={debounce((e) => {
          resetError();
          setSearchValue(e.target.value);

          if (e.target.value) {
            removeFoodSearchQuery("");
          }
        }, 1000)}
        onFocus={() => {
          setResultsTableOpened(true);
        }}
      />

      {resultsTableOpened && !searchQuery.isError ? (
        <Card
          ref={cardRef}
          className="absolute top-[60px] z-10 h-auto w-[90%]  backdrop-blur-2xl "
          onMouseDown={(e) => {
            e.stopPropagation();
          }}
        >
          <FoodList
            foodList={searchQuery.data?.pages.flatMap((page) => page?.items)}
            isPending={isPending}
            fetchNextPage={searchQuery.fetchNextPage}
            isFetchingNextPage={searchQuery.isFetchingNextPage}
            searchValue={searchValue}
            AddFoodButton={AddFoodButton}
            AddFoodDialog={LocalAddFoodDialog}
          />
        </Card>
      ) : null}
    </div>
  );
}

FoodSearch.Loading = function () {
  return (
    <div className="h-max-[calc(90%-195px)] relative mt-[20px] flex w-full flex-col items-center">
      <TextField
        className="text-black"
        fullWidth
        label="Search food..."
        disabled
      ></TextField>
    </div>
  );
};

export default FoodSearch;
