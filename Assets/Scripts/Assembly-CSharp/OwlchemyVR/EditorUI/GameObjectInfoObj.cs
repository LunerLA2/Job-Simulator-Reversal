using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR.EditorUI
{
	public class GameObjectInfoObj
	{
		private GameObject go;

		private List<ComponentInfoObject> componentInfoObjs = new List<ComponentInfoObject>();

		public GameObject GameObject
		{
			get
			{
				return go;
			}
		}

		public List<ComponentInfoObject> ComponentInfoObjs
		{
			get
			{
				return componentInfoObjs;
			}
		}

		public GameObjectInfoObj(GameObject go, List<ComponentInfoObject> componentInfoObjs)
		{
			this.go = go;
			this.componentInfoObjs = componentInfoObjs;
		}
	}
}
