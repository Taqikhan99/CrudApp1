using System.Security.Claims;

namespace CrudApp1.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> claims=new List<Claim>()
        {
            new Claim("Create Role","Create Role"),
            new Claim("Edit Role","Edit Role"),
            new Claim("Delete Role","Delete Role")
        };
    }
}
