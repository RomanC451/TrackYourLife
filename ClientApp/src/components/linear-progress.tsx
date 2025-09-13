// !! TODO: Add animation
function LinearProgress({
  color = "bg-blue-600",
  value = 45,
}: {
  color?: string;
  value?: number;
}) {
  return (
    <div className="h-2.5 w-full rounded-full bg-gray-200 dark:bg-gray-700">
      <div
        className={`${color} h-2.5 rounded-full transition-all duration-300`}
        style={{ width: `${value}%` }}
      ></div>
    </div>
  );
}

export default LinearProgress;
