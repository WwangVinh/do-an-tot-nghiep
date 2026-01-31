<template>
  <div class="page">
    <header class="topbar">
      <div class="title-wrap">
        <button class="btn ghost" @click="goBack">← Quay lại</button>
        <div>
          <h1>📜 Quản lý Đơn hàng</h1>
        </div>
      </div>
      <div class="actions">
        <button class="btn primary" @click="openCreate">+ Thêm đơn mới</button>
      </div>
    </header>

    <section class="card search-card">
      <div class="search-grid">
        <div class="field">
          <label>Tìm khách hàng (ID)</label>
          <div class="input-group">
            <input v-model.trim="searchUserId" type="number" min="1" placeholder="Nhập mã khách..." @keyup.enter="searchByUserId" />
            <button class="btn" @click="searchByUserId" :disabled="loading">Tìm</button>
          </div>
        </div>

        <div class="field">
          <label>Tìm theo từ khóa</label>
          <div class="input-group">
            <input v-model.trim="keyword" type="text" placeholder="Trạng thái, địa chỉ..." />
            <button class="btn" @click="clearKeyword" :disabled="loading && orders.length === 0">Xóa</button>
          </div>
        </div>

        <div class="field right">
          <label>&nbsp;</label>
          <div class="input-group">
            <button class="btn" @click="reload" :disabled="loading">↻ Làm mới</button>
          </div>
        </div>
      </div>
    </section>

    <section class="card">
      <div class="table-head">
        <div class="left">
          <span class="badge">{{ filteredOrders.length }}</span>
          <span class="muted">đơn hàng • Trang {{ currentPage }} / {{ totalPages }}</span>
        </div>
        <div class="right muted" v-if="loading">Đang kết nối API...</div>
      </div>

      <div v-if="error" class="alert error"><b>Lỗi hệ thống:</b> {{ error }}</div>

      <div class="table-wrap">
        <table class="table big">
          <thead>
            <tr>
              <th>Mã Đơn</th>
              <th>Khách hàng</th>
              <th>Tên Xe</th>
              <th>Ngày đặt</th>
              <th>Trạng thái</th>
              <th>Tổng tiền</th>
              <th>Thanh toán</th>
              <th>Địa chỉ giao hàng</th>
              <th>Thao tác</th>
            </tr>
          </thead>

          <transition-group name="row" tag="tbody">
            <tr v-for="o in pagedOrders" :key="o.OrderId || o.orderId" class="row">
              <td class="mono">#{{ o.OrderId || o.orderId }}</td>
              <td class="strong">Khách #{{ o.UserId || o.userId }}</td>
              <td class="strong text-primary">
                {{ o.Car?.Name || o.car?.name || ('Xe #' + (o.CarId || o.carId)) }}
              </td>
              <td class="mono small">{{ formatDateTime(o.OrderDate || o.orderDate) }}</td>
              <td>
                <span class="pill" :class="statusClass(o.Status || o.status)">
                  {{ translateStatus(o.Status || o.status) }}
                </span>
              </td>
              <td class="mono strong text-success">{{ formatMoney(o.TotalAmount || o.totalAmount) }}</td>
              <td class="small">{{ translatePayment(o.PaymentMethod || o.paymentMethod) }}</td>
              <td class="addr line2">{{ o.ShippingAddress || o.shippingAddress || "—" }}</td>
              <td>
                <div class="btn-row">
                  <button class="btn sm" @click="openEdit(o.OrderId || o.orderId)">Sửa</button>
                  <button class="btn sm danger" @click="confirmDelete(o)">Xóa</button>
                </div>
              </td>
            </tr>
          </transition-group>
        </table>
      </div>

      <div class="pager" v-if="filteredOrders.length > 0">
        <div class="pager-left">
          <span class="muted">Hiển thị <b>{{ pagedOrders.length }}</b> / <b>{{ filteredOrders.length }}</b> đơn</span>
        </div>
        <div class="pager-right">
          <button class="btn sm" @click="prevPage" :disabled="currentPage === 1">‹ Trước</button>
          <div class="page-indicator">
            Trang <input class="page-input" type="number" v-model.number="currentPage" @change="goPage(currentPage)" /> / {{ totalPages }}
          </div>
          <button class="btn sm" @click="nextPage" :disabled="currentPage === totalPages">Sau ›</button>
        </div>
      </div>
    </section>

    <transition name="modal">
      <div v-if="modal.open" class="modal-backdrop" @mousedown.self="closeModal">
        <div class="modal">
          <div class="modal-header">
            <h2>{{ modal.mode === 'create' ? '➕ Thêm đơn hàng mới' : '📝 Sửa đơn hàng' }}</h2>
            <button class="btn ghost" @click="closeModal">✕</button>
          </div>
          <form class="form" @submit.prevent="submitModal">
            <div class="grid">
              <div class="field">
                <label>Mã Khách hàng (UserId)</label>
                <input v-model.number="form.userId" type="number" required placeholder="Nhập mã khách..." />
              </div>

              <div class="field">
                <label>Chọn Xe (Car)</label>
                <select v-model="form.carId" class="custom-select" required @change="onCarChange">
                  <option :value="null" disabled>-- Chọn một chiếc xe --</option>
                  <option v-for="car in carList" :key="car.CarId || car.carId" :value="car.CarId || car.carId">
                    {{ car.Name || car.name }} ({{ formatMoney(car.Price || car.price) }})
                  </option>
                </select>
              </div>

              <div class="field">
                <label>Trạng thái đơn hàng</label>
                <select v-model="form.status" class="custom-select">
                  <option value="Pending">⌛ Chờ xử lý</option>
                  <option value="Shipped">🚚 Đang giao</option>
                  <option value="Completed">✅ Hoàn tất</option>
                  <option value="Cancel">❌ Đã hủy</option>
                </select>
              </div>

              <div class="field">
                <label>Phương thức thanh toán</label>
                <select v-model="form.paymentMethod" class="custom-select">
                  <option value="Cash">💵 Tiền mặt</option>
                  <option value="Credit Card">💳 Thẻ tín dụng</option>
                  <option value="Transfer">🏦 Chuyển khoản</option>
                </select>
              </div>

              <div class="field">
                <label>Tổng tiền (VNĐ)</label>
                <input v-model.number="form.totalAmount" type="number" placeholder="Tự động tính theo xe..." />
              </div>

              <div class="field span2">
                <label>Địa chỉ giao hàng</label>
                <input v-model.trim="form.shippingAddress" placeholder="Số nhà, tên đường, thành phố..." required />
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn" @click="closeModal">Hủy bỏ</button>
              <button type="submit" class="btn primary" :disabled="modal.saving">
                {{ modal.saving ? 'Đang lưu...' : (modal.mode === 'create' ? 'Tạo đơn' : 'Cập nhật') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </transition>

    <transition name="modal">
      <div v-if="del.open" class="modal-backdrop" @mousedown.self="del.open = false">
        <div class="modal small">
          <div class="modal-header">
            <h2>⚠️ Xác nhận xóa</h2>
            <button class="btn ghost" @click="del.open = false">✕</button>
          </div>
          <div class="confirm-body" style="padding: 20px;">
            Bạn có chắc muốn xóa đơn hàng <b>#{{ del.order?.OrderId || del.order?.orderId }}</b>?
          </div>
          <div class="modal-footer">
            <button class="btn" @click="del.open = false">Hủy</button>
            <button class="btn danger" @click="doDelete" :disabled="del.saving">Xác nhận Xóa</button>
          </div>
        </div>
      </div>
    </transition>

    <transition name="toast">
      <div v-if="toast.show" class="toast" :class="toast.type">{{ toast.message }}</div>
    </transition>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const API_BASE = "https://localhost:7053/api";
const API_ORDER = `${API_BASE}/Order`;
const API_CAR = `${API_BASE}/Car`; // Giả định API xe của bạn

const orders = ref([]);
const carList = ref([]); // Danh sách xe để hiển thị trong select
const loading = ref(false);
const error = ref(null);
const searchUserId = ref("");
const keyword = ref("");
const pageSize = ref(8);
const currentPage = ref(1);

const toast = reactive({ show: false, message: "", type: "ok" });
const modal = reactive({ open: false, mode: "create", saving: false, editId: null });
const del = reactive({ open: false, saving: false, order: null });
const form = reactive({ userId: null, carId: null, status: "Pending", totalAmount: null, paymentMethod: "Cash", shippingAddress: "" });

watch(keyword, () => (currentPage.value = 1));

// --- Logic Load Data ---
async function fetchList() {
  loading.value = true;
  try {
    const res = await fetch(API_ORDER);
    if (!res.ok) throw new Error("Không thể tải dữ liệu.");
    orders.value = normalizeList(await res.json());
  } catch (e) { error.value = e.message; }
  finally { loading.value = false; }
}

async function fetchCars() {
  try {
    const res = await fetch(API_CAR);
    if (res.ok) carList.value = normalizeList(await res.json());
  } catch (e) { console.error("Lỗi tải danh sách xe:", e); }
}

function onCarChange() {
  const selected = carList.value.find(c => (c.CarId || c.carId) === form.carId);
  if (selected) form.totalAmount = selected.Price || selected.price;
}

// --- API Actions ---
async function submitModal() {
  modal.saving = true;
  try {
    const isEdit = modal.mode === "edit";
    const url = isEdit ? `${API_ORDER}/${modal.editId}` : API_ORDER;
    const method = isEdit ? "PUT" : "POST";

    const fd = new FormData();
    fd.append("userId", form.userId);
    fd.append("carId", form.carId);
    fd.append("status", form.status);
    fd.append("totalAmount", form.totalAmount || 0);
    fd.append("paymentMethod", form.paymentMethod);
    fd.append("shippingAddress", form.shippingAddress);

    const res = await fetch(url, { method, body: fd });

    if (!res.ok && res.status !== 204) {
      const txt = await res.text();
      throw new Error(txt || "Lỗi API");
    }

    showToast(isEdit ? "Cập nhật thành công!" : "Thêm thành công!");
    modal.open = false;
    await fetchList(); 
  } catch (e) {
    showToast(e.message, "err");
  } finally {
    modal.saving = false;
  }
}

async function searchByUserId() {
  if (!searchUserId.value) return reload();
  loading.value = true;
  try {
    const res = await fetch(`${API_ORDER}/search?userId=${searchUserId.value}`);
    orders.value = normalizeList(await res.json());
    currentPage.value = 1;
    showToast("Đã lọc theo khách hàng.");
  } catch (e) { showToast("Lỗi tìm kiếm", "err"); }
  finally { loading.value = false; }
}

async function doDelete() {
  del.saving = true;
  try {
    const id = del.order.OrderId || del.order.orderId;
    const res = await fetch(`${API_ORDER}/${id}`, { method: "DELETE" });
    if (!res.ok && res.status !== 204) throw new Error("Không thể xóa.");
    showToast("Đã xóa đơn hàng.");
    del.open = false;
    await fetchList();
  } catch (e) { showToast(e.message, "err"); }
  finally { del.saving = false; }
}

// --- Helpers & UI ---
function normalizeList(data) {
  if (!data) return [];
  if (Array.isArray(data.$values)) return data.$values;
  return Array.isArray(data) ? data : [];
}

function openCreate() { 
  modal.open = true; modal.mode = "create"; 
  Object.assign(form, { userId: null, carId: null, status: "Pending", totalAmount: null, paymentMethod: "Cash", shippingAddress: "" }); 
}

function openEdit(id) {
  modal.open = true; modal.mode = "edit"; modal.editId = id;
  const item = orders.value.find(o => (o.OrderId || o.orderId) == id);
  if (item) {
    form.userId = item.UserId ?? item.userId;
    form.carId = item.CarId ?? item.carId;
    form.status = item.Status ?? item.status;
    form.totalAmount = item.TotalAmount ?? item.totalAmount;
    form.paymentMethod = item.PaymentMethod ?? item.paymentMethod;
    form.shippingAddress = item.ShippingAddress ?? item.shippingAddress;
  }
}

function goBack() { router.back(); }
function reload() { searchUserId.value = ""; keyword.value = ""; fetchList(); }
function clearKeyword() { keyword.value = ""; }
function closeModal() { if (!modal.saving) modal.open = false; }
function confirmDelete(order) { del.open = true; del.order = order; }

const filteredOrders = computed(() => {
  const k = keyword.value.toLowerCase().trim();
  if (!k) return orders.value;
  return orders.value.filter(o => Object.values(o).some(v => String(v).toLowerCase().includes(k)));
});

const totalPages = computed(() => Math.ceil(filteredOrders.value.length / pageSize.value) || 1);
const pagedOrders = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  return filteredOrders.value.slice(start, start + pageSize.value);
});

