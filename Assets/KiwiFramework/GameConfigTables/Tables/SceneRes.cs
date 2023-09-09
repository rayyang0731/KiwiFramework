//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg
{ 

public sealed partial class SceneRes
{
    private readonly Dictionary<int, TbSceneRes> _dataMap;
    private readonly List<TbSceneRes> _dataList;
    
    public SceneRes(JSONNode _json)
    {
        _dataMap = new Dictionary<int, TbSceneRes>();
        _dataList = new List<TbSceneRes>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = TbSceneRes.DeserializeTbSceneRes(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.ID, _v);
        }
        PostInit();
    }

    public Dictionary<int, TbSceneRes> DataMap => _dataMap;
    public List<TbSceneRes> DataList => _dataList;

    public TbSceneRes GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public TbSceneRes Get(int key) => _dataMap[key];
    public TbSceneRes this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    
    partial void PostInit();
    partial void PostResolve();
}

}