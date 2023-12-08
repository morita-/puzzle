using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class Pzl_StageParams : ScriptableObject
{
    public List<Pzl_StageParamsEntity> Map0; // Replace 'EntityType' to an actual type that is serializable.

    public Pzl_StageParams Clone()
    {
        return (Pzl_StageParams)MemberwiseClone();
    }

    public Pzl_StageParamsEntity GetStageParam(int id)
    {

        for (int i = 0; i < Map0.Count; i++)
        {
            if (Map0[i].StageNo == id)
            {
                return Map0[i];
            }
        }

        return null;
    }

}
