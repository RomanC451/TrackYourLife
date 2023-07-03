import React, { useRef } from "react";

/**
 * @description - Keeps track of the index of an array iteration.
 * @param arrayLength -The length of the array.
 * @param startingIndex - The starting index of the array iteration.
 * @param continuos=true - Whether or not the array iteration is continuos.
 * @returns the current index and the next and previous functions.
 */
const useIndexIterator = (
  arrayLength: number,
  startingIndex: number,
  continuos: boolean = true
): [React.MutableRefObject<number>, () => void, () => void] => {
  if (startingIndex >= arrayLength) {
    throw new Error("Starting index must be less than array length");
  }

  const index = useRef(startingIndex);

  /**
   * @function
   * @name next
   *
   * @description
   * This function updates the value of `index.current` based on the length of the array and the value of `continuous`.
   * If `index.current` is less than `arrayLength - 1`, it increments `index.current` by 1.
   * If `index.current` is equal to or greater than `arrayLength - 1` and `continuous` is `true`, it sets `index.current` to 0.
   *
   * @returns
   */
  const next = () => {
    if (index.current < arrayLength - 1) {
      index.current += 1;
    } else if (continuos) {
      index.current = 0;
    }
  };

  /**
   * @description
   * Decrements the current index value by 1, with the ability to loop around to the end if continuos is set to true.  *
   * @return
   */
  const previous = () => {
    if (index.current > 0) {
      index.current -= 1;
    } else if (continuos) {
      index.current = arrayLength - 1;
    }
  };

  return [index, next, previous];
};

export default useIndexIterator;
