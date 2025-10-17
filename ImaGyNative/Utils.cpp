#include "pch.h"

#include "ImageProcessingUtils.h"
#include <cmath>
#include <iostream>
#include <vector>
#include <complex>
#include <iomanip> 
#include <numeric>
#include <algorithm>
#include <memory>
#include <omp.h>

const double PI = acos(-1);
using Complex = std::complex<double>;
namespace ImaGyNative
{
    /**
      * @brief �Һ� X�� Ŀ���� ����
      * @param kernelSize Ŀ�� ũ�� (Ȧ��).
      * @return double Ÿ���� 1D ���� Ŀ��.
      */
    std::vector<double> createSobelKernelX(int kernelSize) {
        std::vector<double> kernel(kernelSize * kernelSize);
        int center = kernelSize / 2;
#pragma omp parallel for
        for (int y = 0; y < kernelSize; ++y) {
            for (int x = 0; x < kernelSize; ++x) {
                if (x == center) {
                    kernel[y * kernelSize + x] = 0;
                }
                else {
                    kernel[y * kernelSize + x] = (x - center) / (double)((x - center) * (x - center) + (y - center) * (y - center));
                }
            }
        }
        return kernel;
    }

    /**
     * @brief �Һ� Y�� Ŀ���� ����
     * @param kernelSize Ŀ�� ũ�� (Ȧ��).
     * @return double Ÿ���� 1D ���� Ŀ��.
     */
    std::vector<double> createSobelKernelY(int kernelSize) {
        std::vector<double> kernel(kernelSize * kernelSize);
        int center = kernelSize / 2;

#pragma omp parallel for
        for (int y = 0; y < kernelSize; ++y) {
            for (int x = 0; x < kernelSize; ++x) {
                if (y == center) {
                    kernel[y * kernelSize + x] = 0;
                }
                else {
                    kernel[y * kernelSize + x] = (y - center) / (double)((x - center) * (x - center) + (y - center) * (y - center));
                }
            }
        }
        return kernel;
    }

    /**
     * @brief ���ö�þ� Ŀ���� ����
     * @param kernelSize Ŀ�� ũ�� (Ȧ��).
     * @return double Ÿ���� 1D ���� Ŀ��.
     */
    std::vector<double> createLaplacianKernel(int kernelSize)
    {
        if (kernelSize % 2 == 0) {
            throw std::invalid_argument("Kernel size must be an odd number.");
        }
        std::vector<double> kernel(kernelSize * kernelSize, 1.0);
        int centerIndex = (kernelSize / 2) * kernelSize + (kernelSize / 2);
        kernel[centerIndex] = 1.0 - (kernelSize * kernelSize);
        return kernel;
    }


    std::vector<double> createGaussianKernel(int kernelSize, double sigma, bool isCircular)
    {
        if (kernelSize % 2 == 0) {
            throw std::invalid_argument("Kernel size must be an odd number.");
        }

        std::vector<double> kernel(kernelSize * kernelSize);
        double sum = 0.0;
        int center = kernelSize / 2;
        double radiusSq = center * center;

#pragma omp parallel for reduction(+:sum)
        for (int i = 0; i < kernelSize; ++i) {
            for (int j = 0; j < kernelSize; ++j) {
                int x = j - center;
                int y = i - center;

                if (isCircular && (x * x + y * y) > radiusSq) {
                    kernel[i * kernelSize + j] = 0.0;
                    continue;
                }

                double value = exp(-(x * x + y * y) / (2 * sigma * sigma)) / (2 * PI * sigma * sigma);
                kernel[i * kernelSize + j] = value;
                sum += value;
            }
        }

        if (sum > 0) { // 0���� ������ ���� ����
#pragma omp parallel for
            for (double& val : kernel) {
                val /= sum;
            }
        }
        return kernel;
    }

    /**
    * ��� ���͸� ���� (����) Ŀ�� ���� �Լ�
    */
    std::vector<double> createAverageKernel(int kernelSize, bool isCircular)
    {
        if (kernelSize % 2 == 0) kernelSize++;
        std::vector<double> kernel(kernelSize * kernelSize, 0.0);
        int center = kernelSize / 2;
        double radiusSq = center * center;

#pragma omp parallel for
        for (int i = 0; i < kernelSize; ++i) {
            for (int j = 0; j < kernelSize; ++j) {
                if (isCircular) {
                    int x = j - center;
                    int y = i - center;
                    if ((x * x + y * y) <= radiusSq) {
                        kernel[i * kernelSize + j] = 1.0;
                    }
                }
                else {
                    kernel[i * kernelSize + j] = 1.0;
                }
            }
        }
        return kernel;
    }


