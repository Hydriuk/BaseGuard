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
using UnityEngine.Analytics;

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
        //private readonly float _maxRange;
        //private readonly float _sqrMaxRange;

        public GuardProvider(IConfigurationProvider configuration)
        {
            _guardAssets = configuration.Guards.ToDictionary(guard => guard.Id);

            _maxGuardByType = 0;

            //_maxRange = configuration.Guards.Count > 0 ? configuration.Guards.Max(guard => guard.Range) : 0;
            //_sqrMaxRange = Mathf.Pow(_maxRange, 2);

            if (Level.isLoaded)
                InitGuards();
            else
                Level.onLevelLoaded += LateLoad;
        }

        private void LateLoad(int level)
        {
            Level.onLevelLoaded -= LateLoad;
            InitGuards();
        }

        private void InitGuards()
        {
            foreach (var barricadeRegion in BarricadeManager.BarricadeRegions)
            {
                foreach (var drop in barricadeRegion.drops)
                {
                    if (!_guardAssets.TryGetValue(drop.asset.id, out GuardAsset guardAsset))
                        continue;

                    // TODO init active state
                    Guard guard = new Guard(guardAsset, drop.instanceID, true);

                    List<uint> protectedBuildables = FindProtectedBuildables(drop.model.position, guardAsset.Range);

                    foreach (var id in protectedBuildables)
                    {
                        if (_buildableGuardsProvider.TryGetValue(id, out List<Guard> guards))
                            guards.Add(guard);
                        else
                            _buildableGuardsProvider.Add(id, new List<Guard> { guard });
                    }
                }
            }
        }

        private List<uint> FindProtectedBuildables(Vector3 position, float range)
        {
            float sqrRange = range * range;
            List<RegionCoordinate> regions = new List<RegionCoordinate>();
            Regions.getRegionsInRadius(position, range, regions);

            List<Transform> barricades = new List<Transform>();
            List<Transform> structures = new List<Transform>();
            BarricadeManager.getBarricadesInRadius(position, sqrRange, regions, barricades);
            StructureManager.getStructuresInRadius(position, sqrRange, regions, structures);

            List<uint> protectedBuildables = new List<uint>();
            foreach (var barricade in barricades)
            {
                BarricadeDrop protectedDrop = BarricadeManager.FindBarricadeByRootTransform(barricade);
                protectedBuildables.Add(protectedDrop.instanceID);
            }

            foreach (var structure in structures)
            {
                StructureDrop protectedDrop = StructureManager.FindStructureByRootTransform(structure);
                protectedBuildables.Add(protectedDrop.instanceID);
            }

            return protectedBuildables;
        }

        public List<Guard> GetGuards(uint buildableInstanceId, Vector3 position)
        {
            _buildableGuardsProvider.TryGetValue(buildableInstanceId, out List<Guard> guards);

            return guards;
        }
    }
}
