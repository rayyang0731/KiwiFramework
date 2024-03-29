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

public sealed partial class TbSkillExp :  Bright.Config.BeanBase 
{
    public TbSkillExp(JSONNode _json) 
    {
        { if(!_json["Level"].IsNumber) { throw new SerializationException(); }  Level = _json["Level"]; }
        { if(!_json["Exp"].IsObject) { throw new SerializationException(); }  Exp = SkillExpCostData.DeserializeSkillExpCostData(_json["Exp"]);  }
        { if(!_json["PVETriggerExp"].IsObject) { throw new SerializationException(); }  PVETriggerExp = SkillExpCostData.DeserializeSkillExpCostData(_json["PVETriggerExp"]);  }
        { if(!_json["PVPTriggerExp"].IsObject) { throw new SerializationException(); }  PVPTriggerExp = SkillExpCostData.DeserializeSkillExpCostData(_json["PVPTriggerExp"]);  }
        { if(!_json["PVPPassiveExp"].IsObject) { throw new SerializationException(); }  PVPPassiveExp = SkillExpCostData.DeserializeSkillExpCostData(_json["PVPPassiveExp"]);  }
        PostInit();
    }

    public TbSkillExp(int Level, SkillExpCostData Exp, SkillExpCostData PVETriggerExp, SkillExpCostData PVPTriggerExp, SkillExpCostData PVPPassiveExp ) 
    {
        this.Level = Level;
        this.Exp = Exp;
        this.PVETriggerExp = PVETriggerExp;
        this.PVPTriggerExp = PVPTriggerExp;
        this.PVPPassiveExp = PVPPassiveExp;
        PostInit();
    }

    public static TbSkillExp DeserializeTbSkillExp(JSONNode _json)
    {
        return new TbSkillExp(_json);
    }

    /// <summary>
    /// 等级
    /// </summary>
    public int Level { get; private set; }
    /// <summary>
    /// 通用被动
    /// </summary>
    public SkillExpCostData Exp { get; private set; }
    /// <summary>
    /// PVE技能
    /// </summary>
    public SkillExpCostData PVETriggerExp { get; private set; }
    /// <summary>
    /// PVP主动
    /// </summary>
    public SkillExpCostData PVPTriggerExp { get; private set; }
    /// <summary>
    /// PVP被动
    /// </summary>
    public SkillExpCostData PVPPassiveExp { get; private set; }

    public const int __ID__ = 1842928314;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        Exp?.Resolve(_tables);
        PVETriggerExp?.Resolve(_tables);
        PVPTriggerExp?.Resolve(_tables);
        PVPPassiveExp?.Resolve(_tables);
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        Exp?.TranslateText(translator);
        PVETriggerExp?.TranslateText(translator);
        PVPTriggerExp?.TranslateText(translator);
        PVPPassiveExp?.TranslateText(translator);
    }

    public override string ToString()
    {
        return "{ "
        + "Level:" + Level + ","
        + "Exp:" + Exp + ","
        + "PVETriggerExp:" + PVETriggerExp + ","
        + "PVPTriggerExp:" + PVPTriggerExp + ","
        + "PVPPassiveExp:" + PVPPassiveExp + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
