import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { VideoCategory } from "@/services/openapi";

export type CategoryTabValue = "all" | VideoCategory;

interface CategoryTabsProps {
  value: CategoryTabValue;
  onValueChange: (value: CategoryTabValue) => void;
}

function CategoryTabs({ value, onValueChange }: CategoryTabsProps) {
  return (
    <Tabs
      value={value}
      onValueChange={(v) => onValueChange(v as CategoryTabValue)}
    >
      <TabsList>
        <TabsTrigger value="all">All</TabsTrigger>
        <TabsTrigger value="Divertissement">Fun</TabsTrigger>
        <TabsTrigger value="Educational">Educational</TabsTrigger>
      </TabsList>
    </Tabs>
  );
}

export default CategoryTabs;

