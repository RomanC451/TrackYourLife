import React, { forwardRef, useImperativeHandle } from "react";
import { Card } from "~/chadcn/ui/card";

import { debounce, TextField } from "@mui/material";
import { useQueryClient } from "@tanstack/react-query";

import useFoodSearch from "../../../hooks/useFoodSearch";
import FoodList from "./FoodList";

type SearchWithModalProps = { date: Date };

export type SearchWithModalRef = {
  setResultsTableOpened: React.Dispatch<React.SetStateAction<boolean>>;
};

const FoodSearch = forwardRef<SearchWithModalRef, SearchWithModalProps>(
  ({ date }, ref) => {
    const {
      searchQuery,
      searchValue,
      setSearchValue,
      searchError,
      setSearchError,
      resultsTableOpened,
      setResultsTableOpened,
    } = useFoodSearch();

    const queryClient = useQueryClient();

    useImperativeHandle(ref, () => ({
      setResultsTableOpened,
    }));

    return (
      <div className="h-max-[calc(90%-195px)] relative mt-[20px] flex w-full flex-col items-center">
        <TextField
          className="text-black"
          autoComplete={"false"}
          fullWidth
          label="Search food..."
          error={!!searchError}
          helperText={searchError}
          onChange={debounce((e) => {
            setSearchError("");
            if (e.target.value === "") {
              setResultsTableOpened(false);
            } else {
              setResultsTableOpened(true);
            }

            if (e.target.value != searchValue) {
              queryClient.removeQueries({ queryKey: ["foodSearch"] });
            }
            setSearchValue(e.target.value);
          }, 500)}
          onEmptied={() => setResultsTableOpened(false)}
          onClick={(e) => {
            e.stopPropagation();
          }}
          onFocus={(e) => {
            if (e.target.value !== "") {
              setResultsTableOpened(true);
            }
          }}
          onBlur={() => {}}
        ></TextField>

        {resultsTableOpened && !searchQuery.isError ? (
          <Card
            className="absolute top-[60px] z-10 h-auto w-[90%] backdrop-blur-2xl lg:w-[80%] "
            onClick={(e) => {
              e.stopPropagation();
            }}
          >
            <FoodList
              foodList={searchQuery.data?.pages.flatMap(
                (page) => page?.items.items,
              )}
              fetchNextPage={searchQuery.fetchNextPage}
              isFetchingNextPage={searchQuery.isFetchingNextPage}
              searchValue={searchValue}
              date={date}
            />
          </Card>
        ) : null}
      </div>
    );
  },
);

export default FoodSearch;
