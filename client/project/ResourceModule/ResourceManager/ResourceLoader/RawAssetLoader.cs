using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public class RawAssetLoader : AssetLoader
{
    public RawAssetLoader(Action<string, Object> load_fininsh_callback, Action<IEnumerator> start_coroutine) : base(load_fininsh_callback, start_coroutine)
    {
    }

    public override void LoadAsset(string path, bool async = true)
    {
        if (async)
        {
            StartCoroutine(LoadAsync(path));
        }
        else
        {
            OnLoadFinish.Invoke(path, Resources.Load(path));
        }
    }

    IEnumerator LoadAsync(string path)
    {
        var request = Resources.LoadAsync(path);
        yield return request;
        OnLoadFinish.Invoke(path, request.asset);
    }
}
