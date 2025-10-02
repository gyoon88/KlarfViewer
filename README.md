# **WPF Klarf Review Tool 개발 기능 명세서**

## **1. 문서 개요**

- **프로젝트명**: WPF Klarf Review Tool
- **문서 목적**: 본 문서는 'WPF Klarf Review Tool' 개발에 필요한 시스템 아키텍처, 상세 기능 요구사항, 비기능 요구사항을 정의하는 것을 목적으로 한다. 개발팀은 본 명세서를 바탕으로 전체 시스템의 기능 및 동작 방식을 이해하고, 일관된 방향으로 개발 및 테스트를 진행한다.
- **주요 기술**: **`C#, WPF, .NET Framework, MVVM 디자인 패턴`**

### **2. 시스템 아키텍처**

- **플랫폼**: Windows Desktop Application
- **아키텍처**: **MVVM (Model-View-ViewModel) 패턴**을 채택하여 UI와 비즈니스 로직을 명확하게 분리한다.
    - **Model**: Klarf 파일의 데이터 구조를 정의하는 순수 C# 클래스 집합. (예: **`KlarfData`, `WaferInfo`, `DieInfo`, `DefectInfo`** 등)
    - **View**: XAML로 작성된 사용자 인터페이스(UI) 계층. 각 View는 특정 ViewModel과 1:1로 연결(Data-Binding)되며, 자체적인 코드-비하인드 로직을 최소화한다.
    - **ViewModel**: View에 표시될 데이터와 View에서 실행될 명령(Command)을 포함하는 핵심 로직 계층. Model의 데이터를 가공하여 View에 제공하고, View의 사용자 입력을 받아 비즈니스 로직을 처리한다.
- **주요 서비스 모듈**:
    - **`KlarfParsingService`**: 지정된 경로의 Klarf 파일을 읽고 파싱하여 Model 객체로 변환하는 책임.
    - **`NavigationService`**: ViewModel 간의 통신과 상태(현재 선택된 Defect 등)를 관리하는 중재자 역할.
    - **`FileIOService`**: 파일 시스템 접근(폴더 열기, 파일 목록 읽기 등)을 담당하는 책임.

### **3. 기능 요구사항 (Functional Requirements)**

### **FR-1: 파일 관리 및 조회**

- **FR-1.1 (폴더 열기)**: 사용자는 [Open Folder] 버튼을 통해 로컬 시스템의 특정 폴더를 선택할 수 있어야 한다.
- **FR-1.2 (파일 목록 표시)**: 폴더 선택 시, 해당 폴더 내의 Klarf 파일(`.klarf`)이 **File List Viewer**에 **Tree View 형태**로 표시되어야 한다.
- **FR-1.3 (파일 선택 및 로딩)**: 사용자가 Tree View에서 특정 Klarf 파일을 선택하면, 시스템은 해당 파일을 로드하여 파싱을 시작해야 한다.

### **FR-2: Wafer Map Viewer**

- **FR-2.1 (Wafer Map 렌더링)**: 로드된 Klarf 파일의 **`SampleTestPlan`** 정보를 기반으로 Wafer Map을 시각적으로 **렌더링**해야 한다.
- **FR-2.2 (Die 표시)**: 각 **Die**는 사각형 형태로 그려지며, Wafer 내의 상대적 위치(Grid Index)에 정확하게 배치되어야 한다.
- **FR-2.3 (불량 Die 강조)**: 하나 이상의 Defect를 포함하는 Die는 정상 Die와 명확히 구분되는 색상(예: 빨간색)으로 강조 표시되어야 한다.
- **FR-2.4 (선택 동기화)**: `Defect Info Viewer`에서 특정 **Defect** 선택 시, 해당 **Defect**가 속한 **Die**가 **Wafer Map** 상에서 시각적으로 강조(예: 테두리 색상 변경)되어야 한다.
- **FR-2.5 (Die 클릭 이벤트)**: 사용자가 Wafer Map의 특정 Die를 클릭하면, `Defect Info Viewer`의 리스트가 해당 Die에 포함된 Defect들만 필터링하여 보여주어야 한다.

