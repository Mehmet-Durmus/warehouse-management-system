namespace WHMS.Domain.Entities;

public class Warehouse
{
    public Guid Id { get; set; }
    public List<ApplicationUser> ApplicationUsers { get; set; } = [];
}