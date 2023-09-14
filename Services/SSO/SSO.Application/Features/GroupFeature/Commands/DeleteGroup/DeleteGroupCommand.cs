using MediatR;

namespace SSO.Application.Features.GroupFeature.Commands.DeleteGroup
{
    public class DeleteGroupCommand : IRequest
    {
        public long Id { get; set; }
    }
}