function prevPage() { if (currentPage.value > 1) currentPage.value--; }
function nextPage() { if (currentPage.value < totalPages.value) currentPage.value++; }
function goPage(p) { if (p >= 1 && p <= totalPages.value) currentPage.value = p; }

function translateStatus(s) {
  const map = { pending: "⏳ Chờ xử lý", completed: "✅ Hoàn tất", shipped: "🚚 Đang giao", cancel: "❌ Đã hủy" };
  return map[s?.toLowerCase()] || s || "—";
}
function translatePayment(m) {
  const map = { card: "💳 Thẻ", cash: "💵 Tiền mặt", transfer: "🏦 Chuyển khoản", "credit card": "💳 Thẻ tín dụng" };
  return map[m?.toLowerCase()] || m || "—";
}
function formatMoney(v) { return v ? new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(v) : "—"; }
function formatDateTime(d) { return d ? new Date(d).toLocaleString("vi-VN") : "—"; }
function statusClass(s) {
  const status = s?.toLowerCase() || "";
  if (status.includes("comp")) return "ok";
  if (status.includes("pend") || status.includes("ship")) return "warn";
  return "bad";
}
function showToast(msg, type = "ok") {
  toast.show = true; toast.message = msg; toast.type = type;
  setTimeout(() => (toast.show = false), 2500);
}

