Cypress.Commands.add('screenshotOnFail', function () {
  cy.screenshot({ capture: 'runner' });
});

afterEach(function () {
  cy.screenshotOnFail();
});

describe('Scraper page functionality', () => {
  beforeEach(() => {
    cy.visit('/scraper');
  });

  it('Atvaizduoja visų scraperių formas', () => {
    cy.get('.scraper-form').should('have.length', 3);
  });

  const scraperTests = [
    { title: 'Domoplius', allUrl: '/housespotter/scrapers/domo/findhousing/all', recentUrl: '/housespotter/scrapers/domo/findhousing/recent', enrichUrl: '/housespotter/scrapers/domo/enrichhousing' },
    { title: 'Aruodas', allUrl: '/housespotter/scrapers/aruodas/findhousing/all', recentUrl: '/housespotter/scrapers/aruodas/findhousing/recent', enrichUrl: '/housespotter/scrapers/aruodas/enrichhousing' },
    { title: 'Skelbiu', allUrl: '/housespotter/scrapers/skelbiu/findhousing/all', recentUrl: '/housespotter/scrapers/skelbiu/findhousing/recent', enrichUrl: '/housespotter/scrapers/skelbiu/enrichhousing' },
  ];

  scraperTests.forEach((scraper) => {
    it(`Palaiko visų skelbimų scrapinimą - ${scraper.title}`, () => {
      // Stub the POST request
      cy.intercept('POST', scraper.allUrl, {
        statusCode: 200,
        body: { scrapeStatus: 'Success', message: 'Scrape completed' },
      }).as('scrapeAllHousing');

      // Click the button to scrape all housing
      cy.contains('h2', scraper.title)
        .parent('.scraper-form')
        .contains('button', 'Find all housing')
        .click();

      // Wait for the request to complete
      cy.wait('@scrapeAllHousing');

      // Check for a success notification
      cy.contains('Operacija pavyko').should('be.visible');
    });

    it(`Palaiko naujausių skelbimų scrapinimą - ${scraper.title}`, () => {
      // Stub the POST request
      cy.intercept('POST', scraper.recentUrl, {
        statusCode: 200,
        body: { scrapeStatus: 'Success', message: 'Scrape completed' },
      }).as('scrapeRecentHousing');

      // Click the button to scrape all housing
      cy.contains('h2', scraper.title)
        .parent('.scraper-form')
        .contains('button', 'Find recent housing')
        .click();

      // Wait for the request to complete
      cy.wait('@scrapeRecentHousing');

      // Check for a success notification
      cy.contains('Operacija pavyko').should('be.visible');
    });

    it(`Palaiko skelbimų enrichinimą scrapinimą - ${scraper.title}`, () => {
      // Stub the POST request
      cy.intercept('POST', scraper.enrichUrl, {
        statusCode: 200,
        body: { scrapeStatus: 'Success', message: 'Scrape completed' },
      }).as('scrapeEnrichHousing');

      // Click the button to scrape all housing
      cy.contains('h2', scraper.title)
        .parent('.scraper-form')
        .contains('button', 'Enrich housing')
        .click();

      // Wait for the request to complete
      cy.wait('@scrapeEnrichHousing');

      // Check for a success notification
      cy.contains('Operacija pavyko').should('be.visible');
    });

  });
});
