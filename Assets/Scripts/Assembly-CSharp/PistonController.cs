using OwlchemyVR;
using UnityEngine;

public class PistonController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private MeshFilter visualMeshFilter;

	[SerializeField]
	private SelectedChangeOutlineController outline;

	public void Init(Mesh _mesh, WorldItemData _wid)
	{
		visualMeshFilter.sharedMesh = _mesh;
		outline.ForceRefreshMeshes();
		if (_wid != null)
		{
			myWorldItem.ManualSetData(_wid);
		}
	}
}
