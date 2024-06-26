import React, { useLayoutEffect, useRef } from "react";
import { WretchResponse } from "wretch/types";
import { Separator } from "~/chadcn/ui/separator";
import {
  FoodElement,
  TFoodListResponse,
} from "~/features/health/requests/getFoodListRequest";
import useDelayedLoading from "~/hooks/useDelayedLoading";

import {
  FetchNextPageOptions,
  InfiniteData,
  InfiniteQueryObserverResult,
} from "@tanstack/react-query";

import { CircularProgress } from "@mui/material";
import { ScrollArea } from "~/chadcn/ui/scroll-area";
import { colors } from "~/constants/tailwindColors";
import FoodListElement from "./FoodListElement";

type FoodListProps = {
  foodList: FoodElement[] | undefined;
  fetchNextPage: (
    options?: FetchNextPageOptions | undefined,
  ) => Promise<
    InfiniteQueryObserverResult<
      InfiniteData<TFoodListResponse | WretchResponse, unknown>,
      Error
    >
  >;
  isFetchingNextPage: boolean;
  searchValue: string;
  date: Date;
};

const FoodList: React.FC<FoodListProps> = ({
  foodList,
  fetchNextPage,
  isFetchingNextPage,
  searchValue,
  date,
}) => {
  const listRef = useRef<HTMLDivElement>(null);

  const onScrollHandler = (e: React.UIEvent<HTMLDivElement, UIEvent>) => {
    const target = e.target as HTMLDivElement;
    console.log(target);
    if (
      target.scrollHeight - target.scrollTop === target.clientHeight &&
      !isFetchingNextPage
    ) {
      fetchNextPage();
    }
  };

  useLayoutEffect(() => {
    if (listRef.current) {
      listRef.current.scrollTo({ top: 0 });
    }
  }, [searchValue]);

  const loadingState = useDelayedLoading(100, foodList);

  const renderFoodList = () => {
    if (loadingState.isLoading) {
      return Array(4)
        .fill("")
        .map((_, index) => (
          <React.Fragment key={index}>
            <FoodListElement date={date} />
            <Separator className="my-2" />
          </React.Fragment>
        ));
    }

    return foodList?.map((food, index) => (
      <React.Fragment key={food.id}>
        <FoodListElement food={food} key={index} date={date} />
        <Separator className="my-2" />
      </React.Fragment>
    ));
  };

  if (loadingState.isStarting) return null;

  return (
    <div className="">
      {/* <TestPage /> */}
      <ScrollArea
        className="h-64 w-full rounded-md border"
        onScroll={onScrollHandler}
        onTouchMove={onScrollHandler}
        ref={listRef}
      >
        <div className="p-4">
          {renderFoodList()}
          {isFetchingNextPage ? (
            <div className="flex w-full items-center justify-center gap-2 ">
              <CircularProgress size={20} sx={{ color: colors.violet }} />
              <span style={{ color: colors.violet }}>Loading...</span>
            </div>
          ) : null}
        </div>
      </ScrollArea>
    </div>
  );
};

export default FoodList;
