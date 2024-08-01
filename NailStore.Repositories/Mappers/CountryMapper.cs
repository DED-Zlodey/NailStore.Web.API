using NailStore.Core.Models.Country;
using NailStore.Data.Models;
using NetTopologySuite.Geometries;

namespace NailStore.Repositories.Mappers;

public static class CountryMapper
{
    public static Country MapTo(this CountryDTO source) => new()
    {
        Id = source.Id,
        Name = source.Name,
        Regions = new List<Region>()
    };

    public static Region MapTo(RegionDTO source) => new()
{
    Id = source.Id,
    CountryId = source.CountryId,
    Cities = new List<City>(),
    Name = source.Name,
};

    public static City MapTo(this CityDTO source) => new()
    {
        Id = source.Id,
        RegionId = source.RegionId,
        NameCity = source.NameCity!,
        TimeZone = source.TimeZone,
        Coordinates = new Point(source.Longitude, source.Latitude),
    };
}