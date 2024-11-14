using System;
using UnityEngine;

public class MenuPreviewer : MonoBehaviour
{
	private const float lastCollideTimeDelay = 0.05f;

	private bool isPreviewing;

	private bool rightHand = true;

	[SerializeField]
	private MeshRenderer rendererBottom;

	[SerializeField]
	private MeshRenderer rendererTop;

	[SerializeField]
	private MeshRenderer rendererHandle;

	[SerializeField]
	private TriggerListener triggerListener;

	private float lastCollideTime;

	private bool currentlyRumbling;

	private HapticInfoObject invalidPlacementRumble;

	[SerializeField]
	private Transform visuals;

	private GoTween appearTween;

	private void Start()
	{
		invalidPlacementRumble = new HapticInfoObject(450f);
		lastCollideTime = Time.realtimeSinceStartup;
		TriggerListener obj = triggerListener;
		obj.OnStay = (Action<TriggerEventInfo>)Delegate.Combine(obj.OnStay, new Action<TriggerEventInfo>(OnTriggerListenerStay));
	}

	private void OnDestroy()
	{
		TriggerListener obj = triggerListener;
		obj.OnStay = (Action<TriggerEventInfo>)Delegate.Remove(obj.OnStay, new Action<TriggerEventInfo>(OnTriggerListenerStay));
	}

	private void Update()
	{
		if (isPreviewing)
		{
			UpdatePreviewState();
		}
	}

	private void UpdatePreviewState()
	{
		if (IsInValidPosition())
		{
			if (currentlyRumbling)
			{
				if (rightHand)
				{
					GlobalStorage.Instance.MasterHMDAndInputController.RightHand.HapticsController.RemoveHaptic(invalidPlacementRumble);
				}
				else
				{
					GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.HapticsController.RemoveHaptic(invalidPlacementRumble);
				}
			}
			currentlyRumbling = false;
			Color color = new Color(0f, 1f, 0f, 0.5f);
			rendererTop.material.SetColor("_OutlineColor", color);
			rendererBottom.material.SetColor("_OutlineColor", color);
			rendererHandle.material.SetColor("_OutlineColor", color);
			return;
		}
		if (!currentlyRumbling)
		{
			if (rightHand)
			{
				invalidPlacementRumble = GlobalStorage.Instance.MasterHMDAndInputController.RightHand.HapticsController.AddNewHaptic(invalidPlacementRumble);
			}
			else
			{
				invalidPlacementRumble = GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.HapticsController.AddNewHaptic(invalidPlacementRumble);
			}
		}
		currentlyRumbling = true;
		Color color2 = new Color(1f, 0f, 0f, 0.5f);
		rendererTop.material.SetColor("_OutlineColor", color2);
		rendererBottom.material.SetColor("_OutlineColor", color2);
		rendererHandle.material.SetColor("_OutlineColor", color2);
	}

	public void StartPreviewing(bool isRightHand)
	{
		if (!isPreviewing)
		{
			isPreviewing = true;
			base.gameObject.SetActive(true);
			rightHand = isRightHand;
			HapticInfoObject newHaptic = new HapticInfoObject(800f, 0.1f);
			if (rightHand)
			{
				GlobalStorage.Instance.MasterHMDAndInputController.RightHand.HapticsController.AddNewHaptic(newHaptic);
				GlobalStorage.Instance.MasterHMDAndInputController.RightHand.DeactivateHandInteractions();
			}
			else
			{
				GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.HapticsController.AddNewHaptic(newHaptic);
				GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.DeactivateHandInteractions();
			}
			visuals.localScale = Vector3.zero;
			appearTween = Go.to(visuals, 0.3f, new GoTweenConfig().scale(1f).setEaseType(GoEaseType.BackOut));
		}
	}

	public bool StopPreviewing(bool isRightHand)
	{
		if (!isPreviewing)
		{
			return false;
		}
		if (rightHand != isRightHand)
		{
			return false;
		}
		isPreviewing = false;
		if (rightHand)
		{
			GlobalStorage.Instance.MasterHMDAndInputController.RightHand.ReactivateHandInteractions();
		}
		else
		{
			GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.ReactivateHandInteractions();
		}
		if (currentlyRumbling)
		{
			if (rightHand)
			{
				GlobalStorage.Instance.MasterHMDAndInputController.RightHand.HapticsController.RemoveHaptic(invalidPlacementRumble);
			}
			else
			{
				GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.HapticsController.RemoveHaptic(invalidPlacementRumble);
			}
		}
		currentlyRumbling = false;
		if (IsInValidPosition())
		{
			HapticInfoObject newHaptic = new HapticInfoObject(500f, 0.1f);
			if (rightHand)
			{
				GlobalStorage.Instance.MasterHMDAndInputController.RightHand.HapticsController.AddNewHaptic(newHaptic);
			}
			else
			{
				GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.HapticsController.AddNewHaptic(newHaptic);
			}
			base.gameObject.SetActive(false);
			return true;
		}
		if (appearTween != null)
		{
			appearTween.destroy();
			appearTween = null;
		}
		Go.to(visuals, 0.3f, new GoTweenConfig().scale(0f).setEaseType(GoEaseType.BackIn).onIterationEnd(StopPreviewingEndCallback));
		return false;
	}

	private void StopPreviewingEndCallback(AbstractGoTween tween)
	{
		if (!isPreviewing)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnTriggerListenerStay(TriggerEventInfo info)
	{
		if (!info.other.isTrigger)
		{
			lastCollideTime = Time.realtimeSinceStartup;
		}
	}

	private bool IsInValidPosition()
	{
		bool flag = Time.realtimeSinceStartup - lastCollideTime > 0.05f;
		bool result = flag;
		if (GlobalStorage.Instance.MasterHMDAndInputController != null && base.transform.position.y > GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform.position.y)
		{
			result = false;
		}
		return result;
	}

	public Vector3 GetPosition(bool forceHandedness = false, bool forceIsRightHand = false)
	{
		bool flag = rightHand;
		if (forceHandedness)
		{
			flag = forceIsRightHand;
		}
		Vector3 result = Vector3.zero;
		if (flag)
		{
			if (GlobalStorage.Instance.MasterHMDAndInputController.RightHand != null)
			{
				result = GlobalStorage.Instance.MasterHMDAndInputController.RightHand.transform.position;
			}
			else
			{
				Debug.LogWarning("Could not find tracked right hand position when using the menu");
			}
		}
		else if (GlobalStorage.Instance.MasterHMDAndInputController.LeftHand != null)
		{
			result = GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.transform.position;
		}
		else
		{
			Debug.LogWarning("Could not find tracked left hand position when using the menu");
		}
		return result;
	}

	public Quaternion GetRotation()
	{
		Quaternion result = Quaternion.identity;
		if (rightHand)
		{
			if (GlobalStorage.Instance.MasterHMDAndInputController.RightHand != null)
			{
				result = Quaternion.Euler(0f, GlobalStorage.Instance.MasterHMDAndInputController.RightHand.transform.rotation.eulerAngles.y, 0f);
			}
		}
		else if (GlobalStorage.Instance.MasterHMDAndInputController.LeftHand != null)
		{
			result = Quaternion.Euler(0f, GlobalStorage.Instance.MasterHMDAndInputController.LeftHand.transform.rotation.eulerAngles.y, 0f);
		}
		result.eulerAngles = new Vector3(0f, result.eulerAngles.y - 90f, 0f);
		return result;
	}
}
