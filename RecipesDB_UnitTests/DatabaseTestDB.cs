using Microsoft.EntityFrameworkCore;
using Recipes_DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB_UnitTests
{
    public class DatabaseTestDB : IDisposable
    {
        protected readonly Recipes_DB1Context Context;

        public DatabaseTestDB()
        {

            //gebruik van inmemory data voor testdata.
            //gebruik van de context vh testen project
            var options = new
              DbContextOptionsBuilder<Recipes_DB1Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            //property dependancy voorbereiden
            Context = new Recipes_DB1Context(options);

            Context.Database.EnsureCreated(); //seed vanuit het standaard API project door de SaveChanges uit te voeren bij onmodelcreating

            //DatabaseInitializer.Initialize(Context); //extra seeding kan voor de test omgeving met FakeData
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();

            Context.Dispose();
        }
    }

}
