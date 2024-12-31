import Lottie from "lottie-react";
import animationData from "~/assets/animations/BallAnimation.json";
import FullSizeCenteredLayout from "~/layouts/FullSizeCenteredLayout";
import RootLayout from "~/layouts/RootLayout";

const LoadingPage = () => {
  return (
    <RootLayout>
      <FullSizeCenteredLayout>
        <Lottie
          animationData={animationData}
          className="-mt-[20%] aspect-square w-[300px]"
          loop={true}
        />
      </FullSizeCenteredLayout>
    </RootLayout>
  );
};

export default LoadingPage;
