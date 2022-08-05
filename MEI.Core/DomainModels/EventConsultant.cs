using NodaTime;

namespace MEI.Core.DomainModels
{
    public class EventConsultant
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; }

        public int ConsultantId { get; set; }

        public virtual Event Event { get; set; }

        public virtual Consultant Consultant { get; set; }

        //public override string ToString()
        //{
        //    return string.Format("[Id={0}, FirstName={1}, MiddleName={2}, LastName={3}");
        //}
    }
}
