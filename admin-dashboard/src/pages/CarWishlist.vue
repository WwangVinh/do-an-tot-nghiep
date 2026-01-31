<template>
  <div class="page">
    <!-- Top bar -->
    <header class="topbar">
      <div class="title-wrap">
        <button class="btn ghost" @click="goBack">← Quay lại</button>
        <div>
          <h1>Quản lý Wishlist</h1>
          <p class="sub muted">Quản lý xe yêu thích của người dùng (CarWishlist)</p>
        </div>
      </div>

      <div class="actions">
        <button class="btn primary" @click="openCreate">+ Thêm wishlist</button>
      </div>
    </header>

    <!-- Search bar -->
    <section class="card search-card">
      <div class="search-grid">
        <<div class="field">
  <label>Tìm theo tên xe</label>
  <div class="input-group">
    <input v-model.trim="carNameKey" type="text" placeholder="Ví dụ: Porsche" />
    <button class="btn" @click="carNameKey=''">Xóa</button>
  </div>
</div>

        <div class="field">
  <label>Tìm theo tên user</label>
  <div class="input-group">
    <input v-model.trim="userNameKey" type="text" placeholder="Ví dụ: Nguyen Van Admin" />
    <button class="btn" @click="userNameKey=''">Xóa</button>
  </div>
</div>

        <div class="field">
          <label>Lọc nhanh (frontend)</label>
          <div class="input-group">
            <input v-model.trim="keyword" type="text" placeholder="wishlistId / userId / carId..." />
            <button class="btn" @click="clearKeyword" :disabled="loading && items.length===0">Xóa</button>
          </div>
        </div>

        <div class="field right">
          <label>&nbsp;</label>
          <div class="input-group">
            <button class="btn" @click="reload" :disabled="loading">↻ Tải lại</button>
          </div>
        </div>
      </div>
    </section>

    <!-- Table -->
    <section class="card">
      <div class="table-head">
        <div class="left">
          <span class="badge">{{ filteredItems.length }}</span>
          <span class="muted">wishlist</span>
          <span class="muted"> • Trang {{ currentPage }} / {{ totalPages }}</span>
        </div>
        <div class="right muted" v-if="loading">Đang tải...</div>
      </div>

      <div v-if="error" class="alert error">
        <b>Lỗi:</b> {{ error }}
      </div>

      <div class="table-wrap">
        <table class="table big">
          <thead>
            <tr>
              <th>WishlistId</th>
<th>Tên user</th>
<th>Tên xe</th>
              <th>AddedAt</th>
              <th>Thao tác</th>
            </tr>
          </thead>

          <transition-group name="row" tag="tbody">
            <tr v-for="w in pagedItems" :key="w.wishlistId" class="row">
              <td class="mono strong">{{ w.wishlistId }}</td>
              <td class="mono">{{ w.userName  ?? "—" }}</td>
              <td class="mono">{{ w.carName   ?? "—" }}</td>
              <td class="mono">{{ formatDate(w.addedAt) }}</td>
              <td>
                <div class="btn-row">
                  <button class="btn sm" @click="openEdit(w.wishlistId)">Sửa</button>
                  <button class="btn sm danger" @click="confirmDelete(w)">Xóa</button>
                </div>
              </td>
            </tr>
          </transition-group>
        </table>
      </div>

      <!-- Pager -->
      <div class="pager" v-if="filteredItems.length > 0">
        <div class="pager-left">
          <span class="muted">Hiển thị <b>{{ pagedItems.length }}</b> / <b>{{ filteredItems.length }}</b></span>
        </div>

        <div class="pager-right">
          <button class="btn sm" @click="prevPage" :disabled="currentPage === 1">‹ Trước</button>

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

          <button class="btn sm" @click="nextPage" :disabled="currentPage === totalPages">Sau ›</button>
        </div>
      </div>

      <div v-if="filteredItems.length === 0 && !loading" class="empty">
        Không có dữ liệu
      </div>
    </section>

    <!-- Modal Create/Edit -->
    <transition name="modal">
      <div v-if="modal.open" class="modal-backdrop" @mousedown.self="closeModal">
        <div class="modal">
          <div class="modal-header">
            <div>
              <h2>{{ modal.mode === "create" ? "Thêm wishlist" : "Sửa wishlist" }}</h2>
              <p class="muted sub">POST/PUT dùng multipart/form-data</p>
            </div>
            <button class="btn ghost" @click="closeModal">✕</button>
          </div>

          <form class="form" @submit.prevent="submitModal">
            <div class="grid">
              <div class="field">
                <label>UserId <span class="req">*</span></label>
                <input v-model.number="form.userId" type="number" min="1" placeholder="2" required />
              </div>

              <div class="field">
                <label>CarId <span class="req">*</span></label>
                <input v-model.number="form.carId" type="number" min="1" placeholder="5" required />
              </div>

              <div class="field span2">
                <label>AddedAt (tự động)</label>
                <input :value="modal.mode === 'edit' ? (formatDate(form.addedAt) || '—') : 'Tự set khi tạo'" disabled />
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
      <div v-if="del.open" class="modal-backdrop" @mousedown.self="del.open=false">
        <div class="modal small">
          <div class="modal-header">
            <div><h2>Xác nhận xóa</h2></div>
            <button class="btn ghost" @click="del.open=false">✕</button>
          </div>

          <div class="confirm-body">
            Xóa wishlist ID <b>{{ del.item?.WishlistId }}</b> ?
            <div class="muted" style="margin-top:8px">
              UserId: <b>{{ del.item?.UserId }}</b> • CarId: <b>{{ del.item?.CarId }}</b>
            </div>
          </div>

          <div class="modal-footer">
            <button class="btn" @click="del.open=false">Hủy</button>
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

