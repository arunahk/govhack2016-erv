using UnityEngine;
using System.Collections.Generic;
using MIConvexHull;
using System;
using UnityEngine.UI;

public class MyGravityMesh : MonoBehaviour
{
    [Header(" [Material to use]")]
    public Material DefaultMeshMaterial = null;

    void Start()
    {

    }

    void Update()
    {
    }

    public void PopulateMesh(List<HeatMapPoint> HeatMapPoints)
    {
        Mesh mesh = ComputeMesh(HeatMapPoints);
        // Set up game object with mesh;
        MeshRenderer renderer = this.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshFilter filter = this.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshCollider collide = this.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        renderer.sharedMaterial = DefaultMeshMaterial;
        // renderer.sortingOrder = 1;
        filter.mesh = mesh;
        collide.sharedMesh = mesh;
    }

    /// <summary>
    /// Use Delauney triangulation to retrieve the triangles for this mesh
    /// </summary>
    /// <returns></returns>
    private Mesh ComputeMesh(List<HeatMapPoint> HeatMapPoints)
    {
        if (HeatMapPoints.Count < 3)
        {
            Debug.LogWarning("Not creating Zone (need >= 3 vertices)");
            return null;
        }

        List<Vector3> vertexList = new List<Vector3>();

        foreach (HeatMapPoint p in HeatMapPoints)
        {
            Vector3 gravityVector = new Vector3(p.SensorData.PGAHorizontal1, p.SensorData.PGAVertical, p.SensorData.PGAHorizontal2);
            Vector3 vertex = new Vector3(p.Position.x, gravityVector.magnitude, p.Position.z);
            vertexList.Add(vertex);
        }


        Mesh mesh = new Mesh();
        // Drop the y-coordinate in order to triangulate in two dimensions.
        // We don't require the ability to triangulate in 3+ dimensions, although the MIConvexHull lib is capable of doing so.
        //Vertex[] vertices = vertexList.ConvertAll(vert3 => new Vertex(vert3.x, vert3.z)).ToArray();
        List<Vertex> v2 = new List<Vertex>();
        foreach (var vertex3 in vertexList)
        {
            v2.Add(new Vertex(vertex3.x, vertex3.z));
        }
        Vertex[] vertices = v2.ToArray();

        // Triangulate the mesh
        var config = new TriangulationComputationConfig
        {
            PointTranslationType = PointTranslationType.TranslateInternal,
            PlaneDistanceTolerance = 0.00001,
            // the translation radius should be lower than PlaneDistanceTolerance / 2
            PointTranslationGenerator = TriangulationComputationConfig.RandomShiftByRadius(0.000001, 0)
        };

        List<Vector3> meshVerts = new List<Vector3>();
        List<int> indices = new List<int>();

        try
        {
            VoronoiMesh<Vertex, Cell, VoronoiEdge<Vertex, Cell>> voronoiMesh;
            voronoiMesh = VoronoiMesh.Create<Vertex, Cell>(vertices, config);

            foreach (var cell in voronoiMesh.Vertices)
            {
                for (int vertNum = 0; vertNum < 3; vertNum++)
                {
                    float x = (float)cell.Vertices[vertNum].Position[0];
                    float y = 0.0f;
                    float z = (float)cell.Vertices[vertNum].Position[1];

                    foreach (var v in vertexList)
                    {
                        if(x == v.x && z == v.z)
                        {
                            y = v.y;
                            break;
                        }
                    }

                    Vector3 vert = new Vector3(x, y, z);
                    int idx;
                    if (meshVerts.Contains(vert))
                    {
                        idx = meshVerts.IndexOf(vert);
                    }
                    else
                    {
                        meshVerts.Add(vert);
                        idx = meshVerts.Count - 1;
                    }
                    indices.Add(idx);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        mesh.vertices = meshVerts.ToArray();
        mesh.triangles = indices.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
}
