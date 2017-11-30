using BloxEngine;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[SuppressBaseMembersInBlox]
	[BloxBlockIcon("bgs")]
	[HelpURL("https://plyoung.github.io/blox-image-manager.html")]
	public static class ImageManager
	{
		/// <summary> Returns the Sprite with given ident (name) </summary>
		public static Sprite GetSprite(string ident)
		{
			ManagedImage item = ManagedImagesAsset.Instance.GetItem(ident);
			if (item == null)
			{
				return null;
			}
			return item.sprite;
		}

		/// <summary> Returns the Sprite with given id </summary>
		public static Sprite GetSprite(int id)
		{
			ManagedImage item = ManagedImagesAsset.Instance.GetItem(id);
			if (item == null)
			{
				return null;
			}
			return item.sprite;
		}

		/// <summary> Returns the ID of the image with given ident (name). Returns -1 if not found. </summary>
		public static int GetId(string ident)
		{
			ManagedImage item = ManagedImagesAsset.Instance.GetItem(ident);
			if (item != null)
			{
				return item.id;
			}
			return -1;
		}

		/// <summary> Returns the ident (name) of the image with given id. Returns null if not found. </summary>
		public static string GetIdent(int id)
		{
			ManagedImage item = ManagedImagesAsset.Instance.GetItem(id);
			if (item == null)
			{
				return null;
			}
			return item.ident;
		}
	}
}
