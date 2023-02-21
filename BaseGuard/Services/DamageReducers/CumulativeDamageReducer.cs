using BaseGuard.API;
using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseGuard.Services.DamageReducers
{
    public class CumulativeDamageReducer : BaseDamageReducer, IDamageReducer
    {
        private readonly IGuardProvider _guardProvider;

        public CumulativeDamageReducer(IConfigurationAdapter<Configuration> confAdapter, IGuardProvider guardProvider) : base(confAdapter)
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
                .Aggregate((totalShield, guardShield) => totalShield += guardShield);

            protectionCoefficient = Mathf.Clamp01(protectionCoefficient);

            return newDamage * (1 - protectionCoefficient);
        }
    }
}