using System.Text;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NailStore.Application;
using NailStore.Application.Settings;
using NailStore.Core.Models;
using NailStore.Data.Models;

namespace NailStore.xUnit.Tests;

public class UserServiceTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly UserService _userService;
    private readonly Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;
    private readonly Mock<SignInManager<UserEntity>> _signInManagerMock;
    private readonly IOptions<SrvSettings> _srvSettingsMock;
    private readonly Mock<EmailService> _emailServiceMock;
    private readonly Mock<JWTManager> _jwtManagerMock;
    private readonly UserEntity _userEntity;
    private readonly Mock<SmtpClient> _smtpClientMock;


    public UserServiceTests()
    {
        var srv = GetSrvSettingsFromJSON();
        _userEntity = new UserEntity
        {
            Id = Guid.Parse("2f118637-42b3-4fcb-b041-88b23eaaa274"), UserName = "Pavel", Email = "dev@il2-expert.ru",
            NormalizedEmail = "dev@il2-expert.ru".ToUpper(), EmailConfirmed = true,
            SecurityStamp = "f31db32f-8714-451e-b029-99e0819fccea",
        };
        _userManagerMock = new Mock<UserManager<UserEntity>>(
            new Mock<IUserStore<UserEntity>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<UserEntity>>().Object,
            new[] { new Mock<IUserValidator<UserEntity>>().Object },
            new[] { new Mock<IPasswordValidator<UserEntity>>().Object },
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<UserEntity>>>().Object
        );
        _roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
            new Mock<IRoleStore<IdentityRole<Guid>>>().Object, Array.Empty<IRoleValidator<IdentityRole<Guid>>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole<Guid>>>>().Object);

        _signInManagerMock = new Mock<SignInManager<UserEntity>>(
            _userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<UserEntity>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<UserEntity>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<UserEntity>>().Object
        );
        _srvSettingsMock = Options.Create(srv!);
        _smtpClientMock = new Mock<SmtpClient>();
        _emailServiceMock = new Mock<EmailService>(
            _srvSettingsMock,
            new Mock<ILogger<EmailService>>().Object,
            _smtpClientMock.Object
        );
        _jwtManagerMock = new Mock<JWTManager>(
            new Mock<ILogger<JWTManager>>().Object,
            _srvSettingsMock,
            _userManagerMock.Object
        );
        var logger = new Mock<ILogger<UserService>>().Object;
        _userService = new Mock<UserService>(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _signInManagerMock.Object,
            _jwtManagerMock.Object,
            _emailServiceMock.Object,
            logger).Object;
    }

    [Fact]
    /// <summary>
    /// Confirms the email of a user.
    /// </summary>
    /// <param name="userConfirmited">The user's ID and the confirmation code.</param>
    /// <returns>An asynchronous task that represents the confirmation process.</returns>
    /// <remarks>
    /// This function sets up the necessary mocks for the user manager and calls the 
    /// <see cref="UserService.ConfirmedEmailUser(UserConfirmitedEmail)"/> method.
    /// It then asserts the expected results.
    /// </remarks>
    public async Task ConfirmedEmailUser_Success()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByIdAsync(_userEntity.Id.ToString())).ReturnsAsync(_userEntity);
        _userManagerMock.Setup(x => x.ConfirmEmailAsync(_userEntity, It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var userConfirmited = new UserConfirmitedEmail(_userEntity.Id.ToString(),
            WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("testCode")));

        // Act
        var result = await _userService.ConfirmedEmailUser(userConfirmited);

        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        Assert.Equal(string.Empty, result.Header.Error);
        Assert.Equal("Спасибо, что подтвердили свой адрес электронной почты.", result.Result);
    }

    [Fact]
    public async Task GetUserRolesAsync_UserFound_ReturnsRoles()
    {
        // Arrange
        var roles = new[] { "User" };

        _userManagerMock
            .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_userEntity);

        _userManagerMock
            .Setup(x => x.GetRolesAsync(_userEntity))
            .ReturnsAsync(roles);

        // Act
        var result = await _userService.GetUserRolesAsync("User");

        // Assert
        Assert.Equal(roles, result);
    }

    [Fact]
    public async Task GetUserRolesAsync_UserFoundButNoRoles_ReturnsEmptyArray()
    {
        // Arrange
        var user = _userEntity;

        _userManagerMock
            .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        var result = await _userService.GetUserRolesAsync("User");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task LoginUserAsync_Success()
    {
        // Arrange
        var roles = new[] { "User" };
        var password = "Password123";
        _userManagerMock.Setup(x => x.FindByEmailAsync(_userEntity.Email!)).ReturnsAsync(_userEntity);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(_userEntity, password, true))
            .ReturnsAsync(SignInResult.Success);
        _userManagerMock.Setup(x => x.GetRolesAsync(_userEntity)).ReturnsAsync(roles);

        // Act
        var result = await _userService.LoginUserAsync(_userEntity.Email!, password);


        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        Assert.NotNull(result.Result);
    }

    [Fact]
    public async Task RegisterUserAsync_Success()
    {
        // Arrange
        const string url = "http://example.com/confirm/";
        const string userName = "testuser";
        const string email = "test@example.com";
        const string password = "Test123!";
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync("testtoken");
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.RegisterUserAsync(url, userName, email, password);

        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        Assert.Equal(
            "Регистрация прошла успешно! На электронную почту выслано письмо, для завершения регистрации выполните инструкции в отправленном письме. Спасибо!",
            result.Result);
    }

    [Fact]
    public async Task GetUserByIdAsync_UserExists_ReturnsUserWithCorrectProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "TestUser";
        var registerAt = DateTime.UtcNow;
        var phoneNumber = "+1234567890";
        var enable = true;

        var user = new UserEntity
        {
            Id = userId,
            UserName = userName,
            RegisterAt = registerAt,
            PhoneNumber = phoneNumber,
            Enable = enable
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        ;
    }

    [Fact]
    public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsNotFoundResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync((UserEntity)null!);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.Equal($"Пользователь с идентификатором '{userId}' не найден.", result.Header.Error);
        Assert.Equal(404, result.Header.StatusCode);
    }

    [Fact]
    public async Task UserNameIsFreeAsync_TooShortUsername_ReturnsFalse()
    {
        // Arrange
        var username = "A";

        // Act
        var result = await _userService.UserNameIsFreeAsync(username);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UserNameIsFreeAsync_UsernameTaken_ReturnsFalse()
    {
        // Arrange
        var username = "ExistingUser";
        var user = new UserEntity { UserName = username };
        _userManagerMock.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);

        // Act
        var result = await _userService.UserNameIsFreeAsync(username);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UserNameIsFreeAsync_UsernameFree_ReturnsTrue()
    {
        // Arrange
        var username = "NewUser";
        _userManagerMock.Setup(x => x.FindByNameAsync(username)).ReturnsAsync((UserEntity)null!);

        // Act
        var result = await _userService.UserNameIsFreeAsync(username);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UserNameIsFreeAsync_UsernameNull_ReturnsFalse()
    {
        // Arrange
        string username = null!;

        // Act
        var result = await _userService.UserNameIsFreeAsync(username!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UserNameIsFreeAsync_UsernameEmpty_ReturnsFalse()
    {
        // Arrange
        var username = "";

        // Act
        var result = await _userService.UserNameIsFreeAsync(username);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RecoveryPasswordSend_UserExists_EmailSentSuccessfully()
    {
        // Arrange
        var url = "https://example.com/recovery/";
        _userManagerMock.Setup(x => x.FindByNameAsync(_userEntity.UserName!)).ReturnsAsync(_userEntity);

        // Act
        var result = await _userService.RecoveryPasswordSend(_userEntity.Email!, url);

        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        Assert.Equal("На указанную Вами почту отправлены инструкции для восстановления пароля", result.Result);
    }

    [Fact]
    public async Task RecoveryPasswordSend_UserDoesNotExist()
    {
        // Arrange
        var email = "test@example.com";
        var url = "https://example.com/recovery/";
        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((UserEntity)null!);

        // Act
        var result = await _userService.RecoveryPasswordSend(email, url);

        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        Assert.Equal("На указанную Вами почту отправлены инструкции для восстановления пароля", result.Result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task RecoveryPasswordSend_EmailIsNullOrEmpty(string email)
    {
        // Arrange
        var url = "https://example.com/recovery/";

        // Act
        var result = await _userService.RecoveryPasswordSend(email, url);

        // Assert
        Assert.Equal(400, result.Header.StatusCode);
        Assert.Equal("Восстановление пароля невозможно. Reason: Email не может быть пустым или равным null",
            result.Header.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task RecoveryPasswordSend_UrlIsNullOrEmpty(string url)
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(_userEntity.Email!)).ReturnsAsync(_userEntity);

        // Act
        var result = await _userService.RecoveryPasswordSend(_userEntity.Email!, url);

        // Assert
        Assert.Equal(400, result.Header.StatusCode);
        Assert.Equal("Восстановление пароля невозможно. Reason: URL не может быть пустым или равным null",
            result.Header.Error);
    }

    [Fact]
    public async Task RecoveryPassword_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var inputCode = "ascv2345";
        var newPass = "NewSecurePassword123!";
        _userManagerMock.Setup(x => x.FindByIdAsync(_userEntity.Id.ToString())).ReturnsAsync(_userEntity);
        _userManagerMock.Setup(x => x.ResetPasswordAsync(_userEntity, inputCode, newPass))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.RecoveryPassword(_userEntity.Id.ToString(), inputCode, newPass);

        // Assert
        Assert.Equal(200, result.Header.StatusCode);
        Assert.Equal("Пароль успешно изменен", result.Result);
    }

    [Fact]
    public async Task RecoveryPassword_InvalidUserId_ReturnsNotFound()
    {
        // Arrange
        var inputCode = "abc123";
        var newPass = "NewSecurePassword123!";
        _userManagerMock.Setup(x => x.FindByIdAsync(_userEntity.Id.ToString())).ReturnsAsync((UserEntity)null!);

        // Act
        var result = await _userService.RecoveryPassword(_userEntity.Id.ToString(), inputCode, newPass);

        // Assert
        Assert.Equal(404, result.Header.StatusCode);
        Assert.Equal($"Не удалось получить пользователя по его идентификатору: '{_userEntity.Id.ToString()}'.",
            result.Header.Error);
    }

    [Fact]
    public async Task RecoveryPassword_ResetPasswordError_ReturnsInternalServerError()
    {
        // Arrange
        var inputCode = "abc123";
        var newPass = "NewSecurePassword123!";
        _userManagerMock.Setup(x => x.FindByIdAsync(_userEntity.Id.ToString())).ReturnsAsync(_userEntity);
        _userManagerMock.Setup(x => x.ResetPasswordAsync(_userEntity, inputCode, newPass))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Reset password failed" }));

        // Act
        var result = await _userService.RecoveryPassword(_userEntity.Id.ToString(), inputCode, newPass);

        // Assert
        Assert.Equal(500, result.Header.StatusCode);
        Assert.Equal("Не удалось сменить пароль. Reason: Reset password failed", result.Header.Error);
        Assert.Equal("Не удалось сменить пароль. Reason: Reset password failed", result.Result);
    }

    [Fact]
    public async Task RecoveryPassword_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var inputCode = "abc123";
        var newPass = "NewSecurePassword123!";
        _userManagerMock.Setup(x => x.FindByIdAsync(_userEntity.Id.ToString()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _userService.RecoveryPassword(_userEntity.Id.ToString(), inputCode, newPass);

        // Assert
        Assert.Equal(500, result.Header.StatusCode);
        Assert.Equal("Не удалось сменить пароль.", result.Header.Error);
        Assert.Equal("Не удалось сменить пароль.", result.Result);
    }

    [Fact]
    public async Task IsRolesAllowedAsync_UserExistsAndHasRoles_ReturnsTrue()
    {
        // Arrange
        var inputRoles = new List<string> { "Admin", "User" };
        _userManagerMock.Setup(x => x.FindByIdAsync(_userEntity.Id.ToString())).ReturnsAsync(_userEntity);
        _userManagerMock.Setup(x => x.GetRolesAsync(_userEntity)).ReturnsAsync(new List<string> { "Admin", "User" });

        // Act
        var result = await _userService.IsRolesAllowedAsync(_userEntity.Id.ToString(), inputRoles);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsRolesAllowedAsync_UserExistsButDoesNotHaveRoles_ReturnsFalse()
    {
        // Arrange
        var inputRoles = new List<string> { "Admin", "User" };
        _userManagerMock.Setup(x => x.FindByIdAsync(_userEntity.Id.ToString())).ReturnsAsync(_userEntity);
        _userManagerMock.Setup(x => x.GetRolesAsync(_userEntity)).ReturnsAsync(new List<string> { "Master" });


        // Act
        var result = await _userService.IsRolesAllowedAsync(_userEntity.Id.ToString(), inputRoles);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRolesAllowedAsync_UserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var inputRoles = new List<string> { "Admin", "User" };
        _userManagerMock.Setup(x => x.FindByIdAsync(_userEntity.Id.ToString())).ReturnsAsync((UserEntity)null!);

        // Act
        var result = await _userService.IsRolesAllowedAsync(_userEntity.Id.ToString(), inputRoles);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRolesAllowedAsync_UserIdIsNull_ReturnsFalse()
    {
        // Arrange
        string userId = null!;
        var inputRoles = new List<string> { "Admin", "User" };

        // Act
        var result = await _userService.IsRolesAllowedAsync(userId, inputRoles);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRolesAllowedAsync_InputRolesIsEmpty_ReturnsFalse()
    {
        // Arrange
        var inputRoles = new List<string>();
        _userManagerMock.Setup(x => x.FindByIdAsync(_userEntity.Id.ToString())).ReturnsAsync(_userEntity);
        _userManagerMock.Setup(x => x.GetRolesAsync(_userEntity)).ReturnsAsync(new List<string> { "Admin", "User" });

        // Act
        var result = await _userService.IsRolesAllowedAsync(_userEntity.Id.ToString(), inputRoles);

        // Assert
        Assert.False(result);
    }

    private SrvSettings? GetSrvSettingsFromJSON()
    {
        try
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            var srvset = new SrvSettings();
            MyConfig.GetSection("SrvSettings").Bind(srvset);
            return srvset;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}