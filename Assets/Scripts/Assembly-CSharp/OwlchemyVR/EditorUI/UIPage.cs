using UnityEngine;

namespace OwlchemyVR.EditorUI
{
	public abstract class UIPage : MonoBehaviour
	{
		public bool IsPageOpen { get; protected set; }

		public abstract void Open();

		public abstract void Close();
	}
}
