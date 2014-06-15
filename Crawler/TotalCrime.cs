using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Crawler.Models;
using NPOI.SS.Util;
using System.Text.RegularExpressions;

namespace Crawler
{
	public class TotalCrime
	{
		public IReadOnlyList<CrimeCountDto> ReadCrimeData(string filename)
		{
			using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				return ReadCrimeData(file);
			}
		}

		public IReadOnlyList<CrimeCountDto> ReadCrimeData(System.IO.Stream stream)
		{
			var hssfwb = new HSSFWorkbook(stream);
			IDictionary<int, Category> columnMapping = GetColumnMapping(hssfwb);
			var result = new List<CrimeCountDto>();

			for (var sheetIdx = 0; sheetIdx < hssfwb.NumberOfSheets; sheetIdx++)
			{
				result.AddRange(ReadCrimeData(hssfwb.GetSheetAt(sheetIdx), columnMapping));
			}
			return result;
		}

		/// <summary>
		/// Gets all data points in one sheet.
		/// </summary>
		/// <param name="sheet">Sheet used to extract data.</param>
		/// <param name="columnMapping">Mapping of columns to categories.</param>
		/// <returns></returns>
		private IEnumerable<CrimeCountDto> ReadCrimeData(ISheet sheet, IDictionary<int, Category> columnMapping)
		{
			string regionName = GetRegionName(sheet);
			DateTime ytdDate = GetYtdDate(sheet);
			var parsedData = new List<CrimeCountDto>();
			foreach (var row in GetDataRows(sheet))
			{
				foreach (var rowData in GetRowData(row, columnMapping))
				{
					parsedData.Add(new CrimeCountDto
						{
							Tsk = GetTsk(row),
							Category = rowData.Key,
							YtdCount = rowData.Value,
							RegionName = regionName,
							Date = ytdDate
						});
				}
			}
			return parsedData;
		}


		private DateTime GetYtdDate(ISheet sheet)
		{
			var cellReference = new CellReference("H3");
			var timeRange = sheet.GetRow(cellReference.Row).GetCell(cellReference.Col).StringCellValue;
			var parts = timeRange.Split(' ');
			if (parts.Length != 3 || parts[1] != "do")
			{
				throw new InvalidOperationException(string.Format("Unexpected timerange {0}", timeRange));
			}
			var fromDate = DateTime.ParseExact(parts[0], "d.M.yyyy", null);
			var toDate = DateTime.ParseExact(parts[2], "d.M.yyyy", null);
			if (fromDate.Day != 1 || fromDate.Month != 1 || fromDate.Year != toDate.Year 
				|| toDate < fromDate || toDate.Day != DateTime.DaysInMonth(toDate.Year, toDate.Month))
			{
				throw new ArgumentOutOfRangeException(string.Format("Wrong timerange {0}.", timeRange));
			}
			return toDate;
		}

		private Tsk GetTsk(IRow row)
		{
			var tskNumber = row.GetCell(TskColumn).NumericCellValue;
			if (tskNumber != Math.Truncate(tskNumber))
			{
				throw new InvalidOperationException(string.Format("Row {0} doesn't have int TSK.", row.RowNum));
			}
			var tskDescription = row.GetCell(TskColumn + 1).StringCellValue.Trim();
			return new Tsk { Number = (int)tskNumber, Name = tskDescription };
		}

		/// <summary>
		/// Get name of region the data in sheet are for.
		/// </summary>
		/// <param name="sheet">Sheet to get data from.</param>
		/// <returns>Name of the region</returns>
		private string GetRegionName(ISheet sheet)
		{
			// Cell C5
			var regionCell = new CellReference("C5");
			var hqName = sheet.GetRow(regionCell.Row).GetCell(regionCell.Col).StringCellValue.Trim();
			if (hqName == string.Empty)
			{
				throw new ArgumentException(string.Format("Cell {1} at sheet {0} doesn't contain region HQ name.", sheet.SheetName, regionCell.FormatAsString()));
			}
			return hqName;
		}

		private IDictionary<Category, int> GetRowData(IRow row, IDictionary<int, Category> columnMapping)
		{
			var rowData = new Dictionary<Category, int>();
			foreach (var entry in columnMapping)
			{
				var cell = row.Cells[entry.Key];
				if (cell.NumericCellValue != Math.Truncate(cell.NumericCellValue))
				{
					throw new InvalidOperationException(string.Format("Cell {0} doesn't have int value.", new CellReference(cell).FormatAsString()));
				}
				rowData.Add(entry.Value, (int)cell.NumericCellValue);
			}
			return rowData;
		}

