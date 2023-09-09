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

public sealed partial class TbBoxSlot :  Bright.Config.BeanBase 
{
    public TbBoxSlot(JSONNode _json) 
    {
        { if(!_json["ID"].IsNumber) { throw new SerializationException(); }  ID = _json["ID"]; }
        { if(!_json["Condition"].IsNumber) { throw new SerializationException(); }  Condition = _json["Condition"]; }
        { if(!_json["Value"].IsNumber) { throw new SerializationException(); }  Value = _json["Value"]; }
        { if(!_json["Desc"].IsString) { throw new SerializationException(); }  Desc = _json["Desc"]; }
        PostInit();
    }

    public TbBoxSlot(int ID, int Condition, int Value, string Desc ) 
    {
        this.ID = ID;
        this.Condition = Condition;
        this.Value = Value;
        this.Desc = Desc;
        PostInit();
    }

    public static TbBoxSlot DeserializeTbBoxSlot(JSONNode _json)
    {
        return new TbBoxSlot(_json);
    }

    /// <summary>
    /// 槽位ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 槽位开启条件
    /// </summary>
    public int Condition { get; private set; }
    /// <summary>
    /// 具体数值
    /// </summary>
    public int Value { get; private set; }
    /// <summary>
    /// 文字说明
    /// </summary>
    public string Desc { get; private set; }

    public const int __ID__ = -768659461;
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
        + "Condition:" + Condition + ","
        + "Value:" + Value + ","
        + "Desc:" + Desc + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}