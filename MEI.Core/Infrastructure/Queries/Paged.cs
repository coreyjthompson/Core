using System.Runtime.Serialization;

namespace MEI.Core.Infrastructure.Queries
{
    [DataContract(Name = nameof(Paged<T>) + "Of{0}")]
    public class Paged<T>
    {
        [DataMember] public PageInfo Paging { get; set; }

        [DataMember] public T[] Items { get; set; }
    }
}