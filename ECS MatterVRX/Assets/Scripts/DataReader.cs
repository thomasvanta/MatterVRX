using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System;

public static class DataReader
{
    public struct IntStreamline
    {
        public Vector3Int start;
        public List<Vector3Int> line;
        public float strength;
        public Color mixedColor;
        public string endName1;
        public string endName2;
    }

    public struct FloatStreamline
    {
        public Vector3 start;
        public List<Vector3> line;
        public float strength;
        public Color mixedColor;
        public string endName1;
        public string endName2;
    }

    // returns the starting coordinate, and the following direction vector
    public static List<Vector3Int> ReadStreamlineInt(out Vector3Int v, string fileName)
    {
        v = Vector3Int.zero;
        List<Vector3Int> lines = new List<Vector3Int>();
        string path = Application.streamingAssetsPath + "/Resources/" + fileName;
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

    // returns the starting coordinate, and the following direction vector
    public static List<Vector3> ReadStreamlineFloat(out Vector3 v, string fileName)
    {
        v = Vector3.zero;
        List<Vector3> lines = new List<Vector3>();
        string path = Application.streamingAssetsPath + "/Resources/" + fileName;
        StreamReader reader = new StreamReader(path);

        string line1 = reader.ReadLine();
        if (line1 == null) return lines;

        string[] stringCoords = line1.Split(' ');
        Vector3 coords1 = new Vector3(float.Parse(stringCoords[0], CultureInfo.InvariantCulture), float.Parse(stringCoords[1], CultureInfo.InvariantCulture), float.Parse(stringCoords[2], CultureInfo.InvariantCulture));
        v = coords1;

        string line2;

        while (line1 != null && (line2 = reader.ReadLine()) != null)
        {
            stringCoords = line2.Split(' ');
            Vector3 coords2 = new Vector3(float.Parse(stringCoords[0], CultureInfo.InvariantCulture), float.Parse(stringCoords[1], CultureInfo.InvariantCulture), float.Parse(stringCoords[2], CultureInfo.InvariantCulture));

            if (coords1 != coords2) lines.Add(coords2 - coords1);

            line1 = line2;
            coords1 = coords2;
        }

        return lines;
    }

    public static int[,] ReadAdjacencyMatrix(out int max, string fileName, int size = 84)
    {
        int[,] matrix = new int[84, 84];
        char fieldSeparator = ',';
        string path = Application.streamingAssetsPath + "/Resources/" + fileName;
        StreamReader reader = new StreamReader(path);

        max = 0;

        for (int i = 0; i < size; i++)
        {
            string[] fields = reader.ReadLine().Split(fieldSeparator);
            for (int j = i; j < size; j++)
            {
                int val = int.Parse(fields[j]);
                matrix[i, j] = val;
                matrix[j, i] = val;
                if (val > max) max = val;
            }
        }

        return matrix;
    }

    public static List<Tuple<int, int>> ReadAssignments(string fileName, bool ignoreFirstLine = true)
    {
        List<Tuple<int, int>> list = new List<Tuple<int, int>>();
        string path = Application.streamingAssetsPath + "/Resources/" + fileName;
        StreamReader reader = new StreamReader(path);

        string line;
        if (ignoreFirstLine) line = reader.ReadLine();

        while ((line = reader.ReadLine()) != null)
        {
            string[] split = line.Split(' ');
            int first = int.Parse(split[0]);
            int second = int.Parse(split[1]);

            list.Add(Tuple.Create(first, second));
        }

        return list;
    }

    public static int[] ReadLineStrength(out int maxStrength, string assignmentsFileName, string adjacencyFileName, int size = 84, bool ignoreAssignmentFirstLine = true)
    {
        int[,] matrix = ReadAdjacencyMatrix(out maxStrength, adjacencyFileName, size);
        List<Tuple<int, int>> tuples = ReadAssignments(assignmentsFileName, ignoreAssignmentFirstLine);

        int[] strengths = new int[tuples.Count];

        int i = 0;
        foreach (Tuple<int, int> tuple in tuples)
        {
            // take into account the fact that the matrix is size * size starting at 0,0 but nodes are numbered from 1 to 84
            if (tuple.Item1 == 0 || tuple.Item2 == 0) strengths[i] = 0;
            else strengths[i] = matrix[tuple.Item1 - 1, tuple.Item2 - 1];
            i++;
        }

        return strengths;
    }

    public static Color[] ReadNodes(out string[] names, string fileName, int size = 84)
    {
        // example line :
        // 48    R.AM    Right-Amygdala                  103 255 255 255
        size++; // take into account the node 0

        Color[] colors = new Color[size];
        names = new string[size];
        string path = Application.streamingAssetsPath + "/Resources/" + fileName;
        StreamReader reader = new StreamReader(path);

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] subs = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            int i = int.Parse(subs[0]);

            int r = int.Parse(subs[3]);
            int g = int.Parse(subs[4]);
            int b = int.Parse(subs[5]);

            colors[i] = new Color(r, g, b);
            names[i] = subs[2];
        }

