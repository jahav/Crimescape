namespace Crimescape.Migrations
{
	using Crimescape.Entities;
	using Crimescape.Models;
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Crimescape.Entities.CrimeContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Crimescape.Entities.CrimeContext context)
        {
            //  This method will be called after migrating to the latest version.

			context.Regions.AddOrUpdate(
				r => r.Code,
				new Region { Code = "CZ-JC", Name = "Jihoèeský kraj" },
				new Region { Code = "CZ-JM", Name = "Jihomoravský kraj" },
				new Region { Code = "CZ-KA", Name = "Karlovarský kraj" },
				new Region { Code = "CZ-KR", Name = "Královéhradecký kraj" },
				new Region { Code = "CZ-LI", Name = "Liberecký kraj" },
				new Region { Code = "CZ-MO", Name = "Moravskoslezský kraj" },
				new Region { Code = "CZ-OL", Name = "Olomoucký kraj" },
				new Region { Code = "CZ-PA", Name = "Pardubický kraj" },
				new Region { Code = "CZ-PL", Name = "Plzeòský kraj" },
				new Region { Code = "CZ-PR", Name = "Praha, hlavní mìsto" },
				new Region { Code = "CZ-ST", Name = "Støedoèeský kraj" },
				new Region { Code = "CZ-US", Name = "Ústecký kraj" },
				new Region { Code = "CZ-VY", Name = "Vysoèina" },
				new Region { Code = "CZ-ZL", Name = "Zlínský kraj" }
			);

			context.Categories.AddOrUpdate(
				c => c.Name,
				new Category { ApiName = "Detected", Name = "Zjištìno" },
				new Category { ApiName = "Verified", Name = "V provìøování" },
				new Category { ApiName = "Solved", Name = "Objasnìno" },
				new Category { ApiName = "Commited", Name = "Spácháno skutkù" },
				new Category { ApiName = "Investigated", Name = "Stíháno, vyšetøováno osob" },
				new Category { ApiName = "Damages", Name = "Škody" }
			);
        }
    }
}
