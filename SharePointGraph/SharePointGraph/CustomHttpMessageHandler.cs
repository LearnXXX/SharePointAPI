using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharePointGraph
{
    class XLuoRetryHandler : DelegatingHandler
    {
        // Strongly consider limiting the number of retries - "retry forever" is
        // probably not the most user friendly way you could respond to "the
        // network cable got pulled out."
        private const int MaxRetries = 3;

        public XLuoRetryHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        { }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < MaxRetries; i++)
            {
                response = await base.SendAsync(request, new CancellationToken(false));
                if (!response.IsSuccessStatusCode
                       && (response.StatusCode == (HttpStatusCode)429))
                { }
                    if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }

            return response;
        }
    }

    internal class CustomHttpMessageHandler : HttpMessageHandler
    {
        #region Fields

        private static readonly Action<object> s_onCancel = OnCancel;

        private readonly Action<object> _startRequest;
        private readonly AsyncCallback _getRequestStreamCallback;
        private readonly AsyncCallback _getResponseCallback;

        private volatile bool _operationStarted;
        private volatile bool _disposed;

        private long _maxRequestContentBufferSize;
        private int _maxResponseHeadersLength;
        private CookieContainer _cookieContainer;
        private bool _useCookies;
        private DecompressionMethods _automaticDecompression;
        private IWebProxy _proxy;
        private bool _useProxy;
        private ICredentials _defaultProxyCredentials;
        private bool _preAuthenticate;
        private bool _useDefaultCredentials;
        private ICredentials _credentials;
        private bool _allowAutoRedirect;
        private int _maxAutomaticRedirections;
        private string _connectionGroupName;
        private ClientCertificateOption _clientCertOptions;
        private X509Certificate2Collection _clientCertificates;
        private IDictionary<String, Object> _properties;
        private int _maxConnectionsPerServer;
        private bool _maxConnectionsPerServerChanged;
        private Func<
            HttpRequestMessage,
            X509Certificate2,
            X509Chain,
            SslPolicyErrors,
            bool> _serverCertificateCustomValidationCallback;
        private SslProtocols _sslProtocols;
        private bool _checkCertificateRevocationList;
#if NET_4
        private Uri _lastUsedRequestUri;
#endif

#if DEBUG
        // The following delegate is only used for unit-testing: It allows tests to create a custom HttpWebRequest
        // instance.
        internal delegate HttpWebRequest WebRequestCreatorDelegate(HttpRequestMessage request, string connectionGroupName);
        internal WebRequestCreatorDelegate WebRequestCreator = null;
#endif

        #endregion Fields

        #region Properties

        public bool CheckCertificateRevocationList
        {
            get
            {
                return _checkCertificateRevocationList;
            }

            set
            {
                CheckDisposedOrStarted();
                _checkCertificateRevocationList = value;
            }
        }

        public X509CertificateCollection ClientCertificates
        {
            get
            {
                if (_clientCertOptions != ClientCertificateOption.Manual)
                {
                    //throw new InvalidOperationException(string.Format(SR.net_http_invalid_enable_first, "ClientCertificateOptions", "Manual"));
                }

                if (_clientCertificates == null)
                {
                    _clientCertificates = new X509Certificate2Collection();
                }

                return _clientCertificates;
            }
        }

        public ICredentials DefaultProxyCredentials
        {
            get
            {
                return _defaultProxyCredentials;
            }

            set
            {
                CheckDisposedOrStarted();
                _defaultProxyCredentials = value;
            }
        }

        public int MaxConnectionsPerServer
        {
            get
            {
                return _maxConnectionsPerServer;
            }

            set
            {
                CheckDisposedOrStarted();
                _maxConnectionsPerServerChanged = true;
                _maxConnectionsPerServer = value;
            }
        }

        public int MaxResponseHeadersLength
        {
            get
            {
                return _maxResponseHeadersLength;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                CheckDisposedOrStarted();
                _maxResponseHeadersLength = value;
            }
        }

        public IDictionary<String, Object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new Dictionary<String, object>();
                }

                return _properties;
            }
        }

        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
        {
            get
            {
                return _serverCertificateCustomValidationCallback;
            }

            set
            {
                CheckDisposedOrStarted();
                _serverCertificateCustomValidationCallback = value;
            }
        }

        public SslProtocols SslProtocols
        {
            get
            {
                return _sslProtocols;
            }

            set
            {
                SecurityProtocol.ThrowOnNotAllowed(value, allowNone: true);
                CheckDisposedOrStarted();
                _sslProtocols = value;
            }
        }

        public virtual bool SupportsAutomaticDecompression
        {
            get { return true; }
        }

        public virtual bool SupportsProxy
        {
            get { return true; }
        }

        public virtual bool SupportsRedirectConfiguration
        {
            get { return true; }
        }

        public bool UseCookies
        {
            get { return _useCookies; }
            set
            {
                CheckDisposedOrStarted();
                _useCookies = value;
            }
        }

        public CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!UseCookies)
                {
                    //throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                    //    SR.net_http_invalid_enable_first, "UseCookies", "true"));
                }
                CheckDisposedOrStarted();
                _cookieContainer = value;
            }
        }

        public ClientCertificateOption ClientCertificateOptions
        {
            get { return _clientCertOptions; }
            set
            {
                if (value != ClientCertificateOption.Manual
                    && value != ClientCertificateOption.Automatic)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                CheckDisposedOrStarted();
                _clientCertOptions = value;
            }
        }

        public DecompressionMethods AutomaticDecompression
        {
            get { return _automaticDecompression; }
            set
            {
                CheckDisposedOrStarted();
                _automaticDecompression = value;
            }
        }

        public bool UseProxy
        {
            get { return _useProxy; }
            set
            {
                CheckDisposedOrStarted();
                _useProxy = value;
            }
        }

        public IWebProxy Proxy
        {
            get { return _proxy; }
            [SecuritySafeCritical]
            set
            {
                if (!UseProxy && value != null)
                {
                    //throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                    //    SR.net_http_invalid_enable_first, "UseProxy", "true"));
                }
                CheckDisposedOrStarted();
                //ExceptionHelper.WebPermissionUnrestricted.Demand();
                _proxy = value;
            }
        }

        public bool PreAuthenticate
        {
            get { return _preAuthenticate; }
            set
            {
                CheckDisposedOrStarted();
                _preAuthenticate = value;
            }
        }

        public bool UseDefaultCredentials
        {
            get { return _useDefaultCredentials; }
            set
            {
                CheckDisposedOrStarted();
                _useDefaultCredentials = value;
            }
        }

        public ICredentials Credentials
        {
            get { return _credentials; }
            set
            {
                CheckDisposedOrStarted();
                _credentials = value;
            }
        }

        public bool AllowAutoRedirect
        {
            get { return _allowAutoRedirect; }
            set
            {
                CheckDisposedOrStarted();
                _allowAutoRedirect = value;
            }
        }

        public int MaxAutomaticRedirections
        {
            get { return _maxAutomaticRedirections; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                CheckDisposedOrStarted();
                _maxAutomaticRedirections = value;
            }
        }

        public long MaxRequestContentBufferSize
        {
            get { return _maxRequestContentBufferSize; }
            set
            {
                // Setting the value to 0 is OK: It means the user doesn't want the handler to buffer content.
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value > MaxBufferSize)
                {
                    //throw new ArgumentOutOfRangeException("value", value,
                    //    string.Format(CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit,
                    //    MaxBufferSize));
                }
                CheckDisposedOrStarted();
                _maxRequestContentBufferSize = value;
            }
        }

        #endregion Properties

        #region Delegates

        public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> DangerousAcceptAnyServerCertificateValidator { get; } = delegate { return true; };

        #endregion Delegates

        #region De/Constructors

        private static long MaxBufferSize = 0x7fffffffL;
        public CustomHttpMessageHandler()
        {
            _startRequest = StartRequest;
            _getRequestStreamCallback = GetRequestStreamCallback;
            _getResponseCallback = GetResponseCallback;

            _connectionGroupName = RuntimeHelpers.GetHashCode(this).ToString(NumberFormatInfo.InvariantInfo);

            // Set HWR default values
            _allowAutoRedirect = true;
            _maxRequestContentBufferSize = MaxBufferSize;
            _automaticDecompression = DecompressionMethods.None;
            _cookieContainer = new CookieContainer(); // default container used for dealing with auto-cookies.
            _credentials = null;
            _maxAutomaticRedirections = 50;
            _preAuthenticate = false;
            _proxy = null;
            _useProxy = true;
            _useCookies = true; // deal with cookies by default.
            _useDefaultCredentials = false;
            _clientCertOptions = ClientCertificateOption.Manual;

            // New properties added in .NET Framework 4.7.1.
            _maxResponseHeadersLength = HttpWebRequest.DefaultMaximumResponseHeadersLength;
            _defaultProxyCredentials = null;
            _clientCertificates = null; // only create collection when required.
            _properties = null; // only create collection when required.
            _maxConnectionsPerServer = ServicePointManager.DefaultConnectionLimit;
            _maxConnectionsPerServerChanged = false;
            _serverCertificateCustomValidationCallback = null;
            _sslProtocols = SslProtocols.None;
            _checkCertificateRevocationList = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
#if NET_4
                // Our best effort to close the connection group based on the last-used request uri
                if (_lastUsedRequestUri != null)
                {
                    ServicePoint servicePoint = ServicePointManager.FindServicePoint(_lastUsedRequestUri);
                    if (servicePoint != null)
                    {
                        servicePoint.CloseConnectionGroup(_connectionGroupName);
                    }
                }
#else
                // Close all connection groups created by the current handler instance. Since every instance uses a
                // unique connection group name, disposing a handler will remove all these unique connection groups to
                // save resources.
                //ServicePointManager.CloseConnectionGroups(_connectionGroupName);
#endif
            }
            base.Dispose(disposing);
        }

        #endregion De/Constructors

        #region Request Setup

        private HttpWebRequest CreateAndPrepareWebRequest(HttpRequestMessage request)
        {
#if NET_4
            HttpWebRequest webRequest = WebRequest.CreateDefault(request.RequestUri) as HttpWebRequest;
            webRequest.ConnectionGroupName = _connectionGroupName;
#else
            HttpWebRequest webRequest = null;

            // If we have a request-content, make sure to provide HWR with a delegate to CopyTo(). This allows HWR
            // to serialize the content multiple times in case of redirect/authentication.
            // Also note that the connection group name provided is considered an 'internal' connection group. I.e.
            // HWR will add 'I>' after the string we provided. I.e. by default the actual connection group name looks 
            // like '123456S>I>' or '123456U>I>' if UnsafeAuthenticatedConnectionSharing is true. Even is users use the
            // same hashcode for their HWR connection group, they'll end up using a different one, since 'I>' is not
            // added ('123456S>' or '123456U>').
            if (request.Content != null)
            {
                //webRequest = new HttpWebRequest(request.RequestUri, true, _connectionGroupName, request.Content.CopyTo);
            }
            else
            {
                var type = typeof(HttpWebRequest);
                const BindingFlags ctorFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase;
                Type[] paramTypes = new Type[] { typeof(Uri), typeof(bool), typeof(string), typeof(Action<Stream>) };
                ConstructorInfo ctorInfo = type.GetConstructor(ctorFlag, null, paramTypes, null);
                object[] args = new object[] { request.RequestUri, true, _connectionGroupName, null };
                webRequest = ctorInfo.Invoke(args) as HttpWebRequest;
                //webRequest = Activator.CreateInstance(typeof(HttpWebRequest), request.RequestUri, true, _connectionGroupName, null) as HttpWebRequest;
                //webRequest = new HttpWebRequest(request.RequestUri, true, _connectionGroupName, null);
            }
#endif

#if DEBUG
            // For testing purposes only: If the delegate is assigned, it is used to create an instance of 
            // HttpWebRequest. Tests can derive from HttpWebRequest and implement their own behavior.
            if (WebRequestCreator != null)
            {
                webRequest = WebRequestCreator(request, _connectionGroupName);
                Contract.Assert(webRequest != null);
            }
#endif

            //if (Logging.On) Logging.Associate(Logging.Http, request, webRequest);

            webRequest.Method = request.Method.Method;
            webRequest.ProtocolVersion = request.Version;

            SetDefaultOptions(webRequest);
            SetConnectionOptions(webRequest, request);
            SetServicePointOptions(webRequest, request);
            SetRequestHeaders(webRequest, request);
            SetContentHeaders(webRequest, request);
#if !NET_4
            //request.SetRtcOptions(webRequest);
#endif

            // New properties for this OOB HttpClientHandler.
            if (_maxConnectionsPerServerChanged)
            {
                webRequest.ServicePoint.ConnectionLimit = _maxConnectionsPerServer;
            }

            webRequest.MaximumResponseHeadersLength = _maxResponseHeadersLength;
            if ((ClientCertificateOptions == ClientCertificateOption.Manual)
                && (_clientCertificates != null) && (_clientCertificates.Count > 0))
            {
                webRequest.ClientCertificates = _clientCertificates;
            }

            if (_serverCertificateCustomValidationCallback != null)
            {
                webRequest.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
                //webRequest.ServerCertificateValidationCallbackContext = (object)request;
            }

            if (_defaultProxyCredentials != null && _useProxy && _proxy == null && webRequest.Proxy != null)
            {
                // The HttpClientHandler has specified to use a proxy but has not set an explicit IWebProxy.
                // That means to use the default proxy on the underlying webrequest object. The initial value
                // of the webrequest.Proxy when first created comes from the static WebRequest.DefaultWebProxy.
                // In the default case, this value is non-null. But can be set later to null. That is why the
                // 'if' check above validates for a non-null webRequest.Proxy.
                webRequest.Proxy.Credentials = _defaultProxyCredentials;
            }

            //if (_checkCertificateRevocationList)
            //{
            //    webRequest.CheckCertificateRevocationList = _checkCertificateRevocationList;
            //}

            //if (_sslProtocols != SslProtocols.None)
            //{
            //    webRequest.SslProtocols = _sslProtocols;
            //}

            // For Extensibility
            InitializeWebRequest(request, webRequest);

            return webRequest;
        }

        // Used to map the ServerCertificateCustomValidationCallback which uses Func<T> to the
        // HttpWebRequest based RemoteCertificateValidationCallback delegate type.
        private bool ServerCertificateValidationCallback(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            HttpWebRequest hwr = (HttpWebRequest)sender;
            //Debug.Assert(hwr != null);
            //HttpRequestMessage request = (HttpRequestMessage)hwr.ServerCertificateValidationCallbackContext;
            //Debug.Assert(request != null);
            HttpRequestMessage request = null;
            return _serverCertificateCustomValidationCallback(
                request,
                (X509Certificate2)certificate, // This cast will usually always be safe.
                chain,
                sslPolicyErrors);
        }

        // Needs to be internal so that WebRequestHandler can access it from a different assembly.
        internal virtual void InitializeWebRequest(HttpRequestMessage request, HttpWebRequest webRequest)
        {
        }

        private void SetDefaultOptions(HttpWebRequest webRequest)
        {
            webRequest.Timeout = Timeout.Infinite; // Timeouts are handled by HttpClient.

            webRequest.AllowAutoRedirect = _allowAutoRedirect;
            webRequest.AutomaticDecompression = _automaticDecompression;
            webRequest.PreAuthenticate = _preAuthenticate;

            if (_useDefaultCredentials)
            {
                webRequest.UseDefaultCredentials = true;
            }
            else
            {
                webRequest.Credentials = _credentials;
            }

            if (_allowAutoRedirect)
            {
                webRequest.MaximumAutomaticRedirections = _maxAutomaticRedirections;
            }

            if (_useProxy)
            {
                // If 'UseProxy' is true and 'Proxy' is null (default), let HWR figure out the proxy to use. Otherwise
                // set the custom proxy.
                if (_proxy != null)
                {
                    webRequest.Proxy = _proxy;
                }
            }
            else
            {
                // The use explicitly specified to not use a proxy. Set HWR.Proxy to null to make sure HWR doesn't use
                // a proxy for this request.
                webRequest.Proxy = null;
            }

            if (_useCookies)
            {
                webRequest.CookieContainer = _cookieContainer;
            }

#if !NET_4
            //if (_clientCertOptions == ClientCertificateOption.Automatic && ComNetOS.IsWin7orLater)
            //{
            //    X509CertificateCollection automaticClientCerts
            //        = UnsafeNclNativeMethods.NativePKI.FindClientCertificates();
            //    if (automaticClientCerts.Count > 0)
            //    {
            //        webRequest.ClientCertificates = automaticClientCerts;
            //    }
            //}
#endif
        }

        private static void SetConnectionOptions(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            if (request.Version <= HttpVersion.Version10)
            {
                // HTTP 1.0 had some support for persistent connections by allowing "Connection: Keep-Alive". Check
                // whether this value is set.
                bool keepAliveSet = false;
                foreach (string item in request.Headers.Connection)
                {
                    if (string.Compare(item, "Keep-Alive", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        keepAliveSet = true;
                        break;
                    }
                }
                webRequest.KeepAlive = keepAliveSet;
            }
            else
            {
                // HTTP 1.1 uses persistent connections by default. If the user doesn't want to use persistent 
                // connections, he can set 'ConnectionClose' to true (equivalent to header "Connection: close").
                if (request.Headers.ConnectionClose == true)
                {
                    webRequest.KeepAlive = false;
                }
            }
        }

        private void SetServicePointOptions(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            HttpRequestHeaders headers = request.Headers;
            ServicePoint currentServicePoint = null;

            // We have to update the ServicePoint in order to support "Expect: 100-continue". This setting may affect
            // also requests sent by other HWR instances (or HttpClient instances). This is a known limitation.
            bool? expectContinue = headers.ExpectContinue;
            if (expectContinue != null)
            {
                currentServicePoint = webRequest.ServicePoint;
                currentServicePoint.Expect100Continue = (bool)expectContinue;
            }
        }
        private const BindingFlags INVOKEFLAGS = BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.IgnoreCase;

        private static void SetRequestHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            WebHeaderCollection webRequestHeaders = webRequest.Headers;
            HttpRequestHeaders headers = request.Headers;

            // Most headers are just added directly to HWR's internal headers collection. But there are some exceptions
            // requiring different handling.
            // The following bool vars are used to skip string comparison when not required: E.g. if the 'Host' header
            // was not set, we don't need to compare every header in the collection with 'Host' to make sure we don't
            // add it to HWR's header collection.
            bool isHostSet = headers.Contains("Host");
            bool isExpectSet = headers.Contains("Expect");
            bool isTransferEncodingSet = headers.Contains("Transfer-Encoding");
            bool isConnectionSet = headers.Contains("Connection");
#if NET_4
            bool isAcceptSet = headers.Contains(HttpKnownHeaderNames.Accept);
            bool isRangeSet = headers.Contains(HttpKnownHeaderNames.Range);
            bool isRefererSet = headers.Contains(HttpKnownHeaderNames.Referer);
            bool isUserAgentSet = headers.Contains(HttpKnownHeaderNames.UserAgent);

            if (isRangeSet)
            {
                RangeHeaderValue range = headers.Range;
                if (range != null)
                {
                    foreach (var rangeItem in range.Ranges)
                    {
                        webRequest.AddRange((long)rangeItem.From, (long)rangeItem.To);
                    }
                }
            }

            if (isRefererSet)
            {
                Uri referer = headers.Referrer;
                if (referer != null)
                {
                    webRequest.Referer = referer.OriginalString;
                }
            }

            if (isAcceptSet && (headers.Accept.Count > 0))
            {
                webRequest.Accept = headers.Accept.ToString();
            }

            if (isUserAgentSet && headers.UserAgent.Count > 0)
            {
                webRequest.UserAgent = headers.UserAgent.ToString();
            }
#endif
            if (isHostSet)
            {
                string host = headers.Host;
                if (host != null)
                {
                    webRequest.Host = host;
                }
            }

            // The following headers (Expect, Transfer-Encoding, Connection) have both a collection property and a 
            // bool property indicating a special value. Internally (in HttpHeaders) we don't distinguish between 
            // "special" values and other values. So we must make sure that we add all but the special value to HWR.
            // E.g. the 'Transfer-Encoding: chunked' value must be set using HWR.SendChunked, whereas all other values
            // can be added to the 'Transfer-Encoding'.
            //            if (isExpectSet)
            //            {
            //                string expectHeader = headers.Expect.GetHeaderStringWithoutSpecial();
            //                // Was at least one non-special value set?
            //                if (!String.IsNullOrEmpty(expectHeader) || !headers.Expect.IsSpecialValueSet)
            //                {
            //#if NET_4
            //                    webRequest.Expect = expectHeader;
            //#else
            //                    webRequestHeaders.AddInternal(HttpKnownHeaderNames.Expect, expectHeader);
            //#endif
            //                }
            //            }

            //            if (isTransferEncodingSet)
            //            {
            //                string transferEncodingHeader = headers.TransferEncoding.GetHeaderStringWithoutSpecial();
            //                // Was at least one non-special value set?
            //                if (!String.IsNullOrEmpty(transferEncodingHeader) || !headers.TransferEncoding.IsSpecialValueSet)
            //                {
            //#if NET_4
            //                    // Setting SendChunked to true just to set the TransferEncoding header value
            //                    // Actual value for SendChunked will be set later on.
            //                    webRequest.SendChunked = true;
            //                    webRequest.TransferEncoding = transferEncodingHeader;
            //                    webRequest.SendChunked = false;
            //#else
            //                    webRequestHeaders.AddInternal(HttpKnownHeaderNames.TransferEncoding, transferEncodingHeader);
            //#endif
            //                }
            //            }

            if (isConnectionSet)
            {
#if NET_4
                // Both Close and Keep-Alive are considered special values and cannot be set directly on Connection.
                // Both values must be ignored and will be set later on.
                string connectionHeader = string.Join(", ", headers.Connection.Where(
                    item => string.Compare(item, HttpKnownHeaderNames.KeepAlive, StringComparison.OrdinalIgnoreCase) != 0 &&
                            string.Compare(item, HeaderUtilities.ConnectionClose, StringComparison.OrdinalIgnoreCase) != 0)
                    );
#else
                //string connectionHeader = headers.Connection.GetHeaderStringWithoutSpecial();
#endif
                // Was at least one non-special value set?
                //                if (!String.IsNullOrEmpty(connectionHeader) || !headers.Connection.IsSpecialValueSet)
                //                {
                //#if NET_4
                //                    webRequest.Connection = connectionHeader;
                //#else
                //                    webRequestHeaders.AddInternal(HttpKnownHeaderNames.Connection, connectionHeader);
                //#endif
                //                }
            }

            var methodInfo = request.Headers.GetType().GetMethod("GetHeaderStrings", INVOKEFLAGS);
            var stringHeaders = methodInfo.Invoke(request.Headers, new object[] { }) as IEnumerable<KeyValuePair<string, string>>;
            foreach (var header in stringHeaders)
            {
                string headerName = header.Key;

                if ((isHostSet && AreEqual("Host", headerName)) ||
                    (isExpectSet && AreEqual("Expect", headerName)) ||
                    (isTransferEncodingSet && AreEqual("Transfer-Encoding", headerName)) ||
#if NET_4
             (isAcceptSet && AreEqual(HttpKnownHeaderNames.Accept, headerName)) ||
                                (isRangeSet && AreEqual(HttpKnownHeaderNames.Range, headerName)) ||
                                (isRefererSet && AreEqual(HttpKnownHeaderNames.Referer, headerName)) ||
                                (isUserAgentSet && AreEqual(HttpKnownHeaderNames.UserAgent, headerName)) ||
#endif
             (isConnectionSet && AreEqual("Connection", headerName)))
                {
                    continue; // Header was already added.
                }

#if NET_4
                            webRequestHeaders.Add(header.Key, header.Value);
#else
                // Use AddInternal() to skip validation.
                webRequestHeaders.Add(header.Key, header.Value);
#endif
            }
        }

        private static void SetContentHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            if (request.Content != null)
            {
                HttpContentHeaders headers = request.Content.Headers;

                // All content headers besides Content-Length can be added directly to HWR. So just check whether we 
                // have the Content-Length header set. If not, add all headers, otherwise skip the Content-Length 
                // header.
                // Note that this method is called _before_ PrepareWebRequestForContentUpload(): I.e. in most scenarios
                // this means that no one accessed Headers.ContentLength property yet, thus there will be no 
                // Content-Length header in the store. I.e. we'll end up in the 'else' block providing better perf, 
                // since no string comparison is required.
                if (headers.Contains("Content-Length"))
                {
                    foreach (var header in request.Content.Headers)
                    {
                        if (string.Compare("Content-Length", header.Key, StringComparison.OrdinalIgnoreCase) != 0)
                        {
#if NET_4
                            SetContentHeader(webRequest, header);
#else
                            // Use AddInternal() to skip validation.
                            webRequest.Headers.Add(header.Key, string.Join(", ", header.Value));
#endif
                        }
                    }
                }
                else
                {
                    foreach (var header in request.Content.Headers)
                    {
#if NET_4
                        SetContentHeader(webRequest, header);
#else
                        // Use AddInternal() to skip validation.
                        webRequest.Headers.Add(header.Key, string.Join(", ", header.Value));
#endif
                    }
                }
            }
        }
