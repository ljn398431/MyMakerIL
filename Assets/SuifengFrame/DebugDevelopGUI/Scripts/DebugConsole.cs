#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
#define DEBUG
#endif

#if (UNITY_IOS || UNITY_ANDROID)
#define MOBILE
#endif

using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#if UNITY_5_3||UNITY_5_4||UNITY_5_5||UNITY_5_6
using UnityEngine.SceneManagement;
#endif

public class DebugConsole : MonoBehaviour
{
    readonly string version = "1.0";
    [HideInInspector]
    public int m_MaxLinesForDisplay;
    [HideInInspector]
    public Color defaultColor = Message.defaultColor;
    [HideInInspector]
    public Color warningColor = Message.warningColor;
    [HideInInspector]
    public Color errorColor = Message.errorColor;
    [HideInInspector]
    public Color systemColor = Message.systemColor;
    [HideInInspector]
    public Color inputColor = Message.inputColor;
    [HideInInspector]
    public Color outputColor = Message.outputColor;
    [HideInInspector]
    public WindowSize m_WindowSize;
    [HideInInspector]
    public Vector2 m_WindowPos;
    [HideInInspector]
    public string m_SaveDirPath;

    public delegate object DebugCommand(params string[] args);

    public static bool IsOn
    {
        get
        {
            if (DebugConsole.Instance == null)
            {
                return false;
            }
            else
            {
                return DebugConsole.Instance.m_isOn;
            }
        }
        set { DebugConsole.Instance.m_isOn = value; }
    }

    public static bool IsOpen
    {
        get
        {
            if (DebugConsole.Instance == null)
            {
                return false;
            }
            else
            {
                return DebugConsole.Instance.m_isOpen;
            }
        }
        set { DebugConsole.Instance.m_isOpen = value; }
    }

    public static GameObject m_Console;

    static DebugConsole Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;

                if (m_instance != null)
                {
                    return m_instance;
                }

                if (m_Console != null)
                {
                    m_instance = m_Console.AddComponent<DebugConsole>();
                }
            }
            return m_instance;
        }
    }

    public static int m_MaxLineForDisplay
    {
        get { return DebugConsole.Instance.m_MaxLinesForDisplay; }
        set { DebugConsole.Instance.m_MaxLinesForDisplay = value; }
    }

    public static WindowSize m_WindowRect
    {
        get { return DebugConsole.Instance.m_WindowSize; }
        set { DebugConsole.Instance.m_WindowSize = value; }
    }

    public static Vector2 m_WindowLoc
    {
        get { return DebugConsole.Instance.m_WindowPos; }
        set { DebugConsole.Instance.m_WindowPos = value; }
    }

    public static string m_SavePath
    {
        get { return DebugConsole.Instance.m_SaveDirPath; }
        set { DebugConsole.Instance.m_SaveDirPath = value; }
    }

    public static KeyCode m_toggleKey = KeyCode.BackQuote;
    static DebugConsole m_instance;
    Dictionary<string, DebugCommand> m_cmdTable = new Dictionary<string, DebugCommand>();
    public static Dictionary<string, WatchVarBase> m_watchVarTable = new Dictionary<string, WatchVarBase>();

    public static Rect m_windowRect;
#if MOBILE
		static Rect m_fakeWindowRect;
		static Rect m_fakeDragRect;
		bool dragging = false;
		GUIStyle windowOnStyle;
		GUIStyle windowStyle;
#if UNITY_EDITOR
			Vector2 prevMousePos;
