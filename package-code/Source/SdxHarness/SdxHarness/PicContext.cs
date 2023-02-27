using SdxHelpers;
using SdxVisio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SdxHarness
{
    /// <summary>
    /// Context for drawing SDX DataSet on a picturebox.
    /// </summary>
    public class PicContext
    {
        public Pen MyBlackPen = Pens.Black;
        public Pen MyRedPen = Pens.Red;
        public Pen MyGreenPen = Pens.Green;
        public Pen MyBluePen = Pens.Blue;
        public Pen MyArtifactPen = Pens.Gray;

        public Font MyTextFont = new Font("Arial", 9);
        /// <summary>
        /// The range of values for the input (raw) dimensions
        /// </summary>
        private SizeF InRange = new SizeF(100f, 100f); // default

        private SizeF Scaler = new SizeF(1F, 1F); // default
        /// <summary>
        /// The minimum for both X and Y
        /// </summary>
        private PointF InOffset = new PointF(0, 0);

        private Size PicRange = new Size(100, 100); // default
        private Point PicOffset = new Point(0, 0);

        public float XScale { get { return InRange.Width / (float) PicRange.Width;  } }
        public float YScale {  get { return InRange.Height / (float) PicRange.Height; } }

        private PictureBox picBox { get; set; }

        public SdxDataSetContext DsContext { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xRange"></param>
        /// <param name="yRange"></param>
        /// <param name="picHeight"></param>
        /// <param name="picWidth"></param>
        public PicContext(PictureBox pic, SdxDataSetContext dsContext) // float xRange, float yRange, int picHeight, int picWidth)
        {
            MyBlackPen = Pens.Black;
            MyRedPen = Pens.Red;
            MyGreenPen = Pens.Green;
            MyArtifactPen = new Pen(Color.Gray);
            MyArtifactPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

            picBox = pic;
            DsContext = dsContext;

            if ( DsContext != null )
            {
                string explanation = "";
                if (!ComputeDimensions(picBox.Size, DsContext, out explanation))
                    throw new ApplicationException($"PicContext Constructor Err={explanation}");

            }


        }



        /// <summary>
        /// Convert a Pic Point to a PointF based on context dimensions.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public PointF ConvertToPointF(Point pt)
        {
            float xx = (pt.X - PicOffset.X) / (float)PicRange.Width * InRange.Width + InOffset.X;
            float yy = (pt.Y - PicOffset.Y) / (float)PicRange.Width * InRange.Width + InOffset.Y;

            PointF ptF = new PointF(xx, yy);
            return ptF;
        }

        /// <summary>
        /// Convert a PointF to a Point based on context dimensions.
        /// </summary>
        /// <param name="ptF"></param>
        /// <returns></returns>
        public Point ConvertFromPointF(PointF ptF)
        {
            //float x = ((ptF.X - InOffset.X) / InRange.Width) * PicRange.Width + PicOffset.X;
            //float y = ((ptF.Y - InOffset.Y) / InRange.Height) * PicRange.Height + PicOffset.Y;

            float x = (ptF.X - InOffset.X) * Scaler.Width; // + PicOffset.X;
            float y = (ptF.Y - InOffset.Y) * Scaler.Height; // + PicOffset.Y;

            int ix = (int)Math.Round(x);
            int iy = (int)Math.Round(y);

            Point pt = new Point(ix, iy);
            return pt;
        }

        /// <summary>
        /// Convert from the size in the X directon
        /// </summary>
        /// <param name="xSize"></param>
        /// <returns></returns>
        public int ConvertFromXSizeF(float xSize)
        {
            int ww = (int)Math.Round(xSize * Scaler.Width);
            return ww;
        }

        /// <summary>
        /// Convert from the size in the Y direction
        /// </summary>
        /// <param name="ySize"></param>
        /// <returns></returns>
        public int ConvertFromYSizeF(float ySize)
        {
            int hh = (int)Math.Round(ySize * Scaler.Height);
            return hh;
        }

        /// <summary>
        /// Calculate the graphics range, given a border size
        /// </summary>
        /// <param name="gg"></param>
        public void Recalculate(Graphics gg, SdxDataSetContext dsContext)
        {

            float ww = gg.VisibleClipBounds.Width;
            float hh = gg.VisibleClipBounds.Height;

            SizeF size = new SizeF(ww, hh);

            string explanation = "";
            if (!ComputeDimensions(size, dsContext, out explanation))
                throw new ApplicationException($"Err={explanation}");
                
        }


        /// <summary>
        /// Given the picturebox size and dataset context, compute new dimensions
        /// by looking at all of the shapes that are objects or artifacts.
        /// </summary>
        /// <param name="dsContext"></param>
        /// <param name="explanation"></param>
        private bool ComputeDimensions(SizeF size, SdxDataSetContext dsContext, out string explanation)
        {
            explanation = "";
            try
            {
                float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;

                // These are pin points (center of object), so decrease min by 1/2 width, and increase max by 1/2 width
                foreach (SdxObject obj in dsContext.ObjectDict.Values)
                {
                    if (!double.IsNaN(obj.Center.X) && !double.IsNaN(obj.Size.xLength))
                    {
                        minX = (float)Math.Min(minX, obj.Center.X - 0.5 * obj.Size.xLength);
                        maxX = (float)Math.Max(maxX, obj.Center.X + 0.5 * obj.Size.xLength);
                    }

                    if (!double.IsNaN(obj.Center.Z) && !double.IsNaN(obj.Size.zHeight))
                    {
                        minY = (float)Math.Min(minY, obj.Center.Z - 0.5 * obj.Size.zHeight);
                        maxY = (float)Math.Max(maxY, obj.Center.Z + 0.5 * obj.Size.zHeight);
                    }
                }

                foreach (SdxArtifact artifact in dsContext.ArtifactDict.Values)
                {
                    if ( !double.IsNaN(artifact.Center.X) && !double.IsNaN(artifact.Size.xLength))
                    {
                        minX = (float)Math.Min(minX, artifact.Center.X - 0.5 * artifact.Size.xLength);
                        maxX = (float)Math.Max(maxX, artifact.Center.X + 0.5 * artifact.Size.xLength);
                    }

                    if (!double.IsNaN(artifact.Center.Z) && !double.IsNaN(artifact.Size.zHeight))
                    {
                        minY = (float)Math.Min(minY, artifact.Center.Z - 0.5 * artifact.Size.zHeight);
                        maxY = (float)Math.Max(maxY, artifact.Center.Z + 0.5 * artifact.Size.zHeight);
                    }

                }

                InRange = new SizeF(maxX - minX, maxY - minY);
                InOffset = new PointF(minX, minY);

                // Give a 5% border
                PointF border = new PointF(0.05f, 0.05f);
                PicOffset = new Point((int)(border.X * size.Width), (int)(border.Y * size.Height));
                PicRange = new Size((int)(size.Width * (1f - (2 * border.X))), (int)(size.Height * (1f - (2 * border.Y))));

                // To not distort the ratio, choose the larger dimension to match with the picture box.
                bool fixedAspectRatio = true;
                Scaler = new SizeF( .95f * size.Width / InRange.Width, 0.95f * size.Height / InRange.Height);

                if ( fixedAspectRatio)
                {
                    if ( Scaler.Width < Scaler.Height )
                    {
                        Scaler = new SizeF(Scaler.Width, Scaler.Width);
                    }
                    else
                    {
                        Scaler = new SizeF(Scaler.Height, Scaler.Height);
                    }

                }
                else
                {
                    // Leave it 'as is'
                }

                return true;

            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }

        }

        /// <summary>
        /// Draw the SDX objects on a picturebox.
        /// </summary>
        /// <param name="context"></param>
        public bool Draw(Graphics gg, SdxDataSetContext context, out string explanation)
        {
            explanation = "";
            string marker = "Begin";

            try
            {
                if (context?.ObjectDict == null)
                {
                    explanation = $"DS context or Object dictionary is null.";
                    return false;
                }

                SizeF size = new SizeF(gg.VisibleClipBounds.Width, gg.VisibleClipBounds.Height);
                if (!ComputeDimensions(size, context, out explanation))
                    return false;


                foreach (SdxObject obj in context.ObjectDict.Values)
                {
                    marker = $"Object={obj.Name}";
                    DrawObject( gg, obj);
                }

                foreach (SdxLink link in context.LinkDict.Values)
                {
                    marker = $"Link={link.Name}";
                    DrawLink( gg, link);
                }

                foreach (SdxArtifact ai in context.ArtifactDict.Values)
                {
                    DrawArtifact(gg, ai);
                }

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker} Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Draw the link that connects two objects.
        /// </summary>
        /// <param name="link"></param>
        public void DrawLink(Graphics gg, SdxLink link)
        {
            // Convert context
            Point p1 = new Point(), p2;
            int nn = 0;

            foreach ( SdxVector vertex in link.VertexList)
            {
                p2 = ConvertFromPointF(new PointF((float)vertex.X, (float)vertex.Z));

                if ( nn == 0) // Draw caption
                {
                    int ww = ConvertFromXSizeF((float) link.Width);
                    int hh = ConvertFromYSizeF((float) -link.Height);

                    Point p4 = new Point(p2.X - ww / 2, p2.Y - hh / 2);
                    gg.DrawString(link.Name, MyTextFont, Brushes.Black, p2);

                }
                else
                {
                    if ((nn % 3) == 0)
                        gg.DrawLine(MyRedPen, p1, p2);
                    else if ((nn % 3) == 1)
                        gg.DrawLine(MyGreenPen, p1, p2);
                    else
                        gg.DrawLine(MyBluePen, p1, p2);
                }

                p1 = p2;
                nn++;
            } // for each vertex
        }

        /// <summary>
        /// Draw an object.
        /// </summary>
        /// <param name="obj"></param>
        public void DrawObject(Graphics gg, SdxObject obj)
        {
            if (obj.Name.ToLower().Contains("separator"))
            {
                bool isTest = true;
            }
            Point p1 = ConvertFromPointF(new PointF((float)obj.Center.X, (float)obj.Center.Z));
            int ww = ConvertFromXSizeF((float)obj.Size.xLength);
            int hh = ConvertFromYSizeF((float)obj.Size.zHeight);

            // The point is the 'pin' point, which is at the center.
            // To display properly we'll back off by half the width and height

            Point p2 = new Point(p1.X - ww / 2, p1.Y - hh / 2);
            Rectangle rect = new Rectangle(p2, new Size(ww, hh));

            gg.DrawRectangle(MyBlackPen, rect);

            gg.DrawString(obj.Name, MyTextFont, Brushes.Black, p2);
        }
        /// <summary>
        /// Draw an object.
        /// </summary>
        /// <param name="obj"></param>
        public void DrawArtifact(Graphics gg, SdxArtifact ai)
        {

            Point p1 = ConvertFromPointF(new PointF((float)ai.Center.X, (float)ai.Center.Z));
            int ww = ConvertFromXSizeF((float)ai.Size.xLength);
            int hh = ConvertFromYSizeF((float)ai.Size.zHeight);

            // The point is the 'pin' point, which is at the center.
            // To display properly we'll back off by half the width and height

            Point p2 = new Point(p1.X - ww / 2, p1.Y - hh / 2);
            Rectangle rect = new Rectangle(p2, new Size(ww, hh));

            
            gg.DrawRectangle(MyArtifactPen, rect);

            // Show the shape name
            gg.DrawString(ai.Name, MyTextFont, Brushes.Gray, p2);

            SizeF sf = gg.MeasureString("Qwerty", MyTextFont);

            if ( !string.IsNullOrEmpty(ai.TextData))
            {
                p2 = new Point(p2.X, p2.Y + (int)Math.Round(sf.Height));
                gg.DrawString(ai.TextData, MyTextFont, Brushes.Gray, p2);
            }

            if ( !string.IsNullOrEmpty(ai.ForeignData))
            {
                p2 = new Point(p2.X, p2.Y + (int)Math.Round(sf.Height));
                gg.DrawString(ai.TextData, MyTextFont, Brushes.Blue, p2);
            }

        }

    } // picContext

    public class PicVertex
    {
        public PointF PF1;
        public PointF PF2;

    }

    /// <summary>
    /// A POCO for Line of 2 PointF
    /// </summary>
    public class PicLink
    {
        public string Name { get; set; }

        public PointF PF1;
        public PointF PF2;

        public Point P1;
        public Point P2;

        public PicLink(PointF pt1, PointF pt2)
        {
            PF1 = pt1; PF2 = pt2;
        }

        public PointF MidPoint()
        {
            return new PointF((PF1.X + PF2.X) / 2f, (PF1.Y + PF2.Y) / 2f);
        }

        public void Recalculate(PicContext context)
        {
            P1 = context.ConvertFromPointF(this.PF1);
            P2 = context.ConvertFromPointF(this.PF2);

        }

    }

    /// <summary>
    /// An object to display.
    /// It is a Rectangle using float coordinates.
    /// Converted to a Picturebox Rect for display
    /// </summary>
    public class PicObject
    {
        public string Name { get; set; }

        public PointF ULF;
    
        public RectangleF RectF;

        public Rectangle Rect;

        public PicObject( PointF upperLeft, float width, float height)
        {
            RectF = new RectangleF(upperLeft.X, upperLeft.Y, width, height);
        }

        /// <summary>
        /// Called when the picturebox changes size.
        /// </summary>
        public void Recalculate(PicContext context)
        {
            Point pt = context.ConvertFromPointF(ULF);
            int ww = context.ConvertFromXSizeF(RectF.Width);
            int hh = context.ConvertFromYSizeF(RectF.Height);

            Rect = new Rectangle(pt.X, pt.Y, ww, hh);

        }



    }



}
