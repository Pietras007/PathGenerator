using Geometric2.ModelGeneration;
using Geometric2.RasterizationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.Helpers
{
    public struct PatchC0
    {
        public BezierPatchC0 bezier;
        public List<List<Point>> patch;
    };

    public class CreateGregoryClass
    {
        public List<Gregory> CreateGregory(int gregoryHoleNumber, Camera _camera, int width, int height, List<Element> Elements, List<Element> SelectedElements)
        {
            List<Gregory> holes = new List<Gregory>();
            List<BezierPatchC0> patches = new List<BezierPatchC0>();
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i].IsSelected)
                {
                    if (Elements[i] is BezierPatchC0)
                    {
                        patches.Add(Elements[i] as BezierPatchC0);
                    }
                }
            }

            var res = CheckIfCanMerge(Elements, SelectedElements, patches);
            if (res.Count > 0)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    var ress = res[i];
                    List<List<List<Point>>> rr = new List<List<List<Point>>>();
                    rr.Add(ress[0].patch);
                    rr.Add(ress[1].patch);
                    rr.Add(ress[2].patch);
                    Gregory h = new Gregory(gregoryHoleNumber, _camera, width, height, rr);
                    holes.Add(h);
                }
            }

            return holes;
        }

        List<List<PatchC0>> CheckIfCanMerge(List<Element> Elements, List<Element> SelectedElements, List<BezierPatchC0> bezierPatches)
        {
            List<List<PatchC0>> res = new List<List<PatchC0>>();
            List<PatchC0> patches = new List<PatchC0>();
            for (int i = 0; i < bezierPatches.Count; i++)
            {
                var patches_i = bezierPatches[i].GetAllPatches();
                patches.AddRange(patches_i);
            }

            for (int i = 0; i < patches.Count; i++)
                for (int j = i + 1; j < patches.Count; j++)
                    for (int k = j + 1; k < patches.Count; k++)
                    {
                        var merge = CanMerge(patches[i], patches[j], patches[k]);
                        res.AddRange(merge);
                    }
            return res;
        }


        List<List<PatchC0>> CanMerge(PatchC0 patch0, PatchC0 patch1, PatchC0 patch2)
        {
            List<List<PatchC0>> res = new List<List<PatchC0>>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (patch0.patch[0][3] == patch1.patch[0][0] && patch1.patch[0][3] == patch2.patch[0][0] && patch2.patch[0][3] == patch0.patch[0][0])
                        {
                            var ress = new List<PatchC0>();
                            ress.Add(patch0);
                            ress.Add(patch1);
                            ress.Add(patch2);
                            res.Add(ress);
                        }

                        patch2.patch = Swap(patch2.patch);
                        if (k % 2 == 1)
                            patch2.patch = Rotate(patch2.patch);
                    }
                    patch1.patch = Swap(patch1.patch);
                    if (j % 2 == 1)
                        patch1.patch = Rotate(patch1.patch);
                }
                patch0.patch = Swap(patch0.patch);
                if (i % 2 == 1)
                    patch0.patch = Rotate(patch0.patch);
            }

            return res;


            List<List<Point>> Rotate(List<List<Point>> patch)
            {
                List<List<Point>> newPatch = new List<List<Point>>();

                for (int i = 0; i < 4; i++)
                {
                    List<Point> line = new List<Point>();
                    for (int j = 0; j < 4; j++)
                    {
                        line.Add(patch[j][3 - i]);
                    }
                    newPatch.Add(line);
                }

                return newPatch;
            }

            List<List<Point>> Swap(List<List<Point>> patch)
            {
                List<List<Point>> newPatch = new List<List<Point>>();

                for (int i = 0; i < 4; i++)
                {
                    List<Point> line = new List<Point>();
                    for (int j = 0; j < 4; j++)
                    {
                        line.Add(patch[i][3 - j]);
                    }
                    newPatch.Add(line);
                }

                return newPatch;
            }
        }
    }
}
