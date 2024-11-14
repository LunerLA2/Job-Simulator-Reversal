using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class BonsaiController : MonoBehaviour
{
	private const string code = "4423141";

	[SerializeField]
	private WorldItemData bonsaiItemData;

	[SerializeField]
	private List<AttachablePoint> attachablePoints = new List<AttachablePoint>();

	private string currentEntry = string.Empty;

	private void OnEnable()
	{
		foreach (AttachablePoint attachablePoint in attachablePoints)
		{
			attachablePoint.OnObjectWasDetached += FruitDetached;
		}
	}

	private void OnDisable()
	{
		foreach (AttachablePoint attachablePoint in attachablePoints)
		{
			attachablePoint.OnObjectWasDetached -= FruitDetached;
		}
	}

	private void FruitDetached(AttachablePoint point, AttachableObject attachableObject)
	{
		currentEntry += attachablePoints.IndexOf(point) + 1;
		if (currentEntry.Length > "4423141".Length)
		{
			currentEntry = currentEntry.Substring(currentEntry.Length - "4423141".Length);
		}
		if (currentEntry == "4423141" && ExtraPrefs.ExtraProgress >= 1)
		{
			GameEventsManager.Instance.ItemActionOccurred(bonsaiItemData, "ACTIVATED");
			if (ExtraPrefs.ExtraProgress < 2)
			{
				ExtraPrefs.ExtraProgress = 2;
			}
		}
	}
}
