using AutoMapper;
using FluentAssertions;
using Moq;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.GroupFeature.Commands.CreateGroup;
using SSO.Application.Features.GroupFeature.Queries.GetGroup;
using SSO.Application.UnitTests.Common;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.GroupFeature.Commands
{
    public class CreateGroupCommandHandlerTests
    {
        private Mock<IGroupRepository> _groupRepositoryMock;
        private CreateGroupCommandHandler _handler;
        private readonly IMapper _mapper;

        public CreateGroupCommandHandlerTests()
        {
            _groupRepositoryMock = new Mock<IGroupRepository>();
            _mapper = _mapper.GetMapper();

            _handler = new CreateGroupCommandHandler(
                _groupRepositoryMock.Object,
                _mapper
            );
        }

        [Fact]
        public async Task Handle_ReturnsMappedGroupDto()
        {
            // Arrange
            var request = new CreateGroupCommand { Caption = "Test Group" };
            var group = new Group { Caption = request.Caption };
            var createdGroup = new Group { Id = 1, Caption = request.Caption };
            var expectedDto = new GroupDto { Id = 1, Caption = request.Caption, Roles = new() };

            _groupRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Group>(), CancellationToken.None))
                .ReturnsAsync(createdGroup);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDto);
        }
    }


}
