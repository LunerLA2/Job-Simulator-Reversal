using System.Collections;
using TMPro;
using UnityEngine;

public class FlatTopStoveController : StoveController
{
	[SerializeField]
	private GrabbableSlider toolChooserSlider;

	[SerializeField]
	private KitchenToolStasher toolStasher;

	[SerializeField]
	private TextMeshPro toolLabel;

	[SerializeField]
	private string[] toolNames;

	[SerializeField]
	private bool hasSwitchedOnce;

	private void OnEnable()
	{
		toolChooserSlider.OnLowerLocked += SetToPot;
		toolChooserSlider.OnUpperLocked += SetToGrill;
	}

	private void OnDisable()
	{
		toolChooserSlider.OnLowerLocked -= SetToPot;
		toolChooserSlider.OnUpperLocked -= SetToGrill;
	}

	private void SetToPot(GrabbableSlider slider, bool isInitial)
	{
		if (!isInitial)
		{
			toolStasher.RequestModeChange(1);
			toolLabel.text = toolNames[1].ToUpper();
			if (hasSwitchedOnce)
			{
				StartCoroutine(TurnOffStoveAsync());
			}
			hasSwitchedOnce = true;
		}
	}

	private void SetToGrill(GrabbableSlider slider, bool isInitial)
	{
		if (!isInitial)
		{
			toolStasher.RequestModeChange(0);
			toolLabel.text = toolNames[0].ToUpper();
			if (hasSwitchedOnce)
			{
				StartCoroutine(TurnOffStoveAsync());
			}
			hasSwitchedOnce = true;
		}
	}

	private IEnumerator TurnOffStoveAsync()
	{
		if (base.HingeToDriveCookTemp.Grabbable.IsCurrInHand)
		{
			base.HingeToDriveCookTemp.Grabbable.CurrInteractableHand.ManuallyReleaseJoint();
		}
		base.HingeToDriveCookTemp.Unlock();
		float startAngle = base.HingeToDriveCookTemp.transform.localEulerAngles.z;
		float endAngle = 90f + base.HingeToDriveCookTemp.LowerLimit;
		float angleDiff = startAngle - (endAngle + 360f);
		float lerpAmt = 0f;
		if (angleDiff >= 0f && angleDiff <= 1f)
		{
			lerpAmt = 1f;
		}
		while (lerpAmt < 1f)
		{
			base.HingeToDriveCookTemp.transform.localEulerAngles = new Vector3(base.HingeToDriveCookTemp.transform.localEulerAngles.x, base.HingeToDriveCookTemp.transform.localEulerAngles.y, Mathf.Lerp(startAngle, endAngle, lerpAmt));
			lerpAmt += Time.deltaTime * 2f;
			yield return null;
		}
	}
}
