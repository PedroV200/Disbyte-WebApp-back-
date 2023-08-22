namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;                                            

// 3_8_2023 SEGUNDA VERSION para "multiregion" basado en el sheet de MEX y ARG WIP

public class EstimateService: IEstimateService
{
    IUnitOfWork _unitOfWork;
    TarifasByDate misTarifasByDate;
    Carga miContenedor;

    string haltError;
    public EstimateService(IEstimateDetailService estDetailServices,IUnitOfWork unitOfWork, ICnstService constService)
    {
       _estDetServices=estDetailServices;
       _unitOfWork=unitOfWork;
       _cnstService=constService;

    }
    public IEstimateDetailService _estDetServices {get;}
    public ICnstService _cnstService {get;}

// ADVERTENCIA: Esta funcion es equi a init(), debe llamarse antes que cualquier cuenta de modo que las constantes
// esten todas populadas.
// Se encarga de obtener las constantes que se usan en diverso calculos desde la tabla constantes.
// Le pasa estas constantes tmb a estDetailService.
    /*public async Task<EstimateV2> loadConstants(EstimateV2 est)
    {
        est.constantes=await _cnstService.getConstantesLastVers();
        if(est.constantes==null)
        {
            haltError="No existe una instancia de constantes en tabla";
            return null;
        }
        // Le paso las constantes a estDetailService tmb.
        _estDetServices.loadConstants(est.constantes);
        return est;
        
    }*/

    public void loadConstants(CONSTANTES miConst)
    {
        _estDetServices.loadConstants(miConst);
    }

