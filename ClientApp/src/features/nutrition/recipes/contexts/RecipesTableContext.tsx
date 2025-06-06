import { createContext, useCallback, useContext, useState } from "react";

import Assert from "@/lib/assert";

import { invalidateRecipesQuery } from "../../common/queries/useRecipesQuery";
import EditRecipeDialog from "../components/recipesDialogs/EditRecipeDialog";

interface ContextInterface {
  openEditRecipeModal: (recipeId: string) => void;
  closeEditRecipeModal: () => void;
}

const RecipesTableContext = createContext<ContextInterface>(
  {} as ContextInterface,
);

export function RecipesTableContextProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [editRecipeModalRecipeId, setEditRecipeModalRecipeId] = useState<
    string | undefined
  >();

  function openEditRecipeModal(recipeId: string) {
    setEditRecipeModalRecipeId(recipeId);
  }

  const closeEditRecipeModal = useCallback(() => {
    setEditRecipeModalRecipeId(undefined);
    invalidateRecipesQuery();
  }, []);

  return (
    <RecipesTableContext.Provider
      value={{
        openEditRecipeModal,
        closeEditRecipeModal,
      }}
    >
      {children}
      {editRecipeModalRecipeId ? (
        <EditRecipeDialog
          recipeId={editRecipeModalRecipeId}
          onClose={closeEditRecipeModal}
        />
      ) : null}
    </RecipesTableContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useRecipesTableContext() {
  const context = useContext(RecipesTableContext);
  Assert.isNotUndefined(
    context,
    "useRecipesTableContext must be used within a RecipesTableContextProvider.",
  );
  Assert.isNotEmptyObject(
    context,
    "useRecipesTableContext must be used within a RecipesTableContextProvider.",
  );
  return context;
}
