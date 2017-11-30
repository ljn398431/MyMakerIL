using BloxEditor.Databinding;
using BloxGameSystems;
using plyLibEditor;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	public class MainEditorPanel : MainEdChild
	{
		private static readonly GUIContent[] menuLabels = new GUIContent[4]
		{
			new GUIContent("Startup & Scenes"),
			new GUIContent("Properties Manager"),
			new GUIContent("Global Variables"),
			new GUIContent("Settings")
		};

		private static int menuIdx = 0;

		private Vector2[] scroll = new Vector2[2]
		{
			Vector2.zero,
			Vector2.zero
		};

		private Bootstrap bootstrap;

		private bool canTryLoadBootstrap;

		private EdManagedImages edManagedImages = new EdManagedImages();

		private EdAttributeDefinitions edAttributeDefinitions = new EdAttributeDefinitions();

		private static readonly GUIContent GC_GameScenes = new GUIContent(Ico._ellipsis_v + " Scenes");

		private static readonly GUIContent GC_AddScene = new GUIContent(Ico._add_c, "Add scene");

		private static readonly GUIContent GC_AutoloadBootstrap = new GUIContent(Ico._star, "The bootstrap scene should always load first");

		private static readonly GUIContent GC_RenameScene = new GUIContent(Ico._rename, "Rename scene");

		private static readonly GUIContent GC_RemoveScene = new GUIContent(Ico._remove, "Remove scene");

		private static readonly GUIContent GC_FixError = new GUIContent("Fix it now");

		private static readonly GUIContent GC_DropScenes = new GUIContent("Drop additional scenes here");

		private static readonly string STR_BootstrapError1 = "The bootstrap data could not be loaded.";

		private static readonly string STR_BootstrapError2 = "The bootstrap scene must be present and first in the list of scenes.";

		private static GUIContent GC_StartupOrder = new GUIContent(" ", "Toggle startup scene and order of loading");

		private static GUIContent GC_AutoLoad_OFF = new GUIContent(" ", "Toggle scene for auto-loading");

		private static GUIContent GC_AutoLoad_ON = new GUIContent(Ico._star_o, "Toggle scene for auto-loading");

		private static readonly GUIContent GC_PropertiesManager = new GUIContent(Ico._ellipsis_v + " Properties Manager");

		private static readonly GUIContent GC_DefinedProperties = new GUIContent("Defined Properties");

		private static readonly GUIContent GC_PropertyGetter = new GUIContent(Ico._upload, "Property's bound value getter/ reader");

		private static readonly GUIContent GC_PropertySetter = new GUIContent(Ico._download, "Property's bound value setter/ writer");

		private static readonly GUIContent GC_PropertyGetter2 = new GUIContent("Value getter/ reader");

		private static readonly GUIContent GC_PropertySetter2 = new GUIContent("Value setter/ writer");

		private static readonly GUIContent[] GC_RestoreDuringBootstrap = new GUIContent[2]
		{
			new GUIContent(Ico._radio_button_checked, "Persist this property's value changes and restore it via the setter during bootstrap?"),
			new GUIContent(Ico._radio_button_unchecked, "Persist this property's value changes and restore it via the setter during bootstrap?")
		};

		private static readonly GUIContent GC_s0 = new GUIContent();

		private static readonly GUIContent GC_s1 = new GUIContent();

		private plyReorderableList propertiesList;

		private static readonly GUIContent GC_GlobalVars = new GUIContent(Ico._ellipsis_v + " Global Variables");

		private Editor globalsEd;

		private static readonly GUIContent GC_Settings = new GUIContent(Ico._ellipsis_v + " Settings");

		private static readonly GUIContent GC_AutoLoadBootstrap = new GUIContent("Auto-load Bootstrap", "Auto-load Bootstrap when the Unity Play Button is used? This should be on if you are using the Blox Game Systems.");

		public override int order
		{
			get
			{
				return 0;
			}
		}

		public override string label
		{
			get
			{
				return Ico._settings + "Main";
			}
		}

		public override string ident
		{
			get
			{
				return "blox-main-ed";
			}
		}

		public override void OnFocus()
		{
			if ((UnityEngine.Object)this.bootstrap == (UnityEngine.Object)null)
			{
				this.bootstrap = plyEdUtil.LoadPrefab<Bootstrap>(BloxEdGlobal.BootstrapFabPath);
			}
		}

		public override void OnGUI()
		{
			if ((UnityEngine.Object)this.bootstrap == (UnityEngine.Object)null)
			{
				if (this.canTryLoadBootstrap && Event.current.type == EventType.Repaint)
				{
					this.canTryLoadBootstrap = false;
					this.bootstrap = plyEdUtil.LoadPrefab<Bootstrap>(BloxEdGlobal.BootstrapFabPath);
					GUIUtility.ExitGUI();
				}
				else if (plyEdGUI.MessageButton(MainEditorPanel.GC_FixError, MainEditorPanel.STR_BootstrapError1, MessageType.Error))
				{
					this.canTryLoadBootstrap = true;
					BloxEdGlobal.CheckBootstrap();
					GUIUtility.ExitGUI();
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				if (plyEdGUI.Menu(ref MainEditorPanel.menuIdx, ref this.scroll[0], MainEditorPanel.menuLabels, new GUILayoutOption[1]
				{
					GUILayout.Width(150f)
				}))
				{
					switch (MainEditorPanel.menuIdx)
					{
					case 0:
						this.SelectedStartupSettings();
						break;
					case 1:
						this.SelectedPropertiesManager();
						break;
					case 2:
						this.SelectGlobalVars();
						break;
					case 3:
						this.SelectedSettings();
						break;
					case 5:
						this.edAttributeDefinitions.OnSelected(base.editorWindow);
						break;
					case 6:
						this.edManagedImages.OnSelected(base.editorWindow);
						break;
					}
				}
				EditorGUILayout.BeginVertical();
				EditorGUILayout.Space();
				switch (MainEditorPanel.menuIdx)
				{
				case 0:
					this.DrawStartupSettings();
					break;
				case 1:
					this.DrawPropertiesManager();
					break;
				case 2:
					this.DrawGlobalVars();
					break;
				case 3:
					this.DrawSettings();
					break;
				case 5:
					this.edAttributeDefinitions.Draw(base.editorWindow);
					break;
				case 6:
					this.edManagedImages.Draw(base.editorWindow);
					break;
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
		}

		public override void OnForcedShow(string infoString)
		{
			if (infoString == "global-vars")
			{
				MainEditorPanel.menuIdx = 2;
				this.SelectGlobalVars();
			}
			else if (infoString == "attributes")
			{
				MainEditorPanel.menuIdx = 5;
				this.edAttributeDefinitions.OnSelected(base.editorWindow);
			}
			else if (infoString == "images")
			{
				MainEditorPanel.menuIdx = 6;
				this.edManagedImages.OnSelected(base.editorWindow);
			}
			else
			{
				Debug.LogError("not implemented - please report - MainEditorPanel.Show: " + infoString);
			}
		}

		private void SelectedStartupSettings()
		{
		}

		private void DrawStartupSettings()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(MainEditorPanel.GC_GameScenes, plyEdGUI.Styles.HeadLabel);
			EditorGUILayout.Space();
			plyEdHelpManager.Button("blox-scenes");
			EditorGUILayout.Space();
			if (GUILayout.Button(MainEditorPanel.GC_AddScene, plyEdGUI.Styles.BigButtonFlat))
			{
				string text = EditorUtility.OpenFilePanel("Select Scene", plyEdUtil.ProjectFullPath + "Assets/", "unity");
				if (!string.IsNullOrEmpty(text))
				{
					text = plyEdUtil.ProjectRelativePath(text);
					plyEdUtil.AddSceneToBuildSettings(text, false);
				}
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(10f);
			this.scroll[1] = EditorGUILayout.BeginScrollView(this.scroll[1]);
			if (EditorBuildSettings.scenes.Length == 0)
			{
				if (plyEdGUI.MessageButton(MainEditorPanel.GC_FixError, MainEditorPanel.STR_BootstrapError2, MessageType.Error))
				{
					BloxEdGlobal.CheckBootstrap();
					GUIUtility.ExitGUI();
					return;
				}
			}
			else
			{
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
				{
					if (i == 0 && (EditorBuildSettings.scenes[i].path != BloxEdGlobal.BootstrapScenePath || !EditorBuildSettings.scenes[i].enabled))
					{
						if (!plyEdGUI.MessageButton(MainEditorPanel.GC_FixError, MainEditorPanel.STR_BootstrapError2, MessageType.Error))
							break;
						this.canTryLoadBootstrap = true;
						BloxEdGlobal.CheckBootstrap();
						GUIUtility.ExitGUI();
						return;
					}
					if (EditorBuildSettings.scenes[i].enabled)
					{
						GUI.enabled = (i != 0);
						EditorGUILayout.BeginHorizontal();
						string text2 = EditorBuildSettings.scenes[i].path.Substring(7, EditorBuildSettings.scenes[i].path.Length - 13);
						if (i == 0)
						{
							GUILayout.Label(MainEditorPanel.GC_AutoloadBootstrap, plyEdGUI.Styles.ButtonLeft, GUILayout.Width(25f));
							GUILayout.Label(MainEditorPanel.GC_AutoloadBootstrap, plyEdGUI.Styles.ButtonMid, GUILayout.Width(25f));
						}
						else
						{
							int num5 = this.bootstrap.startupScenes.IndexOf(i);
							MainEditorPanel.GC_StartupOrder.text = ((num5 >= 0) ? (num5 + 1).ToString() : " ");
							if (GUILayout.Button(MainEditorPanel.GC_StartupOrder, plyEdGUI.Styles.ButtonLeft, GUILayout.Width(25f)))
							{
								num = i;
							}
							if (GUILayout.Button(this.bootstrap.autoloadScenes.Contains(i) ? MainEditorPanel.GC_AutoLoad_ON : MainEditorPanel.GC_AutoLoad_OFF, plyEdGUI.Styles.ButtonMid, GUILayout.Width(25f)))
							{
								num2 = i;
							}
						}
						if (GUILayout.Button(text2, plyEdGUI.Styles.ButtonMid_LeftText, GUILayout.Width(375f)) && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
						{
							EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path, OpenSceneMode.Single);
							if (SceneView.sceneViews.Count > 0)
							{
								((SceneView)SceneView.sceneViews[0]).Focus();
							}
						}
						if (GUILayout.Button(MainEditorPanel.GC_RenameScene, plyEdGUI.Styles.ButtonMid, GUILayout.Width(30f)))
						{
							num4 = i;
						}
						if (GUILayout.Button(MainEditorPanel.GC_RemoveScene, plyEdGUI.Styles.ButtonRight, GUILayout.Width(30f)))
						{
							num3 = i;
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = true;
					}
				}
				if (num3 >= 1)
				{
					plyEdUtil.RemoveSceneFromBuildSettings(num3);
				}
				if (num4 >= 1)
				{
					plyTextInputWiz.ShowWiz("Rename Scene", "Enter unique name for scene", plyEdUtil.SceneNameFromPath(EditorBuildSettings.scenes[num4].path), this.OnRenameScene, new object[1]
					{
						num4
					}, 250f);
				}
				if (num >= 1)
				{
					if (this.bootstrap.startupScenes.Contains(num))
					{
						this.bootstrap.startupScenes.Remove(num);
					}
					else
					{
						this.bootstrap.startupScenes.Add(num);
					}
					plyEdUtil.SetDirty(this.bootstrap);
				}
				if (num2 >= 1)
				{
					if (this.bootstrap.autoloadScenes.Contains(num2))
					{
						this.bootstrap.autoloadScenes.Remove(num2);
					}
					else
					{
						this.bootstrap.autoloadScenes.Add(num2);
					}
					plyEdUtil.SetDirty(this.bootstrap);
				}
			}
			EditorGUILayout.Space();
			Rect rect = GUILayoutUtility.GetRect(470f, 35f, plyEdGUI.Styles.DropArea, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
			GUI.Box(rect, MainEditorPanel.GC_DropScenes, plyEdGUI.Styles.DropArea);
			Event current = Event.current;
			if (current.type == EventType.DragUpdated && rect.Contains(current.mousePosition))
			{
				if (DragAndDrop.objectReferences.Length != 0 && DragAndDrop.objectReferences[0].GetType() == typeof(SceneAsset))
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
					current.Use();
				}
				else
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
			}
			EventType _ = current.type;
			if (current.type == EventType.DragPerform && rect.Contains(current.mousePosition) && DragAndDrop.objectReferences.Length != 0 && DragAndDrop.objectReferences[0].GetType() == typeof(SceneAsset))
			{
				int num6 = 0;
				while (num6 < DragAndDrop.objectReferences.Length && num6 < DragAndDrop.paths.Length)
				{
					if (DragAndDrop.objectReferences[num6].GetType() == typeof(SceneAsset))
					{
						plyEdUtil.AddSceneToBuildSettings(DragAndDrop.paths[num6], false);
					}
					num6++;
				}
				DragAndDrop.AcceptDrag();
				current.Use();
			}
			EditorGUILayout.EndScrollView();
		}

		private void OnRenameScene(plyTextInputWiz wiz)
		{
			int num = (int)wiz.args[0];
			string text = wiz.text;
			wiz.Close();
			if (num != -1 && !string.IsNullOrEmpty(text))
			{
				string text2 = AssetDatabase.RenameAsset(EditorBuildSettings.scenes[num].path, text);
				if (!string.IsNullOrEmpty(text2))
				{
					EditorUtility.DisplayDialog("Error", text2, "Close");
				}
				base.editorWindow.Repaint();
			}
		}

		private void SelectedPropertiesManager()
		{
			if (this.propertiesList == null)
			{
				this.propertiesList = new plyReorderableList(PropertiesManagerEd.PropertiesManager.properties, typeof(ManagedProperty), false, true, true, true, false, false, false, new plyReorderableList.Button[1]
				{
					new plyReorderableList.Button
					{
						label = new GUIContent(Ico._rename, "Rename selected Property"),
						callback = this.PropertiesList_RenameSelected,
						requireSelected = true
					}
				}, null);
				this.propertiesList.elementHeight = (float)(EditorGUIUtility.singleLineHeight + 6.0);
				this.propertiesList.drawHeaderCallback = this.DrawPropertiesListHead;
				this.propertiesList.drawElementCallback = this.DrawPropertiesListElement;
				this.propertiesList.onAddElement = this.PropertiesList_Add;
				this.propertiesList.onRemoveElement = this.PropertiesList_Remove;
			}
		}

		private void DrawPropertiesManager()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(MainEditorPanel.GC_PropertiesManager, plyEdGUI.Styles.HeadLabel);
			EditorGUILayout.Space();
			plyEdHelpManager.Button("blox-property-manager");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(10f);
			this.scroll[1] = EditorGUILayout.BeginScrollView(this.scroll[1]);
			this.propertiesList.DoLayoutList();
			EditorGUILayout.EndScrollView();
		}

		private void DrawPropertiesListHead(Rect rect)
		{
			GUI.Label(rect, MainEditorPanel.GC_DefinedProperties);
		}

		private void DrawPropertiesListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			ManagedProperty managedProperty = PropertiesManagerEd.PropertiesManager.properties[index];
			float num = (float)(rect.width / 3.0 - 10.0);
			Rect rect2 = new Rect(rect.x, (float)(rect.y + 2.0), num, EditorGUIUtility.singleLineHeight);
			rect2.width = 20f;
			if (GUI.Button(rect2, MainEditorPanel.GC_RestoreDuringBootstrap[(!managedProperty.runDuringBoot) ? 1 : 0], plyEdGUI.Styles.SmallButtonFlat))
			{
				managedProperty.runDuringBoot = !managedProperty.runDuringBoot;
				plyEdUtil.SetDirty(PropertiesManagerEd.PropertiesManager);
			}
			MainEditorPanel.GC_s0.text = managedProperty.propertyName;
			rect2.x += 20f;
			rect2.width = (float)(num - 20.0);
			GUI.Label(rect2, MainEditorPanel.GC_s0);
			MainEditorPanel.GC_s1.text = "[none]";
			Type type = managedProperty.getter.DataType(true);
			Type type2 = managedProperty.setter.DataType(false);
			if (type != null || type2 != null)
			{
				if (type == type2)
				{
					MainEditorPanel.GC_s1.text = "[" + BloxEd.PrettyTypeName(type, true) + "]";
				}
				else if (type == null || type2 == null)
				{
					MainEditorPanel.GC_s1.text = "[" + BloxEd.PrettyTypeName(type, true) + "|" + BloxEd.PrettyTypeName(type2, true) + "]";
				}
				else
				{
					MainEditorPanel.GC_s1.text = "[<color=red>" + BloxEd.PrettyTypeName(type, true) + "|" + BloxEd.PrettyTypeName(type2, true) + "</color>]";
				}
			}
			Vector2 vector = GUI.skin.label.CalcSize(MainEditorPanel.GC_s0);
			vector.x += 2f;
			Vector2 vector2 = plyEdGUI.Styles.Label_RT.CalcSize(MainEditorPanel.GC_s1);
			Rect position = new Rect(rect2.xMax - vector2.x, rect2.y, vector2.x, rect2.height);
			if (position.x < rect2.x + vector.x)
			{
				position.x = rect2.x + vector.x;
				position.width = rect2.xMax - (rect2.x + vector.x);
			}
			if (position.width > 0.0)
			{
				GUI.Label(position, MainEditorPanel.GC_s1, plyEdGUI.Styles.Label_RT);
			}
			rect2.x += (float)(rect2.width + 10.0);
			rect2.width = 20f;
			GUI.Label(rect2, MainEditorPanel.GC_PropertyGetter, plyEdGUI.Styles.Label);
			rect2.x += 20f;
			rect2.width = (float)(num - 20.0);
			DataProviderEd.DataBindingField(rect2, null, managedProperty.getter, PropertiesManagerEd.PropertiesManager, false, null);
			rect2.x += (float)(rect2.width + 10.0);
			rect2.width = 20f;
			GUI.Label(rect2, MainEditorPanel.GC_PropertySetter, plyEdGUI.Styles.Label);
			rect2.x += 20f;
			rect2.width = (float)(num - 20.0);
			DataProviderEd.DataBindingField(rect2, null, managedProperty.setter, PropertiesManagerEd.PropertiesManager, true, null);
		}

		private void PropertiesList_Add()
		{
			plyTextInputWiz.ShowWiz("Define Property", "Enter unique name for property", "", this.PropertiesList_OnAdd, null, 250f);
		}

		private void PropertiesList_OnAdd(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			PropertiesManagerEd.AddProperty(text);
			base.editorWindow.Repaint();
		}

		private void PropertiesList_RenameSelected()
		{
			plyTextInputWiz.ShowWiz("Define Property", "Enter unique name for property", PropertiesManagerEd.PropertiesManager.properties[this.propertiesList.index].propertyName, this.PropertiesList_DoRename, null, 250f);
		}

		private void PropertiesList_DoRename(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			PropertiesManagerEd.RenameProperty(this.propertiesList.index, text);
			base.editorWindow.Repaint();
		}

		private void PropertiesList_Remove()
		{
			PropertiesManagerEd.RemoveProperty(this.propertiesList.index);
		}

		private void SelectGlobalVars()
		{
		}

		private void DrawGlobalVars()
		{
			if ((UnityEngine.Object)this.globalsEd == (UnityEngine.Object)null || this.globalsEd.target != (UnityEngine.Object)BloxEd.GlobalVarsObj)
			{
				this.globalsEd = Editor.CreateEditor(BloxEd.GlobalVarsObj);
			}
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(MainEditorPanel.GC_GlobalVars, plyEdGUI.Styles.HeadLabel);
			EditorGUILayout.Space();
			plyEdHelpManager.Button("blox-variables");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(10f);
			this.scroll[1] = EditorGUILayout.BeginScrollView(this.scroll[1]);
			EditorGUILayout.BeginVertical(GUILayout.Width(MainEditorWindow.PanelContentWidth));
			this.globalsEd.OnInspectorGUI();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}

		private void SelectedSettings()
		{
		}

		private void DrawSettings()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(MainEditorPanel.GC_Settings, plyEdGUI.Styles.HeadLabel);
			EditorGUILayout.Space();
			plyEdHelpManager.Button("blox-bgssettings");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(10f);
			this.scroll[1] = EditorGUILayout.BeginScrollView(this.scroll[1]);
			EditorGUI.BeginChangeCheck();
			BloxEdGlobal.GameSystemsSettings.autoLoadBootstrapOnUnityPlay = EditorGUILayout.Toggle(MainEditorPanel.GC_AutoLoadBootstrap, BloxEdGlobal.GameSystemsSettings.autoLoadBootstrapOnUnityPlay);
			if (EditorGUI.EndChangeCheck())
			{
				plyEdUtil.SetDirty(BloxEdGlobal.GameSystemsSettings);
			}
			EditorGUILayout.Space();
			EditorGUILayout.EndScrollView();
		}
	}
}
