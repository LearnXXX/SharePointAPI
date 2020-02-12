using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharePointGraph
{
   
    class CustomHTTPProvider : IHttpProvider, IDisposable
    {
        public ISerializer Serializer { get; set; }
        private HttpProvider provider;
        public TimeSpan OverallTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CustomHTTPProvider(ISerializer serializer)
        {
            Serializer = serializer;
            provider = new HttpProvider(new XLuoRetryHandler(new HttpClientHandler()), true, serializer);
            //provider = new HttpProvider(new CustomHttpMessageHandler(), true, serializer);
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            provider.SendAsync(request);
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return  provider.SendAsync(request, completionOption, cancellationToken);
            //throw new NotImplementedException();
        }
    }
}
