using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using UnityDebug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class DebugExecution
{
	public static bool m_TimerAdd = false;	
	public static bool m_PlayStart = false;	
	public static string m_CurrentGroupName = "All";
	
	#if UNITY_EDITOR
	 private static string m_filePath = System.IO.Path.GetFullPath(".") + "/Log/_Temporary storage.log";
	#elif(UNITY_IOS || UNITY_ANDROID)	
	 private static string m_filePath = Application.persistentDataPath + "/Log/_Temporary storage.log";
	#else
	 private static string m_filePath = Application.dataPath + "/Log/_Temporary storage.log";
	#endif     
	
    public static void AddLog(LogType type, string message, string messagegroup)
    {	
        StackTrace stackTrace = new StackTrace(true);
		string settime = DateTime.Now.ToString("[hh:mm:ss.fff]");
		List<StackFrame> stackFrames = stackTrace.GetFrames().ToList();
		while (stackFrames.Count != 0 && (stackFrames.First().GetMethod().ReflectedType == typeof(Debug)||stackFrames.First().GetMethod().ReflectedType == typeof(DebugExecution)))
	    {
	        stackFrames.RemoveAt(0);
	    }
	    if (stackFrames.Count == 0) return;
		Container.Log msg = new Container.Log(type, messagegroup, message, stackFrames, settime, m_TimerAdd);
		Container.AddLog(msg);
		Container.AddGroup(messagegroup);
    }
	
	public static void AddSystemLog(LogType type, string message, string stack, string messagegroup)
    {	
		string settime = DateTime.Now.ToString("[hh:mm:ss.fff]");
		List<Container.FrameInfo> infos = new List<Container.FrameInfo>();
		int tSStartNum, tSEndNum, tBarNum, lineNumber;
		string fileName, methodName;
		if (stack == "")
		{
			tSStartNum = message.IndexOf(".cs") + 3;
			if (tSStartNum <= 3) tSStartNum = message.IndexOf(".js") + 3;
			
			tSEndNum  = message.IndexOf(",", tSStartNum);
			fileName = System.IO.Path.GetFullPath(".") + "/" + message.Substring(0, tSStartNum);
			lineNumber = int.Parse(message.Substring(tSStartNum +1, tSEndNum-(tSStartNum+1)));
			methodName = "";
		}else
		{
			tSStartNum  = stack.IndexOf(" (at ") + 5;
			tSEndNum  = stack.IndexOf(")", tSStartNum);
			tBarNum  = stack.IndexOf(":", tSStartNum)+1;
	        fileName = System.IO.Path.GetFullPath(".") + "/" + stack.Substring(tSStartNum, tBarNum-tSStartNum-1);
	        lineNumber = int.Parse(stack.Substring(tBarNum, tSEndNum-tBarNum));
			methodName = "";
		}
        infos.Add(new Container.FrameInfo(fileName, methodName, lineNumber));
		Container.Log msg = new Container.Log(type, messagegroup, message, infos, settime, m_TimerAdd);
		Container.AddLog(msg);
		Container.AddGroup(messagegroup);		
	}

    #region LogContainer
    public sealed class Container
    {
        [Serializable]
        public class Log
        {
            public Log(LogType type, string messagegroup, string message, List<FrameInfo> stackFrames, string settime, bool tview)
            {
                Type = type;
				MsgGroup = messagegroup;
                Text = message;
                Frames = stackFrames;
				Times = settime;
				TimeView = tview;
            }

            public Log(LogType type, string messagegroup, string message, List<StackFrame> stackFrames, string settime, bool tview)
            {
                Type = type;
				MsgGroup = messagegroup;
                Text = message;				
                Frames = new List<FrameInfo>();
                foreach (StackFrame frame in stackFrames)
                {
                    if (frame.GetFileName() != null)
                        Frames.Add(new FrameInfo(frame));
                }
				Times = settime;
				TimeView = tview;
            }

            public override string ToString()
            {
				if (m_TimerAdd)
				{
            		return string.Format("{0} {1}", Times, Text);
				}else{
					return string.Format("{0}", Text);	
				}
			}

            public LogType Type { get; set; }
			public string MsgGroup { get; set; }
            public string Text { get; set; }			
            public List<FrameInfo> Frames { get; set; }
			public string Times { get; set; }
			public bool TimeView { get; set; }
        }

        [Serializable]
        public class FrameInfo
        {
            public FrameInfo(StackFrame frame)
            {
                FileName = frame.GetFileName();
                MethodName = frame.GetMethod().Name;
                LineNumber = frame.GetFileLineNumber();
            }

            public FrameInfo(string fileName, string methodName, int lineNumber)
            {
                FileName = fileName;
                MethodName = methodName;
                LineNumber = lineNumber;
            }

            public override string ToString()
            {
                return string.Format("{1}, Line: {2}; File {0}", FileName, MethodName, LineNumber);
            }

            public string FileName { get; set; }
            public string MethodName { get; set; }
            public int LineNumber { get; set; }
        }

        public static event Action<Log> OnLogAdded;
        public static event Action OnLogsCleared;
		public static event Action<string> OnAddGroup;
		
        public static Log GetLog(int index)
        {
            return Logs[index];
        }

        public static void AddLog(Log item)
        {
            Logs.Add(item);
            SaveLog(item);
            if (OnLogAdded != null) OnLogAdded(item);
        }

        public static int GetLength()
        {
           	return Logs.Count;			
        }

        public static void Clear()
        {
            m_logs = new List<Log>();
            Save();
            if (OnLogsCleared != null) OnLogsCleared();
        }
		
		public static void AddGroup(string groupData)
        {
            if (OnAddGroup != null) OnAddGroup(groupData);
        }

		public static bool IsTimeView {
		get { return m_TimerAdd; }
		set { m_TimerAdd = value; }
		}
		
		public static bool IsPlay {
		get { return m_PlayStart; }
		set { m_PlayStart = value; }
		}
	
        private static List<Log> Logs
        {
            get
            {
                if (m_logs == null)
                {					
                    Load();
                }
                return m_logs;
            }
        }

        private static void SaveLog(Log newLog)
        {
			DirectoryInfo dir;			
			#if UNITY_EDITOR
				dir = new DirectoryInfo(System.IO.Path.GetFullPath(".") + "/Log");
			#elif(UNITY_IOS || UNITY_ANDROID)	
				dir = new DirectoryInfo(Application.persistentDataPath + "/Log");
			#else
				dir = new DirectoryInfo(Application.dataPath + "/Log");
			#endif
			
       		if (dir.Exists == false) dir.Create();
			
            FileStream fs = new FileStream(m_filePath, FileMode.Append);
            StreamWriter swriter = new StreamWriter(fs);
            try
            {
                swriter.WriteLine("[Log]");
				swriter.WriteLine(newLog.Times);
				swriter.WriteLine(newLog.MsgGroup);
                swriter.WriteLine(newLog.Text);
                swriter.WriteLine(newLog.Type.ToString());
                foreach (FrameInfo frameInfo in newLog.Frames)
                {
                    swriter.WriteLine(frameInfo.ToString());
                }
                swriter.WriteLine();
            }
            catch { }
            finally
            {
                swriter.Close();
            }
        }

        private static void Save()
        {
            if (File.Exists(m_filePath))
            {
                File.Delete(m_filePath);
            }
            foreach (Log log in m_logs)
            {
                SaveLog(log);
            }
        }

        private static void Load()
        {
            m_logs = new List<Log>();
			if (IsPlay) return;
			
            if (!File.Exists(m_filePath))
            {
                return;
            }
            FileStream fs = new FileStream(m_filePath, FileMode.Open);
            StreamReader sreader = new StreamReader(fs);
            string line = "";
            while (!sreader.EndOfStream)
            {
                line = sreader.ReadLine();
                if (line == "[Log]")
                {
					string times = sreader.ReadLine();
					string messagegroup = sreader.ReadLine();
                    string text = sreader.ReadLine();
					
					try 
					{
                  		LogType type = (LogType)Enum.Parse(typeof(LogType), sreader.ReadLine());
					
                    List<FrameInfo> infos = new List<FrameInfo>();
					char[] splitters = { ',', ';' };
                    while ((line = sreader.ReadLine()) != "")
                    {
                        string[] parts = line.Split(splitters);
                        string fileName = parts.Last().ToString().Substring(6);
                        string methodName = parts[0];						
                        int lineNumber = int.Parse(parts[1].ToString().Substring(7));
                        infos.Add(new FrameInfo(fileName, methodName, lineNumber));
                    }
                    Log log = new Log(type, messagegroup, text, infos, times, true);
                    m_logs.Add(log);
					}catch(ArgumentException)
					{
						
					}
                }
            }
			fs.Dispose();
        }
		
        private static Container m_container;
        private static List<Log> m_logs;
    }
    #endregion LogContainer
}