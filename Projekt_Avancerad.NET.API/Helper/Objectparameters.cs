public class ObjectParameters
{
    public int PageNumber { get; set; } = 1;
    const int maxPageSize = 10;
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set  => _pageSize = (value > maxPageSize) ? maxPageSize : value;
    }
}