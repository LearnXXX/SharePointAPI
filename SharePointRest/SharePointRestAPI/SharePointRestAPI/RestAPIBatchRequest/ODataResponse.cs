﻿using Microsoft.Data.OData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharePointRestAPI
{

    public class ODataResponse : IODataResponseMessage
    {
        private readonly HttpWebResponse webResponse;

        public ODataResponse(HttpWebResponse webResponse)
        {
            if (webResponse == null)
            {
                throw new ArgumentNullException("webResponse");
            }

            this.webResponse = webResponse;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.webResponse.Headers.AllKeys.Select(headerName =>
                    new KeyValuePair<string, string>(headerName, this.webResponse.Headers.Get(headerName)));
            }
        }

        public int StatusCode
        {
            get
            {
                return (int)this.webResponse.StatusCode;
            }
            set
            {
                throw new InvalidOperationException("The HTTP response is read-only, status code can't be modified on it.");
            }
        }

        public string GetHeader(string headerName)
        {
            if (string.IsNullOrEmpty(headerName))
            {
                throw new ArgumentException(headerName + " is not a valid header name.");
            }

            return this.webResponse.Headers.Get(headerName);
        }



        public Stream GetStream()
        {
            return this.webResponse.GetResponseStream();
        }


        public void SetHeader(string headerName, string headerValue)
        {
            throw new InvalidOperationException("The HTTP response is read-only, headers can't be modified on it.");
        }


    }
}
