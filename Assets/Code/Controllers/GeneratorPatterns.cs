using System.Collections.Generic;
using UnityEngine;

namespace Snowboard.Controllers
{
    public class GeneratorPatterns
    {

        [System.Serializable]
        public class PatternElement
        {
            public string Id;
            public float X;
            public float Y;
        }

        [System.Serializable]
        public class Pattern
        {
            public List<PatternElement> elements = new List<PatternElement>();
        }

        [System.Serializable]
        public class PatternList
        {
            public List<Pattern> patternList = new List<Pattern>();
        }

        static public PatternList Load(string fileContents)
        {
            return JsonUtility.FromJson<PatternList>(fileContents);
        }

    }
}