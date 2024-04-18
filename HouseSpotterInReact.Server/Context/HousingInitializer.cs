using HouseSpotter.Server.Models;
using HouseSpotter.Server.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HouseSpotter.Server.DAL
{
    /// <summary>
    /// Class responsible for initializing the housing database.
    /// </summary>
    public static class HousingInitializer
    {
        /// <summary>
        /// Initializes the housing database with sample data.
        /// </summary>
        /// <param name="context">The housing context.</param>
        public static void Initialize(HousingContext context)
        {
            context.Database.EnsureCreated();

            // Look for any housings.
            if (context.Housings.Any())
            {
                var logger = context.GetService<ILogger<Program>>();
                logger.LogInformation("The database has already been seeded.");
                return;   // DB has been seeded
            }

            var housing = new List<Housing>{
            new Housing
            {
                AnketosKodas = "ANK123",
                Nuotrauka = null,
                Link = null,
                BustoTipas = "Butas",
                Title = "Jaukus butas Vilniaus senamiestyje",
                Kaina = 200000.00,
                Gyvenviete = "Vilnius",
                Gatve = "Senamiestis",
                KainaMen = 600.00,
                NamoNumeris = "5A",
                ButoNumeris = "12",
                KambariuSk = 3,
                Plotas = 75.5,
                SklypoPlotas = "Nėra",
                Aukstas = 2,
                AukstuSk = 5,
                Metai = 1990,
                Irengimas = "Pilnai įrengtas",
                NamoTipas = "Mūrinis",
                PastatoTipas = "Gyvenamasis",
                Sildymas = "Centrinis",
                PastatoEnergijosSuvartojimoKlase = "A",
                Ypatybes = "Su balkonu/Rūsys",
                PapildomosPatalpos = "Rūsys",
                PapildomaIranga = "Virtuvės technika",
                Apsauga = "Signalizacija",
                Vanduo = "Miesto vandentiekis",
                IkiTelkinio = 500,
                ArtimiausiasTelkinys = "Neris",
                RCNumeris = "RC12345",
                Aprasymas = "Patogioje Vilniaus vietoje parduodamas jaukus, šviesus butas."
            },
            new Housing
            {
                AnketosKodas = "ANK456",
                Nuotrauka = null,
                Link = null,
                BustoTipas = "Namas",
                Title = "Šiuolaikiškas namas su dideliu sklypu",
                Kaina = 350000.00,
                Gyvenviete = "Kaunas",
                Gatve = "Žaliakalnis",
                KainaMen = null, // Nes pirkimas, ne nuoma
                NamoNumeris = "8",
                ButoNumeris = null, // Nes tai namas, o ne butas
                KambariuSk = 5,
                Plotas = 120.0,
                SklypoPlotas = "10 a",
                Aukstas = null, // Nes tai namas, o ne daugiaaukštis pastatas
                AukstuSk = 2,
                Metai = 2005,
                Irengimas = "Pilnai įrengtas",
                NamoTipas = "Medinis",
                PastatoTipas = "Gyvenamasis",
                Sildymas = "Dujinis",
                PastatoEnergijosSuvartojimoKlase = "B",
                Ypatybes = "Terasa/Garažas",
                PapildomosPatalpos = "Garažas/Pirtis",
                PapildomaIranga = "Saugos sistema",
                Apsauga = "Video stebėjimas",
                Vanduo = "Artezinis gręžinys",
                IkiTelkinio = 1000,
                ArtimiausiasTelkinys = "Ežeras",
                RCNumeris = "RC67890",
                Aprasymas = "Parduodamas šiuolaikiškas namas su dideliu žemės sklypu."
            }
            };
            housing.ForEach(h => context.Housings.Add(h));

            var users = new List<User>{
            new User
            {
                Username = "user1",
                Password = "Password123!",
                Email = "user1@example.com",
                PhoneNumber = "+37060000001",
                CreatedAt = DateTime.Now,
                SavedSearches = new List<string> { "Vilnius", "Butas" },
                IsAdmin = false
            },
            new User
            {
                Username = "user2",
                Password = "SecurePassword!",
                Email = "user2@example.com",
                PhoneNumber = "+37060000002",
                CreatedAt = DateTime.Now,
                SavedSearches = null, // Galima nustatyti kaip null, jei vartotojas neturi išsaugotų paieškų
                IsAdmin = true
            },
            new User
            {
                Username = "user3",
                Password = "AnotherPass!",
                Email = "user3@example.com",
                PhoneNumber = null, // PhoneNumber neprivalomas, todėl galime palikti null
                CreatedAt = DateTime.Now,
                SavedSearches = new List<string> { "Kaunas", "Namas", "Sodas" },
                IsAdmin = false
            }
        };
            users.ForEach(u => context.Users.Add(u));

            var scrapes = new List<Scrape>{
                new Scrape
                {
                    ScrapeType = ScrapeType.Full,
                    ScrapeStatus = ScrapeStatus.Success,
                    ScrapedSite = ScrapedSite.Aruodas,
                    DateScraped = DateTime.Now,
                    ScrapeTime = new TimeSpan(0, 0, 32, 34),
                    TotalQueries = 100,
                    NewQueries = 100,
                    Message = "Scrape completed successfully"
                },
                new Scrape
                {
                    ScrapeType = ScrapeType.Partial,
                    ScrapeStatus = ScrapeStatus.Failed,
                    ScrapedSite = ScrapedSite.Domo,
                    DateScraped = DateTime.Now,
                    ScrapeTime = new TimeSpan(0, 0, 1, 52),
                    TotalQueries = 0,
                    NewQueries = 0,
                    Message = "Failed to connect to the server"
                }
            };

            scrapes.ForEach(s => context.Scrapes.Add(s));
            context.SaveChanges();
        }
    }
}