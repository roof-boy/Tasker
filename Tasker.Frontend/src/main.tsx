import ReactDOM from "react-dom/client";
import router from "./routes/router";
import { RouterProvider } from "react-router/dom";
import "./index.css";
import { StrictMode } from "react";
import { AuthProvider } from "./contexts/AuthContext";

const root = document.getElementById("root");

ReactDOM.createRoot(root!).render(
  <StrictMode>
    <AuthProvider>
      <RouterProvider router={router} />
    </AuthProvider>
  </StrictMode>
);
