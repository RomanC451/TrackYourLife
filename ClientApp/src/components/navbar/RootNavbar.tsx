import { PropsWithChildren } from "react";

import { ModeToggle } from "../mode-toggle";

function RootNavbar({
  children,
  themeToggle,
}: PropsWithChildren<{ themeToggle: boolean }>) {
  return (
    <nav className="w-full p-3">
      <ul className="flex justify-between gap-10">
        <div className="flex gap-10">{children}</div>
        {themeToggle ? (
          <li>
            <ModeToggle />
          </li>
        ) : null}
      </ul>
    </nav>
  );
}

export default RootNavbar;
