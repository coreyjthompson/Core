using System;
using System.Collections.Generic;

namespace MEI.Core.DomainModels.Training
{
    public partial class Slide
    {
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public int Sequence { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }

        public virtual Chapter Chapter { get; set; } = new Chapter();

        public virtual IList<SlideSpeakerNotes> SpeakerNotes { get; set; }
    }
}
