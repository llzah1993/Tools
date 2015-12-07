using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Cache;
using System.IO;

namespace LTUnityPlugin.WebClient {

/// <summary>
/// Extend copy from .Net Core WebClient
/// Add async resume download, link timeout, read timeout and other diy function.
/// By Hxs1990.
/// Create 2015.06.18
/// Last Update 2015.06.20
/// </summary>

public class HttpWebClient {
    static readonly string downLoadTmpFile = ".lttmp";
    static readonly string urlEncodedCType = "application/x-www-form-urlencoded";
    static byte[] hexBytes;
    ICredentials credentials;
    WebHeaderCollection headers;
    WebHeaderCollection responseHeaders;
    Uri baseAddress;
    string baseString;
    NameValueCollection queryString;
    bool is_busy;
    bool async;
    private bool isBreakpoint = false;
    Thread async_thread;
    Encoding encoding = Encoding.Default;
    IWebProxy proxy;
    private int timeOut = 30000;
    private int readTimeOut = 30000;
    HttpWebClientCoroutine coroutine = new HttpWebClientCoroutine();

    // Constructors
    static HttpWebClient() {
        hexBytes = new byte [16];
        int index = 0;
        for(int i = '0'; i <= '9'; i++, index++)
            hexBytes [index] = (byte) i;

        for(int i = 'a'; i <= 'f'; i++, index++)
            hexBytes [index] = (byte) i;
    }

    public HttpWebClient() {
    }

    public string BaseAddress {
        get {
            if(baseString == null) {
                if(baseAddress == null)
                    return string.Empty;
            }

            baseString = baseAddress.ToString();
            return baseString;
        }

        set {
            if(value == null || value.Length == 0) {
                baseAddress = null;
            } else {
                baseAddress = new Uri(value);
            }
        }
    }

    public System.Collections.IEnumerator Coroutine {
        get { return coroutine.GetEnumerator(); }
    }

    static Exception GetMustImplement() {
        return new NotImplementedException();
    }

    public RequestCachePolicy CachePolicy {
        get {
            throw GetMustImplement();
        } set {
            throw GetMustImplement();
        }
    }

    public bool UseDefaultCredentials {
        get {
            throw GetMustImplement();
        } set {
            throw GetMustImplement();
        }
    }

    public ICredentials Credentials {
        get { return credentials; }
        set { credentials = value; }
    }

    public WebHeaderCollection Headers {
        get {
            if(headers == null)
                headers = new WebHeaderCollection();

            return headers;
        } set { headers = value; }
    }

    public NameValueCollection QueryString {
        get {
            if(queryString == null)
                queryString = new NameValueCollection();

            return queryString;
        } set { queryString = value; }
    }

    public WebHeaderCollection ResponseHeaders {
        get { return responseHeaders; }
    }

    public Encoding Encoding {
        get { return encoding; }
        set {
            if(value == null)
                throw new ArgumentNullException("Encoding");
            encoding = value;
        }
    }

    public IWebProxy Proxy {
        get { return proxy; }
        set { proxy = value; }
    }
    public bool IsBusy {
        get { return is_busy; }
    }

    public bool IsBreakpoint {
        get { return isBreakpoint; }
        set { isBreakpoint = value; }
    }

    public int TimeOut {
        get { return timeOut; }
        set { timeOut = value; }
    }

    public int ReadTimeOut {
        get { return readTimeOut; }
        set { readTimeOut = value; }
    }

    void CheckBusy() {
        if(IsBusy)
            throw new NotSupportedException("WebClient does not support conccurent I/O operations.");
    }

    void SetBusy() {
        lock(this) {
            CheckBusy();
            is_busy = true;
        }
    }

    public byte [] DownloadData(string address) {
        if(address == null)
            throw new ArgumentNullException("address");
        return DownloadData(CreateUri(address));
    }

    public byte [] DownloadData(Uri address) {
        if(address == null)
            throw new ArgumentNullException("address");

        try {
            SetBusy();
            async = false;
            return DownloadDataCore(address, null);
        } finally {
            is_busy = false;
            coroutine.Complete();
        }
    }

    byte [] DownloadDataCore(Uri address, object userToken) {
        WebRequest request = null;
        try {
            request = SetupRequest(address);
            WebResponse response = GetWebResponse(request);
            Stream st = response.GetResponseStream();
            st.ReadTimeout = ReadTimeOut;
            return ReadAll(st, (int) response.ContentLength, userToken);
        } catch(ThreadInterruptedException) {
            if(request != null)
                request.Abort();
            throw;
        } catch(WebException wexc) {
            if(wexc != null) { }
            throw;
        } catch(Exception ex) {
            throw new WebException("An error occurred " +
                                   "performing a WebClient request.", ex);
        }
    }

    //   DownloadFile

    public void DownloadFile(string address, string fileName) {
        if(address == null)
            throw new ArgumentNullException("address");
        DownloadFile(CreateUri(address), fileName);
    }

