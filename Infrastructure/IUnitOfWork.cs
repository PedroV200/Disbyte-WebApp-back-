using WebApiSample.Infrastructure;

namespace WebApiSample.Infrastructure;

public interface IUnitOfWork
{
    IIIBBrepository     IIBBs {get; }
    INCMrepository      NCMs {get; }
    IBancoRepository    Bancos {get; }
    IFwdtteRepository   Fwds {get;}
    ITerminalRepository Terminales {get; }
    IFleteRepository    Fletes {get; }
    ICustodiaRepository Custodias {get; }
    IGestDigitalDocRepository GestDigDoc {get; }
    IEmpresaRepository Empresas { get; }
    ITruckSemiRepository Camiones { get; }

    ICanalRepository Canales {get; }
    IProveedorRepository Proveedores {get; }
    IDepositoRepository Depositos { get; }
    IPolizaRepository Polizas {get; }
    IImpuestoRepository Impuestos {get; }
    IEstimateHeaderDBRepository EstimateHeadersDB {get; }
    IEstimateDetailDBRepository EstimateDetailsDB {get; }
    
    ICargaRepository micarga {get;}
    ITarifasPolizaRepository TarifPoliza {get;}
    ITarifasFleteRepository TarifFlete {get;}
    ITarifasTerminalRepository TarifTerminal { get; }
    ITarifasDepositoRepository TarifasDepositos {get;}
    ITarifasFwdRepository TarifFwd {get;}
    ITarifasBancoRepository TarifBancos {get;}
    ITarifasDespachanteRepository TarifDespa {get;}
    ITarifasGestDigDocRepository TarifGestDigDoc {get;}

    ITarifasMexRepository TarifasMexico {get;}

    //ITarifonMexRepository TarifonMX {get;}

    ICnstRepository Constantes {get;}
    //ITarifasSeguroRepository TarifasSeguros {get;}
    //ITarifasTerminalRepository TarifasTerminales {get;}
    //ITarifasTteRepository TarifasTransportes {get;}

    ITipoDeCambioRepository TiposDeCambio { get; }
    
    IUsuarioRepository Usuarios { get; }
    IDespachanteRepository Despachantes { get; }
    IPaisRegionRepository PaisesRegiones {get;}
    INCM_MexRepository NCM_MEXs {get;}

    IProductoRepository Productos {get;}
    
}