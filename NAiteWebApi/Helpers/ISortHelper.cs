namespace NAiteWebApi.Helpers
{
    public interface ISortHelper<T>
    {
        IQueryable<T> ApplySort(IQueryable<T> entites, string orderByQueryString);
    }
}