		private IEnumerable<IRow> GetDataRows(ISheet sheet)
		{
			foreach (IRow row in sheet)
			{
				if (row.FirstCellNum >= TskColumn && TskColumn < row.LastCellNum)
				{
					continue;
				}
				var cell = row.Cells[TskColumn];
				if (cell.CellType == CellType.Numeric && cell.NumericCellValue == Math.Truncate(cell.NumericCellValue))
				{
					var tsk = Math.Truncate(cell.NumericCellValue);
					yield return row;
				}
			}
		}

		/// <summary>
		/// Also validates.
		/// </summary>
		/// <param name="workbook"></param>
		/// <returns></returns>
		private IDictionary<int, Category> GetColumnMapping(HSSFWorkbook workbook)
		{
			for (var sheetIdx = 0; sheetIdx < workbook.NumberOfSheets; sheetIdx++)
			{
				var sheet = workbook.GetSheetAt(sheetIdx);
				foreach (var tskIdx in GetTskRowIdxs(sheet))
				{
					// tsk is row index of text, the header starts two rows above it.
					return GetColumnMapping(sheet, tskIdx - 2);
				}
			}
			throw new InvalidOperationException("Unable to find header in excel file.");
		}

		/// <summary>
		/// Index of a column that contains TSK codes of data.
		/// </summary>
		const int TskColumn = 1;

		/// <summary>
		/// Get all TSK annotations. Under them are actuall numbers and rows with data.
		/// </summary>
		/// <param name="worksheet"></param>
		/// <returns>Row indexes where TSK is.</returns>
		private IEnumerable<int> GetTskRowIdxs(ISheet worksheet)
		{
			for (int rowIdx = 0; rowIdx <= worksheet.LastRowNum; rowIdx++)
			{
				var row = worksheet.GetRow(rowIdx);
				var cell = row.GetCell(TskColumn);

				if (cell.CellType == CellType.String && cell.StringCellValue == "TSK")
				{
					yield return rowIdx;
				}
			}

		}

		/// <summary>
		/// Create a column-category mapping based on the header.
		/// </summary>
		/// <param name="sheet">Sheel where is the header.</param>
		/// <param name="headerRowIdx">Index of first header row.</param>
		private IDictionary<int, Category> GetColumnMapping(ISheet sheet, int headerRowIdx)
		{
			var patterns = GetSignatures();

			var mapping = new Dictionary<int, Category>();

			var headerRow = sheet.GetRow(headerRowIdx);
			for (int columnIdx = headerRow.FirstCellNum; columnIdx < headerRow.LastCellNum; columnIdx++)
			{
				foreach (var pattern in patterns)
				{
					if (ConfirmPattern(sheet, headerRowIdx, columnIdx, pattern))
					{
						//var categoryWithDataType = mapping.Values.FirstOrDefault(cat => cat.DataType.Name == pattern.DataTypeName);
						//var dataType = categoryWithDataType != null
						//	? categoryWithDataType.DataType
						//	: new DataType { Name = pattern.DataTypeName };

						mapping.Add(columnIdx, 
							new Category 
							{ 
								DamageType = Models.DamageType.CrimesCount, 
								GroupName = pattern.GroupName, 
								CategoryName = pattern.CategoryName 
							});
					}
				}
			}
			return mapping;
		}

		/// <summary>
		/// Does cell at <paramref name="columnIdx"/> and <paramref name="headerRowIdx"/> conform to all signatures in the <paramref name="pattern"/>?
		/// </summary>
		/// <param name="sheet">Tested sheet.</param>
		/// <param name="headerRowIdx"></param>
		/// <param name="columnIdx"></param>
		/// <param name="pattern"></param>
		/// <returns>Ture is all signatures in <paramref name="pattern"/> match, false otherwise</returns>
		private bool ConfirmPattern(ISheet sheet, int headerRowIdx, int columnIdx, GroupSignature pattern)
		{
			foreach (var signature in pattern.Signatures)
			{
				if (headerRowIdx + signature.Offset.Row < sheet.FirstRowNum
					|| headerRowIdx + signature.Offset.Row > sheet.LastRowNum)
				{
					return false;
				}

				var row = sheet.GetRow(headerRowIdx + signature.Offset.Row);

				if (columnIdx + signature.Offset.Column < row.FirstCellNum
					|| columnIdx + signature.Offset.Column >= row.LastCellNum)
				{
					return false;
				}

				var cell = row.Cells[columnIdx + signature.Offset.Column];
				if (cell.ToString().Trim() != signature.Text)
				{
					return false;
				}
			}

			return true;
		}

