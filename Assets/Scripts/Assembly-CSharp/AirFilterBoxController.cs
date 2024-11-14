using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class AirFilterBoxController : BrainControlledObject
{
	public const string RESPAWN_ID = "spawn";

	[SerializeField]
	private AttachablePoint[] spawnPoints;

	[SerializeField]
	private WorldItem myWorldItem;

	public override void Appear(BrainData brain)
	{
		base.Appear(brain);
		base.gameObject.SetActive(true);
	}

	public override void Disappear()
	{
		base.Disappear();
		base.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			spawnPoints[i].OnObjectWasAttached += AirFilterAttached;
			spawnPoints[i].OnObjectWasDetached += AirFilterDetached;
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			spawnPoints[i].OnObjectWasAttached -= AirFilterAttached;
			spawnPoints[i].OnObjectWasDetached -= AirFilterDetached;
		}
	}

	private void Start()
	{
		StartCoroutine(WaitAndSetupEventsForInitialState());
	}

	private IEnumerator WaitAndSetupEventsForInitialState()
	{
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			if (spawnPoints[i].NumAttachedObjects > 0)
			{
				AirFilterAttached(spawnPoints[i], spawnPoints[i].AttachedObjects[0]);
				spawnPoints[i].AttachedObjects[0].GetComponent<VehicleAirFilter>().SetDirty();
			}
		}
	}

	private void AirFilterAttached(AttachablePoint point, AttachableObject obj)
	{
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(obj.PickupableItem.InteractableItem.WorldItemData, myWorldItem.Data, "ADDED_TO");
	}

	private void AirFilterDetached(AttachablePoint point, AttachableObject obj)
	{
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(obj.PickupableItem.InteractableItem.WorldItemData, myWorldItem.Data, "REMOVED_FROM");
	}

	public override void ScriptedEffect(BrainEffect effect)
	{
		if (effect.TextInfo == "spawn")
		{
			RespawnOne();
		}
	}

	public override void MoveToPosition(string positionName, float moveDuration)
	{
		Transform transform = BotUniqueElementManager.Instance.GetObjectByName(positionName).transform;
		if (moveDuration > 0f)
		{
			float duration = Vector3.Distance(base.transform.position, transform.position);
			Go.to(base.transform, duration, new GoTweenConfig().position(transform.position).rotation(transform.rotation).setEaseType(GoEaseType.QuadInOut));
		}
		else
		{
			base.transform.position = transform.position;
			base.transform.rotation = transform.rotation;
		}
	}

	private void RespawnOne()
	{
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			if (spawnPoints[i].NumAttachedObjects == 0)
			{
				spawnPoints[i].RefillOneItemImmediate();
				spawnPoints[i].AttachedObjects[0].GetComponent<VehicleAirFilter>().SetDirty();
				break;
			}
		}
	}
}
