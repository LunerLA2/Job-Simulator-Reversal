using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class PidList : DeserializableList<Pid>
	{
		public PidList(IntPtr a)
		{
			int num = (int)(uint)CAPI.ovr_PidArray_GetSize(a);
			_Data = new List<Pid>(num);
			for (int i = 0; i < num; i++)
			{
				_Data.Add(new Pid(CAPI.ovr_PidArray_GetElement(a, (UIntPtr)(ulong)i)));
			}
		}
	}
}
