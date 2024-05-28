namespace Dannys.AutoMappers;

public class TableAutoMapper:Profile
{
	public TableAutoMapper()
	{
		CreateMap<Table, TableCreateDto>().ReverseMap();
		CreateMap<Table, TableGetDto>().ReverseMap();
	}
}