    public EstimateV2 CalcPesoTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.totalgw=_estDetServices.CalcPesoTotal(ed); 
            if(ed.totalgw<0)
            {
                haltError=$"ATENCION: El articulo modelo '{ed.modelo}' tiene cant pcs x caja = 0. DIV 0 !";
                return null;
            }      
        }
        return est;
    }



    public EstimateV2 CalcFobTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.totalfob=_estDetServices.CalcFob(ed);       
        }
        return est;
    }

    public EstimateV2 CalcCbmTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.totalcbm=_estDetServices.CalcCbmTotal(ed);   
            if(ed.totalcbm<0)
            {
                haltError=$"ATENCION: El articulo '{ed.modelo}' tiene cant pcs por caja = 0. DIV 0 !";
                return null;
            }    
        }
        return est;
    }

    public EstimateV2 CalcFleteTotalByProd(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.freightCharge=_estDetServices.CalcFlete(ed,est.totalfreight_cost,est.estHeader.fob_grandtotal); 
            if(ed.freightCharge<0)
            {
                return null;
            }      
        }
        return est;
    }

    public EstimateV2 CalcSeguro(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {                                               // CELDA C5=0.1*C3
            ed.insuranceCharge=_estDetServices.CalcSeguro(ed,est.estHeader.freight_insurance_cost,est.estHeader.fob_grandtotal);   
            if(ed.insuranceCharge<0)
            {
                return null;
            }    
        }
        return est;
    }

    public EstimateV2 CalcCif(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {                                               
            ed.totalcif=_estDetServices.CalcCif(ed);
        }
        return est;
    }

    // En las columnas P y Q no se hace nada actualmente. 
    // El resultado de los ajsutes es la columna R designada como ValorAduanaEndivisa
    public EstimateV2 CalcAjusteIncDec(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {                                               
            ed.totalcif=_estDetServices.CalcValorEnAduanaDivisa(ed);
        }
        return est;
    }

    /*public async Task<EstimateV2> searchNcmDie(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {                                               
            ed.ncm_arancelgrav=(await _estDetServices.lookUpDie(ed))/100.0;
        }
        return est;
    }*/

    public EstimateV2 CalcDerechos(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {                                               
            ed.arancelgrav_cif=_estDetServices.CalcDerechos(ed);
        }
        return est;
    }  

    public EstimateV2 CalcTasaEstad061(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {                                               
            ed.te_dta_otro_cif=_estDetServices.CalcTasaEstad061(ed);
        }
        return est;
    }  

    public EstimateV2 CalcBaseGcias(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {                                               
            ed.baseiva=_estDetServices.CalcBaseIvaGcias(ed);
        }
        return est;
    }  


    public EstimateV2 CalcIVA415(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.iva_cif=_estDetServices.CalcIVA415(ed);
        }
        return est;
    }

     public async Task<EstimateV2> search_NCM_DATA(EstimateV2 est)
    {
        NCM myNCM=new NCM();

        foreach(EstimateDetail ed in est.estDetails)
        {  
           myNCM=await _estDetServices.lookUp_NCM_Data(ed); 
           if(myNCM==null)
           {    // Logeo que NCM / Articulo fallo
                haltError=$"FALLO NCM='{ed.ncm}', DET= '{ed.modelo}";
                return null;
           }
           ed.ncm_arancelgrav=myNCM.die/100.0;
           //ed.ncm_te_dta_otro=myNCM.te/100.0;
           ed.ncm_te_dta_otro=_estDetServices.CalcTE(myNCM.te)/100.0;
           ed.ncm_iva=myNCM.iva/100.0;
           ed.ncm_ivaad=myNCM.iva_ad/100.0; 
        }
        return est;
    }

    public EstimateV2 CalcIVA_ad_Gcias(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.ivaad_cif=_estDetServices.CalcIvaAdic(ed,est.estHeader.ivaexcento);
        }
        return est;
    }

    public EstimateV2 CalcImpGcias424(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gcias424=_estDetServices.CalcImpGcias424(ed);
        }
        return est;
    }

    public EstimateV2 CalcIIBB900(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.iibb900=_estDetServices.CalcIIBB(ed, est.estHeader.iibbtotal);
        }
        return est;
    }

    public EstimateV2 CalcPrecioUnitUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.preciounit_uss=_estDetServices.CalcPrecioUnitUSS(ed);
            if(ed.preciounit_uss<0)
            {
                haltError=$"ATENCION: Articulo '{ed.modelo}' tiene can pcs = 0. DIV 0 !";
                return null;
            }
        }
        return est;
    }

    public EstimateV2 CalcPagado(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.totalaranceles=_estDetServices.CalcPagado(ed);
        }
        return est;
    }

    public EstimateV2 CalcFactorProdTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.factorproducto=_estDetServices.CalcFactorProducto(ed,est.estHeader.fob_grandtotal);   
            if(ed.factorproducto<0)
            {
                haltError=$"ATENCION: El articulo '{ed.modelo}' tiene un FOB TOT de 0. Div 0!";
                return null;
            }    
        }
        return est;
    }

    public double sumPesoTotal(EstimateV2 est)
    {
        double tmp=0;
        foreach(EstimateDetail ed in est.estDetails)
        {
            tmp+=ed.totalgw;
        }
        return tmp;
    }

    public double sumFobTotal(EstimateV2 est)
    {
        double tmp=0;
        foreach(EstimateDetail ed in est.estDetails)
        {
            tmp+=ed.totalfob;
        }
        return tmp;
    }

    public EstimateV2 CalcCbmGrandTotal(EstimateV2 est)
    {
        double tmp=0;
        foreach(EstimateDetail ed in est.estDetails)
        {
            tmp+=ed.totalcbm;
        }
        est.estHeader.cbm_grandtotal=tmp;
        return est;
    }

    public EstimateV2 CalcCifTotal(EstimateV2 est)
    {
        double tmp=0;
        foreach(EstimateDetail ed in est.estDetails)
        {
            tmp+=ed.totalcif;
        }
        est.estHeader.cif_grandtotal=tmp;
        return est;
    }

    /*public async Task<double> lookUpTarifaFleteCont(EstimateV2 est)
    {
        TarifasFwdCont myTarCont=null;//await _unitOfWork.TarifasFwdContenedores.GetByFwdContTypeAsync(est.estHeader.freight_fwd,est.estHeader.freight_type); 

        if(myTarCont!=null)
        { 
            return myTarCont.costoflete060;
        }
        return -1;
    }*/


    
