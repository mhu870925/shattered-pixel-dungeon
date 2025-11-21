# Unity 포팅 구조 (Korean)

Unity로 이식할 때 사용할 수 있도록 저장소 내부에 `unity/` 트리를 실제로 구성했습니다. 아래 구조와 각 디렉터리 역할을 참고해 바로 탐색하거나, 필요한 파일을 옮겨가며 사용하세요.

```
unity/
├── Assets/                # Unity 프로젝트에서 바로 붙여넣을 기본 Assets 루트
│   ├── Scripts/           # C# 코드 전용 영역 (Core/Systems/Gameplay 하위 섹션 권장)
│   │   └── Ported/Utils/  # com.watabou.utils.* 클래스의 1차 C# 포팅본
│   ├── Art/               # 스프라이트·타일·UI 아트 리소스
│   ├── Audio/             # 사운드/음악 리소스
│   ├── Resources/         # 런타임 로드용 데이터 (JSON, 텍스트 등)
│   └── Scenes/            # 씬 프리팹
└── docs/
    ├── overview-ko.md     # 포팅 과정과 모듈화 전략 개요
    └── folder-map-ko.md   # 기존 Java 패키지 → Unity Scripts 배치 가이드
```

## 바로 보기
- 한국어 개요: [`docs/overview-ko.md`](docs/overview-ko.md)
- 폴더 매핑: [`docs/folder-map-ko.md`](docs/folder-map-ko.md)
- 포팅된 유틸리티 목록: [`docs/ported-utils-ko.md`](docs/ported-utils-ko.md)

## 사용 팁
- `unity/Assets/Scripts/Core`부터 도메인 로직을 정리하고, 렌더/입력/사운드 등 플랫폼 의존 부분은 `Systems`로 분리하세요.
- 원본 Java 소스 위치는 각 문서에 명시되어 있으니, 필요한 클래스만 가져오면서 C#으로 옮기면 됩니다.
- 새로운 리소스를 추가할 때는 Unity가 자동으로 GUID를 생성하므로, 이 트리는 "빈 골격"으로 유지하고 필요 리소스만 채우세요.
- 공통 유틸리티(`Point`, `PointF`, `Rect`, `RectF`, `Random`)는 이미 `Assets/Scripts/Ported/Utils`에 C#으로 변환되어 있으니 직접 참조하거나 확장하면 됩니다.
