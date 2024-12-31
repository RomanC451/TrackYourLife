import React from "react";

export default class Assert {
  static reactChildreanLengthLowerThan(
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

  static isNotUndefined(variable?: unknown, message?: string) {
    if (variable === undefined) {
      throw new Error(message ?? "The variable must not be undefined.");
    }
  }

  static isNotEmptyObject(object: object, message?: string) {
    if (Object.keys(object).length === 0) {
      throw new Error(message ?? "The object must not be empty.");
    }
  }
}
