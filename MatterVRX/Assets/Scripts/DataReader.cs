using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public static class DataReader
{
    public static Vector3Int[,,] ReadStreamlines(int size, string fileName)
    {
        Vector3Int[,,] lines = new Vector3Int[size, size, size];
        string path = "Assets/Resources/" + fileName;
        StreamReader reader = new StreamReader(path);

        string line1 = reader.ReadLine();
        if (line1 == null) return lines;

        int offset = size / 2;

        string[] stringCoords = line1.Split(' ');
        Vector3Int coords1 = Vector3Int.RoundToInt(new Vector3(float.Parse(stringCoords[0], CultureInfo.InvariantCulture), float.Parse(stringCoords[1], CultureInfo.InvariantCulture), float.Parse(stringCoords[2], CultureInfo.InvariantCulture)));

        string line2;

        while (line1 != null && (line2 = reader.ReadLine()) != null)
        {
            stringCoords = line2.Split(' ');
            Vector3Int coords2 = Vector3Int.RoundToInt(new Vector3(float.Parse(stringCoords[0], CultureInfo.InvariantCulture), float.Parse(stringCoords[1], CultureInfo.InvariantCulture), float.Parse(stringCoords[2], CultureInfo.InvariantCulture)));

            if (coords1 != coords2) lines[coords1.x + offset, coords1.y + offset, coords1.z + offset] = coords2 - coords1;

            line1 = line2;
            coords1 = coords2;
        }

        return lines;
    }
}
