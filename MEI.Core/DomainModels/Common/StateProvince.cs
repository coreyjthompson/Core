﻿using System;

namespace MEI.Core.DomainModels.Common
{
    public class StateProvince
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}
