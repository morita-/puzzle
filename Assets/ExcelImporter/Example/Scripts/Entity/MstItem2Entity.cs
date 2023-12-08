using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MstItem2Entity
{
	public int id;
	public string name;
	public int price;
	public bool isNotForSale;
	public float rate;
	public MstItemCategory category;
}

public enum MstItem2Category
{
	Red,
	Green,
	Blue,
}