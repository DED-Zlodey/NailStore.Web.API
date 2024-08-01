using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NailStore.Application;
using NailStore.Core.Models;
using NailStore.Data;
using NailStore.Data.Models;
using NailStore.Repositories;

namespace NailStore.xUnit.Tests;

public class ProviderServiceTests
{
    private readonly ProviderService _providerService;
    private readonly Mock<ServiceRepository> _serviceRepository;
    private readonly ApplicationDbContext _context;
    private readonly UserEntity _testUser;

    public ProviderServiceTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("DbName");
        _context = new ApplicationDbContext(optionsBuilder.Options);
        _serviceRepository = new Mock<ServiceRepository>(_context, new Mock<ILogger<ServiceRepository>>().Object);
        _providerService = new ProviderService(_serviceRepository.Object);
        _testUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "testUser",
            Email = "test@test.com",
            NormalizedEmail = "test@test.com".ToUpper(),
            NormalizedUserName = "testUser".ToUpper(),
            RegisterAt = DateTime.Now,
            LockoutEnabled = false,
        };
        InitDbData();
    }

    /// <summary>
    /// Initializes the database with test data for categories and users.
    /// </summary>
    private void InitDbData()
    {
        // Create a new category with test data
        var category1 = new CategoryServiceModel
        {
            CategoryId = 1,
            CategoryName = "Test Category",
            Description = "Test Description"
        };

        // Check if the category already exists in the database
        var ent = _context.CategoriesServices.Find(category1.CategoryId);

        // If the category does not exist, add it to the database
        if (ent == null)
        {
            _context.CategoriesServices.Add(new CategoryServiceModel
            {
                CategoryId = 1,
                CategoryName = "Test Category",
                Description = "Test Description"
            });
        }

        // Check if the test user already exists in the database
        var user = _context.Users.Find(_testUser.Id);

        // If the user does not exist, add it to the database
        if (user == null)
        {
            _context.Users.Add(_testUser);
        }

        // Save changes to the database
        _context.SaveChanges();
    }

    [Fact]
    public async Task AddServiceAsync_ValidData_ReturnsSuccessResponse()
    {
        // Arrange

        // Act
        var result = await _providerService.AddServiceAsync(_testUser.Id, 1, "Test Service",
            new string[] { "Description 1", "Description 2" }, 100m, 30);

        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        Assert.Equal("Услуга успешно добавлена", result.Body.Message);
    }

    [Fact]
    public async Task AddServiceAsync_InvalidUserId_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _providerService.AddServiceAsync(Guid.Empty, 1, "Test Service",
            new string[] { "Description 1", "Description 2" }, 100m, 30);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddServiceAsync_NegativePrice_ReturnsErrorResponse()
    {
        // Arrange

        // Act
        var result = await _providerService.AddServiceAsync(_testUser.Id, 1, "Test Service",
            new string[] { "Description 1", "Description 2" }, -100m, 30);

        // Assert
        Assert.Equal(400, result.Header.StatusCode);
    }

    [Fact]
    public async Task AddServiceAsync_NegativeDurationTime_ReturnsErrorResponse()
    {
        // Arrange

        // Act
        var result = await _providerService.AddServiceAsync(_testUser.Id, 1, "Test Service",
            new string[] { "Description 1", "Description 2" }, 100m, -10);

        // Assert
        Assert.Equal(400, result.Header.StatusCode);
    }

    [Fact]
    public async Task GetServicesByCategoryAsync_ReturnsResponseModelCore_WhenCalled()
    {
        // Arrange
        int categoryId = 1;
        int pageNumber = 1;
        int pageSize = 10;

        // Act
        var result = await _providerService.GetServicesByCategoryAsync(categoryId, pageNumber, pageSize);

        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        Assert.IsType<ResponseModelCore>(result);
    }

    [Fact]
    public async Task RemoveServiceAsync_UserDoesNotHavePermission_ReturnsDefaultResponse()
    {
        // Arrange
        var serviceId = 1;
        var userId = Guid.NewGuid();

        // Act
        var result = await _providerService.RemoveServiceAsync(serviceId, userId);

        // Assert
        Assert.IsType<ResponseModelCore>(result);
        Assert.Equal(404, result.Header.StatusCode);
    }

    [Fact]
    public async Task RemoveServiceAsync_ReturnsDefaultResponse_WhenUserIdIsInvalid()
    {
        // Arrange

        // Act
        var result = await _providerService.RemoveServiceAsync(1, Guid.Empty);

        // Assert
        Assert.IsType<ResponseModelCore>(result);
        Assert.Equal(404, result.Header.StatusCode);
        Assert.Equal("Не удалось удалить услугу. Услуга не найдена или не принадлежит пользователю",
            result.Header.Error);
    }

    [Fact]
    public async Task RemoveServiceAsync_ReturnsValidResponse_WhenServiceIdAndUserIdAreValid()
    {
        // Arrange
        var expectedResponse = new ResponseModelCore
        {
            Header = new ResponseHeaderCore
            {
                StatusCode = 404, Error = "Не удалось удалить услугу. Услуга не найдена или не принадлежит пользователю"
            }
        };

        // Act
        var result = await _providerService.RemoveServiceAsync(1, Guid.NewGuid());

        // Assert
        Assert.Equal(expectedResponse.Header.StatusCode, result.Header.StatusCode);
        Assert.Equal(expectedResponse.Header.Error, result.Header.Error);
    }

    [Fact]
    public async Task RemoveServiceAsync_ValidData_ReturnsSuccessResponse()
    {
        // Arrange
        await _providerService.AddServiceAsync(_testUser.Id, 1, "Test Service",
            new string[] { "Description 1", "Description 2" }, 100m, 20);
        var res = await _providerService.GetAllServicesByUserIdAsync(_testUser.Id, 0, 0);
        // Act
        var result =
            await _providerService.RemoveServiceAsync(res.Body.GetServices.Services.Single().ServiceId, _testUser.Id);

        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        Assert.Equal("Услуга успешно удалена", result.Body.Message);
    }
}