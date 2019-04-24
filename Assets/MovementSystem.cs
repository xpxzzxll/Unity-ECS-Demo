using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// /* 第一种写法
public class MovementSystem : ComponentSystem
{

    private struct Group
    {
        public readonly int Length;
        public ComponentDataArray<Position> Positions;
        public ComponentDataArray<VelocityComponent> Velocities;
    }

    [Inject] private Group data;
    
    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < data.Length; i++)
        {
            float3 up = new float3(0 ,1,0);
            float3 pos = data.Positions[i].Value;
            float3 dir = data.Velocities[i].moveDir;
            pos += dir * dt;
            data.Positions[i] = new Position{Value = pos};
        }
    }
}
// */

 /* 第二种写法
public class MovementSystem : ComponentSystem
{    
    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;
        ForEach((ref Position p, ref VelocityComponent v) => { p.Value = p.Value + dt * v.moveDir; });
    }
}
// */

 /*
public class MovementSystem : JobComponentSystem
{

    //在未来的版本中被IJobProcessComponentData->IJobForEach
    [BurstCompile]
    struct Group : IJobProcessComponentData<Position, VelocityComponent>
    {
        public float dt;
        public void Execute(ref Position c0, ref VelocityComponent c1)
        {
            c0.Value = c0.Value + c1.moveDir * dt;
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new Group()
        {
            dt = Time.deltaTime
        };
        return job.Schedule(this, inputDeps);
    }
}
// */