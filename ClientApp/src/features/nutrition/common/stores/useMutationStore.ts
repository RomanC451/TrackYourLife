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
  const mutationState = useStore((state) => {
    const states = state.mutationStates[mutationType];
    return states
      ? Object.values(states).filter((active) => active).length > 1
      : false;
  });

  const updateState = (state: boolean) => {
    updateMutationState(mutationType, mutationIdRef.current, state);
  };

  return {
    updateMutationState: updateState,
    mutationState,
  };
};

export default useMutationStore;
