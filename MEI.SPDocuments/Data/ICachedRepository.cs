namespace MEI.SPDocuments.Data
{
    internal interface ICachedRepository
        : IRepository
    {
        ICacheProvider CacheProvider { get; }
    }
}
