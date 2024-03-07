using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

public enum Move
{
    None,
    Vertex,
    Edge,
    Polygon,
    Offset
}

public enum Direction
{
    Any,
    Horizontal,
    Vertical
}

namespace PolygonEditor
{
    internal static class EditMode
    {

        private static Point startPoint = new Point(-1, -1);
        private static int vToMove = -1, e1ToMove = -1, e2ToMove = -1, pToMove = -1;
        private static int pToAddRel = -1, etoAddRel = -1;

        public static ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
        public static ToolStripMenuItem horizontalMenuItem = new ToolStripMenuItem();
        public static ToolStripMenuItem verticalMenuItem = new ToolStripMenuItem();

        public static Move move = Move.None;

        public static int currentOffset = 0;

        #region findPolygon
        private static int pFindPolygon(Point p)
        {
            int index;
            for (index = 0; index < Manager.polygons.Count; index++)
                if (Manager.isInPolygon(p, Manager.polygons[index]))
                    return index;
            return -1;
        }

        #endregion

        #region isNearby

        // ((index of polygon, index of vertex), near vertex)
        private static ((int, int), PointF) isNearbyVertex(Point eP, bool toRemove)
        {
            for (int index = 0; index < Manager.polygons.Count; index++)
                if (!toRemove || Manager.polygons[index].Count > 3)
                    for (int i = 0; i < Manager.polygons[index].Count; i++)
                        if (Manager.isNearbyPoint(Manager.polygons[index][i], eP))
                            return ((index, i), Manager.polygons[index][i]);
            return ((-1, -1), new Point(-1, -1));
        }

        private static ((int, (int, int)), (PointF, PointF)) isNearbyEdge(Point eP)
        {
            for (int index = 0; index < Manager.polygons.Count; index++)
            {
                List<PointF> polygon = Manager.polygons[index];
                for (int i = 0, j = 1; j < polygon.Count; i++, j++)
                {
                    if (Manager.isNearbySegment(eP, polygon[i], polygon[j]))
                        return ((index, (i, j)), (polygon[i], polygon[j]));
                }
                if (Manager.isNearbySegment(eP, polygon.Last(), polygon.First()))
                    return ((index, (polygon.Count - 1, 0)), (polygon.Last(), polygon.First()));
            }
            return ((-1, (-1, -1)), (new Point(-1, -1), new Point(-1, -1)));
        }

        #endregion

        #region updateRel

        private static void updateRel(int i, int j)
        {
            if (Manager.edges[i][j].direction == Direction.Horizontal)
                Manager.polygons[i][(j + 1) % Manager.polygons[i].Count]
                    = new PointF(Manager.polygons[i][(j + 1) % Manager.polygons[i].Count].X, 
                        Manager.polygons[i][j].Y);
            if (Manager.edges[i][j].direction == Direction.Vertical)
                Manager.polygons[i][(j + 1) % Manager.polygons[i].Count]
                    = new PointF(Manager.polygons[i][j].X, 
                        Manager.polygons[i][(j + 1) % Manager.polygons[i].Count].Y);

            int prev = j - 1 == -1 ? Manager.polygons[i].Count - 1 : j - 1;
            if (Manager.edges[i][prev].direction == Direction.Horizontal)
                Manager.polygons[i][prev]
                    = new PointF(Manager.polygons[i][prev].X, Manager.polygons[i][j].Y);
            if (Manager.edges[i][prev].direction == Direction.Vertical)
                Manager.polygons[i][prev]
                    = new PointF(Manager.polygons[i][j].X, Manager.polygons[i][prev].Y);

            updateOffset(i, Manager.offsetsValue[i]);
        }

        private static void deleteRel(int i, int j)
        {
            int prev = j - 1 == -1 ? Manager.polygons[i].Count - 1 : j - 1;
            Manager.edges[i][prev].direction = Direction.Any;
            Manager.edges[i][(j + 1) % Manager.polygons[i].Count].direction = Direction.Any;

            updateOffset(i, Manager.offsetsValue[i]);
        }

        #endregion

        #region updateOffset

        public static void updateOffset(int index,float offsetValue)
        {
            if (index != -1)
            {
                List<PointF> offset = findOffsetPolygon(Manager.polygons[index], offsetValue,
                    out List<PointF> points);
                Manager.offsets[index] = offset;
                Manager.notFixed[index] = points;
                Manager.offsetsValue[index] = offsetValue;
            }
        }

        #endregion

        #region offset

        #region polygon orientation

