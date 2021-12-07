﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Common
{
    public static class Configuration
    {
        public static int GetAllowedApiCallsPerMinute()
        {
            int allowedApiCallsPerMinute;

            //TODO: Rename IGRestRequestQueueEngine in ConfigurationManager
            if (ConfigurationManager.GetSection("IGRestRequestQueueEngine") is NameValueCollection IGRestRequestQueueEngine)
            {
                if (!int.TryParse(IGRestRequestQueueEngine["IGAllowedApiCallsPerMinute"], out allowedApiCallsPerMinute))
                {
                    throw new InvalidOperationException("Could not parse the environment setting IGAllowedApiCallsPerMinute. Make sure the environment are set correctly. It should be an integer that represents the amount of api calls allowed to make per minute (typically 30 in live environment).");
                }
            }
            else
            {
                throw new InvalidOperationException("No environment settings found for IGRestRequestQueueEngine. Make sure the referencing project has the App.config file with environment settings. You can use App.config from this project as template.");
            }

            return allowedApiCallsPerMinute;
        }
    }
}