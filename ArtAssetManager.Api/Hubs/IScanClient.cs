namespace ArtAssetManager.Api.Hubs
{
    // Kontrakt definiujący metody, które serwer może wywołać na klientach (przeglądarkach)
    // Dzięki temu mamy typowaną komunikację SignalR
    public interface IScanClient
    {
        // Aktualizacja paska postępu skanowania
        // status: opis (np. "Indeksowanie plików...")
        // total: całkowita liczba plików
        // current: ile przetworzono do tej pory
        Task ReceiveProgress(string status, int total, int current);
        
        // Aktualizacja ogólnego stanu skanera (np. "scanning", "idle")
        Task ReceiveScanStatus(string status);
    }
}
