import { AnimationSegment } from "lottie-web";
import React from "react";

import { getRouteApi, useNavigate } from "@tanstack/react-router";

console.log("loaded");
const animationSegments = {
  loading: [0, 119] as AnimationSegment,
  successful: [0, 390] as AnimationSegment,
  failed: [419, 820] as AnimationSegment,
  successFrame: [390, 390] as AnimationSegment,
  failedFrame: [820, 820] as AnimationSegment,
};

const routeApi = getRouteApi("/emailVerification");

const EmailVerificationPage: React.FC = (): JSX.Element => {
  const navigate = useNavigate();
  // const { fetchRequest } = useApiRequests();

  // const search = routeApi.useSearch();

  // const [verificationState, setVerificationState] = useState<
  //   "Pending" | "Failed" | "Successful"
  // >("Pending");

  // const animationRef = useRef<LottieRefCurrentProps>(null);
  // const [animationStoped, setAnimationStoped] = useState(false);
  // const [animationFinished, setAnimationFinished] = useState(false);

  // useEffect(() => {
  //   if (!search.token) return;

  //   //!! convert to mutation
  //   fetchRequest({
  //     endpoint: endpoint,
  //     requestType: requestTypes.POST,
  //     // catchers: {
  //     //   notFound: () => {
  //     //     setVerificationState("Failed");
  //     //   },
  //     // },
  //   })
  //     .then(() => {
  //       setVerificationState("Successful");
  //     })
  //     .catch(() => {
  //       setVerificationState("Failed");
  //     });
  // }, []);

  // useEffect(() => {
  //   if (animationRef === null) {
  //     return;
  //   }
  //   animationRef.current?.setSpeed(1.5);
  // }, [animationRef]);

  // const onComplete = () => {
  //   if (animationStoped) {
  //     return;
  //   }
  //   if (verificationState === "Successful") {
  //     animationRef.current?.playSegments(animationSegments.successful);
  //     setAnimationStoped(true);
  //   } else if (verificationState === "Failed") {
  //     animationRef.current?.playSegments(animationSegments.failed);
  //     setAnimationStoped(true);
  //   }
  //   setTimeout(() => {
  //     setAnimationFinished(true);
  //   }, 3500);
  //   setTimeout(() => {
  //     // verificationState === "Successful"
  //     // ? navigate({ to: "/home" })
  //     navigate({ to: "/auth" });
  //   }, 6000);
  // };

  return (
    <div className="grid min-h-[100vh] w-full place-content-center">
      <div className="flex items-center gap-4">
        Success
        {/* <Lottie
          lottieRef={animationRef}
          animationData={animationData}
          className="h-[100px] w-[100px]"
          loop={false}
          autoPlay={false}
          initialSegment={animationSegments.loading}
          onComplete={onComplete}
        />
        <div className=" relative flex h-[108px] w-[230px] items-center justify-center overflow-hidden">
          <AnimatePresence initial={false}>
            <motion.div
              key={animationFinished ? "finished" : "loading"}
              initial={{ opacity: 0, y: -100 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: 100 }}
              transition={{
                y: { type: "spring", duration: 3 },
              }}
              className="absolute left-0 grid h-full w-full place-content-center text-center"
            >
              <span className="text-xl font-semibold text-green-500  ">
                {!animationFinished
                  ? "Verifying email"
                  : animationFinished && verificationState === "Successful"
                    ? "Email verified"
                    : animationFinished && verificationState === "Failed"
                      ? "Email verification failed. Try again."
                      : null}
              </span>
            </motion.div>
          </AnimatePresence>
        </div> */}
      </div>
    </div>
  );
};

export default EmailVerificationPage;
