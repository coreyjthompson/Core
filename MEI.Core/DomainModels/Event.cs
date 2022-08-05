using System;

using NodaTime;

namespace MEI.Core.DomainModels
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public LocalDateTime WhenTakesPlace { get; set; }

        public string TimeZoneAbbreviation { get; set; }

        public override string ToString()
        {
            return string.Format("[Id={0}, Name={1}, WhenTakesPlace={2}]", Id, Name, WhenTakesPlace);
        }

        public int GetYearFromName()
        {
            return GetYearFromName(Name);
        }

        public static int GetYearFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Event name");
            }

            if (name.Length < 2)
            {
                throw new ArgumentException("Event name has an incorrect length. " + name);
            }

            string lastTwo = name.Substring(name.Length - 2, 2);
            int lastTwoYear;

            if (!int.TryParse(lastTwo, out lastTwoYear))
            {
                throw new ArgumentException("Event name has incorrect format. " + name);
            }

            return 2000 + lastTwoYear;

        }

    }
}
