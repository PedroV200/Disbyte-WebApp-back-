using WebApiSample.Models;
using WebApiSample.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


// Una clase controla los claims y los convierte en nemonicos. Luego genera un string con los mismos
// y un metod permite chequear si esta tal o cual permiso. 

public class Cliente
{
    // Nemonico de salida
    public  String finanzas { get { return "FIN";} } 
    // Etiqueta del permiso (permiso actual de Auth0)
    public  String permiso_finanzas { get { return "update_presup:jefe_area_finanzas";} } 
    // ... Repetir x cada permiso. Definir nemonico, y definir el label.
    public  String comex { get { return "COM";} } 
    public  String permiso_comex { get { return "update_presup:jefe_area_comex";} } 
    public String sourcing { get { return "SRC";} } 
    public String permiso_sourcing { get { return "update_presup:jefe_area_sourcing";} }
    public String boss { get { return "BSS";} } 
    public String permiso_boss { get { return "update_presup:superuser";} } 

    public string getClientPermisos(List<Claim> clientClaims)
    {
        string client=null;
        if(clientClaims.ToList().Find(c => c.Type == "permissions" && c.Value==permiso_finanzas)!=null)
        {
            client=finanzas;
        }
        if(clientClaims.ToList().Find(c => c.Type == "permissions" && c.Value==permiso_comex)!=null)
        {   // Si hay mas de un permiso, sumo los nemonicos
            client+=comex;
        }
        if(clientClaims.ToList().Find(c => c.Type == "permissions" && c.Value==permiso_sourcing)!=null)
        {
            client+=sourcing;
        }
        if(clientClaims.ToList().Find(c => c.Type == "permissions" && c.Value==permiso_boss)!=null)
        {   // A boos no lo sumo, con nada, lo asigno
            client=boss;
        }

        return client;
    }

    public bool isGranted(string permisos,string permiso)
    {
        if(permisos.Contains(permiso))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
