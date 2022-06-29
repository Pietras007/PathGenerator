using Geometric2.RasterizationClasses;
using Intersect;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometric2.Intersect;
using Geometric2.DrillLines;
using Geometric2.Global;

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
        GlobalData globalData;
        public List<(Vector2 pParam, Vector2 qParam)> res = null;

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

        public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader, TeselationShader _gregoryShader, GlobalData globalData)
        {
            this.globalData = globalData;
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

        public void RemoveTextures()
        {
            surface1.SetTexture(null, OpenTK.Graphics.OpenGL4.TextureUnit.Texture1, 1);
            surface2.SetTexture(null, OpenTK.Graphics.OpenGL4.TextureUnit.Texture1, 2);
        }


        private void FindIntersection()
        {
            var intersection = FindIntersectionCurve();
            if (intersection.Item1 != null && intersection.Item2 != null)
            {
                List<Vector3> intersectionPoints = intersection.Item1;
                var param = intersection.Item2;
                if (intersectionPoints.Count > 0)
                {
                    intersectionLines = new IntersectionLines(intersectionPoints);
                    (Texture, Texture) textures = CreateTexture(param);
                    surface1.SetTexture(textures.Item1, OpenTK.Graphics.OpenGL4.TextureUnit.Texture1, 1);
                    surface2.SetTexture(textures.Item2, OpenTK.Graphics.OpenGL4.TextureUnit.Texture2, 2);
                }
            }
        }

        private (Texture, Texture) CreateTexture(List<(Vector2 _pParam, Vector2 _qParam)> parameters)
        {
            int tex_size = 1000;
            List<Vector2> pParam = parameters.Select(x => x._pParam).ToList();
            List<Vector2> qParam = parameters.Select(x => x._qParam).ToList();

            float[,] surface1_FullTex = new float[tex_size, tex_size];
            float[,] surface2_FullTex = new float[tex_size, tex_size];

            for(int i=0;i<pParam.Count;i++)
            {
                pParam[i] = new Vector2(pParam[i].X * tex_size, pParam[i].Y * tex_size);
            }

            for (int i = 0; i < qParam.Count; i++)
            {
                qParam[i] = new Vector2(qParam[i].X * tex_size, qParam[i].Y * tex_size);
            }

            Parallel.For(0, tex_size, i =>
            {
                int u = i;
                for (int v = 0; v < tex_size; v ++)
                {
                    if (HelpFunctions.IsInPolygon(new Vector2(u, v), pParam))
                    {
                        surface1_FullTex[u,v] = 1;
                    }
                    else
                    {
                        surface1_FullTex[u, v] = 0;
                    }

                    if (HelpFunctions.IsInPolygon(new Vector2(u, v), qParam))
                    {
                        surface2_FullTex[u, v] = 1;
                    }
                    else
                    {
                        surface2_FullTex[u, v] = 0;
                    }
                }
            });

            //Parallel.For(0, tex_size, i =>
            //{
            //    float u = (float)i / (float)tex_size;
            //    //for (float u = 0; u <= 1; u += 0.001f)
            //    //{
            //    for (float v = 0; v <= 1; v += (float)1/(float)tex_size)
            //    {
            //        if (HelpFunctions.IsInPolygon(new Vector2(u, v), pParam))
            //        {
            //            surface1_FullTex[(int)(u * tex_size), (int)(v * tex_size)] = 1;
            //        }
            //        else
            //        {
            //            surface1_FullTex[(int)(u * tex_size), (int)(v * tex_size)] = 0;
            //        }

            //        if (HelpFunctions.IsInPolygon(new Vector2(u, v), qParam))
            //        {
            //            surface2_FullTex[(int)(u * tex_size), (int)(v * tex_size)] = 1;
            //        }
            //        else
            //        {
            //            surface2_FullTex[(int)(u * tex_size), (int)(v * tex_size)] = 0;
            //        }
            //    }
            //});

            float[] surface1_Tex = new float[tex_size * tex_size];
            float[] surface2_Tex = new float[tex_size * tex_size];


            Parallel.For(0, tex_size, i =>
            {
                for (int j = 0; j < tex_size; j++)
                {
                    var x = surface1_FullTex[i, j];
                    surface1_Tex[j * tex_size + i] = x;
                }
            });

            Parallel.For(0, tex_size, i =>
            {
                for (int j = 0; j < tex_size; j++)
                {
                    var x = surface2_FullTex[i, j];
                    surface2_Tex[j * tex_size + i] = x;
                }
            });

            foreach(var x in surface1_Tex)
            {
                if(x > 0.5)
                {
                    var dupa = 5;
                }
            }

            foreach (var x in surface2_Tex)
            {
                if (x > 0.5)
                {
                    var dupa = 5;
                }
            }

            Texture tex1 = new Texture(tex_size, tex_size, surface1_Tex);
            Texture tex2 = new Texture(tex_size, tex_size, surface2_Tex);

            return (tex1, tex2);
        }

        private void UpdateIntersection()
        {
            //List<Vector3> intersectionPoints = FindIntersectionCurve();
            //intersectionLines.linePoints = intersectionPoints;
        }

        private (List<Vector3>, List<(Vector2 pParam, Vector2 qParam)>) FindIntersectionCurve()
        {
            if ((surface1 is ModelGeneration.BezierPatchC0 || surface1 is ModelGeneration.BezierPatchC2) && (surface2 is ModelGeneration.BezierPatchC0 || surface2 is ModelGeneration.BezierPatchC2))
            {
                int iterations = 0;
                int ile = 0;
                List<Vector3> alpP = new List<Vector3>();
                res = new List<(Vector2, Vector2)>();
                do
                {
                    if (globalData.UseSelectedPoint)
                    {
                        res = Intersection.Curve(surface1, surface2, d, globalData.selectedPoint);
                    }
                    else
                    {
                        res = Intersection.Curve(surface1, surface2, d);
                    }
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
                    if (iterations > 5)
                    {
                        break;
                    }
                }
                while (res == null || ile < res.Count - 2);//|| ile < 100);

                return (alpP, res);
            }

            return (null, null);
        }
    }
}
