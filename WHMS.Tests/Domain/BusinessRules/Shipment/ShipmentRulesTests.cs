using WHMS.Domain.BusinessRules.Shipment;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Domain.BusinessRules.Shipment;

public class ShipmentRulesTests
{
    [Fact]
    public void EnsureExists_ShipmentIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => ShipmentRules.EnsureExists(null));

        Assert.Equal("Shipment not found.", exception.Message);
    }

    [Fact]
    public void EnsureExists_ShipmentIsNotNull_ReturnsTheSameShipment()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { WarehouseId = Guid.NewGuid() };

        var result = ShipmentRules.EnsureExists(shipment);

        Assert.Same(shipment, result);
    }

    [Fact]
    public void EnsureAccessibleForSending_ShipmentIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => ShipmentRules.EnsureAccessibleForSending(null, Guid.NewGuid()));

        Assert.Equal("Shipment not found.", exception.Message);
    }

    [Fact]
    public void EnsureAccessibleForSending_DifferentWarehouse_ThrowsNotFoundException()
    {
        var warehouseId = Guid.NewGuid();
        var shipment = new WHMS.Domain.Entities.Shipment { WarehouseId = Guid.NewGuid() };

        var exception = Assert.Throws<NotFoundException>(() => ShipmentRules.EnsureAccessibleForSending(shipment, warehouseId));

        Assert.Equal("Shipment not found.", exception.Message);
    }

    [Fact]
    public void EnsureAccessibleForSending_SameWarehouse_ReturnsTheSameShipment()
    {
        var warehouseId = Guid.NewGuid();
        var shipment = new WHMS.Domain.Entities.Shipment { WarehouseId = warehouseId };

        var result = ShipmentRules.EnsureAccessibleForSending(shipment, warehouseId);

        Assert.Same(shipment, result);
    }

    [Fact]
    public void EnsureNotSent_AlreadySent_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => ShipmentRules.EnsureNotSent(alreadySent: true));

        Assert.Equal("Shipment has already been sent.", exception.Message);
    }

    [Fact]
    public void EnsureNotSent_NotSent_DoesNotThrow()
    {
        var exception = Record.Exception(() => ShipmentRules.EnsureNotSent(alreadySent: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureNotSentForItemChange_AlreadySent_ThrowsBusinessRuleViolationExceptionWithTypoPreserved()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => ShipmentRules.EnsureNotSentForItemChange(alreadySent: true));

        Assert.Equal("Shipment has alread been sent.", exception.Message);
    }

    [Fact]
    public void EnsureNotSentForItemChange_NotSent_DoesNotThrow()
    {
        var exception = Record.Exception(() => ShipmentRules.EnsureNotSentForItemChange(alreadySent: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureNotAlreadySent_AlreadySent_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => ShipmentRules.EnsureNotAlreadySent(alreadySent: true));

        Assert.Equal("Shipment already sent.", exception.Message);
    }

    [Fact]
    public void EnsureNotAlreadySent_NotSent_DoesNotThrow()
    {
        var exception = Record.Exception(() => ShipmentRules.EnsureNotAlreadySent(alreadySent: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureItemExists_ItemIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => ShipmentRules.EnsureItemExists(null));

        Assert.Equal("Shipment item not found.", exception.Message);
    }

    [Fact]
    public void EnsureItemExists_ItemIsNotNull_ReturnsTheSameItem()
    {
        var item = new WHMS.Domain.Entities.ShipmentItem { ShipmentId = Guid.NewGuid(), SkuId = Guid.NewGuid() };

        var result = ShipmentRules.EnsureItemExists(item);

        Assert.Same(item, result);
    }

    [Theory]
    [InlineData(10, 5, 3, 12)]
    [InlineData(0, 0, 0, 0)]
    [InlineData(10, 0, 5, 5)]
    public void CalculateExpectedStock_ReturnsCurrentPlusIncomingMinusOutgoing(int currentStock, int incoming, int outgoing, int expected)
    {
        var result = ShipmentRules.CalculateExpectedStock(currentStock, incoming, outgoing);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void EnsureSufficientProjectedStock_RequestedExceedsExpected_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => ShipmentRules.EnsureSufficientProjectedStock(expectedStock: 12, requestedQuantity: 13));

        Assert.Equal("Insufficient stock in warehouse for the scheduled shipment date.", exception.Message);
    }

    [Fact]
    public void EnsureSufficientProjectedStock_RequestedEqualsExpected_DoesNotThrow()
    {
        var exception = Record.Exception(() => ShipmentRules.EnsureSufficientProjectedStock(expectedStock: 12, requestedQuantity: 12));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateStockForSending_NoRowsAffected_ReturnsWarning()
    {
        var warning = ShipmentRules.ValidateStockForSending(affectedRows: 0, skuName: "kola");

        Assert.Equal("Insufficient stock for kola.", warning);
    }

    [Fact]
    public void ValidateStockForSending_RowsAffected_ReturnsNull()
    {
        var warning = ShipmentRules.ValidateStockForSending(affectedRows: 1, skuName: "kola");

        Assert.Null(warning);
    }
}
