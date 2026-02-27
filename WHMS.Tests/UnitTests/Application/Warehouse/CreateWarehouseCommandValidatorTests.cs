using FluentValidation.TestHelper;
using WHMS.Application.Features.Command.Warehouse.CreateWarehouse;
using WHMS.Application.Validators.Warehose;

namespace WHMS.Tests.UnitTests.Application.Warehouse;

public class CreateWarehouseCommandValidatorTests
{
    private readonly CreateWarehouseCommandValidator _validator;

    public CreateWarehouseCommandValidatorTests()
    {
        _validator = new CreateWarehouseCommandValidator();
    }

    [Fact]
    public void Validate_CityIdEmpty_ThrowsException()
    {
        // Arrange
        var command = new CreateWarehouseCommandRequest
        {
            CityId = "",
            DistrictId = "district",
            NeighborhoodId = "neighborhood",
            AddressLine = "address",
            PostalCode = "code"
        };

        // Action
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CityId); 
    }

    [Fact]
    public void Validate_DistrictIdEmpty_ThrowsException()
    {
        // Arrange
        var command = new CreateWarehouseCommandRequest
        {
            CityId = "city",
            DistrictId = "",
            NeighborhoodId = "neighborhood",
            AddressLine = "address",
            PostalCode = "code"
        };

        // Action
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DistrictId); 
    }

    [Fact]
    public void Validate_NeighborhoodIdEmpty_ThrowsException()
    {
        // Arrange
        var command = new CreateWarehouseCommandRequest
        {
            CityId = "city",
            DistrictId = "district",
            NeighborhoodId = "",
            AddressLine = "address",
            PostalCode = "code"
        };

        // Action
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NeighborhoodId); 
    }

    [Fact]
    public void Validate_AddressLineEmpty_ThrowsException()
    {
        // Arrange
        var command = new CreateWarehouseCommandRequest
        {
            CityId = "city",
            DistrictId = "district",
            NeighborhoodId = "neighborhood",
            AddressLine = "",
            PostalCode = "code"
        };

        // Action
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AddressLine); 
    }


    [Fact]
    public void Validate_PostalCodeEmpty_ThrowsException()
    {
        // Arrange
        var command = new CreateWarehouseCommandRequest
        {
            CityId = "city",
            DistrictId = "district",
            NeighborhoodId = "neighborhood",
            AddressLine = "address",
            PostalCode = ""
        };

        // Action
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PostalCode); 
    }

    [Fact]
    public void Validate_ValidData_NoException()
    {
        // Arrange
        var command = new CreateWarehouseCommandRequest
        {
            CityId = "city",
            DistrictId = "district",
            NeighborhoodId = "neighborhood",
            AddressLine = "address",
            PostalCode = "code"
        };

        // Action
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors(); 
    }
}