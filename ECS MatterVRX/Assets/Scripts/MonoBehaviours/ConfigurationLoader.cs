using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

public class ConfigurationLoader : MonoBehaviour
{
    private static char[] HexVals = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

    [XmlRoot("LoadRegion")]
    public class LoadRegion
    {
        public int x;
        public int y;
        public int z;
        public int sizeX;
        public int sizeY;
        public int sizeZ;
    }

    [XmlRoot("StreamlineFiles")]
    public class StreamlineFiles
    {
        public string adjacencyMatrix;
        public int size;
        public string assignments;
        public bool ignoreAssignmentFirstLine;
        public string nodes;
        public string streamlineTemplate;
        public int digitsNumber;
    }

    [XmlRoot("FogConfig")]
    public class FogConfig
    {
        public string color;
        public float start;
        public float end;
    }

    [XmlRoot("BaseConfig")]
    public class BaseConfig
    {
        public float zoomFactor;
        public float zoomCenterOffset;
        public float renderDist;
        public float selectionDist;
        public float verticalSpeed;
        public float horizontalSpeed;
        public string fileName;
        public string loadMode;
        public LoadRegion LoadRegion;
        public bool loadStreamlines;
        public StreamlineFiles StreamlineFiles;
        public int dummyTumorPosX;
        public int dummyTumorPosY;
        public int dummyTumorPosZ;
        public float dummyTumorRadius;
        public float dummyTumorPeriphery;
        public float dummyTumorHealthy;
        public string fogColor;
        public float fogStart;
        public float fogEnd;
    }
    

    // Start is called before the first frame update
    void Awake()
    {
        var serializer = new XmlSerializer(typeof(BaseConfig));
        var stream = new FileStream(Path.Combine(Application.streamingAssetsPath, "Configuration/BaseConfig.xml"), FileMode.Open);
        var container = serializer.Deserialize(stream) as BaseConfig;
        stream.Close();

        InputManager.zoomSpeed = container.zoomFactor;
        InputManager.zoomCenterOffset = container.zoomCenterOffset;
        InputManager.renderDist = container.renderDist;
        InputManager.colliderDist = container.selectionDist;
        InputManager.verticalSpeed = container.verticalSpeed;
        InputManager.horizontalSpeed = container.horizontalSpeed;
        EcsSpawner.filename = container.fileName;
        EcsSpawner.loadMode = container.loadMode;
        EcsSpawner.region = container.LoadRegion;
        EcsSpawner.loadStreamlines = container.loadStreamlines;
        EcsSpawner.streamlineFiles = container.StreamlineFiles;
        EcsSpawner.dummyTumorPos = new Unity.Mathematics.int3(container.dummyTumorPosX, container.dummyTumorPosY, container.dummyTumorPosZ);
        EcsSpawner.dummyTumorRadius = container.dummyTumorRadius;
        EcsSpawner.dummyTumorPeripherySize = container.dummyTumorPeriphery;
        EcsSpawner.dummyTumorHealthySize = container.dummyTumorHealthy;

        Color fogColor = ParseHexColor(container.fogColor);

        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogStartDistance = container.fogStart;
        RenderSettings.fogEndDistance = container.fogEnd;

        /*
        bool bConverted = ColorUtility.TryParseHtmlString(container.fogColor, out fogColor);

        if(bConverted)
        {
            print("fog activated");
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogStartDistance = container.fogStart;
            RenderSettings.fogEndDistance = container.fogEnd;
        }
        else
        {
            print("fog deactivated");
            RenderSettings.fog = false;
        }
        */
    }

    public static Color ParseHexColor(string colorString)
    {
        string s = colorString.Trim().Trim('#');
        float r = (16f * Array.IndexOf(HexVals, s[0]) + Array.IndexOf(HexVals, s[1])) / 255f;
        float g = (16f * Array.IndexOf(HexVals, s[2]) + Array.IndexOf(HexVals, s[3])) / 255f;
        float b = (16f * Array.IndexOf(HexVals, s[4]) + Array.IndexOf(HexVals, s[5])) / 255f;
        return new Color(r, g, b, 1f);
    }
}
