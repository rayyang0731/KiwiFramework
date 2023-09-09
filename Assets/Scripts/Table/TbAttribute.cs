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

public sealed partial class TbAttribute :  Bright.Config.BeanBase 
{
    public TbAttribute(JSONNode _json) 
    {
        { if(!_json["ID"].IsNumber) { throw new SerializationException(); }  ID = _json["ID"]; }
        { if(!_json["Name"]["key"].IsString) { throw new SerializationException(); }  Name_l10n_key = _json["Name"]["key"]; if(!_json["Name"]["text"].IsString) { throw new SerializationException(); }  Name = _json["Name"]["text"]; }
        PostInit();
    }

    public TbAttribute(int ID, string Name ) 
    {
        this.ID = ID;
        this.Name = Name;
        PostInit();
    }

    public static TbAttribute DeserializeTbAttribute(JSONNode _json)
    {
        return new TbAttribute(_json);
    }

    /// <summary>
    /// 属性 ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 属性名称
    /// </summary>
    public string Name { get; private set; }
    public string Name_l10n_key { get; }

    public const int __ID__ = -681459794;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        Name = translator(Name_l10n_key, Name);
    }

    public override string ToString()
    {
        return "{ "
        + "ID:" + ID + ","
        + "Name:" + Name + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}