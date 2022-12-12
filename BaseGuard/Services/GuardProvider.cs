using BaseGuard.API;
using BaseGuard.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SocialPlatforms;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class GuardProvider : IGuardProvider
    {
        private readonly Dictionary<uint, HashSet<Guard>> _buildableGuardsProvider = new Dictionary<uint, HashSet<Guard>>();

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

            if (Level.isLoaded)
                InitGuards();
            else
                Level.onPostLevelLoaded += LateLoad;
        }

        private void LateLoad(int level)
        {
            Level.onPostLevelLoaded -= LateLoad;
            InitGuards();
        }

        private void InitGuards()
        {
            foreach (var barricadeRegion in BarricadeManager.BarricadeRegions)
                foreach (var drop in barricadeRegion.drops)
                    AddGuard(drop.asset.id, drop.instanceID, drop.model.position);
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

        private HashSet<Guard> FindGuards(Vector3 position)
        {
            List<RegionCoordinate> regions = new List<RegionCoordinate>();
            Regions.getRegionsInRadius(position, _maxRange, regions);

            List<Transform> barricades = new List<Transform>();
            List<Transform> structures = new List<Transform>();
            BarricadeManager.getBarricadesInRadius(position, _sqrMaxRange, regions, barricades);
            StructureManager.getStructuresInRadius(position, _sqrMaxRange, regions, structures);


            HashSet<Guard> guards = new HashSet<Guard>();
            foreach (var barricade in barricades)
            {
                BarricadeDrop protectedDrop = BarricadeManager.FindBarricadeByRootTransform(barricade);

                if(_guardProvider.TryGetValue(protectedDrop.instanceID, out Guard guard))
                    guards.Add(guard);
            }

            foreach (var structure in structures)
            {
                StructureDrop protectedDrop = StructureManager.FindStructureByRootTransform(structure);

                if (_guardProvider.TryGetValue(protectedDrop.instanceID, out Guard guard))
                    guards.Add(guard);
            }

            return guards;
        }

        public IEnumerable<Guard> GetGuards(uint instanceId, Vector3 position)
        {
            _buildableGuardsProvider.TryGetValue(instanceId, out HashSet<Guard> guards);

            return guards
                .Where(guard => guard.IsActive);
        }

        public void AddGuard(ushort assetId, uint instanceId, Vector3 position)
        {
            if (!_guardAssets.TryGetValue(assetId, out GuardAsset guardAsset))
                return;

            // TODO init active state
            Guard guard = new Guard(guardAsset, instanceId, true);

            List<uint> protectedBuildables = FindProtectedBuildables(position, guardAsset.Range);

            foreach (var id in protectedBuildables)
            {
                if (_buildableGuardsProvider.TryGetValue(id, out HashSet<Guard> guards))
                    guards.Add(guard);
                else
                    _buildableGuardsProvider.Add(id, new HashSet<Guard> { guard });
            }
        }

        public void AddBuilable(uint instanceId, Vector3 position)
        {
            HashSet<Guard> guards = FindGuards(position);

            _buildableGuardsProvider.Add(instanceId, guards);
        }

        public void RemoveBuilable(uint instanceId)
        {
            Console.WriteLine("Remove Buildable");

            _buildableGuardsProvider.Remove(instanceId);

            RemoveGuard(instanceId);
        }

        private void RemoveGuard(uint instanceId)
        {
            if (!_guardProvider.TryGetValue(instanceId, out Guard guard))
                return;

            foreach (var guards in _buildableGuardsProvider.Values)
                guards.Remove(guard);
        }

        public void UpdateGuard(uint instanceId, bool active)
        {
            if (!_guardProvider.TryGetValue(instanceId, out Guard guard))
                return;

            Console.WriteLine("Upadting guard");

            guard.IsActive = active;
        }
    }
}
