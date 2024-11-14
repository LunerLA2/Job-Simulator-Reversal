using System.Collections;
using UnityEngine;

public abstract class ModeContainer : MonoBehaviour
{
	[SerializeField]
	private int initialModeIndex = -1;

	protected int modeIndex = -1;

	protected bool modeChangeFailed;

	private bool isChangingMode;

	private bool modeChangeRequested;

	private int requestedModeIndex;

	private bool hasStartRun;

	public int ModeIndex
	{
		get
		{
			return modeIndex;
		}
	}

	public bool IsChangingMode
	{
		get
		{
			return isChangingMode;
		}
	}

	protected virtual void Start()
	{
		if (initialModeIndex != -1)
		{
			RequestModeChange(initialModeIndex);
			hasStartRun = true;
		}
	}

	private void OnEnable()
	{
		if (hasStartRun && initialModeIndex != -1)
		{
			isChangingMode = false;
			RequestModeChange(initialModeIndex);
		}
	}

	public void RequestModeChange(int newModeIndex)
	{
		modeChangeRequested = true;
		requestedModeIndex = newModeIndex;
	}

	protected virtual void Update()
	{
		if (modeChangeRequested && !isChangingMode)
		{
			modeChangeRequested = false;
			StartCoroutine(InternalChangeModeAsync(requestedModeIndex));
		}
	}

	private IEnumerator InternalChangeModeAsync(int newModeIndex)
	{
		if (newModeIndex != modeIndex)
		{
			isChangingMode = true;
			modeChangeFailed = false;
			yield return StartCoroutine(ChangeModeAsync(newModeIndex));
			if (!modeChangeFailed)
			{
				modeIndex = newModeIndex;
			}
			isChangingMode = false;
		}
	}

	protected virtual IEnumerator ChangeModeAsync(int newModeIndex)
	{
		yield return new WaitForSeconds(0.5f);
	}
}
