using BloxEngine;
using System;
using UnityEngine;

namespace BloxGameSystems
{
	[Serializable]
	[ExcludeFromBlox]
	public class ManagedImagesAsset : GroupedData<ManagedImagesGroup, ManagedImage>
	{
		private static ManagedImagesAsset _instance;

		public static ManagedImagesAsset Instance
		{
			get
			{
				return ManagedImagesAsset._instance ?? (ManagedImagesAsset._instance = Resources.Load<ManagedImagesAsset>("Blox/ManagedImages"));
			}
		}
	}
}
