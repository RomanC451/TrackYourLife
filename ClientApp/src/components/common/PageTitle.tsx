import { CardTitle } from "../ui/card";

function PageTitle({
  title,
  children,
}: {
  title: string;
  children?: React.ReactNode;
}) {
  return (
    <div className="flex w-full flex-wrap items-center justify-between gap-4">
      <CardTitle className="text-3xl font-bold">{title}</CardTitle>
      <div className="ml-auto">{children}</div>
    </div>
  );
}

export default PageTitle;
