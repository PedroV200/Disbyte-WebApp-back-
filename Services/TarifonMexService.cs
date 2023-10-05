
namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;
using System.Net.WebSockets;

// LISTED 4_10_2023
// Tarifon entrega y salva un modelo de datos que es identico a un roq de la master tablrs de MEX
// Al decidirse que el modelo actual de 8 tarifas debi mantenerse, se crea un servicio que recopila toda la info
// de las diferentes tarifas consultadas (consultada por mas reciente), solo para MEX, GUADALAJARA y para todos los tipos de carga 
// consignados en el FCL de Mexico.
// Para grabar un row, los datos son colocados en las respectivas tablas de tarifas, y los datos adicionales son completados, o ignorados
// o en el caso de las FKs, son extraidas de la tabla de constantes donde se han archivado IDs validos para cada una de las
// tarifas consultadas.
// Hay que entender que por contemaplar todas las cargas y las tablas catalogar x carga (ademas de pais region), una entrada del tarifon 
// generar entre 2 o 4 ingresos consecutivos a una tabla, por ejemplo, el caso de flete, por que el tarifon muestra los costos para 4 tipos
// de carga, mas el modelo de Tarifa de Flete, un registro solo puede tener un tipo de carga asociado.
// Ten tercer riel, compre con pantografo.
// LISTED 5_10_2023 Se agrega un endpoint que solo devuelve los gastos locales segun la carga acotando la cantidad de consultas.
// Ese endpoint es para consumir durante una creacion o actualuzacion de presupuesto, una vez que se ha seleccionado la carga.

public class TarifonMexService:ITarifonMexService
{


    IUnitOfWork _unitOfWork;

    string haltError;
    public TarifonMexService(IUnitOfWork unitOfWork)
    {
       _unitOfWork=unitOfWork;
    }

    public string getLastErr()
    {
        return haltError;
    }



    public async Task<GastosLocales>getGloc(int carga_id)
    {
        Tarifas misTarifas=new Tarifas();
        GastosLocales misGloc=new GastosLocales();
        CONSTANTES misConst=new CONSTANTES();

        string hoy=DateTime.Now.ToString("yyyy-MM-dd");

        misConst=await _unitOfWork.Constantes.GetLastIdAsync();
        if(misConst==null)
        {
            haltError="TarifonMEX: Entrada a la tabla de constantes no encontrada";
            return null;
        }

        misTarifas.tDespa=await _unitOfWork.TarifDespa.GetByNearestDateAsync(hoy,misConst.paisreg_mex_guad);
        if(misTarifas.tDespa==null)         
        { 
            haltError=$"La tarifa Despa mas prox no encontrada"; 
            return null;
        }
        //Despa
        misGloc.gloc_despachante_fijo=misTarifas.tDespa.cargo_fijo;
        misGloc.gloc_despachante_var=misTarifas.tDespa.cargo_variable;
        misGloc.gloc_despachante_otro1=misTarifas.tDespa.clasificacion;
        misGloc.gloc_despachante_otro2=misTarifas.tDespa.consultoria;


        if((carga_id==misConst.carga20)||(carga_id==misConst.carga220)||(carga_id==misConst.carga40)||(carga_id==misConst.carga240))
        {
            //Flete y descarga 1*20FT
            misTarifas.tFlete=await _unitOfWork.TarifFlete.GetByNearestDateAsync(hoy, carga_id, misConst.paisreg_mex_guad);
            if(misTarifas.tFlete==null)         
            { 
                haltError=$"La tarifa Flete para car ID: {carga_id} mas prox no encontrada"; 
                return null;
            }
            misGloc.flete_interno=misTarifas.tFlete.flete_interno;
            misGloc.gasto_descarga_depo=misTarifas.tFlete.descarga_depo;

            // Las proximas tarfias no conocen los tipos dobles 2*20FT o 2*40HQ. Eso solo ocurre con el flete y en mexico
            // Aca si la carga es un 2* lo paso al correspondiente tipo sencillo
            if(carga_id==misConst.carga220)
            {
                carga_id=misConst.carga20;
            }
            if(carga_id==misConst.carga240)
            {
                carga_id=misConst.carga40;
            }

            misTarifas.tFwd=await _unitOfWork.TarifFwd.GetByNearestDateAsync(hoy,carga_id, misConst.paisreg_mex_guad, misConst.paisreg_china_shezhen);
            if(misTarifas.tFwd==null)        
            { 
                haltError=$"Tartifon MEX: Tarifa Flete internacional mas reciente para carga ID:{carga_id} no encontrada)"; 
                return null;
            } 
            misGloc.freight_charge=misTarifas.tFwd.costo;
            misGloc.gloc_fwd=misTarifas.tFwd.costo_local;
            misGloc.insurance_charge=misTarifas.tFwd.seguro_porct;

            misTarifas.tTerminal=await _unitOfWork.TarifTerminal.GetByNearestDateAsync(hoy,carga_id, misConst.paisreg_mex_guad);
            if(misTarifas.tTerminal==null)        
            { 
                haltError=$"La tarifa Terminal para carga con ID {carga_id} mas prox no encontrada"; 
                return null;
            } 
            misGloc.gasto_terminal=misTarifas.tTerminal.gasto_fijo;

            misGloc.htimestamp=misTarifas.tTerminal.htimestamp;

            return misGloc;
        }
        else
        {
            haltError=$"Carga con ID: {carga_id} no soportada. Cargas soportadas: 20FT, 40HQ, 2*20FT, 2*40HQ";
            return null;
        }        
    }