#endif
#endif

    Vector2 m_logScrollPos = Vector2.zero;
    Vector2 m_logGroupScrollPos = Vector2.zero;
    Vector2 m_logCommandScrollPos = Vector2.zero;
    Vector2 m_rawLogScrollPos = Vector2.zero;
    Vector2 m_watchVarsScrollPos = Vector2.zero;
    Vector3 m_guiScale = Vector3.one;
    Matrix4x4 restoreMatrix = Matrix4x4.identity;
    bool m_scaled = false;
    bool m_isOpen;
    bool m_isOn = false;

    static WatchVar<object> m_WatchVar;
    static WatchVarBase m_WatchVarBase;

    StringBuilder m_displayLogTxt = new StringBuilder();
    StringBuilder m_displayComTxt = new StringBuilder();
    FPSCounter m_fps;
    bool m_dirty;
    string m_searchgroup = "ALL";
    string m_selectCommand = "";

    #region GUI position values	
    public static Rect scrollRect = new Rect(0, 0, 0, 0);
    public static Rect groupscrollRect = new Rect(0, 0, 0, 0);
    public static Rect commandscrollViewRect = new Rect(0, 0, 0, 0);
    public static Rect commandscrollRect = new Rect(0, 0, 0, 0);
    public static Rect watchscrollRect = new Rect(0, 0, 0, 0);
    public static Rect clearLogRect = new Rect(0, 0, 0, 0);
    public static Rect saveLogRect = new Rect(0, 0, 0, 0);
    public static Rect saveComRect = new Rect(0, 0, 0, 0);
    public static Rect toolbarRect = new Rect(0, 0, 0, 0);
    public static Rect messageLine = new Rect(0, 0, 0, 0);
    public static Rect currentgroup = new Rect(0, 0, 0, 0);
    public static Rect currentCommand = new Rect(0, 0, 0, 0);

    int lineOffset = -4;
    string[] tabs = new string[] { "Show Log", "Watcher", "Command" };

    Rect nameRect;
    Rect valueRect;
    Rect innerRect = new Rect(0, 0, 0, 0);
    Rect groupinnerRect = new Rect(0, 0, 0, 0);
    Rect commandinnerRect = new Rect(0, 0, 0, 0);
    Rect commandinnerViewRect = new Rect(0, 0, 0, 0);
    int innerHeight = 0;
    int groupinnerHeight = 0;
    int commandinnerHeight = 0;
    int commandinnerViewHeight = 0;
    int toolbarIndex = 0;
    GUIContent guiContent = new GUIContent();
    GUI.WindowFunction[] windowMethods;
    GUIStyle labelStyle;
    #endregion

    public enum MessageType
    {
        LOG,
        NORMAL,
        WARNING,
        ERROR,
        SYSTEM,
        INPUT,
        OUTPUT
    }

    struct Message
    {
        string text;
        string formatted;
        string msgGroup;
        MessageType type;

        public Color color { get; private set; }
        public MessageType m_MsgType { get; private set; }

        public string m_MsgGroup
        {
            get { return msgGroup; }
            set { msgGroup = value; }
        }

        public static Color defaultColor = Color.white;
        public static Color warningColor = Color.yellow;
        public static Color errorColor = Color.red;
        public static Color systemColor = Color.gray;
        public static Color inputColor = Color.green;
        public static Color outputColor = Color.cyan;

        public Message(object messageObject) : this(messageObject, MessageType.NORMAL, Message.defaultColor)
        {
        }

        public Message(object messageObject, Color displayColor) : this(messageObject, MessageType.LOG, displayColor)
        {
        }

        public Message(string messageGroup, object messageObject) : this(messageObject, messageGroup, MessageType.LOG, Message.defaultColor)
        {
        }

        public Message(object messageObject, MessageType messageType) : this(messageObject, messageType, Message.defaultColor)
        {
            switch (messageType)
            {
                case MessageType.ERROR:
                    color = errorColor;
                    break;
                case MessageType.SYSTEM:
                    color = systemColor;
                    break;
                case MessageType.WARNING:
                    color = warningColor;
                    break;
                case MessageType.OUTPUT:
                    color = outputColor;
                    break;
                case MessageType.INPUT:
                    color = inputColor;
                    break;
                case MessageType.LOG:
                    color = defaultColor;
                    break;
            }
        }

        public Message(object messageObject, MessageType messageType, Color displayColor) : this()
        {
            this.text = messageObject == null ? "<null>" : messageObject.ToString();

            this.formatted = string.Empty;
            this.type = messageType;
            this.color = displayColor;
        }
        public Message(object messageObject, string messageGroup, Color displayColor) : this()
        {
            this.text = messageObject == null ? "<null>" : messageObject.ToString();
            this.msgGroup = messageGroup;
            this.formatted = string.Empty;
            this.type = MessageType.LOG;
            this.color = displayColor;
        }

        public Message(object messageObject, string messageGroup, MessageType messageType, Color displayColor) : this()
        {
            this.text = messageObject == null ? "<null>" : messageObject.ToString();
            this.msgGroup = messageGroup;
            this.formatted = string.Empty;
            this.type = messageType;
            this.color = displayColor;
        }

        public static Message Normal(object message)
        {
            return new Message(message, MessageType.NORMAL, defaultColor);
        }

        public static Message Log(object message)
        {
            return new Message(message, MessageType.LOG, defaultColor);
        }

        public static Message Log(string messagegroup, object message)
        {
            return new Message(message, messagegroup, MessageType.LOG, defaultColor);
        }

        public static Message System(object message)
        {
            return new Message(message, MessageType.SYSTEM, systemColor);
        }

        public static Message Warning(object message)
        {
            return new Message(message, MessageType.WARNING, warningColor);
        }

        public static Message Warning(string messagegroup, object message)
        {
            return new Message(message, messagegroup, MessageType.WARNING, warningColor);
        }

        public static Message Error(object message)
        {
            return new Message(message, MessageType.ERROR, errorColor);
        }

        public static Message Error(string messagegroup, object message)
        {
            return new Message(message, messagegroup, MessageType.ERROR, errorColor);
        }

        public static Message Output(object message)
        {
            return new Message(message, MessageType.OUTPUT, outputColor);
        }

        public static Message Input(object message)
        {
            return new Message(message, MessageType.INPUT, inputColor);
        }

        public override string ToString()
        {
            switch (type)
            {
                case MessageType.LOG:
                case MessageType.ERROR:
                case MessageType.WARNING:
                    return string.Format("[{0}] {1}", type, text);
                default:
                    return ToGUIString();
            }
        }

        public string ToGUIString()
        {
            if (!string.IsNullOrEmpty(formatted))
            {
                return formatted;
            }

            switch (type)
            {
                case MessageType.INPUT:
                    formatted = string.Format("Input: {0}", text);
                    break;
                case MessageType.OUTPUT:
                    var lines = text.Trim('\n').Split('\n');
                    var output = new StringBuilder();

                    foreach (var line in lines)
                    {
                        output.AppendFormat("OutPut: {0}\n", line);
                    }

                    formatted = output.ToString();
                    break;
                case MessageType.SYSTEM:
                    formatted = string.Format("System: {0}", text);
                    break;
                case MessageType.WARNING:
                    formatted = string.Format("Warring: {0}", text);
                    break;
                case MessageType.ERROR:
                    formatted = string.Format("Error: {0}", text);
                    break;
                case MessageType.LOG:
                    formatted = string.Format("Log: {0}", text);
                    break;
                default:
                    formatted = text;
                    break;
            }
            return formatted;
        }
    }

    List<Message> _messages = new List<Message>();
    List<Message> _commands = new List<Message>();
    List<string> _groups = new List<string>();

    void Awake()
    {

        if (m_instance != null && m_instance != this)
        {
            DestroyImmediate(this, true);
            return;
        }

        m_instance = this;

        Debug.logEvent += Log;
        Debug.logColorEvent += Log;
        Debug.logExternEvent += Log;
        Debug.logGroupEvent += Log;
        Debug.logCmdEvent += LogCommand;
        Debug.logWarningEvent += LogWarning;
        Debug.logWarningGroupEvent += LogWarning;
        Debug.logErrorEvent += LogError;
        Debug.logErrorGroupEvent += LogError;
        Debug.watcherEvent += Watcher;
        Debug.watcherClearEvent += WatcherClear;

#if UNITY_EDITOR
        m_SaveDirPath = System.IO.Path.GetFullPath(".") + "/Log/";
#elif (UNITY_IOS || UNITY_ANDROID)
			m_SaveDirPath = Application.persistentDataPath + "/Log/";
#else
			m_SaveDirPath = Application.dataPath + "/Log/";
#endif
    }

    void OnEnable()
    {
        var scale = Screen.dpi / 160.0f;

        if (scale != 0.0f && scale >= 1.1f)
        {
            m_scaled = true;
            m_guiScale.Set(scale, scale, scale);
        }
        windowMethods = new GUI.WindowFunction[] { LogWindow, WatchVarWindow, CommandWindow };

        m_fps = new FPSCounter();
        StartCoroutine(m_fps.Update());

        nameRect = messageLine;
        valueRect = messageLine;

        Message.defaultColor = defaultColor;
        Message.warningColor = warningColor;
        Message.errorColor = errorColor;
        Message.systemColor = systemColor;
        Message.inputColor = inputColor;
        Message.outputColor = outputColor;
#if MOBILE
			this.useGUILayout = false;
#endif
        m_windowRect = new Rect(m_windowRect.x, m_windowRect.y, m_windowRect.width, m_windowRect.height);

        CommandMessage(Message.System(" Click the left commands."));
        CommandMessage(Message.Normal(""));

        //		this.RegisterCommandCallback("Close", CMDClose);
        this.RegisterCommandCallback("Clear", CMDClear);
        this.RegisterCommandCallback("SystemInfo", CMDSystemInfo);
        this.RegisterCommandCallback("PathInfo", CMDSystemPathInfo);
        this.RegisterCommandCallback("CameraInfo", CMDSystemCameraInfo);

    }

    public static void Watcher(string watcherName, object watcherValue)
    {
        if (DebugConsole.m_watchVarTable.TryGetValue(watcherName, out m_WatchVarBase))
        {
            m_WatchVarBase.m_value = watcherValue;
        }
        else
        {
            m_WatchVar = new WatchVar<object>(watcherName);
            m_WatchVar.Value = watcherValue;
        }
    }
    public static void WatcherClear()
    {
        m_watchVarTable.Clear();
    }
    void SaveLog()
    {
        DirectoryInfo dir = new DirectoryInfo(m_SaveDirPath);
        if (dir.Exists == false) dir.Create();

        FileStream fs = new FileStream(m_SaveDirPath + "LogView " + DateTime.Now.ToString("MM-dd (HH-mm-ss)") + ".log", FileMode.Append);
        StreamWriter swriter = new StreamWriter(fs);
        try
        {
            swriter.Write(GetDisplayLogTxt());
        }
        catch { }
        finally
        {
            swriter.Close();
            LogMessage(Message.System(":: Log save is over!!"));
            LogMessage(Message.System(":: Save Path : " + m_SaveDirPath + "LogView " + DateTime.Now.ToString("MM-dd (HH-mm-ss)") + ".log"));
        }
    }

    void SaveCommand()
    {
        DirectoryInfo dir = new DirectoryInfo(m_SaveDirPath);
        if (dir.Exists == false) dir.Create();

        FileStream fs = new FileStream(m_SaveDirPath + "CommandView " + DateTime.Now.ToString("MM-dd (HH-mm-ss)") + ".log", FileMode.Append);
        StreamWriter swriter = new StreamWriter(fs);
        try
        {
            swriter.Write(GetDisplayComTxt());
        }
        catch { }
        finally
        {
            swriter.Close();
            CommandMessage(Message.System(":: Command Log save is over!!"));
            CommandMessage(Message.System(":: Save Path : " + m_SaveDirPath + "CommandView " + DateTime.Now.ToString("MM-dd (HH-mm-ss)") + ".log"));
        }
    }

    public static void SetWindowSize()
    {
        DebugConsole t_Obj = DebugConsole.Instance;
        m_windowRect = new Rect(t_Obj.m_WindowPos.x, t_Obj.m_WindowPos.y, t_Obj.m_WindowSize.m_WindowWidth, t_Obj.m_WindowSize.m_WindowHeight);
#if MOBILE
		m_fakeWindowRect = new Rect(m_windowRect.x, m_windowRect.y, m_windowRect.width, m_windowRect.height);
		m_fakeDragRect = new Rect(m_windowRect.x, m_windowRect.y, m_windowRect.width - 32, 30);
#endif
        scrollRect = new Rect(150, 50, t_Obj.m_WindowSize.m_WindowWidth - 160, t_Obj.m_WindowSize.m_WindowHeight - 85);
        groupscrollRect = new Rect(10, 70, 135, t_Obj.m_WindowSize.m_WindowHeight - 105);
        commandscrollViewRect = new Rect(150, 50, t_Obj.m_WindowSize.m_WindowWidth - 160, t_Obj.m_WindowSize.m_WindowHeight - 85);
        commandscrollRect = new Rect(10, 50, 135, t_Obj.m_WindowSize.m_WindowHeight - 85);
        watchscrollRect = new Rect(10, 50, t_Obj.m_WindowSize.m_WindowWidth - 20, t_Obj.m_WindowSize.m_WindowHeight - 58);
        clearLogRect = new Rect(t_Obj.m_WindowSize.m_WindowWidth - 160, t_Obj.m_WindowSize.m_WindowHeight - 30, 70, 24);
        saveLogRect = new Rect(t_Obj.m_WindowSize.m_WindowWidth - 80, t_Obj.m_WindowSize.m_WindowHeight - 30, 70, 24);
        saveComRect = new Rect(t_Obj.m_WindowSize.m_WindowWidth - 80, t_Obj.m_WindowSize.m_WindowHeight - 30, 70, 24);
        toolbarRect = new Rect(10, 20, t_Obj.m_WindowSize.m_WindowWidth - 20, 25);
        messageLine = new Rect(4, 0, t_Obj.m_WindowSize.m_WindowWidth - 40, 20);
        currentgroup = new Rect(10, 50, 135, 25);
        currentCommand = new Rect(10, 50, 135, 25);
    }

    [Conditional("DEBUG")]
    void OnGUI()
    {
        var evt = Event.current;

        if (m_scaled)
        {
            restoreMatrix = GUI.matrix;

            GUI.matrix = GUI.matrix * Matrix4x4.Scale(m_guiScale);
        }

        while (_messages.Count > m_MaxLinesForDisplay)
        {
            _messages.RemoveAt(0);
        }
#if UNITY_EDITOR
        if (evt.keyCode == m_toggleKey && evt.type == EventType.KeyUp) m_isOpen = !m_isOpen;
#endif
#if MOBILE
			if (Input.touchCount == 1) {
				var touch = Input.GetTouch(0);
#if DEBUG
					if (evt.type == EventType.Repaint && (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) && touch.tapCount == 3) {
						m_isOpen = !m_isOpen;
					}
#endif
				if (m_isOpen) {
					var pos = touch.position;
					pos.y = Screen.height - pos.y;
					
					if (dragging && (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)) {
						dragging = false;
					}
					else if (!dragging && (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)) {
						var dragRect = m_fakeDragRect;
						
						dragRect.x = m_windowRect.x * m_guiScale.x;
						dragRect.y = m_windowRect.y * m_guiScale.y;
						dragRect.width *= m_guiScale.x;
						dragRect.height *= m_guiScale.y;

						if (dragRect.Contains(pos)) {
							dragging = true;
						}
					}
			
				    if (dragging && evt.type == EventType.Repaint) {
#if UNITY_ANDROID
						      var delta = touch.deltaPosition * 2.0f;
#elif UNITY_IOS
						      var delta = touch.deltaPosition;
						      delta.x /= m_guiScale.x;
						      delta.y /= m_guiScale.y;
#endif
						delta.y = -delta.y;					
						m_windowRect.center += delta;
				    }
				    else {
						var tapRect = scrollRect;
						tapRect.x += m_windowRect.x * m_guiScale.x;
						tapRect.y += m_windowRect.y * m_guiScale.y;
						tapRect.width -= 32;
						tapRect.width *= m_guiScale.x;
						tapRect.height *= m_guiScale.y;
						
						if (tapRect.Contains(pos)) {
							var scrollY = (tapRect.center.y - pos.y) / m_guiScale.y;
							
							switch (toolbarIndex) {
								case 0:
									m_logScrollPos.y -= scrollY;
									break;
								case 1:
									m_rawLogScrollPos.y -= scrollY;
									break;
								case 2:
									m_watchVarsScrollPos.y -= scrollY;
									break;
							}
						}
				    }
				}
			}
			else if (dragging && Input.touchCount == 0) {
				dragging = false;
			}
#endif
        if (!m_isOpen)
        {
            return;
        }
        labelStyle = GUI.skin.label;
        groupinnerRect.width = groupscrollRect.width - 20;
        innerRect.width = scrollRect.width - 20;
        commandinnerRect.width = commandscrollRect.width - 20;
        commandinnerViewRect.width = commandscrollViewRect.width - 20;
#if !MOBILE
        m_windowRect = GUI.Window(-1111, m_windowRect, windowMethods[toolbarIndex], string.Format("Log Watcher Console \tFPS: {1:00.0}", version, m_fps.current));
        GUI.BringWindowToFront(-1111);
#else
			if (windowStyle == null) {
				windowStyle = new GUIStyle(GUI.skin.window);
				windowOnStyle = new GUIStyle(GUI.skin.window);
				windowOnStyle.normal.background = GUI.skin.window.onNormal.background;
			}
		
			GUI.BeginGroup(m_windowRect);
#if UNITY_EDITOR
					if (GUI.RepeatButton(m_fakeDragRect, string.Empty, GUIStyle.none)) {
						Vector2 delta = (Vector2) Input.mousePosition - prevMousePos;
						delta.y = -delta.y;
						
						m_windowRect.center += delta;
						dragging = true;
					}
					
					if (evt.type == EventType.Repaint) {
						prevMousePos = Input.mousePosition;
					}
#endif
				GUI.Box(m_fakeWindowRect, string.Format("Log Watcher Console \tFPS: {1:00.0}", version, m_fps.current), dragging ? windowOnStyle : windowStyle);
				windowMethods[toolbarIndex](0);
			GUI.EndGroup();
#endif

        if (m_scaled)
        {
            GUI.matrix = restoreMatrix;
        }

        if (m_dirty && evt.type == EventType.Repaint)
        {
            m_logScrollPos.y = 50000.0f;
            m_rawLogScrollPos.y = 50000.0f;

            BuildDisplayString();
            m_dirty = false;
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
    #region StaticAccessors	
    public static object Log(object message)
    {
        if (IsOn) DebugConsole.Instance.LogMessage(Message.Log(message));
        return message;
    }

    public static object Log(object message, string messageGroup)
    {
        if (IsOn) DebugConsole.Instance.LogMessage(new Message(messageGroup, message));
        return message;
    }
    public static object LogFormat(string format, params object[] args)
    {
        return Log(string.Format(format, args));
    }

    public static object Log(object message, MessageType messageType)
    {
        if (IsOn)
        {
            if (messageType == MessageType.OUTPUT)
            {
                DebugConsole.Instance.CommandMessage(new Message(message, messageType));
            }
            else
            {
                DebugConsole.Instance.LogMessage(new Message(message, messageType));
            }
        }
        return message;
    }

    public static object Log(object message, Color displayColor)
    {
        if (IsOn) DebugConsole.Instance.LogMessage(new Message(message, displayColor));
        return message;
    }

    public static object Log(object message, string messageGroup, Color displayColor)
    {
        if (IsOn) DebugConsole.Instance.LogMessage(new Message(message, messageGroup, displayColor));
        return message;
    }
    public static object Log(object message, MessageType messageType, Color displayColor)
    {
        if (IsOn) DebugConsole.Instance.LogMessage(new Message(message, messageType, displayColor));
        return message;
    }

    public static object LogWarning(object message)
    {
        if (IsOn) DebugConsole.Instance.LogMessage(Message.Warning(message));
        return message;
    }

    public static object LogError(object message)
    {
        if (IsOn) DebugConsole.Instance.LogMessage(Message.Error(message));
        return message;
    }

    public static object LogWarning(object message, string messageGroup)
    {
        if (IsOn) DebugConsole.Instance.LogMessage(Message.Warning(messageGroup, message));
        return message;
    }

    public static object LogError(object message, string messageGroup)
    {
        if (IsOn) DebugConsole.Instance.LogMessage(Message.Error(messageGroup, message));
        return message;
    }

    public static object LogCommand(object message)
    {
        if (IsOn) DebugConsole.Instance.CommandMessage(Message.Log(message));
        return message;
    }

    [Conditional("DEBUG")]
    public static void Clear()
    {
        if (IsOn) DebugConsole.Instance.ClearLog();
    }

    [Conditional("DEBUG")]
    public static void Execute(string commandString)
    {
        if (IsOn) DebugConsole.Instance.EvalInputString(commandString);
    }

    [Conditional("DEBUG")]
    public static void RegisterCommand(string commandString, DebugCommand commandCallback)
    {
        if (IsOn) DebugConsole.Instance.RegisterCommandCallback(commandString, commandCallback);
    }

    [Conditional("DEBUG")]
    public static void UnRegisterCommand(string commandString)
    {
        if (IsOn) DebugConsole.Instance.UnRegisterCommandCallback(commandString);
    }

    [Conditional("DEBUG")]
    public static void RegisterWatchVar(WatchVarBase watchVar)
    {
        if (IsOn) DebugConsole.Instance.AddWatchVarToTable(watchVar);
    }

    [Conditional("DEBUG")]
    public static void UnRegisterWatchVar(string name)
    {
        if (IsOn) DebugConsole.Instance.RemoveWatchVarFromTable(name);
    }
    #endregion

    #region Console commands
    object CMDClose(params string[] args)
    {
        m_isOpen = false;
        return "closed";
    }

    object CMDClear(params string[] args)
    {
        this.ClearCommand();
        return "clear";
    }

    object CMDSystemInfo(params string[] args)
    {
        var info = new StringBuilder();

        info.AppendFormat("Unity Ver: {0}\n", Application.unityVersion);
        info.AppendFormat("Platform: {0} Language: {1}\n", Application.platform, Application.systemLanguage);
        info.AppendFormat("Screen:({0},{1}) DPI:{2} Target:{3}fps\n", Screen.width, Screen.height, Screen.dpi, Application.targetFrameRate);
#if UNITY_4
        info.AppendFormat("Level: {0} ({1} of {2})\n", Application.loadedLevelName, Application.loadedLevel, Application.levelCount);
#elif UNITY_5
        info.AppendFormat("Level: {0} ({1} of {2})\n", SceneManager.GetActiveScene().name, SceneManager.GetActiveScene().rootCount, SceneManager.sceneCountInBuildSettings);
#endif
        info.AppendFormat("Quality: {0}\n", QualitySettings.names[QualitySettings.GetQualityLevel()]);

#if UNITY_WEBPLAYER
				info.AppendLine();
				info.AppendFormat("URL: {0}\n", Application.absoluteURL);
				info.AppendFormat("srcValue: {0}\n", Application.srcValue);
				info.AppendFormat("security URL: {0}\n", Application.webSecurityHostUrl);
#endif
#if MOBILE
				info.AppendLine();
				info.AppendFormat("Net Reachability: {0}\n", Application.internetReachability);
				info.AppendFormat("Multitouch: {0}\n", Input.multiTouchEnabled);
#endif
#if UNITY_EDITOR
        info.AppendLine();
        info.AppendFormat("EditorApp: {0}\n", UnityEditor.EditorApplication.applicationPath);
        info.AppendFormat("EditorAppContents: {0}\n", UnityEditor.EditorApplication.applicationContentsPath);
        info.AppendFormat("scene: {0}\n", UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name);
#endif
        info.AppendFormat("End: SystemInfo");
        info.AppendLine();

        return info.ToString();
    }

    object CMDSystemPathInfo(params string[] args)
    {
        var info = new StringBuilder();

        info.AppendFormat("Data Path: {0}\n", Application.dataPath);
        info.AppendFormat("Cache Path: {0}\n", Application.temporaryCachePath);
        info.AppendFormat("Persistent Path: {0}\n", Application.persistentDataPath);
        info.AppendFormat("Streaming Path: {0}\n", Application.streamingAssetsPath);
        info.AppendFormat("Log Save Path: {0}\n", m_SaveDirPath);
        info.AppendFormat("End: PathInfo");
        info.AppendLine();
        return info.ToString();
    }

    object CMDSystemCameraInfo(params string[] args)
    {
        var info = new StringBuilder();

        var devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            info.AppendLine("Cameras: ");
            foreach (var device in devices)
            {
                info.AppendFormat("  {0} front:{1}\n", device.name, device.isFrontFacing);
            }
        }
        info.AppendFormat("End: CameraInfo");
        info.AppendLine();
        return info.ToString();
    }

    #endregion

    #region GUI Window Methods
    void DrawBottomControls()
    {
        var index = GUI.Toolbar(toolbarRect, toolbarIndex, tabs);

        if (index != toolbarIndex)
        {
            toolbarIndex = index;
        }
#if !MOBILE
        GUI.DragWindow();
#endif
    }

    void hAddLogItem(int windowID, string additem)
    {
        GUIStyle groupStyle = new GUIStyle();
        groupStyle.normal.textColor = Color.white;
        guiContent.text = additem;
        messageLine.height = labelStyle.CalcHeight(guiContent, groupinnerRect.width);
        messageLine.width = groupinnerRect.width;

        bool tOver = messageLine.Contains(Event.current.mousePosition);
        if (tOver && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1)
        {
            Event.current.Use();
            m_searchgroup = additem;
            LogWindow(windowID);
        }
        else if (Event.current.type == EventType.repaint)
        {
            if ((_messages.Count > 0) && (m_searchgroup == additem))
            {
                groupStyle.normal.textColor = Color.yellow;
                GUI.Label(messageLine, guiContent, groupStyle);
            }
        }
        GUI.Label(messageLine, guiContent, groupStyle);
        messageLine.y += (messageLine.height + lineOffset);
    }

    void hAddCommandItem(int windowID, string additem)
    {
        GUIStyle commandStyle = new GUIStyle();
        commandStyle.normal.textColor = Color.white;
        guiContent.text = additem;
        messageLine.height = labelStyle.CalcHeight(guiContent, commandinnerRect.width);
        messageLine.width = commandinnerRect.width;

        bool tOver = messageLine.Contains(Event.current.mousePosition);
        if (tOver && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1)
        {
            m_selectCommand = additem;
            Event.current.Use();
            EvalInputString(additem);
            CommandWindow(windowID);
        }
        else if (Event.current.type == EventType.repaint)
        {
            if ((m_selectCommand != "") && (m_selectCommand == additem))
            {
                commandStyle.normal.textColor = Color.yellow;
                GUI.Label(messageLine, guiContent, commandStyle);
            }
        }
        GUI.Label(messageLine, guiContent, commandStyle);
        messageLine.y += (messageLine.height + lineOffset);
    }

    void LogWindow(int windowID)
    {
        GUI.Label(currentgroup, "Group: " + m_searchgroup);
        GUI.Box(groupscrollRect, string.Empty);
        GUI.Box(scrollRect, string.Empty);
        groupinnerRect.height = groupinnerHeight < groupscrollRect.height ? groupscrollRect.height : groupinnerHeight;

        m_logGroupScrollPos = GUI.BeginScrollView(groupscrollRect, m_logGroupScrollPos, groupinnerRect, false, true);

        if (_messages != null || _messages.Count > 0)
        {
            messageLine.y = 0;
            _groups.Clear();

            hAddLogItem(windowID, "ALL");

            foreach (Message m in _messages)
            {
                if ((!_groups.Contains(m.m_MsgGroup)) && (m.m_MsgGroup != null))
                {
                    hAddLogItem(windowID, m.m_MsgGroup);
                    groupinnerHeight = messageLine.y > groupscrollRect.height ? (int)messageLine.y : (int)groupscrollRect.height;
                    _groups.Add(m.m_MsgGroup);
                }
            }
        }
        GUI.EndScrollView();

        innerRect.height = innerHeight < scrollRect.height ? scrollRect.height : innerHeight;
        m_logScrollPos = GUI.BeginScrollView(scrollRect, m_logScrollPos, innerRect, false, true);
        if (_messages != null || _messages.Count > 0)
        {
            Color oldColor = GUI.contentColor;
            messageLine.y = 0;

            foreach (Message m in _messages)
            {
                if ((m_searchgroup == "ALL") || (m_searchgroup == m.m_MsgGroup))
                {
                    GUI.contentColor = m.color;
                    guiContent.text = m.ToGUIString();
                    messageLine.height = labelStyle.CalcHeight(guiContent, innerRect.width);
                    messageLine.width = innerRect.width;
                    GUI.Label(messageLine, guiContent);
                    messageLine.y += (messageLine.height + lineOffset);
                    innerHeight = messageLine.y > scrollRect.height ? (int)messageLine.y : (int)scrollRect.height;
                }
            }
            GUI.contentColor = oldColor;
        }
        GUI.EndScrollView();

        if (GUI.Button(clearLogRect, "Clear"))
        {
            this.ClearLog();
        }

        if (GUI.Button(saveLogRect, "Save Log"))
        {
            SaveLog();
        }

        var index = GUI.Toolbar(toolbarRect, toolbarIndex, tabs);

        if (index != toolbarIndex)
        {
            toolbarIndex = index;
        }
#if !MOBILE
        GUI.DragWindow();
#endif
    }

    void CommandWindow(int windowID)
    {

        GUI.Box(commandscrollRect, string.Empty);
        GUI.Box(commandscrollViewRect, string.Empty);

        commandinnerRect.height = commandinnerHeight < commandscrollRect.height ? commandscrollRect.height : commandinnerHeight;

        m_logCommandScrollPos = GUI.BeginScrollView(commandscrollRect, m_logCommandScrollPos, commandinnerRect, false, true);

        messageLine.y = 0;

        foreach (string key in m_cmdTable.Keys)
        {

            hAddCommandItem(windowID, key);
        }
        GUI.EndScrollView();

        commandinnerViewRect.height = commandinnerViewHeight < commandscrollViewRect.height ? commandscrollViewRect.height : commandinnerViewHeight;
        m_rawLogScrollPos = GUI.BeginScrollView(commandscrollViewRect, m_rawLogScrollPos, commandinnerViewRect, false, true);
        if (_commands != null || _commands.Count > 0)
        {
            Color oldColor = GUI.contentColor;
            messageLine.y = 0;

            foreach (Message m in _commands)
            {
                GUI.contentColor = m.color;
                guiContent.text = m.ToGUIString();
                messageLine.height = labelStyle.CalcHeight(guiContent, commandinnerViewRect.width);
                messageLine.width = commandinnerViewRect.width;

                GUI.Label(messageLine, guiContent);
                messageLine.y += (messageLine.height + lineOffset);
                commandinnerViewHeight = messageLine.y > commandscrollViewRect.height ? (int)messageLine.y : (int)commandscrollViewRect.height;
            }
            GUI.contentColor = oldColor;
        }
        GUI.EndScrollView();

        if (GUI.Button(saveComRect, "Save Log"))
        {
            SaveCommand();
        }

        var index = GUI.Toolbar(toolbarRect, toolbarIndex, tabs);

        if (index != toolbarIndex)
        {
            toolbarIndex = index;
        }
#if !MOBILE
        GUI.DragWindow();
#endif
    }

    string GetDisplayLogTxt()
    {
        if (_messages == null) return string.Empty;
        return m_displayLogTxt.ToString();
    }

    string GetDisplayComTxt()
    {
        if (_commands == null) return string.Empty;
        return m_displayComTxt.ToString();
    }

    void BuildDisplayString()
    {
        m_displayLogTxt.Length = 0;
        m_displayComTxt.Length = 0;
        foreach (Message m in _messages) m_displayLogTxt.AppendLine(m.ToString());
        foreach (Message m in _commands) m_displayComTxt.AppendLine(m.ToString());
    }

    void WatchVarWindow(int windowID)
    {
        GUI.Box(watchscrollRect, string.Empty);
        innerRect.height = innerHeight < watchscrollRect.height ? watchscrollRect.height : innerHeight;

        m_watchVarsScrollPos = GUI.BeginScrollView(watchscrollRect, m_watchVarsScrollPos, innerRect, false, true);
        int line = 0;
        nameRect.y = valueRect.y = 0;
        nameRect.x = messageLine.x;
        float totalWidth = watchscrollRect.width - messageLine.x - 20;
        float nameMin;
        float nameMax;
        float valMin;
        float valMax;
        float stepHeight;
        var textAreaStyle = GUI.skin.textArea;

        foreach (var kvp in m_watchVarTable)
        {
            var nameContent = new GUIContent(string.Format("{0}:", kvp.Value.Name));
            var valContent = new GUIContent(kvp.Value.ToString());

            labelStyle.CalcMinMaxWidth(nameContent, out nameMin, out nameMax);
            textAreaStyle.CalcMinMaxWidth(valContent, out valMin, out valMax);

            if (nameMax > totalWidth)
            {
                nameRect.width = totalWidth - valMin;
                valueRect.width = valMin;
            }
            else if (valMax + nameMax > totalWidth)
            {
                valueRect.width = totalWidth - nameMin;
                nameRect.width = nameMin;
            }
            else
            {
                valueRect.width = valMax;
                nameRect.width = nameMax;
            }

            nameRect.height = labelStyle.CalcHeight(nameContent, nameRect.width);
            valueRect.height = textAreaStyle.CalcHeight(valContent, valueRect.width);
            valueRect.x = totalWidth - valueRect.width + nameRect.x;

            GUI.Label(nameRect, nameContent);
            GUI.TextArea(valueRect, valContent.text);

            stepHeight = Mathf.Max(nameRect.height, valueRect.height) + 4;

            nameRect.y += stepHeight;
            valueRect.y += stepHeight;

            innerHeight = valueRect.y > watchscrollRect.height ? (int)valueRect.y : (int)watchscrollRect.height;

            line++;
        }
        GUI.EndScrollView();

        DrawBottomControls();
    }
    #endregion

    #region InternalFunctionality
    [Conditional("DEBUG")]
    void LogMessage(Message msg)
    {
        _messages.Add(msg);
        m_dirty = true;
    }

    void ClearLog()
    {
        _messages.Clear();
    }

    [Conditional("DEBUG")]
    void CommandMessage(Message msg)
    {
        _commands.Add(msg);
        m_dirty = true;
    }

    void ClearCommand()
    {
        _commands.Clear();
    }

    void RegisterCommandCallback(string commandString, DebugCommand commandCallback)
    {
#if !UNITY_FLASH
        m_cmdTable[commandString] = new DebugCommand(commandCallback);
#endif
    }

    void UnRegisterCommandCallback(string commandString)
    {
        m_cmdTable.Remove(commandString);
    }

    void AddWatchVarToTable(WatchVarBase watchVar)
    {
        m_watchVarTable[watchVar.Name] = watchVar;
    }

    void RemoveWatchVarFromTable(string name)
    {
        m_watchVarTable.Remove(name);
    }

    void EvalInputString(string inputString)
    {
        inputString = inputString.Trim();
        if (string.IsNullOrEmpty(inputString))
        {
            LogMessage(Message.Input(string.Empty));
            return;
        }
        CommandMessage(Message.Input(inputString));
        var input = new List<string>(inputString.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries));
        var cmd = input[0];
        if (m_cmdTable.ContainsKey(cmd))
        {
            Log(m_cmdTable[cmd](input.ToArray()), MessageType.OUTPUT);
        }
        else
        {
            CommandMessage(Message.Output(string.Format("Unknown Command: {0}", cmd)));
        }
    }
    #endregion
}

