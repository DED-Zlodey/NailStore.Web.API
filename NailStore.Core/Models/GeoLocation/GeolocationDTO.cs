namespace NailStore.Core.Models.GeoLocation;

public class GeolocationDTO
{
    public int RegionId { get; set; }
    public string Postcode { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string House { get; set; }
    public string Address { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}