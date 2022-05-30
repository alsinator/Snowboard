using System.Collections.Generic;
using UnityEngine;

namespace Snowboard.Views
{
    public class GroundSegment
    {

        public Vector3[] points;


        private GameObject segmentObj;

        private Mesh mesh;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;

        private List<PoolObject> SegmentObjects = new();

        private Vector3[] vertices;
        private Vector2[] uv;

        private const float bottomOffset = 8f;
        private const float rangeOffset = 10f;

        public GroundSegment(int maxPoints, Transform parent, Material groundMaterial)
        {

            mesh = new Mesh();
            vertices = new Vector3[maxPoints * 2];
            uv = new Vector2[maxPoints * 2];

            var indices = new int[maxPoints * 6];

            for (int i = 0; i < maxPoints - 1; i++)
            {
                var idx = i * 6;
                var ptx = i * 2;

                indices[idx] = ptx;
                indices[idx + 1] = ptx + 2;
                indices[idx + 2] = ptx + 1;

                indices[idx + 3] = ptx + 2;
                indices[idx + 4] = ptx + 3;
                indices[idx + 5] = ptx + 1;

            }

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);


            segmentObj = new GameObject("GroundSegment");

            segmentObj.transform.SetParent(parent, false);
            meshFilter = segmentObj.AddComponent<MeshFilter>();
            meshRenderer = segmentObj.AddComponent<MeshRenderer>();

            meshFilter.mesh = mesh;
            meshRenderer.material = groundMaterial;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.allowOcclusionWhenDynamic = false;

        }


        public void GenerateMesh(Vector3[] pointsList)
        {

            var vertOffset = pointsList[0];
            segmentObj.transform.localPosition = vertOffset;

            points = pointsList;

            //float uOffset = Mathf.FloorToInt(pointsList[0].x);

            for (int x = 0; x < pointsList.Length; x++)
            {
                var p = pointsList[x];
                vertices[x * 2] = p - vertOffset;

                var tex = uv[x * 2];
                tex.x = p.x;// - uOffset;
                tex.y = 0f;
                uv[x * 2] = tex;

                p.y -= bottomOffset;
                vertices[x * 2 + 1] = p - vertOffset;

                tex = uv[x * 2 + 1];
                tex.x = p.x;// - uOffset;
                tex.y = 1f;
                uv[x * 2 + 1] = tex;

            }

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);
        }



        public void AddObject(PoolObject newObject, float x, float y)
        {
            var objPos = new Vector3(points[0].x + x, points[0].y, 0f);
            var ground = GetGroundAt(objPos);

            if (ground.y == objPos.y) // Out of Range
            {
                newObject.Despawn();
                return;
            }

            objPos.y = ground.y + y;

            newObject.transform.position = objPos;
            newObject.gameObject.SetActive(true);

            SegmentObjects.Add(newObject);
        }

        public void CleanUp()
        {
            for (int i = 0; i < SegmentObjects.Count; i++)
            {
                // Send all back to pool
                SegmentObjects[i].Despawn();
            }

            SegmentObjects.Clear();
        }


        public bool InRange(Vector3 position)
        {
            return position.x >= points[0].x && position.x < points[^1].x;
        }

        public Vector3 GetGroundAt(Vector3 position)
        {
            for (int i = 1; i < points.Length; i++)
            {
                if (position.x < points[i].x)
                {
                    var length = points[i].x - points[i - 1].x;
                    float delta = (position.x - points[i - 1].x) / length;
                    position.y = points[i].y * delta + points[i - 1].y * (1f - delta);

                    return position;
                }
            }

            return position;
        }

        public bool OutOfRange(Vector3 position)
        {
            return position.x > points[^1].x + rangeOffset;
        }
    }
}