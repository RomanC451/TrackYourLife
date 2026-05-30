import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import HomeYoutubeRecommendation from "@/features/youtube/components/home/HomeYoutubeRecommendation";

function HomePage() {
  return (
    <PageCard>
      <PageTitle title="Home" />
      <section className="space-y-4">
        <div>
          <h2 className="text-lg font-semibold">Recommended for you</h2>
          <p className="text-sm text-muted-foreground">
            A random pick from your favorite channels, refreshed each visit.
          </p>
        </div>
        <HomeYoutubeRecommendation />
      </section>
    </PageCard>
  );
}

export default HomePage;
