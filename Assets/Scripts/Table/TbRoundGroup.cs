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

public sealed partial class TbRoundGroup :  Bright.Config.BeanBase 
{
    public TbRoundGroup(JSONNode _json) 
    {
        { if(!_json["ID"].IsNumber) { throw new SerializationException(); }  ID = _json["ID"]; }
        { if(!_json["LevelID"].IsNumber) { throw new SerializationException(); }  LevelID = _json["LevelID"]; }
        { if(!_json["RoundIndex"].IsNumber) { throw new SerializationException(); }  RoundIndex = _json["RoundIndex"]; }
        { if(!_json["MonsterIDs"].IsString) { throw new SerializationException(); }  MonsterIDs = _json["MonsterIDs"]; }
        { if(!_json["ShowName"].IsString) { throw new SerializationException(); }  ShowName = _json["ShowName"]; }
        { if(!_json["PreATKRound"].IsString) { throw new SerializationException(); }  PreATKRound = _json["PreATKRound"]; }
        { if(!_json["ATKGap"].IsString) { throw new SerializationException(); }  ATKGap = _json["ATKGap"]; }
        { if(!_json["SpecialRound"].IsBoolean) { throw new SerializationException(); }  SpecialRound = _json["SpecialRound"]; }
        { if(!_json["HeadIcon"].IsString) { throw new SerializationException(); }  HeadIcon = _json["HeadIcon"]; }
        { if(!_json["BeginDialog"].IsString) { throw new SerializationException(); }  BeginDialog = _json["BeginDialog"]; }
        { if(!_json["EndDialog"].IsString) { throw new SerializationException(); }  EndDialog = _json["EndDialog"]; }
        PostInit();
    }

    public TbRoundGroup(int ID, int LevelID, int RoundIndex, string MonsterIDs, string ShowName, string PreATKRound, string ATKGap, bool SpecialRound, string HeadIcon, string BeginDialog, string EndDialog ) 
    {
        this.ID = ID;
        this.LevelID = LevelID;
        this.RoundIndex = RoundIndex;
        this.MonsterIDs = MonsterIDs;
        this.ShowName = ShowName;
        this.PreATKRound = PreATKRound;
        this.ATKGap = ATKGap;
        this.SpecialRound = SpecialRound;
        this.HeadIcon = HeadIcon;
        this.BeginDialog = BeginDialog;
        this.EndDialog = EndDialog;
        PostInit();
    }

    public static TbRoundGroup DeserializeTbRoundGroup(JSONNode _json)
    {
        return new TbRoundGroup(_json);
    }

    /// <summary>
    /// 波次 ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 对应关卡ID
    /// </summary>
    public int LevelID { get; private set; }
    /// <summary>
    /// 波次索引
    /// </summary>
    public int RoundIndex { get; private set; }
    /// <summary>
    /// 怪物ID
    /// </summary>
    public string MonsterIDs { get; private set; }
    /// <summary>
    /// 显示名称
    /// </summary>
    public string ShowName { get; private set; }
    /// <summary>
    /// 出手前置回合
    /// </summary>
    public string PreATKRound { get; private set; }
    /// <summary>
    /// 出手间隔
    /// </summary>
    public string ATKGap { get; private set; }
    /// <summary>
    /// 特殊波次
    /// </summary>
    public bool SpecialRound { get; private set; }
    /// <summary>
    /// 图像图标
    /// </summary>
    public string HeadIcon { get; private set; }
    /// <summary>
    /// 波此前对话
    /// </summary>
    public string BeginDialog { get; private set; }
    /// <summary>
    /// 波次结束对话
    /// </summary>
    public string EndDialog { get; private set; }

    public const int __ID__ = -224220481;
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
        + "LevelID:" + LevelID + ","
        + "RoundIndex:" + RoundIndex + ","
        + "MonsterIDs:" + MonsterIDs + ","
        + "ShowName:" + ShowName + ","
        + "PreATKRound:" + PreATKRound + ","
        + "ATKGap:" + ATKGap + ","
        + "SpecialRound:" + SpecialRound + ","
        + "HeadIcon:" + HeadIcon + ","
        + "BeginDialog:" + BeginDialog + ","
        + "EndDialog:" + EndDialog + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
