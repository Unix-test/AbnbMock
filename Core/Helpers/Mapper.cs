using AutoMapper;
using Core.Model.Dto;
using Core.Model.Schemas;

namespace Core.Helpers;

public class Mapper: Profile
{
    public Mapper()
    {
        CreateMap<RoomRequest, RoomType>();
    }
}