import { ToOptions, useNavigate, useRouter } from "@tanstack/react-router";

function useNavigateBackOrDefault(toOptions: ToOptions) {
  const router = useRouter();
  const navigate = useNavigate();

  const navigateBackOrDefault = () => {
    if (router.history.canGoBack()) {
      router.history.back();
    } else {
      navigate(toOptions);
    }
  };

  return navigateBackOrDefault;
}

export default useNavigateBackOrDefault;
