using UnityEngine;

public class HologramController : MonoBehaviour
{
	[Header("Holograms")]
	[SerializeField]
	private GameObject kitchenHologram;

	[SerializeField]
	private GameObject officeHologram;

	[SerializeField]
	private GameObject convenienceStoreHologram;

	[SerializeField]
	private GameObject classroomHologram;

	[SerializeField]
	private GameObject mechanicHologram;

	[SerializeField]
	private GameObject moonHologram;

	[SerializeField]
	private AudioClip hologramActivateClip;

	private void Awake()
	{
		DeactivateAll();
	}

	public void SetHologram(JobCartridgeWithGenieFlags currentlyAttachedJobWithGenieFlags, bool showMoon = false)
	{
		Debug.Log("Hologram is being set here!");
		switch (currentlyAttachedJobWithGenieFlags.BaseJobCartridge.StateData.ID)
		{
		case "Kitchen":
			DeactivateAll();
			kitchenHologram.SetActive(true);
			break;
		case "Office":
			DeactivateAll();
			officeHologram.SetActive(true);
			break;
		case "ConvenienceStore":
			DeactivateAll();
			convenienceStoreHologram.SetActive(true);
			break;
		case "Classroom":
			DeactivateAll();
			classroomHologram.SetActive(true);
			break;
		case "AutoMechanic":
			DeactivateAll();
			mechanicHologram.SetActive(true);
			break;
		default:
			Debug.LogWarning("JobLevelData name doesn't match one of the switches in SetHologram (see Data/JobLevelDatas)");
			break;
		}
		moonHologram.gameObject.SetActive(showMoon);
		AudioManager.Instance.Play(kitchenHologram.transform.position, hologramActivateClip, 1f, 1f);
	}

	public void DeactivateAll()
	{
		moonHologram.SetActive(false);
		kitchenHologram.SetActive(false);
		officeHologram.SetActive(false);
		convenienceStoreHologram.SetActive(false);
		classroomHologram.SetActive(false);
		mechanicHologram.SetActive(false);
	}
}
