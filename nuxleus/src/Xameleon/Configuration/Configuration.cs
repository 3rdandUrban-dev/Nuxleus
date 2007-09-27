// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace Xameleon.Configuration
{

    public class AppSettings
    {

        public String GetSetting(String keyName)
        {
            NameValueCollection appSettings = WebConfigurationManager.AppSettings as NameValueCollection;
            IEnumerator appSettingsEnum = appSettings.GetEnumerator();
            int i = 0;

            try
            {
                while (appSettingsEnum.MoveNext())
                {
                    string key = appSettings.AllKeys[i].ToString();
                    if (key == keyName)
                    {
                        return appSettings[key];
                    }
                    i += 1;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return null;
        }

        public Hashtable GetSettingArray(Hashtable table, String keyName)
        {
            NameValueCollection appSettings = WebConfigurationManager.AppSettings as NameValueCollection;
            IEnumerator appSettingsEnum = appSettings.GetEnumerator();
            int i = 0;

            try
            {
                while (appSettingsEnum.MoveNext())
                {
                    string key = appSettings.AllKeys[i].ToString();
                    if (key.StartsWith(keyName))
                    {
                        table[key.Substring(keyName.Length)] = appSettings[key];
                    }
                    i += 1;
                }
            }
            catch (Exception e)
            {
                table["ERROR"] = e.Message;
                return table;
            }

            return table;
        }
    }
}
