using OwlchemyVR;
using UnityEngine;

public class BreakableDonutBox : MonoBehaviour
{
	[SerializeField]
	private BasePrefabSpawner[] donutSpawners;

	private PickupableItem[] donuts;

	[SerializeField]
	private GameObject disableOnBreak;

	[SerializeField]
	private ParticleSystem pfxOnBreak;

	[SerializeField]
	private AudioClip sfxOnBreak;

	[SerializeField]
	private float breakThresh = 20f;

	private bool opened;

	private void Start()
	{
		donuts = new PickupableItem[donutSpawners.Length];
		for (int i = 0; i < donutSpawners.Length; i++)
		{
			donuts[i] = donutSpawners[i].LastSpawnedPrefabGO.GetComponent<PickupableItem>();
			donuts[i].enabled = false;
			donuts[i].Rigidbody.isKinematic = true;
		}
	}

	private void OnCollisionEnter(Collision c)
	{
		if (!opened)
		{
			float num = c.relativeVelocity.sqrMagnitude;
			PickupableItem component = c.collider.attachedRigidbody.GetComponent<PickupableItem>();
			if (component != null && component.IsCurrInHand)
			{
				num = component.CurrInteractableHand.GetCurrentVelocity().magnitude;
			}
			if (num > breakThresh)
			{
				BustOpen();
			}
		}
	}

	private void BustOpen()
	{
		opened = true;
		AudioManager.Instance.Play(base.transform.position, sfxOnBreak, 1f, 1f);
		pfxOnBreak.Play();
		disableOnBreak.SetActive(false);
		for (int i = 0; i < donuts.Length; i++)
		{
			donuts[i].enabled = true;
			donuts[i].Rigidbody.isKinematic = false;
		}
	}
}
