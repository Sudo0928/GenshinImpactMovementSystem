# 자작 HFSM — 계층형 캐릭터 이동 시스템

Unity에서 캐릭터 이동을 **계층형 상태머신(HFSM)** 으로 직접 설계하고 구현한 학습 프로젝트입니다.
상용 상태머신 라이브러리를 쓰지 않고, 상태 인터페이스와 추상 상태머신부터 직접 만들었습니다.

> **학습 프로젝트입니다.** 원신(Genshin Impact)의 캐릭터 이동 감각 — 걷기에서 달리기와 질주로 이어지고, 점프와 낙하, 착지, 대시가 자연스럽게 섞이는 그 흐름 — 을 재현하며 계층형 상태머신을 익혔습니다. 무에서 독창적으로 설계한 것이 아니라 재현입니다.

- **엔진 / 언어**: Unity · C#
- **개발**: 개인 (전체 설계·구현)

---

## 왜 계층으로 나눴나

이동 상태를 평면으로 나열하면 `Walking`, `Running`, `Sprinting`이 각자 똑같은 중력 처리와 회전 로직을 들고 있게 됩니다. 상태가 늘어날수록 같은 코드가 복사되고, 한 곳을 고치면 나머지를 빠뜨립니다.

그래서 **공통 로직은 상위 상태에 모으고, 하위 상태는 차이만 재정의**하도록 계층을 만들었습니다.

```
PlayerMovementState (최상위 · 입력 처리, 이동·회전 공통 로직)
├── PlayerGroundedState (지면 판정, 경사 부양)
│   ├── PlayerMovingState  → Walking / Running / Sprinting
│   ├── PlayerStoppingState → LightStopping / MediumStopping / HardStopping
│   ├── PlayerLandingState  → LightLanding / Rolling / HardLanding
│   ├── PlayerIdlingState
│   └── PlayerDashingState
└── PlayerAirborneState (공중 판정, 낙하)
    ├── PlayerJumpingState
    └── PlayerFallingState
```

- 상태 클래스 **19개** (추상·중간 계층 6개 + 구체 상태 13개)
- 상속 깊이 최대 **4단계** (`PlayerMovementState → PlayerGroundedState → PlayerMovingState → PlayerRunningState`)
- 머신이 실제로 인스턴스화하는 구체 상태는 **13개**입니다

## 구조에서 신경 쓴 것

**상태 인스턴스는 미리 만들어 재사용합니다.** `PlayerMovementStateMachine` 생성자에서 모든 상태를 한 번 생성해 두고, 전이할 때는 그 인스턴스를 바꿔 끼웁니다. 상태가 바뀔 때마다 `new`를 부르지 않으므로 GC 부담이 없습니다.

**데이터와 로직을 분리했습니다.** 상태 사이에 공유되는 런타임 값은 `PlayerStateReusableData`에, 걷기 속도나 대시 쿨다운 같은 정적 튜닝 값은 `ScriptableObject`(`PlayerSO`)에 두었습니다. 밸런스를 만질 때 코드를 건드리지 않아도 됩니다.

**입력·게임 로직·물리 루프를 나눴습니다.** `HandleInput`, `Update`, `PhysicsUpdate`를 상태 인터페이스에서부터 분리해, 물리 계산이 프레임 루프에 섞이지 않게 했습니다.

## 폴더 구성

```
Scripts/Characters/Player/
├── Player.cs                     플레이어 진입점
├── Data/                         상태별 튜닝 데이터 (ScriptableObject)
│   ├── ScriptableObjects/        PlayerSO
│   └── States/                   Walk·Run·Sprint·Dash·Jump·Fall·Roll 등
└── StateMachines/Movement/
    ├── PlayerMovementStateMachine.cs
    └── State/                    위 계층도의 상태 클래스들
```

## 이어진 곳

여기서 익힌 계층 상태머신 패턴을 이후 **ProjectRAID**(7인 팀 멀티플레이 헌팅 액션 RPG, GStar 2025 출품)의 네트워크 예측 플레이어 컨트롤러에 응용했습니다.

포트폴리오: https://github.com/Sudo0928/portfolio