#if NET_4
        private static void SetContentHeader(HttpWebRequest webRequest, KeyValuePair<string, IEnumerable<string>> header)
        {
            if (string.Compare(HttpKnownHeaderNames.ContentType, header.Key, StringComparison.OrdinalIgnoreCase) == 0)
            {
                webRequest.ContentType = string.Join(", ", header.Value);
            }
            else
            {
                webRequest.Headers.Add(header.Key, string.Join(", ", header.Value));
            }
        }
#endif

        #endregion Message Setup

        #region Request Processing


        private void StartRequest(object obj)
        {
            RequestState state = obj as RequestState;
            Contract.Assert(state != null);

            try
            {
                if (state.requestMessage.Content != null)
                {
                    PrepareAndStartContentUpload(state);
                }
                else
                {
                    state.webRequest.ContentLength = 0;
                    StartGettingResponse(state);
                }
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }
        }

        private void PrepareAndStartContentUpload(RequestState state)
        {
            HttpContent requestContent = state.requestMessage.Content;
            Contract.Assert(requestContent != null);

            try
            {
                // Determine how to communicate the length of the request content.
                if (state.requestMessage.Headers.TransferEncodingChunked == true)
                {
                    state.webRequest.SendChunked = true;
                    StartGettingRequestStream(state);
                }
                else
                {
                    long? contentLength = requestContent.Headers.ContentLength;
                    if (contentLength != null)
                    {
                        state.webRequest.ContentLength = (long)contentLength;
                        StartGettingRequestStream(state);
                    }
                    else
                    {
                        // If we don't have a content length and we don't use chunked, then we must buffer the content.
                        // If the user specified a zero buffer size, we throw.
                        if (_maxRequestContentBufferSize == 0)
                        {
                            //throw new HttpRequestException(SR.net_http_handler_nocontentlength);
                        }

                        // HttpContent couldn't calculate the content length. Chunked is not specified. Buffer the 
                        // content to get the content length.
                        //requestContent.LoadIntoBufferAsync(_maxRequestContentBufferSize).ContinueWithStandard(task =>
                        //{
                        //    if (task.IsFaulted)
                        //    {
                        //        HandleAsyncException(state, task.Exception.GetBaseException());
                        //        return;
                        //    }

                        //    try
                        //    {
                        //        contentLength = requestContent.Headers.ContentLength;
                        //        Contract.Assert(contentLength != null, "After buffering content, ContentLength must not be null.");
                        //        state.webRequest.ContentLength = (long)contentLength;
                        //        StartGettingRequestStream(state);
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        HandleAsyncException(state, e);
                        //    }
                        //});
                    }
                }
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }
        }

        private void StartGettingRequestStream(RequestState state)
        {
            // Manually flow identity context if captured.
            if (state.identity != null)
            {
                using (state.identity.Impersonate())
                {
                    state.webRequest.BeginGetRequestStream(_getRequestStreamCallback, state);
                }
            }
            else
            {
                state.webRequest.BeginGetRequestStream(_getRequestStreamCallback, state);
            }
        }

        private void GetRequestStreamCallback(IAsyncResult ar)
        {
            RequestState state = ar.AsyncState as RequestState;
            Contract.Assert(state != null);

            try
            {
                TransportContext context = null;
                Stream requestStream = state.webRequest.EndGetRequestStream(ar, out context) as Stream;
                state.requestStream = requestStream;
                //state.requestMessage.Content.CopyToAsync(requestStream, context).ContinueWithStandard(task =>
                //{
                //    try
                //    {
                //        if (task.IsFaulted)
                //        {
                //            HandleAsyncException(state, task.Exception.GetBaseException());
                //            return;
                //        }

                //        if (task.IsCanceled)
                //        {
                //            state.tcs.TrySetCanceled(state.cancellationToken);
                //            return;
                //        }

                //        state.requestStream.Close();
                //        StartGettingResponse(state);
                //    }
                //    catch (Exception e)
                //    {
                //        HandleAsyncException(state, e);
                //    }

                //});
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }
        }

        private void StartGettingResponse(RequestState state)
        {
            // Manually flow identity context if captured.
            if (state.identity != null)
            {
                using (state.identity.Impersonate())
                {
                    state.webRequest.BeginGetResponse(_getResponseCallback, state);
                }
            }
            else
            {
                state.webRequest.BeginGetResponse(_getResponseCallback, state);
            }
#if !NET_4
            //state.requestMessage.MarkRtcFlushComplete();
#endif
        }

        private void GetResponseCallback(IAsyncResult ar)
        {
            RequestState state = ar.AsyncState as RequestState;
            Contract.Assert(state != null);

            try
            {
                HttpWebResponse webResponse = state.webRequest.EndGetResponse(ar) as HttpWebResponse;
                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    //state.requestMessage.RequestUri = new Uri("https://graph.microsoft.com/v1.0/sites/m365x157144-my.sharepoint.com,47b37a14-09dd-407f-9509-6fa9b4ad20d4,08d6db6f-5165-4fd4-ad22-7a61201f766a/lists/a44cb590-eb05-45d4-bf28-30f73385cd3e/items/27/driveItem/permissions");
                    HttpWebRequest webRequest = CreateAndPrepareWebRequest(state.requestMessage);
                    state.webRequest = webRequest;

                    //var task =   SendAsync(state.requestMessage, state.cancellationToken);
                    webResponse = state.webRequest.GetResponse() as HttpWebResponse;
                    state.tcs.TrySetResult(CreateResponseMessage(webResponse, state.requestMessage));
                }
                else
                {
                    state.tcs.TrySetResult(CreateResponseMessage(webResponse, state.requestMessage));
                }
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }
        }

