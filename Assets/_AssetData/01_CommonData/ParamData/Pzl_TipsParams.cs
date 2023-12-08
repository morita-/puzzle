using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class Pzl_TipsParams : ScriptableObject
{
	public List<Pzl_TipsParamsTipsEntity> Tips; // Replace 'EntityType' to an actual type that is serializable.
	public List<Pzl_TipsParamsCompTipsEntity> CompTips; // Replace 'EntityType' to an actual type that is serializable.

    public Pzl_TipsParams Clone()
    {
        return (Pzl_TipsParams)MemberwiseClone();

    }

}
