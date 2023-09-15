using AutoMapper;
using FluentAssertions;
using Moq;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.GroupFeature.Queries.GetGroup;
using SSO.Application.UnitTests.Common;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.GroupFeature.Queries
{
    public class GetGroupTests
    {
        private readonly Mock<IGroupRepository> _groupRepository;
        private readonly IMapper _mapper;

        public GetGroupTests()
        {
            _groupRepository = new Mock<IGroupRepository>();
            _mapper = _mapper.GetMapper();
        }

        [Fact]
        public async Task Handle_InvalidGroupId_ThrowNotFoundException()
        {
            //Arrange
            var request = new GetGroupQuery { Id = -1 };
            _groupRepository.Setup(repo => repo.GetWithRolesAsync(request.Id, CancellationToken.None)).ReturnsAsync(() => null);
            var handler = new GetGroupQueryHandler(_groupRepository.Object, _mapper);

            //Act
            var result = FluentActions.Invoking(() => handler.Handle(request, CancellationToken.None));

            //Assert
            await result.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_ValidGroupId_ReturnGroupDtoWithRoles()
        {
            //Arrange
            var request = new GetGroupQuery { Id = 1 };
            var entity = SingleGroupEntityWithRoles(request.Id);
            _groupRepository.Setup(repo => repo.GetAsync(request.Id, CancellationToken.None)).ReturnsAsync(entity);
            var handler = new GetGroupQueryHandler(_groupRepository.Object, _mapper);

            //Act
            var result = await handler.Handle(request, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<GroupDto>();
            result.Id.Should().Be(entity.Id);
            result.Caption.Should().Be(entity.Caption);
            result.Roles.Should().HaveCount(entity.Roles.Count);
        }

        #region Privates

        private Group SingleGroupEntityWithRoles(int Id)
        {
            List<Role> roles = new() {
                new() { Id = 1, Title="Role_1" },
                new() { Id = 2, Title="Role_2" },
                new() { Id = 3, Title="Role_3" }
            };


            return new Group
            {
                Id = Id,
                Caption = "Group_1",
                Roles = roles
            };
        }

        #endregion
    }
}
