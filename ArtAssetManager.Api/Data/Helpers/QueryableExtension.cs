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