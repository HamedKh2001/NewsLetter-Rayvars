using MediatR;

namespace SSO.Application.Features.GroupFeature.Commands.DeleteGroup
{
    public class DeleteGroupCommand : IRequest
    {
        public int Id { get; set; }
    }
}
