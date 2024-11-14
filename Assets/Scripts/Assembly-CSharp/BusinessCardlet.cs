using OwlchemyVR;
using TMPro;
using UnityEngine;

public class BusinessCardlet : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro headingText;

	[SerializeField]
	private TextMeshPro halfBodyText;

	[SerializeField]
	private TextMeshPro fullBodyText;

	[SerializeField]
	private MeshFilter[] meshesToHighlight;

	[SerializeField]
	private MeshRenderer cardRenderer;

	[SerializeField]
	private Material halfBodyMaterial;

	[SerializeField]
	private Material[] fullBodyMaterials;

	private Rigidbody rb;

	private ConfigurableJoint joint;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		joint = GetComponent<ConfigurableJoint>();
	}

	public void Initialize(BusinessCardletInfo cardletInfo, int index)
	{
		if (string.IsNullOrEmpty(cardletInfo.Heading))
		{
			fullBodyText.gameObject.SetActive(true);
			headingText.gameObject.SetActive(false);
			halfBodyText.gameObject.SetActive(false);
			fullBodyText.text = cardletInfo.Body;
			cardRenderer.sharedMaterial = fullBodyMaterials[index % fullBodyMaterials.Length];
		}
		else
		{
			headingText.gameObject.SetActive(true);
			halfBodyText.gameObject.SetActive(true);
			fullBodyText.gameObject.SetActive(false);
			headingText.text = cardletInfo.Heading;
			halfBodyText.text = cardletInfo.Body;
			cardRenderer.sharedMaterial = halfBodyMaterial;
		}
	}

	public void SetAsRoot(GrabbableItem cardGrabbable, SelectedChangeOutlineController cardOutlineController)
	{
		Object.Destroy(joint);
		Object.Destroy(rb);
		rb = cardGrabbable.Rigidbody;
		cardOutlineController.meshFilters = meshesToHighlight;
		cardOutlineController.Build();
		GetComponent<ColliderGrabbableItemPointer>().grabbableItem = cardGrabbable;
	}

	public void LinkTo(BusinessCardlet other, GrabbableItem cardGrabbable)
	{
		base.transform.SetParent(other.transform, false);
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		joint.connectedBody = other.rb;
		joint.connectedAnchor = -joint.anchor;
		GetComponent<ColliderGrabbableItemPointer>().grabbableItem = cardGrabbable;
	}
}
