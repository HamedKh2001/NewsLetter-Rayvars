using AutoMapper;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.RoleFeature.Queries.GetRoles;
using SSO.Domain.Entities;
using FluentAssertions;
using Moq;
using SSO.Application.UnitTests.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.RoleFeature.Queries
{
    public class GetRolesQueryHandlerTests
    {
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly IMapper _mapper;
        private readonly GetRolesQueryHandler _handler;

        public GetRolesQueryHandlerTests()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mapper = _mapper.GetMapper();

            _handler = new GetRolesQueryHandler(_mockRoleRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_WhenCalled_ReturnsListOfRoleDtos()
        {
            // Arrange
            var roles = new List<Role>
        {
            new Role { Id = 1, DisplayTitle = "Admin" },
            new Role { Id = 2, DisplayTitle = "User" }
        };
            var roleDtos = new List<RoleDto>
        {
            new RoleDto { Id = 1, DisplayTitle = "Admin" },
            new RoleDto { Id = 2, DisplayTitle = "User" }
        };

            _mockRoleRepository.Setup(repo => repo.GetAsync(CancellationToken.None)).ReturnsAsync(roles);

            var query = new GetRolesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<RoleDto>>();
            result.Should().BeEquivalentTo(roleDtos);
        }
    }

}
