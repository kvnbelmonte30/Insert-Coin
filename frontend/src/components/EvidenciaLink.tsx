import { Link } from "@mui/material";
import { api } from "../api/client";

export function EvidenciaLink({ url, label }: { url: string; label: string }) {
  const abrir = async () => {
    const ruta = url.startsWith("/api/") ? url.slice(4) : url;
    const { data } = await api.get(ruta, { responseType: "blob" });
    const objectUrl = URL.createObjectURL(data);
    window.open(objectUrl, "_blank");
  };

  return (
    <Link component="button" underline="hover" onClick={abrir} sx={{ mr: 1 }}>
      {label}
    </Link>
  );
}
