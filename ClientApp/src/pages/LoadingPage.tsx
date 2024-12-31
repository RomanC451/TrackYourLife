import Lottie from "lottie-react";

import animationData from "@/assets/lottie/BallAnimation.json";
import FullSizeCenteredLayout from "@/layouts/FullSizeCenteredLayout";

const LoadingPage = () => {
  return (
    <FullSizeCenteredLayout className="h-screen w-full">
      <Lottie
        animationData={animationData}
        className="-mt-[20%] aspect-square w-[300px]"
        loop={true}
      />
    </FullSizeCenteredLayout>
  );
};

export default LoadingPage;
