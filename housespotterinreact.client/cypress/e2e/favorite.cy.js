Cypress.Commands.add('screenshotOnFail', function () {
  cy.screenshot({ capture: 'runner' });
});

afterEach(function () {
  cy.screenshotOnFail();
});

Cypress.Commands.add('login', () => {
  const user = {
    id: '4',
    username: 'Test1',
    email: 'Test1@gmail.com',
    phoneNumber: 'Test1',
    password: 'Test1',
    isAdmin: true,
  };
  cy.window().then((window) => {
    window.localStorage.setItem('user', JSON.stringify(user));
  });
});

Cypress.Commands.add('logout', () => {
  cy.window().then((window) => {
    window.localStorage.removeItem('user');
  });
});

// cypress/integration/favorite_spec.js
describe('Mėgstamiausių skelbimų funkcionalumas', () => {
  beforeEach(() => {
    // Log in and set up intercepts
    cy.login();
    cy.intercept('POST', 'housespotter/db/user/login', {
      id: '4', username: 'Test1', email: 'Test1@gmail.com', phoneNumber: 'Test1', password: 'Test1', isAdmin: true
    }).as('login');
    cy.intercept('GET', '/housespotter/db/user/4/savedSearches', {
      fixture: 'favorites.json' // Make sure this fixture contains an array of favorite items
    }).as('getFavorites');
    cy.visit('/prisijungti');
    cy.get('input[name="text"]').type('Test1');
    cy.get('input[name="password"]').type('Test1');
    cy.get('button[type="submit"]').click();
    cy.wait('@login');
  });

  it('Leidžia vartotojui pridėti skelbimą į mėgstamiausių sąrašą', () => {
    // Now perform actions that require user to be logged in
    cy.visit('/'); // Go to the page with the products
    cy.get('.product__favorite').first().click(); // Click the favorite button on the first product
    cy.wait('@getFavorites'); // Wait for the favorites list to be fetched again

    // Assertion to check the favorite was added, adjust the selector as necessary
    cy.get('.product__favorite.active').should('exist');
  });

  it('Rodo vartotojo mėgstamiausių skelbimų sąrašą', () => {
    cy.visit('/megstamiausi');
    cy.wait('@getFavorites');
    cy.get('.product').should('have.length.at.least', 1);
  });

  it('Leidžia vartotojui šalinti skelbimus iš mėgstamiausių sąrašo', () => {
    cy.visit('/megstamiausi');
    cy.wait('@getFavorites');

    // Get the initial count of favorite products
    cy.get('.product').its('length').then((initialCount) => {
      expect(initialCount).to.be.at.least(1);

      cy.get('.product__favorite.active').first().invoke('attr', 'data-product-id').then((productId) => {
        cy.get('.product__favorite.active').first().click();
        cy.contains('Skelbimas pašalintas iš mėgstamiausių.');
      });
    });
  });


  it('Išsaugo mėgstamiausių skelbimų sąrašą tarp įrenginių ir sesijų', () => {
    cy.visit('/megstamiausi');
    cy.wait('@getFavorites');

    // Get the title of the first favorite product
    cy.get('.product__title').first().then(($title) => {
      const firstProductTitle = $title.text();

      // Simulate logging out
      cy.logout();

      // Simulate logging in again with the same user details
      const user = { id: '4', username: 'Test1', email: 'Test1@gmail.com', phoneNumber: 'Test1', password: 'Test1', isAdmin: true };
      cy.login(user);

      cy.visit('/megstamiausi');
      cy.wait('@getFavorites');

      // Check if the first product is still the same, thus saved across sessions
      cy.get('.product__title').first().should('have.text', firstProductTitle);
    });

  });
});