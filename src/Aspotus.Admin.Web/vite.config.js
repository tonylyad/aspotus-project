import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const gatewayTarget = process.env.VITE_GATEWAY_URL ?? 'http://localhost:5230'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: gatewayTarget,
        changeOrigin: true,
      },
      '/catalog': {
        target: gatewayTarget,
        changeOrigin: true,
      },
      '/orders': {
        target: gatewayTarget,
        changeOrigin: true,
      },
      '/files': {
        target: gatewayTarget,
        changeOrigin: true,
      },
    },
  },
})
