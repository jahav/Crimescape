using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Models
{
	/// <summary>
	/// One category of data. Equivalent of one column in the excel.
	/// </summary>
	public class Category
	{
		public DamageType DamageType { get; set; }

		public string CategoryName {get;set;}
		
		/// <summary>
		/// Which group does this category represents?
		/// </summary>
		public string GroupName { get; set; }
	}
}
