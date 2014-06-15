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
				new Region { Code = "CZ-JC", Name = "Jiho�esk� kraj" },
				new Region { Code = "CZ-JM", Name = "Jihomoravsk� kraj" },
				new Region { Code = "CZ-KA", Name = "Karlovarsk� kraj" },
				new Region { Code = "CZ-KR", Name = "Kr�lov�hradeck� kraj" },
				new Region { Code = "CZ-LI", Name = "Libereck� kraj" },
				new Region { Code = "CZ-MO", Name = "Moravskoslezsk� kraj" },
				new Region { Code = "CZ-OL", Name = "Olomouck� kraj" },
				new Region { Code = "CZ-PA", Name = "Pardubick� kraj" },
				new Region { Code = "CZ-PL", Name = "Plze�sk� kraj" },
				new Region { Code = "CZ-PR", Name = "Praha, hlavn� m�sto" },
				new Region { Code = "CZ-ST", Name = "St�edo�esk� kraj" },
				new Region { Code = "CZ-US", Name = "�steck� kraj" },
				new Region { Code = "CZ-VY", Name = "Vyso�ina" },
				new Region { Code = "CZ-ZL", Name = "Zl�nsk� kraj" }
			);

			context.Categories.AddOrUpdate(
				c => c.Name,
				new Category { ApiName = "Detected", Name = "Zji�t�no" },
				new Category { ApiName = "Verified", Name = "V prov��ov�n�" },
				new Category { ApiName = "Solved", Name = "Objasn�no" },
				new Category { ApiName = "Commited", Name = "Sp�ch�no skutk�" },
				new Category { ApiName = "Investigated", Name = "St�h�no, vy�et�ov�no osob" },
				new Category { ApiName = "Damages", Name = "�kody" }
			);
        }
    }
}
