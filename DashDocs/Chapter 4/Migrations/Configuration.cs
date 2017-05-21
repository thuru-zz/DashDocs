namespace DashDocs.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DashDocs.Services.DashDocsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DashDocs.Services.DashDocsContext context)
        {
            Guid customerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72");

            Guid userIdKron = Guid.Parse("2A37108E-56AF-4C18-99C4-415191591CD9");
            Guid userIdTron = Guid.Parse("C22514F4-976E-48FD-AB3E-C12E945B3652");

            context.Customers.AddOrUpdate(
              c => c.Id,
              new Customer { Id = customerId, Name = "DashDocDevs" }
            );

            context.Users.AddOrUpdate(
                u => u.Id,
                new User { Id = userIdKron, FirstName = "Kron", LastName = "Linda", CustomerId = customerId},
                new User { Id = userIdTron, FirstName = "Tron", LastName = "Spagner", CustomerId = customerId}
            );

        }
    }
}
