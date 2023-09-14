using AutoMapper;
using SharedKernel.Common;
using SSO.Application.Features.GroupFeature.Commands.UpdateGroup;
using SSO.Application.Features.GroupFeature.Queries.GetGroup;
using SSO.Application.Features.GroupFeature.Queries.GetGroupRoles;
using SSO.Application.Features.RoleFeature.Commands.UpdateRole;
using SSO.Application.Features.RoleFeature.Queries.GetRoleGroups;
using SSO.Application.Features.RoleFeature.Queries.GetRoles;
using SSO.Application.Features.RoleFeature.Queries.GetRoleUsers;
using SSO.Application.Features.UserFeature.Commands.CreateUser;
using SSO.Application.Features.UserFeature.Commands.UpdateUser;
using SSO.Application.Features.UserFeature.Queries.GetUser;
using SSO.Domain.Entities;
using System;

namespace SSO.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap(typeof(PaginatedResult<>), typeof(PaginatedList<>));
            CreateMap<DateOnly, DateTime>().ConvertUsing(input => input.ToDateTime(TimeOnly.Parse("00:00 AM")));
            CreateMap<DateOnly?, DateTime?>().ConvertUsing(input => input.HasValue ? input.Value.ToDateTime(TimeOnly.Parse("00:00 AM")) : null);
            CreateMap<DateTime, DateOnly>().ConvertUsing(input => DateOnly.FromDateTime(input));
            CreateMap<DateTime?, DateOnly?>().ConvertUsing(input => input.HasValue ? DateOnly.FromDateTime(input.Value) : null);

            CreateMap<Group, GroupDto>();
            CreateMap<Group, GroupRolesDto>();
            CreateMap<UpdateGroupCommand, Group>();
            CreateMap<Group, GetRoleUsersDto>()
                .ForMember(g => g.GroupId, opt => opt.MapFrom(src => src.Id))
                .ForMember(g => g.GroupCaption, opt => opt.MapFrom(src => src.Caption));
            CreateMap<Group, GetRoleGroupsDto>()
                .ForMember(g => g.GroupId, opt => opt.MapFrom(src => src.Id))
                .ForMember(g => g.GroupCaption, opt => opt.MapFrom(src => src.Caption));

            CreateMap<Role, RoleDto>();
            CreateMap<UpdateRoleCommand, Role>();

            CreateMap<User, UserDto>()
                .ForMember(u => u.Gender, opt => opt.MapFrom(src => (byte)src.Gender));
            CreateMap<CreateUserCommand, User>();
            CreateMap<UpdateUserCommand, User>();
        }
    }
}
