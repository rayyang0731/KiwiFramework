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

public sealed partial class TbGradeConditionDesc :  Bright.Config.BeanBase 
{
    public TbGradeConditionDesc(JSONNode _json) 
    {
        { if(!_json["ID"].IsNumber) { throw new SerializationException(); }  ID = _json["ID"]; }
        { if(!_json["Desc"].IsString) { throw new SerializationException(); }  Desc = _json["Desc"]; }
        PostInit();
    }

    public TbGradeConditionDesc(int ID, string Desc ) 
    {
        this.ID = ID;
        this.Desc = Desc;
        PostInit();
    }

    public static TbGradeConditionDesc DeserializeTbGradeConditionDesc(JSONNode _json)
    {
        return new TbGradeConditionDesc(_json);
    }

    /// <summary>
    /// 评价 ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 评价显示内容
    /// </summary>
    public string Desc { get; private set; }

    public const int __ID__ = 1548779235;
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
        + "ID:" + ID + ","
        + "Desc:" + Desc + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
