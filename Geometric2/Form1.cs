using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Geometric2.RasterizationClasses;
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
using System.Xml;
using System.Linq;
using System.IO;
using Geometric2.DrillLines;

namespace Geometric2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    glControl1.Invalidate();
                    //Thread.Sleep(16);
                }
            });

            thread.Start();

            Thread thread2 = new Thread(() =>
            {
                Thread.Sleep(1000);
                while (true)
                {
                    Invoke(new Action(() =>
                    {
                        coursorGlobalX.Text = coursor.CoursorGloalPosition.X.ToString();
                        coursorGlobalY.Text = coursor.CoursorGloalPosition.Y.ToString();
                        coursorGlobalZ.Text = coursor.CoursorGloalPosition.Z.ToString();

                        coursorScreenX.Text = coursor.CoursorScreenPosition.Item1.ToString();
                        coursorScreenY.Text = coursor.CoursorScreenPosition.Item2.ToString();

                        if (SelectedElements != null && SelectedElements.Count == 1)
                        {
                            var el = SelectedElements[0];
                            scaleDetails.Text = (el.ElementScale + el.TempElementScale).ToString();
                            //rotationXDetails.Text = 
                        }
                        else
                        {
                            scaleDetails.Text = "-1";
                            rotationXDetails.Text = "-1";
                            rotationYDetails.Text = "-1";
                            rotationZDetails.Text = "-1";
                            translationXDetails.Text = "-1";
                            translationYDetails.Text = "-1";
                            translationZDetails.Text = "-1";
                        }
                    }));

                    Thread.Sleep(16);
                }
            });

            thread2.Start();

            coursorModeComboBox.SelectedIndex = 0;
            rotationPointComboBox.SelectedIndex = 2;
            transformCenterLines.selectedElements = SelectedElements;
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
        }

        private Shader _shader;
        private ShaderGeometry _shaderGeometry;
        private ShaderGeometry _patchShaderGeometry;
        private ShaderGeometry _patchShaderGeometryC2;
        private TeselationShader _gregoryShader;
        private TeselationShader _patchC0Shader;
        private Camera _camera;
        private Coursor coursor = new Coursor();


        private XyzLines xyzLines = new XyzLines();
        private List<Element> Elements = new List<Element>();
        private List<Element> SelectedElements = new List<Element>();
        DrawingStatus drawingStatus = DrawingStatus.No;
        private TransformCenterLines transformCenterLines = new TransformCenterLines();

        private Torus selectedTorus = null;
        private BezierC0 selectedBezierC0 = null;
        private BezierC2 selectedBezierC2 = null;
        private InterpolatedBezierC2 selectedInterpolatedBezierC2 = null;

        private BezierPatchC0 selectedBezierPatchC0 = null;
        private BezierPatchTubeC0 selectedBezierPatchTubeC0 = null;

        private BezierPatchC2 selectedBezierPatchC2 = null;
        private BezierPatchTubeC2 selectedBezierPatchTubeC2 = null;

        int prev_xPosMouse = -1, prev_yPosMouse = -1;
        int pointNumber, torusNumber, bezierC0Number, bezierC2Number, interpolatedBezierC2Number, bezierPatchC0Number, bezierPatchTubeC0Number, bezierPatchC2Number, bezierPatchTubeC2Number;
        bool isMovingCameraCentre, anaglyphOn;

        List<ModelGeneration.BezierPatchC0> patchC0 = null;

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (selectedTorus != null)
            {
                float res;
                if (float.TryParse(textBox2.Text, out res))
                {
                    selectedTorus.torus_R = res;
                }

                selectedTorus.RegenerateTorusVertices();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (selectedTorus != null)
            {
                float res;
                if (float.TryParse(textBox3.Text, out res))
                {
                    selectedTorus.torus_r = res;
                }

                selectedTorus.RegenerateTorusVertices();
            }
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            if (selectedTorus != null)
            {
                selectedTorus.torusMajorDividions = trackBar4.Value;
                textBox4.Text = selectedTorus.torusMajorDividions.ToString();
                selectedTorus.RegenerateTorusVertices();
            }
        }



        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            if (selectedTorus != null)
            {
                selectedTorus.torusMinorDividions = trackBar5.Value;
                textBox8.Text = selectedTorus.torusMinorDividions.ToString();
                selectedTorus.RegenerateTorusVertices();
            }
        }

        private void addPoint_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.Point;
            UpdateDrawingStatus();
        }

        private void addTorus_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.Torus;
            UpdateDrawingStatus();
        }

        private void selectElement_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.Select;
            selectElement.FlatStyle = FlatStyle.Flat;
            selectElement.BackColor = Color.Red;
            UpdateDrawingStatus();
        }

        private void deselectButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.Deselect;
            UpdateDrawingStatus();
        }

        private void removeElement_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.Remove;
            UpdateDrawingStatus();
        }

        private void elementsOnScene_SelectedIndexChanged(object sender, EventArgs e)
        {
            Element element;
            switch (drawingStatus)
            {
                case DrawingStatus.Select:
                    element = (Element)elementsOnScene.SelectedItem;
                    if (element is Torus torus)
                    {
                        selectedTorus = torus;
                    }
                    else if (element is BezierC0 _bezierC0)
                    {
                        selectedBezierC0 = _bezierC0;
                    }
                    else if (element is BezierC2 _bezierC2)
                    {
                        selectedBezierC2 = _bezierC2;
                    }

                    else if (element is InterpolatedBezierC2 _interpolatedBezierC2)
                    {
                        selectedInterpolatedBezierC2 = _interpolatedBezierC2;
                    }

                    else if (element is BezierPatchC0 _bezierPatchC0)
                    {
                        selectedBezierPatchC0 = _bezierPatchC0;
                    }

                    else if (element is BezierPatchTubeC0 _bezierPatchTubeC0)
                    {
                        selectedBezierPatchTubeC0 = _bezierPatchTubeC0;
                    }

                    else if (element is BezierPatchC2 _bezierPatchC2)
                    {
                        selectedBezierPatchC2 = _bezierPatchC2;
                    }

                    else if (element is BezierPatchTubeC2 _bezierPatchTubeC2)
                    {
                        selectedBezierPatchTubeC2 = _bezierPatchTubeC2;
                    }


                    SelectedElements.Add(element);
                    element.IsSelected = true;

                    if (element is BezierC0 bezierC0)
                    {
                        bezierListBox.Items.Clear();
                        foreach (var el in bezierC0.bezierPoints)
                        {
                            bezierListBox.Items.Add(el);
                        }
                    }
                    break;

                case DrawingStatus.Deselect:
                    element = (Element)elementsOnScene.SelectedItem;
                    DeselectElement(element);
                    break;

                case DrawingStatus.Remove:
                    element = (Element)elementsOnScene.SelectedItem;
                    if (element is Torus _torus && _torus == selectedTorus)
                    {
                        selectedTorus = null;
                    }
                    else if (element is BezierC0 _bezierC0 && _bezierC0 == selectedBezierC0)
                    {
                        selectedBezierC0 = null;
                        bezierC0DrawPolyline.Checked = false;
                        bezierListBox.Items.Clear();
                    }
                    else if (element is BezierPatchC0 _bezierPatchC0)
                    {
                        foreach (var x in _bezierPatchC0.bezierPoints)
                        {
                            SelectedElements.Remove(x);
                            elementsOnScene.Items.Remove(x);
                            Elements.Remove(x);
                        }
                    }

                    else if (element is BezierPatchTubeC0 _bezierPatchTubeC0)
                    {
                        foreach (var x in _bezierPatchTubeC0.bezierPoints)
                        {
                            SelectedElements.Remove(x);
                            elementsOnScene.Items.Remove(x);
                            Elements.Remove(x);
                        }
                    }

                    else if (element is BezierPatchC2 _bezierPatchC2)
                    {
                        foreach (var x in _bezierPatchC2.bezierPoints)
                        {
                            SelectedElements.Remove(x);
                            elementsOnScene.Items.Remove(x);
                            Elements.Remove(x);
                        }
                    }

                    else if (element is BezierPatchTubeC2 _bezierPatchTubeC2)
                    {
                        foreach (var x in _bezierPatchTubeC2.bezierPoints)
                        {
                            SelectedElements.Remove(x);
                            elementsOnScene.Items.Remove(x);
                            Elements.Remove(x);
                        }
                    }

                    SelectedElements.Remove(element);
                    elementsOnScene.Items.Remove(element);
                    Elements.Remove(element);
                    break;

                case DrawingStatus.Rename:
                    element = (Element)elementsOnScene.SelectedItem;
                    string[] newName = new string[1];
                    RenameElement renameElement = new RenameElement(newName);
                    renameElement.ShowDialog();
                    element.ElementName = newName[0];
                    drawingStatus = DrawingStatus.No;
                    UpdateDrawingStatus();
                    elementsOnScene.Items[elementsOnScene.SelectedIndex] = elementsOnScene.SelectedItem;

                    break;

                case DrawingStatus.BezierC0AddPoint:
                    element = (Element)elementsOnScene.SelectedItem;
                    if (selectedBezierC0 != null && element is ModelGeneration.Point point)
                    {
                        selectedBezierC0.bezierPoints.Add(point);
                        bezierListBox.Items.Add(point);
                        selectedBezierC0.RegenerateBezierC0();
                    }
                    break;

                case DrawingStatus.BezierC2AddPoint:
                    element = (Element)elementsOnScene.SelectedItem;
                    if (selectedBezierC2 != null && element is ModelGeneration.Point point2)
                    {
                        selectedBezierC2.deBoorePoints.Add(point2);
                        bezierC2ListBox.Items.Add(point2);
                        selectedBezierC2.RegenerateBezierC2();
                    }
                    break;

                case DrawingStatus.InterpolationBezierC2AddPoint:
                    element = (Element)elementsOnScene.SelectedItem;
                    if (selectedInterpolatedBezierC2 != null && element is ModelGeneration.Point point3)
                    {
                        selectedInterpolatedBezierC2.interpolatedBC2Points.Add(point3);
                        interpolationBezierC2ListBox.Items.Add(point3);
                        selectedInterpolatedBezierC2.RegenerateInterpolatedBezierC2();
                    }
                    break;
            }
        }

        private void deselectAll_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.No;
            foreach (var el in Elements)
            {
                DeselectElement(el);
                bezierListBox.Items.Clear();
            }

            if (selectedBezierPatchC0 != null)
            {
                foreach (var el in selectedBezierPatchC0.bezierPoints)
                {
                    DeselectElement(el);
                }
            }

            if (selectedBezierPatchTubeC0 != null)
            {
                foreach (var el in selectedBezierPatchTubeC0.bezierPoints)
                {
                    DeselectElement(el);
                }
            }

            if (selectedBezierPatchC2 != null)
            {
                foreach (var el in selectedBezierPatchC2.bezierPoints)
                {
                    DeselectElement(el);
                }
            }

            if (selectedBezierPatchTubeC2 != null)
            {
                foreach (var el in selectedBezierPatchTubeC2.bezierPoints)
                {
                    DeselectElement(el);
                }
            }

            UpdateDrawingStatus();
        }

        private void rename_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.Rename;
            UpdateDrawingStatus();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (coursorModeComboBox.Text)
            {
                case "Hidden":
                    coursor.CoursorMode = CoursorMode.Hidden;
                    setCoursorEnabled(false);
                    break;

                case "Auto":
                    coursor.CoursorMode = CoursorMode.Auto;
                    setCoursorEnabled(false);
                    break;

                case "Manual":
                    coursor.CoursorMode = CoursorMode.Manual;
                    setCoursorEnabled(true);
                    break;

            }
        }

        private void setCoursorEnabled(bool enabled)
        {
            coursorGlobalX.Enabled = enabled;
            coursorGlobalY.Enabled = enabled;
            coursorGlobalZ.Enabled = enabled;
            coursorScreenX.Enabled = enabled;
            coursorScreenY.Enabled = enabled;
        }

        private void rotationPointComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rotationPointComboBox.SelectedIndex)
            {
                case 0:
                    transformCenterLines.rotationPoint = RotationPoint.Center;
                    if (SelectedElements.Count > 0)
                    {
                        Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
                        foreach (var el in SelectedElements)
                        {
                            pos += el.CenterPosition + el.Translation + el.TemporaryTranslation;
                        }

                        transformCenterLines.rotationCenterPoint = pos / SelectedElements.Count;
                    }
                    break;
                case 1:
                    transformCenterLines.rotationPoint = RotationPoint.Coursor;
                    transformCenterLines.rotationCenterPoint = coursor.CoursorGloalPosition;
                    break;
                case 2:
                    transformCenterLines.rotationPoint = RotationPoint.ZeroPoint;
                    transformCenterLines.rotationCenterPoint = new Vector3(0.0f, 0.0f, 0.0f);
                    break;
            }
        }

        private void moveCameraCentre_CheckedChanged(object sender, EventArgs e)
        {
            if (moveCameraCentre.Checked)
            {
                isMovingCameraCentre = true;
            }
            else
            {
                isMovingCameraCentre = false;
            }
        }

        private void centerCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _camera.CameraCenterPoint = new Vector3(0.0f, 0.0f, 0.0f);
            moveCameraCentre.Checked = false;
        }

        private void coursorGlobalX_TextChanged(object sender, EventArgs e)
        {
            float res;
            if (float.TryParse(coursorGlobalX.Text, out res))
            {
                coursor.CoursorGloalPosition.X = res;
            }
        }

        private void coursorGlobalY_TextChanged(object sender, EventArgs e)
        {
            float res;
            if (float.TryParse(coursorGlobalY.Text, out res))
            {
                coursor.CoursorGloalPosition.Y = res;
            }
        }

        private void coursorGlobalZ_TextChanged(object sender, EventArgs e)
        {
            float res;
            if (float.TryParse(coursorGlobalZ.Text, out res))
            {
                coursor.CoursorGloalPosition.Z = res;
            }
        }

        private void coursorScreenX_TextChanged(object sender, EventArgs e)
        {
            int res;
            if (int.TryParse(coursorScreenX.Text, out res))
            {
                coursor.CoursorScreenPosition = (res, coursor.CoursorScreenPosition.Item2);
                coursor.ChangeCoursorScreenPosition = true;
            }
        }

        private void coursorScreenY_TextChanged(object sender, EventArgs e)
        {
            int res;
            if (int.TryParse(coursorScreenY.Text, out res))
            {
                coursor.CoursorScreenPosition = (coursor.CoursorScreenPosition.Item1, res);
                coursor.ChangeCoursorScreenPosition = true;
            }
        }

        private void ApplyRotTransScaleButton_Click(object sender, EventArgs e)
        {
            foreach (var el in SelectedElements)
            {
                if (el is BezierPatchC0 bc0)
                {
                    foreach (var p in bc0.bezierPoints)
                    {
                        p.Translation += p.TemporaryTranslation;
                        p.TemporaryTranslation = new Vector3();
                        p.ElementScale *= p.TempElementScale;
                        p.TempElementScale = 1.0f;

                        var currentCenterPos = new Vector3(new Vector4(p.CenterPosition, 1.0f) * TranslationMatrix.CreateTranslationMatrix(p.Translation) * TranslationMatrix.CreateTranslationMatrix(-transformCenterLines.rotationCenterPoint) * Matrix4.CreateFromQuaternion(p.TempRotationQuaternion) * TranslationMatrix.CreateTranslationMatrix(transformCenterLines.rotationCenterPoint));
                        p.Translation = currentCenterPos - p.CenterPosition;

                        p.RotationQuaternion = p.TempRotationQuaternion * p.RotationQuaternion;
                        p.ElementRotationX = 0.0f;
                        p.ElementRotationY = 0.0f;
                        p.ElementRotationZ = 0.0f;
                        p.TempRotationQuaternion = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.0f);
                    }
                }

                else if (el is BezierPatchTubeC0 bct0)
                {
                    foreach (var p in bct0.bezierPoints)
                    {
                        p.Translation += p.TemporaryTranslation;
                        p.TemporaryTranslation = new Vector3();
                        p.ElementScale *= p.TempElementScale;
                        p.TempElementScale = 1.0f;

                        var currentCenterPos = new Vector3(new Vector4(p.CenterPosition, 1.0f) * TranslationMatrix.CreateTranslationMatrix(p.Translation) * TranslationMatrix.CreateTranslationMatrix(-transformCenterLines.rotationCenterPoint) * Matrix4.CreateFromQuaternion(p.TempRotationQuaternion) * TranslationMatrix.CreateTranslationMatrix(transformCenterLines.rotationCenterPoint));
                        p.Translation = currentCenterPos - p.CenterPosition;

                        p.RotationQuaternion = p.TempRotationQuaternion * p.RotationQuaternion;
                        p.ElementRotationX = 0.0f;
                        p.ElementRotationY = 0.0f;
                        p.ElementRotationZ = 0.0f;
                        p.TempRotationQuaternion = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.0f);
                    }
                }

                else if (el is BezierPatchC2 bc2)
                {
                    foreach (var p in bc2.bezierPoints)
                    {
                        p.Translation += p.TemporaryTranslation;
                        p.TemporaryTranslation = new Vector3();
                        p.ElementScale *= p.TempElementScale;
                        p.TempElementScale = 1.0f;

                        var currentCenterPos = new Vector3(new Vector4(p.CenterPosition, 1.0f) * TranslationMatrix.CreateTranslationMatrix(p.Translation) * TranslationMatrix.CreateTranslationMatrix(-transformCenterLines.rotationCenterPoint) * Matrix4.CreateFromQuaternion(p.TempRotationQuaternion) * TranslationMatrix.CreateTranslationMatrix(transformCenterLines.rotationCenterPoint));
                        p.Translation = currentCenterPos - p.CenterPosition;

                        p.RotationQuaternion = p.TempRotationQuaternion * p.RotationQuaternion;
                        p.ElementRotationX = 0.0f;
                        p.ElementRotationY = 0.0f;
                        p.ElementRotationZ = 0.0f;
                        p.TempRotationQuaternion = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.0f);
                    }
                }

                else if (el is BezierPatchTubeC2 bct2)
                {
                    foreach (var p in bct2.bezierPoints)
                    {
                        p.Translation += p.TemporaryTranslation;
                        p.TemporaryTranslation = new Vector3();
                        p.ElementScale *= p.TempElementScale;
                        p.TempElementScale = 1.0f;

                        var currentCenterPos = new Vector3(new Vector4(p.CenterPosition, 1.0f) * TranslationMatrix.CreateTranslationMatrix(p.Translation) * TranslationMatrix.CreateTranslationMatrix(-transformCenterLines.rotationCenterPoint) * Matrix4.CreateFromQuaternion(p.TempRotationQuaternion) * TranslationMatrix.CreateTranslationMatrix(transformCenterLines.rotationCenterPoint));
                        p.Translation = currentCenterPos - p.CenterPosition;

                        p.RotationQuaternion = p.TempRotationQuaternion * p.RotationQuaternion;
                        p.ElementRotationX = 0.0f;
                        p.ElementRotationY = 0.0f;
                        p.ElementRotationZ = 0.0f;
                        p.TempRotationQuaternion = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.0f);
                    }
                }
                else
                {
                    el.Translation += el.TemporaryTranslation;
                    el.TemporaryTranslation = new Vector3();
                    el.ElementScale *= el.TempElementScale;
                    el.TempElementScale = 1.0f;

                    var currentCenterPos = new Vector3(new Vector4(el.CenterPosition, 1.0f) * TranslationMatrix.CreateTranslationMatrix(el.Translation) * TranslationMatrix.CreateTranslationMatrix(-transformCenterLines.rotationCenterPoint) * Matrix4.CreateFromQuaternion(el.TempRotationQuaternion) * TranslationMatrix.CreateTranslationMatrix(transformCenterLines.rotationCenterPoint));
                    el.Translation = currentCenterPos - el.CenterPosition;

                    el.RotationQuaternion = el.TempRotationQuaternion * el.RotationQuaternion;
                    el.ElementRotationX = 0.0f;
                    el.ElementRotationY = 0.0f;
                    el.ElementRotationZ = 0.0f;
                    el.TempRotationQuaternion = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.0f);
                }
            }

            translationX.Text = "0.0";
            translationY.Text = "0.0";
            translationZ.Text = "0.0";
            rotationX.Text = "0.0";
            rotationY.Text = "0.0";
            rotationZ.Text = "0.0";
            ScaleTextBox.Text = "1.0";
            elementScaleUpDown.Value = 10;
        }

        private void elementScaleUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedElements.Count > 0)
            {
                float tempScaleValue = (float)elementScaleUpDown.Value / 10;
                foreach (var el in SelectedElements)
                {
                    el.TempElementScale = tempScaleValue;
                }

                ScaleTextBox.Text = tempScaleValue.ToString();
            }
        }

        private void translationZ_TextChanged(object sender, EventArgs e)
        {
            if (SelectedElements.Count > 0)
            {
                float res;
                if (float.TryParse(translationZ.Text, out res))
                {
                    foreach (var el in SelectedElements)
                    {
                        if (el is BezierPatchC0 bc0)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bc0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.Z = p.Translation.Z + res;
                                }
                            }
                        }
                        else if (el is BezierPatchTubeC0 bct0)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bct0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.Z = p.Translation.Z + res;
                                }
                            }
                        }
                        else if (el is BezierPatchC2 bc2)
                        {
                            foreach (var p in bc2.bezierPoints)
                            {
                                p.Translation.Z = p.Translation.Z + res;
                            }
                        }
                        else if (el is BezierPatchTubeC2 bct2)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bct2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.Z = p.Translation.Z + res;
                                }
                            }
                        }
                        else
                        {
                            el.TemporaryTranslation.Z = res;
                        }
                    }
                }
            }
        }

        private void translationY_TextChanged(object sender, EventArgs e)
        {
            if (SelectedElements.Count > 0)
            {
                float res;
                if (float.TryParse(translationY.Text, out res))
                {
                    foreach (var el in SelectedElements)
                    {

                        if (el is BezierPatchC0 bc0)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bc0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.Y = p.Translation.Y + res;
                                }
                            }
                        }
                        else if (el is BezierPatchTubeC0 bct0)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bct0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.Y = p.Translation.Y + res;
                                }
                            }
                        }
                        else if (el is BezierPatchC2 bc2)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bc2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.Y = p.Translation.Y + res;
                                }
                            }
                        }
                        else if (el is BezierPatchTubeC2 bct2)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bct2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.Y = p.Translation.Y + res;
                                }
                            }
                        }
                        else
                        {
                            el.TemporaryTranslation.Y = res;
                        }
                    }
                }
            }
        }

        private void translationX_TextChanged(object sender, EventArgs e)
        {
            if (SelectedElements.Count > 0)
            {
                float res;
                if (float.TryParse(translationX.Text, out res))
                {
                    foreach (var el in SelectedElements)
                    {

                        if (el is BezierPatchC0 bc0)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bc0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.X = p.Translation.X + res;
                                }
                            }
                        }
                        else if (el is BezierPatchTubeC0 bct0)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bct0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.X = p.Translation.X + res;
                                }
                            }
                        }
                        else if (el is BezierPatchC2 bc2)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bc2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.X = p.Translation.X + res;
                                }
                            }
                        }
                        else if (el is BezierPatchTubeC2 bct2)
                        {
                            List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                            foreach (var p in bct2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.Translation.X = p.Translation.X + res;
                                }
                            }
                        }
                        else
                        {
                            el.TemporaryTranslation.X = res;
                        }
                    }
                }
            }
        }

        private void clearStatus_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.No;
            UpdateDrawingStatus();
        }

        private void rotationX_TextChanged(object sender, EventArgs e)
        {
            if (SelectedElements.Count > 0)
            {
                float res;
                if (float.TryParse(rotationX.Text, out res))
                {
                    foreach (var el in SelectedElements)
                    {
                        List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                        if (el is BezierPatchC0 bc0)
                        {
                            foreach (var p in bc0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationX = res;
                                }
                            }
                        }

                        else if (el is BezierPatchTubeC0 bct0)
                        {
                            foreach (var p in bct0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationX = res;
                                }
                            }
                        }

                        else if (el is BezierPatchC2 bc2)
                        {
                            foreach (var p in bc2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationX = res;
                                }
                            }
                        }

                        else if (el is BezierPatchTubeC2 bct2)
                        {
                            foreach (var p in bct2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationX = res;
                                }
                            }
                        }
                        else
                        {
                            el.ElementRotationX = res;
                        }
                    }
                }
            }
        }

        private void rotationY_TextChanged(object sender, EventArgs e)
        {
            if (SelectedElements.Count > 0)
            {
                float res;
                if (float.TryParse(rotationY.Text, out res))
                {
                    foreach (var el in SelectedElements)
                    {
                        List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                        if (el is BezierPatchC0 bc0)
                        {
                            foreach (var p in bc0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationY += res;
                                }
                            }
                        }

                        else if (el is BezierPatchTubeC0 bct0)
                        {
                            foreach (var p in bct0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationY += res;
                                }
                            }
                        }

                        else if (el is BezierPatchC2 bc2)
                        {
                            foreach (var p in bc2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationY += res;
                                }
                            }
                        }

                        else if (el is BezierPatchTubeC2 bct2)
                        {
                            foreach (var p in bct2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationY += res;
                                }
                            }
                        }
                        else
                        {
                            el.ElementRotationY = res;
                        }
                    }
                }
            }
        }

        private void rotationZ_TextChanged(object sender, EventArgs e)
        {
            if (SelectedElements.Count > 0)
            {
                float res;
                if (float.TryParse(rotationZ.Text, out res))
                {
                    foreach (var el in SelectedElements)
                    {
                        List<ModelGeneration.Point> mocedPoints = new List<ModelGeneration.Point>();
                        if (el is BezierPatchC0 bc0)
                        {
                            foreach (var p in bc0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationZ += res;
                                }
                            }
                        }

                        else if (el is BezierPatchTubeC0 bct0)
                        {
                            foreach (var p in bct0.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationZ += res;
                                }
                            }
                        }

                        else if (el is BezierPatchC2 bc2)
                        {
                            foreach (var p in bc2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationZ += res;
                                }
                            }
                        }

                        else if (el is BezierPatchTubeC2 bct2)
                        {
                            foreach (var p in bct2.bezierPoints)
                            {
                                if (!mocedPoints.Contains(p))
                                {
                                    mocedPoints.Add(p);
                                    p.ElementRotationZ += res;
                                }
                            }
                        }
                        else
                        {
                            el.ElementRotationZ = res;
                        }
                    }
                }
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bezierC0CreateNewPointButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.BezierC0NewPoint;
            UpdateDrawingStatus();
        }

        private void bezierC0AddPointButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.BezierC0AddPoint;
            UpdateDrawingStatus();
        }

        private void bezierC0RemovePointButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.BezierC0RemovePoint;
            UpdateDrawingStatus();
        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void bezierListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Element element;
            switch (drawingStatus)
            {
                case DrawingStatus.BezierC0RemovePoint:
                    element = (Element)bezierListBox.SelectedItem;
                    if (selectedBezierC0 != null && element is ModelGeneration.Point point)
                    {
                        selectedBezierC0.bezierPoints.Remove(point);
                        bezierListBox.Items.Remove(point);
                        selectedBezierC0.RegenerateBezierC0();
                    }

                    break;
            }
        }

        private void bezierC0DrawPolyline_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedBezierC0 != null)
            {
                if (bezierC0DrawPolyline.Checked)
                {
                    selectedBezierC0.DrawPolyline = true;
                }
                else
                {
                    selectedBezierC0.DrawPolyline = false;
                }
            }
        }

        private void bezierC2CreateNewPointButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.BezierC2NewPoint;
            UpdateDrawingStatus();
        }

        private void bezierC2AddNewPointButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.BezierC2AddPoint;
            UpdateDrawingStatus();
        }

        private void bezierC2RemovePointButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.BezierC2RemovePoint;
            UpdateDrawingStatus();
        }

        private void bezierC2DrawBSplineLine_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedBezierC2 != null)
            {
                if (bezierC2DrawBSplineLine.Checked)
                {
                    selectedBezierC2.DrawBSplineLine = true;
                }
                else
                {
                    selectedBezierC2.DrawBSplineLine = false;
                }
            }
        }

        private void bezierC2DrawBernsteinLine_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedBezierC2 != null)
            {
                if (bezierC2DrawBernsteinLine.Checked)
                {
                    selectedBezierC2.DrawBernsteinLine = true;
                }
                else
                {
                    selectedBezierC2.DrawBernsteinLine = false;
                }
            }
        }

        private void bezierC2BernsteinBasis_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedBezierC2 != null)
            {
                if (bezierC2BernsteinBasis.Checked)
                {
                    selectedBezierC2.UseBernsteinBasis = true;
                    drawingStatus = DrawingStatus.No;
                    foreach (var el in Elements)
                    {
                        DeselectElement(el);
                        bezierListBox.Items.Clear();
                    }

                    drawingStatus = DrawingStatus.Select;


                }
                else
                {
                    selectedBezierC2.UseBernsteinBasis = false;
                }
            }
        }

        private void deselectBernsteinPoints_Click(object sender, EventArgs e)
        {
            if (selectedBezierC2 != null)
            {
                if (bezierC2BernsteinBasis.Checked && selectedBezierC2.bernsteinPoints.Count > 0)
                {
                    foreach (var p in selectedBezierC2.bernsteinPoints)
                    {
                        p.IsSelected = false;
                    }
                }
            }
        }

        private void bezierC2ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Element element;
            switch (drawingStatus)
            {
                case DrawingStatus.BezierC2RemovePoint:
                    element = (Element)bezierC2ListBox.SelectedItem;
                    if (selectedBezierC2 != null && element is ModelGeneration.Point point)
                    {
                        selectedBezierC2.deBoorePoints.Remove(point);
                        bezierC2ListBox.Items.Remove(point);
                        selectedBezierC2.RegenerateBezierC2();
                    }

                    break;
            }
        }

        private void interpolationBezierC2CreateNewPointButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.InterpolationBezierC2NewPoint;
            UpdateDrawingStatus();
        }

        private void interpolationBezierC2AddNewPointButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.InterpolationBezierC2AddPoint;
            UpdateDrawingStatus();
        }

        private void interpolationBezierC2RemovePointButton_Click(object sender, EventArgs e)
        {
            drawingStatus = DrawingStatus.InterpolationBezierC2RemovePoint;
            UpdateDrawingStatus();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void interpolatedBezierC2DrawPolyline_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedInterpolatedBezierC2 != null)
            {
                if (interpolatedBezierC2DrawPolyline.Checked)
                {
                    selectedInterpolatedBezierC2.DrawPolyline = true;
                }
                else
                {
                    selectedInterpolatedBezierC2.DrawPolyline = false;
                }
            }
        }

        private void interpolationBezierC2ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Element element;
            switch (drawingStatus)
            {
                case DrawingStatus.InterpolationBezierC2RemovePoint:
                    element = (Element)interpolationBezierC2ListBox.SelectedItem;
                    if (selectedInterpolatedBezierC2 != null && element is ModelGeneration.Point point)
                    {
                        selectedInterpolatedBezierC2.interpolatedBC2Points.Remove(point);
                        interpolationBezierC2ListBox.Items.Remove(point);
                        selectedInterpolatedBezierC2.RegenerateInterpolatedBezierC2();
                    }

                    break;
            }
        }

        private void focalLengthTrackBar_Scroll(object sender, EventArgs e)
        {
            _camera.FocalLength = focalLengthTrackBar.Value / 10.0f;
            focalLengthTextBox.Text = _camera.FocalLength.ToString();
        }

        private void c0PatchDrawPolyline_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedBezierPatchC0 != null)
            {
                if (c0PatchDrawPolyline.Checked)
                {
                    selectedBezierPatchC0.DrawPolyline = true;
                }
                else
                {
                    selectedBezierPatchC0.DrawPolyline = false;
                }
            }
        }

        private void addBezierPatchTubeC0_Click(object sender, EventArgs e)
        {
            bezierPatchTubeC0Number += 10;
            float[] values = new float[5];
            BezierPatchTube bezierPatch = new BezierPatchTube(values);
            bezierPatch.ShowDialog();
            BezierPatchTubeC0 bezierPatcTubehC0 = new BezierPatchTubeC0(bezierPatchC0Number, _camera, glControl1.Width, glControl1.Height, values);
            checkBox1.Checked = false;
            bezierPatchTubeC0Number++;
            bezierPatcTubehC0.CreateGlElement(_shader, _patchShaderGeometry);
            elementsOnScene.Items.Add(bezierPatcTubehC0);
            Elements.Add(bezierPatcTubehC0);
            List<ModelGeneration.Point> points = new List<ModelGeneration.Point>();
            foreach (var p in bezierPatcTubehC0.bezierPoints)
            {
                if (!points.Contains(p))
                {
                    points.Add(p);
                    p.CreateGlElement(_shader);
                    elementsOnScene.Items.Add(p);
                    Elements.Add(p);
                }
            }

            drawingStatus = DrawingStatus.No;
            selectedBezierPatchTubeC0 = bezierPatcTubehC0;
            UpdateDrawingStatus();
        }


        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            if (selectedBezierPatchC0 != null)
            {
                selectedBezierPatchC0.SegmentsU = trackBar6.Value;
                textBox10.Text = selectedBezierPatchC0.SegmentsU.ToString();
                selectedBezierPatchC0.RegenerateBezierPatchC0();
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (selectedBezierPatchC0 != null)
            {
                selectedBezierPatchC0.SegmentsV = trackBar3.Value;
                textBox9.Text = selectedBezierPatchC0.SegmentsV.ToString();
                selectedBezierPatchC0.RegenerateBezierPatchC0();
            }
        }

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            if (selectedBezierPatchTubeC0 != null)
            {
                selectedBezierPatchTubeC0.SegmentsU = trackBar8.Value;
                textBox12.Text = selectedBezierPatchTubeC0.SegmentsU.ToString();
                selectedBezierPatchTubeC0.RegenerateBezierPatchC0();
            }
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            if (selectedBezierPatchTubeC0 != null)
            {
                selectedBezierPatchTubeC0.SegmentsV = trackBar7.Value;
                textBox11.Text = selectedBezierPatchTubeC0.SegmentsV.ToString();
                selectedBezierPatchTubeC0.RegenerateBezierPatchC0();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedBezierPatchTubeC0 != null)
            {
                if (checkBox1.Checked)
                {
                    selectedBezierPatchTubeC0.DrawPolyline = true;
                }
                else
                {
                    selectedBezierPatchTubeC0.DrawPolyline = false;
                }
            }
        }

        private void TEST_Click(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (selectedBezierPatchC2 != null)
            {
                selectedBezierPatchC2.SegmentsU = trackBar2.Value;
                textBox5.Text = selectedBezierPatchC2.SegmentsU.ToString();
                selectedBezierPatchC2.RegenerateBezierPatchC2();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (selectedBezierPatchC2 != null)
            {
                selectedBezierPatchC2.SegmentsV = trackBar1.Value;
                textBox1.Text = selectedBezierPatchC2.SegmentsV.ToString();
                selectedBezierPatchC2.RegenerateBezierPatchC2();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedBezierPatchC2 != null)
            {
                if (checkBox2.Checked)
                {
                    selectedBezierPatchC2.DrawPolyline = true;
                }
                else
                {
                    selectedBezierPatchC2.DrawPolyline = false;
                }
            }
        }

        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            if (selectedBezierPatchTubeC2 != null)
            {
                selectedBezierPatchTubeC2.SegmentsU = trackBar10.Value;
                textBox7.Text = selectedBezierPatchTubeC2.SegmentsU.ToString();
                selectedBezierPatchTubeC2.RegenerateBezierPatchTubeC2();
            }
        }

        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            if (selectedBezierPatchTubeC2 != null)
            {
                selectedBezierPatchTubeC2.SegmentsV = trackBar9.Value;
                textBox6.Text = selectedBezierPatchTubeC2.SegmentsV.ToString();
                selectedBezierPatchTubeC2.RegenerateBezierPatchTubeC2();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedBezierPatchTubeC2 != null)
            {
                if (checkBox3.Checked)
                {
                    selectedBezierPatchTubeC2.DrawPolyline = true;
                }
                else
                {
                    selectedBezierPatchTubeC2.DrawPolyline = false;
                }
            }
        }

        private void loadModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Elements.Clear();
            SelectedElements.Clear();
            elementsOnScene.Items.Clear();
            Elements.Add(xyzLines);
            Elements.Add(transformCenterLines);
            pointNumber = 0;
            torusNumber = 0;
            bezierC0Number = 0;
            bezierC2Number = 0;
            interpolatedBezierC2Number = 0;
            bezierPatchC0Number = 0;
            bezierPatchTubeC0Number = 0;
            bezierPatchC2Number = 0;
            bezierPatchTubeC2Number = 0;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.Title = "Select Xml File";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                string previous = "";
                List<ModelGeneration.Point> points = new List<ModelGeneration.Point>();
                List<ModelGeneration.Torus> toruses = new List<ModelGeneration.Torus>();
                List<ModelGeneration.BezierC0> bezierC0 = new List<ModelGeneration.BezierC0>();
                List<ModelGeneration.BezierC2> bezierC2 = new List<ModelGeneration.BezierC2>();
                List<ModelGeneration.InterpolatedBezierC2> interBezier = new List<ModelGeneration.InterpolatedBezierC2>();
                patchC0 = new List<ModelGeneration.BezierPatchC0>();
                List<ModelGeneration.BezierPatchC2> patchC2 = new List<ModelGeneration.BezierPatchC2>();
                List<ModelGeneration.BezierPatchTubeC0> patchTubeC0 = new List<ModelGeneration.BezierPatchTubeC0>();
                List<ModelGeneration.BezierPatchTubeC2> patchTubeC2 = new List<ModelGeneration.BezierPatchTubeC2>();

                using (XmlReader reader = XmlReader.Create(fileName, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "Point")
                                {
                                    previous = "Point";
                                    var pointName = reader.GetAttribute("Name");
                                    ModelGeneration.Point point = new ModelGeneration.Point();
                                    point._camera = _camera;
                                    point.FullName = pointName;
                                    point.pointNumber = pointNumber;
                                    pointNumber++;
                                    points.Add(point);
                                }
                                else if (reader.Name == "Position" && previous == "Point")
                                {
                                    ModelGeneration.Point pp = points.Last();
                                    var X = float.Parse(reader.GetAttribute("X"));
                                    var Y = float.Parse(reader.GetAttribute("Y"));
                                    var Z = float.Parse(reader.GetAttribute("Z"));
                                    pp.CenterPosition = new Vector3(X, Y, Z);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                using (XmlReader reader = XmlReader.Create(fileName, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "Torus")
                                {
                                    previous = "Torus";
                                    var torusName = reader.GetAttribute("Name");
                                    var MinorRadius = float.Parse(reader.GetAttribute("MinorRadius"));
                                    var MajorRadius = float.Parse(reader.GetAttribute("MajorRadius"));
                                    var MajorSegments = int.Parse(reader.GetAttribute("MajorSegments"));
                                    var MinorSegments = int.Parse(reader.GetAttribute("MinorSegments"));
                                    ModelGeneration.Torus torus = new ModelGeneration.Torus();
                                    torus.FullName = torusName;
                                    torus.torusNumber = torusNumber;
                                    torus.torusMajorDividions = MajorSegments;
                                    torus.torusMinorDividions = MinorSegments;
                                    torus.torus_r = MinorRadius;
                                    torus.torus_R = MajorRadius;
                                    torusNumber++;
                                    toruses.Add(torus);
                                }
                                else if (reader.Name == "Position" && previous == "Torus")
                                {
                                    ModelGeneration.Torus t = toruses.Last();
                                    var X = float.Parse(reader.GetAttribute("X"));
                                    var Y = float.Parse(reader.GetAttribute("Y"));
                                    var Z = float.Parse(reader.GetAttribute("Z"));
                                    t.CenterPosition = new Vector3(X, Y, Z);
                                }
                                else if (reader.Name == "Rotation" && previous == "Torus")
                                {
                                    ModelGeneration.Torus t = toruses.Last();
                                    var X = float.Parse(reader.GetAttribute("X"));
                                    var Y = float.Parse(reader.GetAttribute("Y"));
                                    var Z = float.Parse(reader.GetAttribute("Z"));
                                    var W = float.Parse(reader.GetAttribute("W"));
                                    t.RotationQuaternion = new Quaternion(X, Y, Z, W);
                                }
                                else if (reader.Name == "Scale" && previous == "Torus")
                                {
                                    ModelGeneration.Torus t = toruses.Last();
                                    var X = float.Parse(reader.GetAttribute("X"));
                                    //var Y = float.Parse(reader.GetAttribute("Y"));
                                    //var Z = float.Parse(reader.GetAttribute("Z"));
                                    t.ElementScale = X;
                                }
                                else if (reader.Name == "BezierC0")
                                {
                                    previous = "BezierC0";
                                    var name = reader.GetAttribute("Name");
                                    ModelGeneration.BezierC0 bezierC0_ = new BezierC0(bezierC0Number, _camera, glControl1.Width, glControl1.Height);
                                    bezierC0_.FullName = name;
                                    bezierC0Number++;
                                    bezierC0.Add(bezierC0_);
                                }
                                else if (reader.Name == "PointRef" && previous == "BezierC0")
                                {
                                    ModelGeneration.BezierC0 bezierC0point = bezierC0.Last();
                                    var pointRefName = reader.GetAttribute("Name");
                                    bezierC0point.bezierPoints.Add(points.Where(x => x.FullName == pointRefName).First());
                                }
                                else if (reader.Name == "BezierC2")
                                {
                                    previous = "BezierC2";
                                    var name = reader.GetAttribute("Name");
                                    ModelGeneration.BezierC2 bezierC2_ = new BezierC2(bezierC2Number, _camera, glControl1.Width, glControl1.Height);
                                    bezierC2_.FullName = name;
                                    bezierC2Number++;
                                    bezierC2.Add(bezierC2_);
                                }
                                else if (reader.Name == "PointRef" && previous == "BezierC2")
                                {
                                    ModelGeneration.BezierC2 bezierC2point = bezierC2.Last();
                                    var pointRefName = reader.GetAttribute("Name");
                                    bezierC2point.deBoorePoints.Add(points.Where(x => x.FullName == pointRefName).First());
                                }
                                else if (reader.Name == "BezierInter")
                                {
                                    previous = "BezierInter";
                                    var name = reader.GetAttribute("Name");
                                    ModelGeneration.InterpolatedBezierC2 interBezier_ = new InterpolatedBezierC2(interpolatedBezierC2Number, _camera, glControl1.Width, glControl1.Height);
                                    interBezier_.FullName = name;
                                    interpolatedBezierC2Number++;
                                    interBezier.Add(interBezier_);
                                }
                                else if (reader.Name == "PointRef" && previous == "BezierInter")
                                {
                                    ModelGeneration.InterpolatedBezierC2 interpolatedBezierPoint = interBezier.Last();
                                    var pointRefName = reader.GetAttribute("Name");
                                    interpolatedBezierPoint.interpolatedBC2Points.Add(points.Where(x => x.FullName == pointRefName).First());
                                }
                                else if (reader.Name == "PatchC0")
                                {
                                    previous = "PatchC0";
                                    var patchName = reader.GetAttribute("Name");
                                    var N = int.Parse(reader.GetAttribute("N"));
                                    var M = int.Parse(reader.GetAttribute("M"));
                                    var NSlices = int.Parse(reader.GetAttribute("NSlices"));
                                    var MSlices = int.Parse(reader.GetAttribute("MSlices"));
                                    ModelGeneration.BezierPatchC0 patch = new ModelGeneration.BezierPatchC0(bezierPatchC0Number, _camera, glControl1.Width, glControl1.Height);
                                    bezierPatchC0Number++;
                                    patch.splitA = M;
                                    patch.splitB = N;
                                    patch.SegmentsU = MSlices;
                                    patch.SegmentsV = NSlices;
                                    patch.FullName = patchName;
                                    patchC0.Add(patch);
                                }
                                else if (reader.Name == "PointRef" && previous == "PatchC0")
                                {
                                    ModelGeneration.BezierPatchC0 patch = patchC0.Last();
                                    var pointRefName = reader.GetAttribute("Name");
                                    patch.bezierPoints.Add(points.Where(x => x.FullName == pointRefName).First());
                                }
                                else if (reader.Name == "PatchC2")
                                {
                                    previous = "PatchC2";
                                    var patchName = reader.GetAttribute("Name");
                                    var N = int.Parse(reader.GetAttribute("N"));
                                    var M = int.Parse(reader.GetAttribute("M"));
                                    var NSlices = int.Parse(reader.GetAttribute("NSlices"));
                                    var MSlices = int.Parse(reader.GetAttribute("MSlices"));
                                    ModelGeneration.BezierPatchC2 patch = new ModelGeneration.BezierPatchC2(bezierPatchC2Number, _camera, glControl1.Width, glControl1.Height);
                                    bezierPatchC2Number++;
                                    patch.splitA = M;
                                    patch.splitB = N;
                                    patch.SegmentsU = MSlices;
                                    patch.SegmentsV = NSlices;
                                    patch.FullName = patchName;
                                    patchC2.Add(patch);
                                }
                                else if (reader.Name == "PointRef" && previous == "PatchC2")
                                {
                                    ModelGeneration.BezierPatchC2 patch = patchC2.Last();
                                    var pointRefName = reader.GetAttribute("Name");
                                    patch.bezierPoints.Add(points.Where(x => x.FullName == pointRefName).First());
                                }

                                Console.WriteLine("Start Element {0}", reader.Name);
                                break;
                            case XmlNodeType.Text:
                                Console.WriteLine("Text Node: {0}", reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                break;
                            default:
                                Console.WriteLine("Other node {0} with value {1}",
                                                reader.NodeType, reader.Value);
                                break;
                        }
                    }
                }

                foreach (var p in points)
                {
                    elementsOnScene.Items.Add(p);
                    p.CreateGlElement(_shader);
                }
                Elements.AddRange(points);

                foreach (var t in toruses)
                {
                    elementsOnScene.Items.Add(t);
                    t.CreateGlElement(_shader);
                }
                Elements.AddRange(toruses);

                foreach (var b in bezierC0)
                {
                    elementsOnScene.Items.Add(b);
                    b.CreateGlElement(_shader, _shaderGeometry);
                }
                Elements.AddRange(bezierC0);

                foreach (var b in bezierC2)
                {
                    elementsOnScene.Items.Add(b);
                    b.CreateGlElement(_shader, _shaderGeometry);
                }
                Elements.AddRange(bezierC2);

                foreach (var b in interBezier)
                {
                    elementsOnScene.Items.Add(b);
                    b.CreateGlElement(_shader, _shaderGeometry);
                }
                Elements.AddRange(interBezier);

                foreach (var p in patchC0)
                {
                    elementsOnScene.Items.Add(p);
                    p.CreateGlElement(_shader, _patchShaderGeometry, _patchC0Shader);
                }
                Elements.AddRange(patchC0);

                foreach (var p in patchC2)
                {
                    elementsOnScene.Items.Add(p);
                    p.CreateGlElement(_shader, _patchShaderGeometry);
                }
                Elements.AddRange(patchC2);

                pointNumber += points.Count;
                torusNumber += toruses.Count;
                bezierC0Number += bezierC0.Count;
                bezierC2Number += bezierC2.Count;
                interpolatedBezierC2Number += interBezier.Count;
                bezierPatchC0Number += patchC0.Count;
                bezierPatchTubeC0Number += patchC0.Count;
                bezierPatchC2Number += patchC2.Count;
                bezierPatchTubeC2Number += patchC2.Count;

                //AllPatches.DrillAndSaveAll(patchC0);

            }
        }

        bool checkIfHeightOk(float x, float y, float z, float R, float[,] topLayer, float height)
        {

            for (float _x = -R; _x <= R; _x += 1)
            {
                for (float _y = -R; _y <= R; _y += 1)
                {
                    if (_x + x >= 0 && _y + y >= 0 && _x + x < 300 && _y + y < 300)
                    {

                        float xa = x;
                        float ya = y;
                        float za = height + R;

                        float xb = x + _x;
                        float yb = y + _y;

                        float val = R * R - (xb - xa) * (xb - xa) - (yb - ya) * (yb - ya);
                        float zb = -(float)Math.Sqrt(val) + za;

                        if (topLayer[(int)(_x + x), (int)(_y + y)] > zb)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        float DeKastilio(float[] vert, float t, int degree)
        {
            for (int i = 0; i < degree; i++)
            {
                for (int j = 0; j < degree - i - 1; j++)
                {
                    vert[j] = (1 - t) * vert[j] + t * vert[j + 1];
                }
            }

            return vert[0];
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog.FileName = "Model";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (XmlWriter writer = XmlWriter.Create(saveFileDialog.FileName, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true }))
                {
                    writer.WriteStartElement("Scene");
                    foreach (var el in Elements)
                    {
                        if (el is ModelGeneration.Point point)
                        {
                            this.addPointFunction(writer, point);
                        }

                        if (el is ModelGeneration.Torus torus)
                        {
                            writer.WriteStartElement("Torus");
                            writer.WriteAttributeString("MinorRadius", torus.torus_r.ToString());
                            writer.WriteAttributeString("MajorRadius", torus.torus_R.ToString());
                            writer.WriteAttributeString("MajorSegments", torus.torusMajorDividions.ToString());
                            writer.WriteAttributeString("MinorSegments", torus.torusMinorDividions.ToString());
                            writer.WriteAttributeString("Name", torus.FullName);
                            writer.WriteStartElement("Position");
                            writer.WriteAttributeString("X", torus.Position().X.ToString());
                            writer.WriteAttributeString("Y", torus.Position().Y.ToString());
                            writer.WriteAttributeString("Z", torus.Position().Z.ToString());
                            writer.WriteStartElement("Rotation");
                            writer.WriteAttributeString("X", torus.RotationQuaternion.X.ToString());
                            writer.WriteAttributeString("Y", torus.RotationQuaternion.Y.ToString());
                            writer.WriteAttributeString("Z", torus.RotationQuaternion.Z.ToString());
                            writer.WriteAttributeString("W", torus.RotationQuaternion.W.ToString());
                            writer.WriteStartElement("Scale");
                            writer.WriteAttributeString("X", torus.ElementScale.ToString());
                            writer.WriteAttributeString("Y", torus.ElementScale.ToString());
                            writer.WriteAttributeString("Z", torus.ElementScale.ToString());
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }

                        if (el is ModelGeneration.BezierC0 bezierC0)
                        {
                            writer.WriteStartElement("BezierC0");
                            writer.WriteAttributeString("Name", bezierC0.FullName);
                            writer.WriteStartElement("Points");
                            foreach (var p in bezierC0.bezierPoints)
                            {
                                this.addPointRef(writer, p);
                            }

                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }

                        if (el is ModelGeneration.BezierC2 bezierC2)
                        {
                            writer.WriteStartElement("BezierC2");
                            writer.WriteAttributeString("Name", bezierC2.FullName);
                            writer.WriteStartElement("Points");
                            foreach (var p in bezierC2.deBoorePoints)
                            {
                                this.addPointRef(writer, p);
                            }

                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }

                        if (el is ModelGeneration.InterpolatedBezierC2 interpolatedbezierC0)
                        {
                            writer.WriteStartElement("BezierInter");
                            writer.WriteAttributeString("Name", interpolatedbezierC0.FullName);
                            writer.WriteStartElement("Points");
                            foreach (var p in interpolatedbezierC0.interpolatedBC2Points)
                            {
                                this.addPointRef(writer, p);
                            }

                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }

                        if (el is ModelGeneration.BezierPatchC0 bezierPatchC0)
                        {
                            writer.WriteStartElement("PatchC0");
                            writer.WriteAttributeString("Name", bezierPatchC0.FullName);
                            writer.WriteAttributeString("M", bezierPatchC0.splitA.ToString());
                            writer.WriteAttributeString("N", bezierPatchC0.splitB.ToString());
                            writer.WriteAttributeString("MSlices", bezierPatchC0.SegmentsU.ToString());
                            writer.WriteAttributeString("NSlices", bezierPatchC0.SegmentsV.ToString());
                            writer.WriteStartElement("Points");
                            foreach (var pp in bezierPatchC0.bezierPoints)
                            {
                                this.addPointRef(writer, pp);
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }


                        if (el is ModelGeneration.BezierPatchTubeC0 bezierPatchTubeC0)
                        {
                            writer.WriteStartElement("PatchC0");
                            writer.WriteAttributeString("Name", bezierPatchTubeC0.FullName);
                            writer.WriteAttributeString("M", bezierPatchTubeC0.splitA.ToString());
                            writer.WriteAttributeString("N", bezierPatchTubeC0.splitB.ToString());
                            writer.WriteAttributeString("MSlices", bezierPatchTubeC0.SegmentsU.ToString());
                            writer.WriteAttributeString("NSlices", bezierPatchTubeC0.SegmentsV.ToString());
                            writer.WriteStartElement("Points");
                            foreach (var pp in bezierPatchTubeC0.bezierPoints)
                            {
                                this.addPointRef(writer, pp);
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }

                        if (el is ModelGeneration.BezierPatchC2 bezierPatchC2)
                        {
                            writer.WriteStartElement("PatchC2");
                            writer.WriteAttributeString("Name", bezierPatchC2.FullName);
                            writer.WriteAttributeString("M", bezierPatchC2.splitA.ToString());
                            writer.WriteAttributeString("N", bezierPatchC2.splitB.ToString());
                            writer.WriteAttributeString("MSlices", bezierPatchC2.SegmentsU.ToString());
                            writer.WriteAttributeString("NSlices", bezierPatchC2.SegmentsV.ToString());
                            writer.WriteStartElement("Points");
                            foreach (var pp in bezierPatchC2.bezierPoints)
                            {
                                this.addPointRef(writer, pp);
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }


                        if (el is ModelGeneration.BezierPatchTubeC2 bezierPatchTubeC2)
                        {
                            writer.WriteStartElement("PatchC2");
                            writer.WriteAttributeString("Name", bezierPatchTubeC2.FullName);
                            writer.WriteAttributeString("M", bezierPatchTubeC2.splitA.ToString());
                            writer.WriteAttributeString("N", bezierPatchTubeC2.splitB.ToString());
                            writer.WriteAttributeString("MSlices", bezierPatchTubeC2.SegmentsU.ToString());
                            writer.WriteAttributeString("NSlices", bezierPatchTubeC2.SegmentsV.ToString());
                            writer.WriteStartElement("Points");
                            foreach (var pp in bezierPatchTubeC2.bezierPoints)
                            {
                                this.addPointRef(writer, pp);
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteEndElement();
                }
            }
        }
        private void addPointRef(XmlWriter writer, ModelGeneration.Point point)
        {
            writer.WriteStartElement("PointRef");
            writer.WriteAttributeString("Name", point.FullName);
            writer.WriteEndElement();
        }

        private void addPointFunction(XmlWriter writer, ModelGeneration.Point point)
        {
            writer.WriteStartElement("Point");
            writer.WriteAttributeString("Name", point.FullName);
            writer.WriteStartElement("Position");
            writer.WriteAttributeString("X", point.Position().X.ToString());
            writer.WriteAttributeString("Y", point.Position().Y.ToString());
            writer.WriteAttributeString("Z", point.Position().Z.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<ModelGeneration.Point> selectedPoints = new List<ModelGeneration.Point>();
            foreach (var el in SelectedElements)
            {
                if (el is ModelGeneration.Point p)
                {
                    selectedPoints.Add(p);
                }
            }

            if (selectedPoints.Count == 2)
            {
                Vector3 pos = selectedPoints[0].Position() + selectedPoints[1].Position();
                selectedPoints[0].CenterPosition = new Vector3(pos.X / 2, pos.Y / 2, pos.Z / 2);
                selectedPoints[0].Translation = new Vector3(0, 0, 0);
                selectedPoints[0].TemporaryTranslation = new Vector3(0, 0, 0);

                ModelGeneration.Point searchingPoint = selectedPoints[1];

                foreach (var ele in Elements)
                {
                    if (ele is BezierPatchC0 bpc0)
                    {
                        ReplacePoint(bpc0.bezierPoints, searchingPoint, selectedPoints[0]);
                    }

                    if (ele is BezierPatchC2 bpc2)
                    {
                        ReplacePoint(bpc2.bezierPoints, searchingPoint, selectedPoints[0]);
                    }

                    if (ele is BezierPatchTubeC0 bptc0)
                    {
                        ReplacePoint(bptc0.bezierPoints, searchingPoint, selectedPoints[0]);
                    }

                    if (ele is BezierPatchTubeC2 bptc2)
                    {
                        ReplacePoint(bptc2.bezierPoints, searchingPoint, selectedPoints[0]);
                    }
                }

                Elements.Remove(searchingPoint);
                SelectedElements.Remove(searchingPoint);
            }
        }

        private void ReplacePoint(List<ModelGeneration.Point> pointList, ModelGeneration.Point point, ModelGeneration.Point newPoint)
        {
            if (pointList.Contains(point))
            {
                var idx = pointList.FindIndex(x => x == point);
                pointList.RemoveAt(idx);
                pointList.Insert(idx, newPoint);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var el in SelectedElements)
            {
                if (el is ModelGeneration.Point pp)
                {
                    pp.CenterPosition = coursor.CoursorGloalPosition;
                    pp.Translation = new Vector3(0, 0, 0);
                    pp.TemporaryTranslation = new Vector3(0, 0, 0);
                }
            }
        }

        private void createPathsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (patchC0 != null && patchC0.Count > 0)
            {
                string path = "C://Users//User//Documents//New folder";
                DrillFat.DrillAndSave(patchC0, path);
                DrillRound.DrillAndSave(patchC0, path);
                DrillAcurate.DrillAndSave(patchC0, path);
                DrillSign.DrillAndSave(path);
            }
        }

        private void addGregory_Click(object sender, EventArgs e)
        {
            CreateGregoryClass gregoryCreator = new CreateGregoryClass();
            List<Gregory> holes = gregoryCreator.CreateGregory(1, _camera, glControl1.Width, glControl1.Height, Elements, SelectedElements);
            Elements.AddRange(holes);
            foreach (var h in holes)
            {
                h.CreateGlElement(_shader, _patchShaderGeometry, _gregoryShader);
                Elements.Add(h);
                elementsOnScene.Items.Add(h);

                h.gp1.CreateGlElement(_shader, _patchShaderGeometry, _gregoryShader);
                Elements.Add(h.gp1);
                elementsOnScene.Items.Add(h.gp1);

                h.gp2.CreateGlElement(_shader, _patchShaderGeometry, _gregoryShader);
                Elements.Add(h.gp2);
                elementsOnScene.Items.Add(h.gp2);

                h.gp3.CreateGlElement(_shader, _patchShaderGeometry, _gregoryShader);
                Elements.Add(h.gp3);
                elementsOnScene.Items.Add(h.gp3);
            }
        }

        private void addBezierPatchC2_Click(object sender, EventArgs e)
        {
            float[] values = new float[4];
            BezierPatch bezierPatch = new BezierPatch(values);
            bezierPatch.ShowDialog();
            BezierPatchC2 bezierPatchC2 = new BezierPatchC2(bezierPatchC0Number, _camera, glControl1.Width, glControl1.Height, values);
            checkBox2.Checked = false;
            bezierPatchC2Number++;
            bezierPatchC2.CreateGlElement(_shader, _patchShaderGeometry);
            elementsOnScene.Items.Add(bezierPatchC2);
            Elements.Add(bezierPatchC2);
            foreach (var p in bezierPatchC2.bezierPoints)
            {
                p.CreateGlElement(_shader);
                elementsOnScene.Items.Add(p);
                Elements.Add(p);
            }

            drawingStatus = DrawingStatus.No;
            selectedBezierPatchC2 = bezierPatchC2;
            UpdateDrawingStatus();
        }

        private void addBezierPatchTubeC2_Click(object sender, EventArgs e)
        {
            float[] values = new float[5];
            BezierPatchTube bezierPatch = new BezierPatchTube(values);
            bezierPatch.ShowDialog();
            BezierPatchTubeC2 bezierPatcTubehC2 = new BezierPatchTubeC2(bezierPatchC2Number, _camera, glControl1.Width, glControl1.Height, values);
            checkBox3.Checked = false;
            bezierPatchTubeC2Number++;
            bezierPatcTubehC2.CreateGlElement(_shader, _patchShaderGeometry);
            elementsOnScene.Items.Add(bezierPatcTubehC2);
            Elements.Add(bezierPatcTubehC2);
            List<ModelGeneration.Point> points = new List<ModelGeneration.Point>();
            foreach (var p in bezierPatcTubehC2.bezierPoints)
            {
                if (!points.Contains(p))
                {
                    points.Add(p);
                    p.CreateGlElement(_shader);
                    elementsOnScene.Items.Add(p);
                    Elements.Add(p);
                }
            }

            drawingStatus = DrawingStatus.No;
            selectedBezierPatchTubeC2 = bezierPatcTubehC2;
            UpdateDrawingStatus();
        }

        private void eyeDistTrackBar_Scroll(object sender, EventArgs e)
        {
            _camera.EyeSeparation = eyeDistTrackBar.Value / 10.0f;
            eyeDistTextBox.Text = _camera.EyeSeparation.ToString();
        }

        private void addBEzierPatchC0_Click(object sender, EventArgs e)
        {
            float[] values = new float[4];
            BezierPatch bezierPatch = new BezierPatch(values);
            bezierPatch.ShowDialog();
            BezierPatchC0 bezierPatchC0 = new BezierPatchC0(bezierPatchC0Number, _camera, glControl1.Width, glControl1.Height, values);
            bezierC0DrawPolyline.Checked = false;
            bezierPatchC0Number++;
            bezierPatchC0.CreateGlElement(_shader, _patchShaderGeometry, _patchC0Shader);
            elementsOnScene.Items.Add(bezierPatchC0);
            Elements.Add(bezierPatchC0);
            foreach (var p in bezierPatchC0.bezierPoints)
            {
                p.CreateGlElement(_shader);
                elementsOnScene.Items.Add(p);
                Elements.Add(p);
            }

            drawingStatus = DrawingStatus.No;
            selectedBezierPatchC0 = bezierPatchC0;
            UpdateDrawingStatus();
        }

        private void anaglyph_CheckedChanged(object sender, EventArgs e)
        {
            if (anaglyph.Checked)
            {
                anaglyphOn = true;
            }
            else
            {
                anaglyphOn = false;
            }
        }

        private void addInterpolatedBezierC2_Click(object sender, EventArgs e)
        {
            InterpolatedBezierC2 interpolatedBezierC2 = new InterpolatedBezierC2(interpolatedBezierC2Number, _camera, glControl1.Width, glControl1.Height);
            interpolatedBezierC2DrawPolyline.Checked = false;
            interpolatedBezierC2Number++;
            interpolatedBezierC2.CreateGlElement(_shader, _shaderGeometry);
            interpolationBezierC2ListBox.Items.Clear();
            elementsOnScene.Items.Add(interpolatedBezierC2);
            Elements.Add(interpolatedBezierC2);
            if (SelectedElements.Count > 0)
            {
                foreach (var el in SelectedElements)
                {
                    if (!(el is ModelGeneration.Point))
                    {
                        goto miss;
                    }
                }
                foreach (var el in SelectedElements)
                {
                    ModelGeneration.Point p = el as ModelGeneration.Point;
                    interpolatedBezierC2.interpolatedBC2Points.Add(p);
                    interpolationBezierC2ListBox.Items.Add(p);
                }

            }
        miss:;
            drawingStatus = DrawingStatus.No;
            selectedInterpolatedBezierC2 = interpolatedBezierC2;
            UpdateDrawingStatus();
        }

        private void addBezierC2_Click(object sender, EventArgs e)
        {
            BezierC2 bezierC2 = new BezierC2(bezierC2Number, _camera, glControl1.Width, glControl1.Height);
            bezierC2BernsteinBasis.Checked = false;
            bezierC2DrawBernsteinLine.Checked = false;
            bezierC2DrawBSplineLine.Checked = false;
            bezierC2Number++;
            bezierC2.CreateGlElement(_shader, _shaderGeometry);
            bezierC2ListBox.Items.Clear();
            elementsOnScene.Items.Add(bezierC2);
            Elements.Add(bezierC2);
            if (SelectedElements.Count > 0)
            {
                foreach (var el in SelectedElements)
                {
                    if (!(el is ModelGeneration.Point))
                    {
                        goto miss;
                    }
                }
                foreach (var el in SelectedElements)
                {
                    ModelGeneration.Point p = el as ModelGeneration.Point;
                    bezierC2.deBoorePoints.Add(p);
                    bezierC2ListBox.Items.Add(p);
                }

            }
        miss:;
            drawingStatus = DrawingStatus.No;
            selectedBezierC2 = bezierC2;
            UpdateDrawingStatus();
        }

        private void addBezierC0_Click(object sender, EventArgs e)
        {
            BezierC0 bezierC0 = new BezierC0(bezierC0Number, _camera, glControl1.Width, glControl1.Height);
            bezierC0DrawPolyline.Checked = false;
            bezierC0Number++;
            bezierC0.CreateGlElement(_shader, _shaderGeometry);
            bezierListBox.Items.Clear();
            elementsOnScene.Items.Add(bezierC0);
            Elements.Add(bezierC0);
            if (SelectedElements.Count > 0)
            {
                foreach (var el in SelectedElements)
                {
                    if (!(el is ModelGeneration.Point))
                    {
                        goto miss;
                    }
                }
                foreach (var el in SelectedElements)
                {
                    ModelGeneration.Point p = el as ModelGeneration.Point;
                    bezierC0.bezierPoints.Add(p);
                    bezierListBox.Items.Add(p);
                }

            }
        miss:;
            drawingStatus = DrawingStatus.No;
            selectedBezierC0 = bezierC0;
            UpdateDrawingStatus();
        }

        private void UpdateDrawingStatus()
        {
            drawStatusLabel.Text = drawingStatus.ToString();
            if (drawingStatus == DrawingStatus.BezierC0AddPoint || drawingStatus == DrawingStatus.BezierC0NewPoint || drawingStatus == DrawingStatus.BezierC0RemovePoint)
            {
                bezierC0Status.Text = drawingStatus.ToString();
            }
            else
            {
                bezierC0Status.Text = "NO";
            }
        }

        private void DeselectElement(Element el)
        {
            el.IsSelected = false;
            el.TemporaryTranslation.X = 0;
            el.TemporaryTranslation.Y = 0;
            el.TemporaryTranslation.Z = 0;
            el.TempElementScale = 1.0f;
            SelectedElements.Remove(el);
        }

    }
}
