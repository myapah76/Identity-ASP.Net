using Application.Dtos.Role.Respone;
using Application.Dtos.User.Request;
using Application.Dtos.User.Respone;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserReq, User>();

            CreateMap<CustomerCreateUserReq, User>();

            CreateMap<UpdateUserReq, User>()
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, UserRep>()
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role));

            CreateMap<Role, RoleUserRep>();
        }
    }
}