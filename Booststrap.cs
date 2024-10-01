using Unity.Entities;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public GameObject starPrefab;
    public string filePath = "The_FilePath.csv that you desire!";

    void Start()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var starEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(starPrefab, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null));

        var spawnerEntity = entityManager.CreateEntity();
        entityManager.AddComponentData(spawnerEntity, new StarSpawner
        {
            EntityPrefab = starEntityPrefab,
            filePath = filePath
        });
    }
}