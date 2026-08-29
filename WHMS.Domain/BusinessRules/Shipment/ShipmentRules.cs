using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.Shipment;

public static class ShipmentRules
{
    public static WHMS.Domain.Entities.Shipment EnsureExists(WHMS.Domain.Entities.Shipment? shipment)
    {
        if (shipment is null)
            throw new NotFoundException("Shipment not found.");
        return shipment;
    }

    // Combines the null-check with the warehouse-ownership check used by SendShipment.
    public static WHMS.Domain.Entities.Shipment EnsureAccessibleForSending(WHMS.Domain.Entities.Shipment? shipment, Guid currentUserWarehouseId)
    {
        if (shipment is null || shipment.WarehouseId != currentUserWarehouseId)
            throw new NotFoundException("Shipment not found.");
        return shipment;
    }

    public static void EnsureNotSent(bool alreadySent)
    {
        if (alreadySent)
            throw new BusinessRuleViolationException("Shipment has already been sent.");
    }

    // Preserves the "alread" typo already present in CreateShipmentItem/UpdateShipmentItem's
    // original message - documenting current behavior, not fixing it.
    public static void EnsureNotSentForItemChange(bool alreadySent)
    {
        if (alreadySent)
            throw new BusinessRuleViolationException("Shipment has alread been sent.");
    }

    // Preserves SendShipment's own distinct phrasing.
    public static void EnsureNotAlreadySent(bool alreadySent)
    {
        if (alreadySent)
            throw new BusinessRuleViolationException("Shipment already sent.");
    }

    public static ShipmentItem EnsureItemExists(ShipmentItem? shipmentItem)
    {
        if (shipmentItem is null)
            throw new NotFoundException("Shipment item not found.");
        return shipmentItem;
    }

    // Cross-aggregate: current stock plus quantity already incoming via pending
    // deliveries, minus quantity already committed to other scheduled shipments,
    // both filtered to before the shipment's expected sending date.
    public static int CalculateExpectedStock(int currentStock, int incomingDeliveryQuantity, int outgoingShipmentQuantity)
        => currentStock + incomingDeliveryQuantity - outgoingShipmentQuantity;

    public static void EnsureSufficientProjectedStock(int expectedStock, int requestedQuantity)
    {
        if (expectedStock < requestedQuantity)
            throw new BusinessRuleViolationException("Insufficient stock in warehouse for the scheduled shipment date.");
    }

    // Non-throwing: returns a warning message if stock update affected no rows, or
    // null otherwise. SendShipment collects these into a response instead of failing
    // the whole request, consistent with WarehouseRules' Validate* methods.
    public static string? ValidateStockForSending(int affectedRows, string skuName)
    {
        if (affectedRows == 0)
            return $"Insufficient stock for {skuName}.";
        return null;
    }
}
