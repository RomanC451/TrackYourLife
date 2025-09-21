import React, { useLayoutEffect, useRef } from "react";
import {
  FetchNextPageOptions,
  InfiniteData,
  InfiniteQueryObserverResult,
} from "@tanstack/react-query";
import { v4 as uuidv4 } from "uuid";

import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import Spinner from "@/components/ui/spinner";
import { PendingState } from "@/hooks/useCustomQuery";
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

  pendingState: PendingState;
  hasNextPage: boolean;
};

function FoodList({
  foodList,
  fetchNextPage,
  isFetchingNextPage,
  hasNextPage,
  searchValue,
  pendingState,
}: FoodListProps) {
  const listRef = useRef<HTMLDivElement>(null);

  useLayoutEffect(() => {
    if (listRef.current) {
      listRef.current.scrollTo({ top: 0 });
    }
  }, [searchValue]);

  const onScrollHandler = (e: React.UIEvent<HTMLDivElement, UIEvent>) => {
    const target = e.target as HTMLDivElement;
    if (
      target.scrollHeight - target.scrollTop === target.clientHeight &&
      !isFetchingNextPage &&
      hasNextPage
    ) {
      fetchNextPage();
    }
  };

  if (pendingState.isDelayedPending) {
    return <FoodList.Loading />;
  }

  return (
    <FoodList.Wrapper listRef={listRef} onScroll={onScrollHandler}>
      <ListContent foods={foodList || []} />

      {isFetchingNextPage ? (
        <Skeleton className="flex h-16 w-full animate-pulse items-center justify-center gap-2 border">
          <Spinner className="size-5" />
        </Skeleton>
      ) : null}
    </FoodList.Wrapper>
  );
}

const ListContent = function ListContent({ foods }: { foods: FoodDto[] }) {
  return foods.map((food, index) => (
    <React.Fragment key={food.id}>
      <FoodListElement food={food} />
      {index !== foods.length - 1 ? (
        <Separator className="mx-auto my-2 w-[95%]" />
      ) : null}
    </React.Fragment>
  ));
};

FoodList.Wrapper = function ({
  children,
  onScroll,
  listRef,
}: {
  children?: React.ReactNode;
  onScroll?: (event: React.UIEvent<HTMLDivElement>) => void;
  listRef?: React.RefObject<HTMLDivElement | null>;
}) {
  return (
    <ScrollArea
      className="h-64 w-full rounded-md border"
      onScroll={onScroll}
      onTouchMove={onScroll}
      ref={listRef}
    >
      <div className="space-y-2 px-3 py-3">{children}</div>
    </ScrollArea>
  );
};

FoodList.Loading = function () {
  return (
    <FoodList.Wrapper>
      {Array(4)
        .fill("")
        .map((_, index) => (
          <React.Fragment key={uuidv4()}>
            <LoadingFoodListElement />
            {index !== 3 ? (
              <Separator className="mx-auto my-2 w-[95%]" />
            ) : null}
          </React.Fragment>
        ))}
    </FoodList.Wrapper>
  );
};

export default FoodList;
