using System;
using OwlchemyVR;
using UnityEngine;

public class CockroachController : MonoBehaviour
{
	[SerializeField]
	private CookableItem cookableItem;

	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private EdibleItem edibleItem;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private GameObject deathEffectPrefab;

	[SerializeField]
	private AudioClip deathSound;

	[SerializeField]
	private Transform[] legs;

	[SerializeField]
	private Transform[] leftSide;

	[SerializeField]
	private Transform[] rightSide;

	[SerializeField]
	private float smashSpeed;

	private Rigidbody rb;

	private bool isAlive = true;

	private bool isBeingHeld;

	private float moveTimer;

	private bool isLowGrav;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.NoGravityMode))
		{
			isLowGrav = true;
		}
	}

	private void OnEnable()
	{
		CookableItem obj = cookableItem;
		obj.OnCooked = (Action<CookableItem>)Delegate.Combine(obj.OnCooked, new Action<CookableItem>(Cooked));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj2.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem obj3 = grabbableItem;
		obj3.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj3.OnReleased, new Action<GrabbableItem>(Released));
		EdibleItem obj4 = edibleItem;
		obj4.OnBiteTaken = (Action<EdibleItem>)Delegate.Combine(obj4.OnBiteTaken, new Action<EdibleItem>(BiteTaken));
	}

	private void OnDisable()
	{
		CookableItem obj = cookableItem;
		obj.OnCooked = (Action<CookableItem>)Delegate.Remove(obj.OnCooked, new Action<CookableItem>(Cooked));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj2.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem obj3 = grabbableItem;
		obj3.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj3.OnReleased, new Action<GrabbableItem>(Released));
		EdibleItem obj4 = edibleItem;
		obj4.OnBiteTaken = (Action<EdibleItem>)Delegate.Remove(obj4.OnBiteTaken, new Action<EdibleItem>(BiteTaken));
	}

	private void OnApplicationQuit()
	{
		isAlive = false;
	}

	private void OnDestroy()
	{
		if (isAlive)
		{
			Death(true);
		}
	}

	private void Update()
	{
		if (!isAlive)
		{
			return;
		}
		if (!isBeingHeld)
		{
			for (int i = 0; i < legs.Length; i++)
			{
				legs[i].localEulerAngles = Vector3.forward * Mathf.Sin((Time.time + (float)i / 2f) * 15f) * 10f;
			}
			if (rb != null && Physics.Raycast(base.transform.position, -base.transform.up, 0.1f) && !isLowGrav)
			{
				rb.AddRelativeForce(Vector3.forward * 1f);
				moveTimer += Time.deltaTime;
				if (moveTimer > 0.1f)
				{
					rb.AddRelativeForce(new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(10f, 20f), UnityEngine.Random.Range(3f, 10f)));
					rb.AddTorque(Vector3.up * UnityEngine.Random.Range(-0.2f, 0.2f));
					moveTimer -= 0.1f;
				}
			}
		}
		else
		{
			for (int j = 0; j < legs.Length; j++)
			{
				legs[j].localEulerAngles = Vector3.forward * Mathf.Sin((Time.time + (float)j * 5f) * 25f) * 25f;
			}
		}
	}

	private void BiteTaken(EdibleItem item)
	{
		Death();
	}

	private void Grabbed(GrabbableItem item)
	{
		isBeingHeld = true;
	}

	private void Released(GrabbableItem item)
	{
		isBeingHeld = false;
	}

	private void Cooked(CookableItem item)
	{
		Death();
	}

	public void Kill()
	{
		Death();
	}

	private void Death(bool willBeDestroyed = false)
	{
		if (!isAlive)
		{
			return;
		}
		isAlive = false;
		if (!willBeDestroyed)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DESTROYED");
			for (int i = 0; i < leftSide.Length; i++)
			{
				Go.to(leftSide[i], UnityEngine.Random.Range(0.3f, 1f), new GoTweenConfig().localEulerAngles(Vector3.left * 65f));
			}
			for (int j = 0; j < rightSide.Length; j++)
			{
				Go.to(rightSide[j], UnityEngine.Random.Range(0.3f, 1f), new GoTweenConfig().localEulerAngles(Vector3.right * 65f));
			}
			UnityEngine.Object.Instantiate(deathEffectPrefab, base.transform.position, base.transform.rotation);
			AudioManager.Instance.Play(base.transform.position, deathSound, 1f, 1f);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.sqrMagnitude >= smashSpeed * smashSpeed)
		{
			Kill();
		}
	}
}
