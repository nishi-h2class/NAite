using NLog;

namespace NAiteWebApi.Libs
{
    public class Logs
    {
        public static Logger Logger { get; private set; } = null!;

        static Logs()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }
    }
}
