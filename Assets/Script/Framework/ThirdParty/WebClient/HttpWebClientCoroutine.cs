using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTUnityPlugin.WebClient {

public class HttpWebClientCoroutineEnum : IEnumerator {

    private bool isDone = true;

    public bool IsDone {
        get {
            lock(this) {
                return isDone;
            }
        }

        private set {
            lock(this) {
                isDone = value;
            }
        }
    }

    public bool MoveNext() {
        return IsDone;
    }

    public void Reset() {
        throw new NotImplementedException();
    }

    public void Complete() {
        IsDone = false;
    }

    object IEnumerator.Current {
        get {
            return null;
        }
    }
}

public class HttpWebClientCoroutine : IEnumerable {
    HttpWebClientCoroutineEnum hwcEnum = new HttpWebClientCoroutineEnum();
    public HttpWebClientCoroutine() {
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return (IEnumerator)GetEnumerator();
    }

    public HttpWebClientCoroutineEnum GetEnumerator() {
        return hwcEnum;
    }

    internal void Complete() {
        hwcEnum.Complete();
    }
}

}
