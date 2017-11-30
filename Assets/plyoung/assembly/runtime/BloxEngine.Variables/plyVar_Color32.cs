using System;
using UnityEngine;

namespace BloxEngine.Variables
{
    [Serializable]
    public class plyVar_Color32 : plyVarValueHandler
    {
        [SerializeField]
        public Color32 storedValue;

        public override Type variableType
        {
            get
            {
                return typeof(Color32);
            }
        }

        public override object GetValue(plyVar wrapper)
        {
            return this.storedValue;
        }

        public override void SetValue(plyVar wrapper, object v)
        {
            if (v == null)
            {
                throw new Exception("Can't convert null to Color32.");
            }
            if (v.GetType() == typeof(Color32))
            {
                this.storedValue = (Color32)v;
            }
            else if (v.GetType() == typeof(Color))
            {
                this.storedValue = (Color)v;
            }
            else
            {
                if (v.GetType() != typeof(Vector4))
                {
                    throw new Exception("Can't convert " + v.GetType().Name + " to Color32.");
                }
                this.storedValue = (Color32)v;
            }

        }

        public override void ClearValue(plyVar wrapper)
        {
            this.storedValue = default(Color32);
        }

        public Color32 GetValue()
        {
            return this.storedValue;
        }

        public void SetValue(object value)
        {
            this.SetValue(null, value);
        }
    }
}
