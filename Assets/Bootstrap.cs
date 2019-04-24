using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

//JobComponentSystem，NativeArray，System并行，组件的先后顺序，读写权限这些跟性能优化相关的点就没有介绍了

public class Bootstrap
{

    private static EntityManager entityManager;
    private static EntityArchetype playerArchetype;
    
    private static bool variant = false;
    
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Awake()
    {
        //获取EntityManager
        entityManager = World.Active.GetOrCreateManager<EntityManager>();

        //创建Archetype的两种方式
        if (true == variant)
        {
            playerArchetype = entityManager.CreateArchetype(
                typeof(Position),
                typeof(VelocityComponent),
                typeof(PlayerComponent)
            );
        }
        else
        {
            playerArchetype = entityManager.CreateArchetype(new ComponentType[]{});
        }
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Start()
    {
        GameObject playerGO = GameObject.Find("Player");
        var info = playerGO.GetComponent<RenderInfo>();
        RenderMesh playerRenderer = new RenderMesh
        {
            mesh = info.referenceMesh,
            material = info.referenceMaterial,
        };
        Object.Destroy(playerGO);

        for (int i = 0; i < 1; i++)
        {
            Entity player = entityManager.CreateEntity(playerArchetype);

            if (variant)
            {
                /*
                 * ComponentType 只能被 Add 一次
                 * 对于已被 Add 的 ComponentType 只能通过 SetComponentData 来设置值
                 */
                entityManager.SetComponentData(player, new Position
                {
                    Value = new Unity.Mathematics.float3(UnityEngine.Random.value, 1.0f, UnityEngine.Random.value)
                });
        
                entityManager.SetComponentData(player, new VelocityComponent()
                {
                    moveDir = float3.zero
                });
                
                entityManager.SetComponentData(player, new PlayerComponent()
                {
                    name = i
                });
        
                /*
                 * 动态添加 ComponentType
                 * 1. 可以通过 AddComponentData/AddSharedComponentData 动态添加 ComponentType 并设置初始值
                 * 2. 也可以先 AddComponent 动态添加 ComponentType 再通过 SetComponentData/SetSharedComponentData 来设置值
                 */
                entityManager.AddSharedComponentData(player, playerRenderer);
                //or
                // entityManager.AddComponent(player, typeof(RenderMesh));
                // entityManager.SetSharedComponentData(player, playerRenderer);
            }
            else
            {
                entityManager.AddComponentData(player, new Position
                {
                    Value = new Unity.Mathematics.float3(UnityEngine.Random.value, 1.0f, UnityEngine.Random.value)
                });
        
                entityManager.AddComponentData(player, new VelocityComponent()
                {
                    moveDir = float3.zero
                });
        
                entityManager.AddComponentData(player, new PlayerComponent()
                {
                    name = i
                });
                
                // entityManager.AddSharedComponentData(player, playerRenderer);
                entityManager.AddComponent(player, typeof(RenderMesh));
                entityManager.SetSharedComponentData(player, playerRenderer);
            }
            
            

            
        }
    }
}
