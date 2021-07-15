using Unity.Entities;
using Unity.Jobs;

public class OutlineSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle jobHandle = Entities.ForEach(
            (ref OutlineColorComponent outlineColor, 
            in OutlineComponent outlineComponent,
            in MainColorComponent mainColor) => 
            {
                if (outlineComponent.isSelected)
                {
                    outlineColor.value = outlineComponent.color;
                }
                else
                {
                    outlineColor.value = mainColor.value;
                }
            }
            ).Schedule(inputDeps);

        return jobHandle;
    }
}
