using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinedRandomGen : MonoBehaviour
{
    public GameObject VFXVoxels;
    public GameObject PrefabVoxels;

    public float interestingChance = 0.2f;

    public int size = 3;
    public Transform prefab;

    // parameter to export in config file
    public float minScale = 0.05f;
    public float maxScale = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        int vfxResolution = VFXVoxels.GetComponent<VoxelRenderer>().SetResolutionFromVolume(size * size * size);

        Vector3[] particlesPos = new Vector3[vfxResolution * vfxResolution];
        Color[] particlesColors = new Color[vfxResolution * vfxResolution];
        int i = 0;

        MaterialPropertyBlock properties = new MaterialPropertyBlock();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    if (Random.value < interestingChance)
                    {
                        Transform t = Instantiate(prefab);
                        t.SetParent(transform);
                        t.position = new Vector3(x, y, z);
                        t.GetComponent<VisualManager>().SetScale(Random.Range(minScale, maxScale));
                        properties.SetColor("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
                        t.GetComponent<MeshRenderer>().SetPropertyBlock(properties);
                    }
                    else
                    {
                        particlesPos[i] = new Vector3(x, y, z);
                        particlesColors[i] = Color.white;
                        i++;
                    }
                }
            }
        }

        VFXVoxels.GetComponent<VoxelRenderer>().SetParticles(particlesPos, particlesColors);
    }
}
