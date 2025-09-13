import { useEffect } from "react";
import Lottie from "lottie-react";
import { XIcon } from "lucide-react";

import animationData from "@/assets/lottie/BallAnimation.json";
import { Button } from "@/components/ui/button";
import useNavigateBackOrDefault from "@/hooks/useNavigateBackOrDefault";
import FullSizeCenteredLayout from "@/layouts/FullSizeCenteredLayout";
import { FileRoutesByTo } from "@/routeTree.gen";

const LoadingPage = ({
  defaultRouteOnCancel,
}: {
  defaultRouteOnCancel: keyof FileRoutesByTo;
}) => {
  const navigateBackOrDefault = useNavigateBackOrDefault({
    to: defaultRouteOnCancel,
  });

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        navigateBackOrDefault();
      }
    };

    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [navigateBackOrDefault]);

  return (
    <FullSizeCenteredLayout className="relative h-screen w-full">
      <Button
        onClick={() => {
          navigateBackOrDefault();
        }}
        variant="outline"
        className="absolute right-4 top-4 z-10 size-8 rounded-full"
      >
        <XIcon className="size-4" />
      </Button>
      <Lottie
        animationData={animationData}
        className="-mt-[20%] aspect-square w-[300px]"
        loop={true}
      />
    </FullSizeCenteredLayout>
  );
};

export default LoadingPage;
