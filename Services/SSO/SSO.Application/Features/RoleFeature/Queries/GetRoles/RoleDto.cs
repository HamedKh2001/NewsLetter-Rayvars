namespace SSO.Application.Features.RoleFeature.Queries.GetRoles
{
    public class RoleDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string DisplayTitle { get; set; }
        public string Discriminator { get; set; }
    }
}
