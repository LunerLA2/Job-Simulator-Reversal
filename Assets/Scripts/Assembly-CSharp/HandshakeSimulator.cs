using System;
using System.Collections;
using OwlchemyVR;
using OwlchemyVR2;
using UnityEngine;

public class HandshakeSimulator : MonoBehaviour
{
	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private OwlchemyVR2.GrabbableSlider grabbableSlider;

	private GrabbableItem slider;

	private float sliderNormalizedPosition;

	private bool hasEntered;

	private bool hasExited;

	[SerializeField]
	private Animation anim;

	[SerializeField]
	private AnimationClip enterClip;

	[SerializeField]
	private AnimationClip exitClip;

	private Coroutine enterCouroutine;

	private Coroutine exitCoroutine;

	public bool IsCurrentlyEntering
	{
		get
		{
			return enterCouroutine != null;
		}
	}

	public bool IsCurrentlyExiting
	{
		get
		{
			return exitCoroutine != null;
		}
	}

	private void OnEnable()
	{
		slider = grabbableSlider.Grabbable;
		sliderNormalizedPosition = grabbableSlider.NormalizedAxisValue;
		GrabbableItem grabbableItem = slider;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(OnGrabbed));
		grabbableSlider.OnUpperLocked += ActivatedShake;
		grabbableSlider.OnLowerLocked += ActivatedShake;
	}

	private void ActivatedShake(OwlchemyVR2.GrabbableSlider arg1, bool arg2)
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "USED");
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = slider;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(OnGrabbed));
		grabbableSlider.OnUpperLocked -= ActivatedShake;
		grabbableSlider.OnLowerLocked -= ActivatedShake;
	}

	private void OnGrabbed(GrabbableItem grabbableItem)
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
	}

	public void Enter(float delay = 0f)
	{
		base.gameObject.SetActive(true);
		hasEntered = false;
		enterCouroutine = StartCoroutine(EnterAsync(delay));
	}

	public void Exit(float delay = 0f)
	{
		hasExited = false;
		exitCoroutine = StartCoroutine(ExitAsync(delay));
	}

	private IEnumerator EnterAsync(float delay)
	{
		yield return new WaitForSeconds(delay);
		anim.clip = enterClip;
		float length = anim.clip.length;
		float timer = 0f;
		anim.Play();
		for (; timer < length; timer += Time.deltaTime)
		{
			yield return null;
		}
		enterCouroutine = null;
		hasEntered = true;
	}

	private IEnumerator ExitAsync(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (enterCouroutine != null)
		{
			yield return enterCouroutine;
		}
		anim.clip = exitClip;
		float length = anim.clip.length;
		float timer = 0f;
		anim.Play();
		for (; timer < length; timer += Time.deltaTime)
		{
			yield return null;
		}
		exitCoroutine = null;
		hasExited = true;
		base.gameObject.SetActive(false);
	}
}
