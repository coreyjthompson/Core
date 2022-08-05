using System;
using System.Collections;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.Document;
using MEI.SPDocuments.TypeCodes;

using Microsoft.Extensions.Options;

namespace MEI.SPDocuments
{
    public interface IDocumentFactory
    {
        IDocument CreateDocument(SPDocumentType documentType);

        IDocument CreateDocument(SPDocumentType documentType, Company company, string filePath, byte[] contents);

        IDocument CreateDocument(SPDocumentType documentType, Company company, string filePath, string savedFilePath);

        IDocument CreateDocument(SPDocumentType documentType, Company company, string filePath);

        IDocument CreateDocument(SPDocumentType documentType, Hashtable values);
    }

    internal class DocumentFactory
        : IDocumentFactory
    {
        private readonly IRepository _repository;
        private readonly IDbUtilities _dbUtilities;
        private readonly IDocumentInfoAggregator _documentInfoAggregator;
        private readonly IEmailer _emailer;
        private readonly IOptions<SPDocumentsOptions> _options;
        private readonly IPdfTools _pdfTools;

        public DocumentFactory(IRepository repository,
                               IDbUtilities dbUtilities,
                               IDocumentInfoAggregator documentInfoAggregator,
                               IEmailer emailer,
                               IOptions<SPDocumentsOptions> options,
                               IPdfTools pdfTools)
        {
            _repository = repository;
            _dbUtilities = dbUtilities;
            _documentInfoAggregator = documentInfoAggregator;
            _emailer = emailer;
            _options = options;
            _pdfTools = pdfTools;
        }

        /// <summary>
        ///     Creates a stock document of the specified <paramref name="documentType" />.
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        /// <returns>A stock document.</returns>
        public IDocument CreateDocument(SPDocumentType documentType)
        {
            Preconditions.CheckEnum("documentType", documentType, SPDocumentType.None);
            IDocument document;

            var info = _documentInfoAggregator.DocumentTypeInfos[documentType];

            switch (documentType)
            {
                case SPDocumentType.ADCV:
                    document = new ADCV(_repository, _dbUtilities, info);
                    break;
                case SPDocumentType.ADFMVT:
                    document = new ADFMVT(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ADFMVTE:
                    document = new ADFMVTE(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.AdhocSlidekit:
                    document = new AdhocSlidekit(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ADPSRF:
                    document = new ADPSRF(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ADSDocument:
                    document = new ADSDocument(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ADSSlideDeck:
                    document = new ADSSlideDeck(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.AirlineTicket:
                    document = new AirlineTicket(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.APE:
                    document = new AttendeeProgramEvaluation(_repository, _dbUtilities, info, _pdfTools);
					break;
                case SPDocumentType.BanquetEventOrder:
                    document = new BanquetEventOrder(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.BudgetWorksheet:
                    document = new BudgetWorksheet(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.CarConfirmationCancellation:
                    document = new CarConfirmationCancellation(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.CateringChecklist:
                    document = new CateringChecklist(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.CCAuthorization:
                    document = new CCAuthorization(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.CheckRequest:
                    document = new CheckRequest(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.DebarmentCheck:
                    document = new DebarmentCheck(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.EInvite:
                    document = new eInvite(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.EPHandout:
                    document = new EPHandout(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.EPInvite:
                    document = new EPInvite(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.EPPlacemat:
                    document = new EPPlacemat(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.EPPoster:
                    document = new EPPoster(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.EPMaterialKit:
                    document = new EPMaterialKit(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.EPSlideKit:
                    document = new EPSlidekit(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.EPThankYou:
                    document = new EPThankYou(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.EPTrifold:
                    document = new EPTrifold(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ExpenseReport:
                    document = new ExpenseReport(_repository, _dbUtilities, info, _emailer);
					break;
                case SPDocumentType.FairMarketValueTool:
                    document = new FairMarketValueTool(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.FairMarketValueToolException:
                    document = new FairMarketValueToolException(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.Handout:
                    document = new Handout(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.HonoDisputeAmendment:
                    document = new HonoDisputeAmendment(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.HotelConfirmationCancellation:
                    document = new HotelConfirmationCancellation(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.Invite:
                    document = new Invite(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.Invoice:
                    document = new Invoice(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ManualPIF:
                    document = new ManualPIF(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ManualSpeakerNomination:
                    document = new ManualSpeakerNomination(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.MaterialLetter:
                    document = new MaterialLetter(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.MSAPIF:
                    document = new MSAPIF(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.OffSiteMealCertification:
                    document = new OffSiteMealCertification(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ParticipantAnnualContract:
                    document = new ParticipantAnnualContract(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ParticipantCheck:
                    document = new ParticipantCheck(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ParticipantExpenseForm:
                    document = new ParticipantExpenseForm(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PharmaceuticalDetailersLicense:
                    document = new PharmaceuticalDetailersLicense(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PED:
                    document = new PWExportDoc(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PID:
                    document = new PWImportDoc(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PIF:
                    document = new PIF(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PRMApproval:
                    document = new PRMApproval(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ProgramChangesTravel:
                    document = new ProgramChangesTravel(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ProgramAttestation:
                    document = new ProgramAttestation(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ProgramCheck:
                    document = new ProgramCheck(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PPPE:
                    document = new ProgramPolicyProcedureException(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ProgramVendorMenu:
                    document = new ProgramVendorMenu(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PSA:
                    document = new PSA(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PSAAddendum:
                    document = new PSAAddendum(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PW9:
                    document = new PW9(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.RepReceipt:
                    document = new RepReceipt(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.Receipt:
                    document = new Receipt(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ReservationCancelNotice:
                    document = new ReservationCancelNotice(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ReservationChecklist:
                    document = new ReservationChecklist(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.RequestforConsultantApprovalForm:
                    document = new RequestforConsultantApprovalForm(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SaveTheDate:
                    document = new SaveTheDate(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SignInSheet:
                    document = new SignInSheet(_repository, _dbUtilities, info, _emailer, _options);
					break;
                case SPDocumentType.AttendeeSignInSheet:
                    document = new AttendeeSignInSheet(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SpeakerBuiltSlideKit:
                    document = new SpeakerBuiltSlideKit(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SpeakerCapException:
                    document = new SpeakerCapException(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SpeakerCheck:
                    document = new SpeakerCheck(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SpeakerNomination:
                    document = new SpeakerNomination(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SpeakerNomInitiationDateExcpt:
                    document = new SpeakerNomInitiationDateExcpt(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SpeakerTrainingDeck:
                    document = new SpeakerTrainingDeck(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SpkrTrainTravelInvoice:
                    document = new SpkrTrainTravelInvoice(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SpkrTrainTravelItinerary:
                    document = new SpkrTrainTravelItinerary(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SpkrTrainTravelReceipt:
                    document = new SpkrTrainTravelReceipt(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.StopPayNotice:
                    document = new StopPayNotice(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.TestingDoc:
                    document = new TestingDoc(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ThankYouLetter:
                    document = new ThankYouLetter(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ThirdPartyEventApproval:
                    document = new ThirdPartyEventApproval(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.TrainingWaiver:
                    document = new TrainingWaiver(_repository, _dbUtilities, info);
					break;

                //case SPDocumentTypeCode.TravelEmail:
                //   document = new TravelEmail(_infoHelper, _dbUtilities, _documentInfoAggregator);
				//	break;
                case SPDocumentType.TravelItinerary:
                    document = new TravelItinerary(_repository, _dbUtilities, info);
					break;

                //case SPDocumentTypeCode.TravelRequest:
                //    document = new TravelRequest(_infoHelper, _dbUtilities, _documentInfoAggregator);
				//	break;
                case SPDocumentType.TravelReceipt:
                    document = new TravelReceipt(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.TravelInvoice:
                    document = new TravelInvoice(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.VenueBEO:
                    document = new VenueBEO(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.VenueContract:
                    document = new VenueContract(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.VenueException:
                    document = new VenueException(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.VendorMenu:
                    document = new VendorMenu(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.W9:
                    document = new W9(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.PID2:
                    document = new PWImportDoc2(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.RSVPInvite:
                    document = new RSVPInvite(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.ReceiptBreakoutWorkpaper:
                    document = new ReceiptBreakoutWorkpaper(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.SampleInvite:
                    document = new SampleInvite(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.RE:
                    document = new ReinstatementException(_repository, _dbUtilities, info);
					break;
                case SPDocumentType.GeneralProgramSupportForm:
                    document = new GeneralProgramSupportForm(_repository, _dbUtilities, info);
                    break;
                case SPDocumentType.ProgramException:
                    document = new ProgramException(_repository, _dbUtilities, info);
                    break;
                case SPDocumentType.ThirdPartySupportingDocument:
                    document = new ThirdPartySupportingDocument(_repository, _dbUtilities, info);
                    break;
                case SPDocumentType.TravelAgencyServiceInvoice:
                    document = new TravelAgencyServiceInvoice(_repository, _dbUtilities, info);
                    break;
                default:
                    throw new ArgumentException(string.Format(Resources.Default.Invalid_documentType_value__0, documentType.ToString()),
                        nameof(documentType));
            }

            return document;
        }

        /// <summary>
        ///     Creates a document of the specified <paramref name="documentType" /> with the specified
        ///     <paramref name="fileName" />, <paramref name="contents" />, and <paramref name="company" />.
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <param name="company">The company the file is associated with.</param>
        /// <returns>A document.</returns>
        public IDocument CreateDocument(SPDocumentType documentType, Company company, string fileName, byte[] contents)
        {
            Preconditions.CheckEnum("documentType", documentType, SPDocumentType.None);

            IDocument document = CreateDocument(documentType);
            document.With(fileName, contents, company);

            return document;
        }

        /// <summary>
        ///     Creates a document of the specified <paramref name="documentType" />
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="savedFilePath"></param>
        /// <param name="company"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IDocument CreateDocument(SPDocumentType documentType, Company company, string filePath, string savedFilePath)
        {
            Preconditions.CheckEnum("documentType", documentType, SPDocumentType.None);

            IDocument document = CreateDocument(documentType);
            document.With(filePath, savedFilePath, company);

            return document;
        }

        /// <summary>
        ///     Creates a document of the specified <paramref name="documentType" /> with specified <paramref name="filePath" />
        ///     and <paramref name="company" />.
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="company">The company.</param>
        /// <returns></returns>
        public IDocument CreateDocument(SPDocumentType documentType, Company company, string filePath)
        {
            Preconditions.CheckEnum("documentType", documentType, SPDocumentType.None);

            IDocument document = CreateDocument(documentType);
            document.With(filePath, company);

            return document;
        }

        public IDocument CreateDocument(SPDocumentType documentType, Hashtable values)
        {
            IDocument doc = CreateDocument(documentType);
            doc.AbstractSetup(values);

            return doc;
        }
    }
}
