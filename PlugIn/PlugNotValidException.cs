namespace SmartWatcher.PlugIn
{
    public class PlugNotValidException : System.Exception
    {

        public PlugNotValidException(System.Type type, string Message)
            : base("The plug-in " + type.Name + " is not valid\n" + Message)
        {
            return;
        }
    }
}
