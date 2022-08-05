namespace MEI.SPDocuments.TypeCodes
{
    public enum Company
    {
        Undefined,
        [EnumDescription("Abbott", "Abbott")]
        Abbott,
        [EnumDescription("AbbottDIV", "AbbottDIV", "AbbottStructuralHeart", "Abbott Structural Heart", "AbbottStructuralHeart")]
        AbbottStructuralHeart,
        [EnumDescription("AbbVieImm", "AbbVieImmunology")]
        AbbVieImmunology,
        [EnumDescription("AbbVieTrain", "AbbVieTrain")]
        AbbVieTrain,
        [EnumDescription("Alkermes", "Alkermes")]
        Alkermes,
        [EnumDescription("Ross", "Ross", "abbottnutrition", "abbott nutrition", "abbott-nutrition")]
        AbbottNutrition,
        [EnumDescription("Solvay", "Solvay")]
        Solvay,
        [EnumDescription("MEIUniversal", "MEIUniversal", "mei universal")]
        MeiUniversal,
        [EnumDescription("LabDevelopment", "LabDevelopment", "lab development")]
        LabDevelopment,
        [EnumDescription("AbbottNutritionCE", "AbbottNutritionCE", "abbott nutrition ce")]
        AbbottNutritionCE,
        [EnumDescription("Kowa", "Kowa")]
        Kowa,
        [EnumDescription("LipoScience", "LipoScience", "lipo science")]
        LipoScience,
        [EnumDescription("RDT", "RDT")]
        RDT,
        [EnumDescription("Quest", "Quest")]
        Quest,

        //Abbott Divisions
        [EnumDescription("PPG", "Pharmaseutical ProductsGroup")]
        PharmaseuticalProductsGroup,
        [EnumDescription("PPD", "Proprietary Pharmaceuticals Division", "ProprietaryPharmaceuticalsDivision")]
        ProprietaryPharmaceuticalsDivision,
        [EnumDescription("AM", "Abbott Molecular")]
        AbbottMolecular,
        [EnumDescription("GPRD", "Global Pharmaceutical Research and Development")]
        GlobalPharmaceuticalResearchAndDevelopment,
        [EnumDescription("GSMS", "Global Strategic Marketing & Services")]
        GlobalStrategicMarketingAndServices,
        [EnumDescription("AAH", "Abbott Animal Health")]
        AbbottAnimalHealth,
        [EnumDescription("ADC", "Abbott Diabetes Care")]
        AbbottDiabetesCare,
        [EnumDescription("ADD", "Abbott Diagnostics Division")]
        AbbottDiagnosticsDivision,
        [EnumDescription("AMO", "Abbott Medical Optics")]
        AbbottMedicalOptics,
        [EnumDescription("DAN", "Div Abbott Nutrition")]
        DivAbbottNutrition,
        [EnumDescription("APOC", "Abbott Point of Care")]
        AbbottPointOfCare,
        [EnumDescription("AV", "Abbott Vascular")]
        AbbottVascular,
        [EnumDescription("CORP", "Corporate")]
        Corporate,
        [EnumDescription("EPD", "Established Products Division")]
        EstablishedProductsDivision,
        [EnumDescription("RA", "Regulatory Affairs, PPG")]
        RegulatoryAffairsPPG,
        [EnumDescription("ARDx", "Abbott Rapid Diagnostics")]
        AbbottRapidDiagnostics,
        [EnumDescription("ExactSciences", "ExactSciences", "ExactSciences")]
        ExactSciences,
        [EnumDescription("CAHF", "CAHF", "FMV Cardiac Arrhythmias and Heart Failure")]
        CAHF,
        [EnumDescription("NM", "NM", "FMV Neuromodulation")]
        NM,
        [EnumDescription("SH", "SH", "FMV Structural Heart")]
        SH
    }

    public static class CompanyExtensions
    {
        private static readonly EnumDescriptionCollection<Company> Description = new EnumDescriptionCollection<Company>();

        public static string ToDisplayNameLong(this Company code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this Company code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static Company ToCompany(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
