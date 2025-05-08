using Microsoft.AspNetCore.Authorization;

public class AllowRoleAttribute : AuthorizeAttribute
{
    public AllowRoleAttribute(Role role)
    {
        Policy = role.ToString();
    }
}
