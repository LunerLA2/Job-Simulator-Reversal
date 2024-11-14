using System.Collections;
using UnityEngine;

public class AnimatedBolt : MonoBehaviour
{
	[SerializeField]
	private Transform twistTransform;

	[SerializeField]
	private float scaleSpeed = 0.1f;

	[SerializeField]
	private float twistSpeed = 1f;

	private Vector3 unscrewedPos;

	private void Awake()
	{
		unscrewedPos = new Vector3(twistTransform.localPosition.x, twistTransform.localPosition.y, twistTransform.localPosition.z - 0.015f);
		DespawnBolt();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SpawnBolt()
	{
		StopAllCoroutines();
		StartCoroutine(SpawnBoltInternal());
	}

	private IEnumerator SpawnBoltInternal()
	{
		Quaternion initialLocalRotation = twistTransform.localRotation;
		GoTweenConfig twistConfig = new GoTweenConfig().localPosition(Vector3.zero).setEaseType(GoEaseType.CircInOut);
		twistTransform.localPosition = unscrewedPos;
		twistConfig.localRotation(new Vector3(initialLocalRotation.eulerAngles.x, initialLocalRotation.eulerAngles.y, initialLocalRotation.eulerAngles.z + 180f));
		yield return new WaitForSeconds(scaleSpeed);
		Go.to(twistTransform, twistSpeed, twistConfig);
	}

	public void DespawnBolt()
	{
		StopAllCoroutines();
		StartCoroutine(DespawnBoltInternal());
	}

	private IEnumerator DespawnBoltInternal()
	{
		Quaternion initialLocalRotation = twistTransform.localRotation;
		GoTweenConfig twistConfig = new GoTweenConfig().localPosition(unscrewedPos).setEaseType(GoEaseType.CircInOut);
		twistConfig.localRotation(new Vector3(initialLocalRotation.eulerAngles.x, initialLocalRotation.eulerAngles.y, initialLocalRotation.eulerAngles.z + 180f));
		Go.to(twistTransform, twistSpeed, twistConfig);
		yield return new WaitForSeconds(twistSpeed);
		yield return new WaitForSeconds(scaleSpeed);
	}
}
