using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Crimescape.Entities
{
	public class CrimeContext : DbContext
	{
		public CrimeContext()
			: base("name=DefaultConnection")
		{
		}

		public DbSet<Region> Regions { get; set; }
		
		public DbSet<Tsk> Tsks { get; set; }
		
		public DbSet<Category> Categories { get; set; }
	
		public DbSet<DataCategory> DataCategories { get; set; }

		public DbSet<CrimeCount> CrimeCounts { get; set; }

		public DbSet<ExcelDataSource> DataSources { get; set; }
	}
}