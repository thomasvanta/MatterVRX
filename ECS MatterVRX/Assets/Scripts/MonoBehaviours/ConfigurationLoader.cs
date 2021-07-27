using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class ConfigurationLoader : MonoBehaviour
{
    [XmlRoot("BaseConfig")]
    public class BaseConfig
    {
        public float zoomFactor;

        public float zoomCenterOffset;

        public float renderDist;

        public float selectionDist;

        public float verticalSpeed;

        public float horizontalSpeed;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        var serializer = new XmlSerializer(typeof(BaseConfig));
        var stream = new FileStream(Path.Combine(Application.dataPath,"Configuration/BaseConfig.xml"), FileMode.Open);
        var container = serializer.Deserialize(stream) as BaseConfig;
        stream.Close();

        InputManager.zoomSpeed = container.zoomFactor;
        InputManager.zoomCenterOffset = container.zoomCenterOffset;
        InputManager.renderDist = container.renderDist;
        InputManager.colliderDist = container.selectionDist;
        InputManager.verticalSpeed = container.verticalSpeed;
        InputManager.horizontalSpeed = container.horizontalSpeed;
    }

}