// Hace las cuentas de la tabla inferior del presupuestador, gastos locales / proyectados.
// Los devuelve en dolarbillete. CELDA D59
    public async Task<EstimateV2> calcularGastosProyecto(EstimateV2 miEst)
    {
        double tmp;
        double result;

       //TarifasByDate myTbd=await _unitOfWork.TarifasPorFecha.GetByIdAsync(miEst.estHeader.tarifasbydateid);
        // Ahora se maneja todo por FK ... para saber el tipo de contenedor necesito consultar la tabla contenedores
       //Contenedor myCont=await _unitOfWork.Contenedores.GetByIdAsync(miEst.estHeader.freight_type);

        tmp=await calcularGastosFwd(miEst,misTarifasByDate,miContenedor);
        if(tmp<0)
        {   // Todos los metodos que consultan una tabla tienen opcion de devolver -1 si algo no salio bien.
            haltError="FALLA CALCULAR GASTOS FWD. TABLA TarifasFWD no accesible o no existen datos para el tipo de contenedor / origen indicados";
            return null;
        }
        // Registro el gasto en cada articulo (ponderado por el factor de producto)
        miEst=Calc_GLOC_FWD_POND(miEst,tmp);
        tmp=await calcularGastosTerminal(miEst,misTarifasByDate);
        if(tmp<0)
        {
            haltError="FALLA AL CALCULAR GASTOS DE TERMINAL. TAbla no accesible o no existen datos para el tipo de contenedor ingresado";
            return null;
        }
        // Registro el gasto en cada articulo
        miEst=Calc_GLOC_TERMINAL_POND(miEst,tmp);

        tmp=await calcularGastosDespachante(miEst);
        if(tmp<0)
        {

            return null;
        }
        miEst=Calc_GLOC_DESPACHANTE_POND(miEst,tmp);

        tmp=await calcularGastosTteLocal(miEst,misTarifasByDate,miContenedor);
        if(tmp<0)
        {
            haltError="FALLA AL CALCULAR LOS GASTOS DE TTE LOC. Tabla de tarifa no accesible o no existen datos para el contenedor ingresado";
            return null;
        }
        miEst=Calc_GLOC_FLETE_POND(miEst,tmp);

        tmp=await calcularGastosCustodia(miEst,misTarifasByDate);

        if(tmp<0)
        {
            haltError="FALLO AL CALCULAR LOS GASTOS DE CUSTODIA. Tabla de tarifa no accesible o no existen datos para el Proveedor de Poliza ingresado";
            return null;
        }
        miEst=Calc_GLOC_CUSTODIA_POND(miEst,tmp);


        tmp=calcularGastosGestDigDocs(miEst);       // Este metodo no involucra una consulta a tabla, tmp np puede ser negativo
        miEst=Calc_GLOC_GESTDIGDOC_POND(miEst,tmp);

        tmp=calcularGastosBancarios(miEst);         // Este idem.
        miEst=Calc_GLOC_BANCARIO_POND(miEst,tmp);


        return miEst;

    }


// Rutinas que distribuyen a lo largo de todos los art del detail los diferentes gastos locales:
// Fowarding (0.50 Freight)
// Terminal
// Despachante
// TteLocal
// Custodia
// Digitalizaion Documental
// Bancarios
/////////////////////////////////
    public EstimateV2 Calc_GLOC_FWD_POND(EstimateV2 est,Double gastoFWD)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_fwd=_estDetServices.CalcGastosProyPond(ed,gastoFWD);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_TERMINAL_POND(EstimateV2 est,Double gastoTerminal)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_terminal=_estDetServices.CalcGastosProyPond(ed,gastoTerminal);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_DESPACHANTE_POND(EstimateV2 est,Double gastoDespachante)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_despachante=_estDetServices.CalcGastosProyPond(ed,gastoDespachante);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_FLETE_POND(EstimateV2 est,Double gastoFlete)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_flete=_estDetServices.CalcGastosProyPond(ed,gastoFlete);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_CUSTODIA_POND(EstimateV2 est,Double gastoCustodia)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_cust=_estDetServices.CalcGastosProyPond(ed,gastoCustodia);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_BANCARIO_POND(EstimateV2 est,Double gastoBanco)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_banco=_estDetServices.CalcGastosProyPond(ed,gastoBanco);       
        }
        return est;
    }

        public EstimateV2 Calc_GLOC_GESTDIGDOC_POND(EstimateV2 est,Double gastoBanco)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_sp1=_estDetServices.CalcGastosProyPond(ed,gastoBanco);       
        }
        return est;
    }
 // Fin rutinas de ponderacion de gastos x producto.   

