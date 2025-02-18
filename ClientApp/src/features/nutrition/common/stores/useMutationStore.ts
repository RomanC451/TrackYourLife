import { useRef } from "react";
import { v4 as uuidv4 } from "uuid";
import { create } from "zustand";

type State = {
  mutationStates: Record<string, Record<string, boolean>>;
  updateMutationState: (
    mutationType: string,
    id: string,
    state: boolean,
  ) => void;
  getMutationState: (mutationType: string) => boolean;
};

const useStore = create<State>((set, get) => ({
  mutationStates: {},
  updateMutationState: (mutationType, id, state) => {
    set((prevState) => ({
      mutationStates: {
        ...prevState.mutationStates,
        [mutationType]: {
          ...prevState.mutationStates[mutationType],
          [id]: state,
        },
      },
    }));
  },
  getMutationState: (mutationType) => {
    const states = get().mutationStates[mutationType];
    return states
      ? Object.values(states).filter((state) => state).length > 1
      : false;
  },
}));

const useMutationStore = (mutationType: string) => {
  const mutationIdRef = useRef(uuidv4());

  const updateMutationState = useStore((state) => state.updateMutationState);
  const getMutationState = useStore((state) => state.getMutationState);

  const updateState = (state: boolean) => {
    updateMutationState(mutationType, mutationIdRef.current, state);
  };

  const mutationState = getMutationState(mutationType);

  return {
    updateMutationState: updateState,
    mutationState,
  };
};

export default useMutationStore;
