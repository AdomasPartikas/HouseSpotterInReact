import { defineConfig } from 'cypress';
import cypressMochawesomeReporterPlugin from 'cypress-mochawesome-reporter/plugin.js';

export default defineConfig({
  projectId: '1wv25r',
  reporter: 'mochawesome',
  reporterOptions: {
    reportDir: 'cypress/reports',
    overwrite: false,
    html: false,
    json: true
  },
  e2e: {
    setupNodeEvents(on, config) { },
    baseUrl: 'https://localhost:5173',
    supportFile: 'cypress/support/e2e.js'
  },
});
