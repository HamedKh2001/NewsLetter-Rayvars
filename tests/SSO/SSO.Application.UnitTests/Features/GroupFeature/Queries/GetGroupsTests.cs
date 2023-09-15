using AutoMapper;
using FluentAssertions;
using Moq;
using SharedKernel.Common;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.GroupFeature.Queries.GetGroup;
using SSO.Application.Features.GroupFeature.Queries.GetGroups;
using SSO.Application.Mapping;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.GroupFeature.Queries
{
    public class GetGroupsTests
    {
        private readonly Mock<IGroupRepository> _groupRepository;
        private readonly IMapper _mapper;

        public GetGroupsTests()
        {
            _groupRepository = new Mock<IGroupRepository>();
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task Handle_ValidFilter_ReturnPaginatedList()
        {
            //Arrange
            var request = new GetGroupsQuery { PageNumber = 1, PageSize = 10 };
            var entities = GroupEntities(request.PageNumber, request.PageSize);
            _groupRepository.Setup(repo => repo.GetAsync(request.Caption, request.PageNumber, request.PageSize, CancellationToken.None)).ReturnsAsync(entities);
            var handler = new GetGroupsQueryHandler(_groupRepository.Object, _mapper);

            //Act
            var result = await handler.Handle(request, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PaginatedList<GroupDto>>();
            result.Items.Should().HaveCount(entities.Items.Count);
        }


        #region Privates

        private PaginatedResult<Group> GroupEntities(int pageNumber, int pageSize)
        {
            List<Group> groups = new()
             {
                 new(){ Id = 1,Caption = "Group_1" },
                 new(){ Id = 2,Caption = "Group_2" },
                 new(){ Id = 3,Caption = "Group_3" },
                 new(){ Id = 4,Caption = "Group_4" },
                 new(){ Id = 5,Caption = "Group_5" },
             };

            return new PaginatedResult<Group>(groups, groups.Count, pageNumber, pageSize);
        }

        #endregion
    }
}
