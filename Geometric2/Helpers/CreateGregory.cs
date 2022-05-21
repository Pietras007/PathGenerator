using Geometric2.ModelGeneration;
using Geometric2.RasterizationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.Helpers
{
	public struct SinglePatch
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
            for (int i = 0; i < Elements.Count; ++i)
            {
                if (Elements[i].IsSelected)
                {
                    if (Elements[i] is BezierPatchC0)
                    {
                        patches.Add(Elements[i] as BezierPatchC0);
                    }
                    else
                    {
                        Console.WriteLine("There sould be only Bezier Patches C0 selected \n");
                        return holes;
                    }
                }
            }

            var res = CheckIfCanMerge(Elements, SelectedElements, patches);

            if (res.Count > 0)
            {
                for (int i = 0; i < res.Count; ++i)
                {
                    var ress = res[i];
                    List<List<List<Point>>> rr = new List<List<List<Point>>>();
                    rr.Add(ress[0].patch);
                    rr.Add(ress[1].patch);
                    rr.Add(ress[2].patch);
                    Gregory h = new Gregory(gregoryHoleNumber, _camera, width, height, rr);
                    //ress[0].bezier.holes.Add(h);
                    //ress[1].bezier.holes.Add(h);
                    //ress[2].bezier.holes.Add(h);

                    holes.Add(h);
                    //if (!program->allGregorys) break;
                }
            }
            else
            {
                Console.WriteLine("Can't Merge\n");
            }

            return holes;
        }

        List<List<SinglePatch>> CheckIfCanMerge(List<Element> Elements, List<Element> SelectedElements, List<BezierPatchC0> bezierPatches)
        {
            List<List<SinglePatch>> res = new List<List<SinglePatch>>();
            List<SinglePatch> patches = new List<SinglePatch>();
            for (int i = 0; i < bezierPatches.Count; ++i)
            {
                var patches_i = bezierPatches[i].GetAllPatches();

                patches.AddRange(patches_i);
            }

            for (int i = 0; i < patches.Count; ++i)
                for (int j = i + 1; j < patches.Count; ++j)
                    for (int k = j + 1; k < patches.Count; ++k)
                    {
                        var merge = CanMerge(patches[i], patches[j], patches[k]);
                        res.AddRange(merge);
                    }
            return res;
        }


        List<List<SinglePatch>> CanMerge(SinglePatch patch0, SinglePatch patch1, SinglePatch patch2)
        {
            List<List<SinglePatch>> res = new List<List<SinglePatch>>();
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    for (int k = 0; k < 8; ++k)
                    {
                        if (patch0.patch[0][3] == patch1.patch[0][0] && patch1.patch[0][3] == patch2.patch[0][0] && patch2.patch[0][3] == patch0.patch[0][0])
                        {
                            bool isOk = true;
                            BezierPatchC0 b1 = patch0.bezier;
                            BezierPatchC0 b2 = patch1.bezier;
                            BezierPatchC0 b3 = patch2.bezier;

                            //for (int a = 0; a < b1.holes.Count; ++a)
                            //    for (int b = 0; b < b2.holes.Count; ++b)
                            //        for (int c = 0; c < b3.holes.Count; ++c)
                            //            if (b1.holes[a] == b2.holes[b] && b1.holes[a] == b3.holes[c])
                            //                isOk = false;

                            if (isOk)
                            {
                                var ress = new List<SinglePatch>();
                                ress.Add(patch0);
                                ress.Add(patch1);
                                ress.Add(patch2);
                                res.Add(ress);
                            }
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

                for (int i = 0; i < 4; ++i)
                {
                    List<Point> line = new List<Point>();
                    for (int j = 0; j < 4; ++j)
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

                for (int i = 0; i < 4; ++i)
                {
                    List<Point> line = new List<Point>();
                    for (int j = 0; j < 4; ++j)
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
