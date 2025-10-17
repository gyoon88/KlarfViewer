
#pragma once

namespace ImaGyNative
{
    // ==========================================
    // --- CUDA �÷� �̹��� ó�� Launcher �Լ� (����) ---
    // ==========================================

    // ����þ� �� (�÷�)
    bool LaunchGaussianBlurColorKernel(unsigned char* pixels, int width, int height, int stride, double sigma, int kernelSize, bool useCircularKernel);

    // ��� ���� (�÷�)
    bool LaunchAverageBlurColorKernel(unsigned char* pixels, int width, int height, int stride, int kernelSize, bool useCircularKernel);

    // ��â (�÷�)
    bool LaunchDilationColorKernel(unsigned char* pixels, int width, int height, int stride, int kernelSize, bool useCircularKernel);

    // ħ�� (�÷�)
    bool LaunchErosionColorKernel(unsigned char* pixels, int width, int height, int stride, int kernelSize, bool useCircularKernel);

    // ������׷� ��Ȱȭ (�÷�)
    bool LaunchEqualizationColorKernel(unsigned char* pixels, int width, int height, int stride);

    // FFT ����Ʈ�� (�÷�)
    bool LaunchFftSpectrumColorKernel(unsigned char* pixels, int width, int height, int stride);
}