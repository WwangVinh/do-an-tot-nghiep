<template>
  <div class="page">
    <!-- Top bar -->
    <header class="topbar">
      <div class="title-wrap">
        <button class="btn ghost" @click="goBack">← Quay lại</button>
        <div>
          <h1>Quản lý Xe</h1>
        </div>
      </div>
      <div class="actions">
        <button class="btn primary" @click="openCreate">+ Thêm xe</button>
      </div>
    </header>

    <!-- Search bar -->
    <section class="card search-card">
      <div class="search-grid">
        <div class="field">
          <label>Tìm theo ID</label>
          <div class="input-group">
            <input
              v-model.trim="searchId"
              type="number"
              min="1"
              placeholder="Ví dụ: 5"
              @keyup.enter="searchById"
            />
            <button class="btn" @click="searchById" :disabled="loading">Tìm</button>
          </div>
        </div>

        <div class="field">
          <label>Tìm theo từ khóa</label>
          <div class="input-group">
            <input
              v-model.trim="keyword"
              type="text"
              placeholder="Nhập tên / hãng / model / màu..."
            />
            <button
              class="btn"
              @click="clearKeyword"
              :disabled="loading && cars.length === 0"
            >
              Xóa
            </button>
          </div>
        </div>

        <div class="field right">
          <label>&nbsp;</label>
          <div class="input-group">
            <button class="btn" @click="reload" :disabled="loading">
              ↻ Tải lại danh sách
            </button>
          </div>
        </div>
      </div>
    </section>

    <!-- Table -->
    <section class="card">
      <div class="table-head">
        <div class="left">
          <span class="muted"> • Trang {{ currentPage }} / {{ totalPages }}</span>

          <span class="muted">xe</span>
        </div>
        <div class="right muted" v-if="loading">Đang tải...</div>
      </div>

      <div v-if="error" class="alert error"><b>Lỗi:</b> {{ error }}</div>

      <div class="table-wrap">
        <table class="table big">
          <thead>
            <tr>
              <th>ID</th>
              <th>Ảnh</th>
              <th>Tên xe</th>
              <th>Model</th>
              <th>Năm</th>
              <th>Giá</th>
              <th>Màu</th>
              <th>Hãng</th>
              <th>Odo</th>
              <th>Trạng thái</th>
              <th>Mô tả</th>
              <th>Ngày tạo</th>
              <th>Cập nhật</th>
              <th>Thao tác</th>
            </tr>
          </thead>

          <transition-group name="row" tag="tbody">
  <tr v-for="car in pagedCars" :key="car.CarId || car.carId" class="row">
    <td class="mono">{{ car.CarId || car.carId }}</td>

    <td>
<img 
  v-if="car.ImageUrl || car.imageUrl" 
  :src="car.ImageUrl || car.imageUrl" 
  class="thumb-lg" 
/>
      <span v-else class="no-img">No Image</span>
    </td>
    
    <td class="strong">{{ car.Name || car.name || "—" }}</td>
    <td>{{ car.Model || car.model || "—" }}</td>
    <td>{{ car.Year || car.year || "—" }}</td>
    
    <td class="mono">{{ formatMoney(car.Price || car.price) }}</td>
    
    <td>{{ car.Color || car.color || "—" }}</td>
    <td>{{ car.Brand || car.brand || "—" }}</td>
    <td class="mono">{{ car.Mileage || car.mileage || 0 }}</td>

    <td>
      <span class="pill" :class="statusClass(car.Status || car.status)">
        {{ car.Status || car.status || "—" }}
      </span>
    </td>

    <td class="desc line2">{{ car.Description || car.description || "—" }}</td>

    <td class="date mono">{{ formatDate(car.CreatedAt || car.createdAt) }}</td>
    <td class="date mono">{{ formatDate(car.UpdatedAt || car.updatedAt) }}</td>

    <td>
      <div class="btn-row">
        <button class="btn sm" @click="openEdit(car.CarId || car.carId)">Sửa</button>
        <button class="btn sm danger" @click="confirmDelete(car)">Xóa</button>
      </div>
    </td>
  </tr>
