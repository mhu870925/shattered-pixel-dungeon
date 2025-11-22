# Unity 폴더 매핑 (Korean)

실제 Unity 프로젝트 안에서 바로 쓸 수 있도록 추천 폴더와 원본 Java 위치를 1:1로 매핑했습니다. 기존 구조를 그대로 복사·붙여넣기보다, 아래 표대로 필요한 스크립트를 나누어 두면 로직/플랫폼 코드가 깔끔히 분리됩니다.

## 추천 구조
```
Assets/
├── Scripts/
│   ├── Core/             # 게임 규칙·엔티티·데이터 (플랫폼 비의존)
│   ├── Systems/          # 렌더링, 입력, 오디오 등 플랫폼 의존 계층
│   ├── Gameplay/         # UI 흐름, 게임 진행 상태 관리
│   └── Shared/           # 공용 유틸, 서비스 로케이터, DI 바인딩
├── Art/
├── Audio/
├── Resources/
└── Scenes/
```

## Java → Unity 매핑
| Unity 위치 | 원본 Java 패키지 | 설명 |
| --- | --- | --- |
| `Assets/Scripts/Core/dungeon/` | `core/src/main/java/com/shatteredpixel/shatteredpixeldungeon` (모듈 로직) | 던전/캐릭터/아이템/스킬 등 게임 규칙. DTO와 순수 로직만 옮기고 렌더링 호출은 제거합니다. |
| `Assets/Scripts/Core/engine/` | `SPD-classes/src/main/java/com/watabou` (noosa, glwrap 등) | 엔진 레벨 추상화. Unity에서는 `ScriptableObject`/`MonoBehaviour` 없는 순수 서비스 인터페이스로 재구성하세요. |
| `Assets/Scripts/Systems/rendering/` | `core/.../scenes`, `SPD-classes/.../noosa`, `.../glscripts` | 렌더링 파이프라인. Unity에서는 카메라, 포스트 FX, 셰이더 래퍼를 이 폴더로 이동합니다. |
| `Assets/Scripts/Systems/input/` | `SPD-classes/.../input` | 입력 처리. Unity Input System에 맞춰 어댑터 클래스를 배치합니다. |
| `Assets/Scripts/Systems/audio/` | `core/.../sounds` + `SPD-classes/.../utils` 일부 | 사운드/음악 재생 래퍼. Unity의 `AudioSource` 기반으로 교체합니다. |
| `Assets/Scripts/Gameplay/ui/` | `core/.../ui` | UI 로직 및 HUD. Unity UI(Canvas)용 Presenter/Controller로 분리해 둡니다. |
| `Assets/Scripts/Gameplay/progression/` | `core/.../scenes/JournalScene`, `.../statistics` | 게임 진행과 프로그레션 관리. 세이브/로드, 진행도 UI를 묶습니다. |
| `Assets/Scripts/Shared/util/` | `SPD-classes/.../utils` | 범용 유틸리티. C#에서 공용 확장 메서드/헬퍼로 변환합니다. |
| `Assets/Resources/Data/` | `core/src/main/resources` | JSON/텍스트/타일 데이터. 리소스 직렬화 포맷을 그대로 두고 Unity `Resources`로 이동합니다. |

## 가져올 때 팁
- 먼저 `Core`를 옮겨 순수 로직이 잘 돌아가는지 **플랫폼 독립 단위 테스트**로 검증한 뒤, `Systems`를 Unity API로 대체하세요.
- 패키지별 세부 클래스 목록은 필요할 때마다 `rg "package com.shatteredpixel" core/src/main/java` 같은 검색으로 빠르게 찾을 수 있습니다.
- `Shared` 폴더는 의존성 주입 컨테이너나 전역 설정을 묶어두면, 나중에 모바일/데스크톱 빌드 설정을 전환하기 쉽습니다.
