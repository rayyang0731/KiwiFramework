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

public sealed partial class TbPVPRankPrize :  Bright.Config.BeanBase 
{
    public TbPVPRankPrize(JSONNode _json) 
    {
        { if(!_json["ID"].IsNumber) { throw new SerializationException(); }  ID = _json["ID"]; }
        { if(!_json["Max"].IsNumber) { throw new SerializationException(); }  Max = _json["Max"]; }
        { if(!_json["Reward"].IsString) { throw new SerializationException(); }  Reward = _json["Reward"]; }
        { if(!_json["BoxID"].IsNumber) { throw new SerializationException(); }  BoxID = _json["BoxID"]; }
        { if(!_json["GroupType"].IsString) { throw new SerializationException(); }  GroupType = _json["GroupType"]; }
        PostInit();
    }

    public TbPVPRankPrize(int ID, int Max, string Reward, int BoxID, string GroupType ) 
    {
        this.ID = ID;
        this.Max = Max;
        this.Reward = Reward;
        this.BoxID = BoxID;
        this.GroupType = GroupType;
        PostInit();
    }

    public static TbPVPRankPrize DeserializeTbPVPRankPrize(JSONNode _json)
    {
        return new TbPVPRankPrize(_json);
    }

    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 上限
    /// </summary>
    public int Max { get; private set; }
    /// <summary>
    /// 奖励类型
    /// </summary>
    public string Reward { get; private set; }
    /// <summary>
    /// 宝箱ID
    /// </summary>
    public int BoxID { get; private set; }
    /// <summary>
    /// 组别
    /// </summary>
    public string GroupType { get; private set; }

    public const int __ID__ = 2123893994;
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
        + "Max:" + Max + ","
        + "Reward:" + Reward + ","
        + "BoxID:" + BoxID + ","
        + "GroupType:" + GroupType + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}