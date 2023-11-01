
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
// LISTED 24_10_2023: Se incorpora la logica que calcula los gastos en casos de cargas dobles. 
// LISTED 31_10_2023: Nace una entidad esepcifica para Mexico, evitando la altisima ineficiencia del tarifon.
// Se Conserva el servicio para atender cualquier calculo entre la tarifa y lod GLOC que se envien al front 

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
        int carga_id_q=0;
        TarifasMex misTarifas=new TarifasMex();
        GastosLocales misGloc=new GastosLocales();
        CONSTANTES misConst=new CONSTANTES();

        string hoy=DateTime.Now.ToString("yyyy-MM-dd");

        misConst=await _unitOfWork.Constantes.GetLastIdAsync();
        if(misConst==null)
        {
            haltError="Tarifasmex: Entrada a la tabla de constantes no encontrada";
            return null;
        }

        misTarifas=await _unitOfWork.TarifasMexico.GetByNearestDateAsync(hoy);
         if(misConst==null)
        {
            haltError=$"Tarifasmex: No es posible encontrar una tarifa cercana a la fecha: {hoy}";
            return null;
        }

        //Gastos Despachante
        misGloc.gloc_despachante_fijo=misTarifas.despa_fijo;
        misGloc.gloc_despachante_var=misTarifas.despa_var;
        misGloc.gloc_despachante_otro1=misTarifas.despa_clasific;
        misGloc.gloc_despachante_otro2=misTarifas.despa_consult;
        //Seguro del embarque
        misGloc.insurance_charge=misTarifas.seguro;

        //Gastos que varian con la carga
        if(carga_id==misConst.carga20)
        {
            misGloc.gasto_terminal=misTarifas.terminal_1p20ft;
            misGloc.gasto_descarga_depo=misTarifas.descarga_meli_1p20ft_guad;
            misGloc.freight_charge=misTarifas.flete_1p20ft;
            misGloc.gloc_fwd=misTarifas.gloc_fwd_1p20ft;
            misGloc.flete_interno=misTarifas.fleteint_1p20ft_guad;
            
        }
        else if(carga_id==misConst.carga220)    // Los tipos 2*20 y 2*40 tienen un gastos definido para el  flete local 
        {                                       // El resto consite en multiplicar x 2 los gastos de una carga simple
            misGloc.gasto_terminal=2*misTarifas.terminal_1p20ft;
            misGloc.gasto_descarga_depo=2*misTarifas.descarga_meli_1p20ft_guad;
            misGloc.freight_charge=2*misTarifas.flete_1p20ft;
            misGloc.gloc_fwd=2*misTarifas.gloc_fwd_1p20ft;
            misGloc.flete_interno=misTarifas.fleteint_2p20ft_guad;
        }
        else if(carga_id==misConst.carga40)
        {
            misGloc.gasto_terminal=misTarifas.terminal_1p40sthq;
            misGloc.gasto_descarga_depo=misTarifas.descarga_meli_1p40sthq_guad;
            misGloc.freight_charge=misTarifas.flete_1p40sthq;
            misGloc.gloc_fwd=misTarifas.gloc_fwd_1p40sthq;
            misGloc.flete_interno=misTarifas.fleteint_1p40sthq_guad;
        }
        else if(carga_id==misConst.carga240)
        {
            misGloc.gasto_terminal=2*misTarifas.terminal_1p40sthq;
            misGloc.gasto_descarga_depo=2-misTarifas.descarga_meli_1p40sthq_guad;
            misGloc.freight_charge=2*misTarifas.flete_1p40sthq;
            misGloc.gloc_fwd=2*misTarifas.gloc_fwd_1p40sthq;
            misGloc.flete_interno=misTarifas.fleteint_2p40sthq_guad;
        }
        else
        {
            haltError=$"Tarifasmex: carga {carga_id} NO DEFINIDA";
            return null;
        }
        return misGloc;
    }


    // Consulta todas las tablas de tarifas y segun el caso en diferentes carga para generar un row del tarifario de MEX.
    // Esto acarrea una gran cantidad de consultas dado que las tablas se organizaron x carga y region pero mejorara la usabilida
    // 3_10_2023. 
    // Siempre se usa el criterio de la fecha mas proxima. No se pueden guardar los ids por que para los datos del tarifon son 
    // extraidos de mas de una fila de las diferentes tablas debido a la alta redndancia de datos que presenta la planilla tarfias MEX
    public async Task<TarifonMex> getTarifon()
    {
        //#### RREGLAR !!! => misTarifasByDate=await _unitOfWork.TarifasPorFecha.GetByIdAsync(miEst.estHeader.tarifasbydateid);
        TarifasMex misTarifas=new TarifasMex();
        TarifonMex tarifonMX=new TarifonMex();
        string hoy=DateTime.Now.ToString("yyyy-MM-dd");

        misTarifas=await _unitOfWork.TarifasMexico.GetByNearestDateAsync(hoy);


        tarifonMX.htimestamp=misTarifas.fecha;
        tarifonMX.flete_internacional_20ft=misTarifas.flete_1p20ft;
        tarifonMX.flete_internacional_40sthq=misTarifas.flete_1p40sthq;
        tarifonMX.seguro=misTarifas.seguro;
        tarifonMX.gastosLocales_20ft=misTarifas.gloc_fwd_1p20ft;
        tarifonMX.gastosLocales_40sthq=misTarifas.gloc_fwd_1p40sthq;
        tarifonMX.terminal_20ft=misTarifas.terminal_1p20ft;
        tarifonMX.terminal_40sthq=misTarifas.terminal_1p40sthq;
        tarifonMX.flete_interno_1p40sthq_guad=misTarifas.fleteint_1p40sthq_guad;
        tarifonMX.flete_interno_1p20ft_guad=misTarifas.fleteint_1p20ft_guad;
        tarifonMX.flete_interno_2p40sthq_guad=misTarifas.fleteint_2p40sthq_guad;
        tarifonMX.flete_interno_2p20ft_guad=misTarifas.fleteint_2p20ft_guad;
        tarifonMX.flete_interno_2p40sthq_cdmx=misTarifas.fleteint_2p40sthq_cdmx;
        tarifonMX.flete_interno_2p20ft_cdmx=misTarifas.fleteint_2p20ft_cdmx;
        tarifonMX.descarga_meli_40sthq_guad=misTarifas.descarga_meli_1p40sthq_guad;
        tarifonMX.descarga_meli_20ft_guad=misTarifas.descarga_meli_1p20ft_guad;
        tarifonMX.despa_fijo=misTarifas.despa_fijo;
        tarifonMX.despa_var=misTarifas.despa_var;
        tarifonMX.despa_clasific_oper=misTarifas.despa_clasific;
        tarifonMX.despa_consult_compl=misTarifas.despa_consult;
        
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


