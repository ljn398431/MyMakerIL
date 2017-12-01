#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
#define DEBUG
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityDebug = UnityEngine.Debug;

public static class DebugLogger
{

    static List<WatchVar<object>> m_WatchVars;
    static WatchVar<object> m_WatchVar;
    static WatchVarBase m_WatchVarBase;

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
    {
        UnityDebug.DrawLine(start, end, color, duration, depthTest);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        UnityDebug.DrawLine(start, end, color, duration);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        UnityDebug.DrawLine(start, end, color);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void DrawLine(Vector3 start, Vector3 end)
    {
        UnityDebug.DrawLine(start, end);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
    {
        UnityDebug.DrawRay(start, dir, color, duration, depthTest);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        UnityDebug.DrawRay(start, dir, color);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void DrawRay(Vector3 start, Vector3 dir)
    {
        UnityDebug.DrawRay(start, dir);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
        UnityDebug.DrawRay(start, dir, color);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Break()
    {
        UnityDebug.Break();
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void DebugBreak()
    {
        UnityDebug.DebugBreak();
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(object message)
    {
        DebugExecution.AddLog(LogType.Log, message.ToString(), "ALL");
        DebugConsole.Log(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(object message, string messagegroup)
    {
        DebugExecution.AddLog(LogType.Log, message.ToString(), messagegroup);
        DebugConsole.Log(message, messagegroup);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(object message, UnityEngine.Object context)
    {
        DebugExecution.AddLog(LogType.Log, message.ToString(), "ALL");
        DebugConsole.Log(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(object message)
    {
        DebugExecution.AddLog(LogType.Error, message.ToString(), "ALL");
        DebugConsole.LogError(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(object message, string messagegroup)
    {
        DebugExecution.AddLog(LogType.Error, message.ToString(), messagegroup);
        DebugConsole.LogError(message, messagegroup);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(object message, UnityEngine.Object context)
    {
        DebugExecution.AddLog(LogType.Error, message.ToString(), "ALL");
        DebugConsole.LogError(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(object message)
    {
        DebugExecution.AddLog(LogType.Warning, message.ToString(), "ALL");
        DebugConsole.LogWarning(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(object message, string messagegroup)
    {
        DebugExecution.AddLog(LogType.Warning, message.ToString(), messagegroup);
        DebugConsole.LogWarning(message, messagegroup);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(object message, UnityEngine.Object context)
    {
        DebugExecution.AddLog(LogType.Warning, message.ToString(), "ALL");
        DebugConsole.LogWarning(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogCommand(object message)
    {
        DebugExecution.AddLog(LogType.Log, message.ToString(), "_Command");
        DebugConsole.LogCommand(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void SystemLog(string message, string stackTrace)
    {
        DebugExecution.AddSystemLog(LogType.Log, message, stackTrace, "ALL");
        DebugConsole.Log(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void SystemLogWarning(string message, string stackTrace)
    {
        DebugExecution.AddSystemLog(LogType.Warning, message, stackTrace, "ALL");
        DebugConsole.LogWarning(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void SystemLogError(string message, string stackTrace)
    {
        DebugExecution.AddSystemLog(LogType.Error, message, stackTrace, "ALL");
        DebugConsole.LogError(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void SystemLogException(string message, string stackTrace)
    {
        DebugExecution.AddSystemLog(LogType.Exception, message, stackTrace, "ALL");
        DebugConsole.LogError(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
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

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void WatcherClear()
    {
        DebugConsole.m_watchVarTable.Clear();
    }

    public static bool isDebugBuild
    {
#if DEBUG
        get { return true; }
#else
	  get { return false; }
#endif
    }

    public static void OnEnable()
    {
        //        Application.RegisterLogCallback(HandleLog);
    }

    public static void OnDisable()
    {
        //        Application.RegisterLogCallback(null);
    }

    public static void HandleLog(string logString, string stackTrace, LogType type)
    {
        //		switch(type)
        //		{
        //		case LogType.Log:
        //			SystemLog(logString, stackTrace);
        //			break;
        //			
        //		case LogType.Warning:
        //			SystemLogWarning(logString, stackTrace);
        //			break;
        //			
        //		case LogType.Error:
        //			SystemLogError(logString, stackTrace);
        //			break;
        //			
        //		case LogType.Exception:
        //			SystemLogException(logString, stackTrace);
        //			break;
        //		
        //		default:
        //			
        //			break;			
        //		}		
    }
}