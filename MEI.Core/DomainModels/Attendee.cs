namespace MEI.Core.DomainModels
{
    public class Attendee
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EventName { get; set; }

        public int EventId { get; set; }

        public override string ToString()
        {
            return string.Format("[Id={0}, FirstName={1}, LastName={2}, EventName={3}, EventId={4}]", Id, FirstName, LastName, EventName, EventId);
        }
    }
}
