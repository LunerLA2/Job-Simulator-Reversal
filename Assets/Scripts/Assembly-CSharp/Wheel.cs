using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class Wheel : MonoBehaviour
{
	public enum InflationState
	{
		None = 0,
		Inflate = 1,
		Deflate = 2
	}

	private const float inflationRate = 0.4f;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private SelectedChangeOutlineController outline;

	[SerializeField]
	private Transform tire;

	[SerializeField]
	private Transform hubcap;

	[SerializeField]
	private AnimatedBolt[] animatedBolts;

	[SerializeField]
	private AttachablePointData wheelAttachPointData;

	[SerializeField]
	private ParticleSystem deflateParticle;

	[SerializeField]
	private AudioSourceHelper deflateSound;

	private Vector3 initialScale;

	private GoTween tireInflationTween;

	private InflationState inflating;

	private float currentInflationSize = 1f;

	[SerializeField]
	private InflationState initialInflation = InflationState.Inflate;

	[SerializeField]
	private float minTireInflationSize = 0.7f;

	[SerializeField]
	private float maxTireInflationSize = 1f;

	[SerializeField]
	private AttachableObject wheelAttach;

	[SerializeField]
	private AttachablePoint inflationAttachPoint;

	private bool inflatedEventState;

	private bool canInflate;

	public float CurrentInflationPercentage
	{
		get
		{
			return Mathf.InverseLerp(minTireInflationSize, maxTireInflationSize, currentInflationSize);
		}
	}

	public void SetInflationPercentage(float perc)
	{
		currentInflationSize = Mathf.Lerp(minTireInflationSize, maxTireInflationSize, perc);
		CheckForActivationEvent();
	}

	private void Awake()
	{
		currentInflationSize = ((initialInflation != InflationState.Inflate) ? minTireInflationSize : maxTireInflationSize);
		CheckForActivationEvent(true);
		inflating = InflationState.None;
		deflateParticle.Stop();
		deflateSound.enabled = false;
	}

	public void Setup(WorldItemData _wi, bool _canInflate, Mesh _tireMesh, Material _tireMat, Material _hubcapMat)
	{
		List<MeshFilter> list = new List<MeshFilter>();
		myWorldItem.ManualSetData(_wi);
		canInflate = _canInflate;
		tire.GetComponent<MeshFilter>().sharedMesh = _tireMesh;
		tire.GetComponent<MeshRenderer>().sharedMaterial = _tireMat;
		list.Add(tire.GetComponent<MeshFilter>());
		if (!_canInflate)
		{
			if (inflationAttachPoint != null)
			{
				UnityEngine.Object.Destroy(inflationAttachPoint.gameObject);
			}
			if (hubcap != null)
			{
				UnityEngine.Object.Destroy(hubcap.gameObject);
			}
			tire.localScale = Vector3.one;
		}
		else
		{
			hubcap.GetComponent<MeshRenderer>().sharedMaterial = _hubcapMat;
			list.Add(hubcap.GetComponent<MeshFilter>());
		}
		for (int i = 0; i < animatedBolts.Length; i++)
		{
			list.Add(animatedBolts[i].GetComponentInChildren<MeshFilter>());
		}
		outline.meshFilters = list.ToArray();
		outline.Build();
	}

	private void Start()
	{
		AttachablePoint currentlyAttachedTo = wheelAttach.CurrentlyAttachedTo;
		if (currentlyAttachedTo != null && currentlyAttachedTo.Data == wheelAttachPointData)
		{
			SpawnBolts();
		}
	}

	private void OnEnable()
	{
		AttachableObject attachableObject = wheelAttach;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(WheelAttached));
		AttachableObject attachableObject2 = wheelAttach;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(WheelDetached));
		if (wheelAttach.CurrentlyAttachedTo == null && inflationAttachPoint != null)
		{
			inflationAttachPoint.enabled = false;
		}
	}

	private void OnDisable()
	{
		AttachableObject attachableObject = wheelAttach;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(WheelAttached));
		AttachableObject attachableObject2 = wheelAttach;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(WheelDetached));
	}

	private void WheelAttached(AttachableObject obj, AttachablePoint point)
	{
		if (point.Data == wheelAttachPointData)
		{
			SpawnBolts();
		}
		if (inflationAttachPoint != null)
		{
			inflationAttachPoint.enabled = true;
		}
		StartInflation(InflationState.Inflate);
	}

	private void WheelDetached(AttachableObject obj, AttachablePoint point)
	{
		if (inflationAttachPoint != null)
		{
			if (inflationAttachPoint.NumAttachedObjects > 0)
			{
				inflationAttachPoint.Detach(inflationAttachPoint.AttachedObjects[0]);
			}
			inflationAttachPoint.enabled = false;
		}
		if (point.Data == wheelAttachPointData)
		{
			DespawnBolts();
		}
		if (CurrentInflationPercentage > 0f)
		{
			StartInflation(InflationState.Deflate);
		}
	}

	private void Update()
	{
		if (!canInflate)
		{
			return;
		}
		if (inflating == InflationState.Inflate)
		{
			InflateOneFrame();
		}
		else if (inflating == InflationState.Deflate)
		{
			DeflateOneFrame();
			if (!deflateParticle.isPlaying)
			{
				deflateParticle.Play();
			}
			if (!deflateSound.IsPlaying)
			{
				deflateSound.enabled = true;
				deflateSound.Play();
			}
		}
		else
		{
			if (deflateParticle.isPlaying)
			{
				deflateParticle.Stop();
			}
			if (deflateSound.IsPlaying)
			{
				deflateSound.Stop();
				deflateSound.enabled = false;
			}
		}
		tire.localScale = new Vector3(tire.localScale.x, currentInflationSize, currentInflationSize);
	}

	private void InflateOneFrame()
	{
		currentInflationSize = Mathf.Min(maxTireInflationSize, currentInflationSize + 0.4f * Time.deltaTime);
		CheckForActivationEvent();
		if (currentInflationSize >= maxTireInflationSize)
		{
			inflating = InflationState.None;
		}
	}

	private void DeflateOneFrame()
	{
		currentInflationSize = Mathf.Max(minTireInflationSize, currentInflationSize - 0.4f * Time.deltaTime);
		CheckForActivationEvent();
		if (currentInflationSize <= minTireInflationSize)
		{
			inflating = InflationState.None;
		}
	}

	private void CheckForActivationEvent(bool forceEvent = false)
	{
		if (currentInflationSize < maxTireInflationSize)
		{
			if (inflatedEventState || forceEvent)
			{
				inflatedEventState = false;
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
			}
		}
		else if (!inflatedEventState || forceEvent)
		{
			inflatedEventState = true;
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
		}
	}

	public void StartInflation(InflationState inflation)
	{
		inflating = inflation;
	}

	public void StopInflation()
	{
		inflating = InflationState.None;
	}

	private void SpawnBolts()
	{
		for (int i = 0; i < animatedBolts.Length; i++)
		{
			animatedBolts[i].SpawnBolt();
		}
	}

	private void DespawnBolts()
	{
		for (int i = 0; i < animatedBolts.Length; i++)
		{
			animatedBolts[i].DespawnBolt();
		}
	}
}
