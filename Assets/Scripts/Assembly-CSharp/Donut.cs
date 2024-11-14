using OwlchemyVR;
using UnityEngine;

public class Donut : EdibleItem
{
	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private ParticleSystem crumbParticle;

	[SerializeField]
	private Transform activeWhenWhole;

	[SerializeField]
	private Transform activeWhenBitten;

	[SerializeField]
	private MeshFilter wholeMeshFilter;

	[SerializeField]
	private MeshFilter bittenMeshFilter;

	[SerializeField]
	private Transform colliderTransform;

	[SerializeField]
	private Transform colliderTransformNormalPos;

	[SerializeField]
	private Transform colliderTransformBittenPos;

	[SerializeField]
	private MeshRenderer[] renderers;

	[SerializeField]
	private SelectedChangeOutlineController outline;

	private bool isBitten;

	private bool wasSetUp;

	private void Start()
	{
		if (!wasSetUp)
		{
			SetupDonut(wholeMeshFilter.mesh, bittenMeshFilter.mesh, renderers[0].material, false);
		}
	}

	public void SetupDonut(Mesh _wholeMesh, Mesh _bittenMesh, Material _mat, bool _startsBitten)
	{
		wholeMeshFilter.mesh = _wholeMesh;
		bittenMeshFilter.mesh = _bittenMesh;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material = _mat;
		}
		SetBittenState(_startsBitten);
		wasSetUp = true;
	}

	public override BiteResultInfo TakeBiteAndGetResult(HeadController head)
	{
		AttachableObject component = GetComponent<AttachableObject>();
		if (component != null && component.CurrentlyAttachedTo != null)
		{
			if (component.CurrentlyAttachedTo.IsRefilling)
			{
				return new BiteResultInfo(false, null, null);
			}
			component.Detach();
		}
		BiteTakenEvent();
		if (!isBitten)
		{
			SetBittenState(true);
			crumbParticle.Play();
			Quaternion rotation = Quaternion.LookRotation(-(base.transform.position - head.transform.position).normalized);
			if (pickupableItem.IsCurrInHand)
			{
				pickupableItem.CurrInteractableHand.ReorientCurrItemInHand(base.transform.position, rotation);
			}
			return new BiteResultInfo(false, null, null);
		}
		if (pickupableItem.CurrInteractableHand != null)
		{
			pickupableItem.CurrInteractableHand.ManuallyReleaseJoint();
		}
		crumbParticle.Play();
		crumbParticle.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		ItemConsumedEvent();
		Object.Destroy(crumbParticle.gameObject, 2f);
		Object.Destroy(base.gameObject, 0.001f);
		return new BiteResultInfo(true, null, null);
	}

	private void SetBittenState(bool bitten)
	{
		isBitten = bitten;
		activeWhenWhole.gameObject.SetActive(!isBitten);
		activeWhenBitten.gameObject.SetActive(isBitten);
		colliderTransform.localPosition = ((!isBitten) ? colliderTransformNormalPos.localPosition : colliderTransformBittenPos.localPosition);
		colliderTransform.localRotation = ((!isBitten) ? colliderTransformNormalPos.localRotation : colliderTransformBittenPos.localRotation);
		colliderTransform.localScale = ((!isBitten) ? colliderTransformNormalPos.localScale : colliderTransformBittenPos.localScale);
		outline.ForceRefreshMeshes();
	}
}