</transition-group>
        </table>
        <div class="pager" v-if="filteredCars.length > 0">
          <div class="pager-left">
            <span class="muted">
              Hiển thị <b>{{ pagedCars.length }}</b> / <b>{{ filteredCars.length }}</b> xe
            </span>
          </div>

          <div class="pager-right">
            <button class="btn sm" @click="prevPage" :disabled="currentPage === 1">
              ‹ Trước
            </button>

            <div class="page-indicator">
              Trang
              <input
                class="page-input"
                type="number"
                :min="1"
                :max="totalPages"
                v-model.number="currentPage"
                @change="goPage(currentPage)"
              />
              / {{ totalPages }}
            </div>

            <button
              class="btn sm"
              @click="nextPage"
              :disabled="currentPage === totalPages"
            >
              Sau ›
            </button>
          </div>
        </div>
      </div>

      <div v-if="filteredCars.length === 0 && !loading" class="empty">
        Không có dữ liệu
      </div>
    </section>
    <!-- Modal Create/Edit -->
    <transition name="modal">
      <div v-if="modal.open" class="modal-backdrop" @mousedown.self="closeModal">
        <div class="modal">
          <div class="modal-header">
            <div>
              <h2>{{ modal.mode === "create" ? "Thêm xe" : "Sửa xe" }}</h2>
            </div>
            <button class="btn ghost" @click="closeModal">✕</button>
          </div>

          <form class="form" @submit.prevent="submitModal">
            <div class="grid">
              <div class="field">
                <label>Tên xe <span class="req">*</span></label>
                <input v-model.trim="form.name" required placeholder="Ví dụ: Porsche" />
              </div>

              <div class="field">
                <label>Hãng</label>
                <input v-model.trim="form.brand" placeholder="Ví dụ: Porsche" />
              </div>

              <div class="field">
                <label>Model</label>
                <input v-model.trim="form.model" placeholder="Ví dụ: sport" />
              </div>

              <div class="field">
                <label>Năm</label>
                <input
                  v-model.number="form.year"
                  type="number"
                  min="0"
                  placeholder="2025"
                />
              </div>

              <div class="field">
                <label>Giá</label>
                <input
                  v-model.number="form.price"
                  type="number"
                  min="0"
                  placeholder="600000"
                />
              </div>

              <div class="field">
                <label>Màu</label>
                <input v-model.trim="form.color" placeholder="đen" />
              </div>

              <div class="field">
                <label>Odo (mileage)</label>
                <input
                  v-model.number="form.mileage"
                  type="number"
                  min="0"
                  placeholder="222222"
                />
              </div>

              <div class="field"> <label>Trạng thái</label> <select v-model="form.status"> <option value="Sold">Sold</option> <option value="Available">Available</option> </select> </div>

              <div class="field span2">
  <label>Xem trước ảnh</label>
 <div class="image-preview-container">
  <img 
    v-if="form.imageUrl" 
    :src="form.imageUrl" 
    class="preview-small" 
    @error="onImgError"
  />
</div>

<input v-model.trim="form.imageUrl" placeholder="Dán link ảnh (https://...)" />
</div>

              <div class="field span2">
                <label>Mô tả</label>
                <textarea
                  v-model.trim="form.description"
                  rows="3"
                  placeholder="abc"
                ></textarea>
              </div>
            </div>

            <div class="modal-footer">
              <button type="button" class="btn" @click="closeModal">Hủy</button>
              <button type="submit" class="btn primary" :disabled="modal.saving">
                <span v-if="modal.saving" class="spinner"></span>
                {{ modal.mode === "create" ? "Thêm" : "Lưu" }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </transition>

    <!-- Confirm delete -->
    <transition name="modal">
      <div v-if="del.open" class="modal-backdrop" @mousedown.self="del.open = false">
        <div class="modal small">
          <div class="modal-header">
            <div>
              <h2>Xác nhận xóa</h2>
            </div>
            <button class="btn ghost" @click="del.open = false">✕</button>
          </div>
          <div class="confirm-body">
            Bạn chắc chắn muốn xóa xe:
            <b>{{ del.car?.name }}</b> (ID: {{ del.car?.carId }}) ?
          </div>
          <div class="modal-footer">
            <button class="btn" @click="del.open = false">Hủy</button>
            <button class="btn danger" @click="doDelete" :disabled="del.saving">
              <span v-if="del.saving" class="spinner"></span>
              Xóa
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- Toast -->
    <transition name="toast">
      <div v-if="toast.show" class="toast" :class="toast.type">
        {{ toast.message }}
      </div>
    </transition>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from "vue";
import { useRouter } from "vue-router";

const totalPages = computed(() => {
  const total = filteredCars.value.length;
  return Math.max(1, Math.ceil(total / pageSize.value));
});

const pagedCars = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  return filteredCars.value.slice(start, start + pageSize.value);
});

