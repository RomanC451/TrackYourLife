import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import HomeNutritionSection from "@/features/home/components/HomeNutritionSection";
import HomeTrainingsSection from "@/features/home/components/HomeTrainingsSection";
import HomeYoutubeSection from "@/features/home/components/HomeYoutubeSection";

function HomePage() {
  return (
    <PageCard>
      <PageTitle title="Home" />
      <div className="flex flex-col gap-8">
        <HomeTrainingsSection />
        <div className="grid grid-cols-1 gap-8 @5xl/page-card:grid-cols-2 @5xl/page-card:items-start">
          <div className="min-w-0">
            <HomeNutritionSection />
          </div>
          <div className="min-w-0">
            <HomeYoutubeSection />
          </div>
        </div>
      </div>
    </PageCard>
  );
}

export default HomePage;
