using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using System.IO;

public class SaveSystem : ComponentSystem
{
    public static bool needsSave = false;

    protected override void OnUpdate()
    {
        if (!needsSave) return;
        needsSave = false;

        string path = "Assets/Resources/Saves/" + DataReader.parsedNiftiName + ".txt";
        StreamWriter writer = new StreamWriter(path, false);

        Entities.ForEach((ref VoxelComponent voxel) => {

            string line = voxel.matrixPosition[0] + "," + voxel.matrixPosition[1] + "," + voxel.matrixPosition[2] + ":";

            int n = 0;
            for (int j = 0; j < 4; j++)
            {
                if (voxel.annotationsIds[j] >= 0) n++;
                line += voxel.annotationsIds[j] + ",";
            }
            if (n > 0) writer.WriteLine(line); // don't write if the voxel is not annotated

        });

        writer.Close();
    }
}