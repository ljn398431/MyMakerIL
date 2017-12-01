using UnityEngine;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

public class WatcherExecution
{
	public static void AddWatcher(string watcherName, object watcherValue)
	{
		Container.Watcher tWatcher = new Container.Watcher(watcherName, watcherValue);
		Container.WatchAdd(tWatcher);
	}

    #region WatcherContainer
    public sealed class Container
    {
		private static Container m_container;
        private static List<Watcher> m_Watchers;
		
        [Serializable]
        public class Watcher
        {
            public Watcher(string watcherName, object watcherValue)
            {
                Name = watcherName;
				Value = watcherValue;
            }

            public override string ToString()
            {
				return string.Format("{0}", Value);	
			}

            public string Name { get; set; }
			public object Value { get; set; }
        }

        public static event Action<Watcher> OnWatchAdd;
        public static event Action OnWatchCleared;

        public static void WatchAdd(Watcher item)
        {
			m_Watchers.Add(item);
            if (OnWatchAdd != null) OnWatchAdd(item);
        }

		public static void Clear()
        {
            m_Watchers = new List<Watcher>();
            if (OnWatchCleared != null) OnWatchCleared();
        }

        private static List<Watcher> Watchers
        {
            get
            {
                if (m_Watchers == null)
                {					
                    m_Watchers = new List<Watcher>();
                }
                return m_Watchers;
            }
        }
    }
    #endregion WatcherContainer
}