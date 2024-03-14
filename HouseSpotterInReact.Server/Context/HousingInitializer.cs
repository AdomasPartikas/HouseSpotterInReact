using System;
using System.Collections.Generic;
using System.Linq;
using HouseSpotter.Server.Models;
using HouseSpotter.Server.Context;

namespace HouseSpotter.Server.DAL
{
    public static class HousingInitializer
    {
        public static void Initialize(HousingContext context)
        {
            context.Database.EnsureCreated();

            // Look for any housings.
            if (context.Housings.Any())
            {
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
            context.SaveChanges();
        }
    }
}