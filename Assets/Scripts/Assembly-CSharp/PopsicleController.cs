using OwlchemyVR;
using UnityEngine;

public class PopsicleController : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer[] meshRenderers;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private ParticleSystem munchPFX;

	public void Setup(Material mat, WorldItemData data)
	{
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			meshRenderers[i].material = mat;
		}
		myWorldItem.ManualSetData(data);
		munchPFX.startColor = myWorldItem.Data.OverallColor;
	}
}
