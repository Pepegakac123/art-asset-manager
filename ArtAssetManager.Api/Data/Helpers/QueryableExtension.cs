using Microsoft.EntityFrameworkCore;
using ArtAssetManager.Api.Entities;
using System.Linq;

namespace ArtAssetManager.Api.Data.Helpers
{
    // Logika budowania dynamicznego zapytania SQL na podstawie filtrów
    public static class QueryableExtensions
    {
        public static IQueryable<Asset> ApplyFilteringAndSorting(this IQueryable<Asset> query, AssetQueryParameters queryParams)
        {
            // Wyszukiwanie po nazwie pliku (SQL LIKE)
            if (!string.IsNullOrEmpty(queryParams.FileName))
            {
                var keyword = $"%{queryParams.FileName}%";
                query = query.Where(a => EF.Functions.Like(a.FileName, keyword));
            }

            // Filtrowanie po Tagach
            if (queryParams.Tags?.Count > 0)
            {
                if (queryParams.MatchAll)
                {
                    // AND: Asset musi mieć WSZYSTKIE podane tagi
                    foreach (var tagName in queryParams.Tags)
                    {
                        query = query.Where(a => a.Tags.Any(t => tagName == t.Name));
                    }
                }
                else
                {
                    // OR: Asset musi mieć PRZYNAJMNIEJ JEDEN z podanych tagów
                    query = query.Where(a => a.Tags.Any(t => queryParams.Tags.Contains(t.Name)));
                }
            }

            // Filtrowanie po Typie pliku (np. image, model)
            if (queryParams.FileType?.Count > 0)
            {
                query = query.Where(a => queryParams.FileType.Any(type => a.FileType == type));
            }

            // Filtrowanie po Ocenie (Rating 0-5)
            if (queryParams.RatingMin != null)
            {
                var minRating = Math.Min(Math.Max(0, Math.Abs(queryParams.RatingMin.Value)), 5);

                query = query.Where(a => a.Rating >= minRating);
            }
            if (queryParams.RatingMax != null)
            {
                var maxRating = Math.Max(Math.Min(5, Math.Abs(queryParams.RatingMax.Value)), 0);
                query = query.Where(a => a.Rating <= maxRating);
            }

            // Filtrowanie po Rozmiarze pliku
            if (queryParams.FileSizeMin != null)
            {
                query = query.Where(a => a.FileSize >= Math.Abs((decimal)queryParams.FileSizeMin));
            }
            if (queryParams.FileSizeMax != null)
            {
                query = query.Where(a => a.FileSize <= Math.Abs((decimal)queryParams.FileSizeMax));
            }

            // Filtrowanie po Wymiarach (dla obrazów)
            if (queryParams.MinWidth != null)
            {
                query = query.Where(a => a.ImageWidth >= queryParams.MinWidth);
            }
            if (queryParams.MaxWidth != null)
            {
                query = query.Where(a => a.ImageWidth <= queryParams.MaxWidth);
            }
            if (queryParams.MinHeight != null)
            {
                query = query.Where(a => a.ImageHeight >= queryParams.MinHeight);
            }
            if (queryParams.MaxHeight != null)
            {
                query = query.Where(a => a.ImageHeight <= queryParams.MaxHeight);
            }

            // Filtrowanie po Kolorach
            if (queryParams.DominantColors?.Count > 0)
            {
                query = query.Where(a => a.DominantColor != null && queryParams.DominantColors.Contains(a.DominantColor));
            }

            // Filtrowanie po Kanale Alpha (przezroczystość)
            if (queryParams.HasAlphaChannel != null)
            {
                query = query.Where(a => a.HasAlphaChannel == queryParams.HasAlphaChannel);
            }

            // Filtrowanie po Hashu (wykrywanie duplikatów)
            if (!string.IsNullOrEmpty(queryParams.FileHash))
            {

                query = query.Where(a => a.FileHash == queryParams.FileHash);
            }

            // Filtrowanie po Dacie dodania
            if (queryParams.DateFrom != null)
            {
                query = query.Where(a => a.DateAdded >= queryParams.DateFrom);
            }
            if (queryParams.DateTo != null)
            {
                query = query.Where(a => a.DateAdded <= queryParams.DateTo);
            }

            // SORTOWANIE WYNIKÓW
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                var normalizedSortBy = queryParams.SortBy.ToLowerInvariant();
                query = normalizedSortBy switch
                {
                    "filename" => queryParams.SortDesc
                                    ? query.OrderByDescending(a => a.FileName)
                                    : query.OrderBy(a => a.FileName),
                    "filesize" => queryParams.SortDesc
                                    ? query.OrderByDescending(a => a.FileSize)
                                    : query.OrderBy(a => a.FileSize),
                    "lastmodified" => queryParams.SortDesc
                                    ? query.OrderByDescending(a => a.LastModified)
                                    : query.OrderBy(a => a.LastModified),
                    _ => queryParams.SortDesc
                            ? query.OrderByDescending(a => a.DateAdded)
                            : query.OrderBy(a => a.DateAdded),
                };
            }
            else
            {
                // Domyślne sortowanie: od najnowszych
                query = query.OrderByDescending(a => a.DateAdded);
            }

            return query;
        }
    }
}
