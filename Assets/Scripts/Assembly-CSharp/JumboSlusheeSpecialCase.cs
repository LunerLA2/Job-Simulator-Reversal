using System;
using OwlchemyVR;
using UnityEngine;

public class JumboSlusheeSpecialCase : MonoBehaviour
{
	[SerializeField]
	private WorldItemData slusheeCupFilledWID;

	[SerializeField]
	private WorldItemData jumboSizerWID;

	[SerializeField]
	private float jumboScaleX = 1.5f;

	[SerializeField]
	private ContainerFluidSystem containerFluid;

	private void OnEnable()
	{
		ContainerFluidSystem containerFluidSystem = containerFluid;
		containerFluidSystem.OnFluidPartiallyFilled = (Action<ContainerFluidSystem>)Delegate.Combine(containerFluidSystem.OnFluidPartiallyFilled, new Action<ContainerFluidSystem>(FluidFilled));
	}

	private void OnDisable()
	{
		ContainerFluidSystem containerFluidSystem = containerFluid;
		containerFluidSystem.OnFluidPartiallyFilled = (Action<ContainerFluidSystem>)Delegate.Remove(containerFluidSystem.OnFluidPartiallyFilled, new Action<ContainerFluidSystem>(FluidFilled));
	}

	private void FluidFilled(ContainerFluidSystem fluid)
	{
		if (base.transform.localScale.x >= jumboScaleX && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(slusheeCupFilledWID, jumboSizerWID, "CREATED_BY");
		}
	}
}
