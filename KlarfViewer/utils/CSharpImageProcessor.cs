
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KlarfViewer.utils
{
    public static class CSharpImageProcessor
    {
        // =================================================================
        // Histogram
        // =================================================================

        public static (int[] R, int[] G, int[] B) CalculateColorHistograms(BitmapSource imageSource)
        {
            if (imageSource.Format != PixelFormats.Bgra32)
            {
                imageSource = new FormatConvertedBitmap(imageSource, PixelFormats.Bgra32, null, 0);
            }

            int width = imageSource.PixelWidth;
            int height = imageSource.PixelHeight;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            imageSource.CopyPixels(pixels, stride, 0);

            int[] rHist = new int[256];
            int[] gHist = new int[256];
            int[] bHist = new int[256];

            for (int i = 0; i < pixels.Length; i += 4)
            {
                bHist[pixels[i]]++;
                gHist[pixels[i + 1]]++;
                rHist[pixels[i + 2]]++;
            }

            return (rHist, gHist, bHist);
        }

        public static int[] CalculateGrayscaleHistogram(BitmapSource imageSource)
        {
            if (imageSource.Format != PixelFormats.Gray8)
            {
                imageSource = new FormatConvertedBitmap(imageSource, PixelFormats.Gray8, null, 0);
            }

            int width = imageSource.PixelWidth;
            int height = imageSource.PixelHeight;
            int stride = width;
            byte[] pixels = new byte[height * stride];
            imageSource.CopyPixels(pixels, stride, 0);

            int[] histogram = new int[256];
            foreach (byte pixel in pixels)
            {
                histogram[pixel]++;
            }

            return histogram;
        }

        // =================================================================
        // Brightness & Contrast
        // =================================================================

        public static BitmapSource ApplyBrightnessContrast(BitmapSource source, int brightness, double contrast)
        {
            if (source.Format != PixelFormats.Bgra32)
            {
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
            }

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            source.CopyPixels(pixels, stride, 0);

            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;

            for (int i = 0; i < pixels.Length; i += 4)
            {
                for (int c = 0; c < 3; c++) // B, G, R
                {
                    double pixel = pixels[i + c];
                    pixel = ((pixel / 255.0 - 0.5) * contrast + 0.5) * 255.0;
                    pixel += brightness;
                    pixel = Math.Max(0, Math.Min(255, pixel));
                    pixels[i + c] = (byte)pixel;
                }
            }

            var result = BitmapSource.Create(width, height, source.DpiX, source.DpiY, source.Format, null, pixels, stride);
            result.Freeze();
            return result;
        }

        // =================================================================
        // --- Gaussian Blur ---
        // =================================================================

        public static BitmapSource ApplyGaussianBlur(BitmapSource source, int radius)
        {
            if (source.Format != PixelFormats.Bgra32)
            {
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
            }
            
            int width = source.PixelWidth;
            int height = source.PixelHeight;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            source.CopyPixels(pixels, stride, 0);

            byte[] resultPixels = new byte[pixels.Length];
            
            double[,] kernel = CreateGaussianKernel(radius);
            int kernelSize = radius * 2 + 1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double rSum = 0, gSum = 0, bSum = 0, kSum = 0;

                    for (int ky = -radius; ky <= radius; ky++)
                    {
                        for (int kx = -radius; kx <= radius; kx++)
                        {
                            int pixelY = y + ky;
                            int pixelX = x + kx;

                            if (pixelY >= 0 && pixelY < height && pixelX >= 0 && pixelX < width)
                            {
                                double kVal = kernel[ky + radius, kx + radius];
                                int index = (pixelY * stride) + (pixelX * 4);

                                bSum += pixels[index] * kVal;
                                gSum += pixels[index + 1] * kVal;
                                rSum += pixels[index + 2] * kVal;
                                kSum += kVal;
                            }
                        }
                    }

                    int resultIndex = (y * stride) + (x * 4);
                    resultPixels[resultIndex] = (byte)(bSum / kSum);
                    resultPixels[resultIndex + 1] = (byte)(gSum / kSum);
                    resultPixels[resultIndex + 2] = (byte)(rSum / kSum);
                    resultPixels[resultIndex + 3] = pixels[resultIndex + 3]; // Alpha
                }
            }

            var result = BitmapSource.Create(width, height, source.DpiX, source.DpiY, source.Format, null, resultPixels, stride);
            result.Freeze();
            return result;
        }

        private static double[,] CreateGaussianKernel(int radius)
        {
            int size = radius * 2 + 1;
            double[,] kernel = new double[size, size];
            double sigma = radius / 2.0;
            double s2 = 2 * sigma * sigma;
            double sum = 0;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    double r = Math.Sqrt(x * x + y * y);
                    double val = (Math.Exp(-(r * r) / s2)) / (Math.PI * s2);
                    kernel[y + radius, x + radius] = val;
                    sum += val;
                }
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    kernel[y, x] /= sum;
                }
            }

            return kernel;
        }
    }
}
