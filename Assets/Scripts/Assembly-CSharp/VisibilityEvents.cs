using System;
using UnityEngine;

public class VisibilityEvents : MonoBehaviour
{
	private bool isVisible;

	public bool IsVisible
	{
		get
		{
			return isVisible;
		}
	}

	public event Action<VisibilityEvents> OnObjectBecameVisible;

	public event Action<VisibilityEvents> OnObjectBecameInvisible;

	private void OnBecameVisible()
	{
		isVisible = true;
		if (this.OnObjectBecameVisible != null)
		{
			this.OnObjectBecameVisible(this);
		}
	}

	private void OnBecameInvisible()
	{
		isVisible = false;
		if (this.OnObjectBecameInvisible != null)
		{
			this.OnObjectBecameInvisible(this);
		}
	}
}
