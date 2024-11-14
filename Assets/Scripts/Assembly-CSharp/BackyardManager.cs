using System;
using System.Collections;
using UnityEngine;

public class BackyardManager : MonoBehaviour
{
	[SerializeField]
	private ChildJobBot[] childJobBots;

	[SerializeField]
	private MagicReceiver[] magicThatImpressesChildren;

	[SerializeField]
	private Transform childMoveToFrontSpot;

	private int childGoingUpToFront;

	private bool childrenBusy;

	private void OnEnable()
	{
		for (int i = 0; i < magicThatImpressesChildren.Length; i++)
		{
			if (magicThatImpressesChildren[i] != null)
			{
				MagicReceiver obj = magicThatImpressesChildren[i];
				obj.OnMagicHappened = (Action<MagicReceiver>)Delegate.Combine(obj.OnMagicHappened, new Action<MagicReceiver>(ChildrenImpressedByMagic));
			}
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < magicThatImpressesChildren.Length; i++)
		{
			if (magicThatImpressesChildren[i] != null)
			{
				MagicReceiver obj = magicThatImpressesChildren[i];
				obj.OnMagicHappened = (Action<MagicReceiver>)Delegate.Remove(obj.OnMagicHappened, new Action<MagicReceiver>(ChildrenImpressedByMagic));
			}
		}
	}

	public void SendUpNextChild()
	{
		childrenBusy = true;
		StartCoroutine(ChildGoesUp());
	}

	private IEnumerator ChildGoesUp()
	{
		childrenBusy = true;
		if (childGoingUpToFront > 0)
		{
			childJobBots[childGoingUpToFront - 1].MoveToDefaultPosition(3f, 0f);
			if (childGoingUpToFront < childJobBots.Length)
			{
				childJobBots[childGoingUpToFront].MoveToWithY(childMoveToFrontSpot.position, 3f, 2f);
				childGoingUpToFront++;
			}
			yield return new WaitForSeconds(5f);
		}
		else
		{
			if (childGoingUpToFront < childJobBots.Length)
			{
				childJobBots[childGoingUpToFront].MoveToWithY(childMoveToFrontSpot.position, 3f, 0f);
				childGoingUpToFront++;
			}
			yield return new WaitForSeconds(3f);
		}
		childrenBusy = false;
	}

	private void ChildrenImpressedByMagic(MagicReceiver magicReceiver)
	{
		if (!childrenBusy)
		{
			StartCoroutine(ChildrenReactToMagic(magicReceiver));
		}
	}

	private IEnumerator ChildrenReactToMagic(MagicReceiver magicReceiver)
	{
		Vector3 storedPosition = magicReceiver.transform.position;
		childrenBusy = true;
		for (int i = 0; i < childJobBots.Length; i++)
		{
			childJobBots[i].SurprisedBy(storedPosition);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.1f));
		}
		yield return new WaitForSeconds(3f);
		childrenBusy = false;
	}
}
