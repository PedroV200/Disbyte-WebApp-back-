using System.Xml;

namespace WebApiSample.Models;
// NOTA: comentarios de celdas segun presup. ARG, libro "N - Duchas Escocesas"
// ADVERTENCIA: No esta mapeada la tabla "comprobantes de pago / gastos locales"


// LISTED 22_09_2023 
// Se necesita mostrar en el detail datos como el NCM y proveedor,  que cuando se hace un get, son solo IDs
// Del mismo modo en el header se necesita saber pais orig, pais dest y carga. Solo llegan IDs.
// Se crea para header y detail tipos vista que adicionan estos datos y surgen de un query unico usando joins
// que es mucho mas eficiente.
//
// Para evitar una seria cascada de cambios, header y detail se mantienen sin cambios.
//
// SOLO PARA CUANDO SE HACE UNA CONSULTA TIPO VISTA - PRESUP_RECLAIM
//
// Los detalles que se adicionan a cada Estimate detail, se mandan a una lista separada, se igual dimencion que el estimate detail
// La lista se publica en el json (para todos los casos)
// Para alojar los datos adicionaes del header, se usaron campos sueltos FUERA del mismo. Esto permite que el EstivameteV2
// en estructura Header sea %100 compatible con el tipo de dato de la DB.



public class EstimateV2
{   
    public EstimateHeaderDB estHeader{get;set;}
    public List<EstimateDetailVistaAdditionalData> estDetAddData{get;set;}
    

    
    public string pais;
    public double totalfreight_cost;
    //public double freight_insurance_cost;
    public Carga miCarga;
    public Tarifas misTarifas;

    public string carga_str{get;set;}
    public string paisdest{get;set;}
    public string paisorig{get;set;}

    public int updated;
    public CONSTANTES constantes;
    public List<EstimateDetail> estDetails {get; set;}

    public EstimateV2()
    {
        this.estDetails=new List<EstimateDetail>();
        this.estHeader=new EstimateHeaderDB();
        this.estDetAddData=new List<EstimateDetailVistaAdditionalData>();
        this.constantes=new CONSTANTES();
        this.misTarifas=new Tarifas();
        this.miCarga=new Carga();
    }
}