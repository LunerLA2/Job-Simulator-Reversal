using UnityEngine;

public class AirPumpInteraction : MonoBehaviour
{
	[SerializeField]
	private Wheel wheel;

	private void OnTriggerEnter(Collider otherObj)
	{
		AirPumpAttachment component = otherObj.GetComponent<AirPumpAttachment>();
		if (component != null)
		{
			wheel.StartInflation(Wheel.InflationState.Inflate);
		}
		VacuumAttachment component2 = otherObj.GetComponent<VacuumAttachment>();
		if (component2 != null)
		{
			wheel.StartInflation(Wheel.InflationState.Deflate);
		}
	}

	private void OnTriggerExit(Collider otherObj)
	{
		AirPumpAttachment component = otherObj.GetComponent<AirPumpAttachment>();
		if (component != null)
		{
			wheel.StopInflation();
		}
		VacuumAttachment component2 = otherObj.GetComponent<VacuumAttachment>();
		if (component2 != null)
		{
			wheel.StopInflation();
		}
	}
}
