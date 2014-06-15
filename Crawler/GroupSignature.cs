using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
	/// <summary>
	/// Signature of a group. Its <see cref="Signature"/> is used to against a column in the excel. If all signatures match,
	/// the column is of this category and group type.
	/// </summary>
	internal class GroupSignature
	{
		/// <summary>
		/// Gets a list of signatures that are checked againt the top row and some column of the header.
		/// </summary>
		public CellSignature[] Signatures { get; set; }

		public string CategoryName { get; set; }

		public string GroupName { get; set; }
	}
}
