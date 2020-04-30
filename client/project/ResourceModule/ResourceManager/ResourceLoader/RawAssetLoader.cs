using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RawAssetLoader : IAssetLoader
{
    public Action<object> OnLoadFinish { get; set; }

    public void LoadAsset(string path, bool async = true)
    {
        if (async)
        {
        }
        else
        {
            OnLoadFinish.Invoke(Resources.Load(path));
        }
    }

    IEnumerator LoadAsync(string path)
    {
        var request = Resources.LoadAsync(path);
        yield return request;
        OnLoadFinish.Invoke(request.asset);
    }
}
