# Shattered Pixel Dungeon 코드 리뷰 & Unity 포팅 가이드 (KR)

이 문서는 Shattered Pixel Dungeon의 모듈 구조를 빠르게 파악하고, Unity로 포팅할 때 도움이 되는 정리 팁을 제공합니다. 원본 프로젝트는 Gradle 기반 멀티플랫폼(LibGDX)으로 구성되어 있으며, 게임 로직은 `core` 모듈에 집중되어 있습니다.

> **신규 구조**: 저장소에 `unity/` 트리를 실제로 추가해 Unity 프로젝트 골격(`Assets/`), 개요(`overview-ko.md`), 폴더 매핑(`folder-map-ko.md`)을 한곳에 모았습니다. 필요한 파일을 옮겨가며 사용할 때 바로 참고하세요.

## 1. 상위 모듈 구조 요약
- `core/`: 게임 로직과 자산이 모두 포함된 모듈. LibGDX/Noosa 기반 렌더링과 업데이트 루프가 여기에서 실행됩니다.
- `android/`, `desktop/`, `ios/`: 플랫폼별 런처와 설정. `core`를 의존해 실행 진입점을 제공하므로, Unity 포팅 시에는 이 모듈들을 Unity 프로젝트의 부트스트랩/플랫폼 설정으로 치환하면 됩니다.

## 2. 핵심 클래스 관점의 코드 리뷰
- `ShatteredPixelDungeon` (`core/.../ShatteredPixelDungeon.java`): Noosa `Game`을 상속해 장면 교체, 오디오 초기화, 플랫폼 추상화를 다룹니다. Unity로 옮길 때는 `Game`을 대체하는 `GameManager` MonoBehaviour를 만들어 **씬 전환·입력·오디오 초기화**를 담당하는 것이 자연스럽습니다.
- `Dungeon` (`core/.../Dungeon.java`): 플레이 상태와 층(레벨) 전환, 제한 드롭 테이블 등을 보관하는 글로벌 상태 컨테이너입니다. 상태 저장/불러오기를 번들(Bundle) 기반으로 처리하므로, Unity에서는 `ScriptableObject + JSON` 조합으로 교체하면 테스트가 쉬워집니다.
- 패키지별 책임:
  - `actors/`: 적/플레이어/버프 등 모든 행위자 로직.
  - `levels/`: 레벨 생성, 특수 방, 보스 방, 층 이동 로직.
  - `items/`: 아이템, 생성기, 장비/소비품 세부 로직.
  - `scenes/`, `ui/`, `windows/`: 씬 전환·UI 위젯·윈도 레이아웃.
  - `sprites/`, `tiles/`: 그래픽 리소스 매핑과 타일 정의.

## 3. Unity로 옮길 때의 정리 제안
### 3.1 파일/폴더 정리 예시 (Unity `Assets/` 하위)
```
Assets/
  Scripts/
    Core/Game/            (GameManager, SceneRouter, SaveService)
    Core/Actors/          (Hero, Mob, Buff, AI 행동트리)
    Core/Levels/          (LevelGenerator, TileMap, Transition)
    Core/Items/           (ItemRegistry, Inventory, LootTables)
    Presentation/UI/      (HUD, Windows, Dialogs)
    Presentation/Rendering(TileSpriteLoader, AnimationClips)
    Systems/Audio/        (MusicService, SFXService)
    Systems/Input/        (InputMapper -> SPDAction 대응)
  ScriptableObjects/
    Items/
    Levels/
    Audio/
  Resources/
    Localization/
    Prefabs/
    Tilesets/
```
- **Core vs Presentation 분리**: 현재 패키지들이 로직과 렌더링을 섞어 사용하므로, Unity에서는 `Core`(순수 로직)와 `Presentation`(렌더링/입력)을 네임스페이스로 분리하면 테스트 가능성이 높아집니다.
- **데이터 중심화**: `Dungeon`에서 관리하는 제한 드롭, 저장 슬롯 등을 `ScriptableObject`로 이전하면 인스펙터에서 조정이 가능해집니다.
- **리소스 로더 일원화**: `Assets.Sounds`, `Assets.Sprites`처럼 자산 정의가 흩어져 있으므로 Unity의 Addressables 또는 `Resources` 경로에 맞춰 `AssetRegistry` 클래스를 두고 통합 관리합니다.

