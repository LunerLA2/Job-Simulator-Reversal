using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BotPathWaypoint : MonoBehaviour
{
	public enum ActionOnReached
	{
		none = 0,
		pause = 1,
		waitForSeconds = 2,
		customAction = 3
	}

	private Collider col;

	private BotPath path;

	public ActionOnReached actionOnHit;

	public UnityEvent OnCustomActionHit;

	public float waitForSeconds;

	public void Initialize(BotPath p)
	{
		path = p;
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(0.5f);
		if (path == null)
		{
			path = base.transform.parent.GetComponent<BotPath>();
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		BotLocomotion component = col.attachedRigidbody.GetComponent<BotLocomotion>();
		if (!(component == null) && !(component.CurrentPath != path))
		{
			if (actionOnHit == ActionOnReached.pause)
			{
				component.pauseTween();
			}
			if (actionOnHit == ActionOnReached.waitForSeconds)
			{
				StartCoroutine(StartWaitingForSeconds(component));
			}
			if (actionOnHit == ActionOnReached.customAction)
			{
				OnCustomActionHit.Invoke();
			}
		}
	}

	private IEnumerator StartWaitingForSeconds(BotLocomotion hitCharacter)
	{
		hitCharacter.pauseTween();
		yield return new WaitForSeconds(waitForSeconds);
		hitCharacter.resumeTween();
	}

	private void OnDrawGizmos()
	{
		col = GetComponent<Collider>();
		if (col == null)
		{
			col = base.gameObject.AddComponent<BoxCollider>();
		}
		col.isTrigger = true;
		base.transform.localScale = new Vector3(0.02f, 1f, 0.02f);
		if (path == null)
		{
			path = base.transform.parent.GetComponent<BotPath>();
		}
		if (!path.showGizmos)
		{
			return;
		}
		Gizmos.color = path.gizmoColor;
		int childCount = base.transform.parent.childCount;
		int num = 0;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.parent.GetChild(i).GetComponent<BotPathWaypoint>() == this)
			{
				num = i;
			}
		}
		if (num != childCount - 1)
		{
			Gizmos.DrawLine(base.transform.position, base.transform.parent.GetChild(num + 1).position);
		}
		Gizmos.DrawSphere(base.transform.position, 0.05f);
		if (actionOnHit != 0)
		{
			if (actionOnHit == ActionOnReached.pause)
			{
				Gizmos.color = Color.red;
			}
			if (actionOnHit == ActionOnReached.customAction)
			{
				Gizmos.color = new Color(0.4f, 0f, 0.8f);
			}
			if (actionOnHit == ActionOnReached.waitForSeconds)
			{
				Gizmos.color = Color.cyan;
			}
			Gizmos.DrawWireSphere(base.transform.position, 0.1f);
		}
	}
}