//###########################################################################
//Funciones de calculo de los gastos locales:
// Fowarding (0.50 Freight)
// Terminal
// Despachante
// TteLocal
// Custodia
// Digitalizaion Documental
// Bancarios
////////////////////////////////////////
public async Task<double> calcularGastosFwd(EstimateV2 miEst,TarifasByDate myTar,Carga myCont)
    {   
        if(miEst==null)
        {
            return -1;
        }
        if(myTar==null)
        {
            return -1;
        }
        if(myCont==null)
        {
            return -1;
        }
        
        if(myCont.description=="LCL")
        {
            return (myTar.freight_cost*miEst.constantes.CNST_FREIGHT_PORCT_ARG*miEst.estHeader.cbm_grandtotal*miEst.estHeader.dolar)+myTar.freight_gastos1*miEst.estHeader.dolar;
        }
        else
        {
            return ((myTar.freight_cost*miEst.constantes.CNST_FREIGHT_PORCT_ARG)+myTar.freight_gastos1)*miEst.estHeader.dolar*miEst.estHeader.cantidadcontenedores;
        }
    }

    public async Task<double> calcularGastosTerminal(EstimateV2 miEst, TarifasByDate myTar)
    {
        if(miEst==null)
        {
            return -1;
        }
        //myTar= await _unitOfWork.TarifasTerminals.GetByContTypeAsync(miEst.estHeader.freight_type);
        if(myTar==null)
        {
            return -1;
        }
        return ((myTar.terminal_gastoFijo+myTar.terminal_gastoVariable)*miEst.estHeader.dolar*miEst.estHeader.cantidadcontenedores);
    }

    public async Task<double> calcularGastosDespachante(EstimateV2 miEst)
    {
        double tmp;
        if(miEst==null)
        {   // OJO con que me pasen NULL.   
            return -1;
        }

        if((miEst.estHeader.cif_grandtotal*miEst.constantes.CNST_GASTOS_DESPA_Cif_Mult)>miEst.constantes.CNST_GASTOS_DESPA_Cif_Thrhld)
        {
            tmp=miEst.estHeader.cif_grandtotal*miEst.constantes.CNST_GASTOS_DESPA_Cif_Mult*miEst.estHeader.dolar;
        }
        else
        {
            tmp=miEst.estHeader.dolar*miEst.constantes.CNST_GASTOS_DESPA_Cif_Min;
        }

        return tmp+(miEst.constantes.CNST_GASTOS_DESPA_Cif_Thrhld*miEst.estHeader.dolar);
    }

    public async Task<double> calcularGastosTteLocal(EstimateV2 miEst,TarifasByDate myTar,Carga myCont)
    {
        double tmp;
        if(miEst==null)
        {
            return -1;
        }

        if(myTar==null)
        {
            return -1;
        }
        // Calculos los gastos totales de transporte. Aun cuando tenga un campo gastostot.
        tmp=myTar.fleteint+myTar.flete_devacio+myTar.flete_demora+myTar.flete_guarderia;
        // si es un LCL, es menos que un contenedor. No se multiplica por Cantidad de Contenedores.
        if(myCont.description=="LCL")
        {
            return tmp;
        }
        else
        {
            return tmp*miEst.estHeader.cantidadcontenedores;
        }
    }

    public async Task<double> calcularGastosCustodia(EstimateV2 miEst,TarifasByDate myTar)
    {   
            if(miEst==null)
            {
                return -1;
            }

            if(myTar==null)
            {
                return -1;
            }

            if(miEst.estHeader.fob_grandtotal>miEst.constantes.CNST_GASTOS_CUSTODIA_Thrshld)
            {
                return (myTar.poliza_prima+myTar.poliza_demora) + ((myTar.poliza_prima+myTar.poliza_demora)*(myTar.poliza_impint/100)*(myTar.poliza_sellos/100));
            }
            else
            {
                return 0;
            }
    }

    public double calcularGastosGestDigDocs(EstimateV2 miEst)
    {
        return miEst.constantes.CNST_GASTOS_GESTDIGDOC_Mult*miEst.estHeader.dolar;
    }

    public double calcularGastosBancarios(EstimateV2 miEst)
    {
        return miEst.constantes.CNST_GASTOS_BANCARIOS_Mult*miEst.estHeader.dolar;
    }
