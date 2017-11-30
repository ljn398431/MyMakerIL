using BloxEngine;
using plyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxGameSystems
{
    [Serializable]
    [ExcludeFromBlox]
    public class GroupedData<GroupT, ItemT> : ScriptableObject, IIdxIdConverter, ISerializationCallbackReceiver where GroupT : GroupedDataGroup<ItemT> where ItemT : GroupedDataItem
    {
        [SerializeField]
        public List<GroupT> groups = new List<GroupT>();

        [SerializeField]
        private int lastGroupId;

        [SerializeField]
        private int lastItemId;

        [NonSerialized]
        private Dictionary<int, GroupedDataItem> itemCache;

        [NonSerialized]
        private Dictionary<string, int> identIdMap;

        [NonSerialized]
        private GUIContent[] labelsCache;

        public List<ItemT> GetAllItems()
        {
            List<ItemT> list = new List<ItemT>();
            for (int i = 0; i < this.groups.Count; i++)
            {
                list.AddRange((IEnumerable<ItemT>)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items);
            }
            return list;
        }

        public void RemoveGroup(int idx)
        {
            this.groups.RemoveAt(idx);
        }

        public ItemT GetItem(int id)
        {
            if (id < 0)
            {
                return null;
            }
            this.BuildCaches();
            GroupedDataItem groupedDataItem = null;
            this.itemCache.TryGetValue(id, out groupedDataItem);
            return (ItemT)groupedDataItem;
        }

        public ItemT GetItem(string ident)
        {
            if (string.IsNullOrEmpty(ident))
            {
                return null;
            }
            this.BuildCaches();
            return this.GetItem(this.IdentToId(ident));
        }

        public int IdentToId(string ident)
        {
            if (string.IsNullOrEmpty(ident))
            {
                return -1;
            }
            this.BuildCaches();
            int result = -1;
            if (this.identIdMap.TryGetValue(ident, out result))
            {
                return result;
            }
            return -1;
        }

        public string IdToIdent(int id)
        {
            if (id >= 0)
            {
                for (int i = 0; i < this.groups.Count; i++)
                {
                    for (int j = 0; j < ((GroupedDataGroup<ItemT>)(object)this.groups[i]).items.Count; j++)
                    {
                        if (((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).id == id)
                        {
                            return ((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).ident;
                        }
                    }
                }
            }
            return "-invalid-";
        }

        private void BuildCaches()
        {
            if (this.itemCache == null)
            {
                this.itemCache = new Dictionary<int, GroupedDataItem>();
                this.identIdMap = new Dictionary<string, int>();
                for (int i = 0; i < this.groups.Count; i++)
                {
                    for (int j = 0; j < ((GroupedDataGroup<ItemT>)(object)this.groups[i]).items.Count; j++)
                    {
                        this.itemCache.Add(((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).id, (GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]);
                        this.identIdMap.Add(((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).ident, ((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).id);
                    }
                }
            }
        }

        public bool GroupNameIsUnique(string name)
        {
            name = name.ToLower();
            for (int i = 0; i < this.groups.Count; i++)
            {
                if (name.Equals(((GroupedDataGroup<ItemT>)(object)this.groups[i]).ident.ToLower()))
                {
                    return false;
                }
            }
            return true;
        }

        public bool ItemNameIsUnique(string name)
        {
            name = name.ToLower();
            for (int i = 0; i < this.groups.Count; i++)
            {
                for (int j = 0; j < ((GroupedDataGroup<ItemT>)(object)this.groups[i]).items.Count; j++)
                {
                    if (name.Equals(((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).ident.ToLower()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string GetUniqueItemName(string name)
        {
            string text = name;
            bool flag = false;
            int num = 1;
            while (!flag)
            {
                if (this.ItemNameIsUnique(text))
                {
                    flag = true;
                }
                else
                {
                    text = name + "_" + num;
                    num++;
                }
            }
            return text;
        }

        public GroupT CreateGroup(string name)
        {
            this.lastGroupId++;
            GroupT val = Activator.CreateInstance<GroupT>();
            ((GroupedDataGroup<ItemT>)(object)val).id = this.lastGroupId;
            ((GroupedDataGroup<ItemT>)(object)val).ident = name;
            this.groups.Add(val);
            return val;
        }

        public ItemT CreateItem(string name, GroupT group)
        {
            this.lastItemId++;
            ItemT val = Activator.CreateInstance<ItemT>();
            ((GroupedDataItem)(object)val).id = this.lastItemId;
            ((GroupedDataItem)(object)val).ident = name;
            ((GroupedDataGroup<ItemT>)(object)group).items.Add(val);
            return val;
        }

        public int CreateItemId()
        {
            this.lastItemId++;
            return this.lastItemId;
        }

        public GUIContent[] Labels()
        {
            if (this.labelsCache == null)
            {
                int num = 0;
                List<GUIContent> list = new List<GUIContent>();
                for (int i = 0; i < this.groups.Count; i++)
                {
                    for (int j = 0; j < ((GroupedDataGroup<ItemT>)(object)this.groups[i]).items.Count; j++)
                    {
                        list.Add(new GUIContent(((GroupedDataGroup<ItemT>)(object)this.groups[i]).ident + "/" + ((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).ident));
                        ((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j])._idx = num;
                        num++;
                    }
                }
                this.labelsCache = list.ToArray();
            }
            return this.labelsCache;
        }

        public void EntriesDirty()
        {
            this.labelsCache = null;
            this.itemCache = null;
            this.identIdMap = null;
        }

        public int IdToIdx(int id)
        {
            GroupedDataItem groupedDataItem = (GroupedDataItem)(object)this.GetItem(id);
            if (groupedDataItem == null)
            {
                return -1;
            }
            return groupedDataItem._idx;
        }

        public int IdxToId(int idx)
        {
            int num = 0;
            for (int i = 0; i < this.groups.Count; i++)
            {
                for (int j = 0; j < ((GroupedDataGroup<ItemT>)(object)this.groups[i]).items.Count; j++)
                {
                    if (num == idx)
                    {
                        return ((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).id;
                    }
                    num++;
                }
            }
            return -1;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            for (int i = 0; i < this.groups.Count; i++)
            {
                for (int j = 0; j < ((GroupedDataGroup<ItemT>)(object)this.groups[i]).items.Count; j++)
                {
                    ((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).meta.Deserialize(true);
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            for (int i = 0; i < this.groups.Count; i++)
            {
                for (int j = 0; j < ((GroupedDataGroup<ItemT>)(object)this.groups[i]).items.Count; j++)
                {
                    ((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groups[i]).items[j]).meta.Serialize();
                }
            }
        }
    }
}
