using System;
using System.Globalization;
using System.Reflection;
using WebApiSample.Infrastructure;


class BuildDateService: IBuilDateService
{

        public string getBuildDate()
        {
            var buildTime = GetLinkerTime(Assembly.GetEntryAssembly());
            string tmp="(c)Disbyte 2023, BUILD: (FECHA: " +buildTime.Date.ToString("yyyy-MM-dd")+" - HORA: "+buildTime.Hour.ToString("00")+":"+buildTime.Minute.ToString("00")+":"+buildTime.Second.ToString("00")+")"
                        +"\r\n"+
@$"
MEMORIA DE CAMBIOS:
31_10_2023: Se crea un entindad especifica para el tarifario de MEX en lugar de distribuir 
el tarifario entre las 8 tables dado que no permite la importacion ni mostrar historicos
06_11_2023: Se sobreescrive el connection string de appsetings.json con una variable de entorno
(https://gparlakov.medium.com/the-confusion-of-asp-net-configuration-with-environment-variables-c06c545ef732 )  
La totalgastos_loc_y_extra en detail se usar solo para alojar la sumar de los extrag. 
Se agrega calculo de los extrag_grand_total que antes se hacia en el front. 


                                ";
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