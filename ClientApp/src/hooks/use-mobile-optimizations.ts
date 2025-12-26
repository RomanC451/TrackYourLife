import { useEffect, useState } from "react";

export function useMobileOptimizations() {
  const [isMobile, setIsMobile] = useState(false);
  const [isLowEndDevice, setIsLowEndDevice] = useState(false);

  useEffect(() => {
    const checkDevice = () => {
      const userAgent = navigator.userAgent;
      const isMobileDevice =
        /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(
          userAgent,
        );

      // Check for low-end device indicators
      const isLowEnd =
        navigator.hardwareConcurrency <= 2 || // 2 or fewer CPU cores
        (navigator as any).deviceMemory <= 2 || // 2GB or less RAM
        /Android.*Chrome\/[0-5][0-9]/.test(userAgent); // Old Android Chrome versions

      setIsMobile(isMobileDevice);
      setIsLowEndDevice(isLowEnd);
    };

    checkDevice();
  }, []);

  return {
    isMobile,
    isLowEndDevice,
    shouldReduceAnimations: isLowEndDevice,
    shouldLazyLoadImages: isMobile || isLowEndDevice,
    shouldVirtualizeLists: isLowEndDevice,
  };
}
