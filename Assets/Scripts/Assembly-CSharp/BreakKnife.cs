using OwlchemyVR;
using UnityEngine;

public class BreakKnife : MonoBehaviour
{
	public GameObject bladeToShatter;

	public int shatterCount = 2;

	public Collider bladeCollider;

	public AudioClip breakKnifeClip;

	public Slicer slicer;

	public Collider knifeThrow;

	public void Break()
	{
		slicer.enabled = false;
		bladeToShatter.transform.parent = GlobalStorage.Instance.ContentRoot;
		bladeToShatter.gameObject.AddComponent<BoxCollider>();
		bladeToShatter.gameObject.AddComponent<Rigidbody>();
		ModelOutlineContainer componentInChildren = bladeToShatter.gameObject.GetComponentInChildren<ModelOutlineContainer>();
		if (componentInChildren != null)
		{
			componentInChildren.gameObject.SetActive(false);
		}
		TurboSlice.instance.shatter(bladeToShatter, shatterCount);
		bladeCollider.isTrigger = true;
		knifeThrow.isTrigger = true;
		AudioManager.Instance.Play(base.transform.position, breakKnifeClip, 1f, 1f);
	}
}