//#########################################################################################







    public EstimateV2 CalcExtraGastoLocProyectoUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.totalgastosloc_uss=_estDetServices.CalcGastosProyPond(ed,est.estHeader.totalgastosloc_uss);       
        }
        return est;
    }




    

    public EstimateV2 CalcOverhead(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.overhead=_estDetServices.CalcOverHeadUnitUSS(ed);  
            if(ed.overhead<0)
            {
                haltError=$"ATENCION: El articulo '{ed.modelo}' tiene un PRECIO USS UNIT de 0. Div 0 !";
                return null;
            }     
        }
        return est;
    }

    public EstimateV2 CalcCostoUnitarioUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.costounituss=_estDetServices.CalcCostoUnitUSS(ed);       
        }
        return est;
    }

    public EstimateV2 CalcCostoUnitario(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.costounit=_estDetServices.CalcCostoUnit(ed,est.estHeader.dolar);       
        }
        return est;
    } 

    public async Task<EstimateV2> CalcularCantContenedores(EstimateV2 est)
    {
        Carga myCont=new Carga();
        myCont=await _unitOfWork.micarga.GetByIdAsync(est.estHeader.freight_type);
        if(myCont==null)        
        {
            return null;
        }
        if(myCont.volume>0 && myCont.weight>0)
        {
            //est.CantidadContenedores=est.CbmGrandTot/myCont.volume;
            if((est.estHeader.cbm_grandtotal/myCont.volume)>(est.estHeader.gw_grandtotal/myCont.weight))
            {// Gana el volumen
                est.estHeader.cantidadcontenedores=est.estHeader.cbm_grandtotal/myCont.volume;
            }
            else
            {// Gan el peso.
                est.estHeader.cantidadcontenedores=est.estHeader.gw_grandtotal/myCont.weight;
            }
        }
        else
        {
            return null;
        }
        return est;
    }

    public EstimateV2 CalcFleteTotal(EstimateV2 est)
    {
        double tmp;
        tmp=misTarifasByDate.freight_cost*est.estHeader.cantidadcontenedores;
        est.totalfreight_cost=tmp;
        return est;
    }

    public EstimateV2 CalcSeguroTotal(EstimateV2 miEst)
    {
        miEst.estHeader.freight_insurance_cost=(miEst.constantes.CNST_SEGURO_PORCT/100)*miEst.estHeader.fob_grandtotal;
        return miEst;
    }

    public EstimateV2 CalcPagadoTot(EstimateV2 miEst)
    {
        double tmp=0;
        foreach(EstimateDetail ed in miEst.estDetails)
        {
            tmp+=ed.totalaranceles;
        }
        miEst.estHeader.total_impuestos=tmp+miEst.constantes.CNST_ARANCEL_SIM;
        return miEst;
    }

    public string getLastError()
    {
        return haltError;
    }

    public void setLastError(string err)
    {
        haltError=err;
    }
    // Carga la tabla de Tarifas con el id provisto en el estheader a una variable global para que la puedan
    // usar todas las funciones.
    public async Task<EstimateV2> loadTarifas(EstimateV2 miEst)
    {
        misTarifasByDate=await _unitOfWork.TarifasPorFecha.GetByIdAsync(miEst.estHeader.tarifasbydateid);
        if(misTarifasByDate==null)
        {
            haltError=$"La tafifa referenciada por el ID{miEst.estHeader.tarifasbydateid} no existe. FK";
            return null;
        }
        return miEst;
    }

    public async Task<EstimateV2> loadContenedor(EstimateV2 miEst)
    {
        miContenedor=await _unitOfWork.micarga.GetByIdAsync(miEst.estHeader.freight_type);
        if(miContenedor==null)
        {
            haltError=$"El tipo de contenedor referenciado con el ID{miEst.estHeader.freight_type} no existe. FK";
        }
        return miEst;
    }

}