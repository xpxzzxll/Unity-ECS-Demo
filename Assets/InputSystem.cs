using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InputSystem : ComponentSystem
{
    private struct Group
    {
        public readonly int Length;
        public ComponentDataArray<VelocityComponent> Velocities;
        public ComponentDataArray<PlayerComponent> Players;
    }

    [Inject] private Group data;
    
    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; i++)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            float3 nor = new float3();
            if ( math.abs(x) > Mathf.Epsilon || math.abs(y) > Mathf.Epsilon)
            {
                nor = math.normalize(new float3(x, 0, y));
            }
            data.Velocities[i] = new VelocityComponent()
            {
                moveDir = nor
            };
        }
    }
}