using SimioAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdxHelpers
{

    [Flags]
    public enum EnumTransformOptions
    {
        AutoScaleObjects = 1,
        CenterAtOffset = 2
    }

    /// <summary>
    /// A class to transform the facility
    /// </summary>
    public class SimioTransform
    {
        public SdxVector DrawingScale { get; set; }

        public SdxVector ObjectScale { get; set; }

        /// <summary>
        /// Where on the Facility to place the incoming drawing
        /// </summary>
        public SdxVector LocationTranslate { get; set; }

        /// <summary>
        /// Flags for transform options
        /// </summary>
        public EnumTransformOptions TransformOptions { get; set; }
            
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="drawingScale"></param>
        /// <param name="objectScale"></param>
        /// <param name="locationTranslate"></param>
        public SimioTransform(SdxVector drawingScale, SdxVector objectScale, SdxVector locationTranslate, EnumTransformOptions options)
        {
            ObjectScale = objectScale;
            DrawingScale = drawingScale;

            LocationTranslate = locationTranslate;

            TransformOptions = options;
        }

        /// <summary>
        /// Scale and then transform a vector.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public SdxVector TransformLocation(SdxVector source)
        {
            SdxVector v3 = source.ScaleBy(DrawingScale);

            v3 = v3.Add(LocationTranslate);
            return v3;
        }

        /// <summary>
        /// Scale a size vector.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public SdxVector TransformSize(SdxVector source)
        {
            SdxVector v3 = source.ScaleBy(ObjectScale);
            return v3;
        }

        /// <summary>
        /// Scale and then transform a vector.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public FacilityLocation TransformFacilityLocation(FacilityLocation loc)
        {
            double xx = loc.X * DrawingScale.X + LocationTranslate.X;
            double yy = loc.Y * DrawingScale.Y + LocationTranslate.Y;
            double zz = loc.Z * DrawingScale.Z + LocationTranslate.Z;

            return new FacilityLocation(xx, yy, zz);
        }

        public override string ToString()
        {
            return $"ObjectScale={ObjectScale}, DrawingScale={DrawingScale}, Translate={LocationTranslate}";
        }
    } // class
} // namespace

