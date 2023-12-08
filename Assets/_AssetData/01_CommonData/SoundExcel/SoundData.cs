using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class SoundData : ScriptableObject
{
	public List<SoundDataBgmEntity> BGM; // Replace 'EntityType' to an actual type that is serializable.
    public List<SoundDataSeEntity> SE; // Replace 'EntityType' to an actual type that is serializable.
}
