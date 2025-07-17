import ReactDOM from "react-dom/client";
import router from "./routes/router";
import { RouterProvider } from "react-router/dom";
import "./index.css";
import { StrictMode } from "react";
import { AuthProvider } from "./contexts/AuthContext";
import { TeamProvider } from "./contexts/TeamContext";

const root = document.getElementById("root");

ReactDOM.createRoot(root!).render(
  <StrictMode>
    <AuthProvider>
      <TeamProvider>
        <RouterProvider router={router} />
      </TeamProvider>
    </AuthProvider>
  </StrictMode>
);
