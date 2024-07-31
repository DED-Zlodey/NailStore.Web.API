namespace NailStore.Data.Models;

public class Country
{
    public int CountryId { get; set; }
    public required string CountryName { get; set; }
    public List<CountryRegion> Regions { get; set; }
}