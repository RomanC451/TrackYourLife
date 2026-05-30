import { Card } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

function HomeTrainingsSectionSkeleton() {
  return (
    <Card className="mb-0 overflow-hidden p-0">
      <div className="space-y-4 p-5">
        <Skeleton className="h-6 w-48" />
        <Skeleton className="h-20 w-full rounded-xl" />
      </div>
      <div className="border-t border-border px-5 py-4">
        <Skeleton className="mb-3 h-4 w-36" />
        <div className="grid grid-cols-3 gap-3">
          <Skeleton className="h-24 rounded-xl" />
          <Skeleton className="h-24 rounded-xl" />
          <Skeleton className="h-24 rounded-xl" />
        </div>
      </div>
    </Card>
  );
}

export default HomeTrainingsSectionSkeleton;
