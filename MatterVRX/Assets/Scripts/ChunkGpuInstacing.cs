using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGpuInstacing : MonoBehaviour
{
    public int globalSize = 100;
    public int chunkSize = 5;
    public int nbChunkDisplay = 5;
    public float minScale = 0.05f;
    public float maxScale = 0.5f;
    public Transform prefab;
    public Vector3 centerOffset = new Vector3(0, 0, 0);

    struct Voxel
    {
        public float size;
        public Color color;
    }

    struct Chunk
    {
        public Transform[,,] voxels;
        public Vector3Int chunkPos;
    }

    private Voxel[,,] dataSet;
    private Chunk[,,] displayChunkMatrix;
    private Vector3Int centerChunk;
    private int maxChunkPos;

    // Start is called before the first frame update
    void Start()
    {
        dataSet = populateDataSet();
        maxChunkPos = (int)System.Math.Ceiling((double)globalSize / chunkSize);
        centerChunk = new Vector3Int(3, 3, 3);
        displayChunkMatrix = initDisplay(dataSet, centerChunk);
        transform.localPosition = -(centerChunk + new Vector3(0.5f, 0.5f, 0.5f)) * chunkSize + centerOffset;
        print("center chunk : " + centerChunk);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = centerOffset - transform.localPosition;
        Vector3 chunkScale = chunkSize * transform.localScale;

        Vector3Int curCenter = new Vector3Int((int)System.Math.Floor(delta.x/chunkScale.x),
            (int)System.Math.Floor(delta.y / chunkScale.y),
            (int)System.Math.Floor(delta.z / chunkScale.z));

        if (curCenter != centerChunk)
        {
            moveDisplayedChunk(dataSet, displayChunkMatrix, centerChunk, curCenter);
        }
    }

    // "read" dataSet (here set randomly)
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


    
    Chunk populateChunk(Voxel[,,] dataSet, Vector3Int chunkPos)
    {
        Chunk chunk;
        chunk.voxels = new Transform[chunkSize, chunkSize, chunkSize];
        chunk.chunkPos = chunkPos;

        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    Vector3Int pos = new Vector3Int(chunkPos.x * chunkSize + x, chunkPos.y * chunkSize + y, chunkPos.z * chunkSize + z);
                    
                    Transform t = Instantiate(prefab);
                    t.SetParent(transform);
                    t.localPosition = pos;
                    t.GetComponent<VisualManager>().SetScale(dataSet[pos.x, pos.y, pos.z].size);
                    properties.SetColor("_Color", dataSet[pos.x, pos.y, pos.z].color);
                    t.GetComponent<MeshRenderer>().SetPropertyBlock(properties);
                    chunk.voxels[x, y, z] = t;
                }
            }
        }
        return chunk;
    }

    void emptyChunk(Chunk chunk)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (chunk.voxels != null && chunk.voxels[x, y, z] != null)
                        Destroy(chunk.voxels[x, y, z].gameObject);
                }
            }
        }
    }



    Chunk[,,] initDisplay(Voxel[,,] dataSet, Vector3Int centerChunk)
    {
        if (nbChunkDisplay % 2 == 0) nbChunkDisplay++; // make sure we have a center chunk
        int offset = (int)System.Math.Floor((double)nbChunkDisplay / 2);

        Chunk[,,] chunks = new Chunk[maxChunkPos, maxChunkPos, maxChunkPos];

        for (int x = 0; x < nbChunkDisplay; x++)
        {
            // do nothing if outside the dataSet
            if (centerChunk.x - offset + x > 0 &&
                centerChunk.x - offset + x < maxChunkPos)
            {
                for (int y = 0; y < nbChunkDisplay; y++)
                {
                    // do nothing if outside the dataSet
                    if (centerChunk.y - offset + y > 0 &&
                        centerChunk.y - offset + y < maxChunkPos)
                    {
                        for (int z = 0; z < nbChunkDisplay; z++)
                        {
                            // do nothing if outside the dataSet
                            if (centerChunk.z - offset + z > 0 &&
                                centerChunk.z - offset + z < maxChunkPos)
                            {
                                Vector3Int chunkPos = centerChunk + new Vector3Int(x - offset, y - offset, z - offset);
                                chunks[chunkPos.x, chunkPos.y, chunkPos.z] = populateChunk(dataSet, chunkPos);
                            }
                        }
                    }
                }
            }
        }

        return chunks;
    }

    void moveDisplayedChunk(Voxel[,,] dataSet, Chunk[,,] chunksMatrix, Vector3Int oldCenterChunk, Vector3Int newCenterChunk)
    {
        Vector3Int delta = newCenterChunk - oldCenterChunk;
        delta.Clamp(-Vector3Int.one, Vector3Int.one);
        int chunkOffset = (int)System.Math.Floor((double)nbChunkDisplay / 2);
        print("delta : " + delta);
        print("offSet : " + chunkOffset);

        Vector3Int pos = Vector3Int.zero;

        if (delta.x != 0)
        {
            for (int i = 0; i < nbChunkDisplay; i++)
            {
                for (int j = 0; j < nbChunkDisplay; j++)
                {
                    pos.Set(oldCenterChunk.x - delta.x * chunkOffset,
                        oldCenterChunk.y - chunkOffset + i,
                        oldCenterChunk.z - chunkOffset + j);
                    if (isInChunkPosRange(pos))
                    { 
                        emptyChunk(chunksMatrix[pos.x, pos.y, pos.z]); 
                    }

                    pos.Set(oldCenterChunk.x + delta.x * (chunkOffset + 1),
                        oldCenterChunk.y - chunkOffset + i,
                        oldCenterChunk.z - chunkOffset + j);
                    if(isInChunkPosRange(pos) &&
                        chunksMatrix[pos.x, pos.y, pos.z].voxels == null)
                    {
                        chunksMatrix[pos.x, pos.y, pos.z] = populateChunk(dataSet, pos);
                    }

                }
            }
        }
        if (delta.y != 0)
        {
            for (int i = 0; i < nbChunkDisplay; i++)
            {
                for (int j = 0; j < nbChunkDisplay; j++)
                {
                    pos.Set(oldCenterChunk.x - chunkOffset + i,
                        oldCenterChunk.y - delta.y * chunkOffset,
                        oldCenterChunk.z - chunkOffset + j);
                    if (isInChunkPosRange(pos))
                    {
                        emptyChunk(chunksMatrix[pos.x, pos.y, pos.z]);
                    }

                    pos.Set(oldCenterChunk.x - chunkOffset + i,
                        oldCenterChunk.y + delta.y * (chunkOffset + 1),
                        oldCenterChunk.z - chunkOffset + j);
                    if (isInChunkPosRange(pos) &&
                        chunksMatrix[pos.x, pos.y, pos.z].voxels == null)
                    {
                        chunksMatrix[pos.x, pos.y, pos.z] = populateChunk(dataSet, pos);
                    }

                }
            }
        }
        if (delta.z != 0)
        {
            for (int i = 0; i < nbChunkDisplay; i++)
            {
                for (int j = 0; j < nbChunkDisplay; j++)
                {
                    pos.Set(oldCenterChunk.x - chunkOffset + i,
                        oldCenterChunk.y - chunkOffset + j,
                        oldCenterChunk.z - delta.z * chunkOffset);
                    if (isInChunkPosRange(pos))
                    {
                        emptyChunk(chunksMatrix[pos.x, pos.y, pos.z]);
                    }

                    pos.Set(oldCenterChunk.x - chunkOffset + i,
                        oldCenterChunk.y - chunkOffset + j,
                        oldCenterChunk.z + delta.z * (chunkOffset + 1));
                    if (isInChunkPosRange(pos) &&
                        chunksMatrix[pos.x, pos.y, pos.z].voxels == null)
                    {
                        chunksMatrix[pos.x, pos.y, pos.z] = populateChunk(dataSet, pos);
                    }

                }
            }
        }
        centerChunk = oldCenterChunk + delta;
    }


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
}
