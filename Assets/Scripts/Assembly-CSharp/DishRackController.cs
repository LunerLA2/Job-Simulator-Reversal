using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishRackController : BrainControlledObject
{
	public const string RESPAWN_PLATE_ID = "spawnPlate";

	[SerializeField]
	private AttachableStackPoint[] plateStacks;

	[SerializeField]
	private int platesToMaintain = 2;

	private List<AttachableObject> platesInWorld = new List<AttachableObject>();

	private bool maintainingPlates;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(1f);
		platesInWorld = new List<AttachableObject>();
		for (int i = 0; i < plateStacks.Length; i++)
		{
			if (plateStacks[i].AttachedObjects.Count > 0)
			{
				platesInWorld.Add(plateStacks[i].AttachedObjects[0]);
			}
		}
		yield return new WaitForSeconds(1f);
		maintainingPlates = true;
	}

	public override void Appear(BrainData brain)
	{
		base.Appear(brain);
		base.gameObject.SetActive(true);
	}

	public override void Disappear()
	{
		base.Disappear();
		base.gameObject.SetActive(false);
		maintainingPlates = false;
	}

	private void Update()
	{
		if (!maintainingPlates)
		{
			return;
		}
		for (int i = 0; i < platesInWorld.Count; i++)
		{
			if (platesInWorld[i] == null)
			{
				platesInWorld.RemoveAt(i);
				i--;
			}
		}
		if (platesInWorld.Count < platesToMaintain)
		{
			RespawnOnePlate();
		}
	}

	public override void ScriptedEffect(BrainEffect effect)
	{
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

	private void RespawnOnePlate()
	{
		for (int i = 0; i < plateStacks.Length; i++)
		{
			if (plateStacks[i].NumAttachedObjects == 0)
			{
				plateStacks[i].RefillOneItemImmediate();
				platesInWorld.Add(plateStacks[i].AttachedObjects[0]);
				break;
			}
		}
	}
}