function prevPage() {
  if (currentPage.value > 1) currentPage.value--;
}

function nextPage() {
  if (currentPage.value < totalPages.value) currentPage.value++;
}

function goPage(p) {
  const page = Number(p);
  if (page >= 1 && page <= totalPages.value) currentPage.value = page;
}

const router = useRouter();
const API_BASE = "https://localhost:7053/api";
const API_CAR = `${API_BASE}/Car`;

const cars = ref([]);
const loading = ref(false);
const error = ref(null);

const searchId = ref("");
const keyword = ref("");
const pageSize = ref(8);
const currentPage = ref(1);
watch(keyword, () => {
  currentPage.value = 1;
});
const toast = reactive({ show: false, message: "", type: "ok" });
let toastTimer = null;
function formatDate(d) {
  if (!d) return "—";
  const date = new Date(d);
  return date.toLocaleString("vi-VN");
}

function showToast(message, type = "ok") {
  toast.show = true;
  toast.message = message;
  toast.type = type;
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => (toast.show = false), 2200);
}

function goBack() {
  // Nếu bạn có route dashboard thì push về đó:
  // router.push("/dashboard");
  // Còn không thì back lịch sử:
  router.back();
}

function onImgError(e) {
  e.target.style.display = "none";
}

function normalizeList(data) {
  // API .NET của bạn trả: { "$id": "...", "$values": [ ... ] }
  if (Array.isArray(data)) return data;
  if (data && Array.isArray(data.$values)) return data.$values;
  return [];
}

async function fetchList() {
  loading.value = true;
  error.value = null;
  try {
    const res = await fetch(API_CAR, { headers: { accept: "text/plain" } });
    if (!res.ok) throw new Error(`GET list failed: ${res.status}`);
    const data = await res.json();
    cars.value = normalizeList(data);
  } catch (e) {
    error.value = e?.message || "Không thể tải danh sách.";
  } finally {
    loading.value = false;
  }
}

function reload() {
  searchId.value = "";
  keyword.value = "";
  currentPage.value = 1;
  fetchList();
}

async function searchById() {
  const id = Number(searchId.value);
  if (!id || id < 1) {
    showToast("Nhập ID hợp lệ.", "warn");
    return;
  }
  loading.value = true;
  error.value = null;
  try {
    const res = await fetch(`${API_CAR}/${id}`, { headers: { accept: "text/plain" } });
    if (res.status === 404) {
      cars.value = [];
      showToast("Không tìm thấy xe.", "warn");
      return;
    }
    if (!res.ok) throw new Error(`GET by id failed: ${res.status}`);
    const data = await res.json();
    cars.value = [data];
currentPage.value = 1
    showToast("Đã tải xe theo ID.", "ok");
  } catch (e) {
    error.value = e?.message || "Không thể tìm theo ID.";
  } finally {
    loading.value = false;
  }
}

function clearKeyword() {
  keyword.value = "";
}

const filteredCars = computed(() => {
  const k = keyword.value.toLowerCase().trim();
  if (!k) return cars.value;

  return cars.value.filter((c) => {
    const hay = [c.name, c.brand, c.model, c.color, c.status, c.description]
      .filter(Boolean)
      .join(" ")
      .toLowerCase();
    return hay.includes(k);
  });
});

/** ---------- Modal Create/Edit ---------- **/
const modal = reactive({
  open: false,
  mode: "create", // 'create' | 'edit'
  saving: false,
  editId: null,
});

const form = reactive({
  name: "",
  brand: "",
  model: "",
  year: null,
  price: null,
  color: "",
  mileage: null,
  status: "",
  imageUrl: "",
  description: "",
});

function resetForm() {
  form.name = "";
  form.brand = "";
  form.model = "";
  form.year = null;
  form.price = null;
  form.color = "";
  form.mileage = null;
  form.status = "";
  form.imageUrl = "";
  form.description = "";
}

