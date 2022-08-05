using System.Collections.Generic;

using MEI.Core.DomainModels.Common;

namespace MEI.Core.DomainModels.Training
{
    public class Module
        : AuditModelBase
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string Title { get; set; }

        public int SlideDeckId { get; set; }
        public virtual SlideDeck SlideDeck { get; set; }

        public virtual IList<Chapter> Chapters { get; set; }

        //public virtual ICollection<SpeakerTrainingKnowledgeCheckComplete> SpeakerTrainingKnowledgeCheckComplete { get; set; }
    }
}