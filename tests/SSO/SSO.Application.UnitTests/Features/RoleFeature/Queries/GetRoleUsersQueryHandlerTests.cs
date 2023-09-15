using AutoMapper;
using Moq;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.RoleFeature.Queries.GetRoleUsers;
using SSO.Application.Features.UserFeature.Queries.GetUser;
using SSO.Application.UnitTests.Common;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.RoleFeature.Queries
{
    public class GetRoleUsersQueryHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly GetRoleUsersQueryHandler _handler;
        private readonly IMapper _mapper;

        public GetRoleUsersQueryHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapper = _mapper.GetMapper();
            _handler = new GetRoleUsersQueryHandler(_roleRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_WithRoleId_ReturnsMappedDto()
        {
            // Arrange
            var roleId = 1;
            var role = new Role
            {
                Id = roleId,
                Discriminator = "Discriminator",
                DisplayTitle = "DisplayTitle",
                Title = "Title",
                Groups = new List<Group> { new Group { Id = 1, Caption = "Group1", Users = new List<User> { new User { Id = 1 } } } }
            };
            var query = new GetRoleUsersQuery { RoleId = roleId };
            var mappedDto = new GetRoleUsersDto { GroupId = 1, GroupCaption = "Group1", Users = new List<UserDto> { new UserDto { Id = 1 } } };
            _roleRepositoryMock.Setup(mock => mock.GetWithUsersAsync(roleId, CancellationToken.None)).ReturnsAsync(role);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var dto = result[0];
            Assert.Equal(mappedDto.GroupId, dto.GroupId);
            Assert.Equal(mappedDto.GroupCaption, dto.GroupCaption);
            Assert.Collection(dto.Users, user =>
            {
                Assert.Equal(mappedDto.Users[0].Id, user.Id);
            });
        }

        [Fact]
        public async Task Handle_WithUnknownRoleId_ThrowsNotFoundException()
        {
            // Arrange
            var roleId = 1;
            var query = new GetRoleUsersQuery { RoleId = roleId };
            _roleRepositoryMock.Setup(mock => mock.GetWithUsersAsync(roleId, CancellationToken.None)).ReturnsAsync(null as Role);

            // Act and assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }

}