#if NET_4
        private bool TryGetExceptionResponse(WebException webException, HttpRequestMessage requestMessage, out HttpResponseMessage httpResponseMessage)
        {
            if (webException != null && webException.Response != null)
            {
                HttpWebResponse webResponse = webException.Response as HttpWebResponse;
                if (webResponse != null)
                {
                    httpResponseMessage = CreateResponseMessage(webResponse, requestMessage);
                    return true;
                }
            }
            httpResponseMessage = null;
            return false;
        }
#endif
        private HttpResponseMessage CreateResponseMessage(HttpWebResponse webResponse, HttpRequestMessage request)
        {
            HttpResponseMessage response = new HttpResponseMessage(webResponse.StatusCode);
            response.ReasonPhrase = webResponse.StatusDescription;
            response.Version = webResponse.ProtocolVersion;
            response.RequestMessage = request;
            response.Content = new StreamContent(new WebExceptionWrapperStream(webResponse.GetResponseStream()));

            // Update Request-URI to reflect the URI actually leading to the response message.
            request.RequestUri = webResponse.ResponseUri;

            WebHeaderCollection webResponseHeaders = webResponse.Headers;
            HttpContentHeaders contentHeaders = response.Content.Headers;
            HttpResponseHeaders responseHeaders = response.Headers;

            // HttpWebResponse.ContentLength is set to -1 if no Content-Length header is provided.
            if (webResponse.ContentLength >= 0)
            {
                contentHeaders.ContentLength = webResponse.ContentLength;
            }

            for (int i = 0; i < webResponseHeaders.Count; i++)
            {
                string currentHeader = webResponseHeaders.GetKey(i);

                // We already set Content-Length
                if (string.Compare(currentHeader, "Content-Length",
                    StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continue;
                }

                string[] values = webResponseHeaders.GetValues(i);

                if (!responseHeaders.TryAddWithoutValidation(currentHeader, values))
                {
                    bool result = contentHeaders.TryAddWithoutValidation(currentHeader, values);
                    // WebHeaderCollection should never return us invalid header names.
                    Contract.Assert(result, "Invalid header name.");
                }
            }
            bool exception = false;
            if (exception)
            {
                throw new Exception();
            }
            return response;
        }

        private void HandleAsyncException(RequestState state, Exception e)
        {
            // Use 'SendAsync' as method name, since this method is only called by methods in the async code path. Using
            // 'SendAsync' as method name helps relate the exception to the operation in log files.
            //if (Logging.On) Logging.Exception(Logging.Http, this, "SendAsync", e);
#if NET_4
            HttpResponseMessage responseMessage;
            if (TryGetExceptionResponse(e as WebException, state.requestMessage, out responseMessage))
            {
                state.tcs.TrySetResult(responseMessage);
            }
            else
#endif
            // If the WebException was due to the cancellation token being canceled, throw cancellation exception.
            if (state.cancellationToken.IsCancellationRequested)
            {
                //state.tcs.TrySetCanceled(state.cancellationToken);
            }
            // Wrap expected exceptions as HttpRequestExceptions since this is considered an error during 
            // execution. All other exception types, including ArgumentExceptions and ProtocolViolationExceptions
            // are 'unexpected' or caused by user error and should not be wrapped.
            else if (e is WebException || e is IOException)
            {
                state.tcs.TrySetException(new HttpRequestException(e.ToString()));
            }
            else
            {
                state.tcs.TrySetException(e);
            }
#if !NET_4
            //state.requestMessage.AbortRtcRequest();
#endif
        }

        private static void OnCancel(object state)
        {
            HttpWebRequest webRequest = state as HttpWebRequest;
            Contract.Assert(webRequest != null);

            webRequest.Abort();
        }

        #endregion Request Processing

        #region Helpers

        private void SetOperationStarted()
        {
            if (!_operationStarted)
            {
                _operationStarted = true;
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        internal void CheckDisposedOrStarted()
        {
            CheckDisposed();
            if (_operationStarted)
            {
                //throw new InvalidOperationException(SR.net_http_operation_started);
            }
        }

        private static bool AreEqual(string x, string y)
        {
            return (string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0);
        }

        // Security: We need an assert for a call into WindowsIdentity.GetCurrent
        [SecuritySafeCritical]
        [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.ControlPrincipal)]
        [SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "Needed for identity flow.")]
        private void SafeCaptureIdenity(RequestState state)
        {
            state.identity = WindowsIdentity.GetCurrent();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                //throw new ArgumentNullException("request", SR.net_http_handler_norequest);
            }
            CheckDisposed();

            //if (Logging.On) Logging.Enter(Logging.Http, this, "SendAsync", request);

            SetOperationStarted();

            TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
            RequestState state = new RequestState();
            state.tcs = tcs;
            state.cancellationToken = cancellationToken;
            state.requestMessage = request;
#if NET_4
            _lastUsedRequestUri = request.RequestUri;
#endif
            try
            {
                // Cancellation: Note that there is no race here: If the token gets canceled before we register the
                // callback, the token will invoke the callback immediately. I.e. HWR gets aborted before we use it.
                HttpWebRequest webRequest = CreateAndPrepareWebRequest(request);
                state.webRequest = webRequest;
                cancellationToken.Register(s_onCancel, webRequest);

                // Preserve context for authentication
                if (ExecutionContext.IsFlowSuppressed())
                {
                    // Check for proxy auth
                    IWebProxy currentProxy = null;
                    if (_useProxy)
                    {
                        currentProxy = _proxy ?? WebRequest.DefaultWebProxy;
                    }

                    if ((UseDefaultCredentials || Credentials != null
                        || (currentProxy != null && currentProxy.Credentials != null)))
                    {
                        SafeCaptureIdenity(state);
                    }
                }

                // BeginGetResponse/BeginGetRequestStream have a lot of setup work to do before becoming async
                // (proxy, dns, connection pooling, etc).  Run these on a separate thread.
                // Do not provide a cancellation token; if this helper task could be canceled before starting then 
                // nobody would complete the tcs.
                Task.Run(() => { _startRequest(state); });
            }
            catch (Exception e)
            {
                HandleAsyncException(state, e);
            }

            //if (Logging.On) Logging.Exit(Logging.Http, this, "SendAsync", tcs.Task);
            return tcs.Task;
        }

        #endregion Helpers

        // Adapted from: https://github.com/dotnet/corefx/blob/master/src/Common/src/System/Net/SecurityProtocol.cs
        private static class SecurityProtocol
        {
            // SSLv2 and SSLv3 are considered insecure and will not be supported by the underlying implementations.
            internal const SslProtocols AllowedSecurityProtocols =
                SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;// | SslProtocols.Tls13;

            internal const SslProtocols DefaultSecurityProtocols =
                SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;// | SslProtocols.Tls13;

            internal const SslProtocols SystemDefaultSecurityProtocols = SslProtocols.None;

            internal static void ThrowOnNotAllowed(SslProtocols protocols, bool allowNone = true)
            {
                if ((!allowNone && (protocols == SslProtocols.None)) || ((protocols & ~AllowedSecurityProtocols) != 0))
                {
                    //throw new NotSupportedException(SR.net_http_securityprotocolnotsupported);
                }
            }
        }

        private class RequestState
        {
            internal HttpWebRequest webRequest;
            internal TaskCompletionSource<HttpResponseMessage> tcs;
            internal CancellationToken cancellationToken;
            internal HttpRequestMessage requestMessage;
            internal Stream requestStream;
            internal WindowsIdentity identity;
        }

        // The ConnectStream returned by HttpWebResponse may throw a WebException when aborted. Wrap them in 
        // IOExceptions. The ConnectStream will be read-only so we don't need to wrap the write methods.
        private class WebExceptionWrapperStream : DelegatingStream
        {
            internal WebExceptionWrapperStream(Stream innerStream)
                : base(innerStream)
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                try
                {
                    return base.Read(buffer, offset, count);
                }
                catch (WebException wex)
                {
                    //throw new IOException(SR.net_http_read_error, wex);
                }
                throw new IOException();
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                try
                {
                    return base.BeginRead(buffer, offset, count, callback, state);
                }
                catch (WebException wex)
                {
                    //throw new IOException(SR.net_http_read_error, wex);
                }
                throw new IOException();
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                try
                {
                    return base.EndRead(asyncResult);
                }
                catch (WebException wex)
                {
                    //throw new IOException(SR.net_http_read_error, wex);
                }
                throw new IOException();
            }
