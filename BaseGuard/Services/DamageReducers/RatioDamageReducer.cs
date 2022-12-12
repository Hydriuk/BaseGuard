using BaseGuard.API;
using BaseGuard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override float ReduceDamage(ushort damage, uint buildableInstanceId, Vector3 position)
        {
            float newDamage = base.ReduceDamage(damage, buildableInstanceId, position); ;

            IEnumerable<Guard> guards = _guardProvider.GetGuards(buildableInstanceId, position);
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
