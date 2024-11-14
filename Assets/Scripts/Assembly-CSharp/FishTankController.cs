using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class FishTankController : MonoBehaviour
{
	private const float MIN_FISH_SPEED = 0.005f;

	private const float MAX_FISH_SPEED = 0.01f;

	private const float MAX_FISH_SCALE = 1.3f;

	private const float MIN_FISH_SCALE = 0.5f;

	[SerializeField]
	private int numberOfFish = 3;

	[SerializeField]
	private Collider fishBounds;

	[SerializeField]
	private GameObject fishPrefab;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents rigidbodyTriggerEvents;

	[SerializeField]
	private ParticleSystem debris;

	[SerializeField]
	private ParticleSystem psvrDebris;

	[SerializeField]
	private AudioClip fishShredderClip;

	private Color defaultDebrisColor;

	private List<Transform> fish;

	private List<Rigidbody> floatingObjects;

	private Vector3 fishBoundsScale;

	private float fishBoundsX;

	private float fishBoundsY;

	private float fishBoundsZ;

	private Rigidbody foodItem;

	private void Start()
	{
		fishBoundsScale = fishBounds.transform.localScale;
		fishBoundsX = fishBoundsScale.x / 2f;
		fishBoundsY = fishBoundsScale.y / 2f;
		fishBoundsZ = fishBoundsScale.z / 2f;
		floatingObjects = new List<Rigidbody>();
		fish = new List<Transform>(numberOfFish);
		defaultDebrisColor = debris.startColor;
		for (int i = 0; i < numberOfFish; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(fishPrefab, fishBounds.transform.position, fishBounds.transform.rotation) as GameObject;
			gameObject.transform.localScale = gameObject.transform.localScale * UnityEngine.Random.Range(0.5f, 1.3f);
			gameObject.transform.SetParent(base.transform);
			fish.Add(gameObject.transform);
			StartCoroutine(FishToNewPoint(i));
		}
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(OnRigidbodyTriggerEnter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(OnRigidbodyTriggerExit));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(OnRigidbodyTriggerEnter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(OnRigidbodyTriggerExit));
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < floatingObjects.Count; i++)
		{
			if (floatingObjects[i] != null)
			{
				floatingObjects[i].velocity = Vector3.Lerp(floatingObjects[i].velocity, Vector3.down * 0.4f, 0.08f);
			}
			else
			{
				floatingObjects.RemoveAt(i);
			}
		}
	}

	private void OnRigidbodyTriggerEnter(Rigidbody rb)
	{
		floatingObjects.Add(rb);
		rb.useGravity = false;
		if (rb.gameObject.GetComponent<BasicEdibleItem>() != null)
		{
			foodItem = rb;
		}
	}

	private void OnRigidbodyTriggerExit(Rigidbody rb)
	{
		rb.useGravity = true;
		floatingObjects.Remove(rb);
		if (rb == foodItem)
		{
			foodItem = null;
		}
	}

	private IEnumerator FishToNewPoint(int fishIndex)
	{
		Vector3 targetPosition = new Vector3
		{
			x = fishBounds.transform.position.x + UnityEngine.Random.Range(0f - fishBoundsX, fishBoundsX),
			y = fishBounds.transform.position.y + UnityEngine.Random.Range(0f - fishBoundsY, fishBoundsY),
			z = fishBounds.transform.position.z + UnityEngine.Random.Range(0f - fishBoundsZ, fishBoundsZ)
		};
		fish[fishIndex].transform.LookAt(targetPosition);
		float moveSpeed = UnityEngine.Random.Range(0.005f, 0.01f);
		while (Vector3.Distance(fish[fishIndex].position, targetPosition) > 0.1f && foodItem == null)
		{
			fish[fishIndex].position = Vector3.Lerp(fish[fishIndex].position, targetPosition, moveSpeed);
			yield return null;
		}
		yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.2f));
		if (foodItem != null)
		{
			StartCoroutine(FishToFoodItem(fishIndex));
		}
		else
		{
			StartCoroutine(FishToNewPoint(fishIndex));
		}
	}

	private IEnumerator FishToFoodItem(int fishIndex)
	{
		while (foodItem != null && Vector3.Distance(fish[fishIndex].position, foodItem.gameObject.transform.position) > 0.2f)
		{
			fish[fishIndex].position = Vector3.Lerp(fish[fishIndex].position, foodItem.gameObject.transform.position, 0.01f);
			fish[fishIndex].transform.LookAt(foodItem.gameObject.transform.position);
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		if (foodItem != null)
		{
			WorldItem wi = foodItem.gameObject.GetComponent<WorldItem>();
			WorldItemData worldItemData = null;
			if (wi != null)
			{
				worldItemData = wi.Data;
			}
			if (worldItemData != null)
			{
				Color debrisColor = worldItemData.OverallColor;
				if (debrisColor.a == 0f)
				{
					debrisColor = defaultDebrisColor;
				}
				debris.startColor = debrisColor;
				GameEventsManager.Instance.ItemActionOccurred(worldItemData, "DESTROYED");
			}
			else
			{
				debris.startColor = defaultDebrisColor;
			}
			debris.transform.position = foodItem.transform.position;
			debris.Play();
			AudioManager.Instance.Play(debris.transform.position, fishShredderClip, 1f, 1f);
			floatingObjects.Remove(foodItem);
			UnityEngine.Object.Destroy(foodItem.gameObject);
		}
		StartCoroutine(FishToNewPoint(fishIndex));
	}
}
