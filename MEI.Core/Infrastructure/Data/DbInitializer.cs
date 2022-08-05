using System;
using System.Collections.Generic;
using System.Linq;
using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MEI.Core.Infrastructure.Data
{
    public static class DbContextExtensions
    {
        public static void Seed(this CoreContext context)
        {
            if (context.Clients.Any())
            {
                return;
            }

            var clients = new List<Client>
            {
                new Client
                {
                    Name = "AbbVie",
                    StreetAddressLine1 = "AbbVie Inc. Headquarters",
                    StreetAddressLine2 = "1 N. Waukegan Road",
                    CityTown = "North Chicago",
                    StateProvince = "IL",
                    Country = "US",
                    PostalCode = "60064",
                    TollFreePhoneNumber = "800-255-5162",
                    Initials = "AB",
                    SharePointCompanyCode = "LabDevelopment"
                    //SharePointCompanyCode = "Abbott"
                },
                new Client
                {
                    Name = "Exact Sciences",
                    StreetAddressLine1 = "Exact Sciences Corporation",
                    StreetAddressLine2 = "441 Charmany Drive",
                    CityTown = "Madison",
                    StateProvince = "WI",
                    Country = "US",
                    PostalCode = "53719",
                    TollFreePhoneNumber = "844-870-8870",
                    Initials = "ES",
                    SharePointCompanyCode = "LabDevelopment"
                    //SharePointCompanyCode = "ExactSciences"
                },
                new Client
                {
                    Name = "MEI",
                    StreetAddressLine1 = "Meetings & Events Intl.",
                    StreetAddressLine2 = "1314 Burch Drive",
                    CityTown = "Evansville",
                    StateProvince = "IN",
                    Country = "US",
                    PostalCode = "47725",
                    TollFreePhoneNumber = "555-555-5555",
                    Initials = "MEI",
                    SharePointCompanyCode = "LabDevelopment"
                },
            };

            foreach (var client in clients)
            {
                context.Clients.Add(client);
            }

            context.SaveChanges();

            var countries = new List<Country>
            {   
                new Country { Name = "Albania", IsoCode = "ALB" },
                new Country { Name = "Algeria", IsoCode = "DZA" },
                new Country { Name = "Angola", IsoCode = "AGO"},
                new Country { Name = "Argentina", IsoCode = "ARG" },
                new Country { Name = "Aruba", IsoCode = "ABW" },
                new Country { Name = "Australia", IsoCode = "AUS" },
                new Country { Name = "Austria", IsoCode = "AUT" },
                new Country { Name = "Bahamas", IsoCode = "BHS" },
                new Country { Name = "Bahrain", IsoCode = "BHR" },
                new Country { Name = "Barbados", IsoCode = "BRB" },
                new Country { Name = "Belarus", IsoCode = "BLR" },
                new Country { Name = "Belgium", IsoCode = "BEL" },
                new Country { Name = "Bermuda", IsoCode = "BMU" },
                new Country { Name = "Bosnia & Herzegovina", IsoCode = "BIH" },
                new Country { Name = "Botswana", IsoCode = "BWA" },
                new Country { Name = "Brazil", IsoCode = "BRA" },
                new Country { Name = "Bulgaria", IsoCode = "BGR" },
                new Country { Name = "Canada", IsoCode = "CAN" },
                new Country { Name = "Cape Verde", IsoCode = "CPV" },
                new Country { Name = "Cayman Islands", IsoCode = "CYM" },
                new Country { Name = "Chile", IsoCode = "CHL" },
                new Country { Name = "China", IsoCode = "CHN" },
                new Country { Name = "Colombia", IsoCode = "COL" },
                new Country { Name = "Costa Rica", IsoCode = "CRI" },
                new Country { Name = "Croatia", IsoCode = "HRV" },
                new Country { Name = "Curacao", IsoCode = "CUW" },
                new Country { Name = "Cyprus", IsoCode = "CYP" },
                new Country { Name = "Czech Republic", IsoCode = "CZE" },
                new Country { Name = "Denmark", IsoCode = "DNK" },
                new Country { Name = "Dominican Republic", IsoCode = "DOM" },
                new Country { Name = "Ecuador", IsoCode = "ECU" },
                new Country { Name = "Egypt", IsoCode = "EGY" },
                new Country { Name = "El Salvador", IsoCode = "SLV" },
                new Country { Name = "Equatorial Guinea", IsoCode = "GNQ" },
                new Country { Name = "Eritrea", IsoCode = "ERI" },
                new Country { Name = "Estonia", IsoCode = "EST" },
                new Country { Name = "Ethiopia", IsoCode = "ETH" },
                new Country { Name = "Finland", IsoCode = "FIN" },
                new Country { Name = "France", IsoCode = "FRA" },
                new Country { Name = "Gambia", IsoCode = "GMB" },
                new Country { Name = "Germany", IsoCode = "DEU" },
                new Country { Name = "Ghana", IsoCode = "GHA" },
                new Country { Name = "Greece", IsoCode = "GRC" },
                new Country { Name = "Guatemala", IsoCode = "GTM" },
                new Country { Name = "Guinea-Bissau", IsoCode = "GNB" },
                new Country { Name = "Honduras", IsoCode = "HND" },
                new Country { Name = "Hong Kong", IsoCode = "HKG" },
                new Country { Name = "Hungary", IsoCode = "HUN" },
                new Country { Name = "India", IsoCode = "IND" },
                new Country { Name = "Indonesia", IsoCode = "IDN" },
                new Country { Name = "Iran", IsoCode = "IRN" },
                new Country { Name = "Iraq", IsoCode = "IRQ" },
                new Country { Name = "Ireland", IsoCode = "IRL" },
                new Country { Name = "Israel", IsoCode = "ISR" },
                new Country { Name = "Italy", IsoCode = "ITA" },
                new Country { Name = "Jamaica", IsoCode = "JAM" },
                new Country { Name = "Japan", IsoCode = "JPN" },
                new Country { Name = "Jordan", IsoCode = "JOR" },
                new Country { Name = "Kazakhstan", IsoCode = "KAZ" },
                new Country { Name = "Kenya", IsoCode = "KEN" },
                new Country { Name = "Kuwait", IsoCode = "KWT" },
                new Country { Name = "Latvia", IsoCode = "LVA" },
                new Country { Name = "Lebanon", IsoCode = "LBN" },
                new Country { Name = "Lesotho", IsoCode = "LSO" },
                new Country { Name = "Liberia", IsoCode = "LBR" },
                new Country { Name = "Libya", IsoCode = "LBY" },
                new Country { Name = "Lithuania", IsoCode = "LTU" },
                new Country { Name = "Luxembourg", IsoCode = "LUX" },
                new Country { Name = "Malawi", IsoCode = "MWI" },
                new Country { Name = "Malaysia", IsoCode = "MYS" },
                new Country { Name = "Malta", IsoCode = "MLT" },
                new Country { Name = "Mauritius", IsoCode = "MUS" },
                new Country { Name = "Mexico", IsoCode = "MEX" },
                new Country { Name = "Morocco", IsoCode = "MAR" },
                new Country { Name = "Mozambique", IsoCode = "MOZ" },
                new Country { Name = "Namibia", IsoCode = "NAM" },
                new Country { Name = "Netherlands", IsoCode = "NLD" },
                new Country { Name = "New Zealand", IsoCode = "NZL" },
                new Country { Name = "Nicaragua", IsoCode = "NIC" },
                new Country { Name = "Nigeria", IsoCode = "NGA" },
                new Country { Name = "Norway", IsoCode = "NOR" },
                new Country { Name = "Oman", IsoCode = "OMN" },
                new Country { Name = "Pakistan", IsoCode = "PAK" },
                new Country { Name = "Panama", IsoCode = "PAN" },
                new Country { Name = "Peru", IsoCode = "PER" },
                new Country { Name = "Philippines", IsoCode = "PHL" },
                new Country { Name = "Poland", IsoCode = "POL" },
                new Country { Name = "Portugal", IsoCode = "PRT" },
                new Country { Name = "Qatar", IsoCode = "QAT" },
                new Country { Name = "Romania", IsoCode = "ROU" },
                new Country { Name = "Russia", IsoCode = "RUS" },
                new Country { Name = "Saudi Arabia", IsoCode = "SAU" },
                new Country { Name = "Senegal", IsoCode = "SEN" },
                new Country { Name = "Serbia", IsoCode = "SRB" },
                new Country { Name = "Sierra Leone", IsoCode = "SLE" },
                new Country { Name = "Singapore", IsoCode = "SGP" },
                new Country { Name = "Slovakia", IsoCode = "SVK" },
                new Country { Name = "Slovenia", IsoCode = "SVN" },
                new Country { Name = "Somalia", IsoCode = "SOM" },
                new Country { Name = "South Africa", IsoCode = "ZAF" },
                new Country { Name = "South Korea", IsoCode = "KOR" },
                new Country { Name = "Spain", IsoCode = "ESP" },
                new Country { Name = "Swaziland", IsoCode = "SWZ" },
                new Country { Name = "Sweden", IsoCode = "SWE" },
                new Country { Name = "Switzerland", IsoCode = "CHE" },
                new Country { Name = "Taiwan", IsoCode = "TWN" },
                new Country { Name = "Tanzania", IsoCode = "TZA" },
                new Country { Name = "Thailand", IsoCode = "THA" },
                new Country { Name = "Trinidad and Tobago", IsoCode = "TTO" },
                new Country { Name = "Tunisia", IsoCode = "TUN" },
                new Country { Name = "Turkey", IsoCode = "TUR" },
                new Country { Name = "UAE", IsoCode = "ARE" },
                new Country { Name = "Uganda", IsoCode = "UGA" },
                new Country { Name = "Ukraine", IsoCode = "UKR" },
                new Country { Name = "United Kingdom", IsoCode = "GBR" },
                new Country { Name = "United States", IsoCode = "USA" },
                new Country { Name = "Uruguay", IsoCode = "URY" },
                new Country { Name = "Venezuela", IsoCode = "VEN" },
                new Country { Name = "Vietnam", IsoCode = "VNM" },
                new Country { Name = "Western Sahara", IsoCode = "ESH" },
                new Country { Name = "Yemen", IsoCode = "YEM" },
                new Country { Name = "Zambia", IsoCode = "ZMB" }
            };

            foreach (var country in countries)
            {
                context.Countries.Add(country);
            }

            context.SaveChanges();


            //Currencies
            var currencies = new List<Currency>
            {
                new Currency { Name = "Lek", IsoSymbol = "ALL", Symbol = "Lek" },
                new Currency { Name = "Algerian Dinar", IsoSymbol = "DZD", Symbol = "د.ج.‏" },
                new Currency { Name = "Argentine Peso", IsoSymbol = "ARS", Symbol = "$" },
                new Currency { Name = "Australian Dollar", IsoSymbol = "AUD", Symbol = "$" },
                new Currency { Name = "Bahamian Dollar", IsoSymbol = "BSD", Symbol = null },
                new Currency { Name = "Bahraini Dinar", IsoSymbol = "BHD", Symbol = "د.ب.‏" },
                new Currency { Name = "Taka", IsoSymbol = "BDT", Symbol = "৳" },
                new Currency { Name = "Armenian Dram", IsoSymbol = "AMD", Symbol = "դր." },
                new Currency { Name = "Barbados Dollar", IsoSymbol = "BBD", Symbol = null },
                new Currency { Name = "Bermudian Dollar", IsoSymbol = "BMD", Symbol = null },
                new Currency { Name = "Boliviano", IsoSymbol = "BOB", Symbol = "$b" },
                new Currency { Name = "Pula", IsoSymbol = "BWP", Symbol = null },
                new Currency { Name = "Belize Dollar", IsoSymbol = "BZD", Symbol = "BZ$" },
                new Currency { Name = "Solomon Islands Dollar", IsoSymbol = "SBD" },
                new Currency { Name = "Brunei Dollar", IsoSymbol = "BND", Symbol = "$" },
                new Currency { Name = "Kyat", IsoSymbol = "MMK", Symbol = null },
                new Currency { Name = "Burundi Franc", IsoSymbol = "BIF", Symbol = null },
                new Currency { Name = "Riel", IsoSymbol = "KHR", Symbol = "៛" },
                new Currency { Name = "Canadian Dollar", IsoSymbol = "CAD", Symbol = "$" },
                new Currency { Name = "Cape Verde Escudo", IsoSymbol = "CVE", Symbol = null },
                new Currency { Name = "Cayman Islands Dollar", IsoSymbol = "KYD", Symbol = null },
                new Currency { Name = "Sri Lanka Rupee", IsoSymbol = "LKR", Symbol = "රු." },
                new Currency { Name = "Chilean Peso", IsoSymbol = "CLP", Symbol = "$" },
                new Currency { Name = "Yuan Renminbi", IsoSymbol = "CNY", Symbol = "¥" },
                new Currency { Name = "Colombian Peso", IsoSymbol = "COP", Symbol = "$" },
                new Currency { Name = "Comoro Franc", IsoSymbol = "KMF", Symbol = null },
                new Currency { Name = "Costa Rican Colon", IsoSymbol = "CRC", Symbol = "₡" },
                new Currency { Name = "Croatian Kuna", IsoSymbol = "HRK", Symbol = "kn" },
                new Currency { Name = "Cuban Peso", IsoSymbol = "CUP", Symbol = null },
                new Currency { Name = "Czech Koruna", IsoSymbol = "CZK", Symbol = "Kč" },
                new Currency { Name = "Danish Krone", IsoSymbol = "DKK", Symbol = "kr." },
                new Currency { Name = "Dominican Peso", IsoSymbol = "DOP", Symbol = "RD$" },
                new Currency { Name = "El Salvador Colon", IsoSymbol = "SVC", Symbol = null },
                new Currency { Name = "Ethiopian Birr", IsoSymbol = "ETB", Symbol = "ETB" },
                new Currency { Name = "Nakfa", IsoSymbol = "ERN", Symbol = null },
                new Currency { Name = "Kroon", IsoSymbol = "EEK", Symbol = null },
                new Currency { Name = "Falkland Islands Pound", IsoSymbol = "FKP", Symbol = null },
                new Currency { Name = "Fiji Dollar", IsoSymbol = "FJD", Symbol = null },
                new Currency { Name = "Djibouti Franc", IsoSymbol = "DJF", Symbol = null },
                new Currency { Name = "Dalasi", IsoSymbol = "GMD", Symbol = null },
                new Currency { Name = "Gibraltar Pound", IsoSymbol = "GIP", Symbol = null },
                new Currency { Name = "Quetzal", IsoSymbol = "GTQ", Symbol = "Q" },
                new Currency { Name = "Guinea Franc", IsoSymbol = "GNF", Symbol = null },
                new Currency { Name = "Guyana Dollar", IsoSymbol = "GYD", Symbol = null },
                new Currency { Name = "Gourde", IsoSymbol = "HTG", Symbol = null },
                new Currency { Name = "Lempira", IsoSymbol = "HNL", Symbol = "L." },
                new Currency { Name = "Hong Kong Dollar", IsoSymbol = "HKD", Symbol = "HK$" },
                new Currency { Name = "Forint", IsoSymbol = "HUF", Symbol = "Ft" },
                new Currency { Name = "Iceland Krona", IsoSymbol = "ISK", Symbol = "kr." },
                new Currency { Name = "Indian Rupee", IsoSymbol = "INR", Symbol = "₹" },
                new Currency { Name = "Rupiah", IsoSymbol = "IDR", Symbol = "Rp" },
                new Currency { Name = "Iranian Rial", IsoSymbol = "IRR", Symbol = "ريال" },
                new Currency { Name = "Iraqi Dinar", IsoSymbol = "IQD", Symbol = "د.ع.‏" },
                new Currency { Name = "New Israeli Sheqel", IsoSymbol = "ILS", Symbol = "₪" },
                new Currency { Name = "Jamaican Dollar", IsoSymbol = "JMD", Symbol = "J$" },
                new Currency { Name = "Yen", IsoSymbol = "JPY", Symbol = "¥" },
                new Currency { Name = "Tenge", IsoSymbol = "KZT", Symbol = "Т" },
                new Currency { Name = "Jordanian Dinar", IsoSymbol = "JOD", Symbol = "د.ا.‏"},
                new Currency { Name = "Kenyan Shilling", IsoSymbol = "KES", Symbol = "S" },
                new Currency { Name = "North Korean Won", IsoSymbol = "KPW", Symbol = null},
                new Currency { Name = "Won", IsoSymbol = "KRW", Symbol = "₩"},
                new Currency { Name = "Kuwaiti Dinar", IsoSymbol = "KWD", Symbol = "د.ك.‏" },
                new Currency { Name = "Som", IsoSymbol = "KGS", Symbol = "сом" },
                new Currency { Name = "Kip", IsoSymbol = "LAK", Symbol = "₭" },
                new Currency { Name = "Lebanese Pound", IsoSymbol = "LBP", Symbol = "ل.ل.‏" },
                new Currency { Name = "Latvian Lats", IsoSymbol = "LVL", Symbol = null },
                new Currency { Name = "Liberian Dollar", IsoSymbol = "LRD", Symbol = null },
                new Currency { Name = "Libyan Dinar", IsoSymbol = "LYD", Symbol = "د.ل.‏" },
                new Currency { Name = "Lithuanian Litas", IsoSymbol = "LTL", Symbol = null },
                new Currency { Name = "Pataca", IsoSymbol = "MOP", Symbol = "MOP" },
                new Currency { Name = "Kwacha", IsoSymbol = "MWK", Symbol = null },
                new Currency { Name = "Malaysian Ringgit", IsoSymbol = "MYR", Symbol = "RM" },
                new Currency { Name = "Rufiyaa", IsoSymbol = "MVR", Symbol = "ރ." },
                new Currency { Name = "Ouguiya", IsoSymbol = "MRO", Symbol = null },
                new Currency { Name = "Mauritius Rupee", IsoSymbol = "MUR", Symbol = null },
                new Currency { Name = "Mexican Peso", IsoSymbol = "MXN", Symbol = "$" },
                new Currency { Name = "Tugrik", IsoSymbol = "MNT", Symbol = "₮" },
                new Currency { Name = "Moldovan Leu", IsoSymbol = "MDL", Symbol = null },
                new Currency { Name = "Moroccan Dirham", IsoSymbol = "MAD", Symbol = "د.م.‏" },
                new Currency { Name = "Rial Omani", IsoSymbol = "OMR", Symbol = "ر.ع.‏" },
                new Currency { Name = "Nepalese Rupee", IsoSymbol = "NPR", Symbol = "रु" },
                new Currency { Name = "Netherlands Antillian Guilder", IsoSymbol = "ANG", Symbol = null },
                new Currency { Name = "Aruban Guilder", IsoSymbol = "AWG", Symbol = null },
                new Currency { Name = "Vatu", IsoSymbol = "VUV", Symbol = null },
                new Currency { Name = "New Zealand Dollar", IsoSymbol = "NZD", Symbol = "$" },
                new Currency { Name = "Cordoba Oro", IsoSymbol = "NIO", Symbol = "C$" },
                new Currency { Name = "Naira", IsoSymbol = "NGN", Symbol = null },
                new Currency { Name = "Norwegian Krone", IsoSymbol = "NOK", Symbol = "kr" },
                new Currency { Name = "Pakistan Rupee", IsoSymbol = "PKR", Symbol = "Rs" },
                new Currency { Name = "Balboa", IsoSymbol = "PAB", Symbol = "B/." },
                new Currency { Name = "Kina", IsoSymbol = "PGK", Symbol = null },
                new Currency { Name = "Guarani", IsoSymbol = "PYG", Symbol = "Gs" },
                new Currency { Name = "Nuevo Sol", IsoSymbol = "PEN", Symbol = "S/." },
                new Currency { Name = "Philippine Peso", IsoSymbol = "PHP", Symbol = "Php" },
                new Currency { Name = "Guinea-Bissau Peso", IsoSymbol = "GWP", Symbol = null },
                new Currency { Name = "Qatari Rial", IsoSymbol = "QAR", Symbol = "ر.ق.‏" },
                new Currency { Name = "Russian Ruble", IsoSymbol = "RUB", Symbol = "₽" },
                new Currency { Name = "Rwanda Franc", IsoSymbol = "RWF", Symbol = "RWF" },
                new Currency { Name = "Saint Helena Pound", IsoSymbol = "SHP", Symbol = null },
                new Currency { Name = "Dobra", IsoSymbol = "STD", Symbol = null },
                new Currency { Name = "Saudi Riyal", IsoSymbol = "SAR", Symbol = "ر.س.‏" },
                new Currency { Name = "Seychelles Rupee", IsoSymbol = "SCR", Symbol = null },
                new Currency { Name = "Leone", IsoSymbol = "SLL", Symbol = null },
                new Currency { Name = "Singapore Dollar", IsoSymbol = "SGD", Symbol = "$" },
                new Currency { Name = "Slovak Koruna", IsoSymbol = "SKK", Symbol = null },
                new Currency { Name = "Dong", IsoSymbol = "VND", Symbol = "₫" },
                new Currency { Name = "Somali Shilling", IsoSymbol = "SOS", Symbol = null },
                new Currency { Name = "Rand", IsoSymbol = "ZAR", Symbol = "R" },
                new Currency { Name = "Zimbabwe Dollar", IsoSymbol = "ZWD", Symbol = null },
                new Currency { Name = "Lilangeni", IsoSymbol = "SZL", Symbol = null },
                new Currency { Name = "Swedish Krona", IsoSymbol = "SEK", Symbol = "kr" },
                new Currency { Name = "Swiss Franc", IsoSymbol = "CHF", Symbol = "CHF" },
                new Currency { Name = "Syrian Pound", IsoSymbol = "SYP", Symbol = "ل.س.‏" },
                new Currency { Name = "Baht", IsoSymbol = "THB", Symbol = "฿" },
                new Currency { Name = "Pa'anga", IsoSymbol = "TOP", Symbol = null },
                new Currency { Name = "Trinidad and Tobago Dollar", IsoSymbol = "TTD", Symbol = "TT$" },
                new Currency { Name = "UAE Dirham", IsoSymbol = "AED", Symbol = "د.إ.‏" },
                new Currency { Name = "Tunisian Dinar", IsoSymbol = "TND", Symbol = "د.ت.‏" },
                new Currency { Name = "Manat", IsoSymbol = "TMM", Symbol = null },
                new Currency { Name = "Uganda Shilling", IsoSymbol = "UGX", Symbol = null },
                new Currency { Name = "Denar", IsoSymbol = "MKD", Symbol = "ден." },
                new Currency { Name = "Egyptian Pound", IsoSymbol = "EGP", Symbol = "ج.م.‏" },
                new Currency { Name = "Pound Sterling", IsoSymbol = "GBP", Symbol = "£" },
                new Currency { Name = "Tanzanian Shilling", IsoSymbol = "TZS", Symbol = null },
                new Currency { Name = "US Dollar", IsoSymbol = "USD", Symbol = "$" },
                new Currency { Name = "Peso Uruguayo", IsoSymbol = "UYU", Symbol = "$U" },
                new Currency { Name = "Uzbekistan Sum", IsoSymbol = "UZS", Symbol = "сўм" },
                new Currency { Name = "Tala", IsoSymbol = "WST", Symbol = null },
                new Currency { Name = "Yemeni Rial", IsoSymbol = "YER", Symbol = "ر.ي.‏" },
                new Currency { Name = "Kwacha", IsoSymbol = "ZMK", Symbol = null },
                new Currency { Name = "New Taiwan Dollar", IsoSymbol = "TWD", Symbol = "NT$" },
                new Currency { Name = "Ghana Cedi", IsoSymbol = "GHS", Symbol = null },
                new Currency { Name = "Bolivar Fuerte", IsoSymbol = "VEF", Symbol = "Bs. F." },
                new Currency { Name = "Sudanese Pound", IsoSymbol = "SDG", Symbol = null },
                new Currency { Name = "Serbian Dinar", IsoSymbol = "RSD", Symbol = "Дин." },
                new Currency { Name = "Metical", IsoSymbol = "MZN", Symbol = null },
                new Currency { Name = "Azerbaijanian Manat", IsoSymbol = "AZN", Symbol = "₼" },
                new Currency { Name = "New Leu", IsoSymbol = "RON", Symbol = "lei" },
                new Currency { Name = "New Turkish Lira", IsoSymbol = "TRY", Symbol = "₺" },
                new Currency { Name = "CFA Franc BEAC", IsoSymbol = "XAF", Symbol = null },
                new Currency { Name = "East Caribbean Dollar", IsoSymbol = "XCD", Symbol = null },
                new Currency { Name = "CFA Franc BCEAO", IsoSymbol = "XOF", Symbol = "XOF" },
                new Currency { Name = "CFP Franc", IsoSymbol = "XPF", Symbol = null },
                new Currency { Name = "Bond Markets Units European Composite Unit (EURCO)", IsoSymbol = "XBA", Symbol = null },
                new Currency { Name = "European Monetary Unit (E.M.U.-6)", IsoSymbol = "XBB", Symbol = null },
                new Currency { Name = "European Unit of Account 9(E.U.A.-9)", IsoSymbol = "XBC", Symbol = null },
                new Currency { Name = "European Unit of Account 17(E.U.A.-17)", IsoSymbol = "XBD", Symbol = null },
                new Currency { Name = "Gold", IsoSymbol = "XAU", Symbol = null },
                new Currency { Name = "SDR", IsoSymbol = "XDR", Symbol = null },
                new Currency { Name = "Silver", IsoSymbol = "XAG", Symbol = null },
                new Currency { Name = "Platinum", IsoSymbol = "XPT", Symbol = null },
                new Currency { Name = "Codes specifically reserved for testing purposes", IsoSymbol = "XTS", Symbol = null },
                new Currency { Name = "Palladium", IsoSymbol = "XPD", Symbol = null },
                new Currency { Name = "Surinam Dollar", IsoSymbol = "SRD", Symbol = null },
                new Currency { Name = "Malagasy Ariary", IsoSymbol = "MGA", Symbol = null },
                new Currency { Name = "Afghani", IsoSymbol = "AFN", Symbol = "؋" },
                new Currency { Name = "Somoni", IsoSymbol = "TJS", Symbol = "т.р." },
                new Currency { Name = "Kwanza", IsoSymbol = "AOA", Symbol = null },
                new Currency { Name = "Belarussian Ruble", IsoSymbol = "BYR", Symbol = null },
                new Currency { Name = "Bulgarian Lev", IsoSymbol = "BGN", Symbol = "лв." },
                new Currency { Name = "Franc Congolais", IsoSymbol = "CDF", Symbol = null },
                new Currency { Name = "Convertible Marks", IsoSymbol = "BAM", Symbol = "КМ" },
                new Currency { Name = "Euro", IsoSymbol = "EUR", Symbol = "€" },
                new Currency { Name = "Hryvnia", IsoSymbol = "UAH", Symbol = "₴" },
                new Currency { Name = "Lari", IsoSymbol = "GEL", Symbol = "₾" },
                new Currency { Name = "Zloty", IsoSymbol = "PLN", Symbol = "zł" },
                new Currency { Name = "Brazilian Real", IsoSymbol = "BRL", Symbol = "R$" }
            };

            foreach (var currency in currencies)
            {
                context.Currencies.Add(currency);
            }

            context.SaveChanges();

            // add travel services
            var usd = currencies.ToList().FirstOrDefault(c => c.IsoSymbol == "USD");
            if (usd != null)
            {
                var travelServices = new List<MEI.Core.DomainModels.Travel.AgencyService>
                {
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Air", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 1 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Hotel", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 2 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Car Service", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 3 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Air (Declined)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 4 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Hotel (Declined)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 5 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Car Service (Declined)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 6 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "After-Hours Air", FeeAmount = 55m, FeeCurrencyId = usd.Id, SortOrder = 7 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "After-Hours Hotel", FeeAmount = 55m, FeeCurrencyId = usd.Id, SortOrder = 8 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "After-Hours Car Service", FeeAmount = 55m, FeeCurrencyId = usd.Id, SortOrder = 9 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Car Rental", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 10 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Car Rental (Declined)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 11 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Rail", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 12 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Rail (Declined)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 13 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Air (Changed)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 14 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Air (Cancelled)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 15 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Hotel (Changed)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 16 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Hotel (Cancelled)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 17 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Car Service (Changed)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 18 },
                    new MEI.Core.DomainModels.Travel.AgencyService {Name = "Car Service (Cancelled)", FeeAmount = 25m, FeeCurrencyId = usd.Id, SortOrder = 19 },
                };

                foreach (var service in travelServices.OrderBy(s => s.SortOrder))
                {
                    context.AgencyServices.Add(service);
                }

                context.SaveChanges();
            }

            WorkflowCategoryEnum[] workflowCategories = (WorkflowCategoryEnum[])Enum.GetValues(typeof(WorkflowCategoryEnum));

            foreach (var category in workflowCategories)
            {
                context.WorkflowCategories.Add(new WorkflowCategory(category));
            }
            context.SaveChanges();

            WorkflowStepEnum[] workflowSteps = (WorkflowStepEnum[]) Enum.GetValues(typeof(WorkflowStepEnum));

            foreach (var step in workflowSteps)
            { 
                var workflow = new WorkflowStep(step)
                {
                    WorkflowCategoryId = 2,
                    StepOrder = (int)step
                };

                context.WorkflowSteps.Add(new WorkflowStep(step));
            }

            context.SaveChanges();

        }
    }
}