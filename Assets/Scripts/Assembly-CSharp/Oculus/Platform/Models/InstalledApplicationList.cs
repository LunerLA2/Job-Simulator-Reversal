using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class InstalledApplicationList : DeserializableList<InstalledApplication>
	{
		public InstalledApplicationList(IntPtr a)
		{
			int num = (int)(uint)CAPI.ovr_InstalledApplicationArray_GetSize(a);
			_Data = new List<InstalledApplication>(num);
			for (int i = 0; i < num; i++)
			{
				_Data.Add(new InstalledApplication(CAPI.ovr_InstalledApplicationArray_GetElement(a, (UIntPtr)(ulong)i)));
			}
		}
	}
}
