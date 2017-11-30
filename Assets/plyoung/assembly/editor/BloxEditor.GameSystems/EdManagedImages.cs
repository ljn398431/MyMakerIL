using BloxGameSystems;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	public class EdManagedImages : GroupedDataEd<ManagedImagesGroup, ManagedImage>
	{
		private static readonly GUIContent GC_SpritesDrop = new GUIContent("Drop Sprites here");

		protected override GroupedData<ManagedImagesGroup, ManagedImage> groupedData
		{
			get
			{
				return BloxEdGlobal.ManagedImages;
			}
		}

		public EdManagedImages() : base("Managed Images", "blox-image-manager")
		{
		}

		protected override void OnFocus()
		{
		}

		protected override void DrawBeforeList()
		{
			if (base.CurrentGroup() != null)
			{
				Event current = Event.current;
				Rect rect = GUILayoutUtility.GetRect(150f, 32f, plyEdGUI.Styles.DropAreaSmallerText, GUILayout.ExpandWidth(false));
				GUI.Box(rect, EdManagedImages.GC_SpritesDrop, plyEdGUI.Styles.DropAreaSmallerText);
				if (current.type != EventType.DragUpdated && current.type != EventType.DragPerform)
					return;
				if (rect.Contains(current.mousePosition))
				{
					if (current.type == EventType.DragUpdated)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
						current.Use();
					}
					if (current.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						current.Use();
						for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
						{
							Sprite sprite = DragAndDrop.objectReferences[i] as Sprite;
							if ((Object)sprite == (Object)null)
							{
								Texture2D texture2D = DragAndDrop.objectReferences[i] as Texture2D;
								if (!((Object)texture2D == (Object)null))
								{
									Object[] array = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture2D));
									for (int j = 0; j < array.Length; j++)
									{
										sprite = (array[j] as Sprite);
										if (!((Object)sprite == (Object)null))
										{
											string uniqueItemName = BloxEdGlobal.ManagedImages.GetUniqueItemName(sprite.name);
											BloxEdGlobal.ManagedImages.CreateItem(uniqueItemName, base.CurrentGroup()).sprite = sprite;
										}
									}
								}
							}
							else
							{
								string uniqueItemName2 = BloxEdGlobal.ManagedImages.GetUniqueItemName(sprite.name);
								BloxEdGlobal.ManagedImages.CreateItem(uniqueItemName2, base.CurrentGroup()).sprite = sprite;
							}
						}
						base.Save();
					}
				}
			}
		}

		protected override void DrawSelected(ManagedImage def)
		{
			def.sprite = (Sprite)EditorGUILayout.ObjectField(def.sprite, typeof(Sprite), false, GUILayout.Width(128f), GUILayout.Height(128f));
			EditorGUILayout.Space();
		}
	}
}
