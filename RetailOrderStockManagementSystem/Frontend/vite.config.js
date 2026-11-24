import react, { reactCompilerPreset } from '@vitejs/plugin-react'
import babel from '@rolldown/plugin-babel'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    babel({ presets: [reactCompilerPreset()] })
  ],
  server: {    
     proxy: {       
      '/api': {         
        target: 'http://localhost:5173',         
        changeOrigin: true,         
        // Optional: remove '/api' prefix before forwarding to backend
        // rewrite: (path) => path.replace(/^\/api/, ''),      
        },     
      },   
    },
})
