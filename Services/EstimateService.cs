namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;                                              

// 3_8_2023 SEGUNDA VERSION para "multiregion" basado en el sheet de MEX y ARG WIP
// 4_9_2023 Refactor logica de tarifas.

public class EstimateService: IEstimateService
{
    IUnitOfWork _unitOfWork;
    Tarifas misTarifas;
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

    public void setConstants(CONSTANTES miConst)
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
                haltError=$"ATENCION: El articulo modelo '{ed.description}' tiene cant pcs x caja = 0. DIV 0 !";
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
                haltError=$"ATENCION: El articulo '{ed.description}' tiene cant pcs por caja = 0. DIV 0 !";
                return null;
            }    
        }
        return est;
    }

    public EstimateV2 CalcFleteTotalByProd(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.freightCharge=_estDetServices.CalcFlete(ed,est.totalfreight_cost,est.estHeader.fob_grand_total); 
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
            ed.insuranceCharge=_estDetServices.CalcSeguro(ed,est.freight_insurance_cost,est.estHeader.fob_grand_total);   
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
                haltError=$"FALLO NCM='{ed.ncm_id}', DET= '{ed.description}";
                return null;
           }
           ed.ncm_arancel=myNCM.die/100.0;
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
            ed.iibb900=_estDetServices.CalcIIBB(ed, est.estHeader.iibb_total);
        }
        return est;
    }

    public EstimateV2 CalcPrecioUnitUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.precio_u=_estDetServices.CalcPrecioUnitUSS(ed);
            if(ed.precio_u<0)
            {
                haltError=$"ATENCION: Articulo '{ed.description}' tiene can pcs = 0. DIV 0 !";
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
            ed.factorproducto=_estDetServices.CalcFactorProducto(ed,est.estHeader.fob_grand_total);   
            if(ed.factorproducto<0)
            {
                haltError=$"ATENCION: El articulo '{ed.description}' tiene un FOB TOT de 0. Div 0!";
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
        est.estHeader.cbm_grand_total=tmp;
        return est;
    }

    public EstimateV2 CalcCifTotal(EstimateV2 est)
    {
        double tmp=0;
        foreach(EstimateDetail ed in est.estDetails)
        {
            tmp+=ed.totalcif;
        }
        est.estHeader.cif_grand_total=tmp;
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
    /*public async Task<EstimateV2> calcularGastosProyecto(EstimateV2 miEst)
    {
        double tmp;
        double result;

       //TarifasByDate myTbd=await _unitOfWork.TarifasPorFecha.GetByIdAsync(miEst.estHeader.tarifasbydateid);
        // Ahora se maneja todo por FK ... para saber el tipo de contenedor necesito consultar la tabla contenedores
       //Contenedor myCont=await _unitOfWork.Contenedores.GetByIdAsync(miEst.estHeader.freight_type);

        tmp=await calcularGastosFwd(miEst);
        if(tmp<0)
        {   // Todos los metodos que consultan una tabla tienen opcion de devolver -1 si algo no salio bien.
            haltError="FALLA CALCULAR GASTOS FWD. TABLA TarifasFWD no accesible o no existen datos para el tipo de contenedor / origen indicados";
            return null;
        }
        // Guardo en el header.
        miEst.estHeader.gloc_fwd=tmp;
        // Registro el gasto en cada articulo (ponderado por el factor de producto)  
        miEst=Calc_GLOC_FWD_POND(miEst,tmp);


        tmp=await calcularGastosTerminal(miEst);
        if(tmp<0)
        {
            haltError="FALLA AL CALCULAR GASTOS DE TERMINAL. TAbla no accesible o no existen datos para el tipo de contenedor ingresado";
            return null;
        }
        //Guardo en el header.
        miEst.estHeader.gloc_terminales=tmp;
        // Registro el gasto en cada articulo
        miEst=Calc_GLOC_TERMINAL_POND(miEst,tmp);



        tmp=await calcularGastosDespachante(miEst);
        if(tmp<0)
        {

            return null;
        }
        // Lo guardo en el header
        miEst.estHeader.gloc_despachantes=tmp;
        // Registro el gasto en el producto
        miEst=Calc_GLOC_DESPACHANTE_POND(miEst,tmp);


        tmp=await calcularGastosTteLocal(miEst);
        if(tmp<0)
        {
            haltError="FALLA AL CALCULAR LOS GASTOS DE TTE LOC. Tabla de tarifa no accesible o no existen datos para el contenedor ingresado";
            return null;
        }
        // Guardo el gasto en el header
        miEst.estHeader.gloc_flete=tmp;
        // Registro el gasto en el producto
        miEst=Calc_GLOC_FLETE_POND(miEst,tmp);



        tmp=await calcularGastosCustodia(miEst);
        if(tmp<0)
        {
            haltError="FALLO AL CALCULAR LOS GASTOS DE CUSTODIA. Tabla de tarifa no accesible o no existen datos para el Proveedor de Poliza ingresado";
            return null;
        }
        // Guardo el gasto en el header
        miEst.estHeader.gloc_polizas=tmp;
        // Registro el gasto
        miEst=Calc_GLOC_CUSTODIA_POND(miEst,tmp);


        tmp=calcularGastosGestDigDocs(miEst);       // Este metodo no involucra una consulta a tabla, tmp np puede ser negativo
        miEst.estHeader.gloc_gestdigdoc=tmp;
        miEst=Calc_GLOC_GESTDIGDOC_POND(miEst,tmp);

        tmp=calcularGastosBancarios(miEst);         // Este idem.
        miEst.estHeader.gloc_bancos=tmp;
        miEst=Calc_GLOC_BANCARIO_POND(miEst,tmp);


        return miEst;

    }*/


// Rutinas que distribuyen a lo largo de todos los art del detail los diferentes gastos locales:
// Fowarding (0.50 Freight)
// Terminal
// Despachante
// TteLocal
// Custodia
// Digitalizaion Documental
// Bancarios
/////////////////////////////////
    public EstimateV2 Calc_GLOC_FWD_POND(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_fwd=_estDetServices.CalcGastosProyPond(ed,est.estHeader.gloc_fwd);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_TERMINAL_POND(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_terminales=_estDetServices.CalcGastosProyPond(ed,est.estHeader.gloc_terminales);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_DESPACHANTE_POND(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_despachantes=_estDetServices.CalcGastosProyPond(ed,est.estHeader.gloc_despachantes);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_FLETE_POND(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_flete=_estDetServices.CalcGastosProyPond(ed,est.estHeader.gloc_flete);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_CUSTODIA_POND(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_polizas=_estDetServices.CalcGastosProyPond(ed,est.estHeader.gloc_polizas);       
        }
        return est;
    }
    public EstimateV2 Calc_GLOC_BANCARIO_POND(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_bancos=_estDetServices.CalcGastosProyPond(ed,est.estHeader.gloc_bancos);       
        }
        return est;
    }

    public EstimateV2 Calc_GLOC_POND(EstimateV2 miEst)
    {
        foreach(EstimateDetail ed in miEst.estDetails)
        {
            ed.gloc_bancos=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.gloc_bancos);
            ed.gloc_depositos=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.gloc_depositos);
            ed.gloc_despachantes=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.gloc_despachantes);
            ed.gloc_flete=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.gloc_flete);
            ed.gloc_fwd=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.gloc_fwd);
            ed.gloc_gestdigdoc=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.gloc_gestdigdoc);
            ed.gloc_polizas=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.gloc_polizas);
            ed.gloc_terminales=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.gloc_terminales);
        }
        return miEst;
    }

    public EstimateV2 Calc_EXTRAG_GLOBAL_POND(EstimateV2 miEst)
    {
        foreach(EstimateDetail ed in miEst.estDetails)
        {
            ed.extrag_glob_comex1=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_comex1);
            ed.extrag_glob_comex2=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_comex2);
            ed.extrag_glob_comex3=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_comex3);
            ed.extrag_glob_comex4=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_comex4);
            ed.extrag_glob_comex5=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_comex5);
            ed.extrag_glob_finan1=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_finan1);
            ed.extrag_glob_finan2=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_finan2);
            ed.extrag_glob_finan3=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_finan3);
            ed.extrag_glob_finan4=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_finan4);
            ed.extrag_glob_finan5=_estDetServices.CalcGastosProyPond(ed,miEst.estHeader.extrag_finan5);
        }
        return miEst;
    }

    public EstimateV2 Calc_GLOC_GESTDIGDOC_POND(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.gloc_gestdigdoc=_estDetServices.CalcGastosProyPond(ed,est.estHeader.gloc_gestdigdoc);       
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
public async Task<double> calcularGastosFwd(EstimateV2 miEst)
    {   
        if(miEst==null)
        {
            return -1;
        }
        if(miEst.misTarifas==null)
        {
            return -1;
        }
        if(miEst.miCarga==null)
        {
            return -1;
        }
        
        if(miEst.miCarga.description=="LCL")
        {
            return (miEst.misTarifas.tFwd.costo_local*miEst.constantes.CNST_FREIGHT_PORCT_ARG*miEst.estHeader.cbm_grand_total*miEst.estHeader.dolar)+miEst.misTarifas.tFwd.gasto_otro1*miEst.estHeader.dolar;
        }
        else
        {
            return ((miEst.misTarifas.tFwd.costo_local*miEst.constantes.CNST_FREIGHT_PORCT_ARG)+miEst.misTarifas.tFwd.gasto_otro1)*miEst.estHeader.dolar*miEst.estHeader.cantidad_contenedores;
        }
    }

    public async Task<double> calcularGastosTerminal(EstimateV2 miEst)
    {
        if(miEst==null)
        {
            return -1;
        }
        //myTar= await _unitOfWork.TarifasTerminals.GetByContTypeAsync(miEst.estHeader.freight_type);
        if(miEst.misTarifas==null)
        {
            return -1;
        }
        return ((miEst.misTarifas.tTerminal.gasto_fijo+miEst.misTarifas.tTerminal.gasto_variable)*miEst.estHeader.dolar*miEst.estHeader.cantidad_contenedores);
    }

    public async Task<double> calcularGastosDespachante(EstimateV2 miEst)
    {
        double tmp;
        if(miEst==null)
        {   // OJO con que me pasen NULL.   
            return -1;
        }

        if((miEst.estHeader.cif_grand_total*miEst.constantes.CNST_GASTOS_DESPA_Cif_Mult)>miEst.constantes.CNST_GASTOS_DESPA_Cif_Thrhld)
        {
            tmp=miEst.estHeader.cif_grand_total*miEst.constantes.CNST_GASTOS_DESPA_Cif_Mult*miEst.estHeader.dolar;
        }
        else
        {
            tmp=miEst.estHeader.dolar*miEst.constantes.CNST_GASTOS_DESPA_Cif_Min;
        }

        return tmp+(miEst.constantes.CNST_GASTOS_DESPA_Cif_Thrhld*miEst.estHeader.dolar);
    }

    public async Task<double> calcularGastosTteLocal(EstimateV2 miEst)
    {
        double tmp;
        if(miEst==null)
        {
            return -1;
        }

        if(miEst.misTarifas==null)
        {
            return -1;
        }
        // Calculos los gastos totales de transporte. Aun cuando tenga un campo gastostot.
        tmp=miEst.misTarifas.tFlete.flete_interno+miEst.misTarifas.tFlete.devolucion_vacio+miEst.misTarifas.tFlete.demora+miEst.misTarifas.tFlete.guarderia;
        // si es un LCL, es menos que un contenedor. No se multiplica por Cantidad de Contenedores.
        if(miEst.miCarga.description=="LCL")
        {
            return tmp;
        }
        else
        {
            return tmp*miEst.estHeader.cantidad_contenedores;
        }
    }

    public async Task<double> calcularGastosCustodia(EstimateV2 miEst)
    {   
            if(miEst==null)
            {
                return -1;
            }

            if(miEst.misTarifas==null)
            {
                return -1;
            }

            if(miEst.estHeader.fob_grand_total>miEst.constantes.CNST_GASTOS_CUSTODIA_Thrshld)
            {
                return (miEst.misTarifas.tPoliza.prima+miEst.misTarifas.tPoliza.demora) + ((miEst.misTarifas.tPoliza.prima+miEst.misTarifas.tPoliza.demora)*(miEst.misTarifas.tPoliza.impuestos_internos/100)*(miEst.misTarifas.tPoliza.sellos/100));
            }
            else
            {
                return 0;
            }
    }

    public double calcularGastosGestDigDocs(EstimateV2 miEst)
    {
        return miEst.misTarifas.tGestDigDoc.factor1*miEst.estHeader.dolar;
    }

    public double calcularGastosBancarios(EstimateV2 miEst)
    {
        return miEst.misTarifas.tBanco.factor1*miEst.estHeader.dolar;
    }
//#########################################################################################







    public EstimateV2 CalcGastos_LOC_Y_EXTRA(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.totalgastos_loc_y_extra=_estDetServices.CalcGastos_Loc_y_Extra(ed);       
        }
        return est;
    }

    public EstimateV2 CalcGastos_LOC_Y_EXTRA_U(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.totalgastos_loc_y_extra_u=_estDetServices.CalcGastos_Loc_y_Extra_Unit(ed);       
        }
        return est;
    }





    

    public EstimateV2 CalcOverhead(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.overhead=_estDetServices.CalcOverHeadUnit(ed);  
            if(ed.overhead<0)
            {
                haltError=$"ATENCION: El articulo '{ed.description}' tiene un PRECIO USS UNIT de 0. Div 0 !";
                return null;
            }     
        }
        return est;
    }

    public EstimateV2 CalcCostoUnitarioUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.estDetails)
        {
            ed.costo_u=_estDetServices.CalcCostoUnitUSS(ed);       
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
        myCont=await _unitOfWork.micarga.GetByIdAsync(est.estHeader.carga_id);
        if(myCont==null)        
        {
            return null;
        }
        if(myCont.volume>0 && myCont.weight>0)
        {
            //est.CantidadContenedores=est.CbmGrandTot/myCont.volume;
            if((est.estHeader.cbm_grand_total/myCont.volume)>(est.estHeader.gw_grand_total/myCont.weight))
            {// Gana el volumen
                est.estHeader.cantidad_contenedores=est.estHeader.cbm_grand_total/myCont.volume;
            }
            else
            {// Gan el peso.
                est.estHeader.cantidad_contenedores=est.estHeader.gw_grand_total/myCont.weight;
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
        tmp=est.misTarifas.tFwd.costo_local*est.estHeader.cantidad_contenedores;
        est.totalfreight_cost=tmp;
        return est;
    }

    public EstimateV2 CalcSeguroTotal(EstimateV2 miEst)
    {
        miEst.freight_insurance_cost=(miEst.constantes.CNST_SEGURO_PORCT/100)*miEst.estHeader.fob_grand_total;
        return miEst;
    }

    public EstimateV2 CalcPagadoTot(EstimateV2 miEst)
    {
        double tmp=0;
        foreach(EstimateDetail ed in miEst.estDetails)
        {
            tmp+=ed.totalaranceles;
        }
        miEst.estHeader.impuestos_total=tmp+miEst.constantes.CNST_ARANCEL_SIM;
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
        //#### RREGLAR !!! => misTarifasByDate=await _unitOfWork.TarifasPorFecha.GetByIdAsync(miEst.estHeader.tarifasbydateid);
        string hoy=DateTime.Now.ToString("yyyy-MM-dd");

        misTarifas=new Tarifas();

       
        // El campo tarifsource es una mascara de bit que comanda que tarifa va a actualizarse 
        // Si el bit 8 esta encendido, toda tarifa que tenga su bit encendido sera actualizada
        // x fecha (mas moderna)
        // Si el octavo bit esta apagado, toda tarfia que tenga su bit encendido sera actualizada
        // usando el ID enviado en el JSON
        // Si el campo tarifsource se envia con todos los bits de tarifa apagados, se usaran los 
        // valores de gastos locales calculados ya guardos en en versiones anteriores.
        // Ningun acceso a datos de tarfia en la DB sera realizado.

        // Actualizo tarifa Banco ?
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifBanco))>0)
        {   // SI. Actualizo por fecha ?
            if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasHoy))>0)
            {   // SI
                misTarifas.tBanco=await _unitOfWork.TarifBancos.GetByNearestDateAsync(hoy,miEst.estHeader.paisregion_id);
                if(misTarifas.tBanco==null)         
                    { haltError=$"La tarifa Banco mas prox no encontrada"; return null;}
            }
            else
            {   // No. Actualizo segun registro elegido ID DB
                misTarifas.tBanco=await _unitOfWork.TarifBancos.GetByIdAsync(miEst.estHeader.tarifasbancos_id);
                if(misTarifas.tBanco==null)         
                    { haltError=$"La tarifa Banco con ID:{miEst.estHeader.tarifasbancos_id} no existe"; return null;}
            }
        }
        // Actualizo Tarifa Deposito ?
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifDepo))>0)
        {   // Actualizo Tarifas Deposito
            if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasHoy))>0)
            {   // Actualizo usando cotizacion mas moderna
                misTarifas.tDepo=await _unitOfWork.TarifasDepositos.GetByNearestDateAsync(hoy,miEst.estHeader.carga_id,miEst.estHeader.paisregion_id);
                if(misTarifas.tDepo==null)          
                    { haltError=$"La tarifa Deposito mas prox no encontrada"; return null;}
            }
            else
            {   // Actualizo usando el ID provisto.
                misTarifas.tDepo=await _unitOfWork.TarifasDepositos.GetByIdAsync(miEst.estHeader.tarifasdepositos_id);
                if(misTarifas.tDepo==null)      
                    { haltError=$"La tarifa Deposito con ID: {miEst.estHeader.tarifasdepositos_id} no existe"; return null;}
            }
        }
        // Actualizo Tarifa Flete ?
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifDespa))>0)
        {
            if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasHoy))>0)
            {
                misTarifas.tFlete=await _unitOfWork.TarifFlete.GetByNearestDateAsync(hoy,miEst.estHeader.carga_id,miEst.estHeader.paisregion_id);
                if(misTarifas.tFlete==null)         
                    { haltError=$"La tarifa Flete mas prox no encontrada"; return null;}
            }
            else                 
            {
                misTarifas.tDespa=await _unitOfWork.TarifDespa.GetByIdAsync(miEst.estHeader.tarifasdespachantes_id);
                if(misTarifas.tDespa==null)  
                    { haltError=$"La tarifa Despachante con ID: {miEst.estHeader.tarifasdespachantes_id} no existe"; return null;}
            }
        }
        // Actualizo Tarifa Fowarder ?
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasFwd))>0)
        {
            if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasHoy))>0)
            {
                misTarifas.tFwd=await _unitOfWork.TarifFwd.GetByNearestDateAsync(hoy,miEst.estHeader.carga_id,miEst.estHeader.paisregion_id,miEst.estHeader.fwdpaisregion_id);
                if(misTarifas.tFwd==null)           
                    { haltError=$"La tarifa Fowarder mas prox no encontrada para paisorig ID: {miEst.estHeader.fwdpaisregion_id} y paisdest ID:{miEst.estHeader.paisregion_id}"; return null;}
            }
            else
            {
                misTarifas.tFwd=await _unitOfWork.TarifFwd.GetByIdAsync(miEst.estHeader.tarifasfwd_id);
                if(misTarifas.tFwd==null)           
                    { haltError=$"La tarifa Fowarder con ID: {miEst.estHeader.tarifasfwd_id}, no existe"; return null;}
            }
        }
        // Actualizo Tarifa Flete ?
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifFlete))>0)
        {
            if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasHoy))>0)
            {
                misTarifas.tFlete=await _unitOfWork.TarifFlete.GetByNearestDateAsync(hoy,miEst.estHeader.carga_id,miEst.estHeader.paisregion_id);
                if(misTarifas.tFlete==null)         
                    { haltError=$"La tarifa Flete mas prox no encontrada"; return null;}
            }
            else
            {
                misTarifas.tFlete=await _unitOfWork.TarifFlete.GetByIdAsync(miEst.estHeader.tarifasflete_id);
                if(misTarifas.tFlete==null)         
                    { haltError=$"La tarifa Flete con ID: {miEst.estHeader.tarifasflete_id}"; return null;}
            }
        }
        // Actualizo tarifa GestDigDoc
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifGestDigDoc))>0)
        {
            if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasHoy))>0)
            {
                misTarifas.tGestDigDoc=await _unitOfWork.TarifGestDigDoc.GetByNearestDateAsync(hoy,miEst.estHeader.paisregion_id);
                if(misTarifas.tGestDigDoc==null)    
                    { haltError=$"La tarifa GestionDigitalDocumental mas prox no encontrada"; return null;}
            }
            else
            {
                misTarifas.tGestDigDoc=await _unitOfWork.TarifGestDigDoc.GetByIdAsync(miEst.estHeader.tarifasgestdigdoc_id);
                if(misTarifas.tGestDigDoc==null)    
                        { haltError=$"La tarifa GestionDigitalDocumental con ID: {miEst.estHeader.tarifasgestdigdoc_id} no existe"; return null;}
            }
        }
        // Actualizo Tarfias Poliza ?
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifPoliza))>0)
        {
            if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasHoy))>0)
            {
                misTarifas.tPoliza=await _unitOfWork.TarifPoliza.GetByNearestDateAsync(hoy,miEst.estHeader.paisregion_id);
                if(misTarifas.tPoliza==null)        
                    { haltError=$"La tarifa Poliza mas prox no encontrada"; return null;} 
            }
            else
            {
                misTarifas.tPoliza=await _unitOfWork.TarifPoliza.GetByIdAsync(miEst.estHeader.tarifaspolizas_id);
                if(misTarifas.tPoliza==null)        
                    { haltError=$"La tarifa Poliza con ID: {miEst.estHeader.tarifaspolizas_id}"; return null;}
            }
        }
        // Las tarfias que levante la fui guardando en "misTarifas". Las guardo en EstimateV2.    
        miEst.misTarifas=misTarifas;

        double tmp;

        // Me fijo si los gastos deben o no recalcularse. Arriba se determino si la tarifa se levanta por fecha o por ID
        // En este punto todas las tarfias han sido cargdas dentro del EstimateV2.

        // Calculo el gasto bancario (ARG), si el bit de actualizar esta encendido
        // Si el bit actualizar esta apagado ... se usara el valor g_loc correspondiente.
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifBanco))>0)
        {
            tmp=calcularGastosBancarios(miEst);         // Este idem.
            // Lo guardo en el header
            miEst.estHeader.gloc_bancos=tmp;
        }
        // Calculo gastos del despachante (ARG)
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifDespa))>0)
        {
            tmp=await calcularGastosDespachante(miEst);
            if(tmp<0)
            {

            return null;
            }
            // Lo guardo en el header
            miEst.estHeader.gloc_despachantes=tmp;
        }
        // Calculo gastos de FLETE INTERNO (ARG)
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifFlete))>0)
        {
            tmp=await calcularGastosTteLocal(miEst);
            if(tmp<0)
            {
                haltError="FALLA AL CALCULAR LOS GASTOS DE TTE LOC. Tabla de tarifa no accesible o no existen datos para el contenedor ingresado";
                return null;
            }
            // Guardo el gasto en el header
            miEst.estHeader.gloc_flete=tmp;
        }
        // Calculo el gasto de Fowarder con el 040 del flete (ARG)
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasFwd))>0)
        {
            tmp=await calcularGastosFwd(miEst);
            if(tmp<0)
            {   // Todos los metodos que consultan una tabla tienen opcion de devolver -1 si algo no salio bien.
                haltError="FALLA CALCULAR GASTOS FWD. TABLA TarifasFWD no accesible o no existen datos para el tipo de contenedor / origen indicados";
                return null;
            }
            // Guardo en el header.
            miEst.estHeader.gloc_fwd=tmp;
        }
        // Calculo los gastos de Gestion digital de documentos
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifGestDigDoc))>0)
        {
            tmp=calcularGastosGestDigDocs(miEst);       // Este metodo no involucra una consulta a tabla, tmp np puede ser negativo
            miEst.estHeader.gloc_gestdigdoc=tmp;
        }
        // Calculo los gastos de custodia / seguro
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifPoliza))>0)
        {
            tmp=await calcularGastosCustodia(miEst);
            if(tmp<0)
            {
                haltError="FALLO AL CALCULAR LOS GASTOS DE CUSTODIA. Tabla de tarifa no accesible o no existen datos para el Proveedor de Poliza ingresado";
                return null;
            }
            // Guardo el gasto en el header
            miEst.estHeader.gloc_polizas=tmp;
        }
        // Calculo los gastos de Terminal
        if((miEst.estHeader.tarifsource&(1<<(int)tarifaControl.tarifasTerm))>0)
        {
            tmp=await calcularGastosTerminal(miEst);
            if(tmp<0)
            {
                haltError="FALLA AL CALCULAR GASTOS DE TERMINAL. TAbla no accesible o no existen datos para el tipo de contenedor ingresado";
                return null;
            }
            //Guardo en el header.
            miEst.estHeader.gloc_terminales=tmp;
        }

        return miEst;
    }

    public EstimateV2 registrarGastosLocalesPorProducto(EstimateV2 miEst)
    {
       /* miEst=Calc_GLOC_BANCARIO_POND(miEst);
        miEst=Calc_GLOC_DESPACHANTE_POND(miEst);
        miEst=Calc_GLOC_FLETE_POND(miEst);
        miEst=Calc_GLOC_FWD_POND(miEst);
        miEst=Calc_GLOC_GESTDIGDOC_POND(miEst);
        miEst=Calc_GLOC_CUSTODIA_POND(miEst);
        miEst=Calc_GLOC_TERMINAL_POND(miEst);*/
        miEst=Calc_GLOC_POND(miEst);

        return miEst;
    }

    public EstimateV2 registrarExtraGastosGlobalesPorProducto(EstimateV2 miEst)
    {
        /*miEst=Calc_GLOC_BANCARIO_POND(miEst);
        miEst=Calc_GLOC_DESPACHANTE_POND(miEst);
        miEst=Calc_GLOC_FLETE_POND(miEst);
        miEst=Calc_GLOC_FWD_POND(miEst);
        miEst=Calc_GLOC_GESTDIGDOC_POND(miEst);
        miEst=Calc_GLOC_CUSTODIA_POND(miEst);
        miEst=Calc_GLOC_TERMINAL_POND(miEst);*/
        miEst=Calc_EXTRAG_GLOBAL_POND(miEst);
        return miEst;
    }

    public async Task<EstimateV2> loadContenedor(EstimateV2 miEst)
    {
        miEst.miCarga=await _unitOfWork.micarga.GetByIdAsync(miEst.estHeader.carga_id);
        if(miEst.miCarga==null)
        {
            haltError=$"El tipo de contenedor referenciado con el ID{miEst.estHeader.carga_id} no existe. FK";
        }
        return miEst;
    }

}