public abstract class WatchVarBase
{

    public string Name { get; private set; }

    public object m_value;

    public WatchVarBase(string name, object val) : this(name)
    {
        m_value = val;
    }

    public WatchVarBase(string name)
    {
        Name = name;
        Register();
    }

    public void Register()
    {
        DebugConsole.RegisterWatchVar(this);
    }

    public void UnRegister()
    {
        DebugConsole.UnRegisterWatchVar(Name);
    }

    public object ObjValue
    {
        get { return m_value; }
    }

    public override string ToString()
    {
        if (m_value == null)
        {
            return "<null>";
        }

        return m_value.ToString();
    }
}

public class WatchVar<T> : WatchVarBase
{
    public T Value
    {
        get { return (T)m_value; }
        set { m_value = value; }
    }

    public WatchVar(string name) : base(name)
    {

    }

    public WatchVar(string name, T val) : base(name, val)
    {

    }
}

public class FPSCounter
{
    public float current = 0.0f;
    public float updateInterval = 0.5f;

    float accum = 0;
    int frames = 1;
    float timeleft;
    float delta;

    public FPSCounter()
    {
        timeleft = updateInterval;
    }

    public IEnumerator Update()
    {

        yield return null;

        while (true)
        {
            delta = Time.deltaTime;

            timeleft -= delta;
            accum += Time.timeScale / delta;
            ++frames;

            if (timeleft <= 0.0f)
            {
                current = accum / frames;
                timeleft = updateInterval;
                accum = 0.0f;
                frames = 0;
            }

            yield return null;
        }
    }
}

