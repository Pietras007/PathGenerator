using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL4;
using Geometric2.MatrixHelpers;
using Geometric2.ModelGeneration;
using System.Numerics;
using OpenTK;
using Geometric2.Helpers;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using Geometric2.Global;
using Geometric2.RasterizationClasses;
using System.Collections.Generic;

namespace Geometric2
{
    public partial class Form1 : Form
    {
        int fBufferRed, texColorBufferRed;
        int fBufferBlue, texColorBufferBlue;
        int anaglyphVAO, anaglyphVBO;
        float[] anaglyphVertices = {
            -1.0f, 1.0f, 0.0f, 1.0f,
            -1.0f, -1.0f, 0.0f, 0.0f,
             1.0f, -1.0f, 1.0f, 0.0f,

            -1.0f, 1.0f, 0.0f, 1.0f,
             1.0f, -1.0f, 1.0f, 0.0f,
             1.0f, 1.0f, 1.0f, 1.0f
        };

        private void glControl1_Load(object sender, EventArgs e)
        {
            Elements.Add(xyzLines);
            Elements.Add(transformCenterLines);

            coursor.width = glControl1.Width;
            coursor.height = glControl1.Height;
            GL.ClearColor(Color.LightCyan);
            GL.Enable(EnableCap.DepthTest);
            _shader = new Shader("./../../../Shaders/VertexShader.vert", "./../../../Shaders/FragmentShader.frag");
            _shaderGeometry = new ShaderGeometry("./../../../Shaders/VertexShaderGeometry.vert", "./../../../Shaders/FragmentShaderGeometry.frag", "./../../../Shaders/GeometryShaderGeometry.geom");
            _patchShaderGeometry = new ShaderGeometry("./../../../Shaders/PatchVertexShader.vert", "./../../../Shaders/PatchFragmentShader.frag", "./../../../Shaders/PatchShaderGeometry.geom");
            _patchShaderGeometryC2 = new ShaderGeometry("./../../../Shaders/PatchVertexShader.vert", "./../../../Shaders/PatchFragmentShader.frag", "./../../../Shaders/PatchShaderGeometryC2.geom");

            coursor.CreateCoursor(_shader);
            foreach (var el in Elements)
            {
                el.CreateGlElement(_shader, _shaderGeometry);
            }

            _camera = new Camera(new Vector3(0, 5, 15), glControl1.Width / (float)glControl1.Height);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            Matrix4 viewMatrix = _camera.GetViewMatrix();
            Matrix4 projectionMatrix = _camera.GetProjectionMatrix();
            _shader.SetMatrix4("view", viewMatrix);
            _shader.SetMatrix4("projection", projectionMatrix);
            _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));

            _shaderGeometry.Use();
            _shaderGeometry.SetMatrix4("view", viewMatrix);
            _shaderGeometry.SetMatrix4("projection", projectionMatrix);

            _patchShaderGeometry.Use();
            _patchShaderGeometry.SetMatrix4("view", viewMatrix);
            _patchShaderGeometry.SetMatrix4("projection", projectionMatrix);

            _patchShaderGeometryC2.Use();
            _patchShaderGeometryC2.SetMatrix4("view", viewMatrix);
            _patchShaderGeometryC2.SetMatrix4("projection", projectionMatrix);
            //_shaderGeometry.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            if (anaglyphOn)
            {
                Matrix4 anaglyphViewMatrix = _camera.GetViewMatrix(_camera.EyeSeparation);
                Matrix4 anaglyphProjectionMatrixRed = _camera.GetAnaglyphProjectionMatrixRed();
                _shader.SetMatrix4("projection", anaglyphProjectionMatrixRed);
                _shaderGeometry.SetMatrix4("projection", anaglyphProjectionMatrixRed);
                _patchShaderGeometry.SetMatrix4("projection", anaglyphProjectionMatrixRed);
                _patchShaderGeometryC2.SetMatrix4("projection", anaglyphProjectionMatrixRed);
                _shader.SetMatrix4("view", anaglyphViewMatrix);
                _shaderGeometry.SetMatrix4("view", anaglyphViewMatrix);
                _patchShaderGeometry.SetMatrix4("view", anaglyphViewMatrix);
                _patchShaderGeometryC2.SetMatrix4("view", anaglyphViewMatrix);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.ColorMask(true, false, false, true);
                RenderScene(viewMatrix, projectionMatrix);

                GL.Clear(ClearBufferMask.DepthBufferBit);
                GL.ColorMask(false, true, true, true);

                anaglyphViewMatrix = _camera.GetViewMatrix(-_camera.EyeSeparation);
                Matrix4 anaglyphProjectionMatrixBlue = _camera.GetAnaglyphProjectionMatrixBlue();
                _shader.SetMatrix4("projection", anaglyphProjectionMatrixBlue);
                _shaderGeometry.SetMatrix4("projection", anaglyphProjectionMatrixBlue);
                _patchShaderGeometry.SetMatrix4("projection", anaglyphProjectionMatrixBlue);
                _patchShaderGeometryC2.SetMatrix4("projection", anaglyphProjectionMatrixBlue);
                _shader.SetMatrix4("view", anaglyphViewMatrix);
                _shaderGeometry.SetMatrix4("view", anaglyphViewMatrix);
                _patchShaderGeometry.SetMatrix4("view", anaglyphViewMatrix);
                _patchShaderGeometryC2.SetMatrix4("view", anaglyphViewMatrix);

                RenderScene(viewMatrix, projectionMatrix);
                GL.ColorMask(true, true, true, true);
            }
            else
            {
                RenderScene(viewMatrix, projectionMatrix);
            }

