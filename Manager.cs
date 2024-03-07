using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public enum Mode
{
    Draw,
    Edit
}

public enum DrawSet
{
    Lib,
    Bresenham,
    WU
}

public enum Offset
{
    I,
    II,
    III
}

namespace PolygonEditor
{
    internal static class Manager
    {
        public static List<List<PointF>> polygons = new List<List<PointF>>();
        public static List<List<Edge>> edges = new List<List<Edge>>();

        public static List<List<PointF>> offsets = new List<List<PointF>>();
        public static List<List<PointF>> notFixed = new List<List<PointF>>();
        public static List<float> offsetsValue = new List<float>();

        public static Mode mode = Mode.Draw;
        public static DrawSet drawSet = DrawSet.Lib;
        public static Offset offset = Offset.I;
        const double eps = 50;

        public static ColorDialog colorDialog = new ColorDialog();
        public static Color currentColor = Color.Black;
        private static int pWidth = 10;
        public static Pen p = new Pen(Color.Black, pWidth), dp = new Pen(Color.Black, pWidth);
        private static SolidBrush b = new SolidBrush(Color.Black);

        #region startScene

        private static void AddPolygon(PointF[] polygon)
        {
            List<Edge> polygonEdges = new List<Edge>();
            for (int i = 0; i < polygon.Length; i++)
                polygonEdges.Add(new Edge());


            polygons.Add(polygon.ToList());
            edges.Add(polygonEdges);

            offsets.Add(new List<PointF>());
            notFixed.Add(new List<PointF>());
            offsetsValue.Add(0);
        }

        public static void generateStartScene()
        {
            PointF[] polygon1 = { new PointF(167, 370), new PointF(160, 907), new PointF(799, 850),
                new PointF(772, 520), new PointF(601, 517), new PointF(622, 758), new PointF(402, 749),
                new PointF(416, 316), new PointF(625, 293), new PointF(492, 487), new PointF(918, 482),
                new PointF(761, 71)};
            PointF[] polygon2 = { new PointF(1565, 139), new PointF(1853, 279), new PointF(1714, 333),
                new PointF(1969, 605), new PointF(2084, 453), new PointF(1899, 303), new PointF(2336, 183),
                new PointF(2135, 814), new PointF(1376, 556)};
            PointF[] polygon3 = { new PointF(2130, 1054), new PointF(2031, 1330), new PointF(2400, 1356),
                new PointF(2380, 1165) };
            PointF[] polygon4 = { new PointF(1314, 1186), new PointF(1211, 1143),
                new PointF(1211, 1286), new PointF(1154, 1289), new PointF(1160, 934), new PointF(1356, 979),
                new PointF(1035, 822), new PointF(1027, 1374), new PointF(1331, 1393)};

            AddPolygon(polygon1);
            AddPolygon(polygon2);
            AddPolygon(polygon3);
            AddPolygon(polygon4);
        }

        #endregion

        #region save/load scene

        public static void SaveData(string filename)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    foreach (List<PointF> polygon in polygons)
                    {
                        string polygonData = string.Join(",", polygon.Select(p => $"{p.X},{p.Y}"));
                        writer.WriteLine($"polygon,{polygonData}");
                    }

                    foreach (List<Edge> edgeList in edges)
                    {
                        string edgeData = string.Join(",", edgeList.Select(e => $"{e.edgeColor.ToArgb()},{e.vertexColor.ToArgb()},{(int)e.direction}"));
                        writer.WriteLine($"edge,{edgeData}");
                    }

                    //foreach (List<PointF> offset in offsets)
                    //{
                    //    string offsetData = string.Join(",", offset.Select(p => $"{p.X},{p.Y}"));
                    //    writer.WriteLine($"offset,{offsetData}");
                    //}

                    //foreach (List<PointF> notFixedPolygon in notFixed)
                    //{
                    //    string notFixedData = string.Join(",", notFixedPolygon.Select(p => $"{p.X},{p.Y}"));
                    //    writer.WriteLine($"notFixed,{notFixedData}");
                    //}

