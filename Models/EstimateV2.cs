namespace WebApiSample.Models;
// NOTA: comentarios de celdas segun presup. ARG, libro "N - Duchas Escocesas"
// ADVERTENCIA: No esta mapeada la tabla "comprobantes de pago / gastos locales"
public class EstimateV2
{   
    public EstimateHeaderDB estHeader{get;set;}
    public double totalfreight_cost{get;set;}
    public CONSTANTES constantes{get;set;} 
    public List<EstimateDetail> estDetails {get; set;}

    public EstimateV2()
    {
        this.estDetails=new List<EstimateDetail>();

        this.estHeader=new EstimateHeaderDB();
        this.constantes=new CONSTANTES();
    }
}