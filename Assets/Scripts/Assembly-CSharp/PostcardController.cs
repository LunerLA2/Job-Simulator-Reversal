using TMPro;
using UnityEngine;

public class PostcardController : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer paper;

	[SerializeField]
	private TextMeshPro text;

	public void SetupSkin(Material material)
	{
		text.gameObject.SetActive(false);
		paper.sharedMaterial = material;
	}

	public void SetupCustomText(string richText)
	{
		text.text = richText;
	}
}
