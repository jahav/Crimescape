using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crimescape.Entities
{
	public class Category
	{
		public int Id { get; set; }

		/// <summary>
		/// Human readable name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Name used in the API, e.g. in URL.
		/// </summary>
		public string ApiName { get; set; }
	}
}
