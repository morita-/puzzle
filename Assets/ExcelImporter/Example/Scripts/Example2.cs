using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Example2 : MonoBehaviour
{
	[SerializeField] MstItems2 mstItems;
	[SerializeField] Text text;

	void Start()
	{
		ShowItems();
	}

	void ShowItems()
	{
		string str = "";

		mstItems.Entities
			.ForEach(entity => str += DescribeMstItem2Entity(entity) + "\n");

		text.text = str;
	}

	string DescribeMstItem2Entity(MstItem2Entity entity)
	{
		return string.Format(
			"{0} : {1}, {2}, {3}, {4}, {5}",
			entity.id,
			entity.name,
			entity.price,
			entity.isNotForSale,
			entity.rate,
			entity.category
		);
	}
}