    public void DownloadFile(Uri address, string fileName) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(fileName == null)
            throw new ArgumentNullException("fileName");
        try {
            SetBusy();
            async = false;
            DownloadFileCoreWithResume(address, fileName, null);
        } catch(WebException wexc) {
            if(wexc != null) { }
            throw;
        } catch(Exception ex) {
            throw new WebException("An error occurred " + "performing a WebClient request.", ex);
        } finally {
            is_busy = false;
            coroutine.Complete();
        }
    }

    void DownloadFileCoreWithResume(Uri address, string fileName, object userToken) {
        fileName += downLoadTmpFile;
        WebRequest request = null;
        using(FileStream f = IsBreakpoint ? new FileStream(fileName, FileMode.Append) : new FileStream(fileName, FileMode.Create)) {
            try {
                request = SetupRequest(address);
                if(IsBreakpoint) {
                    f.Seek(0, SeekOrigin.End);
                    if(request is HttpWebRequest) {
                        // set http header range with current filestream position.
                        ((HttpWebRequest)request).AddRange((int)f.Position);
                    }
                }
                WebResponse response = GetWebResponse(request);
                Stream st = response.GetResponseStream();
                st.ReadTimeout = ReadTimeOut;
                int cLength = (int)response.ContentLength;
                int length = (cLength <= -1 || cLength > 32 * 1024) ? 32 * 1024 : cLength;
                byte[] buffer = new byte[length];

                int nread = 0;
                long notify_total = 0;
                while((nread = st.Read(buffer, 0, length)) != 0) {
                    if(async) {
                        notify_total += nread;
                        OnDownloadProgressChanged(
                            new HttpDownloadProgressChangedEventArgs(notify_total, response.ContentLength, userToken));
                    }
                    f.Write(buffer, 0, nread);
                }
            } catch(ThreadInterruptedException) {
                if(request != null)
                    request.Abort();
                throw;
            } finally {
                if(f != null) {
                    f.Flush();
                    f.Close();
                }
            }
        }
    }

    void DownloadFileCore(Uri address, string fileName, object userToken) {
        WebRequest request = null;
        using(FileStream f = new FileStream(fileName, FileMode.Create)) {
            try {
                request = SetupRequest(address);

                WebResponse response = GetWebResponse(request);
                Stream st = response.GetResponseStream();

                int cLength = (int) response.ContentLength;
                int length = (cLength <= -1 || cLength > 32 * 1024) ? 32 * 1024 : cLength;
                byte [] buffer = new byte [length];

                int nread = 0;
                long notify_total = 0;
                while((nread = st.Read(buffer, 0, length)) != 0) {
                    if(async) {
                        notify_total += nread;
                        OnDownloadProgressChanged(
                            new HttpDownloadProgressChangedEventArgs(notify_total, response.ContentLength, userToken));

                    }
                    f.Write(buffer, 0, nread);
                }
            } catch(ThreadInterruptedException) {
                if(request != null)
                    request.Abort();
                throw;
            }
        }
    }

    public Stream OpenRead(string address) {
        if(address == null)
            throw new ArgumentNullException("address");
        return OpenRead(CreateUri(address));
    }

    public Stream OpenRead(Uri address) {
        if(address == null)
            throw new ArgumentNullException("address");
        WebRequest request = null;
        try {
            SetBusy();
            async = false;
            request = SetupRequest(address);
            WebResponse response = GetWebResponse(request);
            return response.GetResponseStream();
        } catch(WebException wexc) {
            if(wexc != null) { }
            throw;
        } catch(Exception ex) {
            throw new WebException("An error occurred " +
                                   "performing a WebClient request.", ex);
        } finally {
            is_busy = false;
            coroutine.Complete();
        }
    }

    //   OpenWrite

    public Stream OpenWrite(string address) {
        if(address == null)
            throw new ArgumentNullException("address");
        return OpenWrite(CreateUri(address));
    }

    public Stream OpenWrite(string address, string method) {
        if(address == null)
            throw new ArgumentNullException("address");

        return OpenWrite(CreateUri(address), method);
    }
    public Stream OpenWrite(Uri address) {
        return OpenWrite(address, (string) null);
    }

    public Stream OpenWrite(Uri address, string method) {
        if(address == null)
            throw new ArgumentNullException("address");
        try {
            SetBusy();
            async = false;
            WebRequest request = SetupRequest(address, method, true);
            return request.GetRequestStream();
        } catch(WebException wexc) {
            if(wexc != null) { }
            throw;
        } catch(Exception ex) {
            throw new WebException("An error occurred " +
                                   "performing a WebClient request.", ex);
        } finally {
            is_busy = false;
            coroutine.Complete();
        }
    }

    private string DetermineMethod(Uri address, string method, bool is_upload) {
        if(method != null)
            return method;
        if(address.Scheme == Uri.UriSchemeFtp)
            return (is_upload) ? "STOR" : "RETR";
        return (is_upload) ? "POST" : "GET";
    }

    //   UploadData

