using System;

namespace SmartWatcher.PlugIn
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlugDisplayNameAttribute : System.Attribute
    {

        private string _displayName;

        public PlugDisplayNameAttribute(string DisplayName)
            : base()
        {

            _displayName = DisplayName;

            return;

        }


        public override string ToString()
        {
            return _displayName;
        }

    }
}
