import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { LoginResponse } from "../types";

interface AuthState {
  token: string | null;
  usuarioId: string | null;
  nombre: string | null;
  roles: string[];
  locales: LoginResponse["locales"];
  setSession: (data: LoginResponse) => void;
  logout: () => void;
  isAdmin: () => boolean;
  isEmpleado: () => boolean;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      usuarioId: null,
      nombre: null,
      roles: [],
      locales: [],
      setSession: (data) =>
        set({
          token: data.token,
          usuarioId: data.usuarioId,
          nombre: data.nombre,
          roles: data.roles,
          locales: data.locales,
        }),
      logout: () =>
        set({ token: null, usuarioId: null, nombre: null, roles: [], locales: [] }),
      isAdmin: () => get().roles.includes("Administrador"),
      isEmpleado: () => get().roles.includes("Empleado"),
    }),
    { name: "maquinitas-auth" }
  )
);
