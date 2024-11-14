using UnityEngine;

namespace OwlchemyVR
{
	public class SurfaceTypeController : MonoBehaviour
	{
		[SerializeField]
		private SurfaceTypeData surfaceTypeData;

		public SurfaceTypeData Data
		{
			get
			{
				return surfaceTypeData;
			}
		}

		public string SurfaceTypeName
		{
			get
			{
				return (!(surfaceTypeData != null)) ? string.Empty : surfaceTypeData.SurfaceTypeName;
			}
		}
	}
}
