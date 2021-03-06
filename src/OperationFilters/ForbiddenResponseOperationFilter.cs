﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Delobytes.AspNetCore.Swagger.OperationFilters
{
    /// <summary>
    /// Adds a 403 Forbidden response to the Swagger response documentation when the authorization policy contains a
    /// <see cref="ClaimsAuthorizationRequirement"/>, <see cref="NameAuthorizationRequirement"/>,
    /// <see cref="OperationAuthorizationRequirement"/>, <see cref="RolesAuthorizationRequirement"/> or
    /// <see cref="AssertionRequirement"/>.
    /// </summary>
    /// <seealso cref="IOperationFilter" />
    public class ForbiddenResponseOperationFilter : IOperationFilter
    {
        private const string ForbiddenStatusCode = "403";
        private static readonly OpenApiResponse ForbiddenResponse = new OpenApiResponse()
        {
            Description = "Forbidden - The user does not have the necessary permissions to access the resource."
        };

        /// <summary>
        /// Apply the specified operation.
        /// </summary>
        /// <param name="operation">Operation.</param>
        /// <param name="context">Context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            IList<IAuthorizationRequirement> authorizationRequirements = filterDescriptors.GetPolicyRequirements();
            if (!operation.Responses.ContainsKey(ForbiddenStatusCode) &&
                HasAuthorizationRequirement(authorizationRequirements))
            {
                operation.Responses.Add(ForbiddenStatusCode, ForbiddenResponse);
            }
        }

        private static bool HasAuthorizationRequirement(IEnumerable<IAuthorizationRequirement> authorizationRequirements)
        {
            foreach (IAuthorizationRequirement authorizationRequirement in authorizationRequirements)
            {
                if (authorizationRequirement is ClaimsAuthorizationRequirement ||
                    authorizationRequirement is NameAuthorizationRequirement ||
                    authorizationRequirement is OperationAuthorizationRequirement ||
                    authorizationRequirement is RolesAuthorizationRequirement ||
                    authorizationRequirement is AssertionRequirement)
                {
                    return true;
                }
            }

            return false;
        }
    }
}