#if !NET_4
            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
                CancellationToken cancellationToken)
            {
                try
                {
                    return await base.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
                }
                catch (WebException wex)
                {
                    //throw new IOException(SR.net_http_read_error, wex);
                }
                throw new IOException();
            }
#endif
            public override int ReadByte()
            {
                try
                {
                    return base.ReadByte();
                }
                catch (WebException wex)
                {
                    //throw new IOException(SR.net_http_read_error, wex);
                }
                throw new IOException();
            }
        }
    }

    internal abstract class DelegatingStream : Stream
    {
        private Stream innerStream;

        #region Properties

        public override bool CanRead
        {
            get { return innerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return innerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return innerStream.CanWrite; }
        }

        public override long Length
        {
            get { return innerStream.Length; }
        }

        public override long Position
        {
            get { return innerStream.Position; }
            set { innerStream.Position = value; }
        }

        public override int ReadTimeout
        {
            get { return innerStream.ReadTimeout; }
            set { innerStream.ReadTimeout = value; }
        }

        public override bool CanTimeout
        {
            get { return innerStream.CanTimeout; }
        }

        public override int WriteTimeout
        {
            get { return innerStream.WriteTimeout; }
            set { innerStream.WriteTimeout = value; }
        }

        #endregion Properties

        protected DelegatingStream(Stream innerStream)
        {
            Contract.Assert(innerStream != null);
            this.innerStream = innerStream;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                innerStream.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Read

        public override long Seek(long offset, SeekOrigin origin)
        {
            return innerStream.Seek(offset, origin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return innerStream.Read(buffer, offset, count);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback,
            object state)
        {
            return innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return innerStream.EndRead(asyncResult);
        }

        public override int ReadByte()
        {
            return innerStream.ReadByte();
        }
#if !NET_4
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return innerStream.ReadAsync(buffer, offset, count, cancellationToken);
        }
#endif
        #endregion Read

        #region Write

        public override void Flush()
        {
            innerStream.Flush();
        }
#if !NET_4
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return innerStream.FlushAsync(cancellationToken);
        }
#endif
        public override void SetLength(long value)
        {
            innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            innerStream.Write(buffer, offset, count);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback,
            object state)
        {
            return innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            innerStream.EndWrite(asyncResult);
        }

        public override void WriteByte(byte value)
        {
            innerStream.WriteByte(value);
        }
#if !NET_4
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        }
#endif
        #endregion Write
    }
}