### **FR-3: Defect Info Viewer**

- **FR-3.1 (불량 리스트 표시)**: 로드된 Klarf 파일의 모든 Defect 정보를 `DataGrid` 컨트롤에 리스트 형태로 출력해야 한다.
- **FR-3.2 (상세 정보 컬럼)**: `DataGrid`는 최소한 `DEFECTID`, `XREL`, `YREL`, `IMAGEID`, `Die Index` 등의 핵심 정보를 컬럼으로 표시해야 한다.
- **FR-3.3 (불량 선택 기능)**: 사용자는 **`DataGrid`**에서 마우스 클릭으로 특정 Defect 항목을 선택할 수 있어야 한다.
- **FR-3.4 (선택 정보 연동)**: 항목 선택 시, 선택된 Defect의 상세 정보가 **`Defect Image Viewer`**와 **`Wafer** **Map Viewer**`에 즉시 반영되어야 한다.
    - **`Defect Image Viewer`**: 해당 Defect의 TIF 이미지로 변경
    - **`Wafer Map Viewer`**: 해당 Defect의 위치 강조
- **FR-3.5 (불량 이동 버튼)**:
    - [▲], [▼] 버튼을 통해 **`DataGrid`**의 Defect 리스트를 위/아래로 순차적으로 탐색할 수 있어야 한다.
    - 현재 선택된 항목의 인덱스와 전체 개수를 **`(현재 인덱스) / (전체 개수)`** 형태로 표시해야 한다. (예: **`1 / 453`**)
- **FR-3.6 (Die 이동 버튼)**:
    - [◀], [▶] 버튼을 통해 **불량이 있는 Die** 간에 이동할 수 있어야 한다.
    - 현재 선택된 Die의 인덱스와 전체 불량 Die 개수를 표시해야 한다. (예: **`1 / 10`**)

### **FR-4: Defect Image Viewer**

- **FR-4.1 (TIF 이미지 표시)**: 선택된 Defect에 해당하는 TIF(.tif) 이미지를 표시해야 한다. WPF의 **`TiffBitmapDecoder`**를 사용하여 이미지를 로드한다.
- **FR-4.2 (이미지 확대/축소)**:
    - 사용자는 **Bar UI(Slider)** 또는 버튼을 통해 **이미지의 배율**을 조절할 수 있어야 한다.
    - **확대/축소**는 현재 화면의 중앙을 기준으로 수행되어야 한다.
- **FR-4.3 (드래그를 이용한 사이즈 측정)**:
    - 사용자가 이미지 위에서 마우스를 클릭하고 드래그하면, 시작점과 끝점을 잇는 직선이 그려져야 한다.
    - 드래그가 끝나면, 해당 직선의 길이(단위: pixel)를 계산하여 화면의 특정 위치에 표시해야 한다.

### **4. 비기능 요구사항 (Non-Functional Requirements)**

- **NFR-1 (성능)**: 10MB 미만의 일반적인 Klarf 파일은 3초 이내에 로딩 및 파싱이 완료되어야 한다. UI의 모든 상호작용(항목 선택, 버튼 클릭 등)은 200ms 이내에 응답하여 끊김 없는 사용자 경험을 제공해야 한다.
- **NFR-2 (안정성)**: 유효하지 않은 형식의 Klarf 파일을 열거나, 이미지 파일이 없는 경우 등의 예외 상황에서 프로그램이 강제 종료되지 않고, 사용자에게 적절한 오류 메시지를 안내해야 한다.
- **NFR-3 (유지보수성)**: ATI 코딩 표준 및 주석 형식을 철저히 준수해야 한다. MVVM 패턴을 통해 각 모듈의 의존성을 최소화하여 기능 수정 및 확장이 용이한 구조로 설계해야 한다.
- **NFR-4 (개발 프로세스)**:
    - **버전 관리**: 모든 소스 코드는 Git을 통해 관리되어야 한다.
    - **커밋 정책**: 작업 내용은 의미 있는 단위로 분할하여 **매일 최소 1회 이상 커밋(Daily Commit)**하는 것을 원칙으로 한다.