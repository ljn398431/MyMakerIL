using BloxEditor.GameSystems;
using BloxEngine;
using BloxGameSystems;
using plyLibEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BloxEditor
{
	[InitializeOnLoad]
	public class BloxEdGlobal
	{
		public static readonly string DataRoot;

		public static readonly string ResourcesRoot;

		public static readonly string DocsPath;

		public static readonly string DefsPath;

		public static readonly string ProvidersPath;

		public static readonly string ScriptPath;

		public static readonly string MiscPath;

		public static readonly string ScenesPath;

		public static readonly string BootstrapScenePath;

		public static readonly string BootstrapFabPath;

		public static readonly string BloxGlobalFabPath;

		public static readonly string URL_DOCS;

		public static readonly string URL_SUPPORT;

		public static readonly string URL_STORE;

		public static bool DoubleClickOpenBloxDef;

		public static Color CanvasColour;

		public static plyEdTreeViewDrawMode BlocksListMode;

		public static bool DelayedSearch;

		public static bool BlocksListDocked;

		public static bool ShowBloxIconInHierarchyPanel;

		public static bool ShowBloxIconInProjectPanel;

		public static bool GUIDScriptNames;

		public static bool SaveBrokenScripts;

		public static int BlockTheme;

		private static int playTestingMode;

		private static BGSSettings _gameSystemsSettings;

		private static ManagedImagesAsset _managedImages;

		private static CharacterAttributeDefsAsset _attributeDefs;

		private static SkillDefsAsset _skillDefs;

		public static BGSSettings GameSystemsSettings
		{
			get
			{
				if ((UnityEngine.Object)BloxEdGlobal._gameSystemsSettings == (UnityEngine.Object)null)
				{
					plyEdUtil.CheckPath(BloxEdGlobal.ResourcesRoot);
					BloxEdGlobal._gameSystemsSettings = plyEdUtil.LoadOrCreateAsset<BGSSettings>(BloxEdGlobal.ResourcesRoot + "BGSSettings.asset", true);
				}
				return BloxEdGlobal._gameSystemsSettings;
			}
		}

		public static ManagedImagesAsset ManagedImages
		{
			get
			{
				if ((UnityEngine.Object)BloxEdGlobal._managedImages == (UnityEngine.Object)null)
				{
					plyEdUtil.CheckPath(BloxEdGlobal.ResourcesRoot);
					BloxEdGlobal._managedImages = plyEdUtil.LoadOrCreateAsset<ManagedImagesAsset>(BloxEdGlobal.ResourcesRoot + "ManagedImages.asset", true);
				}
				return BloxEdGlobal._managedImages;
			}
		}

		public static CharacterAttributeDefsAsset AttributeDefs
		{
			get
			{
				if ((UnityEngine.Object)BloxEdGlobal._attributeDefs == (UnityEngine.Object)null)
				{
					plyEdUtil.CheckPath(BloxEdGlobal.ResourcesRoot);
					BloxEdGlobal._attributeDefs = plyEdUtil.LoadOrCreateAsset<CharacterAttributeDefsAsset>(BloxEdGlobal.ResourcesRoot + "Attributes.asset", true);
				}
				return BloxEdGlobal._attributeDefs;
			}
		}

		public static SkillDefsAsset SkillDefs
		{
			get
			{
				if ((UnityEngine.Object)BloxEdGlobal._skillDefs == (UnityEngine.Object)null)
				{
					plyEdUtil.CheckPath(BloxEdGlobal.ResourcesRoot);
					BloxEdGlobal._skillDefs = plyEdUtil.LoadOrCreateAsset<SkillDefsAsset>(BloxEdGlobal.ResourcesRoot + "Skills.asset", true);
				}
				return BloxEdGlobal._skillDefs;
			}
		}

		static BloxEdGlobal()
		{
			BloxEdGlobal.DataRoot = plyEdUtil.DataRoot + "Blox/";
			BloxEdGlobal.ResourcesRoot = plyEdUtil.DataRoot + "Resources/Blox/";
			BloxEdGlobal.DocsPath = BloxEdGlobal.DataRoot + "docs/";
			BloxEdGlobal.DefsPath = BloxEdGlobal.DataRoot + "defs/";
			BloxEdGlobal.ProvidersPath = BloxEdGlobal.DataRoot + "dataproviders/";
			BloxEdGlobal.ScriptPath = BloxEdGlobal.DataRoot + "scripts/";
			BloxEdGlobal.MiscPath = BloxEdGlobal.DataRoot + "data/";
			BloxEdGlobal.ScenesPath = BloxEdGlobal.DataRoot + "scenes/";
			BloxEdGlobal.BootstrapScenePath = BloxEdGlobal.ScenesPath + "00-bootstrap.unity";
			BloxEdGlobal.BootstrapFabPath = BloxEdGlobal.ScenesPath + "Bootstrap.prefab";
			BloxEdGlobal.BloxGlobalFabPath = BloxEdGlobal.DataRoot + "BloxGlobal.prefab";
			BloxEdGlobal.URL_DOCS = "https://plyoung.github.io/blox.html";
			BloxEdGlobal.URL_SUPPORT = "http://forum.plyoung.com/c/blox-3/";
			BloxEdGlobal.URL_STORE = "https://www.assetstore.unity3d.com/#!/content/62473?aid=1101lGtB";
			BloxEdGlobal.DoubleClickOpenBloxDef = true;
			BloxEdGlobal.CanvasColour = new Color(0.15f, 0.27f, 0.31f, 1f);
			BloxEdGlobal.BlocksListMode = plyEdTreeViewDrawMode.List;
			BloxEdGlobal.DelayedSearch = false;
			BloxEdGlobal.BlocksListDocked = true;
			BloxEdGlobal.ShowBloxIconInHierarchyPanel = true;
			BloxEdGlobal.ShowBloxIconInProjectPanel = false;
			BloxEdGlobal.GUIDScriptNames = true;
			BloxEdGlobal.SaveBrokenScripts = false;
			BloxEdGlobal.BlockTheme = 0;
			BloxEdGlobal.playTestingMode = 0;
			BloxEdGlobal._gameSystemsSettings = null;
			BloxEdGlobal._managedImages = null;
			BloxEdGlobal._attributeDefs = null;
			BloxEdGlobal._skillDefs = null;
			plyEdGizmoIconManager.RegisterIconPack(plyEdUtil.PackagesFullPath + "Blox/packages/Blox-Icons.zip", "Assets/Gizmos/BloxEngine/BloxContainer icon.png");
			plyEdToolbar.AddButtons(new List<plyEdToolbar.ToolbarButton>
			{
				new plyEdToolbar.ToolbarButton
				{
					label = new GUIContent(Ico._play, "Start Game"),
					order = 0,
					callback = BloxEdGlobal.Menu_StartGame
				},
				new plyEdToolbar.ToolbarButton
				{
					label = new GUIContent(Ico._unirpg, "Open Blox Game Systems Window"),
					order = 1,
					callback = BloxEdGlobal.Menu_ShowBGSWindow
				}
			});
			EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, new EditorApplication.HierarchyWindowItemCallback(BloxEdGlobal.UpdateHierarchyItemIcon));
			EditorApplication.projectWindowItemOnGUI = (EditorApplication.ProjectWindowItemCallback)Delegate.Combine(EditorApplication.projectWindowItemOnGUI, new EditorApplication.ProjectWindowItemCallback(BloxEdGlobal.UpdateProjectItemIcon));
			EditorApplication.delayCall = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.delayCall, new EditorApplication.CallbackFunction(BloxEdGlobal.DelayCall));
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(BloxEdGlobal.OnUpdate));
		}

		private static void UpdateHierarchyItemIcon(int instanceID, Rect r)
		{
			if (BloxEdGlobal.ShowBloxIconInHierarchyPanel && Event.current.type == EventType.Repaint)
			{
				GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)gameObject.GetComponent<BloxContainer>() != (UnityEngine.Object)null)
				{
					r.x = (float)(r.xMax - 20.0);
					r.width = 16f;
					r.height = 16f;
					GUI.DrawTexture(r, BloxEdGUI.Instance.bloxIcon);
				}
			}
		}

		private static void UpdateProjectItemIcon(string guid, Rect r)
		{
			if (BloxEdGlobal.ShowBloxIconInProjectPanel && Event.current.type == EventType.Repaint)
			{
				GameObject gameObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject)) as GameObject;
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)gameObject.GetComponent<BloxContainer>() != (UnityEngine.Object)null)
				{
					r.x = (float)(r.xMax - 20.0);
					r.width = 16f;
					r.height = 16f;
					GUI.DrawTexture(r, BloxEdGUI.Instance.bloxIcon);
				}
			}
		}

		private static void DelayCall()
		{
			BloxEdGlobal.CanvasColour = plyEdUtil.EdPrefs_GetColor("Blox.CanvasColour", BloxEdGlobal.CanvasColour);
			BloxEdGlobal.BlocksListMode = (plyEdTreeViewDrawMode)EditorPrefs.GetInt("Blox.BlocksListMode", (int)BloxEdGlobal.BlocksListMode);
			BloxEdGlobal.DelayedSearch = EditorPrefs.GetBool("Blox.DelayedSearch", BloxEdGlobal.DelayedSearch);
			BloxEdGlobal.BlocksListDocked = EditorPrefs.GetBool("Blox.BlocksListDocked", BloxEdGlobal.BlocksListDocked);
			BloxEdGlobal.ShowBloxIconInHierarchyPanel = EditorPrefs.GetBool("Blox.ShowBloxIconInHierarchyPanel", BloxEdGlobal.ShowBloxIconInHierarchyPanel);
			BloxEdGlobal.ShowBloxIconInProjectPanel = EditorPrefs.GetBool("Blox.ShowBloxIconInProjectPanel", BloxEdGlobal.ShowBloxIconInProjectPanel);
			BloxEdGlobal.GUIDScriptNames = EditorPrefs.GetBool("Blox.GUIDScriptNames", BloxEdGlobal.GUIDScriptNames);
			BloxEdGlobal.SaveBrokenScripts = EditorPrefs.GetBool("Blox.SaveBrokenScripts", BloxEdGlobal.SaveBrokenScripts);
			BloxEdGlobal.BlockTheme = EditorPrefs.GetInt("Blox.BlockTheme", BloxEdGlobal.BlockTheme);
		}

		private static void OnUpdate()
		{
			if (BloxEdGlobal.playTestingMode != 1)
			{
				if (BloxEdGlobal.playTestingMode == 0)
				{
					BloxEdGlobal.playTestingMode = EditorPrefs.GetInt("Blox.playTestingMode", 1);
					if (!EditorApplication.isPlayingOrWillChangePlaymode && BloxEdGlobal.playTestingMode != 1)
					{
						BloxEdGlobal.playTestingMode = 1;
						EditorPrefs.SetInt("Blox.playTestingMode", 1);
					}
				}
				else if (BloxEdGlobal.playTestingMode == 2 && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
				{
					BloxEdGlobal.playTestingMode = 1;
					EditorPrefs.SetInt("Blox.playTestingMode", 1);
					EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
					try
					{
						ScenesSetupInfo scenesSetupInfo = JsonUtility.FromJson<ScenesSetupInfo>(File.ReadAllText(plyEdUtil.ProjectTempFolder + "BloxSceneSetup"));
						if (((scenesSetupInfo != null) ? scenesSetupInfo.sceneSetup.Length : 0) != 0)
						{
							EditorSceneManager.RestoreSceneManagerSetup(scenesSetupInfo.sceneSetup);
						}
						else
						{
							EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
						}
					}
					catch
					{
					}
					try
					{
						File.Delete(plyEdUtil.ProjectTempFolder + "BloxSceneSetup");
					}
					catch
					{
					}
				}
			}
		}

		[MenuItem("GameObject/Blox Game Systems/Area Trigger", priority = 1)]
		[MenuItem("Blox/Game Systems/Area Trigger", priority = 10)]
		public static void Create_AreaTrigger()
		{
			plyEdUtil.SetDirty(new GameObject("AreaTrigger", typeof(AreaTrigger))
			{
				transform = 
				{
					position = plyEdUtil.GetCreatePositionInSceneView(new Vector3(0f, 0.1f, 0f), true, -1)
				}
			}.transform);
		}

		[MenuItem("Blox/Start Game", priority = 1000)]
		public static void Menu_StartGame()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				if (!BloxEdGlobal.DataSpotCheck())
				{
					EditorUtility.DisplayDialog("Blox Game Systems", "The project does not seem to be playable yet. You need to run a data check via menu: Blox > Check Data", "OK");
				}
				else
				{
					try
					{
						string contents = JsonUtility.ToJson(new ScenesSetupInfo
						{
							sceneSetup = EditorSceneManager.GetSceneManagerSetup()
						});
						File.WriteAllText(plyEdUtil.ProjectTempFolder + "BloxSceneSetup", contents);
					}
					catch
					{
					}
					EditorSceneManager.OpenScene(BloxEdGlobal.BootstrapScenePath, OpenSceneMode.Single);
					EditorPrefs.SetInt("Blox.playTestingMode", 2);
					EditorApplication.isPlaying = true;
				}
			}
		}

		[MenuItem("Blox/Check Game Data", priority = 1002)]
		public static void Menu_CheckData()
		{
			BloxEdGlobal.CheckAllData();
		}

		[MenuItem("Blox/Global Variables", priority = 1001)]
		public static void Select_BloxGlobal()
		{
			MainEditorWindow.Show_MainEditorWindow("blox-main-ed", "global-vars");
		}

		[MenuItem("Blox/Update Link File", priority = 1100)]
		public static void Create_LinkFile()
		{
			BloxEd.UpdateLinkFile();
		}

		[MenuItem("Blox/Generate Scripts", priority = 1101)]
		public static void Menu_ScriptGen_CompileAll()
		{
			BloxScriptGenerator.GenerateAllScripts();
		}

		[MenuItem("Blox/Remove all Scripts", priority = 1102)]
		public static void Menu_ScriptGen_RemoveAll()
		{
			BloxScriptGenerator.RemoveAllScripts();
		}

		[MenuItem("Blox/Toolbar", priority = 1200)]
		public static void Menu_ShowToolbar()
		{
			plyEdToolbar.ShowToolbar();
		}

		[MenuItem("Blox/Blox List", priority = 1201)]
		public static void Show_BloxList()
		{
			BloxListWindow.Show_BloxListWindow();
		}

		[MenuItem("Blox/Settings", priority = 1202)]
		public static void Show_BloxSettings()
		{
			BloxSettingsWindow.Show_BloxSettingsWindow();
		}

		[MenuItem("Blox/Game Systems/Main Window", priority = 1)]
		[MenuItem("Blox/Game Systems Window", priority = 1203)]
		public static void Menu_ShowBGSWindow()
		{
			MainEditorWindow.Show_MainEditorWindow();
		}

		[MenuItem("Blox/Documentation", priority = 1500)]
		public static void Menu_OpenHelp()
		{
			plyEdHelpManager.ShowHelp("blox");
		}

		[MenuItem("Blox/Asset Store", priority = 1501)]
		public static void Menu_OpenUAS()
		{
			Application.OpenURL(BloxEdGlobal.URL_STORE);
		}

		[MenuItem("Blox/Support", priority = 1502)]
		public static void Menu_OpenSupport()
		{
			Application.OpenURL(BloxEdGlobal.URL_SUPPORT);
		}

		[MenuItem("Blox/About", priority = 1503)]
		public static void Menu_ShowAbout()
		{
			Texture2D logo = plyEdGUI.LoadTextureResource("BloxEditor.res.logo.png", typeof(BloxEdGlobal).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			plyAboutWindow.Show_AboutWindow(" ", logo, plyEdUtil.GetVersion(plyEdUtil.PackagesFullPath + "version-Blox.txt"), BloxEdGlobal.URL_DOCS, BloxEdGlobal.URL_SUPPORT, BloxEdGlobal.URL_STORE, 320, 110);
		}

		public static bool DataSpotCheck()
		{
			if (!plyEdUtil.RelativeFileExist(BloxEdGlobal.ResourcesRoot + "BGSSettings.asset"))
			{
				return false;
			}
			if (EditorBuildSettings.scenes.Length != 0 && !(EditorBuildSettings.scenes[0].path != BloxEdGlobal.BootstrapScenePath) && EditorBuildSettings.scenes[0].enabled)
			{
				if (plyEdUtil.RelativeFileExist(BloxEdGlobal.BootstrapScenePath) && plyEdUtil.RelativeFileExist(BloxEdGlobal.BootstrapFabPath))
				{
					if (!plyEdUtil.RelativeFileExist(BloxEdGlobal.BloxGlobalFabPath))
					{
						return false;
					}
					return true;
				}
				return false;
			}
			return false;
		}

		public static void CheckAllData()
		{
			BloxEdGlobal.CheckLayersAndTags();
			BloxEd.LoadBloxGlobal();
			BloxEdGlobal.CheckBootstrap();
		}

		public static void CheckLayersAndTags()
		{
		}

		public static void CheckBootstrap()
		{
			plyEdUtil.CheckPath(BloxEdGlobal.DataRoot);
			plyEdUtil.CheckPath(BloxEdGlobal.ScenesPath);
			plyEdUtil.CheckPath(BloxEdGlobal.ResourcesRoot);
			BloxEdGlobal._gameSystemsSettings = plyEdUtil.LoadOrCreateAsset<BGSSettings>(BloxEdGlobal.ResourcesRoot + "BGSSettings.asset", true);
			GameObject gameObject = plyEdUtil.LoadPrefab(BloxEdGlobal.BootstrapFabPath);
			if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
			{
				gameObject = plyEdUtil.CreatePrefab<Bootstrap>("Bootstrap", BloxEdGlobal.BootstrapFabPath);
				AssetDatabase.SaveAssets();
			}
			Bootstrap component = gameObject.GetComponent<Bootstrap>();
			if ((UnityEngine.Object)component == (UnityEngine.Object)null)
			{
				gameObject.AddComponent<Bootstrap>();
				plyEdUtil.SetDirty(gameObject);
			}
			if ((UnityEngine.Object)component.bloxGlobalPrefab != (UnityEngine.Object)BloxEd.BloxGlobalPrefab)
			{
				component.bloxGlobalPrefab = BloxEd.BloxGlobalPrefab;
				plyEdUtil.SetDirty(component);
			}
			if (!plyEdUtil.RelativeFileExist(BloxEdGlobal.BootstrapScenePath))
			{
				if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorUtility.DisplayDialog("Blox Game Systems", "Could not check the Bootstrap scene and data. You can complete this action via menu: Blox > Check Data.", "OK");
					return;
				}
				Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
				GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(gameObject, scene);
				obj.name = "[Bootstrap]";
				plyEdUtil.SetDirty(obj);
				EditorSceneManager.MarkSceneDirty(scene);
				EditorSceneManager.SaveScene(scene, BloxEdGlobal.BootstrapScenePath);
				EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
			}
			plyEdUtil.AddSceneToBuildSettings(BloxEdGlobal.BootstrapScenePath, true);
		}
	}
}
