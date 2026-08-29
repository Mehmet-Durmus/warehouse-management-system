using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.Delivery;

public static class DeliveryRules
{
    public static WHMS.Domain.Entities.Delivery EnsureExists(WHMS.Domain.Entities.Delivery? delivery)
    {
        if (delivery is null)
            throw new NotFoundException("Delivery not found.");
        return delivery;
    }

    // Combines the null-check with the warehouse-ownership check used by ReceiveDelivery,
    // preserving its exact (period-less) original message.
    public static WHMS.Domain.Entities.Delivery EnsureAccessibleForReceipt(WHMS.Domain.Entities.Delivery? delivery, Guid currentUserWarehouseId)
    {
        if (delivery is null || delivery.WarehouseId != currentUserWarehouseId)
            throw new NotFoundException("Delivery not found");
        return delivery;
    }

    public static void EnsureNotReceived(bool alreadyReceived)
    {
        if (alreadyReceived)
            throw new BusinessRuleViolationException("Delivery has already been received.");
    }

    // Preserves ReceiveDelivery's exact (period-less) original message.
    public static void EnsureNotAlreadyReceived(bool alreadyReceived)
    {
        if (alreadyReceived)
            throw new BusinessRuleViolationException("Delivery already received");
    }

    public static DeliveryItem EnsureItemExists(DeliveryItem? deliveryItem)
    {
        if (deliveryItem is null)
            throw new NotFoundException("Delivery item not found.");
        return deliveryItem;
    }
}
