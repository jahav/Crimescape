using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crimescape.Entities
{
	public class Tsk
	{
		public int Id { get; set; }

		/// <summary>
		/// TSK number.
		/// </summary>
		public int Number { get; set; }

		/// <summary>
		/// Human readable name of TSK.
		/// </summary>
		public string Name { get; set; }
	}
}
