using UnityEngine;
using System.Collections;
using System;

public class LocalResoure {

    /// <summary>
    /// 
    /// </summary>
    private string localFilename = "resourcevertion.txt";

    public void Initialize(string url, Action onload)
    {
        Action<WWW, string> oncomlete = (www, tag) =>
        {
            if (www.isDone)
            {
                //onload(www, tag);
            }
        };

        ResourceUpdater.Instance.DownLoadFromurl(url + "/" + localFilename, "", oncomlete);
    }

}
