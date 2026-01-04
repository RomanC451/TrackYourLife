import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { VideoCategory } from "@/services/openapi";

export type CategoryTabValue = VideoCategory;

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
        <TabsTrigger value="Educational">Educational</TabsTrigger>
        <TabsTrigger value="Entertainment">Fun</TabsTrigger>
      </TabsList>
    </Tabs>
  );
}

export default CategoryTabs;
