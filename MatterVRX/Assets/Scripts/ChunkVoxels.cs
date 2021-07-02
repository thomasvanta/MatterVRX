using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class ChunkVoxels : MonoBehaviour
{
    public enum Mode
    {
        Destroy,
        SetInactive,
        AreaLoad,
        FullLoad
    }

    public enum ChunkState
    {
        Null,
        Empty,
        Inactive,
        Active
    }

    public int globalSize = 100;
    public int chunkSize = 5;
    public int nbChunkDisplay = 5;
    public int bufferAreaSize = 2;
    public float minScale = 0.05f;
    public float maxScale = 0.5f;
    public Transform prefab;
    public Vector3 centerPos = new Vector3(0, 0, 0);
    public Vector3Int initCenterChunk = new Vector3Int(3, 3, 3);
    public Mode mode = Mode.Destroy;

    struct Voxel
    {
        public float size;
        public Color color;
    }

    struct Chunk
    {
        public Vector3Int pos;
        public ChunkState state;
        public Transform[,,] voxels;
    }


    private Voxel[,,] dataSet;
    private Chunk[,,] chunkMatrix;
    private Vector3Int centerChunk;
    private Vector3Int oldCenter;
    private int maxChunkPos;
    private int innerOffset;
    private int outerOffset;
    
    /* thread unusable for instancing prefab
    private Thread destroyThread = null;
    private Thread hidenLoadThread = null;
    */

    // Start is called before the first frame update
    void Start()
    {
        dataSet = populateDataSet();
        maxChunkPos = (int)System.Math.Ceiling((double)globalSize / chunkSize);
        centerChunk = initCenterChunk;
        oldCenter = centerChunk;
        setTransformPos();

        if (nbChunkDisplay % 2 == 0) nbChunkDisplay++; // make sure we have a center chunk
        innerOffset = (int)System.Math.Floor((double)nbChunkDisplay / 2);
        outerOffset = innerOffset + bufferAreaSize;

        chunkMatrix = new Chunk[maxChunkPos, maxChunkPos, maxChunkPos];

        if (mode == Mode.FullLoad) fullLoad();
        manageDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int curCenter = getCenterChunkPos();
        if(curCenter != centerChunk)
        {
            oldCenter = centerChunk;
            centerChunk = curCenter;
            manageDisplay();
        }
    }


    // helper
    bool isInChunkPosRange(Vector3Int pos)
    {
        if (pos.x < 0) return false;
        if (pos.x >= maxChunkPos) return false;
        if (pos.y < 0) return false;
        if (pos.y >= maxChunkPos) return false;
        if (pos.z < 0) return false;
        if (pos.z >= maxChunkPos) return false;
        return true;
    }

    void setTransformPos()
    {
        Vector3 newPos = Vector3.Scale(centerChunk + new Vector3(0.5f, 0.5f, 0.5f), transform.localScale);
        newPos *= -chunkSize;
        newPos += centerPos;
        transform.localPosition = newPos;
    }

    Vector3Int getCenterChunkPos()
    {
        Vector3 delta = centerPos - transform.localPosition;
        Vector3 chunkScale = chunkSize * transform.localScale;

        Vector3Int curCenter = new Vector3Int((int)System.Math.Floor(delta.x / chunkScale.x),
            (int)System.Math.Floor(delta.y / chunkScale.y),
            (int)System.Math.Floor(delta.z / chunkScale.z));

        return curCenter;
    }

    Voxel[,,] populateDataSet()
    {
        Voxel[,,] data = new Voxel[globalSize, globalSize, globalSize];
        for (int x = 0; x < globalSize; x++)
        {
            for (int y = 0; y < globalSize; y++)
            {
                for (int z = 0; z < globalSize; z++)
                {
                    Voxel voxel;
                    voxel.size = Random.Range(minScale, maxScale);
                    voxel.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    data[x, y, z] = voxel;
                }
            }
        }
        return data;
    }


    // manage chunks
    Chunk generateEmptyChunk(Vector3Int pos)
    {
        Chunk chunk;
        chunk.voxels = new Transform[chunkSize, chunkSize, chunkSize];
        chunk.pos = pos;
        chunk.state = ChunkState.Empty;
        return chunk;
    }

    void fillEmpty(Chunk chunk, bool hiden)
    {
        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        Vector3Int pos = Vector3Int.zero;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                { 
                    pos.Set(chunk.pos.x * chunkSize + x,
                        chunk.pos.y * chunkSize + y, 
                        chunk.pos.z * chunkSize + z);

                    if (dataSet[pos.x, pos.y, pos.z].size > 0)
                    {
                        Transform t = Instantiate(prefab);
                        t.SetParent(transform);
                        t.localPosition = pos;
                        t.GetComponent<VisualManager>().SetScale(dataSet[pos.x, pos.y, pos.z].size);
                        properties.SetColor("_Color", dataSet[pos.x, pos.y, pos.z].color);
                        t.GetComponent<MeshRenderer>().SetPropertyBlock(properties);
                        chunk.voxels[x, y, z] = t;
                        if(hiden)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        if(hiden) chunk.state = ChunkState.Inactive;
        else chunk.state = ChunkState.Active;
    }

    void setActive(Chunk chunk, bool active)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if(chunk.voxels[x,y,z] != null)
                    {
                        chunk.voxels[x, y, z].gameObject.SetActive(active);
                    }
                }
            }
        }
        if (active) chunk.state = ChunkState.Active;
        else chunk.state = ChunkState.Inactive;
    }

    void emptyChunk(Chunk chunk)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (chunk.state != ChunkState.Null && chunk.state != ChunkState.Empty && chunk.voxels[x, y, z] != null)
                    {
                        Destroy(chunk.voxels[x, y, z].gameObject);
                        chunk.voxels[x, y, z] = null;
                    }
                }
            }
        }
        chunk.state = ChunkState.Empty;
    }

    void loadChunk(Chunk chunk, bool hiden = true)
    {
        switch(chunk.state)
        {
            case ChunkState.Empty:
                fillEmpty(chunk, hiden);
                break;

            case ChunkState.Inactive:
                if(!hiden) setActive(chunk, true);
                break;

            case ChunkState.Active:
                if (hiden) setActive(chunk, false);
                break;

            default:
                break;
        }
    }

    void fullLoad()
    {
        for (int x = 0; x < maxChunkPos; x++)
        {
            for (int y = 0; y < maxChunkPos; y++)
            {
                for (int z = 0; z < maxChunkPos; z++)
                {
                    if (chunkMatrix[x, y, z].state == ChunkState.Null)
                    {
                        chunkMatrix[x, y, z] = generateEmptyChunk(new Vector3Int(x, y, z));
                    }
                    loadChunk(chunkMatrix[x, y, z]);
                }
            }
        }
    }

    // manage chunk matrix
    void manageDisplay()
    {
        switch(mode)
        {
            case Mode.Destroy:
                setDisplayed();
                emptyAtRange(innerOffset, bufferAreaSize);
                /*
                destroyThread = new Thread(emptyAtRange);
                destroyThread.Start(new Vector2Int(innerOffset, bufferAreaSize));
                */
                break;

            case Mode.SetInactive:
                setDisplayed();
                setTrailInactive();
                /*
                hidenLoadThread = new Thread(setTrailInactive);
                hidenLoadThread.Start();
                */
                break;

            case Mode.AreaLoad:
                setDisplayed();
                setBufferArea();
                emptyAtRange(outerOffset, bufferAreaSize);
                /*
                hidenLoadThread = new Thread(setBufferArea);
                hidenLoadThread.Start();
                destroyThread = new Thread(emptyAtRange);
                destroyThread.Start(new Vector2Int(outerOffset, bufferAreaSize));
                */
                break;

            case Mode.FullLoad:
                setDisplayed();
                setBufferArea();
                /*
                hidenLoadThread = new Thread(setBufferArea);
                hidenLoadThread.Start();
                */
                break;

            default:
                break;
        }
    }
    
    void setDisplayed()
    {
        Vector3Int pos = Vector3Int.zero;
        for (int x = 0; x < nbChunkDisplay; x++)
        {
            for (int y = 0; y < nbChunkDisplay; y++)
            {
                for (int z = 0; z < nbChunkDisplay; z++)
                {
                    pos.Set(centerChunk.x - innerOffset + x,
                        centerChunk.y - innerOffset + y,
                        centerChunk.z - innerOffset + z);
                    if(isInChunkPosRange(pos))
                    {
                        if (chunkMatrix[pos.x, pos.y, pos.z].state == ChunkState.Null)
                        {
                            chunkMatrix[pos.x, pos.y, pos.z] = generateEmptyChunk(pos);
                        }
                        loadChunk(chunkMatrix[pos.x, pos.y, pos.z], false);
                    }
                }
            }
        }
    }

    void setBufferArea()
    {
        Vector3Int pos = Vector3Int.zero;
        for(int n = 1; n <= bufferAreaSize; n++)
        {
            // front and back (perpendicular to x axis)
            for (int i = 0; i < 2 * (innerOffset + n) + 1; i++) // y axis
            {
                for (int j = 0; j < 2 * (innerOffset + n) + 1; j++) // z axis
                {
                    //front (x+)
                    pos.Set(centerChunk.x + innerOffset + n,
                        centerChunk.y - innerOffset - n + i,
                        centerChunk.z - innerOffset - n + j);
                    if (isInChunkPosRange(pos)) loadChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                    //back (x-)
                    pos.Set(centerChunk.x - innerOffset - n,
                        centerChunk.y - innerOffset - n + i,
                        centerChunk.z - innerOffset - n + j);
                    if (isInChunkPosRange(pos)) loadChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                }
            }

            // top and down (perpendicular to y axis)
            for (int i = 0; i < 2 * (innerOffset + n-1) + 1; i++) // x axis
            {
                for (int j = 0; j < 2 * (innerOffset + n) + 1; j++) // z axis
                {
                    //top (y+)
                    pos.Set(centerChunk.x - innerOffset - (n-1) + i,
                        centerChunk.y + innerOffset + n ,
                        centerChunk.z - innerOffset - n + j);
                    if (isInChunkPosRange(pos)) loadChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                    //down (y-)
                    pos.Set(centerChunk.x - innerOffset - (n-1) + i,
                        centerChunk.y - innerOffset - n,
                        centerChunk.z - innerOffset - n + j);
                    if (isInChunkPosRange(pos)) loadChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                }
            }

            // right and left (perpendicular to z axis)
            for (int i = 0; i < 2 * (innerOffset + n - 1) + 1; i++) // x axis
            {
                for (int j = 0; j < 2 * (innerOffset + n - 1) + 1; j++) // y axis
                {
                    //top (z+)
                    pos.Set(centerChunk.x - innerOffset - (n - 1) + i,
                        centerChunk.y - innerOffset - (n - 1) + j,
                        centerChunk.z + innerOffset + n);
                    if (isInChunkPosRange(pos)) loadChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                    //down (z-)
                    pos.Set(centerChunk.x - innerOffset - (n - 1) + i,
                        centerChunk.y - innerOffset - (n - 1) + j,
                        centerChunk.z - innerOffset - n );
                    if (isInChunkPosRange(pos)) loadChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                }
            }
        }

    }

    void emptyAtRange(int offset, int range) // non threadable version
    {
        // start emptying 

        Vector3Int pos = Vector3Int.zero;
        for (int n = 1; n <= range; n++)
        {
            // often used value (x-, y-, z- corner)
            Vector3Int lowCorner = new Vector3Int(centerChunk.x - offset - n,
                                        centerChunk.y - offset - n,
                                        centerChunk.z - offset - n);

            // front and back (perpendicular to x axis)
            for (int i = 0; i < 2 * (offset + n) + 1; i++) // y axis
            {
                for (int j = 0; j < 2 * (offset + n) + 1; j++) // z axis
                {
                    //front (x+)
                    pos.Set(centerChunk.x + offset + n,
                        lowCorner.y + i,
                        lowCorner.z + j);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                    //back (x-)
                    pos.Set(lowCorner.x,
                        lowCorner.y + i,
                        lowCorner.z + j);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                }
            }

            // top and down (perpendicular to y axis)
            for (int i = 0; i < 2 * (offset + n - 1) + 1; i++) // x axis
            {
                for (int j = 0; j < 2 * (offset + n) + 1; j++) // z axis
                {
                    //top (y+)
                    pos.Set(lowCorner.x + 1 + i,
                        centerChunk.y + offset + n,
                        lowCorner.z + j);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                    //down (y-)
                    pos.Set(lowCorner.x + 1 + i,
                        lowCorner.y,
                        lowCorner.z + j);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                }
            }

            // left and right (perpendicular to z axis)
            for (int i = 0; i < 2 * (offset + n - 1) + 1; i++) // x axis
            {
                for (int j = 0; j < 2 * (offset + n - 1) + 1; j++) // y axis
                {
                    //left (z+)
                    pos.Set(lowCorner.x + 1 + i,
                        lowCorner.y + 1 + j,
                        centerChunk.z + offset + n);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                    //right (z-)
                    pos.Set(lowCorner.x + 1 + i,
                        lowCorner.y + 1 + j,
                        lowCorner.z);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                }
            }
        }
    }

    void emptyAtRange(System.Object obj)
    {
        // recover data for thread
        Vector2Int data;
        int offset = outerOffset;
        int range = bufferAreaSize;
        data = (Vector2Int)obj;

        if(data == null)
        {
            offset = data.x;
            range = data.y;
        }

        // start emptying 

        Vector3Int pos = Vector3Int.zero;
        for (int n = 1; n <= range; n++)
        {
            // often used value (x-, y-, z- corner)
            Vector3Int lowCorner = new Vector3Int(centerChunk.x - offset - n,
                                        centerChunk.y - offset - n,
                                        centerChunk.z - offset - n);

            // front and back (perpendicular to x axis)
            for (int i = 0; i < 2 * (offset + n) + 1; i++) // y axis
            {
                for (int j = 0; j < 2 * (offset + n) + 1; j++) // z axis
                {
                    //front (x+)
                    pos.Set(centerChunk.x + offset + n,
                        lowCorner.y + i,
                        lowCorner.z + j);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                    //back (x-)
                    pos.Set(lowCorner.x,
                        lowCorner.y + i,
                        lowCorner.z + j);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                }
            }

            // top and down (perpendicular to y axis)
            for (int i = 0; i < 2 * (offset + n - 1) + 1; i++) // x axis
            {
                for (int j = 0; j < 2 * (offset + n) + 1; j++) // z axis
                {
                    //top (y+)
                    pos.Set(lowCorner.x + 1 + i,
                        centerChunk.y + offset + n,
                        lowCorner.z + j);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                    //down (y-)
                    pos.Set(lowCorner.x + 1 + i,
                        lowCorner.y,
                        lowCorner.z + j);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                }
            }

            // left and right (perpendicular to z axis)
            for (int i = 0; i < 2 * (offset + n - 1) + 1; i++) // x axis
            {
                for (int j = 0; j < 2 * (offset + n - 1) + 1; j++) // y axis
                {
                    //left (z+)
                    pos.Set(lowCorner.x + 1 + i,
                        lowCorner.y + 1 + j,
                        centerChunk.z + offset + n);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                    //right (z-)
                    pos.Set(lowCorner.x + 1 + i,
                        lowCorner.y + 1 + j,
                        lowCorner.z);
                    if (isInChunkPosRange(pos)) emptyChunk(chunkMatrix[pos.x, pos.y, pos.z]);

                }
            }
        }
    }

    void setTrailInactive()
    {
        Vector3Int delta = centerChunk - oldCenter;
        delta.Clamp(-Vector3Int.one, Vector3Int.one);

        Vector3Int pos = Vector3Int.zero;
        int size = nbChunkDisplay + 2 * bufferAreaSize;

        // often used value (x-, y-, z- corner)
        Vector3Int lowCorner = new Vector3Int(centerChunk.x - size,
                                        centerChunk.y - size,
                                        centerChunk.z - size);

        for (int n = 1; n <= bufferAreaSize; n++)
        {
            if (delta.x != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        pos.Set(centerChunk.x - delta.x * (innerOffset + n),
                            lowCorner.y + i,
                            lowCorner.z + j);
                        if (isInChunkPosRange(pos) && chunkMatrix[pos.x, pos.y, pos.z].state == ChunkState.Active)
                        {
                            setActive(chunkMatrix[pos.x, pos.y, pos.z], false);
                        }
                    }
                }
            }
            if (delta.y != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        pos.Set(lowCorner.x + i,
                            centerChunk.y - delta.y * (innerOffset + n),
                            lowCorner.z + j);
                        if (isInChunkPosRange(pos) && chunkMatrix[pos.x, pos.y, pos.z].state == ChunkState.Active)
                        {
                            setActive(chunkMatrix[pos.x, pos.y, pos.z], false);
                        }
                    }
                }
            }
            if (delta.z != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        pos.Set(lowCorner.x + i,
                            lowCorner.y + j,
                            centerChunk.z - delta.z * (innerOffset + n));
                        if (isInChunkPosRange(pos) && chunkMatrix[pos.x, pos.y, pos.z].state == ChunkState.Active)
                        {
                            setActive(chunkMatrix[pos.x, pos.y, pos.z], false);
                        }
                    }
                }
            }
        }
    }
}