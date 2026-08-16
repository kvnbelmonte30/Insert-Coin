import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useAuthStore } from "../store/authStore";

export function ProtectedRoute({
  children,
  requireRole,
}: {
  children: ReactNode;
  requireRole?: "Administrador" | "Empleado";
}) {
  const token = useAuthStore((s) => s.token);
  const roles = useAuthStore((s) => s.roles);

  if (!token) return <Navigate to="/login" replace />;
  if (requireRole && !roles.includes(requireRole)) return <Navigate to="/" replace />;

  return <>{children}</>;
}
