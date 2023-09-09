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

public sealed partial class TbGameColor :  Bright.Config.BeanBase 
{
    public TbGameColor(JSONNode _json) 
    {
        { if(!_json["ID"].IsNumber) { throw new SerializationException(); }  ID = _json["ID"]; }
        { if(!_json["Color"].IsString) { throw new SerializationException(); }  Color = _json["Color"]; }
        PostInit();
    }

    public TbGameColor(int ID, string Color ) 
    {
        this.ID = ID;
        this.Color = Color;
        PostInit();
    }

    public static TbGameColor DeserializeTbGameColor(JSONNode _json)
    {
        return new TbGameColor(_json);
    }

    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 颜色值
    /// </summary>
    public string Color { get; private set; }

    public const int __ID__ = 280604579;
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
        + "Color:" + Color + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}