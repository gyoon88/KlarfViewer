#pragma once

#include <vector>

namespace ImaGyNative
{
	std::vector<double> createSobelKernelX(int kernelSize);
	std::vector<double> createSobelKernelY(int kernelSize);
	std::vector<double> createLaplacianKernel(int kernelSize);
	std::vector<double> createGaussianKernel(int kernelSize, double sigma, bool isCircular);
	std::vector<double> createAverageKernel(int kernelSize, bool isCircular);
	int OtsuThreshold(const unsigned char* sourcePixels, int width, int height, int stride);

    struct Complex {
        double real;
        double imag;
    };

    // ����
    inline Complex operator+(const Complex& a, const Complex& b) {
        return { a.real + b.real, a.imag + b.imag };
    }

    // ����
    inline Complex operator-(const Complex& a, const Complex& b) {
        return { a.real - b.real, a.imag - b.imag };
    }

    // ���� (Complex * Complex)
    inline Complex operator*(const Complex& a, const Complex& b) {
        return {
            a.real * b.real - a.imag * b.imag,
            a.real * b.imag + a.imag * b.real
        };
    }

    inline void swap(Complex& a, Complex& b) noexcept
    {
        // ���� ������� �⺻ Ÿ��(double)�̹Ƿ� std::swap�� �״�� ����մϴ�.
        std::swap(a.real, b.real);
        std::swap(a.imag, b.imag);
    }
    /// <summary>
    /// Complex Ÿ���� ��Į��(�Ǽ�) ������ ������ �������Դϴ�.
    /// </summary>
    inline Complex operator/(const Complex& a, double scalar) {
        if (scalar == 0) return { 0, 0 }; // 0���� ������ ��� ���� ó��
        return { a.real / scalar, a.imag / scalar };
    }

    /// <summary>
    /// /= ���� ���� �������Դϴ�.
    /// </summary>
    inline void operator/=(Complex& a, double scalar) {
        if (scalar == 0) {
            a.real = 0;
            a.imag = 0;
        }
        else {
            a.real /= scalar;
            a.imag /= scalar;
        }
    }

    // ���� (Complex * double) - IFFT �����ϸ��� �ʿ�
    inline Complex operator*(const Complex& a, double scalar) {
        return { a.real * scalar, a.imag * scalar };
    }
    void FFT_1D_Iterative(Complex* data, int N, bool isInverse);

    void FFT_1D_Recursive(Complex* data, int N, bool isInverse);
    void ApplyFFT2D_CPU(const void* inputPixels, Complex* outputSpectrum, int width, int height, int stride, bool isInverse);
    void FFT_Shift2D(Complex* spectrum, int width, int height);


    // Clustering

}