                    string offsetsValueData = string.Join(",", offsetsValue.Select(v => v.ToString()));
                    writer.WriteLine($"offsetsValue,{offsetsValueData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }

        public static void LoadData(string filename)
        {
            try
            {
                polygons.Clear();
                edges.Clear();
                offsets.Clear();
                notFixed.Clear();
                offsetsValue.Clear();

                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length < 2)
                            continue;

                        string dataType = parts[0];

                        switch (dataType)
                        {
                            case "polygon":
                                {
                                    List<PointF> polygon = new List<PointF>();
                                    if (!parts[1].Equals(String.Empty))
                                        for (int i = 1; i < parts.Length; i += 2)
                                        {
                                            float x = float.Parse(parts[i]);
                                            float y = float.Parse(parts[i + 1]);
                                            polygon.Add(new PointF(x, y));
                                        }
                                    polygons.Add(polygon);
                                }
                                break;

                            case "edge":
                                {
                                    List<Edge> edgeList = new List<Edge>();
                                    if (!parts[1].Equals(String.Empty))
                                        for (int i = 1; i < parts.Length; i += 3)
                                        {
                                            Color edgeColor = Color.FromArgb(int.Parse(parts[i]));
                                            Color vertexColor = Color.FromArgb(int.Parse(parts[i + 1]));
                                            Direction direction = (Direction)int.Parse(parts[i + 2]);
                                            Edge edge = new Edge { edgeColor = edgeColor, vertexColor = vertexColor, direction = direction };
                                            edgeList.Add(edge);
                                        }
                                    edges.Add(edgeList);
                                }
                                break;

                            case "offset":
                                {
                                    List<PointF> offset = new List<PointF>();
                                    if (!parts[1].Equals(String.Empty))
                                        for (int i = 1; i < parts.Length; i += 2)
                                        {
                                            float x = float.Parse(parts[i]);
                                            float y = float.Parse(parts[i + 1]);
                                            offset.Add(new PointF(x, y));
                                        }
                                    offsets.Add(offset);
                                }
                                break;
                            case "notFixed":
                                {
                                    List<PointF> notFixedPolygon = new List<PointF>();
                                    if (!parts[1].Equals(String.Empty))
                                        for (int i = 1; i < parts.Length; i += 2)
                                        {
                                            float x = float.Parse(parts[i]);
                                            float y = float.Parse(parts[i + 1]);
                                            notFixedPolygon.Add(new PointF(x, y));
                                        }
                                    notFixed.Add(notFixedPolygon);
                                }
                                break;
                            case "offsetsValue":
                                {
                                    List<float> offsetsValueData = new List<float>();
                                    if (!parts[1].Equals(String.Empty))
                                        for (int i = 1; i < parts.Length; i++)
                                        {
                                            float value = float.Parse(parts[i]);
                                            offsetsValueData.Add(value);
                                        }
                                    offsetsValue.AddRange(offsetsValueData);
                                }
                                break;
                            default:
                                break;
                        }

                        //for (int i = 0; i < polygons.Count; i++)
                        //{
                        //    offsets.Add(new List<PointF>());
                        //    notFixed.Add(new List<PointF>());

                        //    if (offsetsValue[i] != 0)
                        //        EditMode.updateOffset(i, offsetsValue[i]);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading data: " + ex.Message);
            }
        }

        #endregion

        #region Bresenham

