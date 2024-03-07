using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor
{
    internal static class DrawMode
    {
       
        private static List<PointF> newPolygon = new List<PointF>();
        private static List<Edge> newEdges = new List<Edge>();
        private static PointF newPoint = new PointF();

        public static void colorButton_Click(object sender, EventArgs e)
        {
            if (newPolygon.Count > 0)
                newEdges.Last().edgeColor = Manager.currentColor;
        }

        #region workSpace events
        public static void workSpace_MouseDown(object sender, MouseEventArgs e)
        { 
            if (newPolygon.Count != 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (newPolygon.Count > 2)
                    {
                        Manager.polygons.Add(newPolygon.ToList());
                        Manager.edges.Add(newEdges.ToList());

                        Manager.offsets.Add(new List<PointF>());
                        Manager.notFixed.Add(new List<PointF>());
                        Manager.offsetsValue.Add(0);
                    }
                    newPolygon.Clear();
                    newEdges.Clear();
                }
                else
                {
                    if (Manager.isNearbyPoint(e.Location, newPolygon.First())
                            && newPolygon.Count > 2)
                    {
                        Manager.polygons.Add(newPolygon.ToList());
                        Manager.edges.Add(newEdges.ToList());

                        Manager.offsets.Add(new List<PointF>());
                        Manager.notFixed.Add(new List<PointF>());
                        Manager.offsetsValue.Add(0);

                        newEdges.Clear();
                        newPolygon.Clear();
                    }
                    else
                    {
                        if (e.Location != newPolygon.Last())
                        {
                            newPolygon.Add(e.Location);
                            newEdges.Add(new Edge());
                            newEdges.Last().edgeColor = Manager.currentColor;
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                newPolygon = new List<PointF>();
                newEdges = new List<Edge>();
                newPoint = e.Location;
                newPolygon.Add(newPoint);
                newEdges.Add(new Edge());
                newEdges.Last().edgeColor = Manager.currentColor;
            }

            ((PictureBox)sender).Invalidate();

        }

        public static void workSpace_MouseMove(object sender, MouseEventArgs e)
        {
            if (newPolygon.Count != 0)
            {
                newPoint = e.Location;
                ((PictureBox)sender).Invalidate();
            }
        }

        private static void drawNewPolygon(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int j = 0; j < newPolygon.Count - 1; j++)
            {
                if (Manager.drawSet == DrawSet.Lib)
                {
                    Manager.p.Color = newEdges[j].edgeColor;
                    g.DrawLine(Manager.p, newPolygon[j], newPolygon[j + 1]);
                }
                else
                    Manager.bresenhamDrawLine(g, newEdges[j].edgeColor, newPolygon[j], newPolygon[j + 1]);

                Manager.drawVertex(g, newPolygon[j]);
            }

            Manager.drawVertex(g, newPolygon.Last());
            if (Manager.drawSet == DrawSet.Lib)
            {
                Manager.dp.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };
                Manager.dp.Color = Manager.currentColor;
                g.DrawLine(Manager.dp, newPolygon.Last(), newPoint);
            }
            else
                Manager.bresenhamDrawLine(g, Manager.currentColor, newPolygon.Last(), newPoint);
        }
        public static void workSpace_Paint(object sender, PaintEventArgs e)
        {
            Manager.drawPolygons(sender, e);

            if (newPolygon.Count != 0)
                drawNewPolygon(e);

        }

        public static void workSpace_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Invalidate();
        }

        #endregion

        public static void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) 
            {
                newPolygon.Clear();
                newEdges.Clear();
            }
        }
    }
}
