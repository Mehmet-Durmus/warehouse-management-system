using FluentValidation.TestHelper;
using WHMS.Application.Features.Command.Login;
using WHMS.Application.Validators.Auth;

namespace WHMS.Tests.UnitTests.Application.Auth;

public class LoginCommandValidatorTests
{
   private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
       _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Validate_UserNameEmpty_ThrowsValidationException()
    {
      // Arrange
      var command = new LoginCommandRequest { UserName = "", Password = "1234"};

      // Act
      var result = _validator.TestValidate(command);

      // Assert
      result.ShouldHaveValidationErrorFor(x => x.UserName);
    }
    
    [Fact]
    public void Validate_UserNameNull_ThrowsValidationException()
    {
      // Arrange
      var command = new LoginCommandRequest { UserName = null, Password = "1234"};

      // Act
      var result = _validator.TestValidate(command);

      // Assert
      result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void Validate_PasswordEmpty_ThrowsValidationException()
    {
      // Arrange
      var command = new LoginCommandRequest { UserName = "user", Password = ""};

      // Act
      var result = _validator.TestValidate(command);

      // Assert
      result.ShouldHaveValidationErrorFor(x => x.Password);
    }
    
    [Fact]
    public void Validate_PasswordNull_ThrowsValidationException()
    {
      // Arrange
      var command = new LoginCommandRequest { UserName = "user", Password = null};

      // Act
      var result = _validator.TestValidate(command);

      // Assert
      result.ShouldHaveValidationErrorFor(x => x.Password);
    }

  [Fact]
  public void Validate_ValidCredantials_NotException()
  {
    // Arrange
    var command = new LoginCommandRequest { UserName = "user", Password = "1234"};

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }
}