        // http://www.csharphelper.com/howtos/howto_polygon_area.html
        // Return the polygon's area in "square units."
        // The value will be negative if the polygon is
        // oriented clockwise.
        private static float SignedPolygonArea(List<PointF> Points)
        {
            // Add the first point to the end.
            int num_points = Points.Count;
            PointF[] pts = new PointF[num_points + 1];
            Points.CopyTo(pts, 0);
            pts[num_points] = Points[0];

            // Get the areas.
            float area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area +=
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2;
            }

            // Return the result.
            return area;
        }

        //http://www.csharphelper.com/howtos/howto_polygon_orientation.html
        // Return true if the polygon is oriented clockwise.
        public static bool PolygonIsOrientedClockwise(List<PointF> Points)
        {
            return (SignedPolygonArea(Points) < 0);
        }

        #endregion

        // http://www.csharphelper.com/howtos/howto_segment_intersection.html
        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        private static void FindIntersection(
            PointF p1, PointF p2, PointF p3, PointF p4,
            out bool lines_intersect, out bool segments_intersect,
            out PointF intersection,
            out PointF close_p1, out PointF close_p2)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new PointF(float.NaN, float.NaN);
                close_p1 = new PointF(float.NaN, float.NaN);
                close_p2 = new PointF(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new PointF(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        private static List<PointF> fixSelfIntesections(List<PointF> points)
        {
            int numPoints = points.Count;
            List<PointF> fixedPoints = new List<PointF>();
            bool inPolygon = false, ifIntersect = false;
            PointF enter = new PointF(-1, -1), exit = new PointF(-1, -1),
                intersection = new PointF(-1, -1);
            int removeStart = -1, addStart = -1, addEnd;
            for (int i = 0; i < numPoints; i++)
            {
                if (!inPolygon)
                    fixedPoints.Add(points[i]);

                if (inPolygon && i != ((removeStart + 1) % numPoints))
                {
                    FindIntersection(points[i], points[(i + 1) % numPoints],
                            points[removeStart], points[(removeStart + 1) % numPoints],
                            out bool _, out ifIntersect, out intersection,
                            out PointF _, out PointF _);

                    if (ifIntersect)
                    {
                        inPolygon = false;
                        //addEnd = removeStart;
                        //exit = new PointF(intersection.X, intersection.Y);

                        //for (int k = addStart; k > addEnd; k--)
                        //    fixedPoints.Add(points[k]);
                        //fixedPoints.Add(exit);
                    }
                }

                for (int j = numPoints - 1; j > i + 1; j--)
                {

                    if (i == 0 && j == numPoints - 1)
                        continue;

                    FindIntersection(points[i], points[(i + 1) % numPoints],
                            points[j], points[(j + 1) % numPoints],
                            out bool _, out ifIntersect, out intersection,
                            out PointF _, out PointF _);

                    if (ifIntersect)
                    {
                        if (!inPolygon)
                        {
                            inPolygon = true;

                            removeStart = i;
                            addStart = j;
                            enter = new PointF(intersection.X, intersection.Y);

                            fixedPoints.Add(enter);

                        }

                        else
                        {
                            inPolygon = false;
                            addEnd = j;
                            exit = new PointF(intersection.X, intersection.Y);

                            for (int k = addStart; k > addEnd; k--)
                                fixedPoints.Add(points[k]);
                            fixedPoints.Add(exit);
                        }
                    }
                }
            }
            
            return fixedPoints;
        }

        private static List<PointF> findOffsetPolygon(List<PointF> polygon, float offset, out List<PointF> points)
        {
            if (PolygonIsOrientedClockwise(polygon))
                offset *= (-1);

            List<PointF> offsetPoints = new List<PointF>();

            for (int j = 0; j < polygon.Count; j++)
            {
                int i = j - 1 == -1 ? polygon.Count - 1 : j - 1;
                int k = (j + 1) % polygon.Count;

                (PointF pij1, PointF pij2) = Manager.findParallelSegment(polygon[i], polygon[j], offset);
                (PointF pjk1, PointF pjk2) = Manager.findParallelSegment(polygon[j], polygon[k], offset);

                // Find the intersection of the shifted lines ij and jk.
                FindIntersection(pij1, pij2, pjk1, pjk2, out bool _, out bool _,
                    out PointF intersection, out PointF _, out PointF _);
                //Debug.Assert(linesIntersect, "Edges " + i + "-->" + j + " and " + j + "-->" + k + " are parallel");

                offsetPoints.Add(intersection);
            }

            points = new List<PointF>(offsetPoints);

            return fixSelfIntesections(offsetPoints);
        }

        public static void drawOffsetPolygon(Graphics g, List<PointF> polygon, float offset)
        {
            if (PolygonIsOrientedClockwise(polygon))
                offset *= (-1);

            for (int j = 0; j < polygon.Count; j++)
            {
                int k = (j + 1) % polygon.Count;
                (PointF pjk1, PointF pjk2) = Manager.findParallelSegment(polygon[j], polygon[k], offset);

                PointF[] rectangle = { polygon[j], polygon[k], pjk2, pjk1 };
                g.FillPolygon(Brushes.Orange, rectangle);

                g.FillEllipse(Brushes.Orange,
                    new RectangleF(new PointF(polygon[j].X - offset, polygon[j].Y - offset), new SizeF(2 * offset, 2 * offset)));
            }
        }

        #endregion

        #region workSpace events

        public static void workSpace_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                move = Move.None;

                ((int index, int i), PointF p) = isNearbyVertex(e.Location, true);
                if (p != new Point(-1, -1))
                {
                    deleteRel(index, i);
                    Manager.polygons[index].Remove(p);
                    Manager.edges[index].RemoveAt(i);
                    if (index > -1 && Manager.offsets[index].Count > 0)
                        updateOffset(index, Manager.offsetsValue[index]);
                }

                else
                {
                    ((index, (int ix1, _)), (PointF p1, _)) = isNearbyEdge(e.Location);
                    if (p1 != new Point(-1, -1))
                    {
                        showDirectionMenu(index, ix1);
                        pToAddRel = index;
                        etoAddRel = ix1;
                        if (index > - 1 && Manager.offsets[index].Count > 0)
                            updateOffset(index, Manager.offsetsValue[index]);
                    }

                    else
                    {
                        pToMove = pFindPolygon(e.Location);
                        if (pToMove != -1)
                        {
                            updateOffset(pToMove, currentOffset);
                            move = Move.Offset;
                        }

                        else
                            move = Move.None;
                    }
                }
            }

            else
            {
                ((pToMove, int ix), PointF p) = isNearbyVertex(e.Location, false);
                if (p != new PointF(-1, -1))
                {
                    vToMove = ix;
                    move = Move.Vertex;
                }

                else
                {
                    ((pToMove, (int ix1, int ix2)), (PointF p1, PointF _)) = isNearbyEdge(e.Location);
                    if (p1 != new PointF(-1, -1))
                    {
                        startPoint = e.Location;
                        e1ToMove = ix1;
                        e2ToMove = ix2;
                        move = Move.Edge;
                    }

                    else
                    {
                        pToMove = pFindPolygon(e.Location);
                        if (pToMove != -1)
                        {
                            startPoint = e.Location;
                            move = Move.Polygon;
                        }

                        else
                            move = Move.None;
                    }
                }
            }

            ((PictureBox)sender).Invalidate();

        }

