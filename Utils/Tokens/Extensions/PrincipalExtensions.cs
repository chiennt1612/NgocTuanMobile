﻿using IdentityModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace Utils.Tokens.Extensions
{
    public static class PrincipalExtensions
    {
        [DebuggerStepThrough]
        public static string GetSubjectId(this IPrincipal principal)
        {
            return principal.Identity.GetSubjectId();
        }

        [DebuggerStepThrough]
        public static string GetSubjectId(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            var claim = id.FindFirst(JwtClaimTypes.Subject);

            if (claim == null) throw new InvalidOperationException("sub claim is missing");
            return claim.Value;
        }

        [DebuggerStepThrough]
        public static long GetAuthenticationTimeEpoch(this IPrincipal principal)
        {
            return principal.Identity.GetAuthenticationTimeEpoch();
        }

        [DebuggerStepThrough]
        public static long GetAuthenticationTimeEpoch(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            var claim = id.FindFirst(JwtClaimTypes.AuthenticationTime);

            if (claim == null) throw new InvalidOperationException("auth_time is missing.");

            return long.Parse(claim.Value);
        }

        [DebuggerStepThrough]
        public static IEnumerable<Claim> GetAuthenticationMethods(this IPrincipal principal)
        {
            return principal.Identity.GetAuthenticationMethods();
        }

        [DebuggerStepThrough]
        public static IEnumerable<Claim> GetAuthenticationMethods(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            return id.FindAll(JwtClaimTypes.AuthenticationMethod);
        }
    }
}
