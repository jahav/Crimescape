using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Crimescape.Models
{
	public class ValidateFileAttribute : RequiredAttribute
	{
		/// <summary>
		/// Maximal size of files in bytes
		/// </summary>
		const int MaxFileSize = 5*1024*1024;

		public override bool IsValid(object value)
		{
			var file = value as HttpPostedFileBase;
			if (file == null)
			{
				return false;
			}

			if (file.ContentLength > MaxFileSize)
			{
				ErrorMessage = string.Format("File size must be less than {0:N0} bytes", MaxFileSize);
				return false;
			}
			return true;
		}
	}

	/// <summary>
	/// View model for excel upload.
	/// </summary>
	public class ExcelDataViewModel
	{
		public string Description { get; set; }

		[ValidateFile]
		public HttpPostedFileBase File { get; set; }
	}
}