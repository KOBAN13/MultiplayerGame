# MultiplayerGame

Unity-клиент для мультиплеерной игры с лобби, авторизацией, сетевой синхронизацией игроков и боевой сценой.

## Обзор

Проект построен на `Unity 6` и использует:
- `SmartFoxServer 2X` как сетевой backend-клиент
- `VContainer` для DI
- `Addressables` для загрузки конфигов и префабов
- `UniTask` для асинхронных операций
- `Cinemachine` для камер
- `Input System` для ввода
- `R3` для реактивного UI и сервисов

Клиент поддерживает:
- подключение к внешнему Web API для получения адреса игрового сервера
- гостевое и пользовательское подключение через SmartFox
- главное меню, список комнат и лобби
- загрузку игровой сцены
- локального игрока с prediction/reconciliation
- удалённых игроков с snapshot/interpolation
- стрельбу и визуализацию попаданий
- layered animation setup для верхней и нижней части тела

## Требования

- `Unity 6000.0.23f1`
- доступ к интернету для получения server config по URL `https://apisfs.ru:9443/getSfsConfig`
- доступный SmartFoxServer backend, совместимый с текущими командами клиента

Версия Unity указана в [ProjectVersion.txt](/mnt/e/MultiplayerGame/ProjectSettings/ProjectVersion.txt).

## Быстрый старт

1. Открой проект через Unity Hub в версии `6000.0.23f1`.
2. Дождись импорта пакетов и Addressables-конфигов.
3. Убедись, что backend доступен:
   - Web API возвращает параметры `Host`, `Port`, `Zone`
   - SmartFoxServer принимает подключение в `GuestZone` и `GameZone`
4. Открой стартовую сцену проекта.
5. Нажми `Play` в Unity Editor.

Если запускаешь несколько клиентов локально, проект содержит интеграцию с `ParrelSync` в editor-only коде.

## Сцены

Основные сцены лежат в [Assets/Scenes](/mnt/e/MultiplayerGame/Assets/Scenes):
- [InitialScene.unity](/mnt/e/MultiplayerGame/Assets/Scenes/InitialScene.unity) — стартовая точка входа
- [MainMenu.unity](/mnt/e/MultiplayerGame/Assets/Scenes/MainMenu.unity) — главное меню и авторизация
- [LobbyScene.unity](/mnt/e/MultiplayerGame/Assets/Scenes/LobbyScene.unity) — лобби и список игроков
- [GameScene.unity](/mnt/e/MultiplayerGame/Assets/Scenes/GameScene.unity) — игровая сцена
- [LoadingScene.unity](/mnt/e/MultiplayerGame/Assets/Scenes/LoadingScene.unity) — промежуточная загрузка

## Архитектура

### DI и composition root

DI-слои находятся в [Assets/Scripts/Di](/mnt/e/MultiplayerGame/Assets/Scripts/Di):
- [RootLifeTimeScope.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Di/RootLifeTimeScope.cs) — регистрация базовых сервисов, SmartFox, Addressables-конфигов
- [MainMenuTimeScope.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Di/MainMenuTimeScope.cs) — зависимости меню
- [LobbyTimeScope.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Di/LobbyTimeScope.cs) — зависимости лобби
- [GameLifeTimeScope.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Di/GameLifeTimeScope.cs) — зависимости игровой сцены

### Сетевой слой

Основная сетевая логика находится в [Assets/Scripts/Services/Connections](/mnt/e/MultiplayerGame/Assets/Scripts/Services/Connections):
- [ConnectionService.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Services/Connections/ConnectionService.cs) — получает конфиг сервера и подключает SmartFox
- [LoginClientService.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Services/Connections/LoginClientService.cs) — логин пользователя
- [LobbyService.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Services/Connections/LobbyService.cs) — работа с лобби
- [GameHubService.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Services/Connections/GameHubService.cs) — создание/управление комнатами
- [PingService.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Services/Connections/PingService.cs) — отправка ping

Константы команд и server URL лежат в [SFSResponseHelper.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Helpers/SFSResponseHelper.cs).

### Игрок и сетевое движение

Ключевая логика игрока находится в [Assets/Scripts/Player](/mnt/e/MultiplayerGame/Assets/Scripts/Player):
- [LocalPlayerMotor.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Local/LocalPlayerMotor.cs) — локальное управление, prediction, анимация
- [RemotePlayer.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Remote/RemotePlayer.cs) — удалённый игрок, визуализация snapshot-данных
- [NetworkStateReceiver.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Remote/NetworkStateReceiver.cs) — приём серверных состояний игроков
- [InputFrameSender.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Prediction/InputFrameSender.cs) — отправка input/precondition на сервер
- [ReconciliationService.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Services/Prediction/ReconciliationService.cs) — reconciliation локального игрока
- [SnapshotsService.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Services/Snapshot/SnapshotsService.cs) — буфер snapshot-ов и интерполяция

### Оружие

