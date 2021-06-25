using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct ObjData
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;
    public Color color;

    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(pos, rot, scale);

        }
    }
    public ObjData(Vector3 pos, Vector3 scale, Quaternion rot, Color color)
    {
        this.pos = pos;
        this.scale = scale;
        this.rot = rot;
        this.color = color;
    }
}


public class RandomGenShader : MonoBehaviour
{
    public int size = 3;
    public Mesh objMesh;
    public Material objMat;

    private int sizeCubed = 9;

    private int cachedInstanceCount = -1;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    private MaterialPropertyBlock block;

    // Start is called before the first frame update
    void Start()
    {
        sizeCubed = size * size * size;
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        block = new MaterialPropertyBlock();
        UpdateBuffers();
    }

    
    // Update is called once per frame
    void Update() 
    {
        // Render
        Graphics.DrawMeshInstancedIndirect(objMesh, 0, objMat, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), 
            argsBuffer, 0, block, UnityEngine.Rendering.ShadowCastingMode.Off);
    }

    void OnGUI()
    {

        GUI.Label(new Rect(265, 25, 200, 30), "Instance Count: " + sizeCubed.ToString());
        sizeCubed = (int)GUI.HorizontalSlider(new Rect(25, 20, 200, 30), (float)sizeCubed, 1.0f, 5000000.0f);
    }

    void UpdateBuffers()
    {

        // positions
        if (positionBuffer != null)
            positionBuffer.Release();
        positionBuffer = new ComputeBuffer(sizeCubed, 16);
        Vector4[] positions = new Vector4[sizeCubed];
        Vector4[] colors = new Vector4[sizeCubed];

        int i = 0;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    float size = Random.Range(0.05f, 0.25f);
                    positions[i] = new Vector4(x, y, z, size);
                    colors[i] = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    i++;
                }
            }
        }

        positionBuffer.SetData(positions);
        objMat.SetBuffer("positionBuffer", positionBuffer);
        
        block.SetVectorArray("_Colors", colors);

        // indirect args
        uint numIndices = (objMesh != null) ? (uint)objMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)sizeCubed;
        argsBuffer.SetData(args);

        cachedInstanceCount = sizeCubed;
    }

    void OnDisable()
    {

        if (positionBuffer != null)
            positionBuffer.Release();
        positionBuffer = null;

        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;
    }

}
