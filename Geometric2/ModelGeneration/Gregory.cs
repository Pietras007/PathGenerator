using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
namespace Geometric2.ModelGeneration
{
    public class Gregory : Element
	{
		List<List<List<ModelGeneration.Point>>> hole = new List<List<List<Point>>>();
		public GregoryPiece gp1;
		public GregoryPiece gp2;
		public GregoryPiece gp3;
		int gregoryHoleNumber;
		Camera _camera;

		public Gregory(int gregoryHoleNumber, Camera _camera, int width, int height, List<List<List<Point>>> hole)
		{
			this.gregoryHoleNumber = gregoryHoleNumber;
			this._camera = _camera;
			gp1 = new GregoryPiece(gregoryHoleNumber * 10 + 1, _camera, width, height);
			gp2 = new GregoryPiece(gregoryHoleNumber * 10 + 2, _camera, width, height);
			gp3 = new GregoryPiece(gregoryHoleNumber * 10 + 3, _camera, width, height);
			this.hole = hole;
		}

		public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader = null)
		{
		}

		public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _patchGeometryShader)
		{
			DrawHole();
		}

		public void DrawHole()
		{
			//if (shouldRecalc)
			{
				List<List<List<Vector3>>> R = new List<List<List<Vector3>>>();
				List<List<List<Vector3>>> S = new List<List<List<Vector3>>>();
				List<List<List<Vector3>>> T = new List<List<List<Vector3>>>();

				for (int i = 0; i < 3; ++i)
				{
					List<List<Vector3>> patchRs = new List<List<Vector3>>();
					for (int j = 0; j < 2; ++j)
					{
						List<Vector3> layer = new List<Vector3>();
						for (int k = 0; k < 3; ++k)
						{
							var p1 = hole[i][j][k].Position();
							var p2 = hole[i][j][k + 1].Position();
							layer.Add((p1 + p2) / 2.0f);
						}
						patchRs.Add(layer);
					}
					R.Add(patchRs);
				}

				for (int i = 0; i < 3; ++i)
				{
					List<List<Vector3>> patchSs = new List<List<Vector3>>();
					for (int j = 0; j < 2; ++j)
					{
						List<Vector3> layer = new List<Vector3>();
						for (int k = 0; k < 2; ++k)
						{
							var p1 = R[i][j][k];
							var p2 = R[i][j][k + 1];
							layer.Add((p1 + p2) / 2.0f);
						}
						patchSs.Add(layer);
					}
					S.Add(patchSs);
				}

				for (int i = 0; i < 3; ++i)
				{
					List<List<Vector3>> patchTs = new List<List<Vector3>>();
					for (int j = 0; j < 2; ++j)
					{
						List<Vector3> layer = new List<Vector3>();
						for (int k = 0; k < 1; ++k)
						{
							var p1 = S[i][j][k];
							var p2 = S[i][j][k + 1];
							layer.Add((p1 + p2) / 2.0f);
						}
						patchTs.Add(layer);
					}
					T.Add(patchTs);
				}

				List<Vector3> P2i = new List<Vector3>();
				List<Vector3> Qi = new List<Vector3>();
				for (int i = 0; i < 3; ++i)
				{
					var t0 = T[i][0][0];
					var t1 = T[i][1][0];
					P2i.Add(t0 + (t0 - t1));
					Qi.Add((3.0f * P2i[i] - t0) / 2.0f);
				}

				Vector3 P = (Qi[0] + Qi[1] + Qi[2]) / 3.0f;
				List<Vector3> P1i = new List<Vector3>();

				for (int i = 0; i < 3; ++i)
				{
					P1i.Add((2.0f * Qi[i] + P) / 3.0f);
				}

				List<List<Vector3>> innerS = new List<List<Vector3>>();
				List<List<Vector3>> innerR = new List<List<Vector3>>();
				List<List<Vector3>> innerInner = new List<List<Vector3>>();

				for (int i = 0; i < 3; ++i)
				{
					List<Vector3> patchInnerS = new List<Vector3>();
					List<Vector3> patchInnerR = new List<Vector3>();
					List<Vector3> patchInnerInner = new List<Vector3>();
					patchInnerS.Add(S[i][0][0] + (S[i][0][0] - S[i][1][0]));
					patchInnerS.Add(S[i][0][1] + (S[i][0][1] - S[i][1][1]));

					patchInnerR.Add(R[i][0][0] + (R[i][0][0] - R[i][1][0]));
					patchInnerR.Add(R[i][0][2] + (R[i][0][2] - R[i][1][2]));

					patchInnerInner.Add(patchInnerS[0] + (P1i[i] - P2i[i]));
					patchInnerInner.Add(patchInnerS[1] + (P1i[i] - P2i[i]));

					innerS.Add(patchInnerS);
					innerR.Add(patchInnerR);
					innerInner.Add(patchInnerInner);
				}

				int left, right;
				List<Vector3> gregory1 = new List<Vector3>();
				List<Vector3> gregory2 = new List<Vector3>();
				List<Vector3> gregory3 = new List<Vector3>();

				left = 0;
				right = 1;

#pragma region initGregory0
				//0
				gregory1.Add(T[left][0][0]);
				//1
				gregory1.Add(P2i[left]);
				//2
				gregory1.Add(P1i[left]);
				//3
				gregory1.Add(P);
				//4
				gregory1.Add(S[left][0][1]);
				//5
				gregory1.Add(innerS[left][1]);
				//6
				gregory1.Add(innerS[left][1]);
				//7
				gregory1.Add(innerInner[left][1]);
				//8
				gregory1.Add(innerInner[right][0]);
				//9
				gregory1.Add(P1i[right]);
				//10
				gregory1.Add(R[left][0][2]);
				//11
				gregory1.Add(innerR[left][1]);
				//12
				gregory1.Add(innerR[right][0]);
				//13
				gregory1.Add(innerS[right][0]);
				//14
				gregory1.Add(innerS[right][0]);
				//15
				gregory1.Add(P2i[right]);
				//16
				gregory1.Add(hole[right][0][0].Position());
				//17
				gregory1.Add(R[right][0][0]);
				//18
				gregory1.Add(S[right][0][0]);
				//19
				gregory1.Add(T[right][0][0]);
#pragma endregion

				left = 1;
				right = 2;

#pragma region initgregory1
				//0
				gregory2.Add(T[left][0][0]);
				//1
				gregory2.Add(P2i[left]);
				//2
				gregory2.Add(P1i[left]);
				//3
				gregory2.Add(P);
				//4
				gregory2.Add(S[left][0][1]);
				//5
				gregory2.Add(innerS[left][1]);
				//6
				gregory2.Add(innerS[left][1]);
				//7
				gregory2.Add(innerInner[left][1]);
				//8
				gregory2.Add(innerInner[right][0]);
				//9
				gregory2.Add(P1i[right]);
				//10
				gregory2.Add(R[left][0][2]);
				//11
				gregory2.Add(innerR[left][1]);
				//12
				gregory2.Add(innerR[right][0]);
				//13
				gregory2.Add(innerS[right][0]);
				//14
				gregory2.Add(innerS[right][0]);
				//15
				gregory2.Add(P2i[right]);
				//16
				gregory2.Add(hole[right][0][0].Position());
				//17
				gregory2.Add(R[right][0][0]);
				//18
				gregory2.Add(S[right][0][0]);
				//19
				gregory2.Add(T[right][0][0]);
#pragma endregion

				left = 2;
				right = 0;

#pragma region initgregory2
				//0
				gregory3.Add(T[left][0][0]);
				//1
				gregory3.Add(P2i[left]);
				//2
				gregory3.Add(P1i[left]);
				//3
				gregory3.Add(P);
				//4
				gregory3.Add(S[left][0][1]);
				//5
				gregory3.Add(innerS[left][1]);
				//6
				gregory3.Add(innerS[left][1]);
				//7
				gregory3.Add(innerInner[left][1]);
				//8
				gregory3.Add(innerInner[right][0]);
				//9
				gregory3.Add(P1i[right]);
				//10
				gregory3.Add(R[left][0][2]);
				//11
				gregory3.Add(innerR[left][1]);
				//12
				gregory3.Add(innerR[right][0]);
				//13
				gregory3.Add(innerS[right][0]);
				//14
				gregory3.Add(innerS[right][0]);
				//15
				gregory3.Add(P2i[right]);
				//16
				gregory3.Add(hole[right][0][0].Position());
				//17
				gregory3.Add(R[right][0][0]);
				//18
				gregory3.Add(S[right][0][0]);
				//19
				gregory3.Add(T[right][0][0]);
#pragma endregion

				for (int i = 0; i < 20; i++)
				{
					gp1.points[i].CenterPosition = gregory1[i];
					//gp1.points[i].Translation = new Vector3(0, 0, 0);
					//gp1.points[i].TemporaryTranslation = new Vector3(0, 0, 0);

					gp2.points[i].CenterPosition = gregory2[i];
					//gp2.points[i].Translation = new Vector3(0, 0, 0);
					//gp2.points[i].TemporaryTranslation = new Vector3(0, 0, 0);

					gp3.points[i].CenterPosition = gregory3[i];
					//gp3.points[i].Translation = new Vector3(0, 0, 0);
					//gp3.points[i].TemporaryTranslation = new Vector3(0, 0, 0);
				}
			}
		}
	}
}
