import React from "react";

export default class Assert {
  static reactChildrenLengthLowerThan(
    children: React.ReactNode,
    length: number,
    message?: string,
  ) {
    if (React.Children.count(children) >= length) {
      throw new Error(
        message ?? "This layout component can only have one child.",
      );
    }
  }

  /**
   * Ensures that the provided variable is not undefined.
   * After this function returns successfully, TypeScript will narrow the type to exclude undefined.
   * @throws {Error} If the variable is undefined
   */
  static isNotUndefined<T>(
    variable: T | undefined,
    message?: string,
  ): asserts variable is NonNullable<T> {
    if (variable === undefined) {
      throw new Error(
        message ?? `Expected variable to be defined but got undefined`,
      );
    }
  }

  static isNotEmptyObject(object: object, message?: string) {
    if (Object.keys(object).length === 0) {
      throw new Error(message ?? "The object must not be empty.");
    }
  }

  static contextIsDefined(context: object, contextName?: string) {
    if (
      context === undefined ||
      context === null ||
      Object.keys(context).length === 0
    ) {
      throw new Error(
        `${contextName} must be used within a ${contextName}Provider`,
      );
    }
  }
}
