import React from "react";
import { useNavigate } from "react-router-dom";

const LandingPage: React.FC = (): JSX.Element => {
  const navigate = useNavigate();

  return (
    <div className="w-full h-full flex justify-center items-center">
      <div className="mt-[-20%]">
        <div>Landing Page</div>
        <button onClick={() => navigate("/authentication")}>Get started</button>
      </div>
    </div>
  );
};

export default LandingPage;
