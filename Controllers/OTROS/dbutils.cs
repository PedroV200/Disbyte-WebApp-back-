using WebApiSample.Models;
using WebApiSample.Infrastructure;
using Microsoft.AspNetCore.Mvc;

// LISTED 25_8_2023 17:39 - cambios para Version 3C
// LISTED 27_09_2023 17:21 
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
        int updated;
        EstimateV2 myEstV2=new EstimateV2();
        myEstV2.estHeader=estimateDB.estHeaderDB;

        // 30/8/2023 12:02PM
        // En el caso de los Estimate detail, la DB tiene solo lo datos que se cuardaran
        // Este tipo se llama EstimateDetailDB. Mientras que el EstimateV2 definido usa el tipo EstimateDetail
        // que contiene no solo los datos que contiene "EstimateDetailDB" sino ademas provicion para los datos
        // calculados. Estos no se guardan en la base. De ahi la existencia de 2 clases "EstimateDetail"
        updated=0;
        foreach(EstimateDetailDB edb in estimateDB.estDetailsDB)
        {
            EstimateDetail tmp=new EstimateDetail();

            tmp.id = edb.id;
            tmp.estimateheader_id = edb.estimateheader_id;
            tmp.proveedores_id	= edb.proveedores_id;
            tmp.ncm_id = edb.ncm_id;
            tmp.ncm_ack	= edb.ncm_ack;
            tmp.sku = edb.sku;
            tmp.description = edb.description;
            tmp.imageurl	= edb.imageurl;
            tmp.exw_u = edb.exw_u;
            tmp.fob_u = edb.fob_u;
            tmp.qty = edb.qty;
            tmp.pcsctn = edb.pcsctn;
            tmp.cbmctn = edb.cbmctn;
            tmp.gwctn = edb.gwctn;
            tmp.cambios_notas =	edb.cambios_notas;
            tmp.ncm_arancel = edb.ncm_arancel;
            tmp.ncm_te_dta_otro = edb.ncm_te_dta_otro;
            tmp.ncm_iva = edb.ncm_iva;
            tmp.ncm_ivaad = edb.ncm_ivaad;
            tmp.gcias = edb.gcias;
            tmp.ncm_sp1	= edb.ncm_sp1;
            tmp.ncm_sp2	= edb.ncm_sp2;
            tmp.precio_u = edb.precio_u;
            tmp.extrag_comex1 =	edb.extrag_comex1;
            tmp.extrag_comex2 =	 edb.extrag_comex2;
            tmp.extrag_comex3 =	 edb.extrag_comex3;
            tmp.extrag_comex_notas = edb.extrag_comex_notas;
            tmp.extrag_src1 = edb.extrag_src1;
            tmp.extrag_src2 = edb.extrag_src2;
            tmp.extrag_finan1 = edb.extrag_finan1;
            tmp.extrag_finan2 = edb.extrag_finan2;
            tmp.extrag_finan3 = edb.extrag_finan3;
            tmp.extrag_finan_notas = edb.extrag_finan_notas;
            tmp.costo_u_est = edb.costo_u_est;
            tmp.costo_u_prov = edb.costo_u_prov;
            tmp.costo_u = edb.costo_u;
            tmp.updated = edb.updated;
            tmp.purchaseorder = edb.purchaseorder;
            tmp.productowner = edb.productowner;
            tmp.comercial_invoice = edb.comercial_invoice;
            tmp.proforma_invoice=edb.proforma_invoice;
            tmp.proveedor_prov=edb.proveedor_prov;
            tmp.detailorder = edb.detailorder;
            tmp.htimestamp = edb.htimestamp;
            if(edb.updated)
            {
                updated++;
            }    
            myEstV2.estDetails.Add(tmp);
        }
        return myEstV2;
    }





