using OwlchemyVR;
using UnityEngine;

public class JumboHotDogSpecialCase : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint attachPoint;

	[SerializeField]
	private WorldItemData jumbosizerWID;

	[SerializeField]
	private WorldItemData completedHotDog;

	[SerializeField]
	private float jumboScaleX = 1.5f;

	private void OnEnable()
	{
		attachPoint.OnObjectWasAttached += OnAttached;
	}

	private void OnDisable()
	{
		attachPoint.OnObjectWasAttached -= OnAttached;
	}

	private void OnAttached(AttachablePoint point, AttachableObject obj)
	{
		if ((base.transform.localScale.x >= jumboScaleX || obj.transform.localScale.x >= jumboScaleX) && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(completedHotDog, jumbosizerWID, "CREATED_BY");
		}
	}
}
