using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Json;
using Crawler.Models;
using System.IO;
using System.Linq;

namespace Crawler.Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void SaveCrimes()
		{
			var serializer = new DataContractJsonSerializer(typeof(CrimeCountDto));
			var result = new TotalCrime().ReadCrimeData(@"c:\tmp\crimescope\Crime2014 - 30.4.xls");
			using (var stream = File.Create(@"c:/tmp/json.json")) {

				stream.Write(new[] {Convert.ToByte('[')}, 0, 1);
				foreach (var dataPoint in result)
				{
					if (result[0] != dataPoint)
					{
						stream.Write(new[] { Convert.ToByte(',') }, 0, 1);
					}
					serializer.WriteObject(stream, dataPoint);
					// extra comma at the end.
					//Console.WriteLine("{0} {1} {2} {3} {4}", dataPoint.Tsk.Number, dataPoint.Tsk.Name, dataPoint.Category.CategoryName, dataPoint.Category.GroupName, dataPoint.YtdCount);
				}
				stream.Write(new[] { Convert.ToByte(']') }, 0, 1);
			}

		}
	}
}
