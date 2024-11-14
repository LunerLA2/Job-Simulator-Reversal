using System;
using OwlchemyVR;
using UnityEngine;

public class ChildJobBot : MonoBehaviour
{
	private Vector3 lookAt;

	private Transform playerHead;

	private bool lookingAtPlayer = true;

	private Vector3 defaultPosition;

	[SerializeField]
	private float lookAtSpeed = 1f;

	[SerializeField]
	private Animation anim;

	[SerializeField]
	private AnimationClip animationSurprised;

	[SerializeField]
	private AnimationClip animationHit;

	[SerializeField]
	private ParticleSystem impactParticle;

	[SerializeField]
	private AudioClip impactZapAudio;

	[SerializeField]
	private ItemCollectionZone hatCollectionZone;

	public Action<PickupableItem> OnItemPlacedOnHead;

	private void Start()
	{
		defaultPosition = base.transform.position;
	}

	private void OnEnable()
	{
		ItemCollectionZone itemCollectionZone = hatCollectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemPlacedOnHead));
	}

	private void OnDisable()
	{
		ItemCollectionZone itemCollectionZone = hatCollectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemPlacedOnHead));
	}

	private void Update()
	{
		if (playerHead == null)
		{
			playerHead = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform;
		}
		LookAtUpdate();
	}

	private void LookAtUpdate()
	{
		Vector3 position = lookAt;
		if (lookingAtPlayer)
		{
			position = playerHead.position;
		}
		Quaternion b = Quaternion.LookRotation(position - base.transform.position, base.transform.up);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, lookAtSpeed * Time.deltaTime);
		base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
	}

	public void SurprisedBy(Vector3 pos)
	{
		lookingAtPlayer = false;
		lookAt = pos;
		anim.clip = animationSurprised;
		anim.Play();
		Invoke("LookBackAtPlayer", UnityEngine.Random.Range(3f, 5f));
	}

	private void LookBackAtPlayer()
	{
		lookingAtPlayer = true;
	}

	public void MoveToDefaultPosition(float time, float delay)
	{
		MoveToWithY(defaultPosition, time, delay);
	}

	public void MoveToWithoutY(Vector3 position, float time, float delay)
	{
		position.y = 0f;
		Go.to(base.transform, time, new GoTweenConfig().position(position).setDelay(delay));
	}

	public void MoveToWithY(Vector3 position, float time, float delay)
	{
		Go.to(base.transform, time, new GoTweenConfig().position(position).setDelay(delay));
	}

	private void ItemPlacedOnHead(ItemCollectionZone zone, PickupableItem item)
	{
		if (OnItemPlacedOnHead != null)
		{
			OnItemPlacedOnHead(item);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.attachedRigidbody != null && (!anim.isPlaying || anim.clip != animationHit))
		{
			LookBackAtPlayer();
			AudioManager.Instance.Play(base.transform.position, impactZapAudio, 0.6f, 1f);
			anim.clip = animationHit;
			anim.Play();
			impactParticle.Play();
		}
	}
}
