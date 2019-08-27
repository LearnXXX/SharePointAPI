using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class ProxyInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="address">ip:port</param>
        public ProxyInfo(string username, string password, string address)
        {
            if (!string.IsNullOrEmpty(username))
            {
                this.Username = username;
            }
            if (!string.IsNullOrEmpty(password))
            {
                this.Password = password;
            }
            this.Address = address;
        }


        public string Username { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }
    }
}
