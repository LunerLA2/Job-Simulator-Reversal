using UnityEngine;

public class BotSimpleFaceBakedAnimator : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer rendererToAnimate;

	private Vector2[] frameOffsets = new Vector2[6]
	{
		new Vector2(0f, -0.25f),
		new Vector2(0.5f, -0.25f),
		new Vector2(0f, -0.5f),
		new Vector2(0.5f, -0.5f),
		new Vector2(0f, -0.75f),
		new Vector2(0.5f, -0.75f)
	};

	private int currentFrame;

	private float timePerFrame = 0.04666f;

	private float t;

	private void Awake()
	{
		t = Random.Range(0f, timePerFrame * (float)frameOffsets.Length);
	}

	private void Update()
	{
		t += Time.deltaTime;
		while (t >= timePerFrame)
		{
			t -= timePerFrame;
			currentFrame++;
			while (currentFrame >= frameOffsets.Length)
			{
				currentFrame -= frameOffsets.Length;
			}
		}
		rendererToAnimate.material.mainTextureOffset = frameOffsets[currentFrame];
	}
}
