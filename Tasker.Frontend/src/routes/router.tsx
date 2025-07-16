import { createBrowserRouter } from "react-router";
import Home from "../Pages/Home";
import MainLayout from "../layouts/MainLayout";
import AuthLayout from "../layouts/AuthLayout";
import Login from "../Pages/auth/Login";
import ProtectedRoute from "./ProtectedRoute";
import AnonymousOnlyRoute from "./AnonymousOnlyRoute";
import NotFound from "../Pages/NotFound";
import ServerFail from "../Pages/ServerFail";

const router = createBrowserRouter([
  {
    path: "/",
    element: (
      <ProtectedRoute>
        <MainLayout />
      </ProtectedRoute>
    ),
    errorElement: <ServerFail />,
    children: [
      {
        index: true,
        element: <Home />,
      },
      {
        path: "*",
        element: <NotFound />
      }
    ],
  },
  {
    path: "/auth",
    element: (
      <AnonymousOnlyRoute>
        <AuthLayout />
      </AnonymousOnlyRoute>
    ),
    children: [
      {
        path: "login",
        element: <Login />,
        index: true,
      },
    ],
  },
]);

export default router;
