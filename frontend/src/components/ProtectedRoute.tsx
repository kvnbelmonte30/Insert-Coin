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
  const debeCambiarContrasena = useAuthStore((s) => s.debeCambiarContrasena);

  if (!token) return <Navigate to="/login" replace />;
  if (debeCambiarContrasena) return <Navigate to="/cambiar-contrasena" replace />;
  if (requireRole && !roles.includes(requireRole)) return <Navigate to="/" replace />;

  return <>{children}</>;
}