        //https://stackoverflow.com/questions/11678693/all-cases-covered-bresenhams-line-algorithm
        public static void bresenhamDrawLine(Graphics g, Color color, PointF s, PointF p)
        {
            int x = (int)s.X, y = (int)s.Y, x2 = (int)p.X, y2 = (int)p.Y;
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                b.Color = color;
                g.FillEllipse(b, new Rectangle(x, y, 1, 1));

                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        #endregion

        #region WULine

        // https://en.wikipedia.org/wiki/Xiaolin_Wu's_line_algorithm

        public static void plot(Graphics g, float x, float y, float c)
        {
            //plot the pixel at(x, y) with brightness c(where 0 ≤ c ≤ 1)

            int alpha = (int)(c * 255); // Scale the brightness to the alpha channel (0-255)

            Color pixelColor = Color.FromArgb(alpha, Color.Black); // You can use any color you want

            using (SolidBrush brush = new SolidBrush(pixelColor))
            {
                g.FillRectangle(brush, x, y, 1, 1); // Draw a 1x1 pixel
            }
        }

        // integer part of x
        public static int ipart(float x)
        {
            return (int)Math.Floor((double)x);
        }

        public static int round(float x)
        {
            return ipart((float)(x + 0.5));
        }

        // fractional part of x
        public static float fpart(float x)
        {
            return x - ipart(x);
        }

        public static float rfpart(float x)
        {
            return 1 - fpart(x);
        }

        public static void wuDrawLine(Graphics g, PointF p0, PointF p1)
        {
            float x0 = p0.X, y0 = p0.Y, x1 = p1.X, y1 = p1.Y;
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            float dx = x1 - x0;
            float dy = y1 - y0;

            float gradient;
            if (dx == 0.0)
                gradient = 1.0f;
            else
                gradient = dy / dx;

            // handle first endpoint
            int xend = round(x0);
            int yend = (int)(y0 + gradient * (xend - x0));
            float xgap = rfpart((float)(x0 + 0.5));
            int xpxl1 = xend;// this will be used in the main loop
            int ypxl1 = ipart(yend);
            if (steep)
            {
                plot(g, ypxl1, xpxl1, rfpart(yend) * xgap);
                plot(g, ypxl1 + 1, xpxl1, fpart(yend) * xgap);
            }
            else
            {
                plot(g, xpxl1, ypxl1, rfpart(yend) * xgap);
                plot(g, xpxl1, ypxl1 + 1, fpart(yend) * xgap);
            }

            float intery = yend + gradient; // first y-intersection for the main loop

            // handle second endpoint
            xend = round(x1);
            yend = (int)(y1 + gradient * (xend - x1));
            xgap = fpart((float)(x1 + 0.5));
            int xpxl2 = xend; //this will be used in the main loop
            int ypxl2 = ipart(yend);
            if (steep)
            {
                plot(g, ypxl2, xpxl2, rfpart(yend) * xgap);
                plot(g, ypxl2 + 1, xpxl2, fpart(yend) * xgap);
            }
            else
            {
                plot(g, xpxl2, ypxl2, rfpart(yend) * xgap);
                plot(g, xpxl2, ypxl2 + 1, fpart(yend) * xgap);
            }

            // main loop
            if (steep)
            {
                for (int x = xpxl1 + 1; x <= xpxl2 - 1; x++)
                {
                    plot(g, ipart(intery), x, rfpart(intery));
                    plot(g, ipart(intery) + 1, x, fpart(intery));
                    intery = intery + gradient;
                }
            }
            else
            {
                for (int x = xpxl1 + 1; x <= xpxl2 - 1; x++)
                {
                    plot(g, x, ipart(intery), rfpart(intery));
                    plot(g, x, ipart(intery) + 1, fpart(intery));
                    intery = intery + gradient;
                }
            }
        }

        #endregion

        #region drawPolygons

        public static void colorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog.Color;
                ((Button)sender).BackColor = currentColor;

                DrawMode.colorButton_Click(sender, e);
            }
        }

        public static void drawVertex(Graphics g, PointF p)
        {
            g.FillEllipse(Brushes.Red,
                        new Rectangle((int)p.X - pWidth, (int)p.Y - pWidth, 2 * pWidth, 2 * pWidth));
        }

