using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRRenderModels
	{
		private IVRRenderModels FnTable;

		internal CVRRenderModels(IntPtr pInterface)
		{
			FnTable = (IVRRenderModels)Marshal.PtrToStructure(pInterface, typeof(IVRRenderModels));
		}

		public EVRRenderModelError LoadRenderModel_Async(string pchRenderModelName, ref IntPtr ppRenderModel)
		{
			return FnTable.LoadRenderModel_Async(pchRenderModelName, ref ppRenderModel);
		}

		public void FreeRenderModel(IntPtr pRenderModel)
		{
			FnTable.FreeRenderModel(pRenderModel);
		}

		public EVRRenderModelError LoadTexture_Async(int textureId, ref IntPtr ppTexture)
		{
			return FnTable.LoadTexture_Async(textureId, ref ppTexture);
		}

		public void FreeTexture(IntPtr pTexture)
		{
			FnTable.FreeTexture(pTexture);
		}

		public EVRRenderModelError LoadTextureD3D11_Async(int textureId, IntPtr pD3D11Device, ref IntPtr ppD3D11Texture2D)
		{
			return FnTable.LoadTextureD3D11_Async(textureId, pD3D11Device, ref ppD3D11Texture2D);
		}

		public EVRRenderModelError LoadIntoTextureD3D11_Async(int textureId, IntPtr pDstTexture)
		{
			return FnTable.LoadIntoTextureD3D11_Async(textureId, pDstTexture);
		}

		public void FreeTextureD3D11(IntPtr pD3D11Texture2D)
		{
			FnTable.FreeTextureD3D11(pD3D11Texture2D);
		}

		public uint GetRenderModelName(uint unRenderModelIndex, StringBuilder pchRenderModelName, uint unRenderModelNameLen)
		{
			return FnTable.GetRenderModelName(unRenderModelIndex, pchRenderModelName, unRenderModelNameLen);
		}

		public uint GetRenderModelCount()
		{
			return FnTable.GetRenderModelCount();
		}

		public uint GetComponentCount(string pchRenderModelName)
		{
			return FnTable.GetComponentCount(pchRenderModelName);
		}

		public uint GetComponentName(string pchRenderModelName, uint unComponentIndex, StringBuilder pchComponentName, uint unComponentNameLen)
		{
			return FnTable.GetComponentName(pchRenderModelName, unComponentIndex, pchComponentName, unComponentNameLen);
		}

		public ulong GetComponentButtonMask(string pchRenderModelName, string pchComponentName)
		{
			return FnTable.GetComponentButtonMask(pchRenderModelName, pchComponentName);
		}

		public uint GetComponentRenderModelName(string pchRenderModelName, string pchComponentName, StringBuilder pchComponentRenderModelName, uint unComponentRenderModelNameLen)
		{
			return FnTable.GetComponentRenderModelName(pchRenderModelName, pchComponentName, pchComponentRenderModelName, unComponentRenderModelNameLen);
		}

		public bool GetComponentState(string pchRenderModelName, string pchComponentName, ref VRControllerState_t pControllerState, ref RenderModel_ControllerMode_State_t pState, ref RenderModel_ComponentState_t pComponentState)
		{
			return FnTable.GetComponentState(pchRenderModelName, pchComponentName, ref pControllerState, ref pState, ref pComponentState);
		}

		public bool RenderModelHasComponent(string pchRenderModelName, string pchComponentName)
		{
			return FnTable.RenderModelHasComponent(pchRenderModelName, pchComponentName);
		}
	}
}
