using NodaTime;

namespace MEI.Core.DomainModels
{
    public class Consultant
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string GetFullName()
        {
            var middleName = string.IsNullOrEmpty(MiddleName) ? string.Empty : $" {MiddleName}"; 
            return $"{FirstName}{middleName} {LastName}";
        }

        public override string ToString()
        {
            return string.Format("[Id={0}, FirstName={1}, MiddleName={2}, LastName={3}");
        }
    }
}