Оружие и FX находятся в [Assets/Scripts/Player/Weapon](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Weapon):
- [SingleShotWeapon.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Weapon/SingleShotWeapon.cs)
- [ShotFxSimulator.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Weapon/Services/ShotFxSimulator.cs)
- [BulletProjectile.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Weapon/Projectile/BulletProjectile.cs)

## Анимации

Animator-контроллер игрока расположен в [Player.controller](/mnt/e/MultiplayerGame/Assets/Asset/Animation/Player.controller).

Связанные assets:
- [Upper.mask](/mnt/e/MultiplayerGame/Assets/Asset/AvatarMask/Upper.mask)
- [Bottom.mask](/mnt/e/MultiplayerGame/Assets/Asset/AvatarMask/Bottom.mask)
- [PlayerAnimationState.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Animation/PlayerAnimationState.cs)
- [AnimatorHash.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Animation/AnimatorHash.cs)

Текущая схема анимации:
- нижний слой отвечает за locomotion
- верхний слой синхронизируется с базовым слоем и ограничивается `AvatarMask`
- параметры `Horizontal`, `Vertical`, `MoveSpeed` обновляются из кода
- события `StartWalkForward`, `StopWalkForward`, `Jump`, `Fire`, `Die` триггерятся кодом

Если меняешь слои аниматора:
- проверь, что веса слоёв не равны `0`
- проверь, что `Upper.mask` и `Bottom.mask` не управляют одними и теми же костями
- проверь, что переходы в Animator используют те же имена параметров, что и в [AnimatorHash.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Animation/AnimatorHash.cs)

## Конфиги

Runtime-конфиги находятся в [Assets/Configs](/mnt/e/MultiplayerGame/Assets/Configs):
- `GameData.asset`
- `PredictionParameters.asset`
- `SnapshotParameters.asset`
- `LocalPlayerParameters.asset`
- `RemotePlayerParameters.asset`
- `WeaponData.asset`
- `CameraParameters.asset`
- `ScreensConfig.asset`

Они загружаются через `Addressables` в `RootLifeTimeScope`.

## Структура проекта

- [Assets/Scripts](/mnt/e/MultiplayerGame/Assets/Scripts) — основной код проекта
- [Assets/Scenes](/mnt/e/MultiplayerGame/Assets/Scenes) — сцены
- [Assets/Configs](/mnt/e/MultiplayerGame/Assets/Configs) — ScriptableObject-конфиги
- [Assets/Asset](/mnt/e/MultiplayerGame/Assets/Asset) — префабы, контроллеры, анимации, UI и арт
- [Assets/AddressableAssetsData](/mnt/e/MultiplayerGame/Assets/AddressableAssetsData) — настройки Addressables
- [Packages](/mnt/e/MultiplayerGame/Packages) — Unity packages
- [ProjectSettings](/mnt/e/MultiplayerGame/ProjectSettings) — настройки проекта

Не редактируй вручную содержимое `Library/`, `Temp/`, `obj/`.

## Сборка

Проект не содержит выделенного CLI build pipeline.

Сборка выполняется стандартными средствами Unity:
1. `File > Build Settings`
2. Выбери target platform
3. Добавь нужные сцены в Build Settings
4. Нажми `Build` или `Build And Run`

## Тестирование

На текущий момент отдельных test assemblies в репозитории нет.

Если будешь добавлять тесты:
- размещай их в `Assets/Tests/` или `Assets/**/Tests/`
- именуй файлы как `*Tests.cs`
- запускай через `Window > General > Test Runner`

## Зависимости

Основные Unity Package Manager зависимости перечислены в [manifest.json](/mnt/e/MultiplayerGame/Packages/manifest.json):
- `com.unity.addressables`
- `com.unity.cinemachine`
- `com.unity.inputsystem`
- `jp.hadashikick.vcontainer`
- `com.cysharp.r3`

Дополнительно в проекте используется NuGet-конфигурация из [NuGet.config](/mnt/e/MultiplayerGame/Assets/NuGet.config).

## Известные особенности

- Клиент зависит от внешнего Web API и SmartFoxServer, поэтому без backend часть сценариев неработоспособна.
- `animationState`, приходящий с сервера, в текущем коде не является единственным источником анимаций; основная анимация движения управляется параметрами Animator из клиента.
- Для корректной работы сетевых анимаций у remote player важно, чтобы snapshot-данные, layer weights и avatar masks были согласованы.

## Рекомендации по разработке

- Держи имена параметров Animator синхронными с [AnimatorHash.cs](/mnt/e/MultiplayerGame/Assets/Scripts/Player/Animation/AnimatorHash.cs).
- При изменениях сетевой логики проверяй обе ветки: `LocalPlayer` prediction и `RemotePlayer` snapshot playback.
- Конфиги, которые должны грузиться на старте, должны быть помечены соответствующим Addressables label.
- Не коммить сгенерированные данные Unity и временные артефакты.
