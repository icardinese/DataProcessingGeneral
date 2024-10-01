using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.IO;

public class StarSpawnerSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Ensure the system runs only once at the start
        RequireSingletonForUpdate<StarSpawner>();
    }

    protected override void OnUpdate()
    {
        var spawner = GetSingleton<StarSpawner>();
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Read CSV file
        string[] lines = File.ReadAllLines(spawner.filePath);
        NativeArray<float3> positions = new NativeArray<float3>(lines.Length, Allocator.TempJob);

        for (int i = 0; i < lines.Length; i++)
        {
            // This is if the dataset gives x,y, and z values. If not there is a specific formula for this
            // string[] values = lines[i].Split(',');
            // if (values.Length >= 3)
            // {
            //     float x = float.Parse(values[0]);
            //     float y = float.Parse(values[1]);
            //     float z = float.Parse(values[2]);
            //     positions[i] = new float3(x, y, z);
            // }

            // If it is a different dataset like the ones in gaia use this:
            
        }

        // Create a job to instantiate entities
        var job = new StarSpawnJob
        {
            EntityPrefab = spawner.EntityPrefab,
            Positions = positions,
            EntityManager = entityManager
        };

        Dependency = job.Schedule(lines.Length, 64, Dependency);
        Dependency.Complete();

        // Clean up
        positions.Dispose();
        EntityManager.DestroyEntity(GetSingletonEntity<StarSpawner>());
    }

    [BurstCompile]
    struct StarSpawnJob : IJobParallelFor
    {
        public Entity EntityPrefab;
        [DeallocateOnJobCompletion] public NativeArray<float3> Positions;
        public EntityManager EntityManager;

        public void Execute(int index)
        {
            var instance = EntityManager.Instantiate(EntityPrefab);
            EntityManager.SetComponentData(instance, new Translation { Value = Positions[index] });
        }
    }
}

public struct StarSpawner : IComponentData
{
    public Entity EntityPrefab;
    public FixedString128Bytes filePath;
}