function openCreate() {
  modal.open = true;
  modal.mode = "create";
  modal.editId = null;
  resetForm();
}

async function openEdit(id) {
  if (!id) return;
  
  modal.open = true;
  modal.mode = "edit";
  modal.editId = id;
  resetForm();

  modal.saving = true;
  try {
    const res = await fetch(`${API_CAR}/${id}`, { 
      headers: { accept: "application/json" } 
    });
    
    if (!res.ok) throw new Error(`Không thể lấy thông tin xe: ${res.status}`);
    
    const c = await res.json();
    console.log("Dữ liệu xe nhận được:", c); // Để bạn kiểm tra trong F12

    // Đổ dữ liệu vào Form - Hỗ trợ cả viết Hoa và viết thường từ API
    form.name = c.Name ?? c.name ?? "";
    form.brand = c.Brand ?? c.brand ?? "";
    form.model = c.Model ?? c.model ?? "";
    form.year = c.Year ?? c.year ?? null;
    form.price = c.Price ?? c.price ?? null;
    form.color = c.Color ?? c.color ?? "";
    form.mileage = c.Mileage ?? c.mileage ?? null;
    form.status = c.Status ?? c.status ?? "";
    form.imageUrl = c.ImageUrl ?? c.imageUrl ?? "";
    form.description = c.Description ?? c.description ?? "";

  } catch (e) {
    showToast(e?.message || "Không tải được dữ liệu xe.", "err");
    modal.open = false; // Đóng modal nếu lỗi để tránh sửa nhầm dữ liệu trống
  } finally {
    modal.saving = false;
  }
}
function closeModal() {
  if (modal.saving) return;
  modal.open = false;
}

function buildFormData() {
  // multipart/form-data: chỉ append các field cần
  const fd = new FormData();

  // IMPORTANT: backend của bạn thường bắt buộc name → luôn gửi name
  fd.append("name", form.name ?? "");

  // Các field còn lại: nếu rỗng thì vẫn có thể gửi (tùy backend), ở đây gửi nếu có giá trị
  if (form.brand !== "" && form.brand != null) fd.append("brand", form.brand);
  if (form.model !== "" && form.model != null) fd.append("model", form.model);
  if (form.color !== "" && form.color != null) fd.append("color", form.color);
  if (form.status !== "" && form.status != null) fd.append("status", form.status);
  if (form.imageUrl !== "" && form.imageUrl != null) fd.append("imageUrl", form.imageUrl);
  if (form.description !== "" && form.description != null)
    fd.append("description", form.description);

  // number: nếu null thì bỏ qua
  if (form.year != null && form.year !== "") fd.append("year", String(form.year));
  if (form.price != null && form.price !== "") fd.append("price", String(form.price));
  if (form.mileage != null && form.mileage !== "")
    fd.append("mileage", String(form.mileage));

  return fd;
}

async function submitModal() {
  if (!form.name || !form.price) {
    showToast("Tên xe và Giá là bắt buộc.", "warn");
    return;
  }

  modal.saving = true;
  try {
    const fd = new FormData();
    // Phải viết thường chính xác theo tham số trong Controller của bạn
    fd.append("name", form.name);
    fd.append("model", form.model || "");
    fd.append("year", form.year ? String(form.year) : "");
    fd.append("price", form.price ? String(form.price) : "");
    fd.append("color", form.color || "");
    fd.append("description", form.description || "");
    fd.append("brand", form.brand || "");
    fd.append("mileage", form.mileage ? String(form.mileage) : "");
    fd.append("imageUrl", form.imageUrl || "");
    fd.append("status", form.status || "");

    const isEdit = modal.mode === "edit";
    const url = isEdit ? `${API_CAR}/${modal.editId}` : API_CAR;
    const method = isEdit ? "PUT" : "POST";

    const res = await fetch(url, {
      method: method,
      headers: { 
        "accept": "*/*" 
        // LƯU Ý: KHÔNG để Content-Type khi gửi FormData, trình duyệt sẽ tự xử lý
      },
      body: fd,
    });

    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(errorText || "Lỗi lưu dữ liệu");
    }

    showToast(isEdit ? "Cập nhật thành công" : "Thêm xe thành công!");
    modal.open = false;
    fetchList();
  } catch (e) {
    showToast(e.message, "err");
  } finally {
    modal.saving = false;
  }
}

