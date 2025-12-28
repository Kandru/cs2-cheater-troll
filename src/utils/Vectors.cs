using CounterStrikeSharp.API.Modules.Utils;

namespace CheaterTroll.Utils
{
    public static class Vectors
    {
        public static float GetDistance(Vector a, Vector b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;
            return MathF.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
        }

        public static float GetDistanceSquared(Vector a, Vector b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;
            return (dx * dx) + (dy * dy) + (dz * dz);
        }

        public static QAngle GetLookAtAngle(Vector source, Vector target)
        {
            // Calculate direction vector from source to target
            Vector direction = new(target.X - source.X, target.Y - source.Y, target.Z - source.Z);
            // Calculate yaw angle (horizontal rotation)
            float yaw = (float)(Math.Atan2(direction.Y, direction.X) * 180 / Math.PI);
            // Calculate pitch angle (vertical rotation)
            float distance = MathF.Sqrt((direction.X * direction.X) + (direction.Y * direction.Y));
            float pitch = (float)(-Math.Atan2(direction.Z, distance) * 180 / Math.PI);
            // Return QAngle with calculated pitch and yaw, roll is 0
            return new QAngle(pitch, yaw, 0);
        }

        public static Vector Normalize(Vector v)
        {
            float length = MathF.Sqrt((v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z));
            return length > 0 ? new Vector(v.X / length, v.Y / length, v.Z / length) : new Vector(0, 0, 0);
        }

        public static Vector Cross(Vector a, Vector b)
        {
            return new Vector(
                (a.Y * b.Z) - (a.Z * b.Y),
                (a.Z * b.X) - (a.X * b.Z),
                (a.X * b.Y) - (a.Y * b.X)
            );
        }

        public static Vector TrimVector(Vector startPos, Vector endPos, float trimDistance)
        {
            // Calculate direction vector from start to end
            Vector direction = new(
                endPos.X - startPos.X,
                endPos.Y - startPos.Y,
                endPos.Z - startPos.Z
            );

            // Calculate the length of the direction vector
            float length = (float)Math.Sqrt((direction.X * direction.X) + (direction.Y * direction.Y) + (direction.Z * direction.Z));

            // If the distance between points is less than or equal to trim distance, return the end position
            if (length <= trimDistance)
            {
                return endPos;
            }

            // Normalize the direction vector
            direction.X /= length;
            direction.Y /= length;
            direction.Z /= length;

            // Move startPos by trimDistance in the direction of endPos
            return new Vector(
                startPos.X + (direction.X * trimDistance),
                startPos.Y + (direction.Y * trimDistance),
                startPos.Z + (direction.Z * trimDistance)
            );
        }

        public static Vector TransformLocalToWorld(Vector localOffset, QAngle angle, bool includeHeight = false)
        {
            // For Source engine, we primarily care about yaw rotation for horizontal positioning
            // Convert yaw to radians (angle.Y is yaw in Source engine)
            float yawRad = (float)(angle.Y * Math.PI / 180.0);

            // Apply simple 2D rotation for X and Y coordinates
            // In Source: X = forward, Y = right, Z = up
            float cos = MathF.Cos(yawRad);
            float sin = MathF.Sin(yawRad);

            // Transform local coordinates to world coordinates
            float worldX = localOffset.X * cos - localOffset.Y * sin;
            float worldY = localOffset.X * sin + localOffset.Y * cos;

            // Handle Z component based on includeHeight parameter
            float worldZ = includeHeight ? localOffset.Z : localOffset.Z;

            return new Vector(worldX, worldY, worldZ);
        }

        /// <summary>
        /// Smooths out curves in a path by inserting interpolated points where the angle between segments exceeds the threshold.
        /// </summary>
        /// <param name="points">The original path points</param>
        /// <param name="curveAngleThreshold">Minimum angle in degrees to consider a curve (default: 30°)</param>
        /// <param name="smoothingFactor">Number of interpolation points to insert per curve (default: 3)</param>
        /// <param name="smoothingRadius">How much to smooth the curve - 0.0 = no smoothing, 1.0 = maximum smoothing (default: 0.5)</param>
        /// <returns>A new list of smoothed path points</returns>
        public static List<Vector> SmoothCurves(List<Vector> points, float curveAngleThreshold = 30f, int smoothingFactor = 3, float smoothingRadius = 0.5f)
        {
            if (points == null || points.Count < 3)
            {
                return points?.ToList() ?? [];
            }

            List<Vector> smoothedPoints = [];
            smoothingRadius = Math.Clamp(smoothingRadius, 0f, 1f);
            smoothingFactor = Math.Max(1, smoothingFactor);
            curveAngleThreshold = Math.Clamp(curveAngleThreshold, 0f, 180f);

            // Always add the first point
            smoothedPoints.Add(points[0]);

            for (int i = 1; i < points.Count - 1; i++)
            {
                Vector prev = points[i - 1];
                Vector current = points[i];
                Vector next = points[i + 1];

                // Calculate angle between the two segments
                float angle = CalculateAngleBetweenSegments(prev, current, next);

                if (angle >= curveAngleThreshold)
                {
                    // This is a curve that needs smoothing
                    List<Vector> curvePoints = CreateSmoothCurve(prev, current, next, smoothingFactor, smoothingRadius);
                    smoothedPoints.AddRange(curvePoints);
                }
                else
                {
                    // Not a significant curve, just add the point
                    smoothedPoints.Add(current);
                }
            }

            // Always add the last point
            smoothedPoints.Add(points[^1]);

            return smoothedPoints;
        }

