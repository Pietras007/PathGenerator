using Geometric2.RasterizationClasses;
using Intersect;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometric2.Intersect;

namespace Geometric2.ModelGeneration
{
    public class IntersectionModel : Element
    {
        private float d;
        public bool DrawPolyline { get; set; }
        ISurface surface1;
        ISurface surface2;
        public IntersectionLines intersectionLines = null;

        Camera _camera;
        int width, height;
        int intersectionNumber;
        int idx = 1000;

        public IntersectionModel(ISurface surface1, ISurface surface2, int intersectionNumber, Camera _camera, int width, int height, float d)
        {
            this.surface1 = surface1;
            this.surface2 = surface2;
            this._camera = _camera;
            this.width = width;
            this.height = height;
            this.intersectionNumber = intersectionNumber;
            this.d = d;
            FullName = "Intersection " + intersectionNumber;
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader, TeselationShader _gregoryShader = null)
        {
            this.FindIntersection();
            if (intersectionLines != null)
            {
                intersectionLines.CreateGlElement(_shader, _geometryShader, _gregoryShader);
            }
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _geometryShader, TeselationShader _gregoryShader = null)
        {
            idx--;
            if (idx == 0)
            {
                this.UpdateIntersection();
                idx = 10;
            }

            if (intersectionLines != null)
            {
                intersectionLines.RenderGlElement(_shader, rotationCentre, _geometryShader, _gregoryShader);
            }
        }

        public void ShowIntersection(bool show)
        {
            if (intersectionLines != null)
            {
                intersectionLines.DrawPolyline = show;
            }
        }


        private void FindIntersection()
        {
            List<Vector3> intersectionPoints = FindIntersectionCurve();
            if (intersectionPoints.Count > 0)
            {
                intersectionLines = new IntersectionLines(intersectionPoints);
            }
        }

        private void UpdateIntersection()
        {
            //List<Vector3> intersectionPoints = FindIntersectionCurve();
            //intersectionLines.linePoints = intersectionPoints;
        }

        private List<Vector3> FindIntersectionCurve()
        {
            if (surface1 is ModelGeneration.BezierPatchC0 bp0 && surface2 is ModelGeneration.BezierPatchC0 bp2)
            {
                var _bp0 = new BestPatch(bp0);
                var _bp1 = new BestPatch(bp2);

                int iterations = 0;
                int ile = 0;
                List<Vector3> alpP = new List<Vector3>();
                List<(Vector2 pParam, Vector2 qParam)> res = new List<(Vector2, Vector2)>();
                do
                {
                    res = Intersection.Curve(surface1, surface2, d);
                    ile = 0;
                    alpP.Clear();
                    if (res != null)
                    {
                        foreach (var x in res)
                        {

                            if (x.pParam.X >= 0 && x.pParam.Y >= 0 && x.qParam.X >= 0 && x.qParam.Y >= 0)
                            {
                                ile++;
                                alpP.Add(surface1.P(x.pParam.X, x.pParam.Y));
                            }
                        }
                    }

                    iterations++;
                    if(iterations > 5)
                    {
                        break;
                    }
                }
                while (res == null || ile < res.Count - 2);//|| ile < 100);

                return alpP;
            }

            return null;
        }
    }
}
