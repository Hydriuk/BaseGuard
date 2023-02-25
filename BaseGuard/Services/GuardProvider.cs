using BaseGuard.API;
using BaseGuard.Extensions;
using BaseGuard.Models;
using Hydriuk.Unturned.SharedModules.Adapters;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace BaseGuard.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class GuardProvider : IGuardProvider
    {
        private readonly IThreadAdatper _threadAdatper;

        private readonly ConcurrentDictionary<uint, HashSet<Guard>> _buildableGuardsProvider = new ConcurrentDictionary<uint, HashSet<Guard>>();
        private readonly ConcurrentDictionary<uint, Guard> _guardProvider = new ConcurrentDictionary<uint, Guard>();

        // Configuration
        private readonly Dictionary<ushort, GuardAsset> _guardAssets;

        private readonly float _maxRange;
        private readonly float _sqrMaxRange;

        public GuardProvider(IConfigurationAdapter<Configuration> confAdapter, IThreadAdatper threadAdatper)
        {
            _threadAdatper = threadAdatper;

            _guardAssets = confAdapter.Configuration.Guards.ToDictionary(guard => guard.Id);

            _maxRange = confAdapter.Configuration.Guards.Count > 0 ? confAdapter.Configuration.Guards.Max(guard => guard.Range) : 0;
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
                    AddGuard(drop.asset.id, drop.instanceID, drop.model.position, drop.interactable.IsActive());
        }

        private List<uint> FindProtectedBuildables(Vector3 position, float range)
        {
            float sqrRange = range * range;

            // TODO Change this for a better way to insure the builadables are correctly found
            // Loop until getInRadius and FindByRoot methods succeed
            while (true)
            {
                try
                {
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
                catch (InvalidOperationException)
                {
                    Thread.Sleep(100);
                }
            }
        }

        private HashSet<Guard> FindGuards(Vector3 position)
        {
            // TODO Change this for a better way to insure the guards are correctly found
            // Loop until getInRadius and FindByRoot methods succeed
            while (true)
            {
                try
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

                        if (_guardProvider.TryGetValue(protectedDrop.instanceID, out Guard guard))
                        {
                            if (Vector3.Distance(position, barricade.transform.position) < guard.Range)
                                guards.Add(guard);
                        }
                    }

                    foreach (var structure in structures)
                    {
                        StructureDrop protectedDrop = StructureManager.FindStructureByRootTransform(structure);

                        if (_guardProvider.TryGetValue(protectedDrop.instanceID, out Guard guard))
                        {
                            if (Vector3.Distance(position, structure.transform.position) < guard.Range)
                                guards.Add(guard);
                        }
                    }
                    return guards;
                }
                catch (InvalidOperationException)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public IEnumerable<Guard> GetGuards(uint instanceId)
        {
            if (!_buildableGuardsProvider.TryGetValue(instanceId, out HashSet<Guard> guards))
                return new List<Guard>();

            return guards
               .Where(guard => guard.IsActive)
               .GroupBy(guard => guard.AssetId)
               .Select(group => group.First());
        }

        public void AddGuard(ushort assetId, uint instanceId, Vector3 position, bool isActive)
        {
            _threadAdatper.RunOnThreadPool(() =>
            {
                if (!_guardAssets.TryGetValue(assetId, out GuardAsset guardAsset))
                    return;

                Guard guard = new Guard(guardAsset, instanceId, isActive);

                List<uint> protectedBuildables = FindProtectedBuildables(position, guardAsset.Range);

                foreach (var id in protectedBuildables)
                {
                    if (_buildableGuardsProvider.TryGetValue(id, out HashSet<Guard> guards))
                        guards.Add(guard);
                    else
                        _buildableGuardsProvider.TryAdd(id, new HashSet<Guard> { guard });
                }

                _guardProvider.TryAdd(instanceId, guard);
            });
        }

        public void AddBuilable(uint instanceId, Vector3 position)
        {
            _threadAdatper.RunOnThreadPool(() =>
            {
                HashSet<Guard> guards = FindGuards(position);

                _buildableGuardsProvider.TryAdd(instanceId, guards);
            });
        }

        public void RemoveBuilable(uint instanceId)
        {
            _threadAdatper.RunOnThreadPool(() =>
            {
                _buildableGuardsProvider.TryRemove(instanceId, out var _);
            });

            RemoveGuard(instanceId);
        }

        private void RemoveGuard(uint instanceId)
        {
            _threadAdatper.RunOnThreadPool(() =>
            {
                if (!_guardProvider.TryGetValue(instanceId, out Guard guard))
                    return;

                foreach (var guards in _buildableGuardsProvider.Values)
                    guards.Remove(guard);
            });
        }

        public void UpdateGuard(uint instanceId, bool active)
        {
            _threadAdatper.RunOnThreadPool(() =>
            {
                if (!_guardProvider.TryGetValue(instanceId, out Guard guard))
                    return;

                guard.IsActive = active;
            });
        }
    }
}