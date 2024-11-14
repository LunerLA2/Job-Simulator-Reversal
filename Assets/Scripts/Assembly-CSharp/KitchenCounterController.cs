using OwlchemyVR2;
using UnityEngine;

public class KitchenCounterController : MonoBehaviour
{
	[SerializeField]
	private KitchenToolStasher toolChooser;

	[SerializeField]
	private OwlchemyVR2.GrabbableHinge dial;

	[SerializeField]
	private GameObject[] optionOnLights;

	[SerializeField]
	private GameObject[] optionOffLights;

	[SerializeField]
	private bool forceSetSnapIndexOnEnable;

	private bool didStart;

	public OwlchemyVR2.GrabbableHinge Dial
	{
		get
		{
			return dial;
		}
	}

	private void OnEnable()
	{
		dial.OnReleasedAtSnapIndex += Dial_OnReleasedAtSnapIndex;
		if (forceSetSnapIndexOnEnable && didStart)
		{
			dial.SetSnapIndex(dial.InitialSnapIndex, true);
		}
		Dial_OnReleasedAtSnapIndex(dial, dial.InitialSnapIndex);
	}

	private void OnDisable()
	{
		dial.OnReleasedAtSnapIndex -= Dial_OnReleasedAtSnapIndex;
	}

	private void Start()
	{
		didStart = true;
	}

	public void ManuallyRefresh()
	{
		InternalRefresh(dial.CurrentSnapIndex);
	}

	private void Dial_OnReleasedAtSnapIndex(LimitedIndirectGrabbableController dial, int selectionIndex)
	{
		InternalRefresh(selectionIndex);
	}

	private void InternalRefresh(int selectionIndex)
	{
		toolChooser.RequestModeChange(selectionIndex);
		for (int i = 0; i < optionOnLights.Length; i++)
		{
			optionOnLights[i].SetActive(i == selectionIndex);
			optionOffLights[i].SetActive(i != selectionIndex);
		}
	}
}
