import { useCallback, useEffect, useState } from "react";

import { useQuery } from "@tanstack/react-query";



import {

  Dialog,

  DialogContent,

  DialogDescription,

  DialogHeader,

  DialogTitle,

} from "@/components/ui/dialog";

import { Skeleton } from "@/components/ui/skeleton";

import {

  defaultExerciseStatsDateWindow,

  exerciseStatsQueryOptions,

  type ExerciseStatsChartMetric,

  type ExerciseStatsSearch,

} from "@/features/trainings/exercises/queries/exerciseStatsQuery";

import ExerciseStatsPage from "@/pages/trainings/ExerciseStatsPage";



interface ExerciseStatsDialogProps {

  exerciseId: string;

  open: boolean;

  onOpenChange: (open: boolean) => void;

}



function getDefaultSearch(): ExerciseStatsSearch {

  const w = defaultExerciseStatsDateWindow();

  return {

    range: "TwelveWeeks",

    chartMetric: "Volume",

    startDate: w.startDate,

    endDate: w.endDate,

  };

}



export default function ExerciseStatsDialog({

  exerciseId,

  open,

  onOpenChange,

}: ExerciseStatsDialogProps) {

  const [search, setSearch] = useState<ExerciseStatsSearch>(getDefaultSearch);

  const [popoverContainer, setPopoverContainer] = useState<HTMLElement | null>(

    null,

  );



  const handlePopoverContainerRef = useCallback((node: HTMLDivElement | null) => {

    setPopoverContainer(node);

  }, []);



  useEffect(() => {

    if (open) {

      setSearch(getDefaultSearch());

    }

  }, [open, exerciseId]);



  const statsQuery = useQuery({

    ...exerciseStatsQueryOptions.bySearch(exerciseId, search),

    enabled: open,

  });



  const goToSearch = (next: ExerciseStatsSearch) => {

    setSearch(next);

  };



  function renderDialogContent() {

    if (statsQuery.isPending && !statsQuery.data) {

      return (

        <div className="space-y-4">

          <Skeleton className="h-8 w-48" />

          <Skeleton className="h-32 w-full" />

          <Skeleton className="h-64 w-full" />

        </div>

      );

    }



    if (statsQuery.isError) {

      return (

        <p className="py-8 text-center text-sm text-muted-foreground">

          Could not load exercise stats.

        </p>

      );

    }



    if (!statsQuery.data) {

      return null;

    }



    return (

      <ExerciseStatsPage

        stats={statsQuery.data}

        search={search}

        className="m-0 sm:px-0"

        popoverContainer={popoverContainer}

        onChartMetricChange={(chartMetric: ExerciseStatsChartMetric) => {

          goToSearch({

            ...search,

            chartMetric,

          });

        }}

        onCustomDateRangeChange={(startDate, endDate) => {

          goToSearch({

            range: "TwelveWeeks",

            chartMetric: search.chartMetric,

            startDate,

            endDate,

          });

        }}

        onAllTimeRangeSelect={() => {

          goToSearch({

            range: "All",

            chartMetric: search.chartMetric,

          });

        }}

        onClearCustomDates={() => {

          const w = defaultExerciseStatsDateWindow();

          goToSearch({

            range: "TwelveWeeks",

            chartMetric: search.chartMetric,

            startDate: w.startDate,

            endDate: w.endDate,

          });

        }}

      />

    );

  }



  return (

    <Dialog open={open} onOpenChange={onOpenChange}>

      <DialogContent className="flex max-h-[90vh] flex-col overflow-visible sm:max-w-4xl">

        <div
          ref={handlePopoverContainerRef}
          className="pointer-events-none absolute inset-0"
        />

        <DialogHeader>

          <DialogTitle className="sr-only">Exercise stats</DialogTitle>

          <DialogDescription hidden>

            View exercise performance stats

          </DialogDescription>

        </DialogHeader>

        <div className="min-h-0 flex-1 overflow-y-auto pr-4">

          {renderDialogContent()}

        </div>

      </DialogContent>

    </Dialog>

  );

}


