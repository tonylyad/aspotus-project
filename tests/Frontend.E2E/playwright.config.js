import { defineConfig, devices } from '@playwright/test'
import { fileURLToPath } from 'node:url'

const repositoryRoot = fileURLToPath(new URL('../..', import.meta.url))

export default defineConfig({
  testDir: './specs',
  fullyParallel: false,
  retries: 0,
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
  },
  projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
  webServer: {
    command: 'docker compose -f src/docker-compose.yml up --build -d',
    cwd: repositoryRoot,
    url: 'http://localhost:5230/swagger/index.html',
    reuseExistingServer: true,
    timeout: 180_000,
  },
})