    // Consulta todas las tablas de tarifas y segun el caso en diferentes carga para generar un row del tarifario de MEX.
    // Esto acarrea una gran cantidad de consultas dado que las tablas se organizaron x carga y region pero mejorara la usabilida
    // 3_10_2023. 
    // Siempre se usa el criterio de la fecha mas proxima. No se pueden guardar los ids por que para los datos del tarifon son 
    // extraidos de mas de una fila de las diferentes tablas debido a la alta redndancia de datos que presenta la planilla tarfias MEX
    public async Task<TarifonMex> getTarifon()
    {
        //#### RREGLAR !!! => misTarifasByDate=await _unitOfWork.TarifasPorFecha.GetByIdAsync(miEst.estHeader.tarifasbydateid);

        Tarifas misTarifas=new Tarifas();
        TarifonMex tarifonMX=new TarifonMex();
        CONSTANTES misConst=new CONSTANTES();

        string hoy=DateTime.Now.ToString("yyyy-MM-dd");

        misConst=await _unitOfWork.Constantes.GetLastIdAsync();
        if(misConst==null)
        {
            haltError="TarifonMEX: Entrada a la tabla de constantes no encontrada";
            return null;
        }

        misTarifas.tDespa=await _unitOfWork.TarifDespa.GetByNearestDateAsync(hoy,misConst.paisreg_mex_guad);
        if(misTarifas.tDespa==null)         
        { 
            haltError=$"La tarifa Despa mas prox no encontrada"; 
            return null;
        }
        //Despa
        tarifonMX.despa_fijo=misTarifas.tDespa.cargo_fijo;
        tarifonMX.despa_var=misTarifas.tDespa.cargo_variable;
        tarifonMX.despa_clasific_oper=misTarifas.tDespa.clasificacion;
        tarifonMX.despa_consult_compl=misTarifas.tDespa.consultoria;

        //Flete internacional 20FT
        misTarifas.tFwd=await _unitOfWork.TarifFwd.GetByNearestDateAsync(hoy,misConst.carga20, misConst.paisreg_mex_guad, misConst.paisreg_china_shezhen);
        if(misTarifas.tFwd==null)        
        { 
            haltError="Tartifon MEX: Tarifa Flete internacional mas reciente 20FT no encontrada)"; 
            return null;
        } 
        tarifonMX.flete_internacional_20ft=misTarifas.tFwd.costo;
        tarifonMX.gastosLocales_20ft=misTarifas.tFwd.costo_local;
        //Flete internacional 40ST/HQ
        misTarifas.tFwd=await _unitOfWork.TarifFwd.GetByNearestDateAsync(hoy,misConst.carga40, misConst.paisreg_mex_guad, misConst.paisreg_china_shezhen);
        if(misTarifas.tTerminal==null)        
        { 
            haltError="Tartifon MEX: Tarifa flete internacional mas reciente 40ST/HQ no encontrada)"; 
            return null;
        } 
        tarifonMX.flete_internacional_40sthq=misTarifas.tFwd.costo;
        tarifonMX.gastosLocales_40sthq=misTarifas.tFwd.costo_local;
        //Seguro (cualquier entrada de la tabla tarfia de flete internacional lo tiene, es el mismo valor)
        tarifonMX.seguro=misTarifas.tFwd.seguro_porct;

        //Terminal 20FT
        misTarifas.tTerminal=await _unitOfWork.TarifTerminal.GetByNearestDateAsync(hoy, misConst.carga20, misConst.paisreg_mex_guad);
        if(misTarifas.tTerminal==null)        
        { 
            haltError=$"La tarifa Terminal para 20FT mas prox no encontrada"; 
            return null;
        } 
        tarifonMX.terminal_20ft=misTarifas.tTerminal.gasto_fijo;
        //Terminal 40FT
        misTarifas.tTerminal=await _unitOfWork.TarifTerminal.GetByNearestDateAsync(hoy, misConst.carga40, misConst.paisreg_mex_guad);
        if(misTarifas.tTerminal==null)        
        { 
            haltError=$"La tarifa Terminal para 40HQ mas prox no encontrada"; 
            return null;
        } 
        tarifonMX.terminal_40sthq=misTarifas.tTerminal.gasto_fijo;

        //Flete y descarga 1*20FT
        misTarifas.tFlete=await _unitOfWork.TarifFlete.GetByNearestDateAsync(hoy, misConst.carga20, misConst.paisreg_mex_guad);
        if(misTarifas.tFlete==null)         
        { 
            haltError=$"La tarifa Flete 1p20ft mas prox no encontrada"; 
            return null;
        }
        tarifonMX.flete_interno_1p20ft_guad=misTarifas.tFlete.flete_interno;
        tarifonMX.descarga_meli_20ft_guad=misTarifas.tFlete.descarga_depo;
        //Flete y descarga 2*20FT
        misTarifas.tFlete=await _unitOfWork.TarifFlete.GetByNearestDateAsync(hoy, misConst.carga220, misConst.paisreg_mex_guad);
        if(misTarifas.tFlete==null)         
        { 
            haltError=$"La tarifa Flete 2p20ft mas prox no encontrada"; 
            return null;
        }
        tarifonMX.flete_interno_2p20ft_guad=misTarifas.tFlete.flete_interno;
        //Flete y descarga 1*40FT
        misTarifas.tFlete=await _unitOfWork.TarifFlete.GetByNearestDateAsync(hoy, misConst.carga40, misConst.paisreg_mex_guad);
        if(misTarifas.tFlete==null)         
        { 
            haltError=$"La tarifa Flete 1p40sthq mas prox no encontrada"; 
            return null;
        }
        tarifonMX.flete_interno_1p40sthq_guad=misTarifas.tFlete.flete_interno;
        tarifonMX.descarga_meli_40sthq_guad=misTarifas.tFlete.descarga_depo;
        //Flete y descarga 2*40FT
        misTarifas.tFlete=await _unitOfWork.TarifFlete.GetByNearestDateAsync(hoy, misConst.carga240, misConst.paisreg_mex_guad);
        if(misTarifas.tFlete==null)         
        { 
            haltError=$"La tarifa Flete 2p40sthq mas prox no encontrada"; 
            return null;
        }
        tarifonMX.flete_interno_2p40sthq_guad=misTarifas.tFlete.flete_interno;

        tarifonMX.htimestamp=misTarifas.tDespa.htimestamp;
        
        return tarifonMX;
    }


