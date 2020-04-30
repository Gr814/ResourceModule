using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = System.Object;

public class ABAssetLoader : AssetLoader
{
    bool m_offset_ab;
    private Action<IEnumerator> m_start_coroutine;

    public ABAssetLoader(Action<Object> load_fininsh_callback, Action<IEnumerator> start_coroutine, bool offset_ab = false):base(load_fininsh_callback, start_coroutine)
    {
        m_offset_ab = offset_ab;
    }


    public override void LoadAsset(string path, bool async = true)
    {
        if (async)
        {
            StartCoroutine(LoadAssetAsync(path));
        }
        else
        {
            OnLoadFinish(AssetBundle.LoadFromFile(path));
        }
    }

    IEnumerator LoadAssetAsync(string path)
    {
        var task = ResourceManager.Instance.CreatTask(path, OnLoadFinish);
        yield return task;
        ResourceManager.Instance.ReleaseTask(path);
    }
}
