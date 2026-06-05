import PageCard from "@/components/common/PageCard";
import PageTitle from "@/components/common/PageTitle";
import WatchHistoryList from "@/features/youtube/watchHistory/components/WatchHistoryList";

function WatchHistoryPage() {
  return (
    <PageCard>
      <PageTitle title="Watch history" />
      <WatchHistoryList />
    </PageCard>
  );
}

export default WatchHistoryPage;
