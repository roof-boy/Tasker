import React from "react";
import { useAuth } from "../contexts/AuthContext";
import { useNavigate } from "react-router";

export default function ProtectedRoute({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user, isLoading } = useAuth();

  const navigate = useNavigate();

  if (!user && !isLoading) {
    navigate("/auth/login");
  } else {
    return children;
  }
}
