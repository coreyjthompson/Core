using System;
using System.Collections.Generic;

namespace MEI.Core.DomainModels.Training
{
    public partial class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MediaType { get; set; }
        public int? LengthInSeconds { get; set; }
        public DateTimeOffset? WhenInactivated { get; set; }
        public string Path { get; set; }
    }
}
