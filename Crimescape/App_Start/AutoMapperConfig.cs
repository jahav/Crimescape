using AutoMapper;

namespace Crimescape
{
	public static class AutoMapperConfig
	{
		public static void RegisterMaps()
		{
			Mapper.CreateMap<Crimescape.Entities.ExcelDataSource, Crimescape.Models.ExcelDataViewModel>();
		}
	}
}