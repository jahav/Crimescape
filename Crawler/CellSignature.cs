using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler
{
	/// <summary>
	/// Signature of a cell. The cell must 
	/// </summary>
	internal class CellSignature
	{
		/// <summary>
		/// Relative offset from the cell.
		/// </summary>
		public CellOffset Offset { get; set; }

		/// <summary>
		/// Text mached against trimmed content of the cell.
		/// </summary>
		public string Text { get; set; }
	}
}
