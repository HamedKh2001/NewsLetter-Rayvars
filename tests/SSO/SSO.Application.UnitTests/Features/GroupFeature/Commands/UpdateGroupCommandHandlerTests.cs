using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.GroupFeature.Commands.UpdateGroup;
using SSO.Domain.Entities;
using Moq;
using SharedKernel.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using SSO.Application.UnitTests.Common;

namespace SSO.Application.UnitTests.Features.GroupFeature.Commands
{
    public class UpdateGroupCommandHandlerTests
    {
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly IMapper _mapper;

        private readonly UpdateGroupCommandHandler _handler;

        public UpdateGroupCommandHandlerTests()
        {
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mapper = _mapper.GetMapper();

            _handler = new UpdateGroupCommandHandler(_mockGroupRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_GroupToUpdateIsNull_ThrowsNotFoundException()
        {
            // Arrange
            var request = new UpdateGroupCommand { Id = 1 };
            _mockGroupRepository.Setup(repo => repo.GetAsync(request.Id, CancellationToken.None)).ReturnsAsync((Group)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_GroupToUpdateExists_MapsRequestAndUpdateGroup()
        {
            // Arrange
            var request = new UpdateGroupCommand { Id = 1 };
            var groupToUpdate = new Group() { Id = 1 };
            _mockGroupRepository.Setup(repo => repo.GetAsync(request.Id, CancellationToken.None)).ReturnsAsync(groupToUpdate);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockGroupRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Group>(), CancellationToken.None), Times.Once);
        }
    }

}