const router = useRouter();

const API_BASE = "https://localhost:7053/api";
const API_WISHLIST = `${API_BASE}/CarWishlist`;
const API_CAR = `${API_BASE}/Car`;
const API_USER = `${API_BASE}/User`;

/** Data */
const items = ref([]);      // wishlist (đã enrich)
const rawItems = ref([]);   // wishlist raw từ API
const cars = ref([]);       // cars list
const users = ref([]);      // users list

const loading = ref(false);
const error = ref(null);

/** Search inputs */
const searchUserId = ref("");
const searchCarId = ref("");
const keyword = ref("");

const carNameKey = ref("");   // tìm theo tên xe
const userNameKey = ref("");  // tìm theo tên user

/** Pagination */
const pageSize = ref(8);
const currentPage = ref(1);
watch([keyword, carNameKey, userNameKey, searchUserId, searchCarId], () => {
  currentPage.value = 1;
});

/** Toast */
const toast = reactive({ show: false, message: "", type: "ok" });
let toastTimer = null;
function showToast(message, type = "ok") {
  toast.show = true;
  toast.message = message;
  toast.type = type;
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => (toast.show = false), 2200);
}

/** Utils */
function normalizeList(data) {
  // .NET list đôi khi trả { "$values": [...] }, user của bạn trả array thuần
  if (Array.isArray(data)) return data;
  if (data && Array.isArray(data.$values)) return data.$values;
  return [];
}

function formatDate(d) {
  if (!d) return "—";
  return new Date(d).toLocaleString("vi-VN");
}

function goBack() {
  router.back();
}

/** Build map: carId -> name, userId -> fullName/username */
const carNameById = computed(() => {
  const map = new Map();
  for (const c of cars.value) {
    map.set(c.carId ?? c.CarId, c.name ?? c.Name);
  }
  return map;
});

const userNameById = computed(() => {
  const map = new Map();
  for (const u of users.value) {
    const id = u.userId ?? u.UserId;
    const name = u.fullName ?? u.FullName ?? u.username ?? u.Username;
    map.set(id, name || "—");
  }
  return map;
});

/** Enrich wishlist: thêm carName + userName */
function enrichWishlist(list) {
  return list.map((w) => {
    const wishlistId = w.wishlistId ?? w.WishlistId;
    const userId = w.userId ?? w.UserId;
    const carId = w.carId ?? w.CarId;
    const addedAt = w.addedAt ?? w.AddedAt;

    return {
      wishlistId,
      userId,
      carId,
      addedAt,
      carName: carNameById.value.get(carId) || "—",
      userName: userNameById.value.get(userId) || "—",
    };
  });
}

/** API fetch */
async function fetchCars() {
  try {
    const res = await fetch(API_CAR, { headers: { accept: "text/plain" } });
    if (!res.ok) throw new Error(`GET cars failed: ${res.status}`);
    const data = await res.json();
    cars.value = normalizeList(data);
  } catch (e) {
    showToast("Không tải được danh sách xe (map tên xe).", "warn");
  }
}

async function fetchUsers() {
  try {
    const res = await fetch(API_USER, { headers: { accept: "text/plain" } });
    if (!res.ok) throw new Error(`GET users failed: ${res.status}`);
    const data = await res.json();
    users.value = normalizeList(data); // user API trả array -> ok
  } catch (e) {
    showToast("Không tải được danh sách user (map tên user).", "warn");
  }
}

