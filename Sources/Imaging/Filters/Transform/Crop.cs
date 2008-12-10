// AForge Image Processing Library
// AForge.NET framework
//
// Copyright � Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Crop an image.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter crops an image providing a new image, which contains only the specified
    /// rectangle of the original image.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Crop filter = new Crop( new Rectangle( 75, 75, 320, 240 ) );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/crop.jpg" width="320" height="240" />
    /// </remarks>
    /// 
    public class Crop : BaseTransformationFilter
    {
        private Rectangle rect;

        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTransalations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
        {
            get { return formatTransalations; }
        }

        /// <summary>
        /// Rectangle to crop.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return rect; }
            set { rect = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crop"/> class.
        /// </summary>
        /// 
        /// <param name="rect">Rectangle to crop.</param>
        /// 
        public Crop( Rectangle rect )
        {
            this.rect = rect;

            formatTransalations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            formatTransalations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            formatTransalations[PixelFormat.Format32bppArgb]      = PixelFormat.Format32bppArgb;
            formatTransalations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTransalations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
            formatTransalations[PixelFormat.Format64bppArgb]      = PixelFormat.Format64bppArgb;
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// 
        /// <returns>New image size - size of the destination image.</returns>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( UnmanagedImage sourceData )
        {
            return new Size( rect.Width, rect.Height );
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            // destination image dimension
            int xmin = Math.Max( 0, Math.Min( width - 1, rect.Left ) );
            int ymin = Math.Max( 0, Math.Min( height - 1, rect.Top ) );
            int xmax = Math.Min( width - 1, xmin + rect.Width - 1 + ( ( rect.Left < 0 ) ? rect.Left : 0 ) );
            int ymax = Math.Min( height - 1, ymin + rect.Height - 1 + ( ( rect.Top < 0 ) ? rect.Top : 0 ) );
            int dstWidth = xmax - xmin + 1;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;
            int pixelSize = Image.GetPixelFormatSize( sourceData.PixelFormat );
            int copySize  = dstWidth * pixelSize;

            // do the job
            byte* src = (byte*) sourceData.ImageData.ToPointer( ) + ymin * srcStride + xmin * pixelSize;
            byte* dst = (byte*) destinationData.ImageData.ToPointer( );

            if ( rect.Top < 0 )
            {
                dst -= dstStride * rect.Top;
            }
            if ( rect.Left < 0 )
            {
                dst -= pixelSize * rect.Left;
            }

            // for each line
            for ( int y = ymin; y <= ymax; y++ )
            {
                AForge.SystemTools.CopyUnmanagedMemory( dst, src, copySize );
                src += srcStride;
                dst += dstStride;
            }
        }
    }
}