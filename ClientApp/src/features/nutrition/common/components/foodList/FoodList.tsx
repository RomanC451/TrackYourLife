import React, { memo, useCallback, useLayoutEffect, useRef } from "react";
import { CircularProgress } from "@mui/material";
import {
  FetchNextPageOptions,
  InfiniteData,
  InfiniteQueryObserverResult,
} from "@tanstack/react-query";
import { isEqual } from "lodash";

import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { colors } from "@/constants/tailwindColors";
import { LoadingState } from "@/hooks/useDelayedLoading";
import { FoodDto, PagedListOfFoodDto } from "@/services/openapi";

import FoodListElement, { LoadingFoodListElement } from "./FoodListElement";

type FoodListProps = {
  foodList: FoodDto[] | undefined;
  fetchNextPage: (
    options?: FetchNextPageOptions | undefined,
  ) => Promise<
    InfiniteQueryObserverResult<
      InfiniteData<PagedListOfFoodDto, unknown>,
      Error
    >
  >;
  isFetchingNextPage: boolean;
  searchValue: string;
  AddFoodButton: React.ComponentType<{
    food: FoodDto;
    className?: string;
  }>;
  AddFoodDialog: React.ComponentType<{ food: FoodDto }>;
  isPending: LoadingState;
};

function FoodList({
  foodList,
  fetchNextPage,
  isFetchingNextPage,
  searchValue,
  AddFoodButton,
  AddFoodDialog,
  isPending,
}: FoodListProps) {
  const listRef = useRef<HTMLDivElement>(null);

  useLayoutEffect(() => {
    if (listRef.current) {
      listRef.current.scrollTo({ top: 0 });
    }
  }, [searchValue]);

  const onScrollHandler = useCallback(
    function (e: React.UIEvent<HTMLDivElement, UIEvent>) {
      const target = e.target as HTMLDivElement;
      if (
        target.scrollHeight - target.scrollTop === target.clientHeight &&
        !isFetchingNextPage
      ) {
        fetchNextPage();
      }
    },
    [fetchNextPage, isFetchingNextPage],
  );

  const [foodListCopy, setFoodListCopy] = React.useState<FoodDto[]>([]);

  React.useEffect(() => {
    if (foodList && !isEqual(foodList, foodListCopy)) {
      setFoodListCopy([...foodList]);
    }
  }, [foodList]);

  if (isPending.isLoading) {
    return <FoodList.Loading />;
  }

  return (
    <FoodList.Wrapper listRef={listRef} onScroll={onScrollHandler}>
      <ListContent
        foods={foodListCopy}
        AddFoodButton={AddFoodButton}
        AddFoodDialog={AddFoodDialog}
      />
      {isFetchingNextPage ? (
        <div className="flex w-full items-center justify-center gap-2">
          <CircularProgress size={20} sx={{ color: colors.violet }} />
          <span style={{ color: colors.violet }}>Loading...</span>
        </div>
      ) : null}
    </FoodList.Wrapper>
  );
}

const ListContent = memo(function ListContent({
  foods,
  AddFoodButton,
  AddFoodDialog,
}: {
  foods: FoodDto[];
  AddFoodButton: React.ComponentType<{ food: FoodDto; className?: string }>;
  AddFoodDialog: React.ComponentType<{ food: FoodDto }>;
}) {
  return foods.map((food) => (
    <React.Fragment key={food.id}>
      <FoodListElement
        AddFoodButton={AddFoodButton}
        AddFoodDialog={AddFoodDialog}
        food={food}
      />
      <Separator className="my-2" />
    </React.Fragment>
  ));
});

FoodList.Wrapper = memo(function ({
  children,
  onScroll,
  listRef,
}: {
  children?: React.ReactNode;
  onScroll?: (event: React.UIEvent<HTMLDivElement>) => void;
  listRef?: React.RefObject<HTMLDivElement>;
}) {
  return (
    <>
      <div className="">
        <ScrollArea
          className="h-64 w-full rounded-md border"
          onScroll={onScroll}
          onTouchMove={onScroll}
          ref={listRef}
        >
          <div className="p-4">{children}</div>
        </ScrollArea>
      </div>
    </>
  );
});

FoodList.Loading = function () {
  return (
    <FoodList.Wrapper>
      {Array(4)
        .fill("")
        .map((_, index) => (
          <React.Fragment key={index}>
            <LoadingFoodListElement />
            <Separator className="my-2" />
          </React.Fragment>
        ))}
    </FoodList.Wrapper>
  );
};

export default FoodList;
