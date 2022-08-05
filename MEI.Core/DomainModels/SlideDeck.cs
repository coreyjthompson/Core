using System.Collections.Generic;

using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Training;

namespace MEI.Core.DomainModels
{
    public class SlideDeck
        : AuditModelBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VersionNumber { get; set; }

        public string FileName { get; set; }

        public IList<Module> TrainingModules { get; set; }

        //public virtual ICollection<TrainingProgramSlidekits> TrainingProgramSlidekits { get; set; }

    }
}