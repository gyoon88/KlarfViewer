using KlarfViewer.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace KlarfViewer.Service
{
    public class FileSystemService
    {
        /// <summary>
        /// 재귀 방식으로 디렉토리 탐색
        /// </summary>
        /// <param name="parentNode"></param>
        public void LoadSubDirectories(FileSystemObjectViewModel parentNode)
        {
            try
            {
                foreach (var dirPath in Directory.GetDirectories(parentNode.FullPath))
                {
                    var subDirNode = new FileSystemObjectViewModel(dirPath, isDirectory: true);
                    LoadSubDirectories(subDirNode);
                    parentNode.Children.Add(subDirNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore folders without access permissions
            }
        }

        /// <summary>
        ///  선택된 디렉토리 내부의 
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        
        public IEnumerable<FileSystemObjectViewModel> GetFiles(string directoryPath)
        {
            string[] filePaths;
            try
            {
                filePaths = Directory.GetFiles(directoryPath);
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore folders without access permissions
                yield break;
            }

            foreach (var filePath in filePaths)
            {
                //
                // mermory efiicient!!
                yield return new FileSystemObjectViewModel(filePath, isDirectory: false);
            }
        }
    }
}
