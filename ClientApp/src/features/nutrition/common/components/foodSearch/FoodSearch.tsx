import React, { useEffect, useRef, useState } from "react";
import { ToOptions } from "@tanstack/react-router";
import { debounce } from "lodash";

import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { InputError } from "@/components/ui/input-error";
import { FoodDto } from "@/services/openapi";

import useFoodSearch, {
  removeFoodSearchQuery,
} from "../../queries/useFoodSearchQuery";
import FoodList from "./FoodList";
import { useFoodSearchContext } from "./useFoodSearchContext";

type SearchWithModalProps = {
  onSelectedFoodToOptions: ToOptions;
  addFoodButtonComponent: React.ComponentType<{
    food: FoodDto;
    className?: string;
  }>;
  placeHolder?: string;
  disabled?: boolean;
};

function FoodSearch({
  onSelectedFoodToOptions,
  addFoodButtonComponent,
  placeHolder,
  disabled,
}: SearchWithModalProps) {
  const [resultsTableOpened, setResultsTableOpened] = useState(false);

  const { setAddFoodButtonComponent, setOnSelectedFoodToOptions } =
    useFoodSearchContext();

  useEffect(() => {
    setOnSelectedFoodToOptions(onSelectedFoodToOptions);
    // Store a component type in state; wrap in a function so React doesn't treat it as an updater
    setAddFoodButtonComponent(() => addFoodButtonComponent);
  }, [
    onSelectedFoodToOptions,
    addFoodButtonComponent,
    setOnSelectedFoodToOptions,
    setAddFoodButtonComponent,
  ]);

  const {
    searchQuery,
    searchValue,
    setSearchValue,
    error,
    resetError,
    pendingState,
  } = useFoodSearch();

  const textFieldRef = useRef<HTMLInputElement>(null);
  const cardRef = useRef<HTMLDivElement>(null);

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
    <div className="h-max-[calc(90%-195px)] relative flex w-full flex-col items-center gap-2">
      <div className="w-full space-y-2">
        <Input
          ref={textFieldRef}
          placeholder={placeHolder ?? "Search food..."}
          autoComplete={"false"}
          disabled={disabled}
          className="h-12"
          onChange={debounce((e) => {
            resetError();
            setSearchValue(e.target.value);

            if (e.target.value) {
              removeFoodSearchQuery("");
            }
          }, 1000)}
          onFocus={() => {
            setResultsTableOpened(true);
            window.scrollTo({
              top: document.documentElement.scrollHeight,
              behavior: "smooth",
            });
          }}
        />
        <InputError message={error} isError={!!error} className="pl-3" />
      </div>
      {resultsTableOpened &&
      !searchQuery.isError &&
      searchQuery.data?.pages[0].items.length != 0 ? (
        <Card
          ref={cardRef}
          className="absolute top-[60px] z-10 h-auto w-[90%] backdrop-blur-2xl"
          onMouseDown={(e) => {
            e.stopPropagation();
          }}
        >
          <FoodList
            foodList={searchQuery.data?.pages.flatMap((page) => page?.items)}
            pendingState={pendingState}
            fetchNextPage={searchQuery.fetchNextPage}
            isFetchingNextPage={searchQuery.isFetchingNextPage}
            hasNextPage={searchQuery.hasNextPage}
            searchValue={searchValue}
          />
        </Card>
      ) : null}
    </div>
  );
}

FoodSearch.Loading = function () {
  return (
    <div className="h-max-[calc(90%-195px)] relative mt-[20px] flex w-full flex-col items-center">
      <Input
        className="w-full text-black"
        placeholder="Search food..."
        disabled
      ></Input>
    </div>
  );
};

export default FoodSearch;
