import { Link } from "@tanstack/react-router";
import React from "react";
import { ModeToggle } from "~/chadcn/mode-toggle";
import { Button } from "~/chadcn/ui/button";

const AuthNavBar: React.FC = (): React.JSX.Element => {
  return (
    <section className="flex w-full justify-center">
      <nav className="w-[80%] py-3">
        <ul className="flex justify-end gap-10">
          <li>
            <Link to="/about" preload={false}>
              <Button type="button" variant="outline">
                About page
              </Button>
            </Link>
          </li>
          <li>
            <Link to="/emailVerification" search={{ token: "asdas" }}>
              <Button type="button" variant="outline">
                Email verification
              </Button>
            </Link>
          </li>
          <li>
            <ModeToggle />
          </li>
        </ul>
      </nav>
    </section>
  );
};

export default AuthNavBar;
