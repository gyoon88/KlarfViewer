using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.Model
{
    // ListView에 표시될 '파일' 항목을 나타내는 클래스
    public class KlarfFileItem
    {
        // Connect for checkbox in on ui
        public bool IsSelected { get; set; }

        // 'Name' 컬럼과 연결될 속성
        public string Name { get; set; }

        // 'Date' 컬럼과 연결될 속성
        public DateTime ModifiedDate { get; set; }

        // 실제 파일 경로를 저장할 속성
        public string FullPath { get; set; }
    }

}
