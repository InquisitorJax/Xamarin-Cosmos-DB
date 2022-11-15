using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Wibci.LogicCommand;

namespace CosmosResourceTokenProvider
{
	internal class AuthHelper
	{
		public static UserIdResponse GetUserId(HttpRequest request)
		{
			var response = new UserIdResponse();

			string authHeader = request.Headers[HeaderNames.Authorization];
			if (authHeader != null)
			{
				//deserialize the bearer token to get access to the user id
				string accessToken = authHeader.Replace("Bearer ", string.Empty);
				var tokenHandler = new JwtSecurityTokenHandler();
				var accessTokenJwt = tokenHandler.ReadJwtToken(accessToken);

				if (!tokenHandler.CanValidateToken)
				{
					response.Notification.Add($"Unable to validate token: {accessToken}");
				}

				//oid identifies the user in AAD Auth claims: https://docs.microsoft.com/en-us/azure/active-directory/develop/access-tokens
				//NOTE: accessTokenJwt.Subject should have the same value
				var oidClaim = accessTokenJwt.Claims.SingleOrDefault(c => c.Type == "oid");
				response.UserId = oidClaim?.Value ?? accessTokenJwt.Subject;

				if (string.IsNullOrEmpty(response.UserId))
				{
					response.Notification.Add("Could not get oid from the auth header");
				}

#if DEBUG
				var expires = accessTokenJwt.ValidTo;

				var ignoreExpires = request?.Headers?["IgnoreExpires"].ToString().ToLower() == "true";

				if (DateTime.UtcNow > expires && !ignoreExpires)
				{
					response.Notification.Add("The access token have expired. " +
											  "Note this error is for debug mode only, for use when testing. " +
											  "In production access token validity is handled by Azure Functions configuration.");
				}
#endif
			}
			else
			{
				response.Notification.Add("Could not get auth header from the http request");
			}

			return response;
		}
	}

	public class UserIdResponse : CommandResult
	{
		public string UserId { get; set; }
	}
}
