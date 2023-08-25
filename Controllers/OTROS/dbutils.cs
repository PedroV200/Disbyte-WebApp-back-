using WebApiSample.Models;
using WebApiSample.Infrastructure;

// LISTED 25_8_2023 17:39 - cambios para Version 3C
  
public class dbutils
{
    public IUnitOfWork _unitOfWork;
    public dbutils(IUnitOfWork unitOfowrk)
    {
        _unitOfWork=unitOfowrk;
    }

    public async Task<EstimateDB> getEstimateLastVers(int estNumber)
    {
        EstimateDB myEst=new EstimateDB(); 
        // La consulta de headers por un numero de presupuesto puede dar como resultado mas
        // de un header (multiples versiones del mismo presupuesto). 
        List<EstimateHeaderDB>misDetalles=new List<EstimateHeaderDB>();
        // La query se hace ORDENADA POR VERSION de MAYOR A MENOR. Es una LISTA de estHeaders
        var result=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVersAsync(estNumber);
        if(result==null)
        {
            
            return null;
        }
        misDetalles=result.ToList();
        // El elemento 0 corresponde al header buscado en con la version MAYOR.
        myEst.estHeaderDB=misDetalles[0];
        // Con el PK del header anterior me voy a buscar los Details que lo referencia en su FK
        var result1=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.id);
        // De nuevo, la consulta x detalles, da una LISTA como resultado.
        myEst.estDetailsDB=result1.ToList();
        // Devuelvo el presupuesto.
        return myEst;
    }

    public async Task<EstimateDB> getEstimateFirstVers(int estNumber)
    {
        EstimateDB myEst=new EstimateDB(); 
        // La consulta de headers por un numero de presupuesto puede dar como resultado mas
        // de un header (multiples versiones del mismo presupuesto). 
        List<EstimateHeaderDB>misDetalles=new List<EstimateHeaderDB>();
        // La query se hace ORDENADA POR VERSION de MAYOR A MENOR. Es una LISTA de estHeaders
        var result=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVersAsync(estNumber);
        misDetalles=result.ToList();
        // El elemento n-1 corresponde al header buscado en con la version MENOR.
        myEst.estHeaderDB=misDetalles[misDetalles.Count-1];
        // Con el PK del header anterior me voy a buscar los Details que lo referencia en su FK
        var result1=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.id);
        // De nuevo, la consulta x detalles, da una LISTA como resultado.
        myEst.estDetailsDB=result1.ToList();
        // Devuelvo el presupuesto.
        return myEst;
    }

    public async Task<int> deleteEstimateByNumByVers(int estNumber, int estVers)
    {
        EstimateHeaderDB myEst=new EstimateHeaderDB();
        myEst=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(estNumber,estVers);
        // Primero liquido los estDetails del ESTIMATE. No puedo borrar primero el header por que las FK quedan sueltas.
        // Primero borro los estDetail con FK coinciente.
        var result= await _unitOfWork.EstimateDetailsDB.DeleteByIdEstHeaderAsync(myEst.id);
        // Ahora si borro el header
        var result1=await _unitOfWork.EstimateHeadersDB.DeleteAsync(myEst.id);
        // devuelvo solo la cantidad de estimateDetail borrados, por que header ya se que borre uno solo.
        return result;
    }

    public async Task<EstimateDB> GetEstimateByNumByVers(int estNumber, int estVers)
    {
        EstimateDB myEst=new EstimateDB();
        myEst.estHeaderDB=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(estNumber,estVers);
        if(myEst.estHeaderDB==null)
        {
            return null;
        }
        var result=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.id);
        myEst.estDetailsDB=result.ToList();
        return myEst;
    }

    public EstimateV2 transferDataFromDBType(EstimateDB estimateDB)
    {
        EstimateV2 myEstV2=new EstimateV2();
        myEstV2.estHeader=estimateDB.estHeaderDB;


        // En el caso de los Estimate detail, la DB tiene solo lo datos que se cuardaran
        // Este tipo se llama EstimateDetailDB. Mientras que el EstimateV2 definido usa el tipo EstimateDetail
        // que contiene no solo los datos que contiene "EstimateDetailDB" sino ademas provicion para los datos
        // calculados. Estos no se guardan en la base. De ahi la existencia de 2 clases "EstimateDetail"
        foreach(EstimateDetailDB edb in estimateDB.estDetailsDB)
        {
            EstimateDetail tmp=new EstimateDetail();

            tmp.id=edb.id;
            tmp.estimateheader_id=edb.estimateheader_id;
            tmp.proveedores_id=edb.proveedores_id;
            tmp.sku=edb.sku;
            tmp.description=edb.description;
            tmp.imageurl=edb.imageurl;
            tmp.exw=edb.exw;
            tmp.fobunit=edb.fobunit;
            tmp.fobunit_adj=edb.fobunit_adj;
            tmp.fobunit_adj_description=edb.fobunit_adj_description;
            tmp.qty=edb.qty;
            tmp.pcsctn=edb.pcsctn;
            tmp.cbmctn=edb.cbmctn;
            tmp.gwctn=edb.gwctn;
            tmp.freight_charge_adj=edb.freight_charge_adj;
            tmp.freight_insurance_adj=edb.freight_insurance_adj;
            tmp.cif=edb.cif;
            tmp.cif_adj=edb.cif_adj;
            tmp.cif_adj_description=edb.cif_adj_description;
            tmp.ncm_id=edb.ncm_id;
            tmp.ncm_arancel=edb.ncm_arancel;
            tmp.ncm_te_dta_otro=edb.ncm_te_dta_otro;
            tmp.ncm_iva=edb.ncm_iva;
            tmp.ncm_ivaad=edb.ncm_ivaad;
            tmp.gcias=edb.gcias;
            tmp.ncm_sp1=edb.ncm_sp1;
            tmp.ncm_sp2=edb.ncm_sp2;
            tmp.gloc_fwd_adj=edb.gloc_fwd_adj;
            tmp.gloc_flete_adj=edb.gloc_flete_adj;
            tmp.gloc_terminal_adj=edb.gloc_terminal_adj;
            tmp.gloc_poliza_adj=edb.gloc_poliza_adj;
            tmp.gloc_deposito_adj=edb.gloc_deposito_adj;
            tmp.gloc_despachante_adj=edb.gloc_despachante_adj;
            tmp.gloc_banco_adj=edb.gloc_banco_adj;
            tmp.gloc_gestdigdoc_adj=edb.gloc_gestdigdoc_adj;
            tmp.gasto_otro1_adj=edb.gasto_otro1_adj;
            tmp.gasto_otro2_adj=edb.gasto_otro2_adj;
            tmp.gasto_otro3_adj=edb.gasto_otro3_adj;
            tmp.ajuste_expre1=edb.ajuste_expre1;
            tmp.ajuste_expre2=edb.ajuste_expre2;
            tmp.ajuste_expre3=edb.ajuste_expre3;
            tmp.gloc_adj_description=edb.gloc_adj_description;
            tmp.precio_u=edb.precio_u;
            tmp.costo_u=edb.costo_u;
            tmp.costo_u_provisorio=edb.costo_u_provisorio;
            tmp.costo_u_provisorio_adj=edb.costo_u_provisorio_adj;
            tmp.costo_u_financiero=edb.costo_u_financiero;
            tmp.costo_u_financiero_adj=edb.costo_u_financiero_adj;
            tmp.extra_gasto1=edb.extra_gasto1;
            tmp.extra_gasto2=edb.extra_gasto2;
            tmp.extra_gasto3=edb.extra_gasto3;
            tmp.extra_gasto4=edb.extra_gasto4;
            tmp.extra_gasto5=edb.extra_gasto5;
            tmp.extra_gasto6=edb.extra_gasto6;
            tmp.extra_gasto7=edb.extra_gasto7;
            tmp.extra_gasto8=edb.extra_gasto8;
            tmp.extra_gasto9=edb.extra_gasto9;
            tmp.extra_gasto10=edb.extra_gasto10;
            tmp.extra_gasto_expre=edb.extra_gasto_expre;
                      
            myEstV2.estDetails.Add(tmp);
        }
        return myEstV2;
    }
