using System.Collections.Specialized;
using System.Configuration;

namespace IGApi.Common
{
    public static class Configuration
    {
        public static bool VerboseLog { get { return isVerboseLog(); } }

        public static int GetAllowedApiCallsPerMinute()
        {
            int allowedApiCallsPerMinute;

            if (ConfigurationManager.GetSection("QueueEngine") is NameValueCollection QueueEngine)
            {
                if (!int.TryParse(QueueEngine["IGAllowedApiCallsPerMinute"], out allowedApiCallsPerMinute))
                {
                    throw new InvalidOperationException("Could not parse the environment setting IGAllowedApiCallsPerMinute. Make sure the environment are set correctly. It should be an integer that represents the amount of api calls allowed to make per minute (typically 30 in live environment).");
                }
            }
            else
            {
                throw new InvalidOperationException("No environment settings found for QueueEngine. Make sure the referencing project has the App.config file with environment settings. You can use App.config from this project as template.");
            }

            return allowedApiCallsPerMinute;
        }

        private static bool isVerboseLog()
        {
            if (ConfigurationManager.GetSection("Settings") is NameValueCollection Settings)
            {
                if (!bool.TryParse(Settings["VerboseLog"], out bool isVerboseLog))
                {
                    throw new InvalidOperationException("Could not parse the environment setting VerboseLog. Make sure the environment are set correctly. It should be an boolean.");
                }

                return isVerboseLog;
            }
            else
            {
                throw new InvalidOperationException("No environment settings found for Settings. Make sure the referencing project has the App.config file with environment settings. You can use App.config from this project as template.");
            }
        }

        public struct WindowDimensions
        {
            public int width;
            public int height;
            public bool isLocked;
        }

        public static WindowDimensions GetWindowDimensions()
        {
            if (ConfigurationManager.GetSection("Settings") is NameValueCollection Settings)
            {
                if (!int.TryParse(Settings["WindowWidth"], out int windowWidth))
                {
                    throw new InvalidOperationException("Could not parse the environment setting WindowWidth. Make sure the environment are set correctly. It should be an integer.");
                }
                if (!int.TryParse(Settings["WindowHeight"], out int windowHeight))
                {
                    throw new InvalidOperationException("Could not parse the environment setting WindowHeight. Make sure the environment are set correctly. It should be an integer.");
                }
                if (!bool.TryParse(Settings["WindowLock"], out bool WindowLock))
                {
                    throw new InvalidOperationException("Could not parse the environment setting WindowLock. Make sure the environment are set correctly. It should be an boolean.");
                }

                return new WindowDimensions() { width = windowWidth, height = windowHeight, isLocked = WindowLock};
            }
            else
            {
                throw new InvalidOperationException("No environment settings found for Settings. Make sure the referencing project has the App.config file with environment settings. You can use App.config from this project as template.");
            }
        }
    }
}