/** ---------- Delete ---------- **/
const del = reactive({ open: false, saving: false, car: null });

function confirmDelete(car) {
  del.open = true;
  del.car = car;
}

async function doDelete() {
  const idToDelete = del.car?.CarId || del.car?.carId;
  if (!idToDelete) return;

  del.saving = true;
  try {
    const res = await fetch(`${API_CAR}/${idToDelete}`, {
      method: "DELETE"
    });

    if (!res.ok) {
      // Nếu server trả về lỗi, thường là lỗi 500 do khóa ngoại
      throw new Error("Không thể xóa xe này vì đã có trong đơn hàng (Orders)!");
    }

    showToast("Đã xóa xe thành công.", "ok");
    del.open = false;
    fetchList();
  } catch (e) {
    console.error("Lỗi xóa:", e);
    // Hiện thông báo rõ ràng cho người dùng
    showToast(e.message, "err");
  } finally {
    del.saving = false;
  }
}
/** ---------- Utils ---------- **/
function formatMoney(val) {
  const n = Number(val);
  if (!Number.isFinite(n)) return "—";
  return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(n);
}

function statusClass(status) {
  const s = String(status ?? "").toLowerCase();
  if (["1", "available", "còn", "con"].some((x) => s.includes(x))) return "ok";
  if (["0", "sold", "hết", "het"].some((x) => s.includes(x))) return "bad";
  return "neutral";
}

onMounted(fetchList);
</script>

<style scoped>
/* ====== Base ====== */
.page {
  max-width: 1200px;
  margin: 26px auto;
  padding: 0 18px 40px;
  font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Arial;
  color: #0f172a;
}

.mono {
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono",
    monospace;
}
.sub {
  margin: 4px 0 0;
}

.card {
  background: rgba(255, 255, 255, 0.78);
  border: 1px solid rgba(15, 23, 42, 0.08);
  border-radius: 16px;
  padding: 16px;
  box-shadow: 0 10px 28px rgba(2, 6, 23, 0.08);
  backdrop-filter: blur(10px);
}
.search-card {
  margin: 14px 0;
}

/* ====== Topbar ====== */
.topbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}
.title-wrap {
  display: flex;
  align-items: center;
  gap: 12px;
}
h1 {
  margin: 0;
  font-size: 22px;
  letter-spacing: 0.2px;
}

