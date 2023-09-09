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

public sealed partial class Icon
{
    private readonly Dictionary<int, TbIcon> _dataMap;
    private readonly List<TbIcon> _dataList;
    
    public Icon(JSONNode _json)
    {
        _dataMap = new Dictionary<int, TbIcon>();
        _dataList = new List<TbIcon>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = TbIcon.DeserializeTbIcon(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.ID, _v);
        }
        PostInit();
    }

    public Dictionary<int, TbIcon> DataMap => _dataMap;
    public List<TbIcon> DataList => _dataList;

    public TbIcon GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public TbIcon Get(int key) => _dataMap[key];
    public TbIcon this[int key] => _dataMap[key];

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