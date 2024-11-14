using UnityEngine;

public class LerpEmissive : MonoBehaviour
{
	private const string emission = "_Emission";

	private Material m;

	private bool doLerp;

	private Color col;

	private float colorMult = 1f;

	private void Start()
	{
		if (GetComponent<Renderer>() != null)
		{
			m = GetComponent<Renderer>().material;
			doLerp = true;
			col = new Color(0.6f, 0.6f, 0.6f, 1f);
		}
	}

	private void Update()
	{
		if (doLerp)
		{
			m.SetColor("_Emission", col * (Mathf.PingPong(Time.time * 5f, colorMult) * 0.1f + 0.7f));
		}
	}
}
