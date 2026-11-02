import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'

// 代理前缀与后端控制器路由分离，开发时无需修改后端跨域设置。
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const proxy = {
    '/api': {
      target: env.API_PROXY_TARGET || 'http://localhost:5123',
      changeOrigin: true,
      rewrite: (path) => path.replace(/^\/api/, ''),
      timeout: 240_000,
      proxyTimeout: 240_000,
    },
  }
  return { plugins: [vue()], server: { port: 5173, strictPort: true, proxy }, preview: { proxy } }
})
