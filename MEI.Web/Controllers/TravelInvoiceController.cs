using System.Collections.Generic;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Queries;
using MEI.Travel.Commands;
using MEI.Travel.Queries;
using MEI.Core.Queries;
using MEI.Web.Models.TravelInvoice;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AbbVieQueries = MEI.AbbVie.Infrastructure.Queries;

namespace MEI.Web.Controllers
{
    public class TravelInvoiceController
        : Controller
    {
        private readonly IQueryProcessor _queries;
        private readonly ICommandProcessor _commands;

        public TravelInvoiceController(IQueryProcessor queries, ICommandProcessor commands)
        {
            _queries = queries;
            _commands = commands;
        }

        public IActionResult ListUsingQuery()
        {
            var query = new Demo_GetAllInvoicesForClientQuery
                        {
                            ClientName = "Client1",
                            Paging = new PageInfo
                            {
                                PageIndex = 0,
                                PageSize = 1
                            }
                        };

            var invoices = _queries.Execute(query);

            var model = new ListViewModel
                        {
                            Invoices = invoices.Result.Items
                        };

            return View("List", model);
        }

        public async Task<JsonResult> TestUsingQueryAsync()
        {
            var query = new Demo_GetAllInvoicesForClientQuery
                        {
                            ClientName = "Client1",
                            Paging = new PageInfo
                                     {
                                         PageIndex = 0,
                                         PageSize = 1
                                     }
                        };

            var invoices = await _queries.Execute(query);

            return Json(invoices.Items);
        }

        public IActionResult Edit(int id)
        {
            var editInvoiceCommand = new Demo_EditInvoiceCommand
                                    {
                                        InvoiceId = 4,
                                        ClientName = "Client1"
                                    };
            _commands.Execute(editInvoiceCommand);

            return View();
        }

        public async Task<JsonResult> TestEdit(int id)
        {
            var editInvoiceCommand = new Demo_EditInvoiceCommand
                                     {
                                         InvoiceId = 4,
                                         ClientName = "Client1"
                                     };

            return Json(new
                              {
                                  invoiceId = await _commands.Execute(editInvoiceCommand)
                              });
        }

        public ActionResult Add()
        {
            _commands.Execute(new Demo_AddInvoiceCommand
                                      {
                                          ClientName = "Client1"
                                      });

            return View("Edit");
        }

    }
}
