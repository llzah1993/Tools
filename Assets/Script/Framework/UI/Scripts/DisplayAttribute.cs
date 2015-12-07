using UnityEngine;
using System.Collections;
using System;
using Framework;

namespace Framework
{
    public class DisplayAttribute : Attribute
    {
        private Type _type;
        public Type type
        {
            get { return _type; }
            set { _type = value; }
        }
        private string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { _displayname = value; }
        }
        public DisplayAttribute(Type _type, string displayname)
        {
            this._type = _type;
            this._displayname = displayname;
        }

    }
}


