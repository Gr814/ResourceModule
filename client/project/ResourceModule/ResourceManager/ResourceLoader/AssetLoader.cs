using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class AssetLoader : IAssetLoader
{

    public Action<Object> OnLoadFinish { get; set;}

    public Action<IEnumerator> StartCoroutine{ get; set;}
    
    public AssetLoader(Action<Object> load_fininsh_callback, Action<IEnumerator>  start_coroutine)
    {
        OnLoadFinish = load_fininsh_callback;
        StartCoroutine = start_coroutine;
    }

    public abstract void LoadAsset(string path, bool async = true);
}
