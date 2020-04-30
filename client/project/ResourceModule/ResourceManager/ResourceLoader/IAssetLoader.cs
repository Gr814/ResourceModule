using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IAssetLoader
{
    void LoadAsset(string path, bool async = true);
}
