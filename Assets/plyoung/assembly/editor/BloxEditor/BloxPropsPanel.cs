using BloxEditor.Variables;
using BloxEngine;
using plyLibEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BloxEditor
{
	public class BloxPropsPanel
	{
		public enum PropsDockState
		{
			CanvasBig = 0,
			CanvasSmall = 1,
			CanvasShort = 2,
			Hidden = 3
		}

		public static readonly List<Type> SupportedEditTypes = new List<Type>
		{
			typeof(string),
			typeof(bool),
			typeof(char),
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(Quaternion),
			typeof(Rect),
			typeof(Color),
			typeof(Color32)
		};

		private static BloxPropsPanel _instance;

		private const float WidePanel = 350f;

		private const float ThinPanel = 250f;

		private const float TallPanel = 270f;

		private const float ShorPanel = 200f;

		public Rect propsRect = new Rect(0f, 0f, 350f, 270f);

		private BloxEdDef shownDef;

		private BloxEventDef activeEventDef;

		private BloxBlockDef selectedBlockDef;

		private BloxBlockEd currBlock;

		private EditorWindow ed;

		private int mouseWentDownIdx = -1;

		private Vector2 scroll = Vector2.zero;

		private static GUIContent GC_HelperLabel = new GUIContent();

		private static readonly GUIContent GC_EventVars = new GUIContent(Ico._event_variable + " Event Variables");

		private static readonly GUIContent GC_Help = new GUIContent(Ico._help_c, "Open documentation for the member or type that this Block was created from");

		private static readonly GUIContent[] GC_Dock = new GUIContent[4]
		{
			new GUIContent(Ico._chevron_right, "Change properties panel size"),
			new GUIContent(Ico._chevron_down, "Change properties panel size"),
			new GUIContent(Ico._chevron_down, "Hide properties panel"),
			new GUIContent(Ico._chevron_up, "Show properties panel")
		};

		private ReorderableList rol;

		private object currRolObj;

		private List<object> currRolList;

		private Type currRolType;

		public static BloxPropsPanel Instance
		{
			get
			{
				return BloxPropsPanel._instance ?? BloxPropsPanel.Create();
			}
		}

		public PropsDockState DockedState
		{
			get;
			private set;
		}

		private static BloxPropsPanel Create()
		{
			BloxPropsPanel._instance = new BloxPropsPanel();
			BloxPropsPanel._instance.DockedState = (PropsDockState)EditorPrefs.GetInt("Blox.PropsPanelDockState", 1);
			BloxPropsPanel._instance.RezizePanel();
			return BloxPropsPanel._instance;
		}

		public void SetActiveEventDef(BloxEventDef def)
		{
			this.activeEventDef = def;
			this.SetShownDef(this.activeEventDef);
		}

		public void SetShownDef(BloxEdDef def)
		{
			this.scroll = Vector2.zero;
			this.shownDef = def;
			if (this.shownDef == null)
			{
				if (this.selectedBlockDef != null)
				{
					this.shownDef = this.selectedBlockDef;
				}
				else
				{
					this.shownDef = this.activeEventDef;
				}
			}
			EditorWindow obj = this.ed;
			if ((object)obj != null)
			{
				obj.Repaint();
			}
		}

		public void SetSelectedBlock(BloxBlockEd block)
		{
			this.currBlock = block;
			this.selectedBlockDef = ((block != null) ? block.def : null);
			this.SetShownDef(this.selectedBlockDef);
		}

		private void RezizePanel()
		{
			switch (this.DockedState)
			{
			case PropsDockState.CanvasBig:
				this.propsRect.width = 350f;
				this.propsRect.height = 270f;
				break;
			case PropsDockState.CanvasSmall:
				this.propsRect.width = 250f;
				this.propsRect.height = 270f;
				break;
			case PropsDockState.CanvasShort:
				this.propsRect.width = 250f;
				this.propsRect.height = 200f;
				break;
			case PropsDockState.Hidden:
				this.propsRect.width = 0f;
				this.propsRect.height = 0f;
				break;
			}
		}

		public void DoGUI(EditorWindow ed, Vector2 lowerRight)
		{
			this.ed = ed;
			if (this.DockedState == PropsDockState.Hidden)
			{
				GUILayout.BeginArea(new Rect((float)(lowerRight.x - 35.0), (float)(lowerRight.y - 20.0), 28f, 20f), BloxEdGUI.Styles.PropsPanel);
				if (GUILayout.Button(BloxPropsPanel.GC_Dock[(int)this.DockedState], plyEdGUI.Styles.SmallButtonFlat, GUILayout.Width(20f)))
				{
					this.DockedState = PropsDockState.CanvasBig;
					EditorPrefs.SetInt("Blox.PropsPanelDockState", (int)this.DockedState);
					this.RezizePanel();
					BloxEditorWindow instance = BloxEditorWindow.Instance;
					if ((object)instance != null)
					{
						instance.Repaint();
					}
				}
				GUILayout.EndArea();
			}
			else
			{
				this.propsRect.x = lowerRight.x - this.propsRect.width;
				this.propsRect.y = lowerRight.y - this.propsRect.height;
				GUILayout.BeginArea(this.propsRect, BloxEdGUI.Styles.PropsPanel);
				EditorGUILayout.BeginHorizontal(plyEdGUI.Styles.TopBar);
				GUILayout.Label((this.shownDef == null) ? " " : this.shownDef.name, BloxEdGUI.Styles.PropsHead, GUILayout.Width((float)(this.propsRect.width - 60.0)));
				if (this.shownDef != null && this.shownDef.bloxdoc != null && !string.IsNullOrEmpty(this.shownDef.bloxdoc.url))
				{
					if (GUILayout.Button(BloxPropsPanel.GC_Help, plyEdGUI.Styles.SmallButtonFlat, GUILayout.Width(20f)))
					{
						Application.OpenURL(this.shownDef.bloxdoc.url);
					}
				}
				else
				{
					GUILayout.Space(24f);
				}
				if (GUILayout.Button(BloxPropsPanel.GC_Dock[(int)this.DockedState], plyEdGUI.Styles.SmallButtonFlat, GUILayout.Width(20f)))
				{
					this.DockedState++;
					if (this.DockedState > PropsDockState.Hidden)
					{
						this.DockedState = PropsDockState.CanvasBig;
					}
					EditorPrefs.SetInt("Blox.PropsPanelDockState", (int)this.DockedState);
					this.RezizePanel();
					ed.Repaint();
				}
				EditorGUILayout.EndHorizontal();
				this.scroll = EditorGUILayout.BeginScrollView(this.scroll);
				if (this.shownDef != null)
				{
					this.DrawProperties();
					BloxEd.Instance.DrawBloxDoc(this.shownDef, true, ed);
				}
				EditorGUILayout.Space();
				EditorGUILayout.EndScrollView();
				GUILayout.EndArea();
			}
		}

		private void DrawProperties()
		{
			if (this.currBlock == null || this.selectedBlockDef != this.shownDef || this.currBlock.def == null)
			{
				this.DrawEventProperties();
			}
			else if (this.currBlock.def.drawer != null)
			{
				EditorGUI.BeginChangeCheck();
				this.currBlock.def.drawer.DrawProperties(BloxEditorWindow.Instance, this.currBlock);
				if (EditorGUI.EndChangeCheck())
				{
					GUI.changed = false;
					BloxEditorWindow.Instance.SaveBlox(true);
				}
				EditorGUILayout.Space();
			}
			else if (this.currBlock.b.blockType == BloxBlockType.Value && this.currBlock.b.returnType != null && this.currBlock.b.returnType != typeof(void) && this.currBlock.def.contextType == null && (this.currBlock.def.mi == null || this.currBlock.def.mi.CanSetValue))
			{
				if (this.currBlock.b.returnValue == null)
				{
					this.currBlock.b.returnValue = BloxMemberInfo.GetDefaultValue(this.currBlock.b.returnType);
					if (this.currBlock.b.returnValue != null)
					{
						GUI.changed = true;
					}
				}
				EditorGUI.BeginChangeCheck();
				this.currBlock.b.returnValue = this.Edit_Field(this.currBlock.b.returnType, this.currBlock.b.returnValue, this.currBlock);
				if (EditorGUI.EndChangeCheck())
				{
					GUI.changed = false;
					BloxEditorWindow.Instance.SaveBlox(true);
				}
				EditorGUILayout.Space();
			}
		}

		private void DrawEventProperties()
		{
			if (((this.activeEventDef != null) ? ((this.activeEventDef == this.shownDef) ? this.activeEventDef.pars.Length : 0) : 0) != 0)
			{
				GUILayout.Label(BloxPropsPanel.GC_EventVars, plyEdGUI.Styles.Label);
				Event current = Event.current;
				for (int i = 0; i < this.activeEventDef.pars.Length; i++)
				{
					BloxPropsPanel.GC_HelperLabel.text = "<b>" + this.activeEventDef.pars[i].name + "</b>: " + this.activeEventDef.pars[i].typeName;
					Rect rect = GUILayoutUtility.GetRect(BloxPropsPanel.GC_HelperLabel, BloxEdGUI.Styles.DocParamNormal);
					if (current.type == EventType.Repaint)
					{
						BloxEdGUI.Styles.DocParamNormal.Draw(rect, BloxPropsPanel.GC_HelperLabel, false, false, false, false);
					}
					if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
					{
						this.mouseWentDownIdx = i;
					}
					else if (current.type == EventType.MouseUp)
					{
						this.mouseWentDownIdx = -1;
					}
					else if (current.type == EventType.MouseDrag && this.mouseWentDownIdx == i && rect.Contains(current.mousePosition))
					{
						this.mouseWentDownIdx = -1;
						plyEdGUI.ClearFocus();
						DragAndDrop.PrepareStartDrag();
						DragAndDrop.objectReferences = new UnityEngine.Object[0];
						DragAndDrop.paths = null;
						DragAndDrop.SetGenericData("plyVariable", new plyVariablesEditor.DragDropData
						{
							name = this.activeEventDef.pars[i].name,
							plyVarType = plyVariablesType.Event,
							varValType = this.activeEventDef.pars[i].type
						});
						DragAndDrop.StartDrag(this.activeEventDef.pars[i].name);
						Event.current.Use();
					}
				}
				EditorGUILayout.Space();
			}
		}
        /// <summary>
        /// 可编辑 输入面板 判断类型并表现
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="block"></param>
        /// <returns></returns>
		private object Edit_Field(Type type, object value, BloxBlockEd block)
		{
			EditorGUIUtility.wideMode = true;
			if (type.IsArray)
			{
				return this.Edit_ListOrArray(type, value, block);
			}
			if (type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(List<>))
				{
					return this.Edit_ListOrArray(type, value, block);
				}
				return value;
			}
			EditorGUILayout.Space();
			if (type.IsEnum)
			{
				if (type.IsDefined(typeof(FlagsAttribute), true))
				{
					return EditorGUILayout.EnumMaskField(value as Enum);
				}
				return EditorGUILayout.EnumPopup(value as Enum);
			}
			if (type == typeof(bool))
			{
				return EditorGUILayout.ToggleLeft(((bool)value) ? " (True)" : " (False)", (bool)value);
			}
			if (type == typeof(int))
			{
				return EditorGUILayout.IntField((int)value);
			}
			if (type == typeof(float))
			{
				return EditorGUILayout.FloatField((float)value);
			}
			if (type == typeof(string))
			{
				return EditorGUILayout.TextArea((string)value, plyEdGUI.Styles.TextArea);
			}
			if (type == typeof(Rect))
			{
				return plyEdGUI.RectField((Rect)value);
			}
			if (type == typeof(Vector2))
			{
				return EditorGUILayout.Vector2Field(GUIContent.none, (Vector2)value);
			}
			if (type == typeof(Vector3))
			{
				return EditorGUILayout.Vector3Field(GUIContent.none, (Vector3)value);
			}
			if (type == typeof(Vector4))
			{
				return plyEdGUI.Vector4Field((Vector4)value);
			}
			if (type == typeof(Quaternion))
			{
				return plyEdGUI.QuaternionField((Quaternion)value);
			}
			if (type == typeof(Color))
			{
				if (value == null)
				{
					value = Color.black;
				}
				return EditorGUILayout.ColorField((Color)value);
			}
			if (type == typeof(Color32))
			{
				return EditorGUILayout.ColorField((Color32)value);
			}
			if (type == typeof(char))
			{
				return plyEdGUI.CharField((char)value);
			}
			if (type == typeof(byte))
			{
				return plyEdGUI.ByteField((byte)value);
			}
			if (type == typeof(sbyte))
			{
				return plyEdGUI.SByteField((sbyte)value);
			}
			if (type == typeof(short))
			{
				return plyEdGUI.Int16Field((short)value);
			}
			if (type == typeof(ushort))
			{
				return plyEdGUI.UInt16Field((ushort)value);
			}
			if (type == typeof(uint))
			{
				return plyEdGUI.UInt32Field((uint)value);
			}
			if (type == typeof(long))
			{
				return plyEdGUI.Int64Field((long)value);
			}
			if (type == typeof(ulong))
			{
				return plyEdGUI.UInt64Field((ulong)value);
			}
			if (type == typeof(double))
			{
				return EditorGUILayout.DoubleField((double)value);
			}
			if (type == typeof(decimal))
			{
				return plyEdGUI.DecimalField((decimal)value);
			}
			return value;
		}

		private object Edit_FieldInListOrArray(Rect r, int index, Type type, object value, BloxBlockEd block)
		{
			EditorGUIUtility.wideMode = true;
			if (type.IsEnum)
			{
				if (type.IsDefined(typeof(FlagsAttribute), false))
				{
					return EditorGUI.EnumMaskField(r, value as Enum);
				}
				return EditorGUI.EnumPopup(r, value as Enum);
			}
			if (type == typeof(bool))
			{
				return EditorGUI.ToggleLeft(r, ((bool)value) ? " (True)" : " (False)", (bool)value);
			}
			if (type == typeof(int))
			{
				return EditorGUI.IntField(r, (int)value);
			}
			if (type == typeof(float))
			{
				return EditorGUI.FloatField(r, (float)value);
			}
			if (type == typeof(string))
			{
				return EditorGUI.TextArea(r, (string)value, plyEdGUI.Styles.TextArea);
			}
			if (type == typeof(Rect))
			{
				return plyEdGUI.RectField(r, (Rect)value);
			}
			if (type == typeof(Vector2))
			{
				return EditorGUI.Vector2Field(r, GUIContent.none, (Vector2)value);
			}
			if (type == typeof(Vector3))
			{
				return EditorGUI.Vector3Field(r, GUIContent.none, (Vector3)value);
			}
			if (type == typeof(Vector4))
			{
				return plyEdGUI.Vector4Field(r, (Vector4)value);
			}
			if (type == typeof(Quaternion))
			{
				return plyEdGUI.QuaternionField(r, (Quaternion)value);
			}
			if (type == typeof(Color))
			{
				return EditorGUI.ColorField(r, (Color)value);
			}
			if (type == typeof(Color32))
			{
				return EditorGUI.ColorField(r, (Color32)value);
			}
			if (type == typeof(char))
			{
				return plyEdGUI.CharField(r, (char)value);
			}
			if (type == typeof(byte))
			{
				return plyEdGUI.ByteField(r, (byte)value);
			}
			if (type == typeof(sbyte))
			{
				return plyEdGUI.SByteField(r, (sbyte)value);
			}
			if (type == typeof(uint))
			{
				return plyEdGUI.UInt32Field(r, (uint)value);
			}
			if (type == typeof(short))
			{
				return plyEdGUI.Int16Field(r, (short)value);
			}
			if (type == typeof(ushort))
			{
				return plyEdGUI.UInt16Field(r, (ushort)value);
			}
			if (type == typeof(long))
			{
				return plyEdGUI.Int64Field(r, (long)value);
			}
			if (type == typeof(ulong))
			{
				return plyEdGUI.UInt64Field(r, (ulong)value);
			}
			if (type == typeof(double))
			{
				return EditorGUI.DoubleField(r, (double)value);
			}
			if (type == typeof(decimal))
			{
				return plyEdGUI.DecimalField(r, (decimal)value);
			}
			return value;
		}

		private object Edit_ListOrArray(Type type, object value, BloxBlockEd block)
		{
			if (this.rol == null)
			{
				this.rol = new ReorderableList(new List<object>(), typeof(object), true, false, true, true);
				this.rol.elementHeight = 19f;
				this.rol.onAddCallback = this.Edit_ListOrArray_OnAdd;
				this.rol.onRemoveCallback = this.Edit_ListOrArray_OnRemove;
				this.rol.drawElementCallback = this.Edit_ListOrArray_DrawElem;
				this.rol.onReorderCallback = this.Edit_ListOrArray_Reorder;
			}
			if (type.IsArray)
			{
				if (this.currRolObj != value || this.currRolObj == null)
				{
					this.currRolType = type.GetElementType();
					Array array = null;
					if (value == null)
					{
						GUI.changed = true;
						array = (Array)(value = Array.CreateInstance(this.currRolType, 0));
					}
					else
					{
						array = (Array)value;
					}
					this.currRolObj = value;
					this.currRolList = new List<object>();
					for (int i = 0; i < array.Length; i++)
					{
						this.currRolList.Add(array.GetValue(i));
					}
					this.rol.list = this.currRolList;
				}
				this.rol.DoLayoutList();
				return this.currRolList.ToArray();
			}
			if (this.currRolObj != value || this.currRolObj == null)
			{
				this.currRolType = type.GetGenericArguments()[0];
				if (value == null)
				{
					GUI.changed = true;
					value = Activator.CreateInstance(type);
					this.currRolList = ((IList)value).Cast<object>().ToList();
				}
				else
				{
					this.currRolList = ((IList)value).Cast<object>().ToList();
				}
				this.currRolObj = this.currRolList;
				this.rol.list = this.currRolList;
			}
			this.rol.DoLayoutList();
			return this.currRolList;
		}

		private void Edit_ListOrArray_OnAdd(ReorderableList rol)
		{
			this.currRolList.Add(BloxMemberInfo.GetDefaultValue(this.currRolType));
			GUI.changed = true;
			EditorWindow obj = this.ed;
			if ((object)obj != null)
			{
				obj.Repaint();
			}
		}

		private void Edit_ListOrArray_OnRemove(ReorderableList rol)
		{
			this.currRolList.RemoveAt(rol.index);
			GUI.changed = true;
			EditorWindow obj = this.ed;
			if ((object)obj != null)
			{
				obj.Repaint();
			}
		}

		private void Edit_ListOrArray_DrawElem(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (index >= 0 && index < this.currRolList.Count)
			{
				object _ = this.currRolList[index];
				rect.x += 10f;
				rect.width -= 10f;
				rect.y += 1f;
				rect.height -= 3f;
				this.currRolList[index] = this.Edit_FieldInListOrArray(rect, index, this.currRolType, this.currRolList[index], this.currBlock);
			}
		}

		private void Edit_ListOrArray_Reorder(ReorderableList list)
		{
			GUI.changed = true;
			EditorWindow obj = this.ed;
			if ((object)obj != null)
			{
				obj.Repaint();
			}
		}
	}
}