        #region Move

        private static void moveVertex(MouseEventArgs e)
        {
            Manager.polygons[pToMove][vToMove] = new PointF(e.Location.X, e.Location.Y);

            updateRel(pToMove, vToMove);
        }

        private static void moveEdge(MouseEventArgs e)
        {
            int dX = e.Location.X - startPoint.X, dY = e.Location.Y - startPoint.Y;

            Manager.polygons[pToMove][e1ToMove]
                = new PointF(Manager.polygons[pToMove][e1ToMove].X + dX,
                            Manager.polygons[pToMove][e1ToMove].Y + dY);
            updateRel(pToMove, e1ToMove);

            Manager.polygons[pToMove][e2ToMove]
                = new PointF(Manager.polygons[pToMove][e2ToMove].X + dX,
                            Manager.polygons[pToMove][e2ToMove].Y + dY);
            updateRel(pToMove, e2ToMove);

            startPoint = e.Location;
        }

        private static void movePolygon(MouseEventArgs e)
        {
            int dX = e.Location.X - startPoint.X, dY = e.Location.Y - startPoint.Y;
            for (int i = 0; i < Manager.polygons[pToMove].Count; i++)
            {
                PointF p = Manager.polygons[pToMove][i];
                Manager.polygons[pToMove][i] = new PointF(p.X + dX, p.Y + dY);
            }
            startPoint = e.Location;
        }

        #endregion
        public static void workSpace_MouseMove(object sender, MouseEventArgs e)
        {
            switch (move)
            {
                case Move.Vertex:
                    moveVertex(e);
                    break;
                case Move.Edge:
                    moveEdge(e);
                    break;
                case Move.Polygon:
                    movePolygon(e);
                    break;
                default:
                    break;
            }

            if (pToMove > - 1 && pToMove < Manager.offsets.Count && Manager.offsets[pToMove].Count > 0)
                updateOffset(pToMove, Manager.offsetsValue[pToMove]);

            ((PictureBox)sender).Invalidate();
        }

