using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public  class ResourceUnit
{
    public EResourceUnitStatus status { get; private set; }

    public int RefCounter { get; private set; }

    private Object m_asset;

    private ResourceUnit[] depend_units;

    public ResourceUnit(ResourceUnit[] depend)
    {
        depend_units = depend;
        for (int i = 0; i < depend_units.Length; i++)
        {
            depend_units[i].Ref();
        }
        status = EResourceUnitStatus.Loading;
    }

    public T GetObject<T>() where T: Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            return CloneAsset() as T;
        }
        else
        {
            return m_asset as T;
        }
    }

    GameObject CloneAsset()
    {
        var obj = Object.Instantiate(m_asset) as GameObject;
        var cpt = obj.AddComponent<RefKeeper>();
        RefCounter++;
        cpt.SetBreakCallBack(BreakLink);
        return obj;
    }

    public void Finish(Object asset)
    {
        m_asset = asset;
        status = EResourceUnitStatus.Done;
    }

    public void BreakLink()
    {
        RefCounter--;
        if (RefCounter == 0)
        {
            Clear();
        }
    }

    public void Ref()
    {
        RefCounter++;
    }

    private void Clear()
    {
        for (int i = 0; i < depend_units.Length; i++)
        {
            depend_units[i].BreakLink();
        }

        Resources.UnloadAsset(m_asset);
        status = EResourceUnitStatus.Unload;
        m_asset = null;
        depend_units = null;
    }
}

public enum EResourceUnitStatus
{
    Done,// 完成
    Loading,//加载中
    Unload,//卸载
}
