using System.Collections;
using UnityEngine;

public class AndromedaModelSystem : MonoBehaviour
{
	public float floatUpSpeed = 0.01f;

	public float floatDownSpeed = 0.01f;

	public float shakeIntensity = 0.1f;

	public GameObject hologramModel;

	public float rotateSpeed;

	private float flickerSpeed;

	public float minFlicker;

	public float maxFlicker = 1f;

	public float offsetX;

	public float offsetY;

	public float xSpeed = 1f;

	public float ySpeed = 1f;

	public Light hologramLight;

	public AudioClip hologramSound;

	public bool useLight;

	public bool useSound;

	public float lowTilingY = 5f;

	public float highTilingY = 15f;

	public float tilingFactor = 0.5f;

	private float tilingY = 10f;

	private int frameCounter;

	private float valToLerpTo;

	private void Start()
	{
		if (useSound)
		{
			GetComponent<AudioSource>().enabled = true;
		}
		if (hologramModel == null)
		{
			Debug.LogError("You need to apply a model to the Hologram Model slot. The model must contain 1st the the Hologram Static Shader then the Hologram Solid Shader. Refer to the demo scene for an example if needed.");
		}
	}

	private void Update()
	{
		frameCounter++;
		offsetX += Time.deltaTime * xSpeed;
		offsetY += Time.deltaTime * ySpeed;
		float b = Random.Range(lowTilingY, highTilingY);
		tilingY = Mathf.Lerp(tilingY, b, tilingFactor * Time.deltaTime);
		flickerSpeed = 0f;
		if (useLight)
		{
			hologramLight.intensity = flickerSpeed;
		}
		if (useSound && GetComponent<AudioSource>().clip == hologramSound)
		{
			if ((double)flickerSpeed > 0.25)
			{
				GetComponent<AudioSource>().enabled = true;
				GetComponent<AudioSource>().volume = flickerSpeed + 0.3f;
				StartCoroutine(Delay());
			}
			if ((double)flickerSpeed < 0.25)
			{
				GetComponent<AudioSource>().enabled = false;
			}
		}
		Color color = hologramModel.GetComponent<Renderer>().material.color;
		hologramModel.GetComponent<Renderer>().material.color = color;
		if (maxFlicker > 2f)
		{
			Debug.LogError("Max flicker amount should not exceed 2");
		}
		if (minFlicker < -1f)
		{
			Debug.LogError("Min flicker amount should not go below -1");
		}
		Material material = hologramModel.GetComponent<Renderer>().material;
		material.mainTextureOffset = new Vector2(offsetX, offsetY);
		float x = material.GetTextureScale("_MainTex").x;
		material.SetTextureScale("_MainTex", new Vector2(x, tilingY));
		material = hologramModel.GetComponent<Renderer>().materials[1];
		Color color2 = material.GetColor("_Color");
		if (frameCounter > Random.Range(2, 5))
		{
			valToLerpTo = Random.Range(0.3f, 0.4f);
			frameCounter = 0;
		}
		if (Random.Range(0, 100) == 1)
		{
			valToLerpTo = 0.14f;
		}
		color2.a = Mathf.Lerp(color2.a, valToLerpTo, 0.18f);
		material.SetColor("_Color", color2);
	}

	private IEnumerator Delay()
	{
		yield return new WaitForSeconds(0.05f);
		GetComponent<AudioSource>().enabled = false;
	}
}
