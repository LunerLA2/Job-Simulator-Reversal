using UnityEngine;

public class HandJiveDriver : MonoBehaviour
{
	[Range(0f, 1f)]
	public float grab;

	[Range(0f, 1f)]
	public float point;

	[Range(0f, 1f)]
	public float thumbUp;

	private Animator a;

	private void Start()
	{
		a = GetComponent<Animator>();
	}

	private void Update()
	{
		a.SetFloat("Grab", grab);
		a.SetFloat("Point", point);
		a.SetLayerWeight(1, point);
		a.SetFloat("ThumbUp", thumbUp);
		a.SetLayerWeight(2, thumbUp);
	}
}