// Solo para PRESUP RECLAIM:
// Necesito que el header me traiga descripciones y no solo FKs. Idem para cada linea del detail.
// En un esfuerzo por manter a EstimateV2 usando el estheaderDB, que no es compatible con estHeaderDBVista
// el header tipo vista (que viene de una query con joins y por ende tiene mas datos) se tranfiere al tipo estimateHeaderDB
// para los datos que son comunes a ambos. Los adicionales se envian a campos sueltos en el EstimateV2.
// 
// El EstimateDetail afronta el mismo dilema. Lo que llega es un detailVista, que tiene mas datos por que FKs han
// sido traducidas a descripcion. Esto hace que el tipo detailVista no sea compatible con el detail.
// En el for se transfieren todos lo datos coincidentes entre ambos tipos. La diferencia va a parar a un objeto VistaAddtionalData
// Una lista de estos (de igual dimension que el detail) guarda toda la info que es diferente entre el detail y vista
public EstimateV2 transferDataFromDBTypeWithVista(EstimateHeaderDBVista miEstHeadV,List<EstimateDetailDBVista> miEstDetV)
    {
        int updated;
        EstimateV2 myEstV2=new EstimateV2();

        // Nada que mirar aca.
        myEstV2.estHeader.id = miEstHeadV.id;
        myEstV2.estHeader.description = miEstHeadV.description;
        myEstV2.estHeader.estnumber = miEstHeadV.estnumber;
        myEstV2.estHeader.estvers = miEstHeadV.estvers;
        myEstV2.estHeader.status = miEstHeadV.status;
        myEstV2.estHeader.paisregion_id = miEstHeadV.paisregion_id;
        myEstV2.estHeader.carga_id = miEstHeadV.carga_id;
        myEstV2.estHeader.fwdpaisregion_id = miEstHeadV.fwdpaisregion_id;
        myEstV2.estHeader.own = miEstHeadV.own;
        myEstV2.estHeader.dolar	= miEstHeadV.dolar;
        myEstV2.estHeader.tarifupdate =	miEstHeadV.tarifupdate;
        myEstV2.estHeader.tarifrecent =	miEstHeadV.tarifrecent;
        myEstV2.estHeader.tarifasfwd_id	= miEstHeadV.tarifasfwd_id;
        myEstV2.estHeader.tarifasflete_id = miEstHeadV.tarifasflete_id;
        myEstV2.estHeader.tarifasterminales_id = miEstHeadV.tarifasterminales_id;
        myEstV2.estHeader.tarifaspolizas_id	= miEstHeadV.tarifaspolizas_id;
        myEstV2.estHeader.tarifasdepositos_id =	miEstHeadV.tarifasdepositos_id;
        myEstV2.estHeader.tarifasdespachantes_id = miEstHeadV.tarifasdespachantes_id;
        myEstV2.estHeader.tarifasbancos_id = miEstHeadV.tarifasbancos_id;
        myEstV2.estHeader.tarifasgestdigdoc_id = miEstHeadV.tarifasgestdigdoc_id;
        myEstV2.estHeader.gloc_fwd = miEstHeadV.gloc_fwd;
        myEstV2.estHeader.gloc_flete = miEstHeadV.gloc_flete;
        myEstV2.estHeader.gloc_terminales =	miEstHeadV.gloc_terminales;
        myEstV2.estHeader.gloc_polizas = miEstHeadV.gloc_polizas;
        myEstV2.estHeader.gloc_depositos = miEstHeadV.gloc_depositos;
        myEstV2.estHeader.gloc_despachantes	= miEstHeadV.gloc_despachantes;
        myEstV2.estHeader.gloc_bancos =	miEstHeadV.gloc_bancos;
        myEstV2.estHeader.gloc_gestdigdoc =	miEstHeadV.gloc_gestdigdoc;
        myEstV2.estHeader.gloc_descarga = miEstHeadV.gloc_descarga; 
        myEstV2.estHeader.extrag_src1	= miEstHeadV.extrag_src1;
        myEstV2.estHeader.extrag_src2	= miEstHeadV.extrag_src2;
        myEstV2.estHeader.extrag_src_notas = miEstHeadV.extrag_src_notas;
        myEstV2.estHeader.extrag_comex1	= miEstHeadV.extrag_comex1;
        myEstV2.estHeader.extrag_comex2	= miEstHeadV.extrag_comex2;
        myEstV2.estHeader.extrag_comex3	= miEstHeadV.extrag_comex3;
        myEstV2.estHeader.extrag_comex_notas = miEstHeadV.extrag_comex_notas;
        myEstV2.estHeader.extrag_finanformula1_id = miEstHeadV.extrag_finanformula1_id;
        myEstV2.estHeader.extrag_finanformula2_id = miEstHeadV.extrag_finanformula2_id;
        myEstV2.estHeader.extrag_finanformula3_id =	miEstHeadV.extrag_finanformula3_id;
        myEstV2.estHeader.extrag_finanformula4_id =	miEstHeadV.extrag_finanformula4_id;
        myEstV2.estHeader.extrag_finanformula5_id =	miEstHeadV.extrag_finanformula5_id;
        myEstV2.estHeader.extrag_finan1	= miEstHeadV.extrag_finan1;
        myEstV2.estHeader.extrag_finan2	= miEstHeadV.extrag_finan2;
        myEstV2.estHeader.extrag_finan3	= miEstHeadV.extrag_finan3;
        myEstV2.estHeader.extrag_finan4	= miEstHeadV.extrag_finan4;
        myEstV2.estHeader.extrag_finan5	= miEstHeadV.extrag_finan5;
        myEstV2.estHeader.extrag_finan_notas = miEstHeadV.extrag_finan_notas;
        myEstV2.estHeader.constantes_id	= miEstHeadV.constantes_id;
        myEstV2.estHeader.ivaexcento = miEstHeadV.ivaexcento;
        myEstV2.estHeader.usarmoneda_local = miEstHeadV.usarmoneda_local;
        myEstV2.estHeader.fob_grand_total =	miEstHeadV.fob_grand_total;
        myEstV2.estHeader.cbm_grand_total =	miEstHeadV.cbm_grand_total;
        myEstV2.estHeader.gw_grand_total = miEstHeadV.gw_grand_total;
        myEstV2.estHeader.cif_grand_total =	miEstHeadV.cif_grand_total;
        myEstV2.estHeader.gastos_loc_total = miEstHeadV.gastos_loc_total;
        myEstV2.estHeader.extragastos_total	= miEstHeadV.extragastos_total;
        myEstV2.estHeader.impuestos_total = miEstHeadV.impuestos_total;
        myEstV2.estHeader.cantidad_contenedores	= miEstHeadV.cantidad_contenedores;
        myEstV2.estHeader.freight_cost = miEstHeadV.freight_cost;
        myEstV2.estHeader.freight_insurance_cost = miEstHeadV.freight_insurance_cost;
        myEstV2.estHeader.iibb_total = miEstHeadV.iibb_total;
        myEstV2.estHeader.project = miEstHeadV.project;
        myEstV2.estHeader.embarque=miEstHeadV.embarque;
        myEstV2.estHeader.tarifonmex_id = miEstHeadV.tarifonmex_id;
        myEstV2.estHeader.htimestamp = miEstHeadV.htimestamp;
        // Datos adicionales, que son propios del headerDBVista, pero incompatibles con headerDB. Se guardan en campos sueltos
        // en el EstimateV2, y que son public en el json.
        myEstV2.paisorig = miEstHeadV.paisorig;
        myEstV2.paisdest = miEstHeadV.paisdest;
        myEstV2.carga_str =	miEstHeadV.carga_str;
        

        // 30/8/2023 12:02PM
        // En el caso de los Estimate detail, la DB tiene solo lo datos que se cuardaran
        // Este tipo se llama EstimateDetailDB. Mientras que el EstimateV2 definido usa el tipo EstimateDetail
        // que contiene no solo los datos que contiene "EstimateDetailDB" sino ademas provicion para los datos
        // calculados. Estos no se guardan en la base. De ahi la existencia de 2 clases "EstimateDetail"
        updated=0;
        foreach(EstimateDetailDBVista edb in miEstDetV)
        {
            EstimateDetail tmp=new EstimateDetail();
            EstimateDetailVistaAdditionalData adata=new EstimateDetailVistaAdditionalData();
            tmp.id = edb.id;
            tmp.estimateheader_id = edb.estimateheader_id;
            tmp.proveedores_id	= edb.proveedores_id;
            tmp.ncm_id = edb.ncm_id;
            tmp.ncm_ack	= edb.ncm_ack;
            tmp.sku = edb.sku;
            tmp.description = edb.description;
            tmp.imageurl	= edb.imageurl;
            tmp.exw_u = edb.exw_u;
            tmp.fob_u = edb.fob_u;
            tmp.qty = edb.qty;
            tmp.pcsctn = edb.pcsctn;
            tmp.cbmctn = edb.cbmctn;
            tmp.gwctn = edb.gwctn;
            tmp.cambios_notas =	edb.cambios_notas;
            tmp.ncm_arancel = edb.ncm_arancel;
            tmp.ncm_te_dta_otro = edb.ncm_te_dta_otro;
            tmp.ncm_iva = edb.ncm_iva;
            tmp.ncm_ivaad = edb.ncm_ivaad;
            tmp.gcias = edb.gcias;
            tmp.ncm_sp1	= edb.ncm_sp1;
            tmp.ncm_sp2	= edb.ncm_sp2;
            tmp.precio_u = edb.precio_u;
            tmp.extrag_comex1 =	edb.extrag_comex1;
            tmp.extrag_comex2 =	 edb.extrag_comex2;
            tmp.extrag_comex3 =	 edb.extrag_comex3;
            tmp.extrag_comex_notas = edb.extrag_comex_notas;
            tmp.extrag_src1 = edb.extrag_src1;
            tmp.extrag_src2 = edb.extrag_src2;
            tmp.extrag_finan1 = edb.extrag_finan1;
            tmp.extrag_finan2 = edb.extrag_finan2;
            tmp.extrag_finan3 = edb.extrag_finan3;
            tmp.extrag_finan_notas = edb.extrag_finan_notas;
            tmp.costo_u_est = edb.costo_u_est;
            tmp.costo_u_prov = edb.costo_u_prov;
            tmp.costo_u = edb.costo_u;
            tmp.updated = edb.updated;
            tmp.purchaseorder = edb.purchaseorder;
            tmp.productowner = edb.productowner;
            tmp.comercial_invoice = edb.comercial_invoice;
            tmp.proforma_invoice = edb.proforma_invoice;
            tmp.proveedor_prov = edb.proveedor_prov;
            tmp.detailorder = edb.detailorder;
            tmp.htimestamp = edb.htimestamp;

            if(edb.updated)
            {
                updated++;
            }    
            myEstV2.estDetails.Add(tmp);
            
            // Cuando piden un query "vista", aparecen mas campos en EstDetail (es un EstDetail tipo VISTA)
            // Para evitar una catarata de cambios, todo lo adicional lo leo a un obejto complementario al detail
            // Es una lista y hay uno de estos por cada detail con info que lo complementa
            // Leo la info adicional que biene del tipo "Vista" de un EstDetailDB. Tipicamente cundo se entrega un resultado
            // convirtiendo FKs a descripcion. Esto agranda el tipo de dato y lo hace incompatible.
            adata.ncm_str=edb.ncm_str;
            adata.proveedor=edb.proveedor;
            // Lista de los detalles adicionales al detail. 
            myEstV2.estDetAddData.Add(adata);
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

            tmp.id = ed.id;
            tmp.estimateheader_id = ed.estimateheader_id;
            tmp.proveedores_id	= ed.proveedores_id;
            tmp.ncm_id = ed.ncm_id;
            tmp.ncm_ack	= ed.ncm_ack;
            tmp.sku = ed.sku;
            tmp.description = ed.description;
            tmp.imageurl	= ed.imageurl;
            tmp.exw_u = ed.exw_u;
            tmp.fob_u = ed.fob_u;
            tmp.qty = ed.qty;
            tmp.pcsctn = ed.pcsctn;
            tmp.cbmctn = ed.cbmctn;
            tmp.gwctn = ed.gwctn;
            tmp.cambios_notas =	ed.cambios_notas;
            tmp.ncm_arancel = ed.ncm_arancel;
            tmp.ncm_te_dta_otro = ed.ncm_te_dta_otro;
            tmp.ncm_iva = ed.ncm_iva;
            tmp.ncm_ivaad = ed.ncm_ivaad;
            tmp.gcias = ed.gcias;
            tmp.ncm_sp1	= ed.ncm_sp1;
            tmp.ncm_sp2	= ed.ncm_sp2;
            tmp.precio_u = ed.precio_u;
            tmp.extrag_comex1 =	ed.extrag_comex1;
            tmp.extrag_comex2 =	 ed.extrag_comex2;
            tmp.extrag_comex3 =	 ed.extrag_comex3;
            tmp.extrag_comex_notas = ed.extrag_comex_notas;
            tmp.extrag_src1 = ed.extrag_src1;
            tmp.extrag_src2 = ed.extrag_src2;
            tmp.extrag_finan1 = ed.extrag_finan1;
            tmp.extrag_finan2 = ed.extrag_finan2;
            tmp.extrag_finan3 = ed.extrag_finan3;
            tmp.extrag_finan_notas = ed.extrag_finan_notas;
            tmp.costo_u_est = ed.costo_u_est;
            tmp.costo_u_prov = ed.costo_u_prov;
            tmp.costo_u = ed.costo_u;
            tmp.updated = ed.updated;
            tmp.purchaseorder = ed.purchaseorder;
            tmp.productowner = ed.productowner;
            tmp.comercial_invoice = ed.comercial_invoice;
            tmp.proforma_invoice = ed.proforma_invoice;
            tmp.proveedor_prov=ed.proveedor_prov;
            tmp.detailorder = ed.detailorder;
            //tmp.htimestamp = ed.htimestamp;
            tmp.htimestamp=DateTime.Now;
            //tmp.htimestamp=ed.htimestamp;
            estimateDB.estDetailsDB.Add(tmp);
        }
        return estimateDB;       
    }

    public EstimateV2 ClearExtraGastosSrc(EstimateV2 miEst)
    {
        miEst.estHeader.extrag_src1=0;
        miEst.estHeader.extrag_src2=0;
        miEst.estHeader.extrag_src_notas="";

        return miEst;
    }

    public EstimateV2 ClearExtraGastosComex(EstimateV2 miEst)
    {
        miEst.estHeader.extrag_comex1=0;
        miEst.estHeader.extrag_comex2=0;
        miEst.estHeader.extrag_comex3=0;
        miEst.estHeader.extrag_comex_notas="";

        return miEst;
    }

    public EstimateV2 ClearExtraGastosFinanzas(EstimateV2 miEst)
    {
        miEst.estHeader.extrag_finan1=0;
        miEst.estHeader.extrag_finan2=0;
        miEst.estHeader.extrag_finan3=0;
        miEst.estHeader.extrag_finan4=0;
        miEst.estHeader.extrag_finan5=0;
        miEst.estHeader.extrag_finan_notas="";
        miEst.estHeader.extrag_finanformula1_id=0;
        miEst.estHeader.extrag_finanformula2_id=0;
        miEst.estHeader.extrag_finanformula3_id=0;
        miEst.estHeader.extrag_finanformula4_id=0;
        miEst.estHeader.extrag_finanformula5_id=0;

        return miEst;
    }

    public EstimateV2 setDefaultEstimateDB(EstimateV2 miEst)
    {
       
        miEst=ClearExtraGastosSrc(miEst);
        miEst=ClearExtraGastosComex(miEst);
        miEst=ClearExtraGastosFinanzas(miEst);
        // IMPORTANTE !!!!! 
        // Valores por defecto: Estodo 0.
        // Todos las tarifas son actualizables (bits 0 a 7)
        // fregith_cost y freight_insurance se calculan desde las tarfias (bits 8 y 9)
        // Las tarfias se actualizaran x fecha y no por ID.
        miEst.estHeader.status=0x00;
        //miEst.estHeader.tarifrecent=1023;
        //miEst.estHeader.tarifupdate=1023;
        // FIN IMPORTANTE
        foreach(EstimateDetail edb in miEst.estDetails)
        {
            edb.extrag_comex1=0;
            edb.extrag_comex2=0;
            edb.extrag_comex3=0;
            edb.extrag_comex_notas="";
            edb.extrag_finan1=0;
            edb.extrag_finan2=0;
            edb.extrag_finan3=0;
            edb.extrag_finan_notas="";
            edb.extrag_src1=0;
            edb.extrag_src2=0;
            edb.updated=false;
        }
        return miEst;
    } 
}


 