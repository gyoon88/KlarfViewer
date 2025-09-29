using System.Collections.Generic;
using System.Windows.Media.Effects;

namespace KlarfViewer.Model
{

    /// <summary>
    /// 웨이퍼 위의 칩(Die) 하나를 대표하는 클래스입니다.
    /// </summary>
    public class DieData
    {
        /// <summary>
        /// Die의 그리드 상 X축 위치 (SampleTestPlan에서 가져옴)
        /// </summary>
        public int XIndex { get; set; }

        /// <summary>
        /// Die의 그리드 상 Y축 위치 (SampleTestPlan에서 가져옴)
        /// </summary>
        public int YIndex { get; set; }

        /// <summary>
        /// 이 Die가 불량을 1개 이상 포함하고 있는지 여부입니다.
        /// Wafer Map에서 붉은색으로 표시할지 결정하는 데 사용됩니다.
        /// </summary>
        public bool IsDefective { get; set; }

        /// <summary>
        /// 이 Die에 속한 모든 Defect(결함)들의 리스트입니다.
        /// 파싱이 끝난 후, 전체 Defect 리스트를 순회하며 채워줍니다.
        /// </summary>
        public List<Defect> Defects { get; set; }

        /// <summary>
        /// Die 객체 생성자입니다.
        /// </summary>
        public Die()
        {
            // 객체가 처음 만들어질 때는 불량이 없는 상태로 초기화합니다.
            IsDefective = false;
            Defects = new List<Defect>();
        }
    }
}
