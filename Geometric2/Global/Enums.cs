using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.Global
{
    public enum DrawingStatus
    {
        No,
        Select,
        Deselect,
        Remove,
        Rename,

        Point,
        Torus,
        BezierC0,

        BezierC0NewPoint,
        BezierC0AddPoint,
        BezierC0RemovePoint,

        BezierC2NewPoint,
        BezierC2AddPoint,
        BezierC2RemovePoint,

        InterpolationBezierC2NewPoint,
        InterpolationBezierC2AddPoint,
        InterpolationBezierC2RemovePoint
    }

    public enum CoursorMode
    {
        Hidden,
        Manual,
        Auto
    }

    public enum RotationPoint
    {
        Center,
        Coursor,
        ZeroPoint
    }
}
