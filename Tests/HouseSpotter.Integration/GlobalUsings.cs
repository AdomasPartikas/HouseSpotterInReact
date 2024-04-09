global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using HouseSpotter.Server.Context;
global using HouseSpotter.Integration.Utils;
global using Newtonsoft.Json;
global using HouseSpotter.Server.Models.DTO;

global using Moq;
global using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]