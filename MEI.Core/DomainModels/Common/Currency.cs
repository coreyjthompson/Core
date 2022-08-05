using System;

namespace MEI.Core.DomainModels.Common
{
    public class Currency
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public string IsoSymbol { get; set; }
		public string Symbol { get; set; }
		public string RegionName { get; set; }
    }
}