    /**
     * ���� �˰������� �Ӱ谪 ����
     */
    int OtsuThreshold(const unsigned char* sourcePixels, int width, int height, int stride)
    {
        // ������׷��� �� �����尡 ���������� ����� �� ��
        std::vector<int> hist(256, 0);

#pragma omp parallel
        {
            std::vector<int> local_hist(256, 0); // �� �����常�� ���� ������׷�
#pragma omp for nowait 
            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    local_hist[sourcePixels[y * stride + x]]++;
                }
            }
#pragma omp critical
            for (int i = 0; i < 256; ++i) {
                hist[i] += local_hist[i];
            }
        }

        long long total = (long long)width * height;
        double sumAll = 0;
        for (int i = 0; i < 256; i++) {
            sumAll += i * hist[i];
        }

        double sumB = 0;
        long long wB = 0;
        long long wF = 0;
        double maxVar = 0;
        int threshold = 0;

        for (int t = 0; t < 256; t++) {
            wB += hist[t];
            if (wB == 0) continue;
            wF = total - wB;
            if (wF == 0) break;
            sumB += (double)(t * hist[t]);
            double mB = sumB / wB;
            double mF = (sumAll - sumB) / wF;
            double varBetween = (double)wB * (double)wF * (mB - mF) * (mB - mF);
            if (varBetween > maxVar) {
                maxVar = varBetween;
                threshold = t;
            }
        }
        return threshold;
    }


    void FFT_1D_Recursive(Complex* data, int N, bool isInverse) {
        if (N <= 1) return;

        // ����Ʈ �����͸� ����Ͽ� �ڵ� �޸� ����
        auto even = std::make_unique<Complex[]>(N / 2);
        auto odd = std::make_unique<Complex[]>(N / 2);

        for (int i = 0; i < N / 2; ++i) {
            even[i] = data[2 * i];
            odd[i] = data[2 * i + 1];
        }

        // �����͸� ������ ���� .get() �޼��� ���
        FFT_1D_Recursive(even.get(), N / 2, isInverse);
        FFT_1D_Recursive(odd.get(), N / 2, isInverse);

        double angleSign = isInverse ? 1.0 : -1.0;

        for (int k = 0; k < N / 2; ++k) {
            double angle = angleSign * 2.0 * PI * k / N;
            // std::complex ������ ���
            Complex twiddle(cos(angle), sin(angle));
            Complex product = twiddle * odd[k];

            data[k] = even[k] + product;
            data[k + N / 2] = even[k] - product;
        }

    }


    /// <summary>
    /// 2D FFT Process Fuction It calls FFT_1D_Recursive
    /// </summary>
    /// <param name="inputPixels">Point: Padded Image data memory Point</param>
    /// <param name="outputSpectrum">Complex: Processed Image data array</param>
    /// <param name="width">Int: Width of Image</param>
    /// <param name="height">Int: Height of Image</param>
    /// <param name="stride">Int: Byte of each pixels</param>
    /// <param name="isInverse">FFT or IFFT option boolian Param</param>
    // isInverse �÷��׸� ���ڷ� �߰�

    // const unsigned char* -> void* �� �����ϰ� ���ο��� ĳ����
    void ApplyFFT2D_CPU(const void* inputPixels, Complex* outputSpectrum, int width, int height, int stride, bool isInverse)
    {
        const unsigned char* pixels = static_cast<const unsigned char*>(inputPixels);
        Complex* tempComplexData = new Complex[width * height];

        // ������ �غ�
#pragma omp parallel for
        for (int y = 0; y < height; ++y) {
            for (int x = 0; x < width; ++x) {
                double grayValue = static_cast<double>(pixels[y * stride + x]);
                tempComplexData[y * width + x] = { grayValue, 0.0 };
            }
        }

        // �� ���� 1D FFT
#pragma omp parallel for

        for (int y = 0; y < height; ++y) {
            FFT_1D_Recursive(&tempComplexData[y * width], width, isInverse);
        }


        // --- �� ���� 1D FFT ���� ---
            // ��� �����尡 ����� ���۸� �� ���� �Ҵ�
        Complex* all_columns_buffer = new Complex[width * height];

#pragma omp parallel for
        for (int x = 0; x < width; ++x) {
            // �� ������� ��ü ���ۿ��� �ڽ��� �۾� ����(�����̽�)�� ���� �����͸� ������
            Complex* tempColumn = &all_columns_buffer[x * height];

            for (int y = 0; y < height; ++y) {
                tempColumn[y] = tempComplexData[y * width + x];
            }
            FFT_1D_Recursive(tempColumn, height, isInverse);
            for (int y = 0; y < height; ++y) {
                tempComplexData[y * width + x] = tempColumn[y];
            }
            // new/delete�� �������� ���� ����
        }
        // ������ ���� �� �� ���� �޸� ����
        delete[] all_columns_buffer;

        // ����ȯ(IFFT)�� ���, ��� �����ϸ�
        if (isInverse) {
            double scale = 1.0 / (width * height);
#pragma omp parallel for
            for (int i = 0; i < width * height; ++i) {
                tempComplexData[i].real *= scale;
                tempComplexData[i].imag *= scale;
            }
        }

        // ���� ��� ����
#pragma omp parallel for

        for (int i = 0; i < width * height; ++i) {
            outputSpectrum[i] = tempComplexData[i];
        }
        delete[] tempComplexData;
    }

    void FFT_Shift2D(Complex* spectrum, int width, int height) {
        int halfWidth = width / 2;
        int halfHeight = height / 2;

        // �� ���� ������ �ϳ��� ��ħ
#pragma omp parallel for
        for (int y = 0; y < halfHeight; ++y) {
            // 1��и� <-> 3��и� ��ȯ
            for (int x = 0; x < halfWidth; ++x) {
                std::swap(spectrum[y * width + x], spectrum[(y + halfHeight) * width + (x + halfWidth)]);
            }
            // 2��и� <-> 4��и� ��ȯ
            for (int x = halfWidth; x < width; ++x) {
                std::swap(spectrum[y * width + x], spectrum[(y + halfHeight) * width + (x - halfWidth)]);
            }
        }
    }

}