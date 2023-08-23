namespace WebApiSample.Models;

// Segun Version 3B 15Ago
public class TarifasDeposito
{
    public int id{ get; set; }              //PK
    public string description{ get; set; }
    public int depositos_id{ get; set; }    //FK a listado de depositos
    public int carga_id{ get; set; }        //FK a listado de tipo de cargas / contenedores
    public int paisregion_id{ get; set; }   //FK a listado de paises/regiones
    public int trucksemi_id{ get; set; }    //FK a listado de tipo de camion / remolque
    public double descarga{ get; set; }
    public double ingreso{ get; set; }
    public double total_ingreso{ get; set; }
    public double carga{ get; set; }
    public double armado{ get; set; }
    public double egreso{ get; set; }
    public double total_egreso{ get; set; }
    public double gasto_otro1{ get; set; }
    public double gasto_otro2{ get; set; }
    public string notas{ get; set; }
    public DateTime htimestamp{ get; set; }
 
}


public class TarifasDepositoVista
{
    public int id{ get; set; }              //PK
    public string description{ get; set; }
    public int depositos_id{ get; set; }    //FK a listado de depositos
    public int carga_id{ get; set; }        //FK a listado de tipo de cargas / contenedores
    public int paisregion_id{ get; set; }   //FK a listado de paises/regiones
    public int trucksemi_id{ get; set; }    //FK a listado de tipo de camion / remolque
    public double descarga{ get; set; }
    public double ingreso{ get; set; }
    public double total_ingreso{ get; set; }
    public double carga{ get; set; }
    public double armado{ get; set; }
    public double egreso{ get; set; }
    public double total_egreso{ get; set; }
    public double gasto_otro1{ get; set; }
    public double gasto_otro2{ get; set; }
    public string notas{ get; set; }
    public DateTime htimestamp{ get; set; }
    public string deposito {get;set;}
    public string freight {get;set;}
    public string pais {get;set;}
    public string semi{get;set;}

}


