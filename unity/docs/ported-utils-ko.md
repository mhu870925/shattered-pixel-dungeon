# 포팅된 공통 유틸리티 (C#)

`Assets/Scripts/Ported/Utils/` 폴더에는 Java 원본 `com.watabou.utils` 패키지의 핵심 유틸리티를 Unity C#으로 옮긴 파일이 들어 있습니다. 그대로 사용하거나, 추가 기능이 필요하면 이 파일들을 확장하세요.

## 포함된 클래스
- **Point.cs** – 정수 2D 좌표. Unity의 `Vector2Int` 변환 헬퍼 포함.
- **PointF.cs** – 부동소수 2D 좌표. 삼각함수·거리 계산 포함.
- **Rect.cs** – 정수 사각형. 중심 계산 시 `Random.Int`를 사용해 원본의 홀/짝 오프셋을 재현.
- **RectF.cs** – 부동소수 사각형.
- **Random.cs** – 스택 기반 RNG. 원본 `pushGenerator()/popGenerator()` API와 가중치 선택·셔플 유틸리티를 그대로 제공합니다.

## 사용 예시
```csharp
using ShatteredPixelDungeon.Utils;

var rect = new Rect(0, 0, 5, 5);
var center = rect.Center();        // 원본 Java 로직과 동일한 방식으로 중심 계산
var randomPoint = Random.OneOf(rect.GetPoints());
var offset = new PointF(1.2f, -0.4f).Normalize();
```

## 원본 경로 매핑
- `com/watabou/utils/Point.java` → `Assets/Scripts/Ported/Utils/Point.cs`
- `com/watabou/utils/PointF.java` → `Assets/Scripts/Ported/Utils/PointF.cs`
- `com/watabou/utils/Rect.java` → `Assets/Scripts/Ported/Utils/Rect.cs`
- `com/watabou/utils/RectF.java` → `Assets/Scripts/Ported/Utils/RectF.cs`
- `com/watabou/utils/Random.java` → `Assets/Scripts/Ported/Utils/Random.cs`
```
포팅 과정에서 플랫폼 종속 호출(Game.reportException 등)은 Unity 로그로 치환했습니다.
``` 