onMounted(() => {
  fetchList();
  fetchCars();
});
</script>

<style scoped>
/* dùng lại style giống CarManagement để đồng bộ admin */
.page{
  max-width: 1200px;
  margin: 26px auto;
  padding: 0 18px 40px;
  font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Arial;
  color:#0f172a;
}
.muted{ color:#64748b; }
.mono{ font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", monospace; }
h1{ margin:0; font-size: 22px; letter-spacing: .2px; }

.card{
  background: rgba(255,255,255,.78);
  border: 1px solid rgba(15,23,42,.08);
  border-radius: 16px;
  padding: 16px;
  box-shadow: 0 10px 28px rgba(2,6,23,.08);
  backdrop-filter: blur(10px);
}
.search-card{ margin: 14px 0; }

/* Topbar */
.topbar{
  display:flex;
  align-items:center;
  justify-content:space-between;
  gap: 16px;
}
.title-wrap{
  display:flex;
  align-items:center;
  gap: 12px;
}

/* Buttons */
.btn{
  appearance:none;
  border: 1px solid rgba(15,23,42,.15);
  background: #fff;
  color:#0f172a;
  padding: 10px 12px;
  border-radius: 12px;
  cursor:pointer;
  transition: transform .15s ease, box-shadow .15s ease, background .15s ease, border-color .15s ease, opacity .15s ease;
  box-shadow: 0 6px 16px rgba(2,6,23,.06);
}
.btn:hover{ transform: translateY(-1px); box-shadow: 0 10px 22px rgba(2,6,23,.10); }
.btn:active{ transform: translateY(0); }
.btn:disabled{ opacity:.55; cursor:not-allowed; transform:none; box-shadow:none; }
.btn.primary{
  background: linear-gradient(135deg, #2563eb, #7c3aed);
  border-color: transparent;
  color:#fff;
}
.btn.ghost{ background: transparent; box-shadow:none; }
.btn.danger{
  background: linear-gradient(135deg, #ef4444, #f97316);
  border-color: transparent;
  color:#fff;
}
.btn.sm{ padding: 8px 10px; border-radius: 10px; }

/* Search */
.search-grid{
  display:grid;
  grid-template-columns: 1.2fr 1.5fr .8fr;
  gap: 14px;
  align-items:end;
}
@media (max-width: 980px){
  .search-grid{ grid-template-columns: 1fr; }
}
.field label{
  display:block;
  font-size: 12px;
  color:#334155;
  margin-bottom: 6px;
}
.hint{ display:block; margin-top:6px; color:#94a3b8; font-size:12px; }
.input-group{ display:flex; gap: 10px; }
input{
  width: 100%;
  padding: 10px 12px;
  border-radius: 12px;
  border: 1px solid rgba(15,23,42,.15);
  outline: none;
  background: rgba(255,255,255,.9);
  transition: box-shadow .15s ease, border-color .15s ease;
}
input:focus{
  border-color: rgba(37,99,235,.55);
  box-shadow: 0 0 0 4px rgba(37,99,235,.12);
}

/* Table */
.table-wrap{ overflow:auto; border-radius: 14px; }
.table{
  width: 100%;
  border-collapse: collapse;
  min-width: 980px;
}
.table.big{ min-width: 1200px; }
thead th{
  text-align:left;
  font-size: 12px;
  letter-spacing: .3px;
  text-transform: uppercase;
  color:#475569;
  padding: 12px 12px;
  background: rgba(2,6,23,.03);
  border-bottom: 1px solid rgba(15,23,42,.10);
}
tbody td{
  padding: 12px 12px;
  border-bottom: 1px solid rgba(15,23,42,.08);
  vertical-align: top;
}
tr.row{ transition: background .15s ease, transform .15s ease; }
tr.row:hover{ background: rgba(37,99,235,.06); }
.btn-row{ display:flex; gap: 8px; }

.table-head{
  display:flex;
  align-items:center;
  justify-content:space-between;
  padding: 6px 2px 12px;
}
.badge{
  display:inline-flex;
  align-items:center;
  justify-content:center;
  min-width: 28px;
  height: 22px;
  padding: 0 8px;
  border-radius: 999px;
  background: rgba(37,99,235,.12);
  color:#1d4ed8;
  font-weight: 700;
  margin-right: 6px;
}

/* line clamp */
.line2{
  display:-webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow:hidden;
}

/* Pills */
.pill{
  display:inline-flex;
  padding: 6px 10px;
  border-radius: 999px;
  font-size: 12px;
  border: 1px solid rgba(15,23,42,.10);
}
.pill.ok{ background: rgba(34,197,94,.10); color:#15803d; border-color: rgba(34,197,94,.22); }
.pill.warn{ background: rgba(245,158,11,.10); color:#92400e; border-color: rgba(245,158,11,.22); }
.pill.bad{ background: rgba(239,68,68,.10); color:#b91c1c; border-color: rgba(239,68,68,.22); }
.pill.neutral{ background: rgba(100,116,139,.10); color:#475569; border-color: rgba(100,116,139,.22); }

/* Alerts / Empty */
.alert{
  padding: 12px 12px;
  border-radius: 14px;
  margin: 10px 0;
  border: 1px solid rgba(15,23,42,.12);
}
.alert.error{ background: rgba(239,68,68,.08); border-color: rgba(239,68,68,.22); color:#991b1b; }
.empty{ padding: 16px; text-align:center; color:#64748b; }

/* Pager */
.pager{
  display:flex;
  align-items:center;
  justify-content:space-between;
  gap:12px;
  padding: 14px 6px 0;
}
.pager-right{ display:flex; align-items:center; gap:10px; }
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

/* Modal */
.modal-backdrop{
  position: fixed;
  inset: 0;
  background: rgba(2,6,23,.55);
  display:flex;
  align-items:center;
  justify-content:center;
  padding: 18px;
  z-index: 50;
}
.modal{
  width: min(860px, 100%);
  background: #fff;
  border-radius: 18px;
  border: 1px solid rgba(15,23,42,.12);
  box-shadow: 0 20px 60px rgba(2,6,23,.30);
  overflow:hidden;
}
.modal.small{ width: min(520px, 100%); }
.modal-header{
  display:flex;
  align-items:flex-start;
  justify-content:space-between;
  gap: 12px;
  padding: 16px 16px 0;
}
.modal-header h2{ margin:0; font-size: 18px; }
.form{ padding: 16px; }
.grid{
  display:grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}
@media (max-width: 860px){
  .grid{ grid-template-columns: 1fr; }
}
.span2{ grid-column: span 2; }
@media (max-width: 860px){
  .span2{ grid-column: auto; }
}
.modal-footer{
  display:flex;
  justify-content:flex-end;
  gap: 10px;
  padding: 14px 16px 16px;
  border-top: 1px solid rgba(15,23,42,.08);
}
.confirm-body{ padding: 14px 16px; color:#0f172a; }

/* Animations */
.row-enter-active, .row-leave-active{ transition: all .18s ease; }
.row-enter-from{ opacity: 0; transform: translateY(6px); }
.row-leave-to{ opacity: 0; transform: translateY(6px); }

.modal-enter-active, .modal-leave-active{ transition: opacity .18s ease; }
.modal-enter-from, .modal-leave-to{ opacity: 0; }

/* Toast */
.toast{
  position: fixed;
  right: 18px;
  bottom: 18px;
  color:#fff;
  padding: 12px 14px;
  border-radius: 14px;
  box-shadow: 0 18px 44px rgba(2,6,23,.35);
  z-index: 60;
}
.toast.ok{ background: linear-gradient(135deg,#16a34a,#22c55e); }
.toast.warn{ background: linear-gradient(135deg,#f59e0b,#f97316); }
.toast.err{ background: linear-gradient(135deg,#ef4444,#f97316); }

.toast-enter-active, .toast-leave-active{ transition: all .18s ease; }
.toast-enter-from{ opacity: 0; transform: translateY(10px); }
.toast-leave-to{ opacity: 0; transform: translateY(10px); }

/* Spinner */
.spinner{
  display:inline-block;
  width: 14px;
  height: 14px;
  border-radius: 999px;
  border: 2px solid rgba(255,255,255,.55);
  border-top-color: rgba(255,255,255,1);
  margin-right: 8px;
  animation: spin .7s linear infinite;
}
.text-primary { color: #2563eb; }
.text-success { color: #16a34a; }
.text-danger { color: #dc2626; }
/* Làm cho ô Select giống hệt ô Input */
.custom-select {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background-color: white;
  font-size: 14px;
  outline: none;
  appearance: none; /* Xóa mũi tên mặc định của trình duyệt */
  background-image: url("data:image/svg+xml;charset=UTF-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 12px center;
  background-size: 16px;
  transition: border-color 0.2s;
}

.custom-select:focus {
  border-color: var(--primary); /* Hoặc màu xanh của bạn */
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}
@keyframes spin{ to{ transform: rotate(360deg); } }
</style>
