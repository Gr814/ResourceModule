
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
/// <summary>
/// 管理所有资源
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    AssetBundleManifest m_manifest;

    IAssetLoader m_loader;

    Dictionary<string, ITask> task_dic = new Dictionary<string, ITask>();

    //所有资源
    Dictionary<string, ResourceUnit> m_all_assets = new Dictionary<string, ResourceUnit>();

    //资源加载完成回掉
    Dictionary<string, Action<Object>> m_load_callback = new Dictionary<string, Action<Object>>();


    public void Setup(AssetBundleManifest mainfest, IAssetLoader loader)
    {
        m_manifest = mainfest;
        m_loader = loader;
    }

    public void GetObject<T>(string path, Action<T> load_callback) where T : Object
    {
        var cb = load_callback as Action<Object>;
        if (m_all_assets.ContainsKey(path))
        {
            var unit = m_all_assets[path];
            if (unit.status == EResourceUnitStatus.Done)
            {
                load_callback.Invoke(unit.GetObject<T>());
            }
            else  
            {
                if (unit.status == EResourceUnitStatus.Unload)
                {
                    loader.LoadAsset(path);

                }
                m_load_callback.Add(path, cb);
            }
        }
        else
        {
            loader.LoadAsset(path);
            m_load_callback.Add(path, cb);
            CreatResourceUnit(path);
        }
    }

    private ResourceUnit CreatResourceUnit(string path)
    {
        var dependencies = manifest.GetAllDependencies(path);
        var length = dependencies.Length;
        var units = new ResourceUnit[length];
        for (int i = 0; i < length; i++)
        {
            units[i] = CreatResourceUnit(dependencies[i]);
        }
        var unit = new ResourceUnit(units);
        m_all_assets.Add(path, unit);
        return unit;
    }

    void OnLoadFinish(string path,Object obj)
    {
        if (obj is AssetBundle)
        {
            var ab = obj as AssetBundle;
            var assets = ab.LoadAllAssets();
            m_all_assets[path].Finish(assets.Length == 1 ?  assets[0] : null);
        }
        else
        {
            m_load_callback[path].Invoke(obj);
            m_load_callback.Remove(path);
        }
    }

    public ITask CreatTask(string path, Action<string, Object> finish_cb)
    {
        var dependencies = manifest.GetAllDependencies(path);
        var length = dependencies.Length;
        var tasks = new ITask[length];
        for (int i = 0; i < length; i++)
        {
            var ab_path = dependencies[i];
            if (task_dic.ContainsKey(ab_path))
            {
                tasks[i] = new TaskLink(task_dic[ab_path]);
            }
            else
            {
                var tsk = CreatTask(ab_path, finish_cb);
                tasks[i] = tsk;
            }
        }
        var task = new Task(tasks, path);
        task.Finish_CB = finish_cb;
        task_dic.Add(path ,task);
        return new Task(tasks, path);
    }

    public void ReleaseTask(string path)
    {
        if (task_dic.ContainsKey(path))
        {
            task_dic.Remove(path);
        }
    }

    public class TaskLink : ITask
    {
        private ITask m_link_task;

        public object Current { get; }
        public bool finished { get; set; }
        public Action<string, Object> Finish_CB { get; set; }

        public TaskLink(ITask task)
        {
            m_link_task = task;
        }

        public bool MoveNext()
        {
            return  !m_link_task.finished;
        }

        public void Reset()
        {
           
        }

        public IEnumerator Execute()
        {
            yield return null;
        }
    }

    public class Task : ITask
    {
        string m_path;
        private uint m_crc;
        private ulong m_offset;
        ITask[] m_subTask;
        private AssetBundle m_ab;

        public bool finished { get; set; }

        public Task(ITask[] sub_task, string path, uint crc = 0, ulong offset = 0)
        {
            m_subTask = sub_task;
            m_path = path;
            m_crc = crc;
            m_offset = offset;
        }

        IEnumerator current;
        public object Current { get => current; }
        public Action<string, Object> Finish_CB { get; set;}

        public IEnumerator Execute()
        {
            var request = AssetBundle.LoadFromFileAsync(m_path, m_crc, m_offset);
            yield return request;
            m_ab = request.assetBundle;
            finished = true;
        }

        public bool MoveNext()
        {
           if (m_subTask != null && m_subTask.Length > index)
            {
                current = m_subTask[index++];
                return true;
            }
           if (!finished)
            {
                current = Execute();
                return true;
            }
            Finish_CB(m_path, m_ab);
            return false;
        }

        int index = 0;
        public void Reset()
        {
            index = 0;
        }
    }

    public interface ITask : IEnumerator
    {
        bool finished { get; set; }
        Action<string, Object> Finish_CB { get; set; }
    }
}
