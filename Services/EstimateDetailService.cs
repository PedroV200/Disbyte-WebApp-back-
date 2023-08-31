namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class EstimateDetailService: IEstimateDetailService
{
    public IUnitOfWork _unitOfWork {get;}

    public ICnstService _constService;

    public CONSTANTES misConsts=new CONSTANTES();

    public EstimateDetailService(IUnitOfWork unitOfWork, ICnstService constService)
    {
        _unitOfWork=unitOfWork;
        _constService=constService;
    }

    public async void loadConstants(CONSTANTES miaConst)
    {
        misConsts=miaConst;
    }


    public async Task<NCM> lookUp_NCM_Data(EstimateDetail estDetails)
    {
        NCM myNCM=await _unitOfWork.NCMs.GetByIdAsync(estDetails.ncm_id); 
        if(myNCM!=null)
        {
            return myNCM;
        }   
        return null;
    }

    public async Task<double> lookUpDie(EstimateDetail estDetails)
    {
        NCM myNCM=await _unitOfWork.NCMs.GetByIdAsync(estDetails.ncm_id); 
        if(myNCM!=null)
        {
            return myNCM.die;
        }   
        return -1;
    }

    public double CalcTE(double te)
    {
            double tmpL;
            // VER COLUMNA U (U15 en adelante).
            // Existe el te ?. No puedo tener un te "EN BLANCO" como el XLS. Lo ideal que x defecto tengan un valor negativo
            // como para indicar que esta "en blanco".
            if(te>0)
            { // Si.
                return te;
            }
            else
            { // No, entonces vale 3%.
                //tmpL=3.00000;
                return misConsts.CONST_NCM_DIE_Min;
            }
    }

    public double CalcPesoTotal(EstimateDetail estD)
    {
        if(estD.pcsctn!=0)
        {
            return (estD.gwctn/estD.pcsctn)*estD.qty;
        }
        return -1;
    }

    public double CalcCbmTotal(EstimateDetail estD)
    {
        if(estD.pcsctn!=0)
        {
            return (estD.qty*estD.cbmctn)/estD.pcsctn;
        }
        return -1;
    }

    public double CalcFob(EstimateDetail estD)
    {
        return (estD.qty*estD.fob_u);
    }

    public double CalcFlete(EstimateDetail estD, double costoFlete,double fobGrandTotal)
    {
        if(fobGrandTotal>0)
        {
            return (estD.totalfob/fobGrandTotal)*costoFlete;
        }
        return -1;
    }

    public double CalcSeguro(EstimateDetail estD, double seguroTotal, double fobGrandTotal)
    {
        if(fobGrandTotal>0)
        {
            return ((estD.totalfob/fobGrandTotal)*seguroTotal);
        }
        return -1;
    }

    public double CalcValorEnAduanaDivisa(EstimateDetail estD)
    {
        return estD.totalcif;
    }

    public double CalcCif(EstimateDetail estD)
    {
         return(estD.insuranceCharge+estD.freightCharge+estD.totalfob);
    }

    public double CalcDerechos(EstimateDetail est)
    {
        return est.totalcif*est.ncm_arancel;
    }
    // Conforme COL V
    public double CalcTasaEstad061(EstimateDetail est)
    {
        if(est.ncm_te_dta_otro==0)
        {
            return 0;
        }
        else
        {
            if(est.totalcif<misConsts.CNST_ESTAD061_ThrhldMAX)
            //if(est.valAduanaDivisa<10000)
            {
                if((est.totalcif*est.ncm_te_dta_otro)>misConsts.CNST_ESTAD061_ThrhldMIN)
                //if((est.valAduanaDivisa*est.Te)>180)
                {
                    //return 180;
                    return misConsts.CNST_ESTAD061_ThrhldMIN;
                }
                else
                {
                    return est.totalcif*est.ncm_te_dta_otro;
                }
            }
            else
            {
                return est.totalcif*est.ncm_te_dta_otro;
            }
        }
    }

    public double CalcBaseIvaGcias(EstimateDetail estD)
    {
         return (estD.totalcif+estD.arancelgrav_cif+estD.te_dta_otro_cif);
    }   


    public async Task<double> lookUpIVA(EstimateDetail estDetails)
    {
            NCM myNCM=await _unitOfWork.NCMs.GetByIdAsync(estDetails.ncm_id); 
            // VER COLUMNA U (U15 en adelante).
            // Existe el te ?. No puedo tener un te "EN BLANCO" como el XLS. Lo ideal que x defecto tengan un valor negativo
            // como para indicar que esta "en blanco".
            if(myNCM!=null)
            { // Si.
                return myNCM.iva;   
            }            
            return -1;
    } 

    public double CalcIVA415(EstimateDetail estDetail)
    {
        return estDetail.baseiva*estDetail.ncm_iva;
    }

    public async Task<double> lookUpIVAadic(EstimateDetail estDetails)
    {
            NCM myNCM=await _unitOfWork.NCMs.GetByIdAsync(estDetails.ncm_id); 
            // VER COLUMNA U (U15 en adelante).
            // Existe el te ?. No puedo tener un te "EN BLANCO" como el XLS. Lo ideal que x defecto tengan un valor negativo
            // como para indicar que esta "en blanco".
            if(myNCM!=null)
            { // Si.
                return myNCM.iva_ad;   
            }            
            return -1;
    } 

    public double CalcIvaAdic(EstimateDetail estDetails, bool ivaEx)
    {
        if(ivaEx)
        {
            return 0;
        }
        else
        {
            return estDetails.baseiva*estDetails.ncm_ivaad;
        }
    }

    public double CalcImpGcias424(EstimateDetail estDetails)
    {
        //return (estDetails.BaseIvaGcias*6)/100;
        return estDetails.baseiva*misConsts.CNST_GCIAS_424_Mult;
    }

    public double CalcIIBB(EstimateDetail estDetails, double sumaFactoresIIBB)
    {
        //double totalIIBB=await _unitOfWork.IIBBs.GetSumFactores();
        return (estDetails.baseiva*(sumaFactoresIIBB/100));
    }

    public double CalcPrecioUnitUSS(EstimateDetail estDetails)
    {
        if(estDetails.qty>0)
        {
            return estDetails.baseiva/estDetails.qty;
        }
        else
        {
            return -1;
        }
    }

    public double CalcPagado(EstimateDetail estDetails)
    {
        return(estDetails.iibb900+estDetails.gcias424+estDetails.ivaad_cif+estDetails.iva_cif+estDetails.te_dta_otro_cif+estDetails.arancelgrav_cif);
    }

    

    public double CalcFactorProducto(EstimateDetail estD, double fobTotal)
    {
        if(fobTotal!=0)
        {
            return (estD.totalfob/fobTotal);
        }
        else
        {
            return -1;
        }
    }

    public double CalcGastosProyPond(EstimateDetail estD, double gastosTotProy)
    {
        return estD.factorproducto*gastosTotProy;
    }
    
    public double CalcGastos_Loc_y_Extra(EstimateDetail estD)
    {
        double tmp;

        tmp=0;
        //tmp=estD.precio_u;
        // Gastos locales ponderados 
        tmp+=estD.gloc_bancos;
        tmp+=estD.gloc_depositos;
        tmp+=estD.gloc_despachantes;
        tmp+=estD.gloc_flete;
        tmp+=estD.gloc_fwd;
        tmp+=estD.gloc_gestdigdoc;
        tmp+=estD.gloc_polizas;
        tmp+=estD.gloc_terminales;
        // Sumo los extragastos que son por producto.
        tmp+=estD.extrag_comex1;
        tmp+=estD.extrag_comex2;
        tmp+=estD.extrag_comex3;
        tmp+=estD.extrag_finan1;
        tmp+=estD.extrag_finan2;
        tmp+=estD.extrag_finan3;
        tmp+=estD.extrag_local1;
        tmp+=estD.extrag_local2;
        // Sumo los extragastos globales que vienen desde el header y ya fueron ponderados.
        tmp+=estD.extrag_glob_comex1;
        tmp+=estD.extrag_glob_comex2;
        tmp+=estD.extrag_glob_comex3;
        tmp+=estD.extrag_glob_comex4;
        tmp+=estD.extrag_glob_comex5;
        tmp+=estD.extrag_glob_finan1;
        tmp+=estD.extrag_glob_finan2;
        tmp+=estD.extrag_glob_finan3;
        tmp+=estD.extrag_glob_finan4;
        tmp+=estD.extrag_glob_comex5;
        return tmp;
    }

    public double CalcGastos_Loc_y_Extra_Unit(EstimateDetail estD)
    {
        if(estD.qty>0)
        {
            return estD.totalgastos_loc_y_extra/estD.qty;
        }
        else
        {
            return -1;
        }
    }
    public double CalcOverHeadUnit(EstimateDetail estD)
    {
        if(estD.precio_u!=0)
        {
            return estD.totalgastos_loc_y_extra_u/estD.precio_u;
        }
        else
        {
            return -1;
        }
    }

    public double CalcCostoUnitUSS(EstimateDetail estD)
    {

        return estD.precio_u+estD.totalgastos_loc_y_extra_u;
    }

    public double CalcCostoUnit(EstimateDetail estD, double dolar)
    {
        return estD.costo_u*dolar;
    }
}