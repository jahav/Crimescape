using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crimescape.Entities
{
	/// <summary>
	/// Link from the data point to some categories.
	/// </summary>
	public class DataCategory
	{
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the category of the data.
		/// </summary>
		public Category Category { get; set; }

		/// <summary>
		/// Gets or sets the group of the data.
		/// </summary>
		public string GroupName { get; set; }
	}
}
