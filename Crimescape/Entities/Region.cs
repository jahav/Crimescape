using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crimescape.Entities
{
	public class Region
	{
		public int Id { get; set; }

		/// <summary>
		/// Name of the region.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Region code based on the ISO 3166-2:CZ. Used by Geochart API.
		/// </summary>
		public string Code { get; set; }
	}
}
