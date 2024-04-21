Cypress.Commands.add('screenshotOnFail', function () {
  cy.screenshot({ capture: 'runner' });
});

afterEach(function () {
  cy.screenshotOnFail();
});

describe('Nekilnojamo turto skelbimų peržiūra', () => {
  beforeEach(() => {
    // Įkeliamas pradinis puslapis
    cy.visit('/');

    // Laukiama, kol užkraus duomenis
    cy.intercept('GET', '/housespotter/db/getallhousing').as('getHousingData');
    cy.wait('@getHousingData');
  });

  it('Rodo skelbimų sąrašą su duomenimis iš dviejų šaltinių', () => {
    // Patikrinama, ar yra skelbimų sąrašas
    cy.get('.products .layout').should('exist');

    // Patikrinama, ar skelbimų sąraše yra įrašai iš dviejų šaltinių
    cy.get('.product').then((products) => {
      const scrapers = new Set();
      // Iteracija per produktus, naudojant jQuery each() metodą
      products.each((index, product) => {
        const scraper = Cypress.$(product).find('.product__scraper').text();
        scrapers.add(scraper.trim());
      });
      // Tikrinama, kad yra bent du skirtingi šaltiniai
      expect(scrapers.size).to.be.at.least(2);
    });
  });

  it('Leidžia vartotojui filtruoti skelbimus pagal kriterijus', () => {
    // Įvedama reikšmė į "kambarių skaičiaus nuo" filtrą
    cy.get('input[name="roomsFrom"]').type('2');
    cy.get('input[name="roomsTo"]').type('2');

    // Spaudžiamas filtravimo mygtukas
    cy.get('button[type="submit"]').click();

    // Laukiama, kol įkels filtruotus rezultatus.
    // Pavyzdžiui, galima laukti kol pasikeis URL arba kol atsiras tam tikras elementas.

    // Patikrinama, ar rezultatuose yra skelbimai su atitinkamu kambarių skaičiumi.
    // Priklausomai nuo to, kaip skelbimai yra rodomi, čia turi būti teisingas selektorius.
    cy.get('.product').each(($product) => {
      // Čia priklauso nuo to, kaip yra pateikiama kambarių informacija.
      // Tarkime, kad .product__rooms yra selektorius, kuris rodo kambarių skaičių.
      cy.wrap($product).find('.product__rooms p span').should('contain', '2');
    });
  });


});