		private static GroupSignature[] GetSignatures()
		{
			var patterns =
				new[] {
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 0 }, Text = "" }, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0 }, Text = "Zjištěno" }, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0 }, Text = "" } },
						CategoryName = "Detected",
						GroupName = "Celkem"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 0}, Text = "z toho"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "ukončeno"},
							new CellSignature { Offset = new CellOffset { Row = 1, Column = -1}, Text = "Zjištěno"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "prověřování"} },
						CategoryName = "Detected",
						GroupName = "Ukončeno prověřování"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 0}, Text = "Celkem"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "v prově-"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0 },Text = "řování"}},
						CategoryName = "Verified",
						GroupName = "Celkem"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 0}, Text = "Objasněno"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = ""}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "Počet" }},
						CategoryName = "Solved",
						GroupName = "Počet"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = -1}, Text = "Objasněno"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0},  Text = "Doda-"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "tečně" }},
						CategoryName = "Solved",
						GroupName = "Dodatečně"
					},
					// 2008 has also ignored percentage column
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = -2}, Text = "Objasněno"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "Doda-"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0 }, Text = "tečně" }},
						CategoryName = "Solved",
						GroupName = "Dodatečně"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 3}, Text = "Spácháno skutků"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0},  Text = "Pod"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "vlivem" }},
						CategoryName = "Commited",
						GroupName = "Pod vlivem"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 2}, Text = "Spácháno skutků"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "Z toho"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "alkohol" }},
						CategoryName = "Commited",
						GroupName = "Pod vlivem alkoholu"
					},
					// 2008
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 2}, Text = "Spácháno skutků"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "Alko-"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "hol" }},
						CategoryName = "Commited",
						GroupName = "Pod vlivem alkoholu"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 1}, Text = "Spácháno skutků"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "Reci-"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "divisté" }},
						CategoryName = "Commited",
						GroupName = "Recidivisté"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 0}, Text = "Spácháno skutků"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "Nezletilí"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "1-14 let" }},
						CategoryName = "Commited",
						GroupName = "Nezletilí"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = -1}, Text = "Spácháno skutků"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "Mladiství"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "15-17 let" }},
						CategoryName = "Commited",
						GroupName = "Mladiství"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = -2}, Text = "Spácháno skutků"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "Děti"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "1-17 let" }},
						CategoryName = "Commited",
						GroupName = "Děti"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 0}, Text = "Stíháno, vyšetřováno osob"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = ""}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "Celkem" }},
						CategoryName = "Investigated",
						GroupName = "Celkem"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = -1}, Text = "Stíháno, vyšetřováno osob"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "Reci-"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "divisté"}},
						CategoryName = "Investigated",
						GroupName = "Recidivisté"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = -2}, Text = "Stíháno, vyšetřováno osob"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "Nezletilí"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "1-14 let"}},
						CategoryName = "Investigated",
						GroupName = "Nezletilí"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = -3 }, Text = "Stíháno, vyšetřováno osob"}, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0 }, Text = "Mladiství"}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0 }, Text = "15-17 let" }},
						CategoryName = "Investigated",
						GroupName = "Mladiství"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = -4}, Text = "Stíháno, vyšetřováno osob" }, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = ""}, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "Ženy"} },
						CategoryName = "Investigated",
						GroupName = "Ženy"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = 0}, Text = "Škody" }, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = 0}, Text = "v tis. Kč" }, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "Celkem"} },
						CategoryName = "Damages",
						GroupName = "Celkem"
					},
					new GroupSignature
					{
						Signatures = new[] { 
							new CellSignature { Offset = new CellOffset { Row = 0, Column = -1}, Text = "Škody" }, 
							new CellSignature { Offset = new CellOffset { Row = 1, Column = -1}, Text = "v tis. Kč" }, 
							new CellSignature { Offset = new CellOffset { Row = 2, Column = 0}, Text = "Zajištěno" } },
						CategoryName = "Damages",
						GroupName = "Zajištěno"
					}
				};
			return patterns;
		}
	}
}
