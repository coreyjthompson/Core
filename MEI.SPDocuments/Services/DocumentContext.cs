using System;
using System.Collections.Generic;
using System.Security;

using Microsoft.SharePoint.Client;

namespace MEI.SPDocuments.Services
{
    public sealed class DocumentContext
        : IDisposable
    {
        private readonly ClientContext _context;

        public DocumentContext(string siteUrl, string userName, SecureString password)
        {
            if (string.IsNullOrEmpty(siteUrl))
            {
                throw new ArgumentNullException(nameof(siteUrl));
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            _context = new ClientContext(siteUrl)
                       {
                           Credentials = new SharePointOnlineCredentials(userName, password)
                       };
        }

        public IList<Microsoft.SharePoint.Client.File> Search(string documentLibraryTitle, string camlQuery, bool includeVersions)
        {
            Web web = _context.Web;
            List list = web.Lists.GetByTitle(documentLibraryTitle);
            _context.Load(list);
            _context.ExecuteQuery();

            var results = new List<Microsoft.SharePoint.Client.File>();
            var query = new CamlQuery
                        {
                            ViewXml = camlQuery
                        };

            ListItemCollection items = list.GetItems(query);
            _context.Load(items);
            _context.ExecuteQuery();

            foreach (ListItem item in items)
            {
                _context.Load(item.File.ListItemAllFields);

                if (includeVersions)
                {
                    _context.Load(item.File.Versions);
                }

                results.Add(item.File);
            }

            _context.ExecuteQuery();

            return results;
        }

        public void Upload(string documentLibraryTitle, string fileName, byte[] fileContents, IDictionary<string, string> userFields)
        {
            Web web = _context.Web;

            List list = web.Lists.GetByTitle(documentLibraryTitle);
            _context.Load(list);
            _context.ExecuteQuery();

            var query = new CamlQuery
                        {
                            ViewXml = string.Format(@"<View>
                                    <Query>
                                        <Where>
                                            <Eq>
                                                <FieldRef Name='FileLeafRef' /><Value Type='Text'>{0}</Value>
                                            </Eq>
                                        </Where>
                                    </Query>
                                </View>",
                                fileName)
                        };

            ListItemCollection items = list.GetItems(query);
            _context.Load(items);
            _context.ExecuteQuery();

            if (items == null || items.Count == 0)
            {
                var fileCreationInformation = new FileCreationInformation
                                              {
                                                  Content = fileContents,
                                                  Url = fileName
                                              };
                var file = list.RootFolder.Files.Add(fileCreationInformation);

                foreach (KeyValuePair<string, string> pair in userFields)
                {
                    file.ListItemAllFields[pair.Key] = pair.Value;
                }

                file.ListItemAllFields["Title"] = fileName;

                // this will create the file at version 1.0 and then the fields at version 2.0
                file.ListItemAllFields.Update();

                // This will create the file and the fields on it as version 1.0
                //file.ListItemAllFields.SystemUpdate();

                _context.ExecuteQuery();
            }
            else
            {
                _context.Load(items[0].File, f => f.CheckOutType, f => f.ListItemAllFields);
                _context.ExecuteQuery();

                var file = items[0].File;

                try
                {
                    file.CheckOut();
                    var fileSaveInformation = new FileSaveBinaryInformation
                                              {
                                                  Content = fileContents
                                              };
                    file.SaveBinary(fileSaveInformation);

                    file.CheckIn("", CheckinType.MajorCheckIn);

                    _context.ExecuteQuery();
                }
                catch
                {
                    if (file != null)
                    {
                        if (file.CheckOutType != CheckOutType.None)
                        {
                            file.UndoCheckOut();

                            _context.ExecuteQuery();
                        }
                    }

                    throw;
                }
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
