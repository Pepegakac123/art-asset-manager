using AutoMapper;
using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Tutaj dodaj CreateMap dla każdego DTO
            CreateMap<Asset, AssetDetailsDto>();
            CreateMap<Tag, TagDto>();
            CreateMap<Asset, AssetDto>().ForMember(
                dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.ThumbnailPath)
            );
            CreateMap<ScanFolder, ScanFolderDto>()
                .ReverseMap();
        }
    }
}