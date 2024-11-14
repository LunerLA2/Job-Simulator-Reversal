using OwlchemyVR;
using UnityEngine;

public class CandyBar : EdibleItem
{
	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private ParticleSystem crumbParticle;

	[SerializeField]
	private MeshFilter wholeMeshFilter;

	[SerializeField]
	private MeshFilter afterFirstBiteMeshFilter;

	[SerializeField]
	private MeshFilter afterSecondBiteMeshFilter;

	[SerializeField]
	private Transform colliderTransform;

	[SerializeField]
	private Transform colliderTransformNormalPos;

	[SerializeField]
	private Transform colliderTransformBiteOnePos;

	[SerializeField]
	private Transform colliderTransformBiteTwoPos;

	[SerializeField]
	private SelectedChangeOutlineController outline;

	private bool wasSetUp;

	private int numBitesTaken;

	private void Start()
	{
		if (!wasSetUp)
		{
			SetupCandyBar(wholeMeshFilter.mesh, afterFirstBiteMeshFilter.mesh, afterSecondBiteMeshFilter.mesh);
		}
	}

	public void SetupCandyBar(Mesh _wholeMesh, Mesh _firstBiteMesh, Mesh _secondBiteMesh)
	{
		wholeMeshFilter.mesh = _wholeMesh;
		afterFirstBiteMeshFilter.mesh = _firstBiteMesh;
		afterSecondBiteMeshFilter.mesh = _secondBiteMesh;
		SetNumberOfBitesTaken(0);
		wasSetUp = true;
	}

	public override BiteResultInfo TakeBiteAndGetResult(HeadController head)
	{
		SetNumberOfBitesTaken(numBitesTaken + 1);
		crumbParticle.Play();
		if (numBitesTaken >= 3)
		{
			if (pickupableItem.CurrInteractableHand != null)
			{
				pickupableItem.CurrInteractableHand.ManuallyReleaseJoint();
			}
			crumbParticle.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			ItemConsumedEvent();
			Object.Destroy(crumbParticle.gameObject, 2f);
			Object.Destroy(base.gameObject, 0.001f);
			return new BiteResultInfo(true, null, null);
		}
		Quaternion rotation = Quaternion.LookRotation(-(base.transform.position - head.transform.position).normalized);
		if (pickupableItem.IsCurrInHand)
		{
			pickupableItem.CurrInteractableHand.ReorientCurrItemInHand(base.transform.position, rotation);
		}
		return new BiteResultInfo(false, null, null);
	}

	public override void SetNumberOfBitesTaken(int num)
	{
		wholeMeshFilter.gameObject.SetActive(num == 0);
		afterFirstBiteMeshFilter.gameObject.SetActive(num == 1);
		afterSecondBiteMeshFilter.gameObject.SetActive(num == 2);
		Transform transform = colliderTransformNormalPos;
		switch (num)
		{
		case 1:
			transform = colliderTransformBiteOnePos;
			break;
		case 2:
			transform = colliderTransformBiteTwoPos;
			break;
		}
		colliderTransform.localPosition = transform.localPosition;
		colliderTransform.localRotation = transform.localRotation;
		colliderTransform.localScale = transform.localScale;
		numBitesTaken = num;
		outline.ForceRefreshMeshes();
	}
}
