namespace NailStore.Core.Models.Country;

public class RegionDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CountryId { get; set; }
    public List<CityDTO> Cities { get; set; }
}