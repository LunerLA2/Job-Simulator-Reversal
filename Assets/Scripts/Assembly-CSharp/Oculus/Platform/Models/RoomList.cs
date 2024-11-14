using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class RoomList : DeserializableList<Room>
	{
		public RoomList(IntPtr a)
		{
			int num = (int)(uint)CAPI.ovr_RoomArray_GetSize(a);
			_Data = new List<Room>(num);
			for (int i = 0; i < num; i++)
			{
				_Data.Add(new Room(CAPI.ovr_RoomArray_GetElement(a, (UIntPtr)(ulong)i)));
			}
			_NextUrl = CAPI.ovr_RoomArray_GetNextUrl(a);
		}
	}
}