async function fetchWishlist(url = API_WISHLIST) {
  loading.value = true;
  error.value = null;
  try {
    const res = await fetch(url, { headers: { accept: "text/plain" } });
    if (!res.ok) throw new Error(`GET wishlist failed: ${res.status}`);
    const data = await res.json();
    rawItems.value = normalizeList(data);
    items.value = enrichWishlist(rawItems.value);
  } catch (e) {
    error.value = e?.message || "Không thể tải wishlist.";
  } finally {
    loading.value = false;
  }
}

/** Search by query params (userId/carId) */
function buildQuery() {
  const u = searchUserId.value?.toString().trim();
  const c = searchCarId.value?.toString().trim();
  const qs = new URLSearchParams();
  if (u) qs.set("userId", u);
  if (c) qs.set("carId", c);
  return qs.toString();
}

function applySearch() {
  const qs = buildQuery();
  const url = qs ? `${API_WISHLIST}?${qs}` : API_WISHLIST;
  fetchWishlist(url);
}

function reload() {
  searchUserId.value = "";
  searchCarId.value = "";
  keyword.value = "";
  carNameKey.value = "";
  userNameKey.value = "";
  fetchWishlist(API_WISHLIST);
}

function clearKeyword() {
  keyword.value = "";
  carNameKey.value = "";
  userNameKey.value = "";
}

/** Frontend filter: keyword + tên xe + tên user */
const filteredItems = computed(() => {
  let list = items.value;

  const k = keyword.value.toLowerCase().trim();
  if (k) {
    list = list.filter((x) => {
      const hay = [
        x.wishlistId,
        x.userId,
        x.carId,
        x.addedAt,
        x.carName,
        x.userName,
      ]
        .filter((v) => v !== null && v !== undefined)
        .join(" ")
        .toLowerCase();
      return hay.includes(k);
    });
  }

  const ck = carNameKey.value.toLowerCase().trim();
  if (ck) list = list.filter((x) => (x.carName || "").toLowerCase().includes(ck));

  const uk = userNameKey.value.toLowerCase().trim();
  if (uk) list = list.filter((x) => (x.userName || "").toLowerCase().includes(uk));

  return list;
});

/** Pagination computed */
const totalPages = computed(() =>
  Math.max(1, Math.ceil(filteredItems.value.length / pageSize.value))
);

const pagedItems = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  return filteredItems.value.slice(start, start + pageSize.value);
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

/** Modal Create/Edit + Delete: giữ như bản trước của bạn (POST/PUT/DELETE vẫn dùng FormData) */
const modal = reactive({ open: false, mode: "create", saving: false, editId: null });
const form = reactive({ userId: null, carId: null, addedAt: null });

function resetForm() {
  form.userId = null;
  form.carId = null;
  form.addedAt = null;
}

function openCreate() {
  modal.open = true;
  modal.mode = "create";
  modal.editId = null;
  resetForm();
}

async function openEdit(id) {
  modal.open = true;
  modal.mode = "edit";
  modal.editId = id;
  resetForm();

  const local = items.value.find((x) => x.wishlistId === id);
  if (local) {
    form.userId = local.userId ?? null;
    form.carId = local.carId ?? null;
    form.addedAt = local.addedAt ?? null;
    return;
  }

  modal.saving = true;
  try {
    const res = await fetch(`${API_WISHLIST}/${id}`, { headers: { accept: "text/plain" } });
    if (!res.ok) throw new Error(`Load wishlist failed: ${res.status}`);
    const w = await res.json();

    form.userId = w.userId ?? w.UserId ?? null;
    form.carId = w.carId ?? w.CarId ?? null;
    form.addedAt = w.addedAt ?? w.AddedAt ?? null;
  } catch (e) {
    showToast(e?.message || "Không tải được wishlist.", "err");
    modal.open = false;
  } finally {
    modal.saving = false;
  }
}

function closeModal() {
  if (modal.saving) return;
  modal.open = false;
}

function buildFormData() {
  const fd = new FormData();
  fd.append("userId", String(form.userId ?? ""));
  fd.append("carId", String(form.carId ?? ""));
  return fd;
}

async function submitModal() {
  if (!form.userId || form.userId < 1) return showToast("UserId là bắt buộc.", "warn");
  if (!form.carId || form.carId < 1) return showToast("CarId là bắt buộc.", "warn");

  modal.saving = true;
  try {
    const fd = buildFormData();

    if (modal.mode === "create") {
      const res = await fetch(API_WISHLIST, {
        method: "POST",
        headers: { accept: "text/plain" },
        body: fd,
      });
      if (res.status === 409) return showToast("Xe này đã có trong wishlist.", "warn");
      if (!res.ok) throw new Error(`POST failed: ${res.status}`);
      showToast("Thêm wishlist thành công.", "ok");
      modal.open = false;
      await fetchWishlist(API_WISHLIST);
      return;
    }

    const res = await fetch(`${API_WISHLIST}/${modal.editId}`, {
      method: "PUT",
      headers: { accept: "*/*" },
      body: fd,
    });
    if (!(res.status === 204 || res.ok)) throw new Error(`PUT failed: ${res.status}`);
    showToast("Cập nhật thành công.", "ok");
    modal.open = false;
    await fetchWishlist(API_WISHLIST);
  } catch (e) {
    showToast(e?.message || "Thao tác thất bại.", "err");
  } finally {
    modal.saving = false;
  }
}

