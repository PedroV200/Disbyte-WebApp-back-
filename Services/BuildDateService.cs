using System;
using System.Globalization;
using System.Reflection;
using WebApiSample.Infrastructure;


class BuildDateService: IBuilDateService
{

        public string getBuildDate()
        {
            var buildTime = GetLinkerTime(Assembly.GetEntryAssembly());
            string tmp="(c)Disbyte 2023, BUILD: (FECHA: "+buildTime.Date.ToString("yyyy-MM-dd")+" - HORA: "+buildTime.Hour.ToString()+":"+buildTime.Minute.ToString()+":"+buildTime.Second.ToString()+")";
            return tmp;//+buildTime.Date.ToString("yyyy-MM-dd")+" "+buildTime.TimeOfDay.ToString("HH:mm:ss");
        }

        public static DateTime GetLinkerTime(Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value[(index + BuildVersionMetadataPrefix.Length)..];
                    return DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss:fffZ", CultureInfo.InvariantCulture);
                }
            }
            return default;
        }

}