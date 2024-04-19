describe('Prisijungimo funkcionalumas', () => {
  beforeEach(() => {
    cy.visit('/prisijungti');
  });

  it('Sėkmingai prisijungia su teisingais duomenimis', () => {
    cy.intercept('POST', 'housespotter/db/user/login', { fixture: 'login.json' }).as('loginRequest');
    cy.get('input[name="text"]').type('user');
    cy.get('input[name="password"]').type('password');
    cy.get('button[type="submit"]').click();
    cy.wait('@loginRequest');
    cy.url().should('include', '/megstamiausi');
    cy.contains('Sveiki prisijungę, user!');
  });

  it('Prisijungimas nepavyksta su neteisingu slaptažodžiu', () => {
    cy.intercept('POST', 'housespotter/db/user/login', { statusCode: 204 }).as('loginFailure');
    cy.get('input[name="text"]').type('user');
    cy.get('input[name="password"]').type('wrongpassword');
    cy.get('button[type="submit"]').click();
    cy.wait('@loginFailure');
    cy.contains('Prisijungti nepavyko.');
  });
});
