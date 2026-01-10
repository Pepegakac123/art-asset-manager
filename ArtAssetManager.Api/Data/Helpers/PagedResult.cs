namespace ArtAssetManager.Api.Data.Helpers
{
    // Wrapper dla wyników zapytania z paginacją
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; } // Całkowita liczba elementów spełniających kryteria (nie tylko na tej stronie)
    }
}
