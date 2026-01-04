# Hướng Dẫn Setup và Training ML-Agents cho Match3 Game

## 📋 Mục Lục

1. [Yêu Cầu Hệ Thống](#yêu-cầu-hệ-thống)
2. [Cài Đặt Python và ML-Agents](#cài-đặt-python-và-ml-agents)
3. [Setup Unity Scene](#setup-unity-scene)
4. [Cấu Hình Training](#cấu-hình-training)
5. [Chạy Training](#chạy-training)
6. [Theo Dõi Training với TensorBoard](#theo-dõi-training-với-tensorboard)
7. [Sử Dụng Model Đã Train](#sử-dụng-model-đã-train)
8. [Troubleshooting](#troubleshooting)

---

## 🔧 Yêu Cầu Hệ Thống

### Phần Mềm Cần Thiết:

- **Unity 2021.3 LTS** hoặc mới hơn
- **Python 3.8 - 3.10** (không dùng Python 3.11+)
- **ML-Agents Package** (đã cài trong Unity)
- **TensorBoard** (để xem metrics)

---

## 🐍 Cài Đặt Python và ML-Agents

### Bước 1: Cài Python

1. Tải Python từ [python.org](https://www.python.org/downloads/)
2. Chọn Python 3.8, 3.9, hoặc 3.10
3. **Quan trọng**: Khi cài, tick vào "Add Python to PATH"

### Bước 2: Cài ML-Agents Python Package

Mở Terminal/PowerShell và chạy:

```bash
# Tạo virtual environment (khuyến nghị)
python -m venv ml-agents-env

# Kích hoạt virtual environment
# Windows:
ml-agents-env\Scripts\activate
# Mac/Linux:
source ml-agents-env/bin/activate

# Cài ML-Agents
pip install mlagents
```

### Bước 3: Kiểm Tra Cài Đặt

```bash
mlagents-learn --help
```

Nếu thấy help message, cài đặt thành công! ✅

---

## 🎮 Setup Unity Scene

### Bước 1: Kiểm Tra Scene

1. Mở scene `GameScene` trong Unity
2. Đảm bảo có GameObject `MLAgents` trong scene

### Bước 2: Setup Behavior Parameters

1. Chọn GameObject `MLAgents`
2. Trong Inspector, tìm component **Behavior Parameters**:
   - **Behavior Name**: `Match3` (phải khớp với config YAML)
   - **Vector Observation**:
     - **Space Size**: Sẽ tự động detect (259 cho grid 8x8)
     - **Stacked Vectors**: `1`
   - **Actions**:
     - **Continuous Actions**: `0`
     - **Discrete Branches**: `4`
     - **Branch 0 Size**: `8` (xDim)
     - **Branch 1 Size**: `8` (yDim)
     - **Branch 2 Size**: `8` (xDim)
     - **Branch 3 Size**: `8` (yDim)
   - **Model**: `None (NN Model)` (sẽ tự động load khi training)
   - **Behavior Type**: `Default`
   - **Team Id**: `0`
   - **Use Child Sensors**: ✅ Checked

### Bước 3: Setup MatchThreeAgent Script

Trong component **Match Three Agent (Script)**:

- **Game References**:

  - ✅ **Game Grid**: Kéo `ChessBoard (Grid)` vào
  - ✅ **Game Play Controller**: Kéo `GamePlayController` vào
  - ✅ **Enemy Character**: Kéo `DemonImage (Enemy Character)` vào
  - ✅ **Player Character**: Kéo `HeroImage (Hero Character)` vào
  - ✅ **Time Controller**: Kéo `TimeController` vào

- **Training Optimizations**:
  - ✅ **Is Training Mode**: **CHECKED** (quan trọng!)
  - **Training Move Interval**: `0.05`
  - **Normal Move Interval**: `1`

### Bước 4: Setup Decision Requester

Trong component **Decision Requester**:

- **Decision Period**: `5` (agent sẽ quyết định mỗi 5 frames)
- ✅ **Take Actions Between Decisions**: Checked

### Bước 5: Setup GamePlayController

1. Chọn GameObject có `GamePlayController`
2. Tìm field **Is Training Mode** trong Inspector
3. ✅ **CHECKED** để bật training mode

### Bước 6: Kiểm Tra Academy

1. Đảm bảo có GameObject `Academy` trong scene
2. Nếu không có, tạo mới:
   - GameObject → Create Empty → Đặt tên `Academy`
   - Add Component → `Academy`
   - **Quan trọng**: Academy phải có trong scene!

---

## ⚙️ Cấu Hình Training

### File Config: `config/match3_config.yaml`

File config đã được setup sẵn với các thông số tối ưu:

```yaml
behaviors:
  Match3: # ← Phải khớp với Behavior Name trong Unity
    trainer_type: ppo
    hyperparameters:
      batch_size: 1024
      buffer_size: 10240
      learning_rate: 0.0003
      # ... (các thông số khác)
    max_steps: 500000
    time_horizon: 64
    summary_freq: 1000

env_settings:
  base_port: 5004 # ← Port để Unity kết nối

checkpoint_settings:
  run_id: Match3_Training
  results_dir: results
```

**Lưu ý quan trọng:**

- `Behavior Name` trong Unity phải khớp với `behaviors:` key trong YAML
- `base_port` phải khớp với port Unity đang listen (mặc định 5004)

---

## 🚀 Chạy Training

### Bước 1: Mở Unity và Scene

1. Mở Unity Editor
2. Mở scene `GameScene`
3. **Đảm bảo**:
   - ✅ `Is Training Mode` = checked trong GamePlayController
   - ✅ `Is Training Mode` = checked trong MatchThreeAgent
   - ✅ Behavior Name = `Match3`
   - ✅ Tất cả references đã được assign

### Bước 2: Chạy Training Command

Mở Terminal/PowerShell trong thư mục project và chạy:

```bash
# Kích hoạt virtual environment (nếu dùng)
ml-agents-env\Scripts\activate  # Windows
# hoặc
source ml-agents-env/bin/activate  # Mac/Linux

# Chạy training
mlagents-learn config/match3_config.yaml --run-id=Match3_Training
```

### Bước 3: Bắt Đầu Training trong Unity

1. Sau khi chạy command, bạn sẽ thấy:
   ```
   [INFO] Listening on port 5004. Start training by pressing the Play button in the Unity Editor.
   ```
2. Nhấn **Play** trong Unity Editor
3. Training sẽ bắt đầu!

### Bước 4: Kiểm Tra Training

Trong Unity Console, bạn sẽ thấy:

- `[ENEMY AI] OnActionReceived...`
- `[ENEMY AI] ✅ VALID MOVE!`
- `[ENEMY AI] PerformMove started...`
- `[SWAP] SwapPiece completed...`

Trong Terminal, bạn sẽ thấy:

- Episode count tăng dần
- Reward metrics
- Training progress

---

## 📊 Theo Dõi Training với TensorBoard

### Bước 1: Cài TensorBoard

```bash
pip install tensorboard
```

### Bước 2: Chạy TensorBoard

Trong Terminal mới (giữ training đang chạy):

```bash
# Kích hoạt virtual environment
ml-agents-env\Scripts\activate  # Windows

# Chạy TensorBoard
tensorboard --logdir results/Match3_Training
```

### Bước 3: Xem Metrics

1. Mở browser và vào: `http://localhost:6006`
2. Bạn sẽ thấy các metrics:
   - **Reward/EpisodeReward**: Reward mỗi episode
   - **Reward/CumulativeReward**: Tổng reward tích lũy
   - **Stats/MoveSuccessRate**: Tỷ lệ moves thành công
   - **Stats/ValidMoveRate**: Tỷ lệ valid moves
   - **Stats/MatchesPerMove**: Số matches trung bình mỗi move
   - **Health/EnemyHealth**, **Health/PlayerHealth**

### Bước 4: Theo Dõi Training

- Refresh browser để xem metrics cập nhật
- Training tốt khi:
  - ✅ EpisodeReward tăng dần
  - ✅ ValidMoveRate tăng dần
  - ✅ MatchesPerMove tăng dần

---

## 🎯 Sử Dụng Model Đã Train

### Bước 1: Tìm Model File

Sau khi training, model sẽ được lưu tại:

```
results/Match3_Training/Match3.onnx
```

### Bước 2: Load Model vào Unity

1. Trong Unity, chọn GameObject `MLAgents`
2. Trong **Behavior Parameters**:
   - **Model**: Kéo file `Match3.onnx` vào
   - **Behavior Type**: Đổi thành `Inference Only`
3. **Tắt Training Mode**:
   - `GamePlayController`: Uncheck `Is Training Mode`
   - `MatchThreeAgent`: Uncheck `Is Training Mode`

### Bước 3: Test Model

1. Nhấn Play
2. Agent sẽ sử dụng model đã train để chơi!

---

## 🔍 Troubleshooting

### Lỗi: "The behavior name Match3 has not been specified"

**Nguyên nhân**: Behavior Name trong Unity không khớp với config YAML
**Giải pháp**:

- Kiểm tra Behavior Name trong Unity = `Match3`
- Kiểm tra key `behaviors:` trong YAML = `Match3`

### Lỗi: "Connection refused" hoặc "Port already in use"

**Nguyên nhân**: Port 5004 đang được sử dụng
**Giải pháp**:

- Đổi port trong config: `base_port: 5005`
- Hoặc đóng process đang dùng port 5004

### Lỗi: "Observation count mismatch"

**Nguyên nhân**: Observation size không khớp
**Giải pháp**:

- Để Unity tự động detect observation size
- Hoặc kiểm tra grid size (8x8 = 259 observations)

### Agent không swap

**Nguyên nhân**:

- Training mode chưa bật
- `isManualSwap` chưa được pass đúng
  **Giải pháp**:
- ✅ Check `Is Training Mode` trong cả GamePlayController và MatchThreeAgent
- Kiểm tra logs trong Console để xem swap có được gọi không

### Training quá chậm

**Giải pháp**:

- Tăng `time_scale` trong config (hiện tại: 20)
- Giảm `Decision Period` (hiện tại: 5)
- Tắt graphics: `no_graphics: true` trong config

### Không thấy metrics trên TensorBoard

**Nguyên nhân**: StatsRecorder chưa được init
**Giải pháp**:

- Đảm bảo có `Academy` trong scene
- Kiểm tra logs: `StatsRecorder initialized successfully`

---

## 📝 Checklist Trước Khi Training

Trước khi bắt đầu training, đảm bảo:

- [ ] Python 3.8-3.10 đã cài và trong PATH
- [ ] ML-Agents package đã cài: `pip install mlagents`
- [ ] Unity scene có GameObject `MLAgents` với Behavior Parameters
- [ ] Behavior Name = `Match3` (khớp với config YAML)
- [ ] Tất cả references trong MatchThreeAgent đã được assign
- [ ] `Is Training Mode` = ✅ checked trong GamePlayController
- [ ] `Is Training Mode` = ✅ checked trong MatchThreeAgent
- [ ] Có `Academy` GameObject trong scene
- [ ] Config YAML file tồn tại tại `config/match3_config.yaml`
- [ ] Port 5004 không bị chiếm dụng

---

## 🎓 Tips và Best Practices

1. **Bắt đầu với config đơn giản**:

   - Giảm `max_steps` xuống 10000 để test nhanh
   - Tăng `summary_freq` để xem metrics thường xuyên hơn

2. **Monitor training**:

   - Luôn mở TensorBoard khi training
   - Kiểm tra Console logs thường xuyên

3. **Save checkpoints**:

   - Model được lưu tự động tại `checkpoint_interval`
   - Có thể resume training: `resume: true` trong config

4. **Tune hyperparameters**:

   - Nếu training không ổn định: giảm `learning_rate`
   - Nếu học quá chậm: tăng `learning_rate`
   - Nếu overfitting: tăng `beta` (entropy coefficient)

5. **Training time**:
   - Training có thể mất vài giờ đến vài ngày
   - Để training qua đêm với `max_steps: 500000`

---

## 📞 Hỗ Trợ

Nếu gặp vấn đề:

1. Kiểm tra Console logs trong Unity
2. Kiểm tra Terminal output
3. Xem TensorBoard metrics
4. Kiểm tra lại checklist ở trên

**Chúc bạn training thành công! 🚀**
