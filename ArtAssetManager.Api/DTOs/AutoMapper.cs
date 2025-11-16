using AutoMapper;
using ArtAssetManager.Api.Entities;

namespace ArtAssetManager.Api.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<Asset, AssetDetailsDto>();
            CreateMap<Asset, ChildDto>();
            CreateMap<Tag, TagDto>();
            CreateMap<Asset, AssetDto>().ForMember(
                dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.ThumbnailPath)
            );
            CreateMap<ScanFolder, ScanFolderDto>()
                .ReverseMap();
            CreateMap<MaterialSet, MaterialSetDto>()
                        .ForMember(

                            dest => dest.TotalAssets,
                            opt => opt.MapFrom(src => src.Assets.Count)
                        );
            CreateMap<MaterialSet, MaterialSetDetailsDto>();
            CreateMap<CreateMaterialSetRequest, MaterialSet>()
                .ForMember(
                    dest => dest.DateAdded,
                    opt => opt.MapFrom(src => DateTime.UtcNow)
                )
                .ForMember(
                    dest => dest.LastModified,
                    opt => opt.MapFrom(src => DateTime.UtcNow)
                );
            CreateMap<UpdateMaterialSet, MaterialSet>();


        }
    }
}