    public async Task<bool> putTarifon(TarifonMex tarifonMX)
    {
        Tarifas misTarifas=new Tarifas();        
        CONSTANTES misConst=new CONSTANTES();
        DateTime hoy=DateTime.Now;
        int res=0;
        misConst=await _unitOfWork.Constantes.GetLastIdAsync();
        if(misConst==null)
        {
            haltError="TarifonMEX: Entrada a la tabla de constantes no encontrada";
            return false;
        }

        
        // Preparo la tarifa flete internacional del 20FT
        misTarifas.tFwd.costo=tarifonMX.flete_internacional_20ft;
        misTarifas.tFwd.costo_local=tarifonMX.gastosLocales_20ft;
        misTarifas.tFwd.paisregion_id=misConst.paisreg_mex_guad;
        misTarifas.tFwd.paisfwd_id=misConst.paisreg_china_shezhen;
        misTarifas.tFwd.carga_id=misConst.carga20;
        misTarifas.tFwd.fwdtte_id=misConst.fwdtte_id;
        misTarifas.tFwd.seguro_porct=tarifonMX.seguro;
        misTarifas.tFwd.description="Ingresado Aut. via Tarif Mex Service";
        misTarifas.tFwd.notas="NA";
        misTarifas.tFwd.htimestamp=hoy;

        res=await _unitOfWork.TarifFwd.AddAsync(misTarifas.tFwd);
        if(res==0)
        {
            haltError="Tarifon MEX: Fallo al insertar tarfia fwd 20FT";
            return false;
        }

        // Preparo la tarifa flete internacional 40STHQ
        misTarifas.tFwd.costo=tarifonMX.flete_internacional_40sthq;
        misTarifas.tFwd.costo_local=tarifonMX.gastosLocales_40sthq;
        misTarifas.tFwd.paisregion_id=misConst.paisreg_mex_guad;
        misTarifas.tFwd.paisfwd_id=misConst.paisreg_china_shezhen;
        misTarifas.tFwd.carga_id=misConst.carga40;
        misTarifas.tFwd.fwdtte_id=misConst.fwdtte_id;
        misTarifas.tFwd.seguro_porct=tarifonMX.seguro;
        misTarifas.tFwd.description="Ingresado Aut. via Tarif Mex Service";
        misTarifas.tFwd.notas="NA";
        misTarifas.tFwd.htimestamp=hoy;

        res=await _unitOfWork.TarifFwd.AddAsync(misTarifas.tFwd);
        if(res==0)
        {
            haltError="Tarifon MEX: Fallo al insertar tarfia fwd 40STHQ";
            return false;
        }

        // Genero la tarifa terminal de un 20ST
        misTarifas.tTerminal.gasto_fijo=tarifonMX.terminal_20ft;
        misTarifas.tTerminal.carga_id=misConst.carga20;
        misTarifas.tTerminal.terminal_id=misConst.terminal_id;
        misTarifas.tTerminal.paisregion_id=misConst.paisreg_mex_guad;
        misTarifas.tTerminal.description="Ingresado Aut. via Tarif Mex Service";
        misTarifas.tFwd.notas="NA";
        misTarifas.tTerminal.htimestamp=hoy;

        res=await _unitOfWork.TarifTerminal.AddAsync(misTarifas.tTerminal);
        if(res==0)
        {
            haltError="Tarifon MEX: Fallo al insertar tarifa Temrinal 20FT";
            return false;
        }

        // Genero la tarifa termimal de un 40STHQ
        misTarifas.tTerminal.gasto_fijo=tarifonMX.terminal_40sthq;
        misTarifas.tTerminal.carga_id=misConst.carga40;
        misTarifas.tTerminal.terminal_id=misConst.terminal_id;
        misTarifas.tTerminal.paisregion_id=misConst.paisreg_mex_guad;
        misTarifas.tTerminal.description="Ingresado Aut. via Tarif Mex Service";
        misTarifas.tFwd.notas="NA";
        misTarifas.tTerminal.htimestamp=hoy;

        res=await _unitOfWork.TarifTerminal.AddAsync(misTarifas.tTerminal);
        if(res==0)
        {
            haltError="Tarifon MEX: Fallo al insertar tarifa Temrinal 40ST/HQ";
            return false;
        }
        // Preparo la tarifa de un flete local sencillo 20FT
        misTarifas.tFlete.flete_interno=tarifonMX.flete_interno_1p20ft_guad;
        misTarifas.tFlete.descarga_depo=tarifonMX.descarga_meli_20ft_guad;
        misTarifas.tFlete.description_depo="MELI MEX GUADALAJARA";
        misTarifas.tFlete.notas="NA";
        misTarifas.tFlete.paisregion_id=misConst.paisreg_mex_guad;
        misTarifas.tFlete.flete_id=misConst.flete_id;
        misTarifas.tFlete.carga_id=misConst.carga20;
        misTarifas.tFlete.trucksemi_id=misConst.trucksemi_id;
        misTarifas.tFlete.description="Ingresado Aut. via Tarif Mex Service";
        misTarifas.tFlete.htimestamp=hoy;

        res=await _unitOfWork.TarifFlete.AddAsync(misTarifas.tFlete);
        if(res==0)
        {
            haltError="Tarifon MEX: fallo al insertar tarfia flete MEX GUAD 1*20FT";
            return false;
        }
        // Preparo la tarfia de un flete sencillo 40ST/HQ
        misTarifas.tFlete.flete_interno=tarifonMX.flete_interno_1p40sthq_guad;
        misTarifas.tFlete.descarga_depo=tarifonMX.descarga_meli_40sthq_guad;
        misTarifas.tFlete.description_depo="MELI MEX GUADALAJARA";
        misTarifas.tFlete.notas="NA";
        misTarifas.tFlete.paisregion_id=misConst.paisreg_mex_guad;
        misTarifas.tFlete.flete_id=misConst.flete_id;
        misTarifas.tFlete.carga_id=misConst.carga40;
        misTarifas.tFlete.trucksemi_id=misConst.trucksemi_id;
        misTarifas.tFlete.description="Ingresado Aut. via Tarif Mex Service";
        misTarifas.tFlete.htimestamp=hoy;

        res=await _unitOfWork.TarifFlete.AddAsync(misTarifas.tFlete);
        if(res==0)
        {
            haltError="Tarifon MEX: fallo al insertar tarfia flete MEX GUAD 1*40ST/HQ";
            return false;
        }
        // Preparo la tarfia de un flete doble 20FT
        misTarifas.tFlete.flete_interno=tarifonMX.flete_interno_2p20ft_guad;
        misTarifas.tFlete.descarga_depo=0;
        misTarifas.tFlete.description_depo="NA";
        misTarifas.tFlete.notas="NA";
        misTarifas.tFlete.paisregion_id=misConst.paisreg_mex_guad;
        misTarifas.tFlete.flete_id=misConst.flete_id;
        misTarifas.tFlete.carga_id=misConst.carga220;
        misTarifas.tFlete.trucksemi_id=misConst.trucksemi_id;
        misTarifas.tFlete.description="Ingresado Aut. via Tarif Mex Service";
        misTarifas.tFlete.htimestamp=hoy;

        res=await _unitOfWork.TarifFlete.AddAsync(misTarifas.tFlete);
        if(res==0)
        {
            haltError="Tarifon MEX: fallo al insertar tarfia flete MEX GUAD 2*20FT";
            return false;
        }
        // Preparo la tarfia de un flete doble 40STHQ
        misTarifas.tFlete.flete_interno=tarifonMX.flete_interno_2p40sthq_guad;
        misTarifas.tFlete.descarga_depo=0;
        misTarifas.tFlete.description_depo="NA";
        misTarifas.tFlete.notas="NA";
        misTarifas.tFlete.paisregion_id=misConst.paisreg_mex_guad;
        misTarifas.tFlete.flete_id=misConst.flete_id;
        misTarifas.tFlete.carga_id=misConst.carga240;
        misTarifas.tFlete.trucksemi_id=misConst.trucksemi_id;
        misTarifas.tFlete.description="Ingresado Aut. via Tarif Mex Service";
        misTarifas.tFlete.htimestamp=hoy;

        res=await _unitOfWork.TarifFlete.AddAsync(misTarifas.tFlete);
        if(res==0)
        {
            haltError="Tarifon MEX: fallo al insertar tarfia flete MEX GUAD 2*40ST/HQ";
            return false;
        }

        misTarifas.tDespa.cargo_fijo=tarifonMX.despa_fijo;
        misTarifas.tDespa.cargo_variable=tarifonMX.despa_var;
        misTarifas.tDespa.clasificacion=tarifonMX.despa_clasific_oper;
        misTarifas.tDespa.consultoria=tarifonMX.despa_consult_compl;
        misTarifas.tDespa.paisregion_id=misConst.paisreg_mex_guad;
        misTarifas.tDespa.despachantes_id=misConst.despachantes_id;
        misTarifas.tDespa.description="Ingresado Aut. via Tarif Mex Service";
        misTarifas.tDespa.notas="NA";
        misTarifas.tDespa.htimestamp=hoy;

        res=await _unitOfWork.TarifDespa.AddAsync(misTarifas.tDespa);
        if(res==0)
        {
            haltError="Tarifon MEX: fallo al insertar tarfia flete MEX GUAD 2*40ST/HQ";
            return false;
        }
        return true;
    }
}


