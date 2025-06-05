import { useState } from "react";
import { ChevronDown, ChevronUp } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import { ExerciseDto } from "@/services/openapi";

function ExerciseDescriptionCollapsible({
  exercise,
}: {
  exercise: ExerciseDto;
}) {
  const [isDescriptionOpen, setIsDescriptionOpen] = useState(false);

  return (
    <Collapsible open={isDescriptionOpen} onOpenChange={setIsDescriptionOpen}>
      <div className="flex w-full">
        <div className="flex-1" />
        <CollapsibleTrigger
          asChild
          className="flex items-center gap-2 text-right hover:underline"
        >
          <Button variant="link" className="p-0">
            Show description
            {isDescriptionOpen ? <ChevronUp /> : <ChevronDown />}
          </Button>
        </CollapsibleTrigger>
      </div>
      <CollapsibleContent className="mt-1 w-full">
        <p className="whitespace-pre-wrap text-gray-300">
          {exercise.description}
        </p>
      </CollapsibleContent>
    </Collapsible>
  );
}

export default ExerciseDescriptionCollapsible;
