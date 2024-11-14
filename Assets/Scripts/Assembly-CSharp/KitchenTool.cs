using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(WorldItem))]
public abstract class KitchenTool : MonoBehaviour
{
	[SerializeField]
	private bool useItemStashingLogic = true;

	[SerializeField]
	private BoxCollider counterHole;

	protected bool isToolBusy;

	protected KitchenToolStasher parentStasher;

	public bool UseItemStashingLogic
	{
		get
		{
			return useItemStashingLogic;
		}
	}

	public BoxCollider CounterHole
	{
		get
		{
			return counterHole;
		}
	}

	public bool IsToolBusy
	{
		get
		{
			return isToolBusy;
		}
	}

	public void SetParentChooser(KitchenToolStasher stasher)
	{
		parentStasher = stasher;
	}

	public virtual void OnDismiss()
	{
	}

	public virtual void OnSummon()
	{
	}

	public virtual void WasCompletelyDismissed()
	{
	}

	public virtual void BeganBeingSummoned()
	{
	}
}