        public static void drawLines(Graphics g, List<PointF> polygon, List<Edge>? edges = default, bool vertices = false)
        {
            p.Color = Color.Black;
            for (int j = 0; j < polygon.Count - 1; j++)
            {
                if (edges != null)
                    p.Color = edges[j].edgeColor;
                if (Manager.drawSet == DrawSet.Lib)
                    g.DrawLine(p, polygon[j], polygon[j+1]);
                if (Manager.drawSet == DrawSet.WU)
                    wuDrawLine(g, polygon[j], polygon[j + 1]);
                else
                    bresenhamDrawLine(g, p.Color, polygon[j], polygon[j + 1]);

                if (vertices)
                    drawVertex(g, polygon[j]);
            }
        }
        public static void drawPolygon(Graphics g, List<PointF> polygon, List<Edge>? edges = default, bool vertices = false)
        {
            drawLines(g, polygon, edges, vertices);
            if (vertices)
                drawVertex(g, polygon.Last());

            p.Color = Color.Black;
            if (edges != null)
                p.Color = edges.Last().edgeColor;

            if (Manager.drawSet == DrawSet.Lib)
                g.DrawLine(p, polygon.Last(), polygon.First());
            if (Manager.drawSet == DrawSet.WU)
                wuDrawLine(g, polygon.Last(), polygon.First());
            else
                bresenhamDrawLine(g, p.Color, polygon.Last(), polygon.First());
        }

        #region drection icons

        public static (PointF p1, PointF p2) findParallelSegment(PointF s, PointF e, float offset)
        {
            // Calculate vector for the segment.
            PointF v = new PointF(e.X - s.X, e.Y - s.Y);
            
            float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);

            // Normalize the vector and multiply by the offset distance.
            if (length > 0)
            {
                v.X /= length;
                v.Y /= length;
                v.X *= offset;
                v.Y *= offset;
            }

            // Calculate the normal vector.
            PointF n = new PointF(-v.Y, v.X);

