using BaseGuard.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGuard.Services
{
    public static class GuardFactory
    {
        public static Guard CreateGuard(GuardAsset guardAsset, uint instanceId)
        {
            return new Guard()
            {
                Id = guardAsset.Id,
                InstanceId = instanceId,
                Range = guardAsset.Range,
                Shield = guardAsset.Shield,
                IsActive = false
            };
        }
    }
}
