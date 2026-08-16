import { BrowserRouter, Navigate, Outlet, Route, Routes } from "react-router-dom";
import { ThemeProvider, createTheme, CssBaseline } from "@mui/material";
import { LoginPage } from "./pages/LoginPage";
import { DashboardPage } from "./pages/admin/DashboardPage";
import { LocalesPage } from "./pages/admin/LocalesPage";
import { LocalDetailPage } from "./pages/admin/LocalDetailPage";
import { MaquinasPage } from "./pages/admin/MaquinasPage";
import { CascadaConfigPage } from "./pages/admin/CascadaConfigPage";
import { AveriasAdminPage } from "./pages/admin/AveriasAdminPage";
import { GastosAdminPage } from "./pages/admin/GastosAdminPage";
import { CierresSemanalesPage } from "./pages/admin/CierresSemanalesPage";
import { UsuariosPage } from "./pages/admin/UsuariosPage";
import { EmpleadoPage } from "./pages/empleado/EmpleadoPage";
import { AveriasEmpleadoPage } from "./pages/empleado/AveriasEmpleadoPage";
import { CascadasEmpleadoPage } from "./pages/empleado/CascadasEmpleadoPage";
import { CierreSemanalEmpleadoPage } from "./pages/empleado/CierreSemanalEmpleadoPage";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { Layout } from "./components/Layout";
import { useAuthStore } from "./store/authStore";

const theme = createTheme({
  palette: {
    primary: { main: "#1e3a5f" },
    secondary: { main: "#c9a227" },
  },
});

function HomeRedirect() {
  const isAdmin = useAuthStore((s) => s.isAdmin());
  return <Navigate to={isAdmin ? "/admin/dashboard" : "/empleado/cuenta"} replace />;
}

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />

          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Layout>
                  <HomeRedirect />
                </Layout>
              </ProtectedRoute>
            }
          />

          <Route
            path="/admin"
            element={
              <ProtectedRoute requireRole="Administrador">
                <Layout>
                  <Outlet />
                </Layout>
              </ProtectedRoute>
            }
          >
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="locales" element={<LocalesPage />} />
            <Route path="locales/:id" element={<LocalDetailPage />} />
            <Route path="maquinas" element={<MaquinasPage />} />
            <Route path="maquinas/:id/cascada" element={<CascadaConfigPage />} />
            <Route path="averias" element={<AveriasAdminPage />} />
            <Route path="gastos" element={<GastosAdminPage />} />
            <Route path="cierres-semanales" element={<CierresSemanalesPage />} />
            <Route path="usuarios" element={<UsuariosPage />} />
          </Route>

          <Route
            path="/empleado"
            element={
              <ProtectedRoute requireRole="Empleado">
                <Layout>
                  <Outlet />
                </Layout>
              </ProtectedRoute>
            }
          >
            <Route path="cuenta" element={<EmpleadoPage />} />
            <Route path="averias" element={<AveriasEmpleadoPage />} />
            <Route path="cascadas" element={<CascadasEmpleadoPage />} />
            <Route path="cierre-semanal" element={<CierreSemanalEmpleadoPage />} />
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}