// La inversa de la anterior. Pasa un EstimateV2 a un EstimateDB.
    public EstimateDB transferDataToDBType(EstimateV2 myEstV2)
    {
         EstimateDB estimateDB=new EstimateDB();


        estimateDB.estHeaderDB=myEstV2.estHeader;

// Aqui se descartan los calculos. Solo se transfieren los valores necesarios para los mismos
        foreach(EstimateDetail ed in myEstV2.estDetails)
        {
            EstimateDetailDB tmp=new EstimateDetailDB();

            tmp.id=ed.id;
            tmp.estimateheader_id=ed.estimateheader_id;
            tmp.proveedores_id=ed.proveedores_id;
            tmp.sku=ed.sku;
            tmp.description=ed.description;
            tmp.imageurl=ed.imageurl;
            tmp.exw=ed.exw;
            tmp.fobunit=ed.fobunit;
            tmp.fobunit_adj=ed.fobunit_adj;
            tmp.fobunit_adj_description=ed.fobunit_adj_description;
            tmp.qty=ed.qty;
            tmp.pcsctn=ed.pcsctn;
            tmp.cbmctn=ed.cbmctn;
            tmp.gwctn=ed.gwctn;
            tmp.freight_charge_adj=ed.freight_charge_adj;
            tmp.freight_insurance_adj=ed.freight_insurance_adj;
            tmp.cif=ed.cif;
            tmp.cif_adj=ed.cif_adj;
            tmp.cif_adj_description=ed.cif_adj_description;
            tmp.ncm_id=ed.ncm_id;
            tmp.ncm_arancel=ed.ncm_arancel;
            tmp.ncm_te_dta_otro=ed.ncm_te_dta_otro;
            tmp.ncm_iva=ed.ncm_iva;
            tmp.ncm_ivaad=ed.ncm_ivaad;
            tmp.gcias=ed.gcias;
            tmp.ncm_sp1=ed.ncm_sp1;
            tmp.ncm_sp2=ed.ncm_sp2;
            tmp.gloc_fwd_adj=ed.gloc_fwd_adj;
            tmp.gloc_flete_adj=ed.gloc_flete_adj;
            tmp.gloc_terminal_adj=ed.gloc_terminal_adj;
            tmp.gloc_poliza_adj=ed.gloc_poliza_adj;
            tmp.gloc_deposito_adj=ed.gloc_deposito_adj;
            tmp.gloc_despachante_adj=ed.gloc_despachante_adj;
            tmp.gloc_banco_adj=ed.gloc_banco_adj;
            tmp.gloc_gestdigdoc_adj=ed.gloc_gestdigdoc_adj;
            tmp.gasto_otro1_adj=ed.gasto_otro1_adj;
            tmp.gasto_otro2_adj=ed.gasto_otro2_adj;
            tmp.gasto_otro3_adj=ed.gasto_otro3_adj;
            tmp.ajuste_expre1=ed.ajuste_expre1;
            tmp.ajuste_expre2=ed.ajuste_expre2;
            tmp.ajuste_expre3=ed.ajuste_expre3;
            tmp.gloc_adj_description=ed.gloc_adj_description;
            tmp.precio_u=ed.precio_u;
            tmp.costo_u=ed.costo_u;
            tmp.costo_u_provisorio=ed.costo_u_provisorio;
            tmp.costo_u_provisorio_adj=ed.costo_u_provisorio_adj;
            tmp.costo_u_financiero=ed.costo_u_financiero;
            tmp.costo_u_financiero_adj=ed.costo_u_financiero_adj;
            tmp.extra_gasto1=ed.extra_gasto1;
            tmp.extra_gasto2=ed.extra_gasto2;
            tmp.extra_gasto3=ed.extra_gasto3;
            tmp.extra_gasto4=ed.extra_gasto4;
            tmp.extra_gasto5=ed.extra_gasto5;
            tmp.extra_gasto6=ed.extra_gasto6;
            tmp.extra_gasto7=ed.extra_gasto7;
            tmp.extra_gasto8=ed.extra_gasto8;
            tmp.extra_gasto9=ed.extra_gasto9;
            tmp.extra_gasto10=ed.extra_gasto10;
            tmp.extra_gasto_expre=ed.extra_gasto_expre;
            tmp.htimestamp=DateTime.Now;
            //tmp.htimestamp=ed.htimestamp;
            estimateDB.estDetailsDB.Add(tmp);
        }
        return estimateDB;       
    }


}


 