            GL.Flush();
            glControl1.SwapBuffers();
        }

        private void RenderScene(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            _shader.Use();
            coursor.DrawCoursor(_shader, viewMatrix, projectionMatrix, _camera);
            foreach (var el in Elements)
            {
                if (transformCenterLines.rotationPoint == RotationPoint.Coursor)
                {
                    if (el is ModelGeneration.BezierPatchC0 bPC0)
                    {
                        bPC0.RenderGlElement(_shader, coursor.CoursorGloalPosition, _patchShaderGeometry);
                    }
                    else if (el is ModelGeneration.BezierPatchTubeC0 bPTC0)
                    {
                        bPTC0.RenderGlElement(_shader, coursor.CoursorGloalPosition, _patchShaderGeometry);
                    }
                    else if (el is ModelGeneration.BezierPatchC2 bPC2)
                    {
                        bPC2.RenderGlElement(_shader, coursor.CoursorGloalPosition, _patchShaderGeometryC2);
                    }
                    else if (el is ModelGeneration.BezierPatchTubeC2 bPTC2)
                    {
                        bPTC2.RenderGlElement(_shader, coursor.CoursorGloalPosition, _patchShaderGeometryC2);
                    }
                    else if (el is ModelGeneration.GregoryPiece gP)
                    {
                        gP.RenderGlElement(_shader, transformCenterLines.rotationCenterPoint, _patchShaderGeometry);
                    }
                    else
                    {
                        el.RenderGlElement(_shader, coursor.CoursorGloalPosition, _shaderGeometry);
                    }
                }
                else
                {
                    if (el is ModelGeneration.BezierPatchC0 bPC0)
                    {
                        bPC0.RenderGlElement(_shader, transformCenterLines.rotationCenterPoint, _patchShaderGeometry);
                    }
                    else if (el is ModelGeneration.BezierPatchTubeC0 bPTC0)
                    {
                        bPTC0.RenderGlElement(_shader, transformCenterLines.rotationCenterPoint, _patchShaderGeometry);
                    }
                    else if (el is ModelGeneration.BezierPatchC2 bPC2)
                    {
                        bPC2.RenderGlElement(_shader, transformCenterLines.rotationCenterPoint, _patchShaderGeometryC2);
                    }
                    else if (el is ModelGeneration.BezierPatchTubeC2 bPTC2)
                    {
                        bPTC2.RenderGlElement(_shader, transformCenterLines.rotationCenterPoint, _patchShaderGeometryC2);
                    }
                    else if (el is ModelGeneration.GregoryPiece gP)
                    {
                        gP.RenderGlElement(_shader, transformCenterLines.rotationCenterPoint, _patchShaderGeometry);
                    }
                    else
                    {
                        el.RenderGlElement(_shader, transformCenterLines.rotationCenterPoint, _shaderGeometry);
                    }
                }
            }

            if (selectedBezierC2 != null && selectedBezierC2.UseBernsteinBasis)
            {
                foreach (var el in selectedBezierC2.bernsteinPoints)
                {
                    if (el is ModelGeneration.Point p)
                    {
                        p.CreateGlElement(_shader);
                        p.RenderGlElement(_shader, coursor.CoursorGloalPosition, _shaderGeometry);
                    }
                }
            }

            if (selectedBezierPatchC0 != null)
            {
                foreach (var p in selectedBezierPatchC0.bezierPoints)
                {
                    if (p.IsSelected)
                    {
                        p.CreateGlElement(_shader);
                        p.RenderGlElement(_shader, coursor.CoursorGloalPosition, _shaderGeometry);
                    }
                }
            }

            if (selectedBezierPatchTubeC0 != null)
            {
                foreach (var p in selectedBezierPatchTubeC0.bezierPoints)
                {
                    if (p.IsSelected)
                    {
                        p.CreateGlElement(_shader);
                        p.RenderGlElement(_shader, coursor.CoursorGloalPosition, _shaderGeometry);
                    }
                }
            }

            if (selectedBezierPatchC2 != null)
            {
                foreach (var p in selectedBezierPatchC2.bezierPoints)
                {
                    if (p.IsSelected)
                    {
                        p.CreateGlElement(_shader);
                        p.RenderGlElement(_shader, coursor.CoursorGloalPosition, _shaderGeometry);
                    }
                }
            }

            if (selectedBezierPatchTubeC2 != null)
            {
                foreach (var p in selectedBezierPatchTubeC2.bezierPoints)
                {
                    if (p.IsSelected)
                    {
                        p.CreateGlElement(_shader);
                        p.RenderGlElement(_shader, coursor.CoursorGloalPosition, _shaderGeometry);
                    }
                }
            }
        }

        private void glControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (drawingStatus)
                {
                    case DrawingStatus.No:
                        break;
                    case DrawingStatus.Point:
                        ModelGeneration.Point point = new ModelGeneration.Point(coursor.CoursorGloalPosition, pointNumber, _camera);
                        pointNumber++;
                        point.CreateGlElement(_shader);
                        elementsOnScene.Items.Add(point);
                        Elements.Add(point);
                        break;
                    case DrawingStatus.Torus:
                        Torus torus = new Torus(coursor.CoursorGloalPosition, torusNumber);
                        torusNumber++;
                        torus.CreateGlElement(_shader);
                        elementsOnScene.Items.Add(torus);
                        Elements.Add(torus);
                        selectedTorus = torus;
                        break;

                    case DrawingStatus.Select:
                        float dist = float.MaxValue;
                        float maxDist = 1.0f;
                        Element element = null;
                        if (selectedBezierC2 != null && selectedBezierC2.UseBernsteinBasis == true)
                        {
                            foreach (var el in selectedBezierC2.bernsteinPoints)
                            {
                                if (el is ModelGeneration.Point)
                                {
                                    Vector3 u = coursor.CoursorGloalPosition - _camera.GetCameraPosition();
                                    Vector3 ap = coursor.CoursorGloalPosition - (el.CenterPosition + el.Translation + el.TemporaryTranslation);
                                    float currentDist = Vector3.Cross(ap, u).Length / u.Length;
                                    if (currentDist < dist)
                                    {
                                        dist = currentDist;
                                        element = el;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var el in Elements)
                            {
                                if (el is ModelGeneration.Point)
                                {
                                    Vector3 u = coursor.CoursorGloalPosition - _camera.GetCameraPosition();
                                    Vector3 ap = coursor.CoursorGloalPosition - (el.CenterPosition + el.Translation + el.TemporaryTranslation);
                                    float currentDist = Vector3.Cross(ap, u).Length / u.Length;
                                    if (currentDist < dist)
                                    {
                                        dist = currentDist;
                                        element = el;
                                    }
                                }
                            }
                        }


                        if (selectedBezierPatchC0 != null)
                        {
                            foreach (var p in selectedBezierPatchC0.bezierPoints)
                            {
                                Vector3 u = coursor.CoursorGloalPosition - _camera.GetCameraPosition();
                                Vector3 ap = coursor.CoursorGloalPosition - (p.CenterPosition + p.Translation + p.TemporaryTranslation);
                                float currentDist = Vector3.Cross(ap, u).Length / u.Length;
                                if (currentDist < dist)
                                {
                                    dist = currentDist;
                                    element = p;
                                }
                            }
                        }

                        if (selectedBezierPatchTubeC0 != null)
                        {
                            foreach (var p in selectedBezierPatchTubeC0.bezierPoints)
                            {
                                Vector3 u = coursor.CoursorGloalPosition - _camera.GetCameraPosition();
                                Vector3 ap = coursor.CoursorGloalPosition - (p.CenterPosition + p.Translation + p.TemporaryTranslation);
                                float currentDist = Vector3.Cross(ap, u).Length / u.Length;
                                if (currentDist < dist)
                                {
                                    dist = currentDist;
                                    element = p;
                                }
                            }
                        }

                        if (selectedBezierPatchC2 != null)
                        {
                            foreach (var p in selectedBezierPatchC2.bezierPoints)
                            {
                                Vector3 u = coursor.CoursorGloalPosition - _camera.GetCameraPosition();
                                Vector3 ap = coursor.CoursorGloalPosition - (p.CenterPosition + p.Translation + p.TemporaryTranslation);
                                float currentDist = Vector3.Cross(ap, u).Length / u.Length;
                                if (currentDist < dist)
                                {
                                    dist = currentDist;
                                    element = p;
                                }
                            }
                        }

                        if (selectedBezierPatchTubeC2 != null)
                        {
                            foreach (var p in selectedBezierPatchTubeC2.bezierPoints)
                            {
                                Vector3 u = coursor.CoursorGloalPosition - _camera.GetCameraPosition();
                                Vector3 ap = coursor.CoursorGloalPosition - (p.CenterPosition + p.Translation + p.TemporaryTranslation);
                                float currentDist = Vector3.Cross(ap, u).Length / u.Length;
                                if (currentDist < dist)
                                {
                                    dist = currentDist;
                                    element = p;
                                }
                            }
                        }

                        if (dist < maxDist && element != null)
                        {
                            if (selectedBezierC2 != null && selectedBezierC2.UseBernsteinBasis == true)
                            {
                                SelectedElements.Clear();
                                foreach (var el in selectedBezierC2.bernsteinPoints)
                                {
                                    el.IsSelected = false;
                                }
                            }

                            element.IsSelected = true;
                            SelectedElements.Add(element);
                        }

                        break;

                    case DrawingStatus.BezierC0NewPoint:
                        if (selectedBezierC0 != null)
                        {
                            ModelGeneration.Point p = new ModelGeneration.Point(coursor.CoursorGloalPosition, pointNumber, _camera);
                            pointNumber++;
                            p.CreateGlElement(_shader);
                            elementsOnScene.Items.Add(p);
                            Elements.Add(p);
                            selectedBezierC0.bezierPoints.Add(p);
                            bezierListBox.Items.Add(p);
                            selectedBezierC0.RegenerateBezierC0();
                            break;
                        }
                        break;

                    case DrawingStatus.BezierC2NewPoint:
                        if (selectedBezierC2 != null)
                        {
                            ModelGeneration.Point p = new ModelGeneration.Point(coursor.CoursorGloalPosition, pointNumber, _camera);
                            pointNumber++;
                            p.CreateGlElement(_shader);
                            elementsOnScene.Items.Add(p);
                            Elements.Add(p);
                            selectedBezierC2.deBoorePoints.Add(p);
                            bezierC2ListBox.Items.Add(p);
                            selectedBezierC2.RegenerateBezierC2();
                            break;
                        }
                        break;

                    case DrawingStatus.InterpolationBezierC2NewPoint:
                        if (selectedInterpolatedBezierC2 != null)
                        {
                            ModelGeneration.Point p = new ModelGeneration.Point(coursor.CoursorGloalPosition, pointNumber, _camera);
                            pointNumber++;
                            p.CreateGlElement(_shader);
                            elementsOnScene.Items.Add(p);
                            Elements.Add(p);
                            selectedInterpolatedBezierC2.interpolatedBC2Points.Add(p);
                            interpolationBezierC2ListBox.Items.Add(p);
                            selectedInterpolatedBezierC2.RegenerateInterpolatedBezierC2();
                            break;
                        }
                        break;
                }
            }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (coursor.CoursorMode != CoursorMode.Manual || coursor.CoursorMoving == true)
            {
                coursor.CoursorScreenPosition = (e.X, e.Y);
            }

            int xPosMouse, yPosMouse;
            if (e.Button == MouseButtons.Middle)
            {
                xPosMouse = e.X;
                yPosMouse = e.Y;
                if (prev_xPosMouse != -1 && prev_yPosMouse != -1)
                {
                    var deltaX = xPosMouse - prev_xPosMouse;
                    var deltaY = yPosMouse - prev_yPosMouse;
                    if (isMovingCameraCentre)
                    {
                        Vector3 newCameraCenterPoint = _camera.CameraCenterPoint;
                        newCameraCenterPoint.X -= (float)deltaX * _camera.CameraDist / glControl1.Width;
                        newCameraCenterPoint.Z -= (float)deltaY * _camera.CameraDist / glControl1.Height;
                        _camera.CameraCenterPoint = newCameraCenterPoint;
                    }
                    else
                    {
                        _camera.RotationX -= (float)(2 * Math.PI * deltaY / glControl1.Height);
                        _camera.RotationY += (float)(2 * Math.PI * deltaX / glControl1.Width);
                    }
                }

                prev_xPosMouse = xPosMouse;
                prev_yPosMouse = yPosMouse;
            }

            if (e.Button == MouseButtons.Right)
            {
                xPosMouse = e.X;
                yPosMouse = e.Y;
                if (prev_xPosMouse != -1 && prev_yPosMouse != -1)
                {
                    var deltaX = xPosMouse - prev_xPosMouse;
                    var deltaY = yPosMouse - prev_yPosMouse;
                    Coursor tempCoursor = new Coursor();
                    tempCoursor.width = glControl1.Width;
                    tempCoursor.height = glControl1.Height;
                    Matrix4 viewMatrix = _camera.GetViewMatrix();
                    Matrix4 projectionMatrix = _camera.GetProjectionMatrix();
                    Vector3 prevMousePos = tempCoursor.GetCoursorGlobalPosition((prev_xPosMouse, prev_yPosMouse), viewMatrix, projectionMatrix, _camera);
                    Vector3 currentMousePos = tempCoursor.GetCoursorGlobalPosition((xPosMouse, yPosMouse), viewMatrix, projectionMatrix, _camera);
                    Vector3 mouseMove = currentMousePos - prevMousePos;
                    foreach (var el in SelectedElements)
                    {
                        if (el is ModelGeneration.Point p)
                        {
                            p.Translation += mouseMove;
                            if (selectedBezierC2 != null && selectedBezierC2.UseBernsteinBasis)
                            {
                                int indexElement = selectedBezierC2.bernsteinPoints.IndexOf(p);
                                int bernstainCase = selectedBezierC2.bernsteinPoints.IndexOf(p);
                                int bezierParts = (int)Math.Ceiling((double)(bernstainCase) / 3.0);
                                if (bezierParts > 0)
                                {
                                    bernstainCase += bezierParts - 1;
                                }

                                int bernsteinCase = bernstainCase % 4;
                                int deBooreToMove = bernsteinCase == 0 ? bernstainCase / 4 + 1 : bernstainCase / 4 + 2;

                                selectedBezierC2.MoveDeBoorePoints(indexElement, bernsteinCase, deBooreToMove);
                            }
                        }

                        if (el is ModelGeneration.Torus t)
                        {
                            t.Translation += mouseMove;
                        }

                        if (el is ModelGeneration.BezierPatchC0 bpC0)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            //bpC0.Translation += mouseMove;
                            foreach (var pp in bpC0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(pp))
                                {
                                    mocedPoints.Add(pp);
                                    pp.Translation += mouseMove;
                                }
                            }
                        }

                        if (el is ModelGeneration.BezierPatchTubeC0 bpTC0)
                        {
                            //bpC0.Translation += mouseMove;
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var pp in bpTC0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(pp))
                                {
                                    mocedPoints.Add(pp);
                                    pp.Translation += mouseMove;
                                }
                            }
                        }

                        if (el is ModelGeneration.BezierPatchC2 bpC2)
                        {
                            //bpC0.Translation += mouseMove;
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var pp in bpC2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(pp))
                                {
                                    mocedPoints.Add(pp);
                                    pp.Translation += mouseMove;
                                }
                            }
                        }

                        if (el is ModelGeneration.BezierPatchTubeC2 bpTC2)
                        {
                            //bpC0.Translation += mouseMove;
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var pp in bpTC2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(pp))
                                {
                                    mocedPoints.Add(pp);
                                    pp.Translation += mouseMove;
                                }
                            }
                        }
                    }
                }

                prev_xPosMouse = xPosMouse;
                prev_yPosMouse = yPosMouse;
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left)
            {
                prev_xPosMouse = -1;
                prev_yPosMouse = -1;
            }
        }

        private void glControl1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            if (_camera.CameraDist - numberOfTextLinesToMove > 1.0f)
            {
                _camera.CameraDist -= numberOfTextLinesToMove;
            }
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                coursor.CoursorMoving = true;
            }
        }

        private void glControl1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
        }

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                coursor.CoursorMoving = false;
            }
        }
    }
}