### 3.2 로직 가독성 개선 체크리스트
- **씬/상태 전환**: `ShatteredPixelDungeon`이 장면 리셋·노페이드 전환을 모두 맡고 있으니, Unity에서는 `SceneRouter`를 별도 클래스로 분리하고, 전환 옵션(페이드 여부 등)을 데이터 구조로 분리해 테스트를 단순화합니다.
- **글로벌 상태 최소화**: `Dungeon`의 정적 필드를 `GameSession`(런타임 상태)과 `PersistentSave`(저장 데이터)로 나누면 직렬화·테스트가 명확해집니다.
- **행동/턴 시스템**: `actors` 패키지의 `Actor` 업데이트 루프를 Unity `Update`에 직접 매핑하기보다, `TurnScheduler`(ScriptableObject + MonoBehaviour)로 캡슐화해 시뮬레이션과 렌더링을 분리합니다.
- **UI 계층 분리**: `ui`와 `windows`가 게임 로직을 직접 호출하는 부분을 이벤트 기반(예: `UnityEvent` or `C# event`)으로 바꾸면 의존성이 줄어듭니다.

## 4. 패키지 맵 (빠른 탐색용)
- `core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/actors/...` — 캐릭터, 몬스터, 버프.
- `core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/levels/...` — 층 생성, 방, 보스.
- `core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/items/...` — 아이템과 파생 타입.
- `core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/scenes/...` — 타이틀/게임/설정 등 씬.
- `core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/ui/...` — HUD, 버튼, 슬롯 UI.
- `core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/windows/...` — 팝업/다이얼로그 레이아웃.
- `core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/tiles/...` — 타일셋 정의.
- `core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/sprites/...` — 스프라이트 애니메이션.

## 5. 포팅 워크플로우 제안
1. **엔트리 포인트 치환**: `ShatteredPixelDungeon`의 씬 관리·오디오 초기화를 Unity `GameManager`로 구현하고, 기존 씬 전환 로직을 `SceneRouter` 서비스로 이관합니다.
2. **데이터 계층 분리**: `Dungeon`에서 사용하는 정적 데이터를 `ScriptableObject`로 분리하고, 저장/로드는 JSON 기반으로 대체합니다.
3. **턴 기반 시뮬레이션 독립화**: `Actor` 업데이트 로직을 순수 C# 서비스(`TurnScheduler`)로 구현하고, 렌더링 객체는 MonoBehaviour에서 상태만 구독하도록 만듭니다.
4. **에셋 파이프라인 정리**: 스프라이트/타일 정의를 Addressables 키 또는 `Resources` 경로로 매핑하는 `AssetRegistry`를 만들어, 코드에서 문자열 상수를 줄이고 타입 안전성을 확보합니다.
5. **UI 이벤트化**: UI가 게임 상태를 직접 조작하는 코드를 이벤트 기반으로 재배치해 테스트와 교체를 쉽게 만듭니다.

## 6. 빠른 진입용 체크리스트
- **게임 루프**: `ShatteredPixelDungeon`의 `create()`와 `switchScene()` 흐름을 살펴 Unity `Awake/Start` + 씬 전환으로 맵핑.
- **상태/세이브**: `Dungeon`의 번들 직렬화를 `ScriptableObject + JSON`으로 바꾸는 계획 수립.
- **타일/스프라이트**: `tiles/`와 `sprites/` 패키지를 Unity Tilemap/Animator로 이전하기 위한 매핑 테이블 작성.
- **입력/액션**: `SPDAction`에 해당하는 입력 매핑을 `Input System`의 Action Map으로 설계.
- **테스트 포인트**: 턴 스케줄러, 아이템 드롭 테이블, 레벨 생성기를 우선적으로 단위 테스트.

> 위 정리는 기존 코드의 책임을 유지하면서 Unity 친화적인 구조로 재배치하기 위한 로드맵을 제공합니다. 추가로 필요한 세부 설계(예: 에셋 이름 매핑, 직렬화 포맷)는 실제 포팅 범위에 맞춰 별도 문서로 확장하면 됩니다.
