using BloxEditor.Variables;
using BloxEngine;
using plyLibEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
    public class BloxEditorWindow : EditorWindow
    {
        private enum DragOut
        {
            None = 0,
            DontWorryAboutIt = 1,
            Unlinked1st = 2,
            Event1st = 3,
            ParentBlock1st = 4,
            UnderBlock = 5,
            WasContext = 6,
            WasParam = 7
        }

        public static Action<Blox> ToolbarDrawCallbacks;

        private static readonly GUIContent GC_Settings = new GUIContent(Ico._settings, "Open Blox settings");

        private static readonly GUIContent GC_Cleanup = new GUIContent(Ico._delete, "Remove all unlinked Blocks (Blocks that are not part of the Event graph)");

        private static readonly GUIContent GC_ResetView = new GUIContent(Ico._move, "Reset view");

        private static readonly GUIContent GC_Events = new GUIContent("Events");

        private static readonly GUIContent GC_Editing = new GUIContent(" Editing:");

        private static readonly GUIContent GC_LoadingEvents = new GUIContent("loading events");

        private static readonly GUIContent GC_SelectBlox = new GUIContent("Select Blox to Edit");

        private static readonly GUIContent GC_EventDisabled = new GUIContent(Ico._disabled);

        private const float MinPanelWidth = 190f;

        private static readonly Color InactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

        private static BloxBlockDef.Param EmptyBloxBlockDefParam = new BloxBlockDef.Param
        {
            name = "",
            type = typeof(object),
            showName = false,
            emptyVal = "null"
        };

        public static BloxEditorWindow Instance;

        [NonSerialized]
        public Blox blox;

        [NonSerialized]
        private int eventIdx = -2;

        [NonSerialized]
        private BloxEventEd currEvent = new BloxEventEd();

        [NonSerialized]
        private static bool isOpenFromBloxLoad = false;

        private Vector2[] scroll = new Vector2[3]
        {
            Vector2.zero,
            Vector2.zero,
            Vector2.zero
        };

        private float[] panelWidth = new float[2]
        {
            200f,
            220f
        };

        private Rect[] splitterRect = new Rect[2];

        private int draggingSplitter = -1;

        private bool draggingCanvas;

        private bool showPropsPanel = true;

        private bool doRepaint;

        private Rect canvasRect;

        private plyEdGUI.ListOps evListOps;

        private BloxEventsPopup eventsPopup;

        private BloxBlockDef dragDrop;

        private BloxBlockEd currBlock;

        private BloxBlockEd currBlock_change;

        private bool dragDropInsertTop;

        private bool dragDropInsertTop_change;

        private BloxBlockEd dragDropInsertContext;

        private BloxBlockEd dragDropInsertContext_change;

        private int dragDropInsertFieldIdx = -1;

        private int dragDropInsertFieldIdx_change = -1;

        private float dragDropExtraWidth;

        private float dragDropExtraWidth_change;

        private bool mustShowContextMenu;

        private int inactiveDepth;

        private Vector2 blockAreaV2;

        private DragOut dragOut;

        private Vector2 dragOutViewOffs = Vector2.zero;

        private BloxBlockEd dragOutBlockRef;

        private BloxBlockEd dragDropBlock;

        private BloxBlockEd copiedBlock;

        private bool blockDragStartDetected;

        private Stopwatch blockDragTimer;

        private const float BLOCK_DRAG_SENSE = 120f;

        private bool wasLoadingEvent;

        private GUIContent GC_EventWindowLabel = new GUIContent();

        private GUIContent GC_BlockLabel = new GUIContent();

        private GUIContent GC_DragDropLabel = new GUIContent();

        public static void Show_BloxEditorWindow(Blox bloxDef)
        {
            if (!((UnityEngine.Object)bloxDef == (UnityEngine.Object)null))
            {
                BloxEditorWindow.isOpenFromBloxLoad = true;
                BloxEditorWindow obj = BloxEditorWindow.Instance = EditorWindow.GetWindow<BloxEditorWindow>("BloxEd");
                Texture2D image = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.blox_mono" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(BloxEditorWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
                obj.titleContent = new GUIContent("BloxEd", image);
                obj.LoadBloxDef(bloxDef, false);
            }
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable ", "BloxEditorWindow", Color.green);
            BloxEditorWindow.Instance = this;
            plyEdGUI.AddThemeChangeCallback(this.OnThemeChanged);
            Texture2D image = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.blox_mono" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(BloxEditorWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
            base.titleContent = new GUIContent("BloxEd", image);
            this.panelWidth[0] = EditorPrefs.GetFloat("Blox.panelWidth.0", 200f);
            this.panelWidth[1] = EditorPrefs.GetFloat("Blox.panelWidth.1", 220f);
            this.showPropsPanel = EditorPrefs.GetBool("Blox.ShowPropsPanel", this.showPropsPanel);
            BloxEd.LoadBloxGlobal();
            BloxEd.Instance.LoadBlockDefs(false);
            BloxEd.Instance.LoadEventDefs();
            this.eventsPopup = new BloxEventsPopup();
            if (this.blockDragTimer == null)
            {
                this.blockDragTimer = new Stopwatch();
            }
            if (this.evListOps == null)
            {
                //创建事件列表基类
                Debug.Log("plyEdGUI.ListOps creat evListOps ", "BloxEditorWindow", Color.green);
                this.evListOps = new plyEdGUI.ListOps
                {
                    emptyMsg = "No Events defined",
                    toolbarStyle = plyEdGUI.ListOps.ToobarStyle.List,
                    drawBackground = false,
                    onDrawHeader = this.DrawEventsListHeader,
                    onDrawElement = this.DrawEventsListElement,
                    onAction = this.EventsListAction,
                    extraButtons = new plyEdGUI.ListOpsExtraToolbarButton[1]
                    {
                        new plyEdGUI.ListOpsExtraToolbarButton
                        {
                            label = new GUIContent(Ico._rename, "Rename selected Event"),
                            callback = this.RenameEvent,
                            enabled = (() => this.eventIdx >= 0)
                        }
                    },
                    canAdd = true,
                    canDuplicate = true,
                    canRemove = true,
                    canChangePosition = true

                };
            }
            this.RestoreLastLoadedBlox();
        }

        private void OnDestroy()
        {
            plyEdGUI.RemoveThemeChangeCallback(this.OnThemeChanged);
        }

        private void OnFocus()
        {
            BloxEditorWindow.Instance = this;
            base.wantsMouseMove = true;
        }

        private void OnThemeChanged()
        {
            BloxEventsPopup obj = this.eventsPopup;
            if (obj != null)
            {
                obj.OnThemeChanged();
            }
            BloxBlocksList.OnThemeChanged();
        }

        private void RestoreLastLoadedBlox()
        {
            if ((UnityEngine.Object)this.blox != (UnityEngine.Object)null || BloxEditorWindow.isOpenFromBloxLoad)
            {
                BloxEditorWindow.isOpenFromBloxLoad = false;
            }
            else
            {
                string @string = EditorPrefs.GetString("Blox.LastLoadedBlox." + plyEdUtil.ProjectEdPrefsKey, null);
                Blox blox = BloxEd.BloxGlobalObj.FindBloxDef(@string);
                if ((UnityEngine.Object)blox == (UnityEngine.Object)null && (UnityEngine.Object)BloxListWindow.Instance != (UnityEngine.Object)null)
                {
                    blox = BloxListWindow.Instance.CurrentlySelectedBlox();
                    if ((UnityEngine.Object)blox == (UnityEngine.Object)null)
                        return;
                }
                this.LoadBloxDef(blox, false);
            }
        }

        private void LoadBloxDef(Blox bloxDef, bool forcedReload = false)
        {
            if ((UnityEngine.Object)this.blox == (UnityEngine.Object)bloxDef && !forcedReload)
                return;
            this.eventIdx = -2;
            this.currEvent.Clear();
            this.blox = bloxDef;
            if (!((UnityEngine.Object)this.blox == (UnityEngine.Object)null))
            {
                Debug.Log("Deserialize", "BloxEditorWindow", Color.yellow);
                EditorPrefs.SetString("Blox.LastLoadedBlox." + plyEdUtil.ProjectEdPrefsKey, this.blox.ident);
                this.blox.Deserialize();
                BloxListWindow instance = BloxListWindow.Instance;
                if ((object)instance != null)
                {
                    instance.SetSelectBlox(this.blox);
                }
                this.doRepaint = true;
            }
        }

        public void SaveBlox(bool blocksChanged)
        {
            if (blocksChanged)
            {
                Debug.Log("SaveBlox", "BloxEditorWindow", Color.cyan);
                BloxEvent ev = this.currEvent.ev;
                if (ev != null)
                {
                    ev._SetDirty();
                }
                this.blox.scriptDirty = true;
                this.blox._SetDirty();
            }
            plyEdUtil.SetDirty(this.blox);
        }

        public void UnselectAll()
        {
            plyEdGUI.ClearFocus();
            this.currBlock_change = null;
        }

        public static void Reload()
        {
            if (!((UnityEngine.Object)BloxEditorWindow.Instance == (UnityEngine.Object)null) && !((UnityEngine.Object)BloxEditorWindow.Instance.blox == (UnityEngine.Object)null))
            {
                BloxEditorWindow.Instance.LoadBloxDef(BloxEditorWindow.Instance.blox, true);
            }
        }

        protected void OnGUI()
        {
            if ((UnityEngine.Object)this.blox == (UnityEngine.Object)null)
            {
                Debug.Log("OnGUI blox is null", "BlockEditorWindows", Color.red);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(300f));
                if ((UnityEngine.Object)BloxListWindow.Instance == (UnityEngine.Object)null)
                {
                    EditorGUILayout.Space();
                    if (GUILayout.Button(BloxEditorWindow.GC_SelectBlox))
                    {
                        BloxListWindow.Show_BloxListWindow();
                    }
                    EditorGUILayout.Space();
                }
                else
                {
                    BloxListWindow.Instance.DrawBloxList();
                }
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                //Debug.Log("OnGUI blox " + blox, "BlockEditorWindows", Color.green);
                Event current = Event.current;
                if (current.type == EventType.Repaint)
                {
                    this.dragDropInsertTop_change = false;
                    this.dragDropInsertContext_change = null;
                    this.dragDropInsertFieldIdx_change = -1;
                    this.dragDropExtraWidth_change = 0f;
                }
                if (this.blockDragStartDetected && current.button == 0 && (current.type == EventType.MouseDown || current.type == EventType.MouseUp))
                {
                    this.blockDragStartDetected = false;
                    this.blockDragTimer.Stop();
                }
                EditorGUILayout.BeginHorizontal();
                this.DoEventsList();
                EditorGUILayout.BeginVertical();
                this.DoCanvasToolbar();
                this.DoCanvas();
                EditorGUILayout.EndVertical();
                this.DoBlocksList();
                EditorGUILayout.EndHorizontal();
                this.HandleEvents();
                if (current.type == EventType.Repaint)
                {
                    if (this.currEvent.Loading)
                    {
                        this.currEvent.DoUpdate();
                        this.doRepaint = true;
                        this.wasLoadingEvent = true;
                    }
                    if (BloxEd.Instance.EventDefsLoading || BloxEd.Instance.BlockDefsLoading)
                    {
                        BloxEd.Instance.DoUpdate();
                        this.doRepaint = true;
                    }
                    else
                    {
                        if (this.currBlock != this.currBlock_change)
                        {
                            plyEdGUI.ClearFocus();
                            this.currBlock = this.currBlock_change;
                            BloxPropsPanel.Instance.SetSelectedBlock(this.currBlock);
                            this.doRepaint = true;
                        }
                        this.dragDropInsertTop = this.dragDropInsertTop_change;
                        this.dragDropInsertContext = this.dragDropInsertContext_change;
                        this.dragDropInsertFieldIdx = this.dragDropInsertFieldIdx_change;
                        this.dragDropExtraWidth = this.dragDropExtraWidth_change;
                    }
                }
                if (this.doRepaint)
                {
                    this.doRepaint = false;
                    base.Repaint();
                }
            }
        }
        /// <summary>
        /// 处理画布面板事件
        /// </summary>
        private void HandleEvents()
        {
            //Debug.Log("HandleEvents", "BlockEditorWindows", Color.green);
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseMove:
                case EventType.KeyUp:
                case EventType.Layout:
                case EventType.Ignore:
                case EventType.Used:
                    break;
                case EventType.ValidateCommand:
                    {
                        string commandName = current.commandName;
                        switch (commandName)
                        {
                            default:
                                if (commandName == "Delete")
                                    break;
                                return;
                            case "UndoRedoPerformed":
                                this.UndoRedoPerformed();
                                return;
                            case "Duplicate":
                            case "Cut":
                            case "Copy":
                            case "Paste":
                                break;
                        }
                        current.Use();
                        break;
                    }
                case EventType.ExecuteCommand:
                    {
                        string commandName = current.commandName;
                        switch (commandName)
                        {
                            default:
                                if (commandName == "Delete")
                                {
                                    this.DeleteSelectedBlock(false);
                                    current.Use();
                                }
                                break;
                            case "Duplicate":
                                this.StoreSelectedAsCopied();
                                this.PasteBlock();
                                current.Use();
                                break;
                            case "Cut":
                                this.CutSelectedBlock(false);
                                current.Use();
                                break;
                            case "Copy":
                                this.StoreSelectedAsCopied();
                                current.Use();
                                break;
                            case "Paste":
                                this.PasteBlock();
                                current.Use();
                                break;
                        }
                        break;
                    }
                case EventType.KeyDown:
                    if (!GUI.changed)
                    {
                        if (current.keyCode == KeyCode.Delete || current.keyCode == KeyCode.Backspace)
                        {
                            if (this.currBlock != null)
                            {
                                this.DeleteSelectedBlock(false);
                                current.Use();
                            }
                        }
                        else if (Event.current.keyCode == KeyCode.Escape)
                        {
                            if (this.dragDropBlock != null)
                            {
                                this.RestoreFromDragOut();
                                this.ClearDragDropBlock();
                                current.Use();
                            }
                            else if (this.currBlock != null)
                            {
                                this.currBlock_change = null;
                                this.doRepaint = true;
                                current.Use();
                            }
                        }
                    }
                    break;
                case EventType.MouseDown:
                    if (current.button == 1 && this.dragDropBlock != null)
                    {
                        this.RestoreFromDragOut();
                        this.ClearDragDropBlock();
                        current.Use();
                        
                    }
                    else if (current.button == 0 && this.dragDropBlock != null)
                    {
                        plyEdGUI.ClearFocus();
                        Debug.Log("current" + current, Color.blue);
                        current.Use();
                        Debug.Log("current after" + current, Color.blue);
                        if (this.canvasRect.Contains(current.mousePosition) && !BloxPropsPanel.Instance.propsRect.Contains(current.mousePosition))
                        {
                            Debug.Log("MouseDown add block", "BloxEditorWindow", Color.magenta);
                            this.AddBlock();
                        }
                    }
                    else
                    {
                        if ((current.button == 1 || (current.button == 0 && current.modifiers == EventModifiers.Control)) && this.canvasRect.Contains(current.mousePosition) && !BloxPropsPanel.Instance.propsRect.Contains(current.mousePosition))
                        {
                            this.mustShowContextMenu = true;
                            current.Use();
                        }
                        if (current.button == 0)
                        {
                            if (this.splitterRect[0].Contains(current.mousePosition))
                            {
                                this.draggingSplitter = 0;
                                plyEdGUI.ClearFocus();
                                current.Use();
                                
                            }
                            else if (BloxEdGlobal.BlocksListDocked && this.splitterRect[1].Contains(current.mousePosition))
                            {
                                this.draggingSplitter = 1;
                                plyEdGUI.ClearFocus();
                                current.Use();
                            }
                            else if (this.canvasRect.Contains(current.mousePosition) && !BloxPropsPanel.Instance.propsRect.Contains(current.mousePosition))
                            {
                                this.draggingCanvas = true;
                                plyEdGUI.ClearFocus();
                                //Debug.Log("current" + current, Color.magenta);
                                current.Use();
                                //Debug.Log("current after" + current, Color.magenta);
                            }
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (current.button == 0)
                    {
                        if (this.dragOut != 0)
                        {
                            current.Use();
                            if (this.dragDropBlock != null)
                            {
                                Debug.Log("MouseUp add block", "BloxEditorWindow", Color.magenta);
                                this.AddBlock();
                            }
                        }
                        else if (this.draggingCanvas)
                        {
                            this.SaveBlox(false);
                            this.draggingCanvas = false;
                            current.Use();
                            this.doRepaint = true;
                        }
                        else if (this.draggingSplitter >= 0)
                        {
                            EditorPrefs.SetFloat("Blox.panelWidth." + this.draggingSplitter, this.panelWidth[this.draggingSplitter]);
                            this.draggingSplitter = -1;
                            current.Use();
                            this.doRepaint = true;
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (current.button == 0)
                    {
                        if (this.blockDragStartDetected)
                        {
                            if ((float)this.blockDragTimer.ElapsedMilliseconds > 120.0)
                            {
                                this.blockDragStartDetected = false;
                                this.blockDragTimer.Stop();
                                this.CutSelectedBlock(true);
                            }
                        }
                        else if (this.draggingCanvas)
                        {
                            if (this.currEvent.ev != null)
                            {
                                this.currEvent.ev._ed_viewOffs.x += current.delta.x;
                                this.currEvent.ev._ed_viewOffs.y += current.delta.y;
                                current.Use();
                                this.doRepaint = true;
                            }
                            else
                            {
                                this.draggingCanvas = false;
                            }
                        }
                        else if (this.draggingSplitter >= 0)
                        {
                            this.panelWidth[this.draggingSplitter] += (float)((this.draggingSplitter == 0) ? current.delta.x : (0.0 - current.delta.x));
                            this.panelWidth[this.draggingSplitter] = Mathf.Clamp(this.panelWidth[this.draggingSplitter], 190f, (float)(base.position.width / 2.0));
                            EditorPrefs.SetFloat("Blox.panelWidth." + this.draggingSplitter, this.panelWidth[this.draggingSplitter]);
                            current.Use();
                            this.doRepaint = true;
                        }
                    }
                    break;
                case EventType.ScrollWheel:
                    if (this.currEvent.ev != null)
                    {
                        if (current.modifiers == EventModifiers.Control)
                        {
                            if (current.delta.y < 0.0)
                            {
                                BloxEdGUI.Styles.FontSize++;
                            }
                            else if (current.delta.y > 0.0)
                            {
                                BloxEdGUI.Styles.FontSize--;
                            }
                        }
                        else
                        {
                            this.currEvent.ev._ed_viewOffs.y -= (float)(current.delta.y * 15.0);
                        }
                        current.Use();
                    }
                    break;
                case EventType.DragExited:
                    this.ClearDragDropBlock();
                    current.Use();
                    break;
                case EventType.DragPerform:
                    if (this.dragDrop != null)
                    {
                        this.dragOut = DragOut.None;
                        this.dragOutBlockRef = null;
                        DragAndDrop.AcceptDrag();
                        plyEdGUI.ClearFocus();
                        current.Use();
                        Debug.Log("DragPerform add block", "BloxEditorWindow", Color.magenta);
                        this.AddBlock();
                        this.dragDrop = null;
                    }
                    break;
                case EventType.DragUpdated:
                    if (this.currEvent.ev != null && !this.currEvent.Loading)
                    {
                        plyEdTreeItem<BloxBlockDef> plyEdTreeItem = DragAndDrop.GetGenericData("plyEdTreeView:BloxBlockDef") as plyEdTreeItem<BloxBlockDef>;
                        if (plyEdTreeItem != null && plyEdTreeItem.data != null)
                        {
                            if ((!plyEdTreeItem.data.isYieldBlock || this.currEvent.ev.yieldAllowed) && this.canvasRect.Contains(current.mousePosition) && !BloxPropsPanel.Instance.propsRect.Contains(current.mousePosition))
                            {
                                this.dragDrop = plyEdTreeItem.data;
                                this.GC_DragDropLabel.text = this.dragDrop.name;
                                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                                current.Use();
                                break;
                            }
                        }
                        else
                        {
                            plyVariablesEditor.DragDropData dragDropData = DragAndDrop.GetGenericData("plyVariable") as plyVariablesEditor.DragDropData;
                            if (dragDropData != null && this.canvasRect.Contains(current.mousePosition) && !BloxPropsPanel.Instance.propsRect.
                                Contains(current.mousePosition) && BloxEd.Instance.blockDefs.TryGetValue("Common/Variable", out this.dragDrop))
                            {
                                Variable_Block variable_Block = (Variable_Block)this.InstantiateBlock(this.dragDrop);
                                this.dragDropBlock = new BloxBlockEd(variable_Block, null, null, null, -1, true);
                                variable_Block.varName = dragDropData.name;
                                variable_Block.varType = dragDropData.plyVarType;
                                variable_Block.returnType = dragDropData.varValType;
                                this.dragOut = DragOut.DontWorryAboutIt;
                                this.GC_DragDropLabel.text = this.dragDropBlock.def.shortName;
                                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                                current.Use();
                                break;
                            }
                        }
                    }
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    this.dragDrop = null;
                    break;
                case EventType.Repaint:
                    if (this.mustShowContextMenu)
                    {
                        this.mustShowContextMenu = false;
                        plyEdGUI.ClearFocus();
                        this.ShowContextMenu();
                    }
                    else if (this.dragDropInsertContext != null || this.dragDropInsertTop)
                    {
                        if (this.dragDropBlock != null)
                        {
                            this.doRepaint = true;
                        }
                    }
                    else
                    {
                        if (this.dragDropBlock == null || this.dragDrop == null)
                        {
                            if (this.dragDrop == null)
                                break;
                            if (DragAndDrop.visualMode != DragAndDropVisualMode.Move)
                                break;
                        }
                        Rect position = this.CalcStyleRect(this.GC_DragDropLabel, this.dragDrop.style[0], current.mousePosition - new Vector2(12f, 3f));
                        position.width = 180f;
                        this.dragDrop.style[0].Draw(position, this.GC_DragDropLabel, false, false, false, false);
                        this.doRepaint = true;
                    }
                    break;
            }
        }

        private void ShowContextMenu()
        {
            if (this.currEvent.ev != null && !this.currEvent.Loading)
            {
                GenericMenu genericMenu = new GenericMenu();
                if (this.currBlock_change != null)
                {
                    this.currBlock = this.currBlock_change;
                    base.Repaint();
                }
                if (this.currBlock != null)
                {
                    genericMenu.AddItem(new GUIContent("Cut Selected Block"), false, this.OnContextMenu, new object[1]
                    {
                        1
                    });
                    genericMenu.AddItem(new GUIContent("Copy Selected Block"), false, this.OnContextMenu, new object[1]
                    {
                        2
                    });
                    genericMenu.AddItem(new GUIContent("Copy with Next"), false, this.OnContextMenu, new object[1]
                    {
                        5
                    });
                }
                else if (this.currEvent.firstBlock != null)
                {
                    genericMenu.AddItem(new GUIContent("Copy all Event Graph Blocks"), false, this.OnContextMenu, new object[1]
                    {
                        6
                    });
                }
                if (this.copiedBlock != null)
                {
                    genericMenu.AddItem(new GUIContent("Paste"), false, this.OnContextMenu, new object[1]
                    {
                        3
                    });
                }
                if (this.currBlock != null)
                {
                    genericMenu.AddSeparator("");
                    genericMenu.AddItem(new GUIContent("Delete Selected Block"), false, this.OnContextMenu, new object[1]
                    {
                        4
                    });
                }
                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("Toggle Event Active"), false, this.OnContextMenu, new object[1]
                {
                    8
                });
                if (this.currBlock != null)
                {
                    genericMenu.AddItem(new GUIContent("Toggle Block Active"), false, this.OnContextMenu, new object[1]
                    {
                        7
                    });
                }
                genericMenu.ShowAsContext();
            }
        }

        private void OnContextMenu(object arg)
        {
            switch ((int)((object[])arg)[0])
            {
                case 1:
                    this.CutSelectedBlock(false);
                    break;
                case 2:
                    this.StoreSelectedAsCopied();
                    break;
                case 3:
                    this.PasteBlock();
                    break;
                case 4:
                    this.DeleteSelectedBlock(false);
                    break;
                case 5:
                    this.StoreAsCopiedWithLinked(this.currBlock);
                    break;
                case 6:
                    this.StoreAllGraphBlocksAsCopied();
                    break;
                case 7:
                    this.ToggleCurrBlockActive();
                    break;
                case 8:
                    this.ToggleEventActive();
                    break;
            }
        }
        /// <summary>
        /// 积木列表
        /// </summary>
        private void DoBlocksList()
        {
            //Debug.Log("DoBlocksList", "BlockEditorWindows", Color.red);
            if (BloxEdGlobal.BlocksListDocked)
            {
                //Debug.Log("DoBlocksList BloxEdGlobal.BlocksListDocked true", "BlockEditorWindows", Color.red);
                this.panelWidth[1] = Mathf.Clamp(this.panelWidth[1], 190f, (float)(base.position.width / 2.0));
                if (BloxBlocksList.Instance.DoGUI(this, this.panelWidth[1]))
                {
                    //Debug.Log("DoBlocksList BloxEdGlobal.BlocksListDocked true DoGUI(this, this.panelWidth[1]", "BlockEditorWindows", Color.red);
                    BloxBlocksWindow.Show_BloxBlocksWindow();
                    this.doRepaint = true;
                }
                this.splitterRect[1] = GUILayoutUtility.GetLastRect();
                this.splitterRect[1].x -= 5f;
                this.splitterRect[1].width = 5f;
                EditorGUIUtility.AddCursorRect(this.splitterRect[1], MouseCursor.ResizeHorizontal);
            }
        }
        /// <summary>
        /// 事件面板和界面canvas
        /// </summary>
        private void DoEventsList()
        {
            //Debug.Log("DoEventsList", "BlockEditorWindows", Color.green);
            if (BloxEd.Instance.EventDefsLoading || BloxEd.Instance.eventDefs.Count == 0)
            {
                //Debug.Log("DoEventsList BloxEd.Instance.EventDefsLoading " + BloxEd.Instance.EventDefsLoading+ 
                //    "BloxEd.Instance.eventDefs.Count "+ BloxEd.Instance.eventDefs.Count, "BlockEditorWindows", Color.green);
                EditorGUILayout.BeginVertical(GUILayout.Width(this.panelWidth[0]));
                GUILayout.Box(GUIContent.none, plyEdGUI.Styles.Toolbar, GUILayout.ExpandWidth(true));
                EditorGUILayout.Space();
                //plyEdGUI.DrawSpinner(BloxEditorWindow.GC_LoadingEvents, true, true);
                EditorGUILayout.EndVertical();
                this.doRepaint = true;
            }
            else
            {
                if (this.currEvent.ev == null || this.currEvent.def == null)
                {
                    Debug.Log("DoEventsList currEvent.ev||currEvent.def == null", "BlockEditorWindows", Color.green);
                    this.eventIdx = -2;
                    this.currEvent.Clear();
                }
                //Debug.Log("BloxEditor.BloxEditorWindow.DoEventsList.else blox.events Length" + blox.events.Length, "BlockEditorWindows", Color.green);
                this.panelWidth[0] = Mathf.Clamp(this.panelWidth[0], 190f, (float)(base.position.width / 2.0));
                int num = plyEdGUI.List<BloxEvent>(ref this.eventIdx, ref this.blox.events, ref this.scroll[0], this.evListOps, new GUILayoutOption[1]
                {
                    GUILayout.Width(this.panelWidth[0])
                });
                if (num != 0 && num < 3)
                {
                    Debug.Log("num != 0 && num < 3 num is " + num, "BlockEditorWindows", Color.green);
                    this.doRepaint = true;
                    this.currEvent.Set((this.eventIdx < 0) ? null : this.blox.events[this.eventIdx], false);
                    BloxPropsPanel.Instance.SetActiveEventDef(this.currEvent.def);
                    if (this.currEvent.ev != null)
                    {
                        Debug.Log("this.currEvent.ev.screenName " + this.currEvent.ev.screenName, "BlockEditorWindows", Color.green);
                        this.GC_EventWindowLabel.text = this.currEvent.ev.screenName;
                    }
                    if (this.currEvent.ev != null && this.currEvent.ev.yieldAllowed != this.currEvent.def.yieldAllowed)
                    {
                        Debug.Log("SaveBlox " + this.currEvent.ev.screenName, "BlockEditorWindows", Color.green);
                        this.currEvent.ev.yieldAllowed = this.currEvent.def.yieldAllowed;
                        this.SaveBlox(false);
                    }
                }
                this.splitterRect[0] = GUILayoutUtility.GetLastRect();
                this.splitterRect[0].x = this.splitterRect[0].xMax;
                this.splitterRect[0].width = 5f;
                EditorGUIUtility.AddCursorRect(this.splitterRect[0], MouseCursor.ResizeHorizontal);
            }
        }

        private void DrawEventsListHeader()
        {
            EditorGUILayout.Space();
            GUILayout.Label(BloxEditorWindow.GC_Events, plyEdGUI.Styles.GUIToolbarLabel);
        }

        private void DrawEventsListElement(Rect rect, int index, bool selected)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Rect rect2 = new Rect(rect.x, rect.y, 22f, rect.height);
                BloxEdGUI.Styles.EventListElement.Draw(rect2, GUIContent.none, selected, selected, selected, selected);
                if (selected && this.currEvent.Loading)
                {
                    rect2.x += 5f;
                    rect2.width = 16f;
                    rect2.height = 16f;
                    rect2.y = (float)(rect.y + (rect.height / 2.0 - 8.0));
                    plyEdGUI.DrawSpinner(rect2);
                }
                else
                {
                    BloxEventDef bloxEventDef = (index == this.eventIdx) ? this.currEvent.def : BloxEd.Instance.FindEventDef(this.blox.events[index]);
                    if (bloxEventDef != null)
                    {
                        Texture2D texture2D = (bloxEventDef.iconName == null) ? null : BloxEdGUI.Instance.namedIcons[bloxEventDef.iconName];
                        if ((UnityEngine.Object)texture2D != (UnityEngine.Object)null)
                        {
                            rect2.x += 5f;
                            rect2.width = 16f;
                            rect2.height = 16f;
                            rect2.y = (float)(rect.y + (rect.height / 2.0 - 8.0));
                            GUI.DrawTexture(rect2, texture2D);
                        }
                    }
                }
                rect.x += 22f;
                rect.width -= 22f;
                BloxEdGUI.Styles.EventListElement.Draw(rect, this.blox.events[index].screenName, selected, selected, selected, selected);
                if (!this.blox.events[index].active)
                {
                    rect.x = (float)(rect.xMax - 20.0);
                    rect.width = 20f;
                    GUI.Label(rect, BloxEditorWindow.GC_EventDisabled, plyEdGUI.Styles.Label);
                }
            }
        }

        private int EventsListAction(plyEdGUI.ListOps.ListAction act)
        {
            switch (act)
            {
                case plyEdGUI.ListOps.ListAction.DoAdd:
                    PopupWindow.Show(new Rect((float)(Event.current.mousePosition.x + 20.0), Event.current.mousePosition.y, 0f, 0f), this.eventsPopup);
                    break;
                case plyEdGUI.ListOps.ListAction.DoRemoveSelected:
                    if (EditorUtility.DisplayDialog("Blox", "Removing the Event can't be undone. Continue?", "Yes", "Cancel"))
                    {
                        ArrayUtility.RemoveAt<BloxEvent>(ref this.blox.events, this.eventIdx);
                        this.SaveBlox(false);
                    }
                    break;
                case plyEdGUI.ListOps.ListAction.DoDuplicateSelected:
                    this.DuplicateEvent(this.currEvent.ev);
                    break;
                case plyEdGUI.ListOps.ListAction.OnOrderChanged:
                    this.SaveBlox(false);
                    break;
            }
            return -1;
        }

        private void RenameEvent()
        {
            if ((UnityEngine.Object)this.blox != (UnityEngine.Object)null)
            {
                plyTextInputWiz.ShowWiz("Rename Event", "", this.currEvent.ev.screenName, this.OnRenameEvent, null, 250f);
            }
        }

        private void OnRenameEvent(plyTextInputWiz wiz)
        {
            string text = wiz.text;
            wiz.Close();
            if (!string.IsNullOrEmpty(text) && !((UnityEngine.Object)this.blox == (UnityEngine.Object)null) && this.currEvent.ev != null)
            {
                if (this.currEvent.ev.ident == "Custom")
                {
                    if (!plyEdUtil.IsValidVariableName(text))
                    {
                        EditorUtility.DisplayDialog("Rename Error", "A Custom Event's name should start with an alpha character [a-z] and may only contain alphanumeric [0-9,a-z] and the underscore characters.", "OK");
                        return;
                    }
                    if (BloxEd.Instance.eventMethodNames.Contains(text))
                    {
                        EditorUtility.DisplayDialog("Rename Error", "An Custom Event may not be renamed to one of the system defined Event names.", "OK");
                        return;
                    }
                }
                this.currEvent.ev.screenName = text;
                this.GC_EventWindowLabel.text = text;
                this.SaveBlox(false);
                base.Repaint();
            }
        }

        public void AddEvent(BloxEventDef evd)
        {
            if (!((UnityEngine.Object)this.blox == (UnityEngine.Object)null))
            {
                BloxEvent item = new BloxEvent
                {
                    ident = evd.ident,
                    screenName = evd.name,
                    active = true,
                    yieldAllowed = evd.yieldAllowed
                };
                ArrayUtility.Add<BloxEvent>(ref this.blox.events, item);
                this.SaveBlox(false);
                base.Repaint();
            }
        }

        public void DuplicateEvent(BloxEvent ev)
        {
            if (!((UnityEngine.Object)this.blox == (UnityEngine.Object)null) && ev != null)
            {
                ArrayUtility.Add<BloxEvent>(ref this.blox.events, ev.Copy());
                this.SaveBlox(false);
                this.doRepaint = true;
            }
        }
        /// <summary>
        /// blox面板工具栏
        /// </summary>
        private void DoCanvasToolbar()
        {
            //Debug.Log("DoCanvasToolbar", "BlockEditorWindows", Color.green);
            EditorGUILayout.BeginHorizontal(plyEdGUI.Styles.Toolbar);
            EditorGUILayout.Space();
            GUILayout.Label(BloxEditorWindow.GC_Editing, plyEdGUI.Styles.GUIToolbarLabel, GUILayout.Width(50f));
            GUILayout.Label(this.blox.screenName, BloxEdGUI.Styles.ToolbarBloxName);
            GUILayout.FlexibleSpace();
            if (BloxEditorWindow.ToolbarDrawCallbacks != null)
            {
                BloxEditorWindow.ToolbarDrawCallbacks(this.blox);
                EditorGUILayout.Space();
            }
            if (GUILayout.Button(BloxEditorWindow.GC_ResetView, plyEdGUI.Styles.ToolbarButton) && this.currEvent.ev != null)
            {
                this.currEvent.ev._ed_viewOffs = Vector2.zero;
                this.SaveBlox(false);
            }
            if (GUILayout.Button(BloxEditorWindow.GC_Cleanup, plyEdGUI.Styles.ToolbarButton))
            {
                this.RemoveAllUnlinked();
            }
            if (GUILayout.Button(BloxEditorWindow.GC_Settings, plyEdGUI.Styles.ToolbarButton))
            {
                BloxSettingsWindow.Show_BloxSettingsWindow();
            }
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// editor 画布面板
        /// </summary>
        private void DoCanvas()
        {
            //Debug.Log("DoCanvas", "BlockEditorWindows", Color.green);
            EditorGUIUtility.SetIconSize(new Vector2((float)BloxEdGUI.Styles.FontSize, (float)BloxEdGUI.Styles.FontSize));
            this.canvasRect = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            plyEdGUI.DrawFilled(this.canvasRect, BloxEdGlobal.CanvasColour);
            if (this.currEvent.ev != null && this.currEvent.def != null && !this.currEvent.Loading)
            {
                if (this.wasLoadingEvent)
                {
                    this.doRepaint = true;
                    this.wasLoadingEvent = false;
                }
                this.inactiveDepth = ((!this.currEvent.ev.active) ? 1 : 0);
                GUI.color = ((this.inactiveDepth > 0) ? BloxEditorWindow.InactiveColor : Color.white);
                GUI.BeginGroup(this.canvasRect);
                base.BeginWindows();
                this.DrawUnlinkedBlocks();
                this.blockAreaV2 = new Vector2((float)(this.currEvent.ev._ed_viewOffs.x + this.panelWidth[0] + 20.0), (float)(this.currEvent.ev._ed_viewOffs.y + 30.0));
                Vector2 vector = BloxEdGUI.Styles.Event[0].CalcSize(this.GC_EventWindowLabel);
                vector.x += 20f;
                if (vector.x < 200.0)
                {
                    vector.x = 200f;
                }
                Rect screenRect = new Rect((float)(this.currEvent.ev._ed_viewOffs.x + 20.0), (float)(this.currEvent.ev._ed_viewOffs.y + 15.0), vector.x, 25f);
                GUILayout.Window(1, screenRect, this.EventWindow, this.GC_EventWindowLabel, BloxEdGUI.Styles.Event[(!this.currEvent.ev.active) ? 1 : 0]);
                base.EndWindows();
                GUI.EndGroup();
                GUI.color = Color.white;
            }
            //doc 说明 入口
            BloxPropsPanel instance = BloxPropsPanel.Instance;
            Rect position = base.position;
            double x = position.width - (BloxEdGlobal.BlocksListDocked ? (this.panelWidth[1] + 1.0) : 0.0);
            position = base.position;
            instance.DoGUI(this, new Vector2((float)x, position.height));
            plyEdGUI.DrawInnerShadow(this.canvasRect);
        }
        /// <summary>
        /// 整个事件绘画
        /// </summary>
        /// <param name="id"></param>
        private void EventWindow(int id)
        {
            Event current = Event.current;
            if (!this.currEvent.ev.active)
            {
                GUI.color = BloxEditorWindow.InactiveColor;
            }
            if (this.dragDrop != null && (this.dragDrop.blockType == BloxBlockType.Action || this.dragDrop.blockType == BloxBlockType.Container))
            {
                Rect position = this.CalcStyleRect(this.GC_DragDropLabel, this.dragDrop.style[0], new Vector2(5f, 29f));
                position.width = 180f;
                if (this.dragDropInsertTop && this.dragDropInsertContext == null)
                {
                    GUILayoutUtility.GetRect(position.width, position.height);
                    if (current.type == EventType.Repaint)
                    {
                        this.dragDrop.style[0].Draw(position, this.GC_DragDropLabel, false, false, false, false);
                    }
                }
                else
                {
                    position.height = 10f;
                }
                if (current.type == EventType.Repaint && position.Contains(current.mousePosition))
                {
                    this.dragDropInsertTop_change = true;
                }
            }
            if (this.currEvent.firstBlock == null)
            {
                GUILayout.Space(15f);
            }
            else
            {
                for (BloxBlockEd bloxBlockEd = this.currEvent.firstBlock; bloxBlockEd != null; bloxBlockEd = bloxBlockEd.next)
                {
                    this.DrawBlock(current, bloxBlockEd, false);
                }
            }
        }

        private void DrawUnlinkedBlocks()
        {
            if (this.currEvent.unlinkedBlocks.Count != 0)
            {
                int num = 2;
                foreach (BloxBlockEd unlinkedBlock in this.currEvent.unlinkedBlocks)
                {
                    this.blockAreaV2 = new Vector2(unlinkedBlock.b._ed_viewOffs.x + this.currEvent.ev._ed_viewOffs.x + this.canvasRect.x, unlinkedBlock.b._ed_viewOffs.y + this.currEvent.ev._ed_viewOffs.y + this.canvasRect.y);
                    Vector2 vector = BloxEdGUI.Styles.Event[0].CalcSize(this.GC_EventWindowLabel);
                    vector.x += 20f;
                    if (vector.x < 200.0)
                    {
                        vector.x = 200f;
                    }
                    Rect screenRect = new Rect(unlinkedBlock.b._ed_viewOffs.x + this.currEvent.ev._ed_viewOffs.x, unlinkedBlock.b._ed_viewOffs.y + this.currEvent.ev._ed_viewOffs.y, vector.x, 25f);
                    GUILayout.Window(num++, screenRect, this.BlockWindow, GUIContent.none, BloxEdGUI.Styles.CanvasBlock);
                }
            }
        }

        private void BlockWindow(int id)
        {
            Event current = Event.current;
            BloxBlockEd bloxBlockEd = this.currEvent.unlinkedBlocks[id - 2];
            this.DrawBlock(current, bloxBlockEd, false);
            for (BloxBlockEd next = bloxBlockEd.next; next != null; next = next.next)
            {
                this.DrawBlock(current, next, false);
            }
        }

        private Rect DrawBlock(Event ev, BloxBlockEd bdi, bool asField)
        {
            if (this.inactiveDepth > 0)
            {
                this.inactiveDepth++;
            }
            if (!bdi.b.active)
            {
                this.inactiveDepth++;
            }
            if (this.inactiveDepth > 0)
            {
                GUI.color = BloxEditorWindow.InactiveColor;
            }
            bool flag = this.dragDrop != null && !asField && (this.dragDrop.blockType == BloxBlockType.Action || this.dragDrop.blockType == BloxBlockType.Container);
            if (asField || (this.dragDropInsertContext != null && this.dragDropInsertContext != bdi) || this.dragDropInsertFieldIdx >= 0)
            {
                flag = false;
            }
            Rect rect;
            if (bdi.def == null || bdi.b == null || bdi.def.blockType == BloxBlockType.Unknown || bdi.b.blockType == BloxBlockType.Unknown)
            {
                flag = false;
                rect = this.DrawErrorBlock(ev, bdi);
            }
            else if (bdi.b.blockType == BloxBlockType.Value | asField)
            {
                flag = false;
                rect = this.DrawValueBlock(ev, bdi);
            }
            else if (bdi.b.blockType == BloxBlockType.Action)
            {
                rect = this.DrawActionBlock(ev, bdi);
            }
            else if (bdi.b.blockType == BloxBlockType.Container)
            {
                rect = this.DrawContainerBlock(ev, bdi);
            }
            else
            {
                flag = false;
                rect = this.DrawErrorBlock(ev, bdi);
            }
            #region 画布内  积木操作
            if (ev.type == EventType.MouseDown && !this.IsDragging() && !flag && (ev.button == 0 || ev.button == 1) && rect.Contains(ev.mousePosition) && !BloxPropsPanel.Instance.propsRect.Contains(ev.mousePosition + this.blockAreaV2))
            {
                plyEdGUI.ClearFocus();
                this.currBlock_change = bdi;
                this.mustShowContextMenu = (ev.button == 1 || (ev.button == 0 && ev.modifiers == EventModifiers.Control));
                ev.Use();
            }
            if (ev.button == 0 && ev.type == EventType.MouseDrag && !this.IsDragging() && rect.Contains(ev.mousePosition) && !BloxPropsPanel.Instance.propsRect.Contains(ev.mousePosition + this.blockAreaV2))
            {
                plyEdGUI.ClearFocus();
                this.blockDragStartDetected = true;
                this.blockDragTimer.Reset();
                this.blockDragTimer.Start();
            }
            #endregion
            if (bdi.b.blockType == BloxBlockType.Container)
            {
                Rect rect2 = EditorGUILayout.BeginVertical(bdi.def.style[2]);
                if (flag)
                {
                    rect.x += 5f;
                    this.CheckDragDropInsert(ev, rect, bdi, false);
                }
                if (bdi.firstChild != null)
                {
                    for (BloxBlockEd bloxBlockEd = bdi.firstChild; bloxBlockEd != null; bloxBlockEd = bloxBlockEd.next)
                    {
                        this.DrawBlock(ev, bloxBlockEd, false);
                    }
                }
                else
                {
                    GUILayout.Space(15f);
                }
                EditorGUILayout.EndVertical();
                if (this.currBlock == bdi && ev.type == EventType.Repaint)
                {
                    GUI.color = Color.white;
                    BloxEdGUI.Styles.Select[3].Draw(rect2, false, false, false, false);
                }
                if (flag)
                {
                    this.CheckDragDropInsert(ev, rect2, bdi, true);
                }
            }
            else if (flag)
            {
                this.CheckDragDropInsert(ev, rect, bdi, true);
            }
            if (this.inactiveDepth > 0)
            {
                this.inactiveDepth--;
                if (!bdi.b.active && this.inactiveDepth > 0)
                {
                    this.inactiveDepth--;
                }
                if (this.inactiveDepth == 0)
                {
                    GUI.color = Color.white;
                }
            }
            return rect;
        }

        private Rect DrawValueBlock(Event ev, BloxBlockEd bdi)
        {
            Rect rect = new Rect(0,0,0,0);
            if (bdi.def.overrideRenderFields == 1 || (bdi.def.overrideRenderFields == 0 && bdi.def.paramDefs.Length == 0) || (bdi.def.mi != null && (bdi.def.mi.MemberType == MemberTypes.Field || bdi.def.mi.MemberType == MemberTypes.Property)))
            {
                rect = EditorGUILayout.BeginHorizontal(BloxEdGUI.Styles.Value[0]);
                if (bdi.def.drawer == null)
                {
                    if (bdi.def.contextType != null)
                    {
                        this.DrawBlockContext(ev, bdi);
                    }
                    else
                    {
                        string text = null;
                        if (bdi.def.mi != null)
                        {
                            text = bdi.def.shortName;
                        }
                        else if (bdi.b.returnValue == null)
                        {
                            text = "null (" + bdi.def.returnName + ")";
                        }
                        else if (bdi.b.returnType.IsArray || bdi.b.returnType.IsGenericType)
                        {
                            text = bdi.def.returnName;
                        }
                        else
                        {
                            string text2 = bdi.b.returnValue.ToString();
                            if (text2 != null)
                            {
                                if (bdi.b.returnType == typeof(string))
                                {
                                    text2 = text2.Trim();
                                    text = ((text2.Length > 20) ? (text2.Substring(0, 20) + "...") : text2);
                                    text = text.Replace("\n", "");
                                    if (text.Length == 0)
                                    {
                                        text = "-empty-";
                                    }
                                }
                                else
                                {
                                    text = text2;
                                    if (typeof(UnityEngine.Object).IsAssignableFrom(bdi.b.returnValue.GetType()) && text.Contains("("))
                                    {
                                        text = text.Substring(0, text.IndexOf("("));
                                    }
                                }
                            }
                            else
                            {
                                text = "-empty-";
                            }
                        }
                        this.GC_BlockLabel.image = (bdi.def.showIconInCanvas ? bdi.def.icon : null);
                        this.GC_BlockLabel.text = text;
                        GUILayout.Label(this.GC_BlockLabel, BloxEdGUI.Styles.ValueLabel);
                    }
                }
                else
                {
                    bdi.def.drawer.DrawHead(this, bdi);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                rect = EditorGUILayout.BeginHorizontal(BloxEdGUI.Styles.Horizontal);
                if (bdi.def.overrideRenderFields != 4)
                {
                    EditorGUILayout.BeginHorizontal(BloxEdGUI.Styles.Value[3]);
                    if (bdi.def.drawer == null)
                    {
                        if (bdi.def.contextType != null)
                        {
                            this.DrawBlockContext(ev, bdi);
                        }
                        else
                        {
                            this.GC_BlockLabel.image = (bdi.def.showIconInCanvas ? bdi.def.icon : null);
                            this.GC_BlockLabel.text = bdi.def.shortName;
                            GUILayout.Label(this.GC_BlockLabel, BloxEdGUI.Styles.ActionLabel);
                        }
                    }
                    else
                    {
                        bdi.def.drawer.DrawHead(this, bdi);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal(BloxEdGUI.Styles.Value[(bdi.def.overrideRenderFields == 4) ? 1 : 4]);
                if (bdi.def.drawer == null)
                {
                    for (int i = 0; i < bdi.paramBlocks.Length; i++)
                    {
                        this.DrawBlockField(ev, bdi, i);
                    }
                }
                else
                {
                    bdi.def.drawer.DrawFields(this, bdi);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();
            }
            if (this.currBlock == bdi && ev.type == EventType.Repaint)
            {
                GUI.color = Color.white;
                BloxEdGUI.Styles.Select[0].Draw(rect, false, false, false, false);
            }
            return rect;
        }

        private Rect DrawActionBlock(Event ev, BloxBlockEd bdi)
        {
            Rect rect;
            if (bdi.def.paramDefs.Length == 0 && bdi.def.overrideRenderFields != 1 && bdi.def.overrideRenderFields != 3)
            {
                rect = EditorGUILayout.BeginHorizontal(bdi.def.style[0]);
                if (bdi.def.drawer == null)
                {
                    if (bdi.def.contextType != null)
                    {
                        this.DrawBlockContext(ev, bdi);
                    }
                    else
                    {
                        this.GC_BlockLabel.image = (bdi.def.showIconInCanvas ? bdi.def.icon : null);
                        this.GC_BlockLabel.text = bdi.def.shortName;
                        GUILayout.Label(this.GC_BlockLabel, BloxEdGUI.Styles.ActionLabel);
                    }
                }
                else
                {
                    bdi.def.drawer.DrawHead(this, bdi);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                rect = EditorGUILayout.BeginHorizontal(BloxEdGUI.Styles.Horizontal);
                EditorGUILayout.BeginHorizontal(bdi.def.style[1]);
                if (bdi.def.drawer == null)
                {
                    if (bdi.def.contextType != null)
                    {
                        this.DrawBlockContext(ev, bdi);
                    }
                    else
                    {
                        this.GC_BlockLabel.image = (bdi.def.showIconInCanvas ? bdi.def.icon : null);
                        this.GC_BlockLabel.text = bdi.def.shortName;
                        GUILayout.Label(this.GC_BlockLabel, BloxEdGUI.Styles.ActionLabel);
                    }
                }
                else
                {
                    bdi.def.drawer.DrawHead(this, bdi);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(bdi.def.style[2]);
                if (bdi.def.drawer == null)
                {
                    for (int i = 0; i < bdi.paramBlocks.Length; i++)
                    {
                        this.DrawBlockField(ev, bdi, i);
                    }
                }
                else
                {
                    bdi.def.drawer.DrawFields(this, bdi);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();
            }
            if (ev.type == EventType.Repaint && this.currBlock == bdi)
            {
                GUI.color = Color.white;
                BloxEdGUI.Styles.Select[1].Draw(rect, false, false, false, false);
            }
            return rect;
        }

        private Rect DrawContainerBlock(Event ev, BloxBlockEd bdi)
        {
            Rect rect = EditorGUILayout.BeginHorizontal(bdi.def.style[1]);
            if (bdi.def.drawer == null)
            {
                this.GC_BlockLabel.image = null;
                this.GC_BlockLabel.text = bdi.def.shortName;
                GUILayout.Label(this.GC_BlockLabel, BloxEdGUI.Styles.ActionLabel);
                if (bdi.def.paramDefs.Length != 0 && !bdi.def.paramDefs[0].showName)
                {
                    GUILayout.Space(10f);
                }
                for (int i = 0; i < bdi.paramBlocks.Length; i++)
                {
                    this.DrawBlockField(ev, bdi, i);
                }
            }
            else
            {
                bdi.def.drawer.DrawHead(this, bdi);
                bdi.def.drawer.DrawFields(this, bdi);
            }
            EditorGUILayout.EndHorizontal();
            if (this.currBlock == bdi && ev.type == EventType.Repaint)
            {
                GUI.color = Color.white;
                BloxEdGUI.Styles.Select[2].Draw(rect, false, false, false, false);
            }
            return rect;
        }

        private Rect DrawErrorBlock(Event ev, BloxBlockEd bdi)
        {
            if (bdi.b == null)
            {
                this.GC_BlockLabel.text = "error";
                this.currEvent.hasUndefinedblocks = true;
            }
            else if (bdi.def != null && bdi.def.blockType == BloxBlockType.Unknown)
            {
                this.currEvent.hasUndefinedblocks = true;
                this.GC_BlockLabel.text = "undefined";
            }
            else if (BloxEd.Instance.BlockDefsLoading && bdi.def == null)
            {
                if (bdi._changeDefTo == null)
                {
                    bdi._changeDefTo = BloxEd.Instance.FindBlockDef(bdi.b);
                }
                this.GC_BlockLabel.text = "loading...";
                this.doRepaint = true;
            }
            else if (bdi.def == null)
            {
                if (bdi._changeDefTo == null)
                {
                    bdi._changeDefTo = BloxEd.Instance.FindBlockDef(bdi.b);
                }
                this.GC_BlockLabel.text = "loading...";
                this.doRepaint = true;
            }
            else
            {
                this.GC_BlockLabel.text = "error";
                this.currEvent.hasUndefinedblocks = true;
            }
            this.GC_BlockLabel.image = null;
            Rect result = EditorGUILayout.BeginHorizontal(BloxEdGUI.Styles.Error);
            GUILayout.Label(this.GC_BlockLabel, BloxEdGUI.Styles.ActionLabel);
            EditorGUILayout.EndHorizontal();
            if (ev.type == EventType.Repaint && bdi._changeDefTo != null && bdi.def == null)
            {
                bdi.def = bdi._changeDefTo;
                bdi._changeDefTo = null;
            }
            return result;
        }

        private void CheckDragDropInsert(Event ev, Rect r, BloxBlockEd b, bool toTail)
        {
            if (this.dragDrop != null)
            {
                GUIStyle gUIStyle = this.dragDrop.style[0];
                r = this.CalcStyleRect(this.GC_DragDropLabel, gUIStyle, new Vector2(r.x, (float)(r.yMax - 3.0)));
                r.width = 180f;
                if (this.dragDropInsertContext == b && toTail != this.dragDropInsertTop)
                {
                    GUILayoutUtility.GetRect(r.width, r.height);
                    if (ev.type == EventType.Repaint)
                    {
                        gUIStyle.Draw(r, this.GC_DragDropLabel, false, false, false, false);
                    }
                }
                else
                {
                    r.height = 10f;
                }
                if (ev.type == EventType.Repaint && this.dragDropInsertContext_change == null && !this.dragDropInsertTop_change && this.dragDropInsertFieldIdx_change < 0 && r.Contains(ev.mousePosition) && !BloxPropsPanel.Instance.propsRect.Contains(ev.mousePosition + this.blockAreaV2))
                {
                    this.dragDropInsertContext_change = b;
                    if (!toTail)
                    {
                        this.dragDropInsertTop_change = true;
                    }
                }
            }
        }

        public void DrawBlockContext(Event ev, BloxBlockEd bdi)
        {
            ev = (ev ?? Event.current);
            Rect position;
            if (bdi.contextBlock == null)
            {
                string str = (bdi.def.contextType == typeof(GameObject) || typeof(Component).IsAssignableFrom(bdi.def.contextType)) ? "self: " : "null: ";
                str += bdi.def.contextName;
                this.GC_BlockLabel.image = (bdi.def.showIconInCanvas ? bdi.def.icon : null);
                this.GC_BlockLabel.text = str;
                position = GUILayoutUtility.GetRect(this.GC_BlockLabel, BloxEdGUI.Styles.Value[0]);
                GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                GUI.Label(position, this.GC_BlockLabel, BloxEdGUI.Styles.Value[0]);
                GUI.contentColor = Color.white;
            }
            else
            {
                position = this.DrawBlock(ev, bdi.contextBlock, true);
            }
            this.GC_BlockLabel.text = bdi.def.shortName;
            if (bdi.def.mi != null && bdi.owningBlock == null && bdi.def.mi.CanSetValue && bdi.def.mi.MemberType != MemberTypes.Method && bdi.def.mi.MemberType != MemberTypes.Constructor)
            {
                GUIContent gC_BlockLabel = this.GC_BlockLabel;
                gC_BlockLabel.text += " =";
            }
            GUILayout.Label(this.GC_BlockLabel.text, BloxEdGUI.Styles.ActionLabel);
            if (this.dragDrop != null)
            {
                GUIStyle gUIStyle = null;
                if (this.CanAllowInDragDrop(this.dragDrop, this.dragDropBlock))
                {
                    gUIStyle = BloxEdGUI.Styles.Value[0];
                }
                else
                {
                    Type type = (this.dragDropBlock != null) ? this.dragDropBlock.b.returnType : this.dragDrop.returnType;
                    if (type != null && type != typeof(void) && this.CanAllowInDragDrop(type, bdi.def.contextType))
                    {
                        gUIStyle = BloxEdGUI.Styles.Value[0];
                    }
                }
                if (gUIStyle != null)
                {
                    if (this.dragDropInsertContext == bdi && this.dragDropInsertFieldIdx == 9999 && this.dragDropExtraWidth > 0.0)
                    {
                        Rect rect = GUILayoutUtility.GetRect(this.dragDropExtraWidth, position.height, GUILayout.ExpandWidth(false));
                        position.width += rect.width;
                    }
                    if (ev.type == EventType.Repaint)
                    {
                        if (this.dragDropInsertContext_change != null && this.dragDropInsertFieldIdx_change >= 0)
                            return;
                        if (position.Contains(ev.mousePosition) && !BloxPropsPanel.Instance.propsRect.Contains(ev.mousePosition))
                        {
                            this.dragDropExtraWidth_change = (float)((this.dragDropExtraWidth > 0.0) ? this.dragDropExtraWidth : (120.0 - position.width));
                            this.dragDropInsertContext_change = bdi;
                            this.dragDropInsertFieldIdx_change = 9999;
                            gUIStyle.Draw(position, this.GC_DragDropLabel, false, false, false, false);
                        }
                    }
                }
            }
        }

        public void DrawBlockField(Event ev, BloxBlockEd bdi, int idx)
        {
            ev = (ev ?? Event.current);
            BloxBlockDef.Param param = (idx < bdi.def.paramDefs.Length) ? bdi.def.paramDefs[idx] : BloxEditorWindow.EmptyBloxBlockDefParam;
            if (param.showName)
            {
                GUILayout.Label(param.name, BloxEdGUI.Styles.FieldLabel);
            }
            else
            {
                GUILayout.Space(2f);
            }
            Rect position;
            if (bdi.paramBlocks[idx] == null)
            {
                string text = param.emptyVal;
                if (text == null)
                {
                    if (bdi.DefaultParamVals[idx] != null)
                    {
                        text = bdi.DefaultParamVals[idx].ToString();
                    }
                    else
                    {
                        Type type = null;
                        Type[] array = bdi.b.ParamTypes();
                        if (array != null && array.Length != 0 && idx < array.Length)
                        {
                            type = array[idx];
                        }
                        else if (bdi.def.paramDefs.Length != 0)
                        {
                            type = bdi.def.paramDefs[idx].type;
                        }
                        text = ((type == null || (type != typeof(GameObject) && !typeof(Component).IsAssignableFrom(type))) ? "null" : ("self: " + type.Name));
                    }
                }
                this.GC_BlockLabel.image = null;
                this.GC_BlockLabel.text = text;
                position = GUILayoutUtility.GetRect(this.GC_BlockLabel, BloxEdGUI.Styles.Value[0]);
                GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                GUI.Label(position, this.GC_BlockLabel, BloxEdGUI.Styles.Value[0]);
                GUI.contentColor = Color.white;
            }
            else
            {
                position = this.DrawBlock(ev, bdi.paramBlocks[idx], true);
            }
            if (this.dragDrop != null)
            {
                GUIStyle gUIStyle = null;
                if (this.CanAllowInDragDrop(this.dragDrop, this.dragDropBlock))
                {
                    gUIStyle = BloxEdGUI.Styles.Value[0];
                }
                else
                {
                    Type type2 = (this.dragDropBlock != null) ? this.dragDropBlock.b.returnType : this.dragDrop.returnType;
                    if (type2 != null && type2 != typeof(void))
                    {
                        Type[] array2 = bdi.b.ParamTypes();
                        if (array2 != null && array2.Length != 0 && idx < array2.Length)
                        {
                            if (this.CanAllowInDragDrop(type2, array2[idx]))
                            {
                                gUIStyle = BloxEdGUI.Styles.Value[0];
                            }
                        }
                        else if (bdi.def.paramDefs.Length != 0 && idx < bdi.def.paramDefs.Length && !bdi.def.paramDefs[idx].isRefType && this.CanAllowInDragDrop(type2, bdi.def.paramDefs[idx].type))
                        {
                            gUIStyle = BloxEdGUI.Styles.Value[0];
                        }
                    }
                }
                if (gUIStyle != null)
                {
                    if (this.dragDropInsertContext == bdi && this.dragDropInsertFieldIdx == idx && this.dragDropExtraWidth > 0.0)
                    {
                        Rect rect = GUILayoutUtility.GetRect(this.dragDropExtraWidth, position.height, GUILayout.ExpandWidth(false));
                        position.width += rect.width;
                    }
                    if (ev.type == EventType.Repaint)
                    {
                        if (this.dragDropInsertContext_change != null && this.dragDropInsertFieldIdx_change >= 0)
                            return;
                        if (position.Contains(ev.mousePosition) && !BloxPropsPanel.Instance.propsRect.Contains(ev.mousePosition))
                        {
                            this.dragDropExtraWidth_change = (float)((this.dragDropExtraWidth > 0.0) ? this.dragDropExtraWidth : (120.0 - position.width));
                            this.dragDropInsertContext_change = bdi;
                            this.dragDropInsertFieldIdx_change = idx;
                            gUIStyle.Draw(position, this.GC_DragDropLabel, false, false, false, false);
                        }
                    }
                }
            }
        }

        private void AddBlock()
        {
            if (this.currEvent.ev != null)
            {
                if (this.dragDropBlock != null)
                {
                    this.AddCopiedBlock();
                }
                else
                {
                    if (this.dragDrop == null)
                        return;
                    this.AddNewBlock();
                }
                this.ClearDragDropBlock();
                this.SaveBlox(true);
            }
        }

        private void AddNewBlock()
        {
            Debug.Log("AddNewBlock", "BloxEditorWindow", Color.magenta);
            BloxBlock bloxBlock = this.InstantiateBlock(this.dragDrop);
            BloxBlockEd bloxBlockEd = null;
            if (bloxBlock == null)
            {
                UnityEngine.Debug.LogError("Could not instantiate Block from: " + this.dragDrop.ident);
            }
            else
            {
                Undo.RegisterCompleteObjectUndo(this.blox, "Add Block");
                if (this.dragDropInsertTop)
                {
                    if (this.dragDropInsertContext == null)
                    {
                        BloxBlockEd firstBlock = this.currEvent.firstBlock;
                        bloxBlockEd = (this.currEvent.firstBlock = new BloxBlockEd(bloxBlock, null, null, null, -1, true));
                        this.currEvent.firstBlock.next = firstBlock;
                        this.currEvent.ev.firstBlock = bloxBlock;
                        if (firstBlock != null)
                        {
                            firstBlock.prev = this.currEvent.firstBlock;
                            this.currEvent.ev.firstBlock.next = firstBlock.b;
                        }
                    }
                    else
                    {
                        BloxBlockEd firstChild = this.dragDropInsertContext.firstChild;
                        bloxBlockEd = (this.dragDropInsertContext.firstChild = new BloxBlockEd(bloxBlock, null, (firstChild != null) ? firstChild.parentBlock : null, null, -1, true));
                        this.dragDropInsertContext.firstChild.next = firstChild;
                        this.dragDropInsertContext.b.firstChild = bloxBlock;
                        if (firstChild != null)
                        {
                            firstChild.prev = this.dragDropInsertContext.firstChild;
                            this.dragDropInsertContext.b.firstChild.next = firstChild.b;
                        }
                    }
                }
                else if (this.dragDropInsertContext != null)
                {
                    if (this.dragDropInsertFieldIdx >= 0)
                    {
                        if (this.dragDropInsertFieldIdx == 9999)
                        {
                            bloxBlockEd = (this.dragDropInsertContext.contextBlock = new BloxBlockEd(bloxBlock, null, null, this.dragDropInsertContext, -1, true));
                            this.dragDropInsertContext.b.contextBlock = bloxBlock;
                        }
                        else
                        {
                            bloxBlockEd = (this.dragDropInsertContext.paramBlocks[this.dragDropInsertFieldIdx] = new BloxBlockEd(bloxBlock, null, null, this.dragDropInsertContext, this.dragDropInsertFieldIdx, true));
                            this.dragDropInsertContext.b.paramBlocks[this.dragDropInsertFieldIdx] = bloxBlock;
                        }
                    }
                    else
                    {
                        BloxBlockEd next = this.dragDropInsertContext.next;
                        bloxBlockEd = (this.dragDropInsertContext.next = new BloxBlockEd(bloxBlock, this.dragDropInsertContext, (next != null) ? next.parentBlock : null, null, -1, true));
                        this.dragDropInsertContext.next.next = next;
                        this.dragDropInsertContext.b.next = bloxBlock;
                        if (next != null)
                        {
                            next.prev = this.dragDropInsertContext.next;
                            this.dragDropInsertContext.b.next.next = next.b;
                        }
                    }
                }
                else
                {
                    bloxBlock._ed_viewOffs = Event.current.mousePosition - new Vector2(12f, 3f) - (this.currEvent.ev._ed_viewOffs + new Vector2(this.canvasRect.x, this.canvasRect.y));
                    this.currEvent.ev.unlinkedBlocks.Add(bloxBlock);
                    this.currEvent.unlinkedBlocks.Add(bloxBlockEd = new BloxBlockEd(bloxBlock, null, null, null, -1, true));
                }
                if (!this.RestoreOrRemoveParamBlock(bloxBlockEd))
                {
                    this.AddDefaultFieldBlocks(bloxBlockEd);
                }
                this.currBlock_change = bloxBlockEd;
            }
        }

        private void AddCopiedBlock()
        {
            BloxBlockEd bloxBlockEd = this.CopyWithLinkedBlocks(this.dragDropBlock);
            Undo.RegisterCompleteObjectUndo(this.blox, "Add Block");
            if (this.dragDropInsertTop)
            {
                if (this.dragDropInsertContext == null)
                {
                    BloxBlockEd firstBlock = this.currEvent.firstBlock;
                    this.currEvent.firstBlock = bloxBlockEd;
                    this.currEvent.ev.firstBlock = bloxBlockEd.b;
                    BloxBlockEd bloxBlockEd2 = bloxBlockEd.LastInChain();
                    bloxBlockEd2.next = firstBlock;
                    if (firstBlock != null)
                    {
                        firstBlock.prev = bloxBlockEd2;
                        bloxBlockEd2.b.next = firstBlock.b;
                    }
                }
                else
                {
                    BloxBlockEd firstChild = this.dragDropInsertContext.firstChild;
                    this.dragDropInsertContext.firstChild = bloxBlockEd;
                    this.dragDropInsertContext.b.firstChild = bloxBlockEd.b;
                    bloxBlockEd.parentBlock = this.dragDropInsertContext;
                    BloxBlockEd bloxBlockEd3 = bloxBlockEd.LastInChain();
                    bloxBlockEd3.next = firstChild;
                    if (firstChild != null)
                    {
                        firstChild.prev = bloxBlockEd3;
                        bloxBlockEd3.b.next = firstChild.b;
                    }
                }
            }
            else if (this.dragDropInsertContext != null)
            {
                if (this.dragDropInsertFieldIdx >= 0)
                {
                    if (this.dragDropInsertFieldIdx == 9999)
                    {
                        this.dragDropInsertContext.contextBlock = bloxBlockEd;
                        this.dragDropInsertContext.b.contextBlock = bloxBlockEd.b;
                        bloxBlockEd.owningBlock = this.dragDropInsertContext;
                        bloxBlockEd.fieldIdx = -1;
                    }
                    else
                    {
                        this.dragDropInsertContext.paramBlocks[this.dragDropInsertFieldIdx] = bloxBlockEd;
                        this.dragDropInsertContext.b.paramBlocks[this.dragDropInsertFieldIdx] = bloxBlockEd.b;
                        bloxBlockEd.owningBlock = this.dragDropInsertContext;
                        bloxBlockEd.fieldIdx = this.dragDropInsertFieldIdx;
                    }
                }
                else
                {
                    BloxBlockEd next = this.dragDropInsertContext.next;
                    this.dragDropInsertContext.next = bloxBlockEd;
                    this.dragDropInsertContext.b.next = bloxBlockEd.b;
                    bloxBlockEd.prev = this.dragDropInsertContext;
                    bloxBlockEd.parentBlock = this.dragDropInsertContext.parentBlock;
                    BloxBlockEd bloxBlockEd4 = bloxBlockEd.LastInChain();
                    bloxBlockEd4.next = next;
                    if (next != null)
                    {
                        next.prev = bloxBlockEd4;
                        bloxBlockEd4.b.next = next.b;
                    }
                }
            }
            else
            {
                bloxBlockEd.b._ed_viewOffs = Event.current.mousePosition - new Vector2(12f, 3f) - (this.currEvent.ev._ed_viewOffs + new Vector2(this.canvasRect.x, this.canvasRect.y));
                this.currEvent.ev.unlinkedBlocks.Add(bloxBlockEd.b);
                this.currEvent.unlinkedBlocks.Add(bloxBlockEd);
            }
            this.RestoreOrRemoveParamBlock(bloxBlockEd);
            this.currBlock_change = bloxBlockEd;
        }

        private bool RestoreOrRemoveParamBlock(BloxBlockEd bdi)
        {
            if (bdi.b.mi != null && bdi.b.mi.MemberType != MemberTypes.Constructor && bdi.b.mi.MemberType != MemberTypes.Method)
            {
                if (bdi.owningBlock == null)
                {
                    if (bdi.b.mi != null && bdi.b.mi.CanSetValue && (bdi.b.paramBlocks == null || bdi.b.paramBlocks.Length == 0))
                    {
                        bdi.b.paramBlocks = new BloxBlock[1];
                        bdi.paramBlocks = new BloxBlockEd[1];
                        this.AddDefaultFieldBlocks(bdi);
                        return true;
                    }
                    return false;
                }
                bdi.b.paramBlocks = null;
                bdi.paramBlocks = new BloxBlockEd[0];
                return true;
            }
            return false;
        }

        private void AddDefaultFieldBlocks(BloxBlockEd bdi)
        {
            if (bdi.def.blockType != BloxBlockType.Value && bdi.def.blockType != BloxBlockType.Container && (bdi.owningBlock == null || bdi.def.mi == null || !bdi.def.mi.IsFieldOrProperty))
            {
                Type[] array = bdi.ParameterTypes();
                if (((array != null) ? array.Length : 0) != 0)
                {
                    if (array.Length != bdi.paramBlocks.Length)
                    {
                        UnityEngine.Debug.LogErrorFormat("Found types array [{0}] is not same length as the param array [{1}]", array.Length, bdi.paramBlocks.Length);
                    }
                    else
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            Type type = array[i];
                            if (type.IsByRef)
                            {
                                BloxBlockDef bloxBlockDef = BloxEd.Instance.FindBlockDef("Common/Variable");
                                if (bloxBlockDef != null)
                                {
                                    Variable_Block variable_Block = this.InstantiateBlock(bloxBlockDef) as Variable_Block;
                                    variable_Block.varName = "-var-name-";
                                    variable_Block.varType = plyVariablesType.Event;
                                    bdi.paramBlocks[i] = new BloxBlockEd(variable_Block, null, null, bdi, i, true);
                                    bdi.b.paramBlocks[i] = variable_Block;
                                }
                            }
                            else
                            {
                                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    type = type.GetGenericArguments()[0];
                                }
                                if (type.IsEnum || BloxPropsPanel.SupportedEditTypes.Contains(type))
                                {
                                    BloxBlockDef bloxBlockDef2 = BloxEd.Instance.FindValueBlockDef(type);
                                    if (bloxBlockDef2 != null)
                                    {
                                        BloxBlock bloxBlock = this.InstantiateBlock(bloxBlockDef2);
                                        bdi.paramBlocks[i] = new BloxBlockEd(bloxBlock, null, null, bdi, i, true);
                                        bdi.b.paramBlocks[i] = bloxBlock;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private BloxBlock InstantiateBlock(BloxBlockDef def)
        {
            BloxBlock bloxBlock = (BloxBlock)Activator.CreateInstance(def.blockSystemType);
            bloxBlock.ident = def.ident;
            bloxBlock.blockType = def.blockType;
            bloxBlock.active = true;
            bloxBlock._ed_viewOffs = Vector2.zero;
            bloxBlock.returnType = def.returnType;
            bloxBlock.mi = def.mi;
            if (bloxBlock.returnType != null)
            {
                bloxBlock.returnValue = BloxMemberInfo.GetDefaultValue(bloxBlock.returnType);
            }
            if (def.paramDefs.Length != 0)
            {
                bloxBlock.paramBlocks = new BloxBlock[def.paramDefs.Length];
            }
            if (def.singleton != null)
            {
                bloxBlock.contextBlock = this.InstantiateBlock(def.singleton);
            }
            return bloxBlock;
        }

        private void DeleteSelectedBlock(bool isDragOut)
        {
            this.dragOut = DragOut.None;
            this.dragOutBlockRef = null;
            if (this.currBlock != null && this.currEvent.ev != null)
            {
                Undo.RegisterCompleteObjectUndo(this.blox, "Remove Block");
                if (this.currBlock.owningBlock != null)
                {
                    if (this.currBlock.fieldIdx >= 0)
                    {
                        if (this.currBlock.owningBlock.paramBlocks != null && this.currBlock.fieldIdx < this.currBlock.owningBlock.paramBlocks.Length)
                        {
                            if (isDragOut)
                            {
                                this.dragOut = (DragOut)(7 + this.currBlock.fieldIdx);
                                this.dragOutBlockRef = this.currBlock.owningBlock;
                            }
                            this.currBlock.owningBlock.b.paramBlocks[this.currBlock.fieldIdx] = null;
                            this.currBlock.owningBlock.paramBlocks[this.currBlock.fieldIdx] = null;
                        }
                        else
                        {
                            object[] obj = new object[2]
                            {
                                this.currBlock.fieldIdx,
                                null
                            };
                            BloxBlockEd[] paramBlocks = this.currBlock.owningBlock.paramBlocks;
                            obj[1] = ((paramBlocks != null) ? new int?(paramBlocks.Length) : null);
                            UnityEngine.Debug.LogErrorFormat("Trying to delete field at {0} while Block only has {1} fields.", obj);
                        }
                    }
                    else
                    {
                        if (isDragOut)
                        {
                            this.dragOut = DragOut.WasContext;
                            this.dragOutBlockRef = this.currBlock.owningBlock;
                        }
                        this.currBlock.owningBlock.b.contextBlock = null;
                        this.currBlock.owningBlock.contextBlock = null;
                    }
                }
                else
                {
                    BloxBlockEd next = this.currBlock.next;
                    BloxBlockEd prev = this.currBlock.prev;
                    if (this.currEvent.firstBlock == this.currBlock)
                    {
                        if (isDragOut)
                        {
                            this.dragOut = DragOut.Event1st;
                        }
                        this.currEvent.ev.firstBlock = ((next != null) ? next.b : null);
                        this.currEvent.firstBlock = next;
                    }
                    else if (this.currBlock.parentBlock != null && this.currBlock.parentBlock.firstChild == this.currBlock)
                    {
                        if (isDragOut)
                        {
                            this.dragOut = DragOut.ParentBlock1st;
                            this.dragOutBlockRef = this.currBlock.parentBlock;
                        }
                        this.currBlock.parentBlock.b.firstChild = ((next != null) ? next.b : null);
                        this.currBlock.parentBlock.firstChild = next;
                    }
                    else if (this.currEvent.unlinkedBlocks.Contains(this.currBlock))
                    {
                        if (isDragOut)
                        {
                            this.dragOut = DragOut.Unlinked1st;
                            this.dragOutViewOffs = this.currBlock.b._ed_viewOffs;
                            this.dragOutBlockRef = this.currBlock.next;
                        }
                        this.currEvent.ev.unlinkedBlocks.Remove(this.currBlock.b);
                        this.currEvent.unlinkedBlocks.Remove(this.currBlock);
                        if (next != null)
                        {
                            next.b._ed_viewOffs = this.currBlock.b._ed_viewOffs;
                            this.currEvent.ev.unlinkedBlocks.Add(next.b);
                            this.currEvent.unlinkedBlocks.Add(next);
                        }
                    }
                    else if (isDragOut)
                    {
                        this.dragOut = DragOut.UnderBlock;
                        this.dragOutBlockRef = this.currBlock.prev;
                    }
                    if (next != null)
                    {
                        next.prev = prev;
                    }
                    if (prev != null)
                    {
                        prev.b.next = ((next != null) ? next.b : null);
                        prev.next = next;
                    }
                }
                this.currBlock = null;
                this.currBlock_change = null;
                BloxPropsPanel.Instance.SetSelectedBlock(null);
                this.SaveBlox(true);
            }
        }

        private void RemoveAllUnlinked()
        {
            Undo.RegisterCompleteObjectUndo(this.blox, "Remove unlinked Blocks");
            this.currEvent.ev.unlinkedBlocks.Clear();
            this.currEvent.unlinkedBlocks.Clear();
            this.SaveBlox(true);
        }

        private void ToggleCurrBlockActive()
        {
            if (this.currBlock != null)
            {
                this.currBlock.b.active = !this.currBlock.b.active;
                this.SaveBlox(true);
            }
        }

        private void ToggleEventActive()
        {
            if (this.currEvent.ev != null)
            {
                this.currEvent.ev.active = !this.currEvent.ev.active;
                this.SaveBlox(false);
            }
        }

        private BloxBlockEd CopyWithLinkedBlocks(BloxBlockEd bdi)
        {
            if (bdi == null)
            {
                return null;
            }
            return new BloxBlockEd(this.CopyBlockWithNext(bdi.b), null, null, null, -1, true);
        }

        private void StoreAllGraphBlocksAsCopied()
        {
            this.StoreAsCopiedWithLinked(this.currEvent.firstBlock);
        }

        private void StoreAsCopiedWithLinked(BloxBlockEd bdi)
        {
            this.copiedBlock = this.CopyWithLinkedBlocks(bdi);
        }

        private void StoreSelectedAsCopied()
        {
            if (this.currBlock != null)
            {
                BloxBlock b = this.CopyBlock(this.currBlock.b);
                this.copiedBlock = new BloxBlockEd(b, null, null, null, -1, true);
            }
        }

        private BloxBlock CopyBlockWithNext(BloxBlock source)
        {
            BloxBlock bloxBlock = this.CopyBlock(source);
            if (source.next != null)
            {
                BloxBlock next = source.next;
                BloxBlock bloxBlock2 = bloxBlock;
                while (next != null)
                {
                    bloxBlock2 = (bloxBlock2.next = this.CopyBlock(next));
                    next = next.next;
                }
            }
            return bloxBlock;
        }

        private BloxBlock CopyBlock(BloxBlock source)
        {
            if (source == null)
            {
                return null;
            }
            BloxBlock bloxBlock = (BloxBlock)Activator.CreateInstance(source.GetType());
            source.CopyTo(bloxBlock);
            if (source.contextBlock != null)
            {

                Debug.LogError("contextBlock " + source.contextBlock.ToString(), "Test");
                bloxBlock.contextBlock = this.CopyBlock(source.contextBlock);
            }
            if (((source.paramBlocks != null) ? source.paramBlocks.Length : 0) != 0)
            {
                bloxBlock.paramBlocks = new BloxBlock[source.paramBlocks.Length];
                for (int i = 0; i < source.paramBlocks.Length; i++)
                {
                    bloxBlock.paramBlocks[i] = this.CopyBlock(source.paramBlocks[i]);
                }
            }
            if (source.firstChild != null)
            {
                BloxBlock bloxBlock2 = source.firstChild;
                BloxBlock bloxBlock3 = null;
                while (bloxBlock2 != null)
                {
                    BloxBlock bloxBlock4 = this.CopyBlock(bloxBlock2);
                    if (bloxBlock3 == null)
                    {
                        bloxBlock.firstChild = bloxBlock4;
                    }
                    else
                    {
                        bloxBlock3.next = bloxBlock4;
                    }
                    bloxBlock3 = bloxBlock4;
                    bloxBlock2 = bloxBlock2.next;
                }
            }
            return bloxBlock;
        }

        private void CutSelectedBlock(bool isDragOut)
        {
            this.StoreSelectedAsCopied();
            this.DeleteSelectedBlock(isDragOut);
            if (isDragOut)
            {
                this.PasteBlock();
            }
        }

        private void PasteBlock()
        {
            if (this.copiedBlock != null)
            {
                this.dragDrop = this.copiedBlock.def;
                this.dragDropBlock = this.copiedBlock;
                this.GC_DragDropLabel.text = this.dragDrop.name;
            }
        }

        private void ClearDragDropBlock()
        {
            this.dragOut = DragOut.None;
            this.dragDrop = null;
            this.dragDropBlock = null;
            this.dragOutBlockRef = null;
            this.dragDropInsertContext_change = null;
            this.dragDropInsertFieldIdx_change = -1;
            this.dragDropExtraWidth_change = 0f;
            this.doRepaint = true;
        }

        private void RestoreFromDragOut()
        {
            if (this.dragOut == DragOut.None || this.dragOut == DragOut.DontWorryAboutIt)
            {
                Debug.Log("RestoreFromDragOut  null", "BloxEditorWindow", Color.magenta);
                this.ClearDragDropBlock();
            }
            else
            {
                this.dragDropBlock = this.copiedBlock;
                if (this.dragOut == DragOut.Unlinked1st)
                {
                    this.dragDropInsertTop = false;
                    this.dragDropInsertFieldIdx = -1;
                    this.dragDropInsertContext = null;
                }
                else if (this.dragOut == DragOut.Event1st)
                {
                    this.dragDropInsertTop = true;
                    this.dragDropInsertFieldIdx = -1;
                    this.dragDropInsertContext = null;
                }
                else if (this.dragOut == DragOut.ParentBlock1st)
                {
                    this.dragDropInsertTop = true;
                    this.dragDropInsertFieldIdx = -1;
                    this.dragDropInsertContext = this.dragOutBlockRef;
                }
                else if (this.dragOut == DragOut.UnderBlock)
                {
                    this.dragDropInsertTop = false;
                    this.dragDropInsertFieldIdx = -1;
                    this.dragDropInsertContext = this.dragOutBlockRef;
                }
                else if (this.dragOut == DragOut.WasContext)
                {
                    this.dragDropInsertTop = false;
                    this.dragDropInsertFieldIdx = 9999;
                    this.dragDropInsertContext = this.dragOutBlockRef;
                }
                else if (this.dragOut >= DragOut.WasParam)
                {
                    this.dragDropInsertTop = false;
                    this.dragDropInsertFieldIdx = (int)(this.dragOut - 7);
                    this.dragDropInsertContext = this.dragOutBlockRef;
                }
                Debug.Log("RestoreFromDragOut not null", "BloxEditorWindow", Color.magenta);
                this.AddBlock();
            }
        }

        private Rect CalcStyleRect(GUIContent gc, GUIStyle st, Vector2 position)
        {
            Vector2 vector = st.CalcSize(gc);
            return new Rect(position.x, position.y, vector.x, vector.y);
        }

        private bool IsDragging()
        {
            if (this.dragDrop == null && this.dragDropBlock == null)
            {
                return this.blockDragStartDetected;
            }
            return true;
        }

        private void UndoRedoPerformed()
        {
            Blox bloxDef = this.blox;
            int num = this.eventIdx;
            this.currEvent.Set(null, false);
            this.blox = null;
            this.LoadBloxDef(bloxDef, false);
            this.eventIdx = ((num >= this.blox.events.Length) ? (-2) : num);
            this.currEvent.Set((this.eventIdx < 0) ? null : this.blox.events[this.eventIdx], false);
            base.Repaint();
        }

        private bool CanAllowInDragDrop(BloxBlockDef def, BloxBlockEd bdi)
        {
            if (def.blockSystemType != typeof(Variable_Block))
            {
                if (this.dragDropBlock != null)
                {
                    return this.dragDropBlock.b.GetType() == typeof(Variable_Block);
                }
                return false;
            }
            return true;
        }

        private bool CanAllowInDragDrop(Type dragDropType, Type expectedType)
        {
            if (expectedType.IsAssignableFrom(dragDropType))
            {
                return true;
            }
            if (dragDropType.IsAssignableFrom(expectedType))
            {
                return true;
            }
            if (!typeof(GameObject).IsAssignableFrom(dragDropType) && !typeof(Component).IsAssignableFrom(dragDropType))
            {
                goto IL_0060;
            }
            if (!typeof(GameObject).IsAssignableFrom(expectedType) && !typeof(Component).IsAssignableFrom(expectedType))
            {
                goto IL_0060;
            }
            return true;
        IL_0060:
            if (dragDropType == typeof(object))
            {
                return true;
            }
            if (expectedType.IsArray && (dragDropType == typeof(Array) || dragDropType == typeof(object[])))
            {
                return true;
            }
            if (expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == typeof(List<>) && dragDropType == typeof(List<object>))
            {
                return true;
            }
            return false;
        }
    }
}
