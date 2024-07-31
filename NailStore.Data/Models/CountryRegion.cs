namespace NailStore.Data.Models;

public class CountryRegion
{
    public int RegionId { get; set; }
    public int CountryId { get; set; }
    public required string RegionName { get; set; }
    public List<City> Cities { get; set; }
    public Country? Country { get; set; }
}