using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Beatecho
{
    class FTPClient
    {
        private string _host;
        private NetworkCredential _credentials;
        private WebClient _wc;

        public FTPClient(string host, string username, string password)
        {
            if (!host.StartsWith("http://") && !host.StartsWith("https://") && !host.StartsWith("ftp://"))
            {
                host = "https://" + host;
            }

            _host = host;
            _credentials = new(username, password);

            _wc = new()
            {
                BaseAddress = _host,
                Credentials = _credentials
            };
        }

        public bool UploadFile(string localFile, string remoteFile)
        {
            try
            {
                _wc.UploadFile(remoteFile, localFile);
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private WebRequest CreateRequest(string path, string method)
        {
            var req = WebRequest.Create(_host + path);
            req.Method = method;
            req.Credentials = _credentials;
            return req;
        }

        public bool DownloadFile(string remoteFile, string localFile)
        {
            try
            {
                _wc.DownloadFile(remoteFile, localFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateDirectory(string remoteDir)
        {
            try
            {
                var req = CreateRequest(remoteDir, WebRequestMethods.Ftp.MakeDirectory);
                var resp = req.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetFiles(string remoteDir)
        {
            try
            {
                var req = CreateRequest(remoteDir, WebRequestMethods.Ftp.ListDirectory);
                var resp = req.GetResponse();
                var sr = new StreamReader(resp.GetResponseStream());
                var l = new List<string>();

                string s = null;
                while (true)
                {
                    if (string.IsNullOrEmpty(s = sr.ReadLine()))
                    {
                        break;
                    }
                    l.Add(s);
                }

                sr.Close();

                return l;
            }
            catch
            {
                return null;
            }
        }
    }
}
