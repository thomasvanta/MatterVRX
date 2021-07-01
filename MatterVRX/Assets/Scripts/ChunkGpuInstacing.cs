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
        Vector3 adjustedPosition = transform.localPosition / transform.localScale.x - centerOffset;
        Vector3Int curCenter = new Vector3Int((int)System.Math.Floor(-0.5f + adjustedPosition.x / chunkSize),
            (int)System.Math.Floor(-0.5f + adjustedPosition.y / chunkSize),
            (int)System.Math.Floor(-0.5f + adjustedPosition.z / chunkSize));
        curCenter *= - 1;
        if (curCenter != centerChunk)
        {
            centerChunk = curCenter;
            print("center chunk : " + centerChunk);
            moveDisplayedChunk(dataSet, displayChunkMatrix, curCenter);
        }
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

    void emptyChunk(Chunk chunk)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if(chunk.voxels != null && chunk.voxels[x, y, z] != null) 
                        Destroy(chunk.voxels[x, y, z].gameObject);
                }
            }
        }
    }


    void moveDisplayedChunk(Voxel[,,] dataSet, Chunk[,,] chunksMatrix, Vector3Int newCenterChunk)
    {
        //remove unused chunk
        int outerLayerOffset = (int)System.Math.Floor((double)nbChunkDisplay / 2) + 1;
        Vector3Int pos = Vector3Int.zero;
        for(int i = 0; i < nbChunkDisplay + 2; i++)
        {
            for(int j = 0; j < nbChunkDisplay + 2; j++)
            {
                pos.Set(newCenterChunk.x - outerLayerOffset + i,
                        newCenterChunk.y - outerLayerOffset + j, 
                        newCenterChunk.z - outerLayerOffset);
                if(isInChunkPosRange(pos)) emptyChunk(chunksMatrix[pos.x, pos.y, pos.z]);

                pos.Set(newCenterChunk.x - outerLayerOffset + i,
                        newCenterChunk.y - outerLayerOffset + j,
                        newCenterChunk.z + outerLayerOffset);
                if(isInChunkPosRange(pos)) emptyChunk(chunksMatrix[pos.x, pos.y, pos.z]);

                pos.Set(newCenterChunk.x - outerLayerOffset + i,
                        newCenterChunk.y - outerLayerOffset,
                        newCenterChunk.z - outerLayerOffset + j);
                if (isInChunkPosRange(pos)) emptyChunk(chunksMatrix[pos.x, pos.y, pos.z]);

                pos.Set(newCenterChunk.x - outerLayerOffset + i,
                        newCenterChunk.y + outerLayerOffset,
                        newCenterChunk.z - outerLayerOffset + j);
                if (isInChunkPosRange(pos)) emptyChunk(chunksMatrix[pos.x, pos.y, pos.z]);

                pos.Set(newCenterChunk.x - outerLayerOffset,
                    newCenterChunk.y - outerLayerOffset + i,
                    newCenterChunk.z - outerLayerOffset + j);
                if (isInChunkPosRange(pos)) emptyChunk(chunksMatrix[pos.x, pos.y, pos.z]);

                pos.Set(newCenterChunk.x + outerLayerOffset,
                    newCenterChunk.y - outerLayerOffset + i,
                    newCenterChunk.z - outerLayerOffset + j);
                if (isInChunkPosRange(pos)) emptyChunk(chunksMatrix[pos.x, pos.y, pos.z]);
            }
        }

        // add missing voxels
        int offset = (int)System.Math.Floor((double)nbChunkDisplay / 2);
        for (int x = 0; x < nbChunkDisplay; x++)
        {
            // do nothing if outside the dataSet
            if (newCenterChunk.x - offset + x > 0 &&
                newCenterChunk.x - offset + x < maxChunkPos)
            {
                for (int y = 0; y < nbChunkDisplay; y++)
                {
                    // do nothing if outside the dataSet
                    if (newCenterChunk.y - offset + y > 0 &&
                        newCenterChunk.y - offset + y < maxChunkPos)
                    {
                        for (int z = 0; z < nbChunkDisplay; z++)
                        {
                            // do nothing if outside the dataSet
                            if (newCenterChunk.z - offset + z > 0 &&
                                newCenterChunk.z - offset + z < maxChunkPos)
                            {
                                // check if the chunk is already there
                                Vector3Int chunkPos = newCenterChunk + new Vector3Int(x - offset, y - offset, z - offset);
                                if (chunksMatrix[chunkPos.x, chunkPos.y, chunkPos.z].voxels == null)
                                {
                                    chunksMatrix[chunkPos.x, chunkPos.y, chunkPos.z] = populateChunk(dataSet, chunkPos);
                                }
                            }
                        }
                    }
                }
            }
        }
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
