import { Alert, AlertDescription } from "@/components/ui/alert";

function SuggestFinishedBanner() {
  return (
    <Alert>
      <AlertDescription>
        You have reached the last page. Consider marking this book as Finished.
      </AlertDescription>
    </Alert>
  );
}

export default SuggestFinishedBanner;
