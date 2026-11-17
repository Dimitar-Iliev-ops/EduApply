import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

export default defineConfig({
  plugins: [react()],
  server: {
    host: '127.0.0.1',
    port: 5173,
    strictPort: true,
    proxy: {
      '/api/catalog': 'http://127.0.0.1:5201',
      '/api/applications': 'http://127.0.0.1:5202',
      '/api/ranking': 'http://127.0.0.1:5203',
    },
  },
})
