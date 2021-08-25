using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class ConfigurationLoader : MonoBehaviour
{
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
    }

}
