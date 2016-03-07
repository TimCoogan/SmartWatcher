using System;

namespace SmartWatcher.PlugIn
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlugDescriptionAttribute : System.Attribute
    {

        private string _description;

        public PlugDescriptionAttribute(string Description)
            : base()
        {

            _description = Description;

            return;

        }

        public override string ToString()
        {
            return _description;
        }

    }
}
