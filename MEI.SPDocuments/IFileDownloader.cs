using System;
using System.Net;

namespace MEI.SPDocuments
{
    public interface IFileDownloader
    {
        byte[] DownloadFile(string userName, string password, string domain, Uri fileUri);
    }

    internal class WebDownloader
        : IFileDownloader
    {
        public byte[] DownloadFile(string userName, string password, string domain, Uri fileUri)
        {
            var credentials = new NetworkCredential(userName, password, domain);

            using (var webClient = new WebClient
                                   {
                                       Credentials = credentials
                                   })
            {
                return webClient.DownloadData(fileUri);
            }
        }
    }
}
