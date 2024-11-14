using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(AttachableObject))]
public class BusinessCard : MonoBehaviour
{
	[SerializeField]
	private float unfurlDelay = 0.3f;

	[SerializeField]
	private float unfurlInterval = 0.15f;

	[SerializeField]
	private BusinessCardlet cardletPrefab;

	[SerializeField]
	private AudioClip unfurlSound;

	[SerializeField]
	private TriggerListener killTrigger;

	[SerializeField]
	private AudioClip poofSound;

	[SerializeField]
	private ParticleSystem poofParticles;

	private AttachableObject attachable;

	private GrabbableItem grabbable;

	private List<BusinessCardlet> cardlets = new List<BusinessCardlet>();

	private void Awake()
	{
		grabbable = GetComponent<GrabbableItem>();
		attachable = GetComponent<AttachableObject>();
		AttachableObject attachableObject = attachable;
		attachableObject.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnDetach, new Action<AttachableObject, AttachablePoint>(Detached));
		TriggerListener triggerListener = killTrigger;
		triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(triggerListener.OnEnter, new Action<TriggerEventInfo>(KillTriggerEntered));
	}

	public void Initialize(BusinessCardInfo cardInfo)
	{
		for (int i = 0; i < cardInfo.CardletInfos.Count; i++)
		{
			AddCardlet(cardInfo.CardletInfos[i]);
		}
	}

	private void AddCardlet(BusinessCardletInfo cardletInfo)
	{
		BusinessCardlet businessCardlet = UnityEngine.Object.Instantiate(cardletPrefab);
		businessCardlet.gameObject.RemoveCloneFromName();
		businessCardlet.transform.SetParent(base.transform, false);
		businessCardlet.transform.localPosition = Vector3.zero;
		businessCardlet.transform.localRotation = Quaternion.identity;
		businessCardlet.Initialize(cardletInfo, cardlets.Count);
		if (cardlets.Count == 0)
		{
			businessCardlet.SetAsRoot(grabbable, GetComponent<SelectedChangeOutlineController>());
		}
		else
		{
			businessCardlet.LinkTo(cardlets[cardlets.Count - 1], grabbable);
			businessCardlet.gameObject.SetActive(false);
		}
		cardlets.Add(businessCardlet);
	}

	private void Detached(AttachableObject obj, AttachablePoint point)
	{
		StartCoroutine(UnfurlAsync());
	}

	private IEnumerator UnfurlAsync()
	{
		for (int i = 1; i < cardlets.Count; i++)
		{
			yield return new WaitForSeconds((i != 1) ? unfurlInterval : unfurlDelay);
			cardlets[i].gameObject.SetActive(true);
			AudioManager.Instance.Play(cardlets[i].transform.position, unfurlSound, 1f, 1f + 0.25f * (float)i);
		}
	}

	private void KillTriggerEntered(TriggerEventInfo info)
	{
		if (!info.other.isTrigger && !grabbable.IsCurrInHand && !(attachable.CurrentlyAttachedTo != null))
		{
			AudioManager.Instance.Play(base.transform.position, poofSound, 1f, 1f);
			poofParticles.Play();
			GetComponent<Rigidbody>().isKinematic = true;
			for (int i = 0; i < cardlets.Count; i++)
			{
				cardlets[i].gameObject.SetActive(false);
			}
			UnityEngine.Object.Destroy(base.gameObject, 1f);
		}
	}
}
