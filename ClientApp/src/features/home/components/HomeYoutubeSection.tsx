import { Link } from "@tanstack/react-router";

import { Button } from "@/components/ui/button";
import HomeYoutubeRecommendation from "@/features/youtube/components/home/HomeYoutubeRecommendation";

import HomeSection from "./HomeSection";

function HomeYoutubeSection() {
  return (
    <HomeSection
      title="From your favorites"
      description="A random pick from your favorite channels, refreshed each visit."
      action={
        <Button variant="outline" size="sm" asChild>
          <Link to="/youtube/videos">All videos</Link>
        </Button>
      }
    >
      <HomeYoutubeRecommendation />
    </HomeSection>
  );
}

export default HomeYoutubeSection;