/* ====== Buttons ====== */
.btn {
  appearance: none;
  border: 1px solid rgba(15, 23, 42, 0.15);
  background: #fff;
  color: #0f172a;
  padding: 10px 12px;
  border-radius: 12px;
  cursor: pointer;
  transition: transform 0.15s ease, box-shadow 0.15s ease, background 0.15s ease,
    border-color 0.15s ease, opacity 0.15s ease;
  box-shadow: 0 6px 16px rgba(2, 6, 23, 0.06);
}
.btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 22px rgba(2, 6, 23, 0.1);
}
.btn:active {
  transform: translateY(0);
}
.btn:disabled {
  opacity: 0.55;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.btn.primary {
  background: linear-gradient(135deg, #2563eb, #7c3aed);
  border-color: transparent;
  color: #fff;
}
.btn.ghost {
  background: transparent;
  box-shadow: none;
}
.btn.danger {
  background: linear-gradient(135deg, #ef4444, #f97316);
  border-color: transparent;
  color: #fff;
}
.btn.sm {
  padding: 8px 10px;
  border-radius: 10px;
}

/* ====== Search ====== */
.search-grid {
  display: grid;
  grid-template-columns: 1.2fr 1.5fr 0.8fr;
  gap: 14px;
  align-items: end;
}
@media (max-width: 980px) {
  .search-grid {
    grid-template-columns: 1fr;
  }
}
.field label {
  display: block;
  font-size: 12px;
  color: #334155;
  margin-bottom: 6px;
}
.hint {
  display: block;
  margin-top: 6px;
  color: #94a3b8;
  font-size: 12px;
}

.input-group {
  display: flex;
  gap: 10px;
}
input,
textarea {
  width: 100%;
  padding: 10px 12px;
  border-radius: 12px;
  border: 1px solid rgba(15, 23, 42, 0.15);
  outline: none;
  background: rgba(255, 255, 255, 0.9);
  transition: box-shadow 0.15s ease, border-color 0.15s ease;
}
input:focus,
textarea:focus {
  border-color: rgba(37, 99, 235, 0.55);
  box-shadow: 0 0 0 4px rgba(37, 99, 235, 0.12);
}

/* ====== Table ====== */
.table-wrap {
  overflow: auto;
  border-radius: 14px;
}
.table {
  width: 100%;
  border-collapse: collapse;
  min-width: 980px;
}
thead th {
  text-align: left;
  font-size: 12px;
  letter-spacing: 0.3px;
  text-transform: uppercase;
  color: #475569;
  padding: 12px 12px;
  background: rgba(2, 6, 23, 0.03);
  border-bottom: 1px solid rgba(15, 23, 42, 0.1);
}
tbody td {
  padding: 12px 12px;
  border-bottom: 1px solid rgba(15, 23, 42, 0.08);
  vertical-align: top;
}
tr.row {
  transition: background 0.15s ease, transform 0.15s ease;
}
tr.row:hover {
  background: rgba(37, 99, 235, 0.06);
}
.btn-row {
  display: flex;
  gap: 8px;
  justify-content: flex-start;
}

.name-cell {
  display: flex;
  gap: 10px;
  align-items: flex-start;
}
.thumb {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  object-fit: cover;
  border: 1px solid rgba(15, 23, 42, 0.1);
  box-shadow: 0 8px 18px rgba(2, 6, 23, 0.08);
}
.name {
  font-weight: 650;
}
.line1 {
  display: -webkit-box;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.table-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 6px 2px 12px;
}
.badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 28px;
  height: 22px;
  padding: 0 8px;
  border-radius: 999px;
  background: rgba(37, 99, 235, 0.12);
  color: #1d4ed8;
  font-weight: 700;
  margin-right: 6px;
}

/* ====== Pills ====== */
.pill {
  display: inline-flex;
  padding: 6px 10px;
  border-radius: 999px;
  font-size: 12px;
  border: 1px solid rgba(15, 23, 42, 0.1);
}
.pill.ok {
  background: rgba(34, 197, 94, 0.1);
  color: #15803d;
  border-color: rgba(34, 197, 94, 0.22);
}
.pill.bad {
  background: rgba(239, 68, 68, 0.1);
  color: #b91c1c;
  border-color: rgba(239, 68, 68, 0.22);
}
.pill.neutral {
  background: rgba(100, 116, 139, 0.1);
  color: #475569;
  border-color: rgba(100, 116, 139, 0.22);
}

/* ====== Alerts / Empty ====== */
.alert {
  padding: 12px 12px;
  border-radius: 14px;
  margin: 10px 0;
  border: 1px solid rgba(15, 23, 42, 0.12);
}
.alert.error {
  background: rgba(239, 68, 68, 0.08);
  border-color: rgba(239, 68, 68, 0.22);
  color: #991b1b;
}
.empty {
  padding: 16px;
  text-align: center;
  color: #64748b;
}

/* ====== Skeleton ====== */
.skeleton-wrap {
  padding: 6px;
}
.skeleton-row {
  height: 44px;
  border-radius: 12px;
  background: linear-gradient(
    90deg,
    rgba(2, 6, 23, 0.05),
    rgba(2, 6, 23, 0.1),
    rgba(2, 6, 23, 0.05)
  );
  background-size: 240% 100%;
  animation: shimmer 1.2s infinite;
  margin-bottom: 10px;
}
@keyframes shimmer {
  0% {
    background-position: 100% 0;
  }
  100% {
    background-position: -140% 0;
  }
}

/* ====== Modal ====== */
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(2, 6, 23, 0.55);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 18px;
  z-index: 50;
}
.modal {
  width: min(860px, 100%);
  background: #fff;
  border-radius: 18px;
  border: 1px solid rgba(15, 23, 42, 0.12);
  box-shadow: 0 20px 60px rgba(2, 6, 23, 0.3);
  overflow: hidden;
}
.modal.small {
  width: min(520px, 100%);
}
.modal-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 12px;
  padding: 16px 16px 0;
}
.modal-header h2 {
  margin: 0;
  font-size: 18px;
}
.form {
  padding: 16px;
}
.grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}
@media (max-width: 860px) {
  .grid {
    grid-template-columns: 1fr;
  }
}
.span2 {
  grid-column: span 2;
}
@media (max-width: 860px) {
  .span2 {
    grid-column: auto;
  }
}
.req {
  color: #ef4444;
}
.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 14px 16px 16px;
  border-top: 1px solid rgba(15, 23, 42, 0.08);
}
.confirm-body {
  padding: 14px 16px;
  color: #0f172a;
}

