﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Crimescape.Entities
{
	public class ExcelDataSource
	{
		public int Id { get; set; }

		/// <summary>
		/// Real filename
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Original filename, can differ when uploading two files with same name.
		/// </summary>
		public string OriginalFilename { get; set; }

		public string Description { get; set; }

		/// <summary>
		/// Were data inserted into
		/// </summary>
		public bool Processed { get; set; }

		/// <summary>
		/// Date when the file was last inserted into database. If never, null.
		/// </summary>
		[DisplayFormat(NullDisplayText="Never")]
		public DateTime? ProcessedDate { get; set; }
	}
}