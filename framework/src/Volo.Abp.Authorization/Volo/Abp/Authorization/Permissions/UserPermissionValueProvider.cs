﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Security.Claims;

namespace Volo.Abp.Authorization.Permissions
{
    public class UserPermissionValueProvider : PermissionValueProvider
    {
        public const string ProviderName = "U";

        public override string Name => ProviderName;

        public UserPermissionValueProvider(IPermissionStore permissionStore)
            : base(permissionStore)
        {

        }

        public async override Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
        {
            var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;

            if (userId == null)
            {
                return PermissionGrantResult.Undefined;
            }

            return await PermissionStore.IsGrantedAsync(context.Permission.Name, Name, userId)
                ? PermissionGrantResult.Granted
                : PermissionGrantResult.Undefined;
        }

        public async override Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
        {
            var result = new MultiplePermissionGrantResult();
            var permissionNames = context.Permissions.Select(x => x.Name).ToList();
            foreach (var name in permissionNames)
            {
                result.Result.Add(name, PermissionGrantResult.Undefined);
            }

            var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
            if (userId == null)
            {
                return result;
            }

            return await PermissionStore.IsGrantedAsync(permissionNames.ToArray(), Name, userId);
        }
    }
}
