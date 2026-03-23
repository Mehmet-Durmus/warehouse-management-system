namespace WHMS.Application.Common.Filtering;

public abstract class QueryFilter
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
}