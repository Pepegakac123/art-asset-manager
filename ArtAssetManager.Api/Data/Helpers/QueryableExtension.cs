using Microsoft.EntityFrameworkCore;
using ArtAssetManager.Api.Entities;
using System.Linq;

namespace ArtAssetManager.Api.Data.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<Asset> ApplyFilteringAndSorting(this IQueryable<Asset> query, AssetQueryParameters queryParams)
        {
            if (!string.IsNullOrEmpty(queryParams.FileName))
            {
                var keyword = $"%{queryParams.FileName}%";
                query = query.Where(a => EF.Functions.Like(a.FileName, keyword));
            }

            // Filtrowanie po Tagach (logika MatchAll/MatchAny)
            if (queryParams.Tags?.Count > 0)
            {
                if (queryParams.MatchAll)
                {
                    foreach (var tagName in queryParams.Tags)
                    {
                        query = query.Where(a => a.Tags.Any(t => tagName == t.Name));
                    }
                }
                else
                {
                    query = query.Where(a => a.Tags.Any(t => queryParams.Tags.Contains(t.Name)));
                }
            }

            // Filtrowanie po FileType
            if (queryParams.FileType?.Count > 0)
            {
                query = query.Where(a => queryParams.FileType.Any(type => a.FileType == type));
            }

            // Filtrowanie po Rating
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

            // Filtrowanie po FileSize
            if (queryParams.FileSizeMin != null)
            {
                query = query.Where(a => a.FileSize >= Math.Abs((decimal)queryParams.FileSizeMin));
            }
            if (queryParams.FileSizeMax != null)
            {
                query = query.Where(a => a.FileSize <= Math.Abs((decimal)queryParams.FileSizeMax));
            }

            // Filtrowanie po polu Metadata
            if (!string.IsNullOrEmpty(queryParams.MetadataValue))
            {
                var keyword = $"%{queryParams.MetadataValue}%";
                query = query.Where(a => EF.Functions.Like(a.MetadataJson, keyword));
            }

            // Filtrowanie po hashData
            if (!string.IsNullOrEmpty(queryParams.FileHash))
            {

                query = query.Where(a => a.FileHash == queryParams.FileHash);
            }

            // Filtrowanie po DateRange
            if (queryParams.DateFrom != null)
            {
                query = query.Where(a => a.DateAdded >= queryParams.DateFrom);
            }
            if (queryParams.DateTo != null)
            {
                query = query.Where(a => a.DateAdded <= queryParams.DateTo);
            }

            // SORTOWANIE
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
                // Domyślne sortowanie
                query = query.OrderByDescending(a => a.DateAdded);
            }

            return query;
        }
    }
}