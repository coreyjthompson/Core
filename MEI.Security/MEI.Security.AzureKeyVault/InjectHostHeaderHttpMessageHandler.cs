namespace MEI.Security.AzureKeyVault
{
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class InjectHostHeaderHttpMessageHandler
        : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Uri requestUri = request.RequestUri;
            Uri targetUri = requestUri;

            // NOTE: The KmsNetworkUrl setting is purely for development testing on the
            //	Microsoft Azure Development Fabric and should not be used outside that environment.
            string networkUrl = ConfigurationManager.AppSettings["KmsNetworkUrl"];

            if (!string.IsNullOrEmpty(networkUrl))
            {
                string authority = targetUri.Authority;
                targetUri = new Uri(new Uri(networkUrl), targetUri.PathAndQuery);

                request.Headers.Add("Host", authority);
                request.RequestUri = targetUri;
            }

            return base.SendAsync(request, cancellationToken).ContinueWith(response => response.Result);
        }
    }
}