/* ====== Animations ====== */
.row-enter-active,
.row-leave-active {
  transition: all 0.18s ease;
}
.row-enter-from {
  opacity: 0;
  transform: translateY(6px);
}
.row-leave-to {
  opacity: 0;
  transform: translateY(6px);
}

.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.18s ease;
}
.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.toast {
  position: fixed;
  right: 18px;
  bottom: 18px;
  background: #0f172a;
  color: #fff;
  padding: 12px 14px;
  border-radius: 14px;
  box-shadow: 0 18px 44px rgba(2, 6, 23, 0.35);
  z-index: 60;
}
.toast.ok {
  background: linear-gradient(135deg, #16a34a, #22c55e);
}
.toast.warn {
  background: linear-gradient(135deg, #f59e0b, #f97316);
}
.toast.err {
  background: linear-gradient(135deg, #ef4444, #f97316);
}

.toast-enter-active,
.toast-leave-active {
  transition: all 0.18s ease;
}
.toast-enter-from {
  opacity: 0;
  transform: translateY(10px);
}
.toast-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

/* spinner */
.spinner {
  display: inline-block;
  width: 14px;
  height: 14px;
  border-radius: 999px;
  border: 2px solid rgba(255, 255, 255, 0.55);
  border-top-color: rgba(255, 255, 255, 1);
  margin-right: 8px;
  animation: spin 0.7s linear infinite;
}
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
.table.big {
  min-width: 1800px;
}
.preview-small {
  max-width: 200px; /* Giới hạn chiều rộng */
  max-height: 120px; /* Giới hạn chiều cao */
  display: block;
  margin: 10px 0;
  border: 1px solid #ddd;
}
.thumb-lg {
  width: 80px;
  height: 50px;
  object-fit: cover;
  border-radius: 4px;
}

.no-img {
  font-size: 12px;
  color: #94a3b8;
}

.strong {
  font-weight: 700;
}

.desc {
  max-width: 260px;
  color: #475569;
}

.line2{
  display:-webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow:hidden;
}
.preview-box {
    margin-top: 10px;
    border: 1px dashed #ccc;
    padding: 5px;
    text-align: center;
    max-height: 200px; /* Giới hạn chiều cao khu vực xem trước */
    overflow: hidden;
}

.preview-box img {
    max-width: 100%;
    max-height: 150px; /* Ảnh preview chỉ cao tối đa 150px */
    object-fit: contain; /* Giữ nguyên tỉ lệ ảnh */
    border-radius: 4px;
}

/* Đảm bảo Modal có thể cuộn nếu nội dung quá dài */
.modal-body {
    max-height: 70vh;
    overflow-y: auto;
    padding: 20px;
}
.date {
  font-size: 12px;
  color: #64748b;
}
.pager{
  display:flex;
  align-items:center;
  justify-content:space-between;
  gap:12px;
  padding: 14px 6px 0;
}

.pager-right{
  display:flex;
  align-items:center;
  gap:10px;
}

.page-indicator{
  display:flex;
  align-items:center;
  gap:8px;
  padding: 8px 10px;
  border-radius: 12px;
  border: 1px solid rgba(15,23,42,.12);
  background: rgba(255,255,255,.75);
  box-shadow: 0 8px 18px rgba(2,6,23,.06);
}

.page-input{
  width: 72px;
  padding: 6px 8px;
  border-radius: 10px;
  border: 1px solid rgba(15,23,42,.15);
  outline:none;
}

.page-input:focus{
  border-color: rgba(37,99,235,.55);
  box-shadow: 0 0 0 4px rgba(37,99,235,.12);
}
.field select { padding: 8px 12px; border: 1px solid #ccc; border-radius: 6px; font-size: 14px; background-color: #fff; color: #333; width: 100%; box-sizing: border-box; transition: border-color 0.3s ease; } .field select:focus { border-color: #007bff; outline: none; }
</style>
