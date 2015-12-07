
using System;
using System.ComponentModel;
using System.IO;

namespace LTUnityPlugin.WebClient {

/// <summary>
/// Work with hxs's HttpWebClient
/// By Hxs1990.
/// Create 2015.06.18
/// Last Update 2015.06.19
/// </summary>

public delegate void HttpDownloadDataCompletedEventHandler(object sender, HttpDownloadDataCompletedEventArgs e);
public delegate void HttpDownloadFileCompletedEventHandler(object sender, HttpDownloadFileCompletedEventArgs e);
public delegate void HttpDownloadProgressChangedEventHandler(object sender, HttpDownloadProgressChangedEventArgs e);
public delegate void HttpDownloadStringCompletedEventHandler(object sender, HttpDownloadStringCompletedEventArgs e);
public delegate void HttpOpenReadCompletedEventHandler(object sender, HttpOpenReadCompletedEventArgs e);
public delegate void HttpOpenWriteCompletedEventHandler(object sender, HttpOpenWriteCompletedEventArgs e);
public delegate void HttpUploadDataCompletedEventHandler(object sender, HttpUploadDataCompletedEventArgs e);
public delegate void HttpUploadFileCompletedEventHandler(object sender, HttpUploadFileCompletedEventArgs e);
public delegate void HttpUploadProgressChangedEventHandler(object sender, HttpUploadProgressChangedEventArgs e);
public delegate void HttpUploadStringCompletedEventHandler(object sender, HttpUploadStringCompletedEventArgs e);
public delegate void HttpUploadValuesCompletedEventHandler(object sender, HttpUploadValuesCompletedEventArgs e);


public class HttpDownloadDataCompletedEventArgs : AsyncCompletedEventArgs {
    public HttpDownloadDataCompletedEventArgs(byte [] result,
            Exception error, bool cancelled, object userState)
    : base(error, cancelled, userState) {
        this.result = result;
    }

    byte [] result;

    public byte [] Result {
        get {
            return result;
        }
    }
}

public class HttpDownloadFileCompletedEventArgs : AsyncCompletedEventArgs {
    public HttpDownloadFileCompletedEventArgs(string file,
            Exception error, bool cancelled, object userState)
    : base(error, cancelled, userState) {
        this.file = file;
    }

    string file;
    public string File {
        get {
            return file;
        }
    }
}

public class HttpDownloadProgressChangedEventArgs : ProgressChangedEventArgs {
    public HttpDownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive, object userState)
    : base(totalBytesToReceive != -1 ? ((int)(bytesReceived * 100 / totalBytesToReceive)) : 0, userState) {
        this.received = bytesReceived;
        this.total = totalBytesToReceive;
        this.progressPercentageFloat = totalBytesToReceive != -1 ? (float)bytesReceived / totalBytesToReceive : 0.0f;
    }

    float progressPercentageFloat;

    public float ProgressPercentageFloat {
        get { return progressPercentageFloat; }
        set { progressPercentageFloat = value; }
    }

    long received, total;

    public long BytesReceived {
        get { return received; }
    }

    public double BytesReceivedKB {
        get { return Math.Round(received / 1024.0 , 2);}
    }

    public long TotalBytesToReceive {
        get { return total; }
    }

    public double TotalBytesToReceiveKB {
        get { return Math.Round(total / 1024.0, 2); }
    }
}

public class HttpDownloadStringCompletedEventArgs : AsyncCompletedEventArgs {
    public HttpDownloadStringCompletedEventArgs(string result,
            Exception error, bool cancelled, object userState)
    : base(error, cancelled, userState) {
        this.result = result;
    }

    string result;

    public string Result {
        get {
            RaiseExceptionIfNecessary();
            return result;
        }
    }
}

public class HttpOpenReadCompletedEventArgs : AsyncCompletedEventArgs {
    public HttpOpenReadCompletedEventArgs(Stream result, Exception error, bool cancelled, object userState)
    : base(error, cancelled, userState) {
        this.result = result;
    }
    Stream result;
    public Stream Result {
        get {
            RaiseExceptionIfNecessary();
            return result;
        }
    }
}

public class HttpOpenWriteCompletedEventArgs : AsyncCompletedEventArgs {
    public HttpOpenWriteCompletedEventArgs(Stream result,
                                           Exception error, bool cancelled, object userState)
    : base(error, cancelled, userState) {
        this.result = result;
    }

    Stream result;

    public Stream Result {
        get {
            RaiseExceptionIfNecessary();
            return result;
        }
    }
}

public class HttpUploadDataCompletedEventArgs : AsyncCompletedEventArgs {
    public HttpUploadDataCompletedEventArgs(byte [] result,
                                            Exception error, bool cancelled, object userState)
    : base(error, cancelled, userState) {
        this.result = result;
    }

    byte [] result;

    public byte [] Result {
        get { return result; }
    }
}

public class HttpUploadFileCompletedEventArgs : AsyncCompletedEventArgs {
    public HttpUploadFileCompletedEventArgs(byte [] result,
                                            Exception error, bool cancelled, object userState)
    : base(error, cancelled, userState) {
        this.result = result;
    }

    byte [] result;

    public byte [] Result {
        get { return result; }
    }
}

public class HttpUploadProgressChangedEventArgs : ProgressChangedEventArgs {
    public HttpUploadProgressChangedEventArgs(
        long bytesReceived, long totalBytesToReceive,
        long bytesSent, long totalBytesToSend,
        int progressPercentage, object userState)
    : base(progressPercentage, userState) {
        this.received = bytesReceived;
        this.total_recv = totalBytesToReceive;
        this.sent = bytesSent;
        this.total_send = totalBytesToSend;
    }

    long received, sent, total_recv, total_send;
    public long BytesReceived {
        get { return received; }
    }

    public long TotalBytesToReceive {
        get { return total_recv; }
    }

    public long BytesSent {
        get { return sent; }
    }

    public long TotalBytesToSend {
        get { return total_send; }
    }
}

public class HttpUploadStringCompletedEventArgs : AsyncCompletedEventArgs {
    public HttpUploadStringCompletedEventArgs(string result,
            Exception error, bool cancelled, object userState)
    : base(error, cancelled, userState) {
        this.result = result;
    }

    string result;

    public string Result {
        get {
            RaiseExceptionIfNecessary();
            return result;
        }
    }
}

public class HttpUploadValuesCompletedEventArgs : AsyncCompletedEventArgs {
    public HttpUploadValuesCompletedEventArgs(byte[] result,
            Exception error, bool cancelled, object userState)
    : base(error, cancelled, userState) {
        this.result = result;
    }

    byte[] result;

    public byte[] Result {
        get { return result; }
    }
}

}
