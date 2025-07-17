import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";
import tailwindcss from "@tailwindcss/vite";
import basicSsl from "@vitejs/plugin-basic-ssl";

export default defineConfig({
  base: "/",
  plugins: [react(), tailwindcss(), basicSsl()],
  preview: {
    port: 8080,
    strictPort: true,
  },
  server: {
    https: true,
    port: 8080,
    strictPort: true,
    host: true,
    origin: "http://0.0.0.0:8080",
  },
});
