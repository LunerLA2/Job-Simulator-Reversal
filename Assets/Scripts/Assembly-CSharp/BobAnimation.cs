using System;
using UnityEngine;

public class BobAnimation : MonoBehaviour
{
	[SerializeField]
	private bool move = true;

	[SerializeField]
	private Vector3 moveVector = Vector3.up;

	[SerializeField]
	private float moveRange = 2f;

	[SerializeField]
	private float moveSpeed = 0.5f;

	private Vector3 startPosition;

	private float currentRadians;

	private void Start()
	{
		startPosition = base.transform.localPosition;
	}

	private void Update()
	{
		currentRadians += Time.deltaTime * moveSpeed;
		currentRadians %= (float)Math.PI * 2f;
		if (move)
		{
			base.transform.localPosition = startPosition + moveVector * (moveRange * Mathf.Sin(currentRadians));
		}
	}
}
