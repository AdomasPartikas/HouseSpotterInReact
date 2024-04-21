Cypress.Commands.add('screenshotOnFail', function () {
  cy.screenshot({ capture: 'runner' });
});

afterEach(function () {
  cy.screenshotOnFail();
});

describe('Product page functionality', () => {
  beforeEach(() => {
    cy.visit('/skelbimas/3');
    cy.intercept('GET', '/housespotter/db/getallhousing').as('getProductDetails');
  });

  it('Produkto detalės atvaizduojamos tinkamai', () => {
    cy.wait('@getProductDetails');

    // Check for the presence of product attributes
    cy.get('.product__block').within(() => {
      cy.get('.product__photo').should('exist');
      cy.get('h1 a').should('have.attr', 'href');
      cy.get('.product__content').should('contain', 'Būsto tipas:');
    });
  });

  it('Produkto aprašymas atvaizduojamas tinkamai', () => {
    cy.wait('@getProductDetails');

    // Check for description
    cy.get('.product__description').within(() => {
      cy.get('h2').should('contain', 'Aprašymas');
      cy.get('p').should('exist');
    });
  });
});
