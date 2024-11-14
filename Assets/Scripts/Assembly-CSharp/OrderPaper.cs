using System;
using OwlchemyVR;
using UnityEngine;

public class OrderPaper : MonoBehaviour
{
	private AttachableObject attachable;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private SelectedChangeOutlineController outlineController;

	private void Awake()
	{
		attachable = GetComponent<AttachableObject>();
	}

	public void SetupOrderPaper(Mesh mesh, WorldItemData worldItemData)
	{
		meshFilter.sharedMesh = mesh;
		outlineController.ForceRefreshMeshes();
		myWorldItem.ManualSetData(worldItemData);
	}

	private void OnEnable()
	{
		AttachableObject attachableObject = attachable;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
		AttachableObject attachableObject2 = attachable;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(Detached));
	}

	private void OnDisable()
	{
		AttachableObject attachableObject = attachable;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(Attached));
		AttachableObject attachableObject2 = attachable;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(Detached));
	}

	private void Attached(AttachableObject o, AttachablePoint attachPoint)
	{
		Rigidbody attachedRigidbody = attachPoint.GetComponent<Collider>().attachedRigidbody;
		attachedRigidbody.transform.localRotation = Quaternion.identity;
	}

	private void Detached(AttachableObject o, AttachablePoint attachPoint)
	{
		Rigidbody attachedRigidbody = attachPoint.GetComponent<Collider>().attachedRigidbody;
		attachedRigidbody.transform.localRotation = Quaternion.identity;
	}
}
