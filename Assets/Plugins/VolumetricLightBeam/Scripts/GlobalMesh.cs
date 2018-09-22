using UnityEngine;
using System.Collections;

namespace VLB
{
    public static class GlobalMesh
    {
        public static Mesh mesh
        {
            get
            {
                if(ms_Mesh == null)
                {
                    ms_Mesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
                    ms_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
                }

                return ms_Mesh;
            }
        }

#if UNITY_EDITOR
        public static void Destroy()
        {
            if (ms_Mesh != null)
            {
                GameObject.DestroyImmediate(ms_Mesh);
                ms_Mesh = null;
            }
        }
#endif

        static Mesh ms_Mesh = null;
    }
}
