using WHMS.Domain.BusinessRules.Delivery;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Domain.BusinessRules.Delivery;

public class DeliveryRulesTests
{
    [Fact]
    public void EnsureExists_DeliveryIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => DeliveryRules.EnsureExists(null));

        Assert.Equal("Delivery not found.", exception.Message);
    }

    [Fact]
    public void EnsureExists_DeliveryIsNotNull_ReturnsTheSameDelivery()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { WarehouseId = Guid.NewGuid() };

        var result = DeliveryRules.EnsureExists(delivery);

        Assert.Same(delivery, result);
    }

    [Fact]
    public void EnsureAccessibleForReceipt_DeliveryIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => DeliveryRules.EnsureAccessibleForReceipt(null, Guid.NewGuid()));

        Assert.Equal("Delivery not found", exception.Message);
    }

    [Fact]
    public void EnsureAccessibleForReceipt_DifferentWarehouse_ThrowsNotFoundException()
    {
        var warehouseId = Guid.NewGuid();
        var delivery = new WHMS.Domain.Entities.Delivery { WarehouseId = Guid.NewGuid() };

        var exception = Assert.Throws<NotFoundException>(() => DeliveryRules.EnsureAccessibleForReceipt(delivery, warehouseId));

        Assert.Equal("Delivery not found", exception.Message);
    }

    [Fact]
    public void EnsureAccessibleForReceipt_SameWarehouse_ReturnsTheSameDelivery()
    {
        var warehouseId = Guid.NewGuid();
        var delivery = new WHMS.Domain.Entities.Delivery { WarehouseId = warehouseId };

        var result = DeliveryRules.EnsureAccessibleForReceipt(delivery, warehouseId);

        Assert.Same(delivery, result);
    }

    [Fact]
    public void EnsureNotReceived_AlreadyReceived_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => DeliveryRules.EnsureNotReceived(alreadyReceived: true));

        Assert.Equal("Delivery has already been received.", exception.Message);
    }

    [Fact]
    public void EnsureNotReceived_NotReceived_DoesNotThrow()
    {
        var exception = Record.Exception(() => DeliveryRules.EnsureNotReceived(alreadyReceived: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureNotAlreadyReceived_AlreadyReceived_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => DeliveryRules.EnsureNotAlreadyReceived(alreadyReceived: true));

        Assert.Equal("Delivery already received", exception.Message);
    }

    [Fact]
    public void EnsureNotAlreadyReceived_NotReceived_DoesNotThrow()
    {
        var exception = Record.Exception(() => DeliveryRules.EnsureNotAlreadyReceived(alreadyReceived: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureItemExists_ItemIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => DeliveryRules.EnsureItemExists(null));

        Assert.Equal("Delivery item not found.", exception.Message);
    }

    [Fact]
    public void EnsureItemExists_ItemIsNotNull_ReturnsTheSameItem()
    {
        var item = new WHMS.Domain.Entities.DeliveryItem { DeliveryId = Guid.NewGuid(), SkuId = Guid.NewGuid() };

        var result = DeliveryRules.EnsureItemExists(item);

        Assert.Same(item, result);
    }
}
