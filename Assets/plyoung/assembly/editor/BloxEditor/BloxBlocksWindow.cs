using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	public class BloxBlocksWindow : EditorWindow
	{
		public static BloxBlocksWindow Instance;

		public static void Show_BloxBlocksWindow()
		{
            Debug.Log("Show_BloxBlocksWindow", "Blockwindows", Color.green);
            BloxEdGlobal.BlocksListDocked = false;
			EditorPrefs.SetBool("Blox.BlocksListDocked", BloxEdGlobal.BlocksListDocked);
			if ((Object)BloxBlocksWindow.Instance == (Object)null)
			{
				BloxBlocksWindow obj = BloxBlocksWindow.Instance = EditorWindow.GetWindow<BloxBlocksWindow>("Blocks");
				Texture2D image = plyEdGUI.LoadTextureResource("res.icons.blox_mono" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(BloxListWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
				obj.titleContent = new GUIContent("Blocks", image);
			}
		}

		public static void Close_BloxBlocksWindow()
		{
            Debug.Log("Close_BloxBlocksWindow", "Blockwindows", Color.green);
            BloxEdGlobal.BlocksListDocked = true;
			EditorPrefs.SetBool("Blox.BlocksListDocked", BloxEdGlobal.BlocksListDocked);
			if ((Object)BloxBlocksWindow.Instance != (Object)null)
			{
				BloxBlocksWindow.Instance.Close();
				BloxBlocksWindow.Instance = null;
			}
		}

		protected void OnEnable()
		{
            Debug.Log("OnEnable", "Blockwindows", Color.green);
            BloxBlocksWindow.Instance = this;
			Texture2D image = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.blox_mono" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(BloxListWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			base.titleContent = new GUIContent("Blocks", image);
		}

		protected void OnFocus()
		{
            Debug.Log("OnFocus", "Blockwindows", Color.green);
            BloxBlocksWindow.Instance = this;
			base.wantsMouseMove = true;
		}

		protected void OnDestroy()
		{
            Debug.Log("OnDestroy", "Blockwindows", Color.green);
            BloxBlocksWindow.Instance = null;
			BloxEdGlobal.BlocksListDocked = true;
			EditorPrefs.SetBool("Blox.BlocksListDocked", BloxEdGlobal.BlocksListDocked);
			BloxEditorWindow instance = BloxEditorWindow.Instance;
			if ((object)instance != null)
			{
				instance.Repaint();
			}
		}

		protected void OnGUI()
		{
			BloxBlocksList.Instance.DoGUI(this, base.position.width);
		}
	}
}
