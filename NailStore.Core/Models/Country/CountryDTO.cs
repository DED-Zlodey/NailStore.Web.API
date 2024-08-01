namespace NailStore.Core.Models.Country;

public class CountryDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<RegionDTO> Regions { get; set; }
}