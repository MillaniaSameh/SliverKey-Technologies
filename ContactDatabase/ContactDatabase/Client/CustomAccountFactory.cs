//using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
//using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
//using System.Security.Claims;
//using System.Text.Json;

//namespace ContactDatabase.Client;

//public class CustomAccountFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
//{
//    public CustomAccountFactory(IAccessTokenProviderAccessor accessor)
//        : base(accessor)
//    { }

//    public async override ValueTask<ClaimsPrincipal> CreateUserAsync(
//      RemoteUserAccount account,
//      RemoteAuthenticationUserOptions options)
//    {
//        var userAccount = await base.CreateUserAsync(account, options);
//        var userIdentity = (ClaimsIdentity)userAccount.Identity;

//        if (userIdentity.IsAuthenticated)
//        {
//            var roles = account.AdditionalProperties[userIdentity.RoleClaimType] as JsonElement?;

//            if (roles?.ValueKind == JsonValueKind.Array)
//            {
//                userIdentity.TryRemoveClaim(userIdentity.Claims.FirstOrDefault(c => c.Type == userIdentity.RoleClaimType));
//                foreach (JsonElement element in roles.Value.EnumerateArray())
//                {
//                    userIdentity.AddClaim(new Claim(userIdentity.RoleClaimType, element.GetString()));
//                }
//            }
//        }
//        return userAccount;
//    }
//}