using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class CloudStorageMetadataList : DeserializableList<CloudStorageMetadata>
	{
		public CloudStorageMetadataList(IntPtr a)
		{
			int num = (int)(uint)CAPI.ovr_CloudStorageMetadataArray_GetSize(a);
			_Data = new List<CloudStorageMetadata>(num);
			for (int i = 0; i < num; i++)
			{
				_Data.Add(new CloudStorageMetadata(CAPI.ovr_CloudStorageMetadataArray_GetElement(a, (UIntPtr)(ulong)i)));
			}
			_NextUrl = CAPI.ovr_CloudStorageMetadataArray_GetNextUrl(a);
		}
	}
}
