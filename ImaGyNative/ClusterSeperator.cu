//#include "pch.h"
//#include "CudaKernel.cuh"
//#include <cuda_runtime.h>
//#include <device_launch_parameters.h>
//#include <vector>
//#include <numeric>
//#include <algorithm>
//#include <cmath>
//#include <cufft.h> // For using cuFFT  
//#include <random>
//#include <limits>
//
//namespace ImaGyNative {
//
//	//==========================================
//	//-- - K - Means Clustering Ŀ�� ����-- -
//	//==========================================
//	struct Point5D {
//		float r, g, b, x, y;
//	};
//#define CUDA_CHECK(err_code) do { cudaError_t _err = (err_code); if (_err != cudaSuccess) { return false; } } while (0)
//	// --- NEW: cuFFT vailable state macro
//#define CUFFT_CHECK(err_code) do { cufftResult _err = (err_code); if (_err != CUFFT_SUCCESS) { return false; } } while (0)
//
////�Ҵ� �ܰ� : �� �ȼ��� ���� ����� ��ǥ���� �Ҵ��ϴ� Ŀ��
//	__global__ void KMeansAssignmentKernel(const Point5D* normalizedPixels, const Point5D* centroids, int* assignments, int numPixels, int k) {
//		int i = blockIdx.x * blockDim.x + threadIdx.x;
//		if (i >= numPixels) return;
//
//		float minDistSq = 1e10f; // ����� ū ������ �ʱ�ȭ
//		int bestCluster = 0;
//
//		for (int c = 0; c < k; ++c) {
//			float dr = normalizedPixels[i].r - centroids[c].r;
//			float dg = normalizedPixels[i].g - centroids[c].g;
//			float db = normalizedPixels[i].b - centroids[c].b;
//			float dx = normalizedPixels[i].x - centroids[c].x;
//			float dy = normalizedPixels[i].y - centroids[c].y;
//			float distSq = dr * dr + dg * dg + db * db + dx * dx + dy * dy;
//
//			if (distSq < minDistSq) {
//				minDistSq = distSq;
//				bestCluster = c;
//			}
//		}
//		assignments[i] = bestCluster;
//	}
//	//������Ʈ �ܰ� : �� Ŭ�������� �հ�� ī��Ʈ�� ����ϴ� Ŀ��
//	__global__ void KMeansUpdateKernel(const Point5D* normalizedPixels, const int* assignments,
//		Point5D* newCentroids, int* counts, int numPixels) {
//		int i = blockIdx.x * blockDim.x + threadIdx.x;
//		if (i >= numPixels) return;
//
//		int clusterId = assignments[i];
//
//		// atomicAdd�� ����Ͽ� ���� �����尡 ���ÿ� �����ϰ� ���� ���� �� �ֵ��� ��
//		atomicAdd(&newCentroids[clusterId].r, normalizedPixels[i].r);
//		atomicAdd(&newCentroids[clusterId].g, normalizedPixels[i].g);
//		atomicAdd(&newCentroids[clusterId].b, normalizedPixels[i].b);
//		atomicAdd(&newCentroids[clusterId].x, normalizedPixels[i].x);
//		atomicAdd(&newCentroids[clusterId].y, normalizedPixels[i].y);
//		atomicAdd(&counts[clusterId], 1);
//	}
//
//	// ���� �ܰ� : �ջ�� ���� �������� ���ο� ��ǥ���� ����� ����ϴ� Ŀ��
//	__global__ void KMeansFinalizeCentroidsKernel(Point5D* centroids, const Point5D* newCentroids,
//		const int* counts, int k) {
//		int c = blockIdx.x * blockDim.x + threadIdx.x;
//		if (c >= k) return;
//
//		if (counts[c] > 0) {
//			centroids[c].r = newCentroids[c].r / counts[c];
//			centroids[c].g = newCentroids[c].g / counts[c];
//			centroids[c].b = newCentroids[c].b / counts[c];
//			centroids[c].x = newCentroids[c].x / counts[c];
//			centroids[c].y = newCentroids[c].y / counts[c];
//		}
//	}
//
//	// ���� ������ : �� �ȼ��� ���� ��ǥ �������� ĥ�ϴ� Ŀ��
//	__global__ void KMeansRenderKernel(unsigned char* pixels, const int* assignments, const Point5D* centroids, int width, int height, int stride) {
//		int x = blockIdx.x * blockDim.x + threadIdx.x;
//		int y = blockIdx.y * blockDim.y + threadIdx.y;
//
//		if (x >= width || y >= height) return;
//
//		int clusterId = assignments[y * width + x];
//		unsigned char* p = pixels + y * stride + x * 4;
//
//		p[2] = (unsigned char)(centroids[clusterId].r * 255.0f); // R
//		p[1] = (unsigned char)(centroids[clusterId].g * 255.0f); // G
//		p[0] = (unsigned char)(centroids[clusterId].b * 255.0f); // B
//	}
//
//	// --- K-Means CUDA Launcher �Լ� ---
//	bool LaunchKMeansKernel(void* pixels, int width, int height, int stride, int k, int iteration) {
//		unsigned char* h_pixels = static_cast<unsigned char*>(pixels);
//		int numPixels = width * height;
//
//		// GPU �޸� �Ҵ�
//		unsigned char* d_pixels;
//		Point5D* d_normalizedPixels;
//		Point5D* d_centroids;
//		int* d_assignments;
//		Point5D* d_newCentroids;
//		int* d_counts;
//
//		CUDA_CHECK(cudaMalloc(&d_pixels, (size_t)height * stride));
//		CUDA_CHECK(cudaMalloc(&d_normalizedPixels, (size_t)numPixels * sizeof(Point5D)));
//		CUDA_CHECK(cudaMalloc(&d_centroids, (size_t)k * sizeof(Point5D)));
//		CUDA_CHECK(cudaMalloc(&d_assignments, (size_t)numPixels * sizeof(int)));
//		CUDA_CHECK(cudaMalloc(&d_newCentroids, (size_t)k * sizeof(Point5D)));
//		CUDA_CHECK(cudaMalloc(&d_counts, (size_t)k * sizeof(int)));
//
//
//		// ������ �غ�: CPU���� ����ȭ �� GPU�� ����
//		std::vector<Point5D> h_normalizedPixels(numPixels);
//		double w_minus_1 = width > 1 ? (double)(width - 1) : 1.0;
//		double h_minus_1 = height > 1 ? (double)(height - 1) : 1.0;
//
//#pragma omp parallel for
//		for (int y = 0; y < height; ++y) {
//			for (int x = 0; x < width; ++x) {
//				unsigned char* p = h_pixels + y * stride + x * 4;
//				h_normalizedPixels[y * width + x] = {
//					(float)(p[2] / 255.0), (float)(p[1] / 255.0), (float)(p[0] / 255.0),
//					(float)(x / w_minus_1), (float)(y / h_minus_1)
//				};
//			}
//		}
//		CUDA_CHECK(cudaMemcpy(d_normalizedPixels, h_normalizedPixels.data(), (size_t)numPixels * sizeof(Point5D), cudaMemcpyHostToDevice));
//
//		// �ʱ� ��ǥ�� ���� �� GPU�� ����
//		std::vector<Point5D> h_centroids(k);
//		std::mt19937 rng(std::random_device{}());
//		std::uniform_int_distribution<int> dist(0, numPixels - 1);
//		for (int i = 0; i < k; ++i) {
//			h_centroids[i] = h_normalizedPixels[dist(rng)];
//		}
//		CUDA_CHECK(cudaMemcpy(d_centroids, h_centroids.data(), (size_t)k * sizeof(Point5D), cudaMemcpyHostToDevice));
//
//		// �׸��� �� ��� ����
//
//		dim3 block1D_pixels(256);
//		dim3 grid1D_pixels((numPixels + block1D_pixels.x - 1) / block1D_pixels.x);
//
//		dim3 block1D_k(256);
//		dim3 grid1D_k((k + block1D_k.x - 1) / block1D_k.x);
//
//		// K-Means �ݺ� (������ �ٽ� ����)
//		for (int i = 0; i < iteration; ++i) {
//			// 1. �Ҵ� �ܰ� Ŀ�� ���� (������ ����)
//			KMeansAssignmentKernel << <grid1D_pixels, block1D_pixels >> > (d_normalizedPixels, d_centroids, d_assignments, numPixels, k);
//			CUDA_CHECK(cudaGetLastError());
//
//			// GPU �޸� �ʱ�ȭ
//			CUDA_CHECK(cudaMemset(d_newCentroids, 0, (size_t)k * sizeof(Point5D)));
//			CUDA_CHECK(cudaMemset(d_counts, 0, (size_t)k * sizeof(int)));
//
//			// 2. ������Ʈ �ܰ� Ŀ�� ���� (CPU ��� ��ü)
//			KMeansUpdateKernel << <grid1D_pixels, block1D_pixels >> > (d_normalizedPixels, d_assignments, d_newCentroids, d_counts, numPixels);
//			CUDA_CHECK(cudaGetLastError());
//
//			// 3. ���� ��ǥ�� ��� Ŀ�� ����
//			KMeansFinalizeCentroidsKernel << <grid1D_k, block1D_k >> > (d_centroids, d_newCentroids, d_counts, k);
//			CUDA_CHECK(cudaGetLastError());
//		}
//
//		// ���� ������
//		CUDA_CHECK(cudaMemcpy(d_pixels, h_pixels, (size_t)height * stride, cudaMemcpyHostToDevice)); // ���� �̹��� ������ ����
//		dim3 grid2D((width + 15) / 16, (height + 15) / 16);
//		dim3 block2D(16, 16);
//		KMeansRenderKernel << <grid2D, block2D >> > (d_pixels, d_assignments, d_centroids, width, height, stride);
//		CUDA_CHECK(cudaGetLastError());
//		CUDA_CHECK(cudaDeviceSynchronize());
//
//		// ���� ����� ȣ��Ʈ �޸𸮷� ����
//		CUDA_CHECK(cudaMemcpy(pixels, d_pixels, (size_t)height * stride, cudaMemcpyDeviceToHost));
//
//		// GPU �޸� ����
//		cudaFree(d_pixels);
//		cudaFree(d_normalizedPixels);
//		cudaFree(d_centroids);
//		cudaFree(d_assignments);
//		cudaFree(d_newCentroids);
//		cudaFree(d_counts);
//
//		return true;
//	}
//}