        public static void workSpace_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(((PictureBox)sender).BackColor);

            Manager.drawPolygons(sender, e);

        }

        public static void workSpace_MouseUp(object sender, MouseEventArgs e)
        {
            if (move != Move.Offset)
            {
                move = Move.None;
                pToMove = -1;
            }
            ((PictureBox)sender).Invalidate();
        }

        public static void workSpace_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ((pToMove, (int ix1, int ix2)), (PointF p1, PointF p2)) = isNearbyEdge(e.Location);
            if (p1 != new PointF(-1, -1))
            {
                Manager.edges[pToMove][ix1].direction = Direction.Any;
                _ = Manager.findDistanceToSegment(e.Location, p1, p2, out PointF closest);
                PointF point = new Point((int)closest.X, (int)closest.Y);
                Manager.polygons[pToMove].Insert(ix2, point);
                Manager.edges[pToMove].Insert(ix2, new Edge());
            }
            ((PictureBox)sender).Invalidate();
        }

        #endregion

        #region ContextMenuStrip

        private static void showDirectionMenu(int index, int ix1)
        {
            horizontalMenuItem.Checked = false;
            verticalMenuItem.Checked = false;
            if (Manager.edges[index][ix1].direction == Direction.Horizontal)
                horizontalMenuItem.Checked = true;
            if (Manager.edges[index][ix1].direction == Direction.Vertical)
                verticalMenuItem.Checked = true;

            horizontalMenuItem.Enabled = true;
            verticalMenuItem.Enabled = true;
            if (Manager.edges[index][(ix1 + 1) % Manager.polygons[index].Count].direction
                == Direction.Horizontal)
                horizontalMenuItem.Enabled = false;
            if (Manager.edges[index][(ix1 + 1) % Manager.polygons[index].Count].direction
                == Direction.Vertical)
                verticalMenuItem.Enabled = false;
            int prev = ix1 - 1 == -1 ? Manager.polygons[index].Count - 1 : ix1 - 1;
            if (Manager.edges[index][prev].direction == Direction.Horizontal)
                horizontalMenuItem.Enabled = false;
            if (Manager.edges[index][prev].direction == Direction.Vertical)
                verticalMenuItem.Enabled = false; 

            contextMenuStrip.Show(Cursor.Position);
        }

        public static void horizontalMenuItem_Click(object sender, EventArgs e)
        {
            if (horizontalMenuItem.Checked)
            {
                horizontalMenuItem.Checked = false;
                Manager.edges[pToAddRel][etoAddRel].direction = Direction.Any;
            }
            else
            {
                horizontalMenuItem.Checked = true;
                verticalMenuItem.Checked = false;
                Manager.edges[pToAddRel][etoAddRel].direction = Direction.Horizontal;
                updateRel(pToAddRel, etoAddRel);
            }
        }

        public static void verticalMenuItem_Click(object sender, EventArgs e)
        {
            if (verticalMenuItem.Checked)
            {
                verticalMenuItem.Checked = false;
                Manager.edges[pToAddRel][etoAddRel].direction = Direction.Any;
            }
            else
            {
                verticalMenuItem.Checked = true;
                horizontalMenuItem.Checked = false;
                Manager.edges[pToAddRel][etoAddRel].direction = Direction.Vertical;
                updateRel(pToAddRel, etoAddRel);
            }
        }

        #endregion

        #region OffsetTrackBar

        public static void offsetTrackBar_Scroll(object sender, EventArgs e)
        {
            currentOffset = ((TrackBar)sender).Value;

            if (move == Move.Offset)
            {
                if (Manager.offsets[pToMove].Count > 0)
                {
                    List<PointF> offset = findOffsetPolygon(Manager.polygons[pToMove], currentOffset,
                        out List<PointF> points);
                    Manager.offsets[pToMove] = offset;
                    Manager.notFixed[pToMove] = points;
                    Manager.offsetsValue[pToMove] = currentOffset;
                }
            }
        }

        #endregion

        public static void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && move == Move.Offset)
            {
                Manager.polygons.RemoveAt(pToMove);
                Manager.edges.RemoveAt(pToMove);

                Manager.offsets.RemoveAt(pToMove);
                Manager.notFixed.RemoveAt(pToMove);
                Manager.offsetsValue.RemoveAt(pToMove);

            }
        }
    }
}
