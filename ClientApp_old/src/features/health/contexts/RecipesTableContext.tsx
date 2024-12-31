import { createContext, useContext, useState } from "react";
import { Assert } from "~/utils";
import EditRecipeDialog from "../components/recipes/recipeDialogs/EditRecipeDialog";

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
}): JSX.Element {
  const [editRecipeModalRecipeId, setEditRecipeModalRecipeId] = useState<
    string | undefined
  >();

  function openEditRecipeModal(recipeId: string) {
    setEditRecipeModalRecipeId(recipeId);
  }

  function closeEditRecipeModal() {
    setEditRecipeModalRecipeId(undefined);
  }

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
