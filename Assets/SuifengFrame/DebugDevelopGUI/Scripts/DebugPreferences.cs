#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
#define DEBUG
#endif

using UnityEngine;
using System;
using System.Collections;

[AddComponentMenu("Suifeng Debug Tool/DebugPreferences")]
public class DebugPreferences : MonoBehaviour {
	
	public bool m_ShowDebugView = false;	
	public int m_MaxLinesForDisplay;
	public KeyCode m_ToggleKey = KeyCode.BackQuote;
	public WindowSize m_WindowSize;
	public Vector2 m_WindowPos;
	public DebugCommand[] m_Commands;	
	
	static DebugPreferences m_Instance;
	
	void Awake ()
	{
		if (m_Instance == null) {
			#if DEBUG
			Debug.OnEnable();
			m_Instance = this;			
			DebugConsole.m_Console = this.gameObject;
			DebugConsole.IsOn = true; //Debug Console Active.
			DebugConsole.IsOpen = m_ShowDebugView; //Debug Console Show.
			DebugConsole.m_toggleKey = m_ToggleKey;
			DebugConsole.m_MaxLineForDisplay = m_MaxLinesForDisplay;				
			DebugConsole.m_WindowRect = m_WindowSize;
			DebugConsole.m_WindowLoc = m_WindowPos;
			DebugConsole.SetWindowSize();

            Debug.addLogToFileEvent += DebugExecution.AddLog;
			foreach(DebugCommand t_Command in m_Commands)
			{
				t_Command.hInit();
			}			
			
			DontDestroyOnLoad (gameObject);
			#endif
			
		} else if (m_Instance != this) {
			Destroy (gameObject);
		}		
	}
    public static void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    public static void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public static void HandleLog(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                SystemLog(logString, stackTrace);
                break;

            case LogType.Warning:
                SystemLogWarning(logString, stackTrace);
                break;

            case LogType.Error:
                SystemLogError(logString, stackTrace);
                break;

            case LogType.Exception:
                SystemLogException(logString, stackTrace);
                break;

            default:
                SystemLogException(logString, stackTrace);
                break;
        }
    }
    public static void SystemLog(string message, string stackTrace)
    {
        //DebugExecution.AddSystemLog(LogType.Log, message, stackTrace, "ALL");
        //DebugConsole.Log(message);
    }

    public static void SystemLogWarning(string message, string stackTrace)
    {
        //DebugExecution.AddSystemLog(LogType.Warning, message, stackTrace, "ALL");
        //DebugConsole.LogWarning(message);
    }

    public static void SystemLogError(string message, string stackTrace)
    {
        //DebugExecution.AddSystemLog(LogType.Error, message, stackTrace, "ALL");
        //DebugConsole.LogError(message);
    }

    public static void SystemLogException(string message, string stackTrace)
    {
        //DebugExecution.AddSystemLog(LogType.Exception, message, stackTrace, "ALL");
        //DebugConsole.LogError(message);
    }
}
	
[System.Serializable]
public class WindowSize {
	public float m_WindowWidth = 600;
	public float m_WindowHeight = 800;
}

[System.Serializable]
public class DetailObj {
	public GameObject m_TargetObject;
	public string m_ComponetName;
}

[System.Serializable]
public class DebugCommand {
	public string m_Command;	
	public string m_FunctionName;	
	public DetailObj m_TargetDetail;
	
	public void hInit()
	{
		if ((m_Command != null) && (m_FunctionName != null))
		DebugConsole.RegisterCommand(m_Command, hRun);
	}
	
	public object hRun(params string[] args)
	{		
		if (m_TargetDetail.m_TargetObject == null)
		{
			GameObject[] gos = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));
			foreach(GameObject go in gos)
			{
				if (go && go.transform.parent == null)
				{
					go.gameObject.BroadcastMessage(m_FunctionName, SendMessageOptions.DontRequireReceiver);	
				}
			}
		}else if (m_TargetDetail.m_ComponetName == "")
		{
			m_TargetDetail.m_TargetObject.BroadcastMessage(m_FunctionName, SendMessageOptions.DontRequireReceiver);
		}else
		{
			m_TargetDetail.m_TargetObject.GetComponent(m_TargetDetail.m_ComponetName).SendMessage(m_FunctionName, SendMessageOptions.DontRequireReceiver);
		}
		return ("End: "+m_Command);
	}	
}