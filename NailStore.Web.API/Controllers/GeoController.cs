using Microsoft.AspNetCore.Mvc;
using NailStore.Core.Interfaces;
using NailStore.Core.Models.Country;
using NailStore.Core.Models.GeoLocation;

namespace NailStore.Web.API.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class GeoController : ControllerBase
{
    private readonly IGeoService _geoService;

    public GeoController(IGeoService geoService)
    {
        _geoService = geoService;
    }

    [HttpGet]
    [Route("CitiesByRegionId")]
    public async Task<ActionResult<List<CityDTO>>> GetCitiesFromRegionIdAsync(int regionId)
    {
        return Ok(await _geoService.GetCitiesFromRegionId(regionId));
    }

    [HttpPost]
    public async Task<ActionResult<string>> AddLocationsAsync(List<GeolocationDTO> locations)
    {
        var result = await _geoService.AddGeolocationsAsync(locations);
        if (result.Header.StatusCode != 200)
        {
            // Если при вызове сервиса возникли ошибки, возвращается ошибка с описанием причины
            return Problem
            (
                detail: result.Header.Error,
                statusCode: result.Header.StatusCode,
                instance: HttpContext.Request.Path
            );
        }
        return Ok("Ok");
    }
}