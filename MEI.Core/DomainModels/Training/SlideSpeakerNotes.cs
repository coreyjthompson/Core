using System;
using System.Collections.Generic;

namespace MEI.Core.DomainModels.Training
{
    public partial class SlideSpeakerNotes
    {
        public int Id { get; set; }
        public string Contents { get; set; }
        public int SlideId { get; set; }

        public int SlideDeckId { get; set; }

        public DateTimeOffset? WhenInactivated { get; set; }

        public virtual Slide Slide { get; set; }

        public virtual SlideDeck SlideDeck { get; set; }
    }
}