/** Delete */
const del = reactive({ open: false, saving: false, item: null });

function confirmDelete(item) {
  del.open = true;
  del.item = item;
}

async function doDelete() {
  if (!del.item) return;
  del.saving = true;
  try {
    const res = await fetch(`${API_WISHLIST}/${del.item.wishlistId}`, {
      method: "DELETE",
      headers: { accept: "*/*" },
    });
    if (!(res.status === 204 || res.ok)) throw new Error(`DELETE failed: ${res.status}`);
    showToast("Đã xóa.", "ok");
    del.open = false;
    await fetchWishlist(API_WISHLIST);
  } catch (e) {
    showToast(e?.message || "Xóa thất bại.", "err");
  } finally {
    del.saving = false;
  }
}

/** Init */
onMounted(async () => {
  await Promise.all([fetchCars(), fetchUsers()]);
  await fetchWishlist(API_WISHLIST);
});
</script>

<style scoped>
/* ====== Base ====== */
.page{
  max-width: 1200px;
  margin: 26px auto;
  padding: 0 18px 40px;
  font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Arial;
  color:#0f172a;
}
.muted{ color:#64748b; }
.mono{ font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", monospace; }
.strong{ font-weight: 750; }
.sub{ margin:4px 0 0; }

/* ====== Cards ====== */
.card{
  background: rgba(255,255,255,.78);
  border: 1px solid rgba(15,23,42,.08);
  border-radius: 16px;
  padding: 16px;
  box-shadow: 0 10px 28px rgba(2,6,23,.08);
  backdrop-filter: blur(10px);
}
.search-card{ margin: 14px 0; }

/* ====== Topbar ====== */
.topbar{
  display:flex;
  align-items:center;
  justify-content:space-between;
  gap: 16px;
}
.title-wrap{
  display:flex;
  align-items:flex-start;
  gap: 12px;
}
h1{ margin:0; font-size: 22px; letter-spacing: .2px; }

/* ====== Buttons ====== */
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
.btn.ghost{
  background: transparent;
  box-shadow:none;
}
.btn.danger{
  background: linear-gradient(135deg, #ef4444, #f97316);
  border-color: transparent;
  color:#fff;
}
.btn.sm{ padding: 8px 10px; border-radius: 10px; }

/* ====== Search ====== */
.search-grid{
  display:grid;
  grid-template-columns: 1.1fr 1.1fr 1.4fr .7fr;
  gap: 14px;
  align-items:end;
}
@media (max-width: 1100px){
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

/* ====== Table ====== */
.table-wrap{ overflow:auto; border-radius: 14px; }
.table{
  width: 100%;
  border-collapse: collapse;
  min-width: 900px;
}
.table.big{ min-width: 980px; }

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

/* ====== Pager ====== */
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

/* ====== Alerts / Empty ====== */
.alert{
  padding: 12px 12px;
  border-radius: 14px;
  margin: 10px 0;
  border: 1px solid rgba(15,23,42,.12);
}
.alert.error{ background: rgba(239,68,68,.08); border-color: rgba(239,68,68,.22); color:#991b1b; }
.empty{ padding: 16px; text-align:center; color:#64748b; }

/* ====== Modal ====== */
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
@media (max-width: 860px){ .grid{ grid-template-columns: 1fr; } }
.span2{ grid-column: span 2; }
@media (max-width: 860px){ .span2{ grid-column: auto; } }

.req{ color:#ef4444; }
.modal-footer{
  display:flex;
  justify-content:flex-end;
  gap: 10px;
  padding: 14px 16px 16px;
  border-top: 1px solid rgba(15,23,42,.08);
}
.confirm-body{ padding: 14px 16px; color:#0f172a; }

/* ====== Animations ====== */
.row-enter-active, .row-leave-active{ transition: all .18s ease; }
.row-enter-from{ opacity: 0; transform: translateY(6px); }
.row-leave-to{ opacity: 0; transform: translateY(6px); }

.modal-enter-active, .modal-leave-active{ transition: opacity .18s ease; }
.modal-enter-from, .modal-leave-to{ opacity: 0; }

/* ====== Toast ====== */
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

/* ====== Spinner ====== */
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
@keyframes spin{ to{ transform: rotate(360deg); } }
</style>
