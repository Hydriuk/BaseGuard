using BaseGuard.API;
using BaseGuard.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class GuardProvider : IGuardProvider
    {
        private readonly Dictionary<uint, List<Guard>> _buildableGuardsProvider = new Dictionary<uint, List<Guard>>();

        private readonly Dictionary<uint, Guard> _guardProvider = new Dictionary<uint, Guard>();

        // Configuration
        private readonly Dictionary<ushort, GuardAsset> _guardAssets;
        private readonly int _maxGuardByType;
        private readonly float _maxRange;
        private readonly float _sqrMaxRange;

        public GuardProvider(IConfigurationProvider configuration)
        {
            _guardAssets = configuration.Guards.ToDictionary(guard => guard.Id);

            _maxGuardByType = 0;

            _maxRange = configuration.Guards.Count > 0 ? configuration.Guards.Max(guard => guard.Range) : 0;
            _sqrMaxRange = Mathf.Pow(_maxRange, 2);
        }

        public List<Guard> GetGuards(uint buildableInstanceId, Vector3 position)
        {
            if(!_buildableGuardsProvider.TryGetValue(buildableInstanceId, out List<Guard> guards))
            {
                guards = FindGuards(position);

                _buildableGuardsProvider.Add(buildableInstanceId, guards);
            }

            return guards;
        }

        private List<Guard> FindGuards(Vector3 position)
        {
            List<RegionCoordinate> regions = new List<RegionCoordinate>();
            Regions.getRegionsInRadius(position, _maxRange, regions);

            List<Transform> barricadeTransforms = new List<Transform>();
            BarricadeManager.getBarricadesInRadius(position, _sqrMaxRange, regions, barricadeTransforms);

            List<Guard> guards = new List<Guard>();
            foreach (var barricadeTransform in barricadeTransforms)
            {
                BarricadeDrop barricade = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);

                // _maxGuardByType filter

                if (!_guardAssets.TryGetValue(barricade.asset.id, out GuardAsset guardAsset))
                    continue;

                if (!_guardProvider.TryGetValue(barricade.instanceID, out Guard guard))
                {
                    guard = GuardFactory.CreateGuard(guardAsset, barricade.instanceID);

                    _guardProvider.Add(guard.InstanceId, guard);
                }

                guards.Add(guard);
            }

            return guards;
        }
    }
}
