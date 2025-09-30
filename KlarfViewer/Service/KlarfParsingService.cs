using KlarfViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlarfViewer.Service
{
    public class KlarfParsingService
    {
        // 일단 die, data, klarf data defectinfo wafer info 모두 있어야함
        private DieData dieData;
        private KlarfData klarfData;
        private DefectInfo defectInfo;
        private WaferInfo waferInfo;


        public KlarfParsingService() 
        {
            dieData = new DieData();
            klarfData = new KlarfData();
            defectInfo = new DefectInfo();
            waferInfo = new WaferInfo();
        }    


    }
}
