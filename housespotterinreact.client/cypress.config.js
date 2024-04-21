import { defineConfig } from 'cypress'

export default defineConfig({
  e2e: {
    setupNodeEvents(on, config) { },
    baseUrl: 'https://localhost:5173',
    supportFile: 'cypress/support/e2e.js'
  },
})
