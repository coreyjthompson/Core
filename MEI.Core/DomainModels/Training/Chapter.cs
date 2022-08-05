using System;
using System.Collections.Generic;

namespace MEI.Core.DomainModels.Training
{
    public partial class Chapter
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int Sequence { get; set; }
        public string ContentType { get; set; }
        public string Name { get; set; }
        public int? VideoId { get; set; }

        public int? KnowledgeCheckId { get; set; }

        public virtual IList<Slide> Slides { get; set; } = new List<Slide>();

        public virtual Video Video { get; set; } = new Video();

        public virtual Module Module { get; set; } = new Module();

        //public virtual KnowledgeCheck KnowledgeCheck { get; set; }
    }
}
