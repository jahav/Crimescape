using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Models
{
	/// <summary>
	/// Number of certain crimes since start of year.
	/// </summary>
	public class CrimeCountDto
	{
		/// <summary>
		/// Date, generally last day of some month.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Number of crimes since the start of the year to the date..
		/// </summary>
		public int YtdCount { get; set; }

		public Tsk Tsk { get; set; }

		public string RegionName { get; set; }

		public Category Category { get; set;}
	}
}
