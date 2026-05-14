import * as React from "react";
import { Link } from "@tanstack/react-router";

import { Button, type ButtonProps } from "./button";

type LinkProps = React.ComponentPropsWithoutRef<typeof Link>;

export type LinkButtonProps = Omit<ButtonProps, "asChild" | "type"> &
  Pick<
    LinkProps,
    | "to"
    | "params"
    | "search"
    | "hash"
    | "state"
    | "replace"
    | "preload"
    | "activeProps"
    | "inactiveProps"
    | "mask"
    | "from"
    | "target"
    | "rel"
    | "download"
    | "reloadDocument"
  >;

const LinkButton = React.forwardRef<HTMLAnchorElement, LinkButtonProps>(
  (
    {
      to,
      params,
      search,
      hash,
      state,
      replace,
      preload = "intent",
      activeProps,
      inactiveProps,
      mask,
      from,
      target,
      rel,
      download,
      reloadDocument,
      children,
      ...buttonProps
    },
    ref,
  ) => {
    return (
      <Button asChild {...buttonProps}>
        <Link
          ref={ref}
          to={to}
          params={params}
          search={search}
          hash={hash}
          state={state}
          replace={replace}
          preload={preload}
          activeProps={activeProps}
          inactiveProps={inactiveProps}
          mask={mask}
          from={from}
          target={target}
          rel={rel}
          download={download}
          reloadDocument={reloadDocument}
        >
          {children}
        </Link>
      </Button>
    );
  },
);
LinkButton.displayName = "LinkButton";

export { LinkButton };