        /// <summary>
        /// Calculates the angle in degrees between two line segments that meet at a point.
        /// </summary>
        /// <param name="prev">Previous point</param>
        /// <param name="current">Current point (vertex of the angle)</param>
        /// <param name="next">Next point</param>
        /// <returns>Angle in degrees (0-180)</returns>
        private static float CalculateAngleBetweenSegments(Vector prev, Vector current, Vector next)
        {
            // Calculate direction vectors
            Vector dir1 = new(current.X - prev.X, current.Y - prev.Y, current.Z - prev.Z);
            Vector dir2 = new(next.X - current.X, next.Y - current.Y, next.Z - current.Z);

            // Normalize the vectors
            dir1 = Normalize(dir1);
            dir2 = Normalize(dir2);

            // Calculate dot product
            float dotProduct = (dir1.X * dir2.X) + (dir1.Y * dir2.Y) + (dir1.Z * dir2.Z);

            // Clamp to avoid numerical errors
            dotProduct = Math.Clamp(dotProduct, -1f, 1f);

            // Calculate angle in radians, then convert to degrees
            float angleRad = (float)Math.Acos(Math.Abs(dotProduct));
            float angleDeg = (float)(angleRad * 180.0 / Math.PI);

            return angleDeg;
        }

        /// <summary>
        /// Creates a smooth curve between three points using Bézier-like interpolation.
        /// </summary>
        /// <param name="prev">Previous point</param>
        /// <param name="current">Current point (will be smoothed around)</param>
        /// <param name="next">Next point</param>
        /// <param name="numPoints">Number of interpolation points to generate</param>
        /// <param name="smoothingRadius">How much to smooth (0.0 = no smoothing, 1.0 = maximum smoothing)</param>
        /// <returns>List of interpolated points that create a smooth curve</returns>
        private static List<Vector> CreateSmoothCurve(Vector prev, Vector current, Vector next, int numPoints, float smoothingRadius)
        {
            List<Vector> curvePoints = [];

            // Calculate control points for smooth curve
            Vector dir1 = new(current.X - prev.X, current.Y - prev.Y, current.Z - prev.Z);
            Vector dir2 = new(next.X - current.X, next.Y - current.Y, next.Z - current.Z);

            float dist1 = GetDistance(prev, current);
            float dist2 = GetDistance(current, next);
            float avgDist = (dist1 + dist2) * 0.5f;

            // Create control points that pull the curve away from the sharp corner
            float controlDistance = avgDist * smoothingRadius * 0.3f; // Adjust this factor to control curve tightness

            dir1 = Normalize(dir1);
            dir2 = Normalize(dir2);

            Vector control1 = new(
                current.X - (dir1.X * controlDistance),
                current.Y - (dir1.Y * controlDistance),
                current.Z - (dir1.Z * controlDistance)
            );

            Vector control2 = new(
                current.X + (dir2.X * controlDistance),
                current.Y + (dir2.Y * controlDistance),
                current.Z + (dir2.Z * controlDistance)
            );

            // Generate smooth curve points using quadratic Bézier interpolation
            for (int i = 1; i <= numPoints; i++)
            {
                float t = (float)i / (numPoints + 1);
                Vector curvePoint = QuadraticBezier(control1, current, control2, t);
                curvePoints.Add(curvePoint);
            }

            return curvePoints;
        }

        /// <summary>
        /// Calculates a point on a quadratic Bézier curve.
        /// </summary>
        /// <param name="p0">Start point</param>
        /// <param name="p1">Control point</param>
        /// <param name="p2">End point</param>
        /// <param name="t">Parameter from 0 to 1</param>
        /// <returns>Point on the Bézier curve</returns>
        private static Vector QuadraticBezier(Vector p0, Vector p1, Vector p2, float t)
        {
            float oneMinusT = 1 - t;
            float oneMinusTSquared = oneMinusT * oneMinusT;
            float tSquared = t * t;
            float twoOneMinusT = 2 * oneMinusT * t;

            return new Vector(
                (oneMinusTSquared * p0.X) + (twoOneMinusT * p1.X) + (tSquared * p2.X),
                (oneMinusTSquared * p0.Y) + (twoOneMinusT * p1.Y) + (tSquared * p2.Y),
                (oneMinusTSquared * p0.Z) + (twoOneMinusT * p1.Z) + (tSquared * p2.Z)
            );
        }
    }
}