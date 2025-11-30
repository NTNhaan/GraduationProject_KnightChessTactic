# CHECKLIST: Tại sao OnMouse events không hoạt động

## ✅ CÁC ĐIỀU KIỆN BẮT BUỘC ĐỂ OnMouse EVENTS HOẠT ĐỘNG:

### 1. **Camera phải có Physics2DRaycaster**

- [ ] Chọn Main Camera trong Hierarchy
- [ ] Kiểm tra có component "Physics 2D Raycaster" không
- [ ] Nếu KHÔNG có → Add Component → Physics 2D Raycaster
- **Đây là nguyên nhân phổ biến nhất!**

### 2. **GamePieces phải có BoxCollider2D**

- [ ] Mở prefab của piece (piece_prefab hoặc piece_pf)
- [ ] Kiểm tra có BoxCollider2D component không
- [ ] BoxCollider2D phải ENABLED (checkbox ticked)
- [ ] BoxCollider2D có thể nằm trên parent hoặc child object

### 3. **EventSystem phải tồn tại trong scene**

- [ ] Trong Hierarchy, tìm GameObject có component "Event System"
- [ ] Nếu KHÔNG có → GameObject → UI → Event System
- [ ] EventSystem phải ACTIVE

### 4. **UI không được che phủ pieces**

- [ ] Kiểm tra Canvas có che lên board không
- [ ] Tạm thời disable Canvas để test
- [ ] Kiểm tra CanvasGroup có `blocksRaycasts = true` không

### 5. **Layer không bị ignore raycast**

- [ ] Chọn một piece trong scene
- [ ] Kiểm tra Layer (top right của Inspector)
- [ ] Layer KHÔNG được là "Ignore Raycast"

### 6. **GamePieces GameObject phải ACTIVE**

- [ ] Kiểm tra GameObject có GamePieces script phải ACTIVE
- [ ] Checkbox "Active" phải được ticked

### 7. **Camera phải nhìn thấy pieces**

- [ ] Kiểm tra Camera culling mask
- [ ] Pieces phải nằm trong camera view
- [ ] Camera không bị disable

## 🔧 CÁCH TEST NHANH:

1. **Thêm script InputDebugger vào scene:**

   - Tạo empty GameObject
   - Add Component → InputDebugger
   - Chạy game và click vào pieces
   - Xem Console logs để biết vấn đề ở đâu

2. **Test thủ công:**

   - Chọn Main Camera
   - Add Component → Physics 2D Raycaster
   - Chạy lại game

3. **Test với Update() method:**
   - Uncomment Update() method trong GamePieces.cs (dòng 151-186)
   - Chạy game và click
   - Nếu thấy logs `[DEBUG INPUT]` → OnMouse events không hoạt động, nhưng input vẫn detect được

## 🐛 CÁC VẤN ĐỀ THƯỜNG GẶP:

1. **Camera thiếu Physics2DRaycaster** ← Phổ biến nhất!
2. **Collider bị disable** (trong ClearAllValidMatches)
3. **UI che phủ** (Canvas blocksRaycasts = true)
4. **EventSystem không tồn tại**
5. **Layer bị ignore raycast**

## 📝 LOGS CẦN KIỂM TRA:

Khi click vào piece, bạn PHẢI thấy:

- `[MOUSE] OnMouseDown called on piece at (x, y)`
- `[INPUT] PressPiece: Piece at (x, y)`

Nếu KHÔNG thấy → OnMouse events không hoạt động → Kiểm tra checklist trên!
