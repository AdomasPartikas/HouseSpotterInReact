import { defineConfig } from 'cypress'

export default defineConfig({
  projectId: '1wv25r',
  e2e: {
    setupNodeEvents(on, config) { },
    baseUrl: 'https://localhost:5173',
    supportFile: 'cypress/support/e2e.js'
  },
})
