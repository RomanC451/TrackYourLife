import { useCallback, useMemo, useState } from "react";

import { IngredientDto } from "@/services/openapi";

export function useIngredientsSelection(ingredients: IngredientDto[] = []) {
  const [selectedIds, setSelectedIds] = useState<string[]>([]);

  const toggle = useCallback((id: string) => {
    setSelectedIds((prev) =>
      prev.includes(id) ? prev.filter((i) => i !== id) : [...prev, id],
    );
  }, []);

  const selectedIngredients = useMemo(
    () => ingredients.filter((i) => selectedIds.includes(i.id)),
    [ingredients, selectedIds],
  );

  const isAllSelected = useMemo(() => {
    if (ingredients.length === 0) return false;
    return selectedIds.length === ingredients.length;
  }, [ingredients.length, selectedIds.length]);

  const handleSelectAll = useCallback(
    (select: boolean) => {
      if (select) {
        setSelectedIds(ingredients.map((i) => i.id));
      } else {
        setSelectedIds([]);
      }
    },
    [ingredients],
  );

  const clearSelection = useCallback(() => {
    setSelectedIds([]);
  }, []);

  return {
    selectedIds,
    toggle,
    selectedIngredients,
    isAllSelected,
    handleSelectAll,
    clearSelection,
  };
}
