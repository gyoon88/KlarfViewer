using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.ViewModel
{
    public class WaferMapViewModel:BaseViewModel
    {

        // wafer 변수
        private WaferInfo waferInfomation;

        // 속성
        public WaferInfo WaferInfomation
        {
            get => waferInfomation;
            set => SetProperty(ref waferInfomation, value);

        }
        // 생성자
        public WaferMapViewModel()
        { 
            
        }
    
        

        // 함수
    }
}
