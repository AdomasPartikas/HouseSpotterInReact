describe('Registration Functionality', () => {
  beforeEach(() => {
    cy.visit('/registruotis');
  });

  it('Successfully registers with valid data', () => {
    cy.intercept('POST', 'housespotter/db/user/register', { fixture: 'register.json' }).as('registerRequest');
    cy.get('input[name="username"]').type('newuser');
    cy.get('input[name="email"]').type('newuser@email.com');
    cy.get('input[name="phone"]').type('1234567890');
    cy.get('input[name="password"]').type('newpassword');
    cy.get('button[type="submit"]').click();
    cy.wait('@registerRequest');
    cy.url().should('include', '/prisijungti');
    cy.contains('Sveikiname sėkmingai prisiregistravus. Dabar galite prisijungti.');
  });

  it('Registration fails with an existing username', () => {
    cy.intercept('POST', 'housespotter/db/user/register', { statusCode: 400 }).as('registerFailure');
    cy.get('input[name="username"]').type('Test1');
    cy.get('input[name="email"]').type('Test1@gmail.com');
    cy.get('input[name="phone"]').type('1234567890');
    cy.get('input[name="password"]').type('Test1');
    cy.get('button[type="submit"]').click();
    cy.wait('@registerFailure');
    cy.contains('Registracija nepavyko.');
  });
});