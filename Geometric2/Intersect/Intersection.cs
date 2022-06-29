using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Intersect
{
    public static class Intersection
    {
        private const float H = 1e-3f;
        private static Random random = new Random();

        public static (Vector2 pParam, Vector2 qParam)? Point(ISurface p, ISurface q, Vector3? closeTo = null)
        {
            bool self = p == q;
            (Vector2 param, Vector3 pos) P = self ? (new Vector2(0.75f), p.P(0.75f, 0.75f)) : (new Vector2(0.5f), p.P(0.5f, 0.5f));
            (Vector2 param, Vector3 pos) Q = self ? (new Vector2(0.25f), q.P(0.25f, 0.25f)) : (new Vector2(0.5f), q.P(0.5f, 0.5f));
            float R = 1f;
            if (closeTo != null)
            {
                List<((Vector2, Vector3) point, float dist)> pTemp = new List<((Vector2, Vector3), float)>();
                List<((Vector2, Vector3) point, float dist)> qTemp = new List<((Vector2, Vector3), float)>();
                for (int j = 0; j < 65536; j++)
                {
                    Vector2 puv = UV(new Vector2(Rand(1), Rand(1)) + P.param);
                    Vector2 quv = UV(new Vector2(Rand(1), Rand(1)) + Q.param);
                    Vector3 ppp = p.P(puv.X, puv.Y);
                    Vector3 qqq = q.P(quv.X, puv.Y);

                    pTemp.Add(((puv, ppp), Vector3.DistanceSquared(ppp, closeTo.Value)));
                    qTemp.Add(((quv, qqq), Vector3.DistanceSquared(qqq, closeTo.Value)));
                }
                List<(Vector2, Vector3)> ppTemp = pTemp
                    .OrderBy(s => s.dist)
                    .Take(4096)
                    .Select(s => s.point)
                    .ToList();
                List<(Vector2, Vector3)> qqTemp = qTemp
                    .OrderBy(s => s.dist)
                    .Take(4096)
                    .Select(s => s.point)
                    .ToList();
                float bestDist = Vector3.DistanceSquared(P.pos, Q.pos);
                foreach ((Vector2 param, Vector3 pos) qq in qqTemp)
                {
                    foreach ((Vector2 param, Vector3 pos) pp in ppTemp)
                    {
                        if (self && Vector2.DistanceSquared(pp.param, qq.param) < 0.3) continue;

                        float nextDist = Vector3.DistanceSquared(qq.pos, pp.pos);
                        if (nextDist < bestDist)
                        {
                            bestDist = nextDist;
                            Q = qq;
                            P = pp;
                        }
                    }
                }
                R = 0.1f;
            }
            for (int i = 0; i < 100; i++)
            {
                List<(Vector2 param, Vector3 pos)> pTemp = new List<(Vector2, Vector3)>();
                List<(Vector2 param, Vector3 pos)> qTemp = new List<(Vector2, Vector3)>();
                for (int j = 0; j < 1024; j++)
                {
                    Vector2 puv = UV(new Vector2(Rand(R), Rand(R)) + P.param);
                    Vector2 quv = UV(new Vector2(Rand(R), Rand(R)) + Q.param);
                    pTemp.Add((puv, p.P(puv.X, puv.Y)));
                    qTemp.Add((quv, q.P(quv.X, quv.Y)));
                }
                float lastDist = Vector3.DistanceSquared(P.pos, Q.pos);
                float bestDist = lastDist;
                foreach ((Vector2 param, Vector3 pos) qq in qTemp)
                {
                    foreach ((Vector2 param, Vector3 pos) pp in pTemp)
                    {
                        if (self && Vector2.DistanceSquared(pp.param, qq.param) < 0.3) continue;
                        float nextDist = Vector3.DistanceSquared(qq.pos, pp.pos);
                        if (nextDist < bestDist)
                        {
                            bestDist = nextDist;
                            Q = qq;
                            P = pp;
                        }
                    }
                }
                if (lastDist > bestDist) R = R * (float)Math.Sqrt(bestDist / lastDist) * 1.5f;
                else if (closeTo != null) R = R * 0.1f;
                if (bestDist < 1e-8f)
                {
                    if (self && Vector2.DistanceSquared(P.param, Q.param) < 1e-3f) return null;
                    return (P.param, Q.param);
                }
            }
            return null;
        }

        private static (Vector2 pParam, Vector2 qParam) Next(ISurface p, ISurface q, (Vector2 pParam, Vector2 qParam) x0, float d, int maxIter)
        {
            Vector4 xn = new Vector4(x0.pParam.X, x0.pParam.Y, x0.qParam.X, x0.qParam.Y), xp;
            Vector3 tangent = Vector3.Cross(p.N(xn.X, xn.Y), q.N(xn.X, xn.Y)).Normalized();
            
            do
            {
                xp = new Vector4(xn);
                xn -= func(xn.X, xn.Y, xn.Z, xn.W, p, q, x0, tangent, d) * J(xn.X, xn.Y, xn.Z, xn.W, p, q, x0, tangent, d);
            }
            while ((xp - xn).LengthSquared > 1e-6 && maxIter-- > 0);
            return (xn.Xy, xn.Zw);
        }

        public static Vector4 func(float u, float v, float s, float t, ISurface p, ISurface q, (Vector2 pParam, Vector2 qParam) x0, Vector3 tangent, float d)
        {
            Vector3 pmq = p.P(u, v) - q.P(s, t);
            float val = Vector3.Dot(p.P(u, v) - p.P(x0.pParam.X, x0.pParam.Y), tangent) - d;
            return new Vector4(pmq, val);
        }

        public static Matrix4 J(float u, float v, float s, float t, ISurface p, ISurface q, (Vector2 pParam, Vector2 qParam) x0, Vector3 tangent, float d)
        {
            Vector4 f = func(u, v, s, t, p, q, x0, tangent, d);
            return new Matrix4(
                (func(u + H, v, s, t, p, q, x0, tangent, d) - f) / H,
                (func(u, v + H, s, t, p, q, x0, tangent, d) - f) / H,
                (func(u, v, s + H, t, p, q, x0, tangent, d) - f) / H,
                (func(u, v, s, t + H, p, q, x0, tangent, d) - f) / H
            ).Inverted();
        }

        public static List<(Vector2 pParam, Vector2 qParam)> Curve(ISurface p, ISurface q, float d, Vector3? closeTo = null)
        {
            (Vector2 pParam, Vector2 qParam)? start = Point(p, q, closeTo);
            if (start == null) return null;
            Vector3 found = p.P(start.Value.pParam.X, start.Value.pParam.Y);
            (Vector2 pParam, Vector2 qParam) next = (start.Value.pParam, start.Value.qParam);
            Func<float, float> wrap = val => val < 0 ? val + 1 : val > 1 ? val - 1 : val;
            LinkedList<(Vector2 pParam, Vector2 qParam)> parameters = new LinkedList<(Vector2 pParam, Vector2 qParam)>();
            parameters.AddLast(start.Value);
            int i = 1000;
            while (i-- > 000)
            {
                next = Next(p, q, next, d, 100);
                parameters.AddLast(next);
                if (!p.WrapsU() && (next.pParam.X > 1 || next.pParam.X < 0)) break;
                else next.pParam.X = wrap(next.pParam.X);
                if (!p.WrapsV() && (next.pParam.Y > 1 || next.pParam.Y < 0)) break;
                else next.pParam.Y = wrap(next.pParam.Y);
                if (!q.WrapsU() && (next.qParam.X > 1 || next.qParam.X < 0)) break;
                else next.qParam.X = wrap(next.qParam.X);
                if (!q.WrapsV() && (next.qParam.Y > 1 || next.qParam.Y < 0)) break;
                else next.qParam.Y = wrap(next.qParam.Y);
                if (Vector3.Distance(found, p.P(next.pParam.X, next.pParam.Y)) < d && i < 995)
                {
                    parameters.AddLast(start.Value);
                    return parameters.ToList();
                }
            }
            i = 1000;
            next = (start.Value.pParam, start.Value.qParam);
            while (i-- > 0)
            {
                next = Next(p, q, next, -d, 100);
                parameters.AddFirst(next);
                if (!p.WrapsU() && (next.pParam.X > 1 || next.pParam.X < 0)) break;
                else next.pParam.X = wrap(next.pParam.X);
                if (!p.WrapsV() && (next.pParam.Y > 1 || next.pParam.Y < 0)) break;
                else next.pParam.Y = wrap(next.pParam.Y);
                if (!q.WrapsU() && (next.qParam.X > 1 || next.qParam.X < 0)) break;
                else next.qParam.X = wrap(next.qParam.X);
                if (!q.WrapsV() && (next.qParam.Y > 1 || next.qParam.Y < 0)) break;
                else next.qParam.Y = wrap(next.qParam.Y);
            }

            return parameters.ToList();
        }

        private static Vector2 UV(Vector2 uv)
        {
            uv.X = uv.X % 1;
            if (uv.X < 0) uv.X += 1;
            uv.Y = uv.Y % 1;
            if (uv.Y < 0) uv.Y += 1;
            return uv;
            //uv.X = Clamp(uv.X, 0, 1);
            //uv.Y = Clamp(uv.Y, 0, 1);
            //return uv;
        }

        private static float Clamp(float val, float min, float max)
        {
            if (val < min)
            {
                return min;
            }
            else if (val > max)
            {
                return max;
            }
            else if (val >= min && val <= max)
            {
                return val;
            }
            else
            {
                return 0.5f;
            }
        }

        private static float Rand(float r)
        {
            return r * ((float)random.NextDouble() - 0.5f);
        }
    }
}