using NetTopologySuite.Geometries;

namespace NailStore.Data.Models;

public class City
{
    public int CityId { get; set; }
    public int RegionId { get; set; }
    public string NameCity { get; set; }
    public string TimeZone { get; set; }
    public Point Coordinates { get; set; } 
    public CountryRegion Region { get; set; }
    public List<ServiceModel> Services { get; set; }
}