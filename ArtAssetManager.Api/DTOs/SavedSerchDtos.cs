namespace ArtAssetManager.Api.DTOs
{
    public class SavedSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public object Filter { get; set; } = string.Empty;
    }

    public class CreateSavedSearchRequest
    {
        public string Name { get; set; } = string.Empty;
        public object Filter { get; set; } = string.Empty;
    }

    public class UpdateSavedSearchRequest
    {
        public string Name { get; set; } = string.Empty;
        public object Filter { get; set; } = string.Empty;
    }
}