using UnityEngine;

public class LottoWinnerIniter : MonoBehaviour
{
	[SerializeField]
	private Mesh[] meshes;

	[SerializeField]
	private MeshFilter meshFilter;

	private void OnEnable()
	{
		meshFilter.mesh = meshes[Random.Range(0, meshes.Length)];
	}
}
