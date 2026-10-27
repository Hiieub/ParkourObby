# 🏃 ParkourObby

Game vượt chướng ngại vật parkour 3D

---

<a href="https://hiieu.itch.io/parkourobby">
  <img src="https://img.itch.zone/aW1nLzMxNDI1ODcxLnBuZw==/315x250%23c/oDHdj8.png" alt="Ảnh bìa parkourobby" width="180">
</a>

<br><br>

[![Play on itch.io](https://img.shields.io/badge/▶%20Chơi%20ngay-itch.io-fa5c5c?logo=itch.io&logoColor=white&style=for-the-badge)](https://hiieu.itch.io/parkourobby)
[![Developer](https://img.shields.io/badge/Dev-Hiieu-ff7a1a?logo=itch.io&logoColor=white&style=for-the-badge)](https://hiieu.itch.io)
[![Platform](https://img.shields.io/badge/Nền%20tảng-Web%20%2F%20itch.io-2a2520?style=for-the-badge)](https://hiieu.itch.io/parkourobby)


---

## 🎮 Tổng Quan Gameplay

- Di chuyển qua các chướng ngại vật từ điểm xuất phát đến đích
- Thu thập **coin** rải rác trong màn chơi
- Chạm **checkpoint** để lưu tiến trình và theo dõi giai đoạn
- Tránh các chướng ngại vật nguy hiểm — rơi vào vùng chết sẽ hồi sinh tại checkpoint gần nhất
- Hoàn thành màn chơi để nhận **1–3 sao** dựa trên số lần chết và thời gian hoàn thành
- Mở khóa và trang bị **skin** nhân vật bằng coin thu thập được

---

## ✨ Tính Năng

### 🧍 Di Chuyển Nhân Vật
- **Đi bộ & Chạy nước rút** với di chuyển theo hướng camera
- **nhảy đôi** (Double Jump)
- **Coyote Time** — cho phép nhảy trong chớp nhoáng sau khi rời khỏi mép nền
- **Jump Buffering** — ghi nhận lệnh nhảy ngay trước khi chạm đất
- Hiệu ứng hạt khi chạy và hệ thống animation state machine

### 🏁 Hệ Thống Màn Chơi
- Màn chơi được định nghĩa qua ScriptableObject `LevelConfigSO` và quản lý bởi `LevelDatabaseSO`
- Level được tải lúc runtime dưới dạng prefab — không cần reload scene
- **Xếp hạng sao** được tính riêng cho từng màn dựa trên ngưỡng cấu hình (số lần chết & thời gian)
- Lưu thành tích thời gian tốt nhất theo từng màn

### 🚧 Chướng Ngại Vật
| Chướng ngại vật | Hành vi |
|---|---|
| **Kill Zone** | Hồi sinh người chơi ngay lập tức |
| **Falling Platform** | Biến mất khi bị đứng lên; hồi sinh sau một khoảng thời gian |
| **Bounce Pad** | Bắn người chơi lên cao |
| **Swinging Obstacle** | Chuyển động con lắc; chạm vào sẽ chết |
| **Rotating Platform** | Quay liên tục |

### 🛒 Cửa Hàng & Skin
- Cửa hàng trong game để duyệt và mua skin nhân vật bằng coin
- Lựa chọn skin được lưu qua `PlayerPrefs`
- Cơ sở dữ liệu skin quản lý qua `SkinDatabaseSO`

### 🔊 Âm Thanh
- Nhạc nền (BGM) với chế độ lặp
- Hiệu ứng âm thanh: nhảy, nhặt coin, checkpoint, nảy, hồi sinh, chiến thắng, bấm nút
- Điều chỉnh âm lượng riêng biệt (nhạc / SFX) lưu vào `PlayerPrefs`

### 📱 Điều Khiển
- Bàn phím & chuột (PC) qua Unity Input System
- Hỗ trợ cảm ứng mobile qua **Joystick Pack**
- Vùng cảm ứng xoay camera riêng cho mobile

---

## 🗂️ Cấu Trúc Thư Mục

```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Core/           # GameManager, LevelManager, DataManager, AudioManager, VfxPool
│   │   ├── Player/         # Player controller, Checkpoint, SkinDatabase
│   │   ├── Obstacle/       # BouncePad, FallingPlatform, KillZone, RotatingPlatform, SwingingObstacle
│   │   ├── Collectibles/   # Coin, ICollectable
│   │   ├── Input/          # GameInput, LookTouchZone
│   │   └── UI/             # UIManager, MainMenuController, LevelSelectManager, ShopManager
│   ├── Art/
│   │   ├── Models/         # Mô hình 3D
│   │   ├── Animations/     # Animation clip
│   │   ├── Materials/      # Vật liệu URP
│   │   └── AudioClip/      # File âm thanh BGM và SFX
│   ├── Data/               # ScriptableObject asset (levels, skins)
│   ├── Prefabs/            # Prefab nhân vật, chướng ngại vật, UI, môi trường
│   └── Scenes/
│       ├── MainMenu.unity
│       └── GameScene.unity
├── Joystick Pack/          # Asset joystick cho mobile
└── TextMesh Pro/           # Gói TMP
```

---

## 🏗️ Kiến Trúc

### State Machine
Game sử dụng finite state machine (`GameStateMachine`) với bốn trạng thái:

```
Init → Playing ⇄ Paused
              ↓
           Victory
```

### Luồng Dữ Liệu
- **`DataManager`** — class tĩnh; lưu trữ coin, level/skin đã chọn, sao, thành tích thời gian và cài đặt âm thanh qua `PlayerPrefs`
- **`GameManager`** — singleton; quản lý trạng thái game, đăng ký checkpoint, theo dõi coin/số lần chết và phát sự kiện
- **`LevelManager`** — singleton; tải prefab level lúc runtime và đặt nhân vật tại `StartPos`

### Sự Kiện (C# delegates)
```csharp
GameManager.OnGameStateChanged   // (trạng thái cũ, trạng thái mới)
GameManager.OnProgressUpdated    // (tỉ lệ tiến trình, giai đoạn hiện tại, tổng giai đoạn)
GameManager.OnTimerUpdated       // (thời gian)
GameManager.OnCoinChanged        // (tổng coin)
GameManager.OnLevelWon           // (thời gian, coin, sao)
```

---

## 🔧 Công Nghệ Sử Dụng

| Công nghệ | Phiên bản |
|---|---|
| Unity | 6 (URP 17.3.0) |
| C# | .NET (Unity Managed) |
| Input System | 1.19.0 |
| Cinemachine | 3.1.7 |
| TextMesh Pro | Tích hợp sẵn |
| DOTween | Tích hợp sẵn (Demigiant) |
| Joystick Pack | Tích hợp sẵn |
| Shader Graph | 17.3.0 |

---

## 📋 Tham Chiếu Script Chính

| Script | Vị trí | Mô tả |
|---|---|---|
| [`GameManager`](Assets/_Game/Scripts/Core/GameManager.cs) | Core | Trạng thái game, checkpoint, theo dõi coin/chết/thời gian |
| [`LevelManager`](Assets/_Game/Scripts/Core/LevelManager.cs) | Core | Tải prefab level lúc runtime |
| [`DataManager`](Assets/_Game/Scripts/Core/DataManager.cs) | Core | Lưu trữ dữ liệu qua PlayerPrefs |
| [`AudioManager`](Assets/_Game/Scripts/Core/AudioManager.cs) | Core | Phát BGM và SFX |
| [`Player`](Assets/_Game/Scripts/Player/Player.cs) | Player | Di chuyển, nhảy, quản lý skin |
| [`LevelConfigSO`](Assets/_Game/Scripts/Core/LevelConfigSO.cs) | Core | Cấu hình màn chơi & ngưỡng xếp hạng sao |
| [`ShopManager`](Assets/_Game/Scripts/UI/ShopManager.cs) | UI | Logic mua và chọn skin |

---