        return colors;
    }

    public static List<IntStreamline> ReadIntLines(string adjacencyFileName = "connectome_1M.csv",
                                                    string assignmentFileName = "assignments_radial_1M.csv",
                                                    string nodesFileName = "fs_default_mod.txt",
                                                    string streamlineFileNameTemplate = "data/output-", int digitsNumber = 7,
                                                    int size = 84, bool ignoreAssignmentFirstLine = true)
    {
        int max;
        int[] lineStrength = ReadLineStrength(out max, assignmentFileName, adjacencyFileName, size, ignoreAssignmentFirstLine);
        float maxStrength = (float)max;

        string[] names;
        Color[] colors = ReadNodes(out names, nodesFileName, size);

        List<Tuple<int, int>> lineEnds = ReadAssignments(assignmentFileName, ignoreAssignmentFirstLine);

        List<IntStreamline> lines = new List<IntStreamline>();

        int i = 0;
        int j = 0;
        string fillerZeros = new String('0', digitsNumber - 1);
        string linePath = streamlineFileNameTemplate + fillerZeros + i.ToString() + ".txt";

        while (File.Exists(Application.streamingAssetsPath + "/Resources/" + linePath))
        {
            Tuple<int, int> lineEnd = lineEnds[i];
            IntStreamline line = new IntStreamline();

            line.line = ReadStreamlineInt(out line.start, linePath);
            line.strength = lineStrength[i] / maxStrength;
            line.mixedColor = Color.Lerp(colors[lineEnd.Item1], colors[lineEnd.Item2], 0.5f);
            line.endName1 = names[lineEnd.Item1];
            line.endName2 = names[lineEnd.Item2];

            lines.Add(line);

            i++;
            if (j < Mathf.FloorToInt(Mathf.Log10(i)))
            {
                j++;
                fillerZeros = new String('0', digitsNumber - j - 1);
            }
            linePath = streamlineFileNameTemplate + fillerZeros + i.ToString() + ".txt";
        }

        return lines;
    }

    public static List<FloatStreamline> ReadFloatLines(string adjacencyFileName = "connectome_1M.csv",
                                                        string assignmentFileName = "assignments_radial_1M.csv",
                                                        string nodesFileName = "fs_default_mod.txt",
                                                        string streamlineFileNameTemplate = "data/output-", int digitsNumber = 7,
                                                        int size = 84, bool ignoreAssignmentFirstLine = true)
    {
        int max;
        int[] lineStrength = ReadLineStrength(out max, assignmentFileName, adjacencyFileName, size, ignoreAssignmentFirstLine);
        float maxStrength = (float)max;

        string[] names;
        Color[] colors = ReadNodes(out names, nodesFileName, size);

        List<Tuple<int, int>> lineEnds = ReadAssignments(assignmentFileName, ignoreAssignmentFirstLine);

        List<FloatStreamline> lines = new List<FloatStreamline>();

        int i = 0;
        int j = 0;
        string fillerZeros = new String('0', digitsNumber - 1);
        string linePath = streamlineFileNameTemplate + fillerZeros + i.ToString() + ".txt";

        while (File.Exists(Application.streamingAssetsPath + "/Resources/" + linePath))
        {
            Tuple<int, int> lineEnd = lineEnds[i];
            FloatStreamline line = new FloatStreamline();

            line.line = ReadStreamlineFloat(out line.start, linePath);
            line.strength = lineStrength[i] / maxStrength;
            line.mixedColor = Color.Lerp(colors[lineEnd.Item1], colors[lineEnd.Item2], 0.5f);
            line.endName1 = names[lineEnd.Item1];
            line.endName2 = names[lineEnd.Item2];

            lines.Add(line);

            i++;
            if (j < Mathf.FloorToInt(Mathf.Log10(i)))
            {
                j++;
                fillerZeros = new String('0', digitsNumber - j - 1);
            }
            linePath = streamlineFileNameTemplate + fillerZeros + i.ToString() + ".txt";
        }

        return lines;
    }

    public static void PrintParsed()
    {
        var lines = ReadFloatLines();
        int i = 0;
        foreach (var line in lines)
        {
            Debug.Log(i.ToString() + " : starts at " + line.start.ToString() + " and has " + line.line.Count.ToString() + " segments. It is of strength "
                + line.strength + " and goes from " + line.endName1 + " to " + line.endName2 + ", with a color of " + line.mixedColor.ToString());
            i++;
        }
    }

    // ============================== END OF STREAMLINE PARSING, BEGINNING OF VOXEL PARSING =======================================

    public enum ColorMap { Grey, Hot, Cool, Jet }

    public static string parsedNiftiName = "";

    // please refer to https://github.com/MRtrix3/mrtrix3/blob/master/src/colourmap.cpp for more color maps
    // amplitude must be between 0 and 1
    public static Vector4 ConvertAmplitudeToColor(float amplitude, ColorMap colorMap = ColorMap.Grey)
    {
        switch (colorMap)
        {
            case ColorMap.Grey:
                return new Vector4(Mathf.Clamp01(amplitude), Mathf.Clamp01(amplitude), Mathf.Clamp01(amplitude), 1f);

            case ColorMap.Hot:
                return new Vector4(Mathf.Clamp01(2.7213f * amplitude), Mathf.Clamp01(2.7213f * amplitude - 1), Mathf.Clamp01(3.7727f * amplitude - 2.7727f), 1f);

            case ColorMap.Cool:
                return new Vector4(Mathf.Clamp01(1.0f - (2.7213f * (1.0f - amplitude))),
                                   Mathf.Clamp01(1.0f - (2.7213f * (1.0f - amplitude) - 1.0f)),
                                   Mathf.Clamp01(1.0f - (3.7727f * (1.0f - amplitude) - 2.7727f)), 1f);

            case ColorMap.Jet:
                return new Vector4(Mathf.Clamp01(1.5f - 4.0f * Mathf.Abs(1.0f - amplitude - 0.25f)),
                                   Mathf.Clamp01(1.5f - 4.0f * Mathf.Abs(1.0f - amplitude - 0.5f)),
                                   Mathf.Clamp01(1.5f - 4.0f * Mathf.Abs(1.0f - amplitude - 0.75f)), 1f);

            default:
                return new Vector4(0, 0, 0, 1);
        }
    }

    public static Nifti.NET.Nifti<float> ParseNifti(out float maxAmp, string fileName = "T1w_acpc_dc_restore_brain.nii.gz")
    {
        parsedNiftiName = fileName.Split('.')[0];
        string path = Application.streamingAssetsPath + "/Resources/" + fileName;
        var nii = Nifti.NET.NiftiFile.Read(path);

        if (!nii.IsType(typeof(float)))
        {
            Debug.Log(".nii is not of type float, stopped parsing");
            maxAmp = -1;
            return default;
        }

        Nifti.NET.Nifti<float> nifti = nii.AsType<float>();
        maxAmp = Mathf.Max(nifti.Data);

        return nifti;
    }
}