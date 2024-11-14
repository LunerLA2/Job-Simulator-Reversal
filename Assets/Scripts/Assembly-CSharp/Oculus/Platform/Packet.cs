using System;
using System.Runtime.InteropServices;

namespace Oculus.Platform
{
	public sealed class Packet : IDisposable
	{
		private readonly ulong size;

		private readonly IntPtr packetHandle;

		public ulong SenderID
		{
			get
			{
				return CAPI.ovr_Packet_GetSenderID(packetHandle);
			}
		}

		public ulong Size
		{
			get
			{
				return size;
			}
		}

		public SendPolicy Policy
		{
			get
			{
				return CAPI.ovr_Packet_GetSendPolicy(packetHandle);
			}
		}

		public Packet(IntPtr packetHandle)
		{
			this.packetHandle = packetHandle;
			size = (ulong)CAPI.ovr_Packet_GetSize(packetHandle);
		}

		public ulong ReadBytes(byte[] destination)
		{
			if ((ulong)destination.LongLength < size)
			{
				throw new ArgumentException(string.Format("Destination array was not big enough to hold {0} bytes", size));
			}
			Marshal.Copy(CAPI.ovr_Packet_GetBytes(packetHandle), destination, 0, (int)size);
			return size;
		}

		~Packet()
		{
			Dispose();
		}

		public void Dispose()
		{
			CAPI.ovr_Packet_Free(packetHandle);
			GC.SuppressFinalize(this);
		}
	}
}
