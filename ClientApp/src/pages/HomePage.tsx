import { env } from "@/lib/env";

function HomePage() {
  return (
    <>
      {env.VITE_HIDE_TOOLS ? "true" : "false"}
      HomePage
    </>
  );
}
export default HomePage;
