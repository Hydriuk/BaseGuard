using BaseGuard.API;
using BaseGuard.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseGuard.Services.DamageReducers
{
    public class RatioDamageReducer : BaseDamageReducer, IDamageReducer
    {
        private readonly IGuardProvider _guardProvider;

        public RatioDamageReducer(IConfigurationProvider configuration, IGuardProvider guardProvider) : base(configuration)
        {
            _guardProvider = guardProvider;
        }

        public override float ReduceDamage(ushort damage, ushort assetId, uint buildableInstanceId)
        {
            float newDamage = base.ReduceDamage(damage, assetId, buildableInstanceId); ;

            IEnumerable<Guard> guards = _guardProvider.GetGuards(buildableInstanceId);
            if (guards == null || guards.Count() == 0)
                return newDamage;

            float protectionCoefficient = guards
                .Select(guard => guard.Shield)
                .Aggregate((totalShield, guardShield) => totalShield += (1 - totalShield) * guardShield);

            protectionCoefficient = Mathf.Clamp01(protectionCoefficient);

            return newDamage * (1 - protectionCoefficient);
        }
    }
}