            // Calculate the new points after offset.
            PointF p1 = new PointF(s.X + n.X, s.Y + n.Y);
            PointF p2 = new PointF(e.X + n.X, e.Y + n.Y);
            return (p1, p2);
        }

        private static void drawIcon(Graphics g, PointF s, PointF e, Direction direction)
        {

            (PointF p1, PointF p2) = findParallelSegment(s, e, 2 * pWidth + 10);
            float midX = (float)((p1.X + p2.X) / 2 - 5);
            float midY = (float)((p1.Y + p2.Y) / 2 - 5);
            PointF midPoint = new PointF(midX, midY);

            switch (direction)
            {
                case Direction.Horizontal:
                    using (Font myFont = new Font("Arial", 10))
                    {
                        g.DrawString("H", myFont, Brushes.Green, midPoint);
                    }
                    break;
                case Direction.Vertical:
                    using (Font myFont = new Font("Arial", 10))
                    {
                        g.DrawString("V", myFont, Brushes.Green, midPoint);
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        public static void drawPolygons(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(((PictureBox)sender).BackColor);

            switch (offset)
            {
                case Offset.I:
                    for (int i = 0; i < offsets.Count; i++)
                    {
                        if (offsets[i].Count > 0)
                        {
                            p.Width = 4;
                            drawPolygon(g, offsets[i]);
                            //g.FillPolygon(Brushes.Plum, offsets[i].ToArray());

                            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
                            g.FillPolygon(semiTransBrush, offsets[i].ToArray());
                            semiTransBrush.Dispose();

                            p.Width = pWidth;
                        }
                    }
                    break;
                case Offset.II:
                    for (int i = 0; i < notFixed.Count; i++)
                    {
                        dp.Color = Color.Brown;
                        if (notFixed[i].Count > 0)
                        {
                            dp.Width = 4;
                            g.DrawPolygon(dp, notFixed[i].ToArray());

                            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(128, 122, 45, 255));
                            g.FillPolygon(semiTransBrush, offsets[i].ToArray());
                            semiTransBrush.Dispose();

                            dp.Width = pWidth;
                        }
                        p.Color = Color.Black;
                    }
                    break;
                case Offset.III:
                    for (int i = 0; i < notFixed.Count; i++)
                        if (offsetsValue[i] > 0)
                        EditMode.drawOffsetPolygon(g, polygons[i], offsetsValue[i]);
                    break;
            }


            for (int i = 0; i < polygons.Count; i++)
            {
                SolidBrush backColorBrush = new SolidBrush(((PictureBox)sender).BackColor);
                g.FillPolygon(backColorBrush, polygons[i].ToArray());
                backColorBrush.Dispose();
                drawPolygon(g, polygons[i], edges[i], true);
            }

            for (int i = 0; i < polygons.Count; i++)
                for (int j = 0; j < polygons[i].Count; j++)
                    drawIcon(g, polygons[i][j], polygons[i][(j + 1) % polygons[i].Count], edges[i][j].direction);
        }

        #endregion

        #region isNearby
        private static double getDistance(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        public static bool isNearbyPoint (PointF p1, PointF p2)
        {
            return getDistance(p1, p2) < eps;
        }

        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        // http://www.csharphelper.com/howtos/howto_point_segment_distance.html
        public static double findDistanceToSegment(PointF pt, PointF p1, PointF p2, out PointF closest)
        {

            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
            
            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new PointF(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new PointF(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static bool isNearbySegment(PointF p, PointF p1, PointF p2)
        {
            return findDistanceToSegment(p, p1, p2, out PointF _) < eps;
        }

        #endregion

        #region isInPolygon

        // Return True if the point is in the polygon.
        // http://www.csharphelper.com/howtos/howto_polygon_geometry_point_inside.html
        public static bool isInPolygon(PointF p, List<PointF> Points)
        {
            float X = p.X, Y = p.Y;
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = Points.Count() - 1;
            float total_angle = GetAngle(
                Points[max_point].X, Points[max_point].Y,
                X, Y,
                Points[0].X, Points[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    Points[i].X, Points[i].Y,
                    X, Y,
                    Points[i + 1].X, Points[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }

        // Return the angle ABC.
        // Return a value between PI and -PI.
        // Note that the value is the opposite of what you might
        // expect because Y coordinates increase downward.
        public static float GetAngle(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the dot product.
            float dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            float cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return (float)Math.Atan2(cross_product, dot_product);
        }

        // Return the dot product AB . BC.
        // Note that AB x BC = |AB| * |BC| * Cos(theta).
        private static float DotProduct(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }

        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        public static float CrossProductLength(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        #endregion

        #region workSpace events
        public static void workSpace_MouseDown(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Draw:
                    DrawMode.workSpace_MouseDown(sender, e);
                    break;
                case Mode.Edit:
                    EditMode.workSpace_MouseDown(sender, e);
                    break;
                default: 
                    break;
            }
        }

        public static void workSpace_MouseMove(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Draw:
                    DrawMode.workSpace_MouseMove(sender, e);
                    break;
                case Mode.Edit:
                    EditMode.workSpace_MouseMove(sender, e);
                    break;
                default:
                    break;
            }
        }


        public static void workSpace_Paint(object sender, PaintEventArgs e)
        {
            switch (mode)
            {
                case Mode.Draw:
                    DrawMode.workSpace_Paint(sender, e);
                    break;
                case Mode.Edit:
                    EditMode.workSpace_Paint(sender, e);
                    break;
                default:
                    break;
            }

        }

        public static void workSpace_MouseUp(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Draw:
                    DrawMode.workSpace_MouseUp(sender, e);
                    break;
                case Mode.Edit:
                    EditMode.workSpace_MouseUp(sender, e);
                    break;
                default:
                    break;
            }
        }

        public static void workSpace_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.Edit:
                    EditMode.workSpace_MouseDoubleClick(sender, e);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region ContextMenuStrip

        public static void horizontalMenuItem_Click(object sender, EventArgs e)
        {
            switch (mode)
            {
                case Mode.Edit:
                    EditMode.horizontalMenuItem_Click(sender, e);
                    break;
                default:
                    break;
            }
        }

        public static void verticalMenuItem_Click(object sender, EventArgs e)
        {
            switch (mode)
            {
                case Mode.Edit:
                    EditMode.verticalMenuItem_Click(sender, e);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region OffsetTrackBar

        public static void offsetTrackBar_Scroll(object sender, EventArgs e)
        {
            switch (mode)
            {
                case Mode.Edit:
                    EditMode.offsetTrackBar_Scroll(sender, e);
                    break;
                default:
                    break;
            }
        }

        #endregion

        public static void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (mode)
            {
                case Mode.Draw:
                    DrawMode.MainForm_KeyDown(sender, e);
                    break;
                case Mode.Edit:
                    EditMode.MainForm_KeyDown(sender, e);
                    break;
                default:
                    break;
            }
        }
    }
}
