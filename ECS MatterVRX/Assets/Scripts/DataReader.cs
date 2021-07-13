using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public static class DataReader
{
    // returns the starting coordinate, and the following direction vector
    public static List<Vector3Int> ReadStreamline(out Vector3Int v, string fileName)
    {
        v = Vector3Int.zero;
        List<Vector3Int> lines = new List<Vector3Int>();
        string path = "Assets/Resources/" + fileName;
        StreamReader reader = new StreamReader(path);

        string line1 = reader.ReadLine();
        if (line1 == null) return lines;

        string[] stringCoords = line1.Split(' ');
        Vector3Int coords1 = Vector3Int.RoundToInt(new Vector3(float.Parse(stringCoords[0], CultureInfo.InvariantCulture), float.Parse(stringCoords[1], CultureInfo.InvariantCulture), float.Parse(stringCoords[2], CultureInfo.InvariantCulture)));
        v = coords1;

        string line2;

        while (line1 != null && (line2 = reader.ReadLine()) != null)
        {
            stringCoords = line2.Split(' ');
            Vector3Int coords2 = Vector3Int.RoundToInt(new Vector3(float.Parse(stringCoords[0], CultureInfo.InvariantCulture), float.Parse(stringCoords[1], CultureInfo.InvariantCulture), float.Parse(stringCoords[2], CultureInfo.InvariantCulture)));

            if (coords1 != coords2) lines.Add(coords2 - coords1);

            line1 = line2;
            coords1 = coords2;
        }

        return lines;
    }
}