    public byte [] UploadData(string address, byte [] data) {
        if(address == null)
            throw new ArgumentNullException("address");
        return UploadData(CreateUri(address), data);
    }

    public byte [] UploadData(string address, string method, byte [] data) {
        if(address == null)
            throw new ArgumentNullException("address");
        return UploadData(CreateUri(address), method, data);
    }

    public byte [] UploadData(Uri address, byte [] data) {
        return UploadData(address, (string) null, data);
    }

    public byte [] UploadData(Uri address, string method, byte [] data) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(data == null)
            throw new ArgumentNullException("data");
        try {
            SetBusy();
            async = false;
            return UploadDataCore(address, method, data, null);
        } catch(WebException) {
            throw;
        } catch(Exception ex) {
            throw new WebException("An error occurred " +
                                   "performing a WebClient request.", ex);
        } finally {
            is_busy = false;
            coroutine.Complete();
        }
    }

    byte [] UploadDataCore(Uri address, string method, byte [] data, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(data == null)
            throw new ArgumentNullException("data");
        WebRequest request = SetupRequest(address, method, true);
        try {
            int contentLength = data.Length;
            request.ContentLength = contentLength;
            using(Stream stream = request.GetRequestStream()) {
                stream.Write(data, 0, contentLength);
            }

            WebResponse response = GetWebResponse(request);
            Stream st = response.GetResponseStream();
            return ReadAll(st, (int) response.ContentLength, userToken);
        } catch(ThreadInterruptedException) {
            if(request != null)
                request.Abort();
            throw;
        }
    }

    //   UploadFile

    public byte [] UploadFile(string address, string fileName) {
        if(address == null)
            throw new ArgumentNullException("address");
        return UploadFile(CreateUri(address), fileName);
    }

    public byte [] UploadFile(Uri address, string fileName) {
        return UploadFile(address, (string) null, fileName);
    }

    public byte [] UploadFile(string address, string method, string fileName) {
        return UploadFile(CreateUri(address), method, fileName);
    }

    public byte [] UploadFile(Uri address, string method, string fileName) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(fileName == null)
            throw new ArgumentNullException("fileName");
        try {
            SetBusy();
            async = false;
            return UploadFileCore(address, method, fileName, null);
        } catch(WebException wexc) {
            if(wexc != null) {
            }
            throw;
        } catch(Exception ex) {
            throw new WebException("An error occurred " +
                                   "performing a WebClient request.", ex);
        } finally {
            is_busy = false;
            coroutine.Complete();
        }
    }

    byte [] UploadFileCore(Uri address, string method, string fileName, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");

        string fileCType = Headers ["Content-Type"];
        if(fileCType != null) {
            string lower = fileCType.ToLower();
            if(lower.StartsWith("multipart/"))
                throw new WebException("Content-Type cannot be set to a multipart" +
                                       " type for this request.");
        } else {
            fileCType = "application/octet-stream";
        }

        string boundary = "------------" + DateTime.Now.Ticks.ToString("x");
        Headers ["Content-Type"] = String.Format("multipart/form-data; boundary={0}", boundary);
        Stream reqStream = null;
        Stream fStream = null;
        byte [] resultBytes = null;

        fileName = Path.GetFullPath(fileName);

        WebRequest request = null;
        try {
            fStream = File.OpenRead(fileName);
            request = SetupRequest(address, method, true);
            reqStream = request.GetRequestStream();
            byte [] realBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            reqStream.Write(realBoundary, 0, realBoundary.Length);
            string partHeaders = String.Format("Content-Disposition: form-data; " +
                                               "name=\"file\"; filename=\"{0}\"\r\n" +
                                               "Content-Type: {1}\r\n\r\n",
                                               Path.GetFileName(fileName), fileCType);

            byte [] partHeadersBytes = Encoding.UTF8.GetBytes(partHeaders);
            reqStream.Write(partHeadersBytes, 0, partHeadersBytes.Length);
            int nread;
            byte [] buffer = new byte [4096];
            while((nread = fStream.Read(buffer, 0, 4096)) != 0)
                reqStream.Write(buffer, 0, nread);

            reqStream.WriteByte((byte) '\r');
            reqStream.WriteByte((byte) '\n');
            reqStream.Write(realBoundary, 0, realBoundary.Length);
            reqStream.Close();
            reqStream = null;
            WebResponse response = GetWebResponse(request);
            Stream st = response.GetResponseStream();
            resultBytes = ReadAll(st, (int) response.ContentLength, userToken);
        } catch(ThreadInterruptedException) {
            if(request != null)
                request.Abort();
            throw;
        } finally {
            if(fStream != null)
                fStream.Close();

            if(reqStream != null)
                reqStream.Close();
        }

        return resultBytes;
    }

    public byte[] UploadValues(string address, NameValueCollection data) {
        if(address == null)
            throw new ArgumentNullException("address");
        return UploadValues(CreateUri(address), data);
    }

    public byte[] UploadValues(string address, string method, NameValueCollection data) {
        if(address == null)
            throw new ArgumentNullException("address");
        return UploadValues(CreateUri(address), method, data);
    }
    public byte[] UploadValues(Uri address, NameValueCollection data) {
        return UploadValues(address, (string) null, data);
    }

    public byte[] UploadValues(Uri address, string method, NameValueCollection data) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(data == null)
            throw new ArgumentNullException("data");
        try {
            SetBusy();
            async = false;
            return UploadValuesCore(address, method, data, null);
        } catch(WebException wexc) {
            if(wexc != null) {
            }
            throw;
        } catch(Exception ex) {
            throw new WebException("An error occurred " +
                                   "performing a WebClient request.", ex);
        } finally {
            is_busy = false;
            coroutine.Complete();
        }
    }

    byte[] UploadValuesCore(Uri uri, string method, NameValueCollection data, object userToken) {
        if(data == null)
            throw new ArgumentNullException("data");

        string cType = Headers ["Content-Type"];
        if(cType != null && String.Compare(cType, urlEncodedCType, true) != 0)
            throw new WebException("Content-Type header cannot be changed from its default " +
                                   "value for this request.");

        Headers ["Content-Type"] = urlEncodedCType;
        WebRequest request = SetupRequest(uri, method, true);
        try {
            MemoryStream tmpStream = new MemoryStream();
            foreach(string key in data) {
                byte [] bytes = Encoding.UTF8.GetBytes(key);
                UrlEncodeAndWrite(tmpStream, bytes);
                tmpStream.WriteByte((byte) '=');
                bytes = Encoding.UTF8.GetBytes(data [key]);
                UrlEncodeAndWrite(tmpStream, bytes);
                tmpStream.WriteByte((byte) '&');
            }

            int length = (int) tmpStream.Length;
            if(length > 0)
                tmpStream.SetLength(--length);  // remove trailing '&'

            byte [] buf = tmpStream.GetBuffer();
            request.ContentLength = length;
            using(Stream rqStream = request.GetRequestStream()) {
                rqStream.Write(buf, 0, length);
            }
            tmpStream.Close();

            WebResponse response = GetWebResponse(request);
            Stream st = response.GetResponseStream();
            return ReadAll(st, (int) response.ContentLength, userToken);
        } catch(ThreadInterruptedException) {
            request.Abort();
            throw;
        }
    }

    public string DownloadString(string address) {
        if(address == null)
            throw new ArgumentNullException("address");

        return encoding.GetString(DownloadData(CreateUri(address)));
    }

    public string DownloadString(Uri address) {
        if(address == null)
            throw new ArgumentNullException("address");

        return encoding.GetString(DownloadData(CreateUri(address)));
    }

    public string UploadString(string address, string data) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(data == null)
            throw new ArgumentNullException("data");

        byte [] resp = UploadData(address, encoding.GetBytes(data));
        return encoding.GetString(resp);
    }

    public string UploadString(string address, string method, string data) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(data == null)
            throw new ArgumentNullException("data");

        byte [] resp = UploadData(address, method, encoding.GetBytes(data));
        return encoding.GetString(resp);
    }

    public string UploadString(Uri address, string data) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(data == null)
            throw new ArgumentNullException("data");

        byte [] resp = UploadData(address, encoding.GetBytes(data));
        return encoding.GetString(resp);
    }

    public string UploadString(Uri address, string method, string data) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(data == null)
            throw new ArgumentNullException("data");

        byte [] resp = UploadData(address, method, encoding.GetBytes(data));
        return encoding.GetString(resp);
    }

    public event HttpDownloadDataCompletedEventHandler DownloadDataCompleted;
    public event HttpDownloadFileCompletedEventHandler DownloadFileCompleted;
    public event HttpDownloadProgressChangedEventHandler DownloadProgressChanged;
    public event HttpDownloadStringCompletedEventHandler DownloadStringCompleted;
    public event HttpOpenReadCompletedEventHandler OpenReadCompleted;
    public event HttpOpenWriteCompletedEventHandler OpenWriteCompleted;
    public event HttpUploadDataCompletedEventHandler UploadDataCompleted;
    public event HttpUploadFileCompletedEventHandler UploadFileCompleted;
    public event HttpUploadProgressChangedEventHandler UploadProgressChanged;
    public event HttpUploadStringCompletedEventHandler UploadStringCompleted;
    public event HttpUploadValuesCompletedEventHandler UploadValuesCompleted;

    Uri CreateUri(string address) {
        return MakeUri(address);
    }

    Uri CreateUri(Uri address) {
        string query = address.Query;
        if(String.IsNullOrEmpty(query))
            query = GetQueryString(true);

        if(baseAddress == null && query == null)
            return address;

        if(baseAddress == null)
            return new Uri(address.ToString() + query);

        if(query == null)
            return new Uri(baseAddress, address.ToString());

        return new Uri(baseAddress, address.ToString() + query);

    }

    string GetQueryString(bool add_qmark) {
        if(queryString == null || queryString.Count == 0)
            return null;

        StringBuilder sb = new StringBuilder();
        if(add_qmark)
            sb.Append('?');

        foreach(string key in queryString)
            sb.AppendFormat("{0}={1}&", key, UrlEncode(queryString [key]));

        if(sb.Length != 0)
            sb.Length--; // removes last '&' or the '?' if empty.

        if(sb.Length == 0)
            return null;

        return sb.ToString();
    }

    Uri MakeUri(string path) {
        string query = GetQueryString(true);
        if(baseAddress == null && query == null) {
            try {
                return new Uri(path);
            } catch(ArgumentNullException) {
                if(System.Environment.UnityWebSecurityEnabled) throw;
                path = Path.GetFullPath(path);
                return new Uri("file://" + path);
            } catch(UriFormatException) {
                if(System.Environment.UnityWebSecurityEnabled) throw;
                path = Path.GetFullPath(path);
                return new Uri("file://" + path);
            }
        }

        if(baseAddress == null)
            return new Uri(path + query);

        if(query == null)
            return new Uri(baseAddress, path);

        return new Uri(baseAddress, path + query);
    }

    WebRequest SetupRequest(Uri uri) {
        Console.WriteLine("Setup Request " + uri.AbsoluteUri);
        WebRequest request = GetWebRequest(uri);
        request.Timeout = TimeOut;
        if(Proxy != null)
            request.Proxy = Proxy;
        request.Credentials = credentials;

        // Special headers. These are properties of HttpWebRequest.
        // What do we do with other requests differnt from HttpWebRequest?
        if(headers != null && headers.Count != 0 && (request is HttpWebRequest)) {
            HttpWebRequest req = (HttpWebRequest) request;
            string expect = headers ["Expect"];
            string contentType = headers ["Content-Type"];
            string accept = headers ["Accept"];
            string connection = headers ["Connection"];
            string userAgent = headers ["User-Agent"];
            string referer = headers ["Referer"];
            headers.Remove("Expect");
            headers.Remove("Content-Type");
            headers.Remove("Accept");
            headers.Remove("Connection");
            headers.Remove("Referer");
            headers.Remove("User-Agent");
            request.Headers = headers;

            if(expect != null && expect.Length > 0)
                req.Expect = expect;

            if(accept != null && accept.Length > 0)
                req.Accept = accept;

            if(contentType != null && contentType.Length > 0)
                req.ContentType = contentType;

            if(connection != null && connection.Length > 0)
                req.Connection = connection;

            if(userAgent != null && userAgent.Length > 0)
                req.UserAgent = userAgent;

            if(referer != null && referer.Length > 0)
                req.Referer = referer;
        }

        responseHeaders = null;
        return request;
    }

    WebRequest SetupRequest(Uri uri, string method, bool is_upload) {
        WebRequest request = SetupRequest(uri);
        request.Method = DetermineMethod(uri, method, is_upload);
        return request;
    }

    byte [] ReadAll(Stream stream, int length, object userToken) {
        MemoryStream ms = null;

        bool nolength = (length == -1);
        int size = ((nolength) ? 8192 : length);
        if(nolength)
            ms = new MemoryStream();

//          long total = 0;
        int nread = 0;
        int offset = 0;
        byte [] buffer = new byte [size];
        while((nread = stream.Read(buffer, offset, size)) != 0) {
            if(nolength) {
                ms.Write(buffer, 0, nread);
            } else {
                offset += nread;
                size -= nread;
            }
            if(async) {
//                  total += nread;
                OnDownloadProgressChanged(new HttpDownloadProgressChangedEventArgs(nread, length, userToken));
            }
        }

        if(nolength)
            return ms.ToArray();

        return buffer;
    }

    public static string UrlEncode(string str) {
        StringBuilder result = new StringBuilder();

        int len = str.Length;
        for(int i = 0; i < len; i++) {
            char c = str [i];
            if(c == ' ')
                result.Append('+');
            else if((c < '0' && c != '-' && c != '.') ||
                    (c < 'A' && c > '9') ||
                    (c > 'Z' && c < 'a' && c != '_') ||
                    (c > 'z')) {
                result.Append('%');
                int idx = ((int) c) >> 4;
                result.Append((char) hexBytes [idx]);
                idx = ((int) c) & 0x0F;
                result.Append((char) hexBytes [idx]);
            } else {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    static void UrlEncodeAndWrite(Stream stream, byte [] bytes) {
        if(bytes == null)
            return;

        int len = bytes.Length;
        if(len == 0)
            return;

        for(int i = 0; i < len; i++) {
            char c = (char) bytes [i];
            if(c == ' ')
                stream.WriteByte((byte) '+');
            else if((c < '0' && c != '-' && c != '.') ||
                    (c < 'A' && c > '9') ||
                    (c > 'Z' && c < 'a' && c != '_') ||
                    (c > 'z')) {
                stream.WriteByte((byte) '%');
                int idx = ((int) c) >> 4;
                stream.WriteByte(hexBytes [idx]);
                idx = ((int) c) & 0x0F;
                stream.WriteByte(hexBytes [idx]);
            } else {
                stream.WriteByte((byte) c);
            }
        }
    }

    public void CancelAsync() {
        lock(this) {
            if(async_thread == null)
                return;

            //
            // We first flag things as done, in case the Interrupt hangs
            // or the thread decides to hang in some other way inside the
            // event handlers, or if we are stuck somewhere else.  This
            // ensures that the WebClient object is reusable immediately
            //
            Thread t = async_thread;
            CompleteAsync();
            t.Interrupt();
        }
    }

    void CompleteAsync() {
        lock(this) {
            is_busy = false;
            async_thread = null;
        }
    }

    //    DownloadDataAsync

    public void DownloadDataAsync(Uri address) {
        DownloadDataAsync(address, null);
    }

    public void DownloadDataAsync(Uri address, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");

        lock(this) {
            SetBusy();
            async = true;

            async_thread = new Thread(delegate(object state) {
                object [] args = (object []) state;
                try {
                    byte [] data = DownloadDataCore((Uri) args [0], args [1]);
                    OnDownloadDataCompleted(
                        new HttpDownloadDataCompletedEventArgs(data, null, false, args[1]));
                } catch(ThreadInterruptedException) {
                    OnDownloadDataCompleted(
                        new HttpDownloadDataCompletedEventArgs(null, null, true, args[1]));
                    throw;
                } catch(Exception e) {
                    OnDownloadDataCompleted(
                        new HttpDownloadDataCompletedEventArgs(null, e, false, args[1]));
                }
            });
            object [] cb_args = new object [] {address, userToken};
            async_thread.Start(cb_args);
        }
    }

    //    DownloadFileAsync

    public void DownloadFileAsync(Uri address, string fileName) {
        DownloadFileAsync(address, fileName, null);
    }

    public void DownloadFileAsync(Uri address, string fileName, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(fileName == null)
            throw new ArgumentNullException("fileName");

        lock(this) {
            SetBusy();
            async = true;

            async_thread = new Thread(delegate(object state) {
                object [] args = (object []) state;
                try {
                    DownloadFileCoreWithResume((Uri) args [0], (string) args [1], args [2]);
                    if(File.Exists(fileName))
                        File.Delete(fileName);
                    File.Move(fileName + downLoadTmpFile, fileName);
                    OnDownloadFileCompleted(
                        new HttpDownloadFileCompletedEventArgs(fileName, null, false, args[2]));
                } catch(ThreadInterruptedException) {
                    OnDownloadFileCompleted(
                        new HttpDownloadFileCompletedEventArgs(fileName, null, true, args[2]));
                } catch(Exception e) {
                    if(IsBreakpoint && e != null) {
                        if(e is WebException) {
                            WebException ex = (WebException) e;
                            using(HttpWebResponse rep = ex.Response as HttpWebResponse) {
                                // 断点续传有问题时 删除缓存文件
                                if(rep != null && rep.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                                    try {
                                        if(File.Exists(fileName + downLoadTmpFile))
                                            File.Delete(fileName + downLoadTmpFile);
                                    } catch(System.Exception) {
                                    }
                            }
                        }
                    }
                    OnDownloadFileCompleted(
                        new HttpDownloadFileCompletedEventArgs(fileName, e, false, args[2]));
                }
            });
            object [] cb_args = new object [] {address, fileName, userToken};
            async_thread.Start(cb_args);
        }
    }

//    DownloadStringAsync

    public void DownloadStringAsync(Uri address) {
        DownloadStringAsync(address, null);
    }

    public void DownloadStringAsync(Uri address, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");

        lock(this) {
            SetBusy();
            async = true;

            async_thread = new Thread(delegate(object state) {
                object [] args = (object []) state;
                try {
                    string data = encoding.GetString(DownloadDataCore((Uri) args [0], args [1]));
                    OnDownloadStringCompleted(
                        new HttpDownloadStringCompletedEventArgs(data, null, false, args[1]));
                } catch(ThreadInterruptedException) {
                    OnDownloadStringCompleted(
                        new HttpDownloadStringCompletedEventArgs(null, null, true, args[1]));
                } catch(Exception e) {
                    OnDownloadStringCompleted(
                        new HttpDownloadStringCompletedEventArgs(null, e, false, args[1]));
                }
            });
            object [] cb_args = new object [] {address, userToken};
            async_thread.Start(cb_args);
        }
    }

//    OpenReadAsync

    public void OpenReadAsync(Uri address) {
        OpenReadAsync(address, null);
    }

    public void OpenReadAsync(Uri address, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");

        lock(this) {
            SetBusy();
            async = true;

            async_thread = new Thread(delegate(object state) {
                object [] args = (object []) state;
                WebRequest request = null;
                try {
                    request = SetupRequest((Uri) args [0]);
                    WebResponse response = GetWebResponse(request);
                    Stream stream = response.GetResponseStream();
                    OnOpenReadCompleted(
                        new HttpOpenReadCompletedEventArgs(stream, null, false, args[1]));
                } catch(ThreadInterruptedException) {
                    if(request != null)
                        request.Abort();
                    OnOpenReadCompleted(new HttpOpenReadCompletedEventArgs(null, null, true, args[1]));
                } catch(Exception e) {
                    OnOpenReadCompleted(new HttpOpenReadCompletedEventArgs(null, e, false, args[1]));
                }
            });
            object [] cb_args = new object [] {address, userToken};
            async_thread.Start(cb_args);
        }
    }

//    OpenWriteAsync

    public void OpenWriteAsync(Uri address) {
        OpenWriteAsync(address, null);
    }

    public void OpenWriteAsync(Uri address, string method) {
        OpenWriteAsync(address, method, null);
    }

    public void OpenWriteAsync(Uri address, string method, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");

        lock(this) {
            SetBusy();
            async = true;

            async_thread = new Thread(delegate(object state) {
                object [] args = (object []) state;
                WebRequest request = null;
                try {
                    request = SetupRequest((Uri) args [0], (string) args [1], true);
                    Stream stream = request.GetRequestStream();
                    OnOpenWriteCompleted(
                        new HttpOpenWriteCompletedEventArgs(stream, null, false, args[2]));
                } catch(ThreadInterruptedException) {
                    if(request != null)
                        request.Abort();
                    OnOpenWriteCompleted(
                        new HttpOpenWriteCompletedEventArgs(null, null, true, args[2]));
                } catch(Exception e) {
                    OnOpenWriteCompleted(
                        new HttpOpenWriteCompletedEventArgs(null, e, false, args[2]));
                }
            });
            object [] cb_args = new object [] {address, method, userToken};
            async_thread.Start(cb_args);
        }
    }

//    UploadDataAsync

    public void UploadDataAsync(Uri address, byte [] data) {
        UploadDataAsync(address, null, data);
    }

    public void UploadDataAsync(Uri address, string method, byte [] data) {
        UploadDataAsync(address, method, data, null);
    }

    public void UploadDataAsync(Uri address, string method, byte [] data, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(data == null)
            throw new ArgumentNullException("data");

        lock(this) {
            SetBusy();
            async = true;

            async_thread = new Thread(delegate(object state) {
                object [] args = (object []) state;
                byte [] data2;

                try {
                    data2 = UploadDataCore((Uri) args [0], (string) args [1], (byte []) args [2], args [3]);

                    OnUploadDataCompleted(
                        new HttpUploadDataCompletedEventArgs(data2, null, false, args[3]));
                } catch(ThreadInterruptedException) {
                    OnUploadDataCompleted(
                        new HttpUploadDataCompletedEventArgs(null, null, true, args[3]));
                } catch(Exception e) {
                    OnUploadDataCompleted(
                        new HttpUploadDataCompletedEventArgs(null, e, false, args[3]));
                }
            });
            object [] cb_args = new object [] {address, method, data,  userToken};
            async_thread.Start(cb_args);
        }
    }

//    UploadFileAsync

    public void UploadFileAsync(Uri address, string fileName) {
        UploadFileAsync(address, null, fileName);
    }

    public void UploadFileAsync(Uri address, string method, string fileName) {
        UploadFileAsync(address, method, fileName, null);
    }

    public void UploadFileAsync(Uri address, string method, string fileName, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(fileName == null)
            throw new ArgumentNullException("fileName");

        lock(this) {
            SetBusy();
            async = true;

            async_thread = new Thread(delegate(object state) {
                object [] args = (object []) state;
                byte [] data;

                try {
                    data = UploadFileCore((Uri) args [0], (string) args [1], (string) args [2], args [3]);
                    OnUploadFileCompleted(
                        new HttpUploadFileCompletedEventArgs(data, null, false, args[3]));
                } catch(ThreadInterruptedException) {
                    OnUploadFileCompleted(
                        new HttpUploadFileCompletedEventArgs(null, null, true, args[3]));
                } catch(Exception e) {
                    OnUploadFileCompleted(
                        new HttpUploadFileCompletedEventArgs(null, e, false, args[3]));
                }
            });
            object [] cb_args = new object [] {address, method, fileName,  userToken};
            async_thread.Start(cb_args);
        }
    }

//    UploadStringAsync

    public void UploadStringAsync(Uri address, string data) {
        UploadStringAsync(address, null, data);
    }

    public void UploadStringAsync(Uri address, string method, string data) {
        UploadStringAsync(address, method, data, null);
    }

    public void UploadStringAsync(Uri address, string method, string data, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(data == null)
            throw new ArgumentNullException("data");

        lock(this) {
            CheckBusy();
            async = true;

            async_thread = new Thread(delegate(object state) {
                object [] args = (object []) state;

                try {
                    string data2 = UploadString((Uri) args [0], (string) args [1], (string) args [2]);
                    OnUploadStringCompleted(
                        new HttpUploadStringCompletedEventArgs(data2, null, false, args[3]));
                } catch(ThreadInterruptedException) {
                    OnUploadStringCompleted(
                        new HttpUploadStringCompletedEventArgs(null, null, true, args[3]));
                } catch(Exception e) {
                    OnUploadStringCompleted(
                        new HttpUploadStringCompletedEventArgs(null, e, false, args[3]));
                }
            });
            object [] cb_args = new object [] {address, method, data, userToken};
            async_thread.Start(cb_args);
        }
    }

//    UploadValuesAsync

    public void UploadValuesAsync(Uri address, NameValueCollection values) {
        UploadValuesAsync(address, null, values);
    }

    public void UploadValuesAsync(Uri address, string method, NameValueCollection values) {
        UploadValuesAsync(address, method, values, null);
    }

    public void UploadValuesAsync(Uri address, string method, NameValueCollection values, object userToken) {
        if(address == null)
            throw new ArgumentNullException("address");
        if(values == null)
            throw new ArgumentNullException("values");

        lock(this) {
            CheckBusy();
            async = true;

            async_thread = new Thread(delegate(object state) {
                object [] args = (object []) state;
                try {
                    byte [] data = UploadValuesCore((Uri) args [0], (string) args [1], (NameValueCollection) args [2], args [3]);
                    OnUploadValuesCompleted(
                        new HttpUploadValuesCompletedEventArgs(data, null, false, args[3]));
                } catch(ThreadInterruptedException) {
                    OnUploadValuesCompleted(
                        new HttpUploadValuesCompletedEventArgs(null, null, true, args[3]));
                } catch(Exception e) {
                    OnUploadValuesCompleted(
                        new HttpUploadValuesCompletedEventArgs(null, e, false, args[3]));
                }
            });
            object [] cb_args = new object [] {address, method, values,  userToken};
            async_thread.Start(cb_args);
        }
    }

    protected virtual void OnDownloadDataCompleted(HttpDownloadDataCompletedEventArgs args) {
        CompleteAsync();
        if(DownloadDataCompleted != null)
            DownloadDataCompleted(this, args);
        coroutine.Complete();
    }

    protected virtual void OnDownloadFileCompleted(HttpDownloadFileCompletedEventArgs args) {
        CompleteAsync();
        if(DownloadFileCompleted != null)
            DownloadFileCompleted(this, args);
        coroutine.Complete();
    }

    protected virtual void OnDownloadProgressChanged(HttpDownloadProgressChangedEventArgs e) {
        if(DownloadProgressChanged != null)
            DownloadProgressChanged(this, e);
    }

    protected virtual void OnDownloadStringCompleted(HttpDownloadStringCompletedEventArgs args) {
        CompleteAsync();
        if(DownloadStringCompleted != null)
            DownloadStringCompleted(this, args);
        coroutine.Complete();
    }

    protected virtual void OnOpenReadCompleted(HttpOpenReadCompletedEventArgs args) {
        CompleteAsync();
        if(OpenReadCompleted != null)
            OpenReadCompleted(this, args);
        coroutine.Complete();
    }

    protected virtual void OnOpenWriteCompleted(HttpOpenWriteCompletedEventArgs args) {
        CompleteAsync();
        if(OpenWriteCompleted != null)
            OpenWriteCompleted(this, args);
        coroutine.Complete();
    }

    protected virtual void OnUploadDataCompleted(HttpUploadDataCompletedEventArgs args) {
        CompleteAsync();
        if(UploadDataCompleted != null)
            UploadDataCompleted(this, args);
        coroutine.Complete();
    }

    protected virtual void OnUploadFileCompleted(HttpUploadFileCompletedEventArgs args) {
        CompleteAsync();
        if(UploadFileCompleted != null)
            UploadFileCompleted(this, args);
        coroutine.Complete();
    }

    protected virtual void OnUploadProgressChanged(HttpUploadProgressChangedEventArgs e) {
        if(UploadProgressChanged != null)
            UploadProgressChanged(this, e);
    }

    protected virtual void OnUploadStringCompleted(HttpUploadStringCompletedEventArgs args) {
        CompleteAsync();
        if(UploadStringCompleted != null)
            UploadStringCompleted(this, args);
        coroutine.Complete();
    }

    protected virtual void OnUploadValuesCompleted(HttpUploadValuesCompletedEventArgs args) {
        CompleteAsync();
        if(UploadValuesCompleted != null)
            UploadValuesCompleted(this, args);
        coroutine.Complete();
    }

    protected virtual WebResponse GetWebResponse(WebRequest request, IAsyncResult result) {
        WebResponse response = request.EndGetResponse(result);
        responseHeaders = response.Headers;
        return response;
    }

    protected virtual WebRequest GetWebRequest(Uri address) {
        return WebRequest.Create(address);
    }

    protected virtual WebResponse GetWebResponse(WebRequest request) {
        WebResponse response = request.GetResponse();
        responseHeaders = response.Headers;
        return response;
    }

}
}
