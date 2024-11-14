using UnityEngine;

public class TrashCubeController : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer[] recolorMeshes;

	public void RecolorCube(Color[] colors)
	{
		int num = 0;
		for (int i = 0; i < recolorMeshes.Length; i++)
		{
			if (colors.Length - 1 >= num)
			{
				recolorMeshes[i].material.SetColor("_DiffColor", colors[num]);
				num++;
			}
			else
			{
				num = 0;
				i--;
			}
		}
	}
}
