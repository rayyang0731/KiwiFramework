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

public sealed partial class SkillExpCostData :  Bright.Config.BeanBase 
{
    public SkillExpCostData(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["count"].IsNumber) { throw new SerializationException(); }  Count = _json["count"]; }
        PostInit();
    }

    public SkillExpCostData(int id, int count ) 
    {
        this.Id = id;
        this.Count = count;
        PostInit();
    }

    public static SkillExpCostData DeserializeSkillExpCostData(JSONNode _json)
    {
        return new SkillExpCostData(_json);
    }

    /// <summary>
    /// 道具 ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; private set; }

    public const int __ID__ = -1615520925;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Count:" + Count + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}