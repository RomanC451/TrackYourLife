import { AnimatePresence, motion } from "framer-motion";
import Lottie, { LottieRefCurrentProps } from "lottie-react";
import { AnimationSegment } from "lottie-web";
import React, { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import animationData from "~/assets/animations/LoadingAnimation.json";
import { useApiContext } from "~/contexts/ApiContextProvider";
import { userEndpoints } from "~/data/apiSettings";
import { postFetch } from "~/services/postFetch";

const animationSegments = {
  loading: [0, 119] as AnimationSegment,
  successful: [0, 390] as AnimationSegment,
  failed: [419, 820] as AnimationSegment,
  successFrame: [390, 390] as AnimationSegment,
  failedFrame: [820, 820] as AnimationSegment
};

const EmailVerificationPage: React.FC = (): JSX.Element => {
  const location = useLocation();
  const navigate = useNavigate();
  const { defaultApi, setJwtToken } = useApiContext();

  const [verificationState, setVerificationState] = useState<
    "Pending" | "Failed" | "Successful"
  >("Pending");

  const animationRef = useRef<LottieRefCurrentProps>(null);
  const [animationStoped, setAnimationStoped] = useState(false);
  const [animationFinished, setAnimationFinished] = useState(false);

  useEffect(() => {
    const token = new URLSearchParams(location.search).get("token");
    if (!token) return;

    const param = encodeURIComponent(token);

    const endpoint = `${userEndpoints.verifyEmail}?token=${param}`;

    postFetch(defaultApi, {}, endpoint, setJwtToken)
      .badRequest(() => {
        setVerificationState("Failed");
      })
      .json(() => {
        setVerificationState("Successful");
      })
      .catch(() => {
        setVerificationState("Failed");
      });
  }, []);

  useEffect(() => {
    if (animationRef === null) {
      return;
    }
    animationRef.current?.setSpeed(1.5);
  }, [animationRef]);

  const onComplete = () => {
    if (animationStoped) {
      return;
    }
    if (verificationState === "Successful") {
      animationRef.current?.playSegments(animationSegments.successful);
      setAnimationStoped(true);
    } else if (verificationState === "Failed") {
      animationRef.current?.playSegments(animationSegments.failed);
      setAnimationStoped(true);
    }
    setTimeout(() => {
      setAnimationFinished(true);
    }, 3500);
    setTimeout(() => {
      verificationState === "Successful"
        ? navigate("/home")
        : navigate("/auth");
    }, 6000);
  };

  return (
    <div className="min-h-[100vh] w-full grid place-content-center">
      <div className="flex items-center gap-4">
        <Lottie
          lottieRef={animationRef}
          animationData={animationData}
          className="w-[100px] h-[100px]"
          loop={false}
          autoPlay={false}
          initialSegment={animationSegments.loading}
          onComplete={onComplete}
        />
        <div className=" w-[230px] overflow-hidden h-[108px] flex items-center justify-center relative">
          <AnimatePresence initial={false}>
            <motion.div
              key={animationFinished ? "finished" : "loading"}
              initial={{ opacity: 0, y: -100 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: 100 }}
              transition={{
                y: { type: "spring", duration: 3 }
              }}
              className="w-full h-full absolute left-0 grid place-content-center text-center"
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
        </div>
      </div>
    </div>
  );
};

export default EmailVerificationPage;
