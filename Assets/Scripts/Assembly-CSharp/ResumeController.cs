using OwlchemyVR;
using UnityEngine;

public class ResumeController : MonoBehaviour
{
	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private GameObject[] visualFronts;

	[SerializeField]
	private AudioClip[] shakeSounds;

	private ElementSequence<AudioClip> shakeSoundSequence;

	[SerializeField]
	private AudioSourceHelper shakeSource;

	private void Awake()
	{
		shakeSoundSequence = new ElementSequence<AudioClip>(shakeSounds);
	}

	private void Update()
	{
		if (pickupableItem.IsCurrInHand && pickupableItem.CurrInteractableHand.GrabbedItemCurrVelocity.magnitude > 15f && !shakeSource.IsPlaying)
		{
			shakeSource.SetClip(shakeSoundSequence.GetNext());
			shakeSource.Play();
		}
	}

	public void SetupEmployeeEvaluation(int portaitIndex, WorldItemData _worldItemData)
	{
		for (int i = 0; i < visualFronts.Length; i++)
		{
			visualFronts[i].SetActive(i == portaitIndex);
		}
		worldItem.ManualSetData(_worldItemData);
	}
}
