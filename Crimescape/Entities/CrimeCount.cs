using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crimescape.Entities
{

	public class CrimeCount
	{
		public int Id { get; set; }

		/// <summary>
		/// Last day of some month. Crime count is from start of the year to this date. 
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Number of crimes since the start of the year to the <see cref="Date"/>.
		/// </summary>
		public int YtdCount { get; set; }

		public Tsk Tsk { get; set; }

		public Region Region { get; set; }

		public DataCategory Category { get; set; }
	}
}