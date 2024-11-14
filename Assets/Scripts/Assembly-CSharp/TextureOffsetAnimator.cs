using UnityEngine;

public class TextureOffsetAnimator : MonoBehaviour
{
	public Renderer rendererToAnimate;

	public float offsetX;

	public float offsetY;

	private void Update()
	{
		rendererToAnimate.material.mainTextureOffset = Vector2.right * offsetX + Vector2.up * offsetY;
	}
}
