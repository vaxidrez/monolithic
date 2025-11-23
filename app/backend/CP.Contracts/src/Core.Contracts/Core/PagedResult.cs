namespace CP.Core.Contracts.Core;

public class PagedResult<T> : Result<T>
{
    public PagedInfo PagedInfo { get; }

    public PagedResult(PagedInfo pagedInfo, T value)
        : base(value)
    {
        PagedInfo = pagedInfo;
    }
}
