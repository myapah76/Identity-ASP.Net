using Application.Dtos.Role.Request;
using Application.Dtos.Role.Respone;
using AutoMapper;
using Domain.Entities;
namespace Application.Mappers
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleRep>();
            CreateMap<CreateRoleReq, Role>();
            CreateMap<UpdateRoleReq, Role>();
        }
    }
}
