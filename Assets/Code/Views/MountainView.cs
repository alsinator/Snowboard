using Snowboard;
using System.Collections.Generic;
using UnityEngine;

namespace Snowboard.Views
{
    public class MountainView : MonoBehaviour
    {
        public ObjectsPool pool;

        public Material groundMaterial;

        public List<ParallaxBackground> backgrounds;

        public TextAsset patternsFile;

        public float segmentLength = 2f;

        public int maxSegmentPoints = 100;

        public int maxSegments;
    }
}