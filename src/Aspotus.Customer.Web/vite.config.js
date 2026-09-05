import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

const gatewayTarget = globalThis.process?.env.VITE_GATEWAY_URL ?? 'http://localhost:5230';

export default defineConfig({
    plugins: [plugin()],
    test: {
        environment: 'jsdom',
        setupFiles: './src/test/setup.js',
    },
    server: {
        port: 5173,
        proxy: {
            '/api': { target: gatewayTarget, changeOrigin: true },
            '/catalog': { target: gatewayTarget, changeOrigin: true },
            '/orders': { target: gatewayTarget, changeOrigin: true },
            '/files': { target: gatewayTarget, changeOrigin: true },
        },
    }
})
