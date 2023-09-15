using AutoMapper;
using Moq;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.GroupFeature.Commands.UpdateGroupUsers;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.GroupFeature.Commands
{
    public class UpdateGroupUsersCommandHandlerTests
    {
        private readonly Mock<IGroupRepository> _groupRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UpdateGroupUsersCommandHandler _sut;
        private readonly IMapper _mapper;

        public UpdateGroupUsersCommandHandlerTests()
        {
            _groupRepositoryMock = new Mock<IGroupRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _sut = new UpdateGroupUsersCommandHandler(
                _groupRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_GroupNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var command = new UpdateGroupUsersCommand { GroupId = 1 };
            _groupRepositoryMock.Setup(x => x.GetWithUsersAsync(command.GroupId, CancellationToken.None))
                .ReturnsAsync((Group)null);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _sut.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_AddAndDeleteUsers_Success()
        {
            // Arrange
            var command = new UpdateGroupUsersCommand
            {
                GroupId = 1,
                UserIds = new List<int> { 2, 3 }
            };
            var existingGroup = new Group
            {
                Id = 1,
                Users = new List<User>
            {
                new User { Id = 1 },
                new User { Id = 2 },
                new User { Id = 3 }
            }
            };
            _groupRepositoryMock.Setup(x => x.GetWithUsersAsync(command.GroupId, CancellationToken.None))
                .ReturnsAsync(existingGroup);
            _userRepositoryMock.Setup(x => x.GetByUserIdsAsync(command.UserIds, CancellationToken.None))
                .ReturnsAsync(new List<User>
                {
                new User { Id = 2 },
                new User { Id = 3 }
                });

            // Act
            await _sut.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(2, existingGroup.Users.Count);
            Assert.Contains(existingGroup.Users, u => u.Id == 2);
            Assert.Contains(existingGroup.Users, u => u.Id == 3);
            Assert.DoesNotContain(existingGroup.Users, u => u.Id == 1);
            _groupRepositoryMock.Verify(x => x.UpdateAsync(existingGroup, CancellationToken.None), Times.Once);
        }

    }

}
