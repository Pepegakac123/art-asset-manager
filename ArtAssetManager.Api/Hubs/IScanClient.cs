namespace ArtAssetManager.Api.Hubs
{
    public interface IScanClient
    {
        Task ReceiveProgress(string status, int total, int current);
    }
}