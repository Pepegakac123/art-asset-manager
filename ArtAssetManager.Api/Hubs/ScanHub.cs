using Microsoft.AspNetCore.SignalR;

namespace ArtAssetManager.Api.Hubs
{
    // Hub SignalR - centralny punkt komunikacji w czasie rzeczywistym
    // Dziedziczymy po Hub<IScanClient>, aby wymusić implementację interfejsu klienta
    // Front-end nasłuchuje zdarzeń zdefiniowanych w IScanClient
    public class ScanHub : Hub<IScanClient>
    {
        // Tutaj można dodać metody, które klient może wywołać na serwerze (np. anulowanie skanowania)
        // Aktualnie służy głównie do jednostronnego raportowania postępu (Server -> Client)
    }
}
