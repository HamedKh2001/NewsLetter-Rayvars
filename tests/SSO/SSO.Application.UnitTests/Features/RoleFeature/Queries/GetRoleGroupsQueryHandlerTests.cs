using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.RoleFeature.Queries.GetRoleGroups;
using SSO.Domain.Entities;
using FluentAssertions;
using Moq;
using SharedKernel.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using SSO.Application.UnitTests.Common;

namespace SSO.Application.UnitTests.Features.RoleFeature.Queries
{
    public class GetRoleGroupsQueryHandlerTests
    {
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly GetRoleGroupsQueryHandler _handler;
        private readonly IMapper _mapper;

        public GetRoleGroupsQueryHandlerTests()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mapper = _mapper.GetMapper();
            _handler = new GetRoleGroupsQueryHandler(_mockRoleRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_WhenRoleExists_ShouldReturnListOfGetRoleGroupsDto()
        {
            // Arrange
            var roleId = 1;
            var request = new GetRoleGroupsQuery { RoleId = roleId };
            var role = new Role { Id = roleId, DisplayTitle = "Role Name", Groups = new List<Group> { new Group { Id = 1, Caption = "Group 1" }, new Group { Id = 2, Caption = "Group 2" } } };
            _mockRoleRepository.Setup(x => x.GetWithGroupsAsync(roleId, CancellationToken.None)).ReturnsAsync(role);
            var expectedDtoList = new List<GetRoleGroupsDto> { new GetRoleGroupsDto { GroupId = 1, GroupCaption = "Group 1" }, new GetRoleGroupsDto { GroupId = 2, GroupCaption = "Group 2" } };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDtoList);
        }

        [Fact]
        public async Task Handle_WhenRoleDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var roleId = 1;
            var request = new GetRoleGroupsQuery { RoleId = roleId };
            _mockRoleRepository.Setup(x => x.GetWithGroupsAsync(roleId, CancellationToken.None)).ReturnsAsync((Role)null);

            // Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
        }
    }

}
