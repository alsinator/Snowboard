using Snowboard.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Snowboard.Controllers
{
    public class MountainController : GameActor
    {

        private MountainView view;

        private ObjectsPool pool;

        private GeneratorPatterns.PatternList generatorPatterns;

        private List<GroundSegment> segments;

        private int lastSegment = 0;
        private int lastPattern = 0;

        private Vector3 groundHead;
        private Vector3 groundDirection;

        private Vector3 targetDirection;
        private Vector3 originalDirection;

        private int maxSegmentPoints;
        private float segmentLength;
        private int maxSegments;

        private const float chanceFlatAfterFlat = 0.4f;
        private const float chanceUpAfterFlat = 0.2f;
        private const float chanceFlatAfterDown = 0.5f;
        private const float chanceFlatAfterUp = 0.4f;

        private enum GeneratorDirections
        {
            Flat,
            Up,
            Down
        }

        private GeneratorDirections generatorDirection = GeneratorDirections.Flat;


        public MountainController(Camera mainCamera, MountainView mountainView)
        {
            view = mountainView;
            pool = view.pool;

            maxSegmentPoints = view.maxSegmentPoints; // This should be separated into something like mountainProperties
            maxSegments = view.maxSegments;
            segmentLength = view.segmentLength;

            generatorPatterns = GeneratorPatterns.Load(view.patternsFile.ToString());

            pool.Init();

            for (int i = 0; i < view.backgrounds.Count; i++)
            {
                view.backgrounds[i].Camera = mainCamera;
            }

            GenerateInitialSegments();
        }


        public void UpdatePlayerPosition(Vector3 player)
        {
            var segment = segments[lastSegment];

            // Generate a new segment if the player is already far
            if (segment.OutOfRange(player))
            {
                // Resue one of the existing ones
                lastSegment = (lastSegment + 1) % segments.Count;
                UpdateSegment(segment);
            }
        }

        public override void OnGameReset()
        {
            GenerateInitialSegments();
            lastSegment = 0;
        }

        public override void Update()
        {
        }

        private Vector3[] GenerateSegmentPoints()
        {
            var points = new Vector3[view.maxSegmentPoints];

            var dist = segmentLength / maxSegmentPoints;

            for (int i = 0; i < maxSegmentPoints; i++)
            {
                points[i] = groundHead;

                if (i < maxSegmentPoints - 1)
                {
                    // Reuse the last point of a segment as the first for the next one
                    groundHead += groundDirection * dist;
                    float delta = (float)i / maxSegmentPoints;
                    groundDirection = Vector3.Slerp(originalDirection, targetDirection, delta);
                }
            }

            ChangeDirection();

            return points;
        }


        private void ChangeDirection()
        {
            originalDirection = groundDirection;

            if (generatorDirection == GeneratorDirections.Flat)
            {
                if (Random.value < chanceFlatAfterFlat)
                {
                    GoFlat();
                    return;
                }

                if (Random.value < chanceUpAfterFlat)
                {
                    GoUp();
                    return;
                }

                GoDown();
                return;
            }

            if (generatorDirection == GeneratorDirections.Down)
            {
                if (Random.value < chanceFlatAfterDown)
                {
                    GoFlat();
                    return;
                }

                GoDown();
                return;
            }

            if (Random.value < chanceFlatAfterUp)
            {
                GoFlat();
                return;
            }

            GoDown();
        }


        public Vector3 GetGroundAt(Vector3 position)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];
                if (segment.InRange(position))
                {
                    return segment.GetGroundAt(position);
                }
            }

            return Vector3.zero;
        }


        private void GoFlat()
        {
            generatorDirection = GeneratorDirections.Flat;
            targetDirection = new Vector3(1f, Random.value * -0.1f - 0.1f, 0f);
            targetDirection.Normalize();
        }

        private void GoUp()
        {
            generatorDirection = GeneratorDirections.Up;
            targetDirection = new Vector3(1f, Random.value * 0.25f + 0.5f, 0f);
            targetDirection.Normalize();
        }

        private void GoDown()
        {
            generatorDirection = GeneratorDirections.Down;
            targetDirection = new Vector3(1f, Random.value * -0.65f - 0.35f, 0f);
            targetDirection.Normalize();
        }


        private void GenerateInitialSegments()
        {
            groundHead = new Vector3(-6f, -1f, 0f);

            groundDirection = new Vector3(1f, -0.07f, 0f);
            groundDirection.Normalize();
            originalDirection = targetDirection = groundDirection;

            if (segments == null)
            {
                segments = new(maxSegments);

                for (int i = 0; i < maxSegments; i++)
                {
                    var points = GenerateSegmentPoints();
                    GenerateSegment(points, i > 2);
                }
            }
            else
            {
                for (int i = 0; i < maxSegments; i++)
                {
                    UpdateSegment(segments[i]);
                }
            }
        }

        private void GenerateSegment(Vector3[] points, bool populate)
        {
            var segment = new GroundSegment(maxSegmentPoints, view.transform, view.groundMaterial);

            segment.GenerateMesh(points);

            if (populate)
            {
                PopulateSegment(segment);
            }

            segments.Add(segment);
        }

        private void UpdateSegment(GroundSegment segment)
        {
            segment.CleanUp();

            var newPoints = GenerateSegmentPoints();
            segment.GenerateMesh(newPoints);
            PopulateSegment(segment);
        }


        private void PopulateSegment(GroundSegment segment)
        {
            // Choose random pattern
            int patternIdx;
            do
            {
                patternIdx = Random.Range(0, generatorPatterns.patternList.Count);
            } while (patternIdx == lastPattern && generatorPatterns.patternList.Count > 1);

            var pattern = generatorPatterns.patternList[patternIdx].elements;
            lastPattern = patternIdx;

            for (int i = 0; i < pattern.Count; i++)
            {
                var deco = pool.GetObject(pattern[i].Id);
                if (deco != null)
                {
                    segment.AddObject(deco, pattern[i].X, pattern[i].Y);
                }
            }
        }
    }
}