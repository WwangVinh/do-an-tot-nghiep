<template>
  <div class="page">
    <header class="topbar">
      <div class="title-wrap">
        <button class="btn ghost" @click="router.back()">← Quay lại</button>
        <div>
          <h1>📝 Chi tiết Đơn hàng</h1>
          <span class="muted small">Quản lý danh sách sản phẩm trong đơn</span>
        </div>
      </div>
      <div class="actions">
        <button class="btn primary" @click="openCreate">+ Thêm sản phẩm</button>
      </div>
    </header>

    <section class="card search-card">
      <div class="search-grid">
        <div class="field">
          <label>Tìm theo Mã Đơn (OrderId)</label>
          <div class="input-group">
            <input v-model.number="searchOrderId" type="number" placeholder="Nhập mã đơn..." @keyup.enter="fetchList" />
            <button class="btn" @click="fetchList">Tìm</button>
          </div>
        </div>
        <div class="field">
          <label>Từ khóa sản phẩm</label>
          <div class="input-group">
            <input v-model.trim="keyword" type="text" placeholder="Tên xe, giá..." />
            <button class="btn" @click="keyword = ''">Xóa</button>
          </div>
        </div>
        <div class="field right">
          <label>&nbsp;</label>
          <button class="btn" @click="reload" :disabled="loading">↻ Làm mới</button>
        </div>
      </div>
    </section>

    <section class="card">
      <div class="table-head">
        <div class="left">
          <span class="badge">{{ filteredItems.length }}</span>
          <span class="muted">mục đơn hàng • Trang {{ currentPage }} / {{ totalPages }}</span>
        </div>
        <div class="right muted" v-if="loading">Đang kết nối API...</div>
      </div>

      <div class="table-wrap">
        <table class="table big">
          <thead>
            <tr>
              <th>ID</th>
              <th>Mã Đơn</th>
              <th>Tên Xe (Sản phẩm)</th>
              <th>Số lượng</th>
              <th>Đơn giá</th>
              <th>Thành tiền</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <transition-group name="row" tag="tbody">
            <tr v-for="item in pagedItems" :key="item.OrderItemId || item.orderItemId" class="row">
              <td class="mono">#{{ item.OrderItemId || item.orderItemId }}</td>
              <td class="strong">Đơn #{{ item.OrderId || item.orderId }}</td>
              <td class="text-primary strong">
                {{ item.Car?.Name || item.car?.name || ('Xe #' + (item.CarId || item.carId)) }}
              </td>
              <td><b>{{ item.Quantity || item.quantity }}</b></td>
              <td class="mono">{{ formatMoney(item.Price || item.price) }}</td>
              <td class="mono strong text-success">
                {{ formatMoney((item.Price || item.price) * (item.Quantity || item.quantity)) }}
              </td>
              <td>
                <div class="btn-row">
                  <button class="btn sm" @click="openEdit(item)">Sửa</button>
                  <button class="btn sm danger" @click="confirmDelete(item)">Xóa</button>
                </div>
              </td>
            </tr>
          </transition-group>
        </table>
      </div>

      <div class="pager" v-if="filteredItems.length > 0">
        <div class="pager-left muted">Hiển thị <b>{{ pagedItems.length }}</b> sản phẩm</div>
        <div class="pager-right">
          <button class="btn sm" :disabled="currentPage === 1" @click="currentPage--">‹ Trước</button>
          <div class="page-indicator">Trang {{ currentPage }} / {{ totalPages }}</div>
          <button class="btn sm" :disabled="currentPage === totalPages" @click="currentPage++">Sau ›</button>
        </div>
      </div>
      <div v-else-if="!loading" class="empty">📭 Không tìm thấy chi tiết nào.</div>
    </section>

    <transition name="modal">
      <div v-if="modal.open" class="modal-backdrop" @mousedown.self="modal.open = false">
        <div class="modal">
          <div class="modal-header">
            <h2>{{ modal.mode === 'create' ? '➕ Thêm sản phẩm vào đơn' : '📝 Cập nhật mục #' + modal.editId }}</h2>
            <button class="btn ghost" @click="modal.open = false">✕</button>
          </div>
          <form @submit.prevent="handleSubmit" class="form">
            <div class="grid">
              <div class="field">
                <label>Chọn Đơn hàng</label>
                <select v-model="form.orderId" class="custom-select" required>
                  <option :value="null" disabled>-- Chọn mã đơn --</option>
                  <option v-for="o in orderList" :key="o.OrderId || o.orderId" :value="o.OrderId || o.orderId">
                    Đơn #{{ o.OrderId || o.orderId }} (Khách #{{ o.UserId || o.userId }})
                  </option>
                </select>
              </div>

              <div class="field">
                <label>Chọn Xe (Sản phẩm)</label>
                <select v-model="form.carId" class="custom-select" required @change="updatePrice">
                  <option :value="null" disabled>-- Chọn dòng xe --</option>
                  <option v-for="c in carList" :key="c.CarId || c.carId" :value="c.CarId || c.carId">
                    {{ c.Name || c.name }} - {{ formatMoney(c.Price || c.price) }}
                  </option>
                </select>
              </div>

              <div class="field">
                <label>Số lượng</label>
                <input v-model.number="form.quantity" type="number" min="1" required />
              </div>

              <div class="field">
                <label>Giá bán hiện tại (VNĐ)</label>
                <input v-model.number="form.price" type="number" step="0.01" placeholder="Tự động lấy theo giá xe..." />
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn" @click="modal.open = false">Hủy</button>
              <button type="submit" class="btn primary" :disabled="modal.saving">
                <span v-if="modal.saving" class="spinner"></span>
                {{ modal.saving ? 'Đang lưu...' : 'Xác nhận' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </transition>

    <transition name="modal">
      <div v-if="del.open" class="modal-backdrop" @mousedown.self="del.open = false">
        <div class="modal small">
          <div class="modal-header"><h2>⚠️ Xác nhận xóa</h2></div>
          <div class="confirm-body">Xóa sản phẩm này khỏi đơn hàng? Hành động này không thể hoàn tác.</div>
          <div class="modal-footer">
            <button class="btn" @click="del.open = false">Hủy</button>
            <button class="btn danger" @click="doDelete" :disabled="del.saving">
              {{ del.saving ? 'Đang xóa...' : 'Xóa ngay' }}
            </button>
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
import { computed, onMounted, reactive, ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const API_BASE = "https://localhost:7053/api";

// --- STATES ---
const items = ref([]);
const carList = ref([]);
const orderList = ref([]);
const loading = ref(false);
const keyword = ref("");
const searchOrderId = ref(null);
const currentPage = ref(1);
const pageSize = 8;

const toast = reactive({ show: false, message: "", type: "ok" });
const modal = reactive({ open: false, mode: 'create', saving: false, editId: null });
const del = reactive({ open: false, saving: false, orderItem: null });
const form = reactive({ orderId: null, carId: null, quantity: 1, price: 0 });

// --- HELPERS ---
const formatMoney = (v) => v ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v) : "—";

const showToast = (msg, type = "ok") => {
  toast.show = true; toast.message = msg; toast.type = type;
  setTimeout(() => (toast.show = false), 2500);
};

const normalize = (data) => data?.$values || (Array.isArray(data) ? data : []);

// --- API ACTIONS ---
const fetchList = async () => {
  loading.value = true;
  try {
    const res = await fetch(`${API_BASE}/OrderItem`);
    items.value = normalize(await res.json());
  } catch (e) { 
    showToast("Lỗi tải danh sách", "err"); 
  } finally { 
    loading.value = false; 
  }
};

const fetchRefs = async () => {
  try {
    const [cRes, oRes] = await Promise.all([
      fetch(`${API_BASE}/Car`),
      fetch(`${API_BASE}/Order`)
    ]);
    carList.value = normalize(await cRes.json());
    orderList.value = normalize(await oRes.json());
  } catch (e) { 
    console.error("Lỗi tải dữ liệu tham chiếu (Xe/Đơn hàng)"); 
  }
};

const updatePrice = () => {
  const selected = carList.value.find(c => (c.CarId || c.carId) === form.carId);
  if (selected) form.price = selected.Price || selected.price;
};

const handleSubmit = async () => {
  modal.saving = true;
  try {
    const isEdit = modal.mode === 'edit';
    const url = isEdit ? `${API_BASE}/OrderItem/${modal.editId}` : `${API_BASE}/OrderItem`;
    
    // Sử dụng FormData để khớp với [FromForm] ở Backend
    const fd = new FormData();
    fd.append("orderId", form.orderId);
    fd.append("carId", form.carId);
    fd.append("quantity", form.quantity);
    fd.append("price", form.price);

    const res = await fetch(url, { 
      method: isEdit ? 'PUT' : 'POST', 
      body: fd 
    });

    if (!res.ok && res.status !== 204) throw new Error("Gửi dữ liệu lên Server thất bại");

    showToast(isEdit ? "Cập nhật thành công" : "Thêm vào đơn hàng thành công");
    modal.open = false;
    fetchList();
  } catch (e) { 
    showToast(e.message, "err"); 
  } finally { 
    modal.saving = false; 
  }
};

const doDelete = async () => {
  del.saving = true;
  try {
    const id = del.orderItem.OrderItemId || del.orderItem.orderItemId;
    const res = await fetch(`${API_BASE}/OrderItem/${id}`, { method: 'DELETE' });
    if (!res.ok) throw new Error();
    
    showToast("Đã xóa sản phẩm khỏi đơn");
    del.open = false;
    fetchList();
  } catch (e) { 
    showToast("Xóa thất bại", "err"); 
  } finally { 
    del.saving = false; 
  }
};

// --- UI LOGIC ---
const openCreate = () => {
  modal.mode = 'create'; 
  modal.open = true;
  Object.assign(form, { orderId: null, carId: null, quantity: 1, price: 0 });
};

const openEdit = (item) => {
  modal.mode = 'edit'; 
  modal.open = true;
  modal.editId = item.OrderItemId || item.orderItemId;
  Object.assign(form, {
    orderId: item.OrderId || item.orderId,
    carId: item.CarId || item.carId,
    quantity: item.Quantity || item.quantity,
    price: item.Price || item.price
  });
};

const confirmDelete = (item) => { 
  del.orderItem = item; 
  del.open = true; 
};

const reload = () => { 
  searchOrderId.value = null; 
  keyword.value = ""; 
  fetchList(); 
};

// --- COMPUTED (Lọc & Phân trang) ---
const filteredItems = computed(() => {
  return items.value.filter(i => {
    // Lọc theo mã đơn hàng nếu có nhập
    const matchId = searchOrderId.value ? (i.OrderId || i.orderId) === searchOrderId.value : true;
    // Lọc theo từ khóa (tên xe hoặc giá)
    const matchK = keyword.value ? JSON.stringify(i).toLowerCase().includes(keyword.value.toLowerCase()) : true;
    return matchId && matchK;
  });
});

const totalPages = computed(() => Math.ceil(filteredItems.value.length / pageSize) || 1);

const pagedItems = computed(() => {
  const start = (currentPage.value - 1) * pageSize;
  return filteredItems.value.slice(start, start + pageSize);
});

// Khởi chạy
onMounted(() => { 
  fetchList(); 
  fetchRefs(); 
});
</script>
<style scoped>
/* --- Layout Chung --- */
.page {
  max-width: 1200px;
  margin: 26px auto;
  padding: 0 18px 40px;
  font-family: ui-sans-serif, system-ui, -apple-system, Segoe UI, Roboto, Arial;
  color: #0f172a;
}
.muted { color: #64748b; }
.mono { font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace; }
h1 { margin: 0; font-size: 22px; letter-spacing: .2px; }

/* --- Card & Backdrop --- */
.card {
  background: rgba(255, 255, 255, .78);
  border: 1px solid rgba(15, 23, 42, .08);
  border-radius: 16px;
  padding: 16px;
  box-shadow: 0 10px 28px rgba(2, 6, 23, .08);
  backdrop-filter: blur(10px);
}
.search-card { margin: 14px 0; }

/* --- Topbar & Buttons --- */
.topbar { display: flex; align-items: center; justify-content: space-between; gap: 16px; }
.title-wrap { display: flex; align-items: center; gap: 12px; }

.btn {
  appearance: none; border: 1px solid rgba(15, 23, 42, .15); background: #fff;
  color: #0f172a; padding: 10px 12px; border-radius: 12px; cursor: pointer;
  transition: all .15s ease; box-shadow: 0 6px 16px rgba(2, 6, 23, .06);
}
.btn:hover { transform: translateY(-1px); box-shadow: 0 10px 22px rgba(2, 6, 23, .10); }
.btn.primary { background: linear-gradient(135deg, #2563eb, #7c3aed); border-color: transparent; color: #fff; }
.btn.danger { background: linear-gradient(135deg, #ef4444, #f97316); border-color: transparent; color: #fff; }
.btn.ghost { background: transparent; box-shadow: none; border: none; }
.btn.sm { padding: 8px 10px; border-radius: 10px; font-size: 13px; }

/* --- Form & Search --- */
.search-grid { display: grid; grid-template-columns: 1.2fr 1.5fr .8fr; gap: 14px; align-items: end; }
.field label { display: block; font-size: 12px; color: #334155; margin-bottom: 6px; font-weight: 600; }
.input-group { display: flex; gap: 10px; }

input, .custom-select {
  width: 100%; padding: 10px 12px; border-radius: 12px;
  border: 1px solid rgba(15, 23, 42, .15); outline: none;
  background: rgba(255, 255, 255, .9); transition: all .15s ease;
}
input:focus, .custom-select:focus { border-color: #2563eb; box-shadow: 0 0 0 4px rgba(37, 99, 235, .1); }

/* --- Custom Select Mũi Tên --- */
.custom-select {
  appearance: none;
  background-image: url("data:image/svg+xml;charset=UTF-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%23475569' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E");
  background-repeat: no-repeat; background-position: right 12px center; background-size: 16px;
  padding-right: 32px;
}

/* --- Table Styles --- */
.table-wrap { overflow: auto; border-radius: 14px; }
.table { width: 100%; border-collapse: collapse; min-width: 1000px; }
.table.big { min-width: 1200px; }
thead th {
  text-align: left; font-size: 12px; letter-spacing: .3px; text-transform: uppercase;
  color: #475569; padding: 12px; background: rgba(2, 6, 23, .03);
  border-bottom: 1px solid rgba(15, 23, 42, .10);
}
tbody td { padding: 12px; border-bottom: 1px solid rgba(15, 23, 42, .08); }
tr.row:hover { background: rgba(37, 99, 235, .04); }
.btn-row { display: flex; gap: 8px; }

/* --- Badge & Pagination --- */
.badge {
  display: inline-flex; align-items: center; justify-content: center;
  min-width: 28px; height: 22px; padding: 0 8px; border-radius: 999px;
  background: rgba(37, 99, 235, .12); color: #1d4ed8; font-weight: 700; margin-right: 6px;
}
/* Container tổng của phân trang */
.pager {
  display: flex;
  align-items: center;
  justify-content: space-between; /* Đẩy thông tin số lượng sang trái, nút bấm sang phải */
  padding-top: 20px;
  margin-top: 20px;
  border-top: 1px solid rgba(15, 23, 42, 0.08);
}

/* Khối chứa các nút bấm - Đây là phần quan trọng nhất */
.pager-right {
  display: flex;       /* Kích hoạt Flexbox để các thành phần nằm ngang */
  align-items: center; /* Căn giữa các nút theo chiều dọc */
  gap: 10px;           /* Tạo khoảng cách đều giữa các nút và số trang */
}

/* Định dạng ô hiển thị số trang cho đẹp hơn */
.page-indicator {
  padding: 6px 14px;
  background: #f1f5f9;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  color: #475569;
  white-space: nowrap; /* Đảm bảo chữ "Trang..." không bị xuống dòng */
  border: 1px solid rgba(15, 23, 42, 0.05);
}

/* --- Modal --- */
.modal-backdrop {
  position: fixed; inset: 0; background: rgba(2, 6, 23, .55);
  display: flex; align-items: center; justify-content: center; z-index: 50; padding: 20px;
}
.modal {
  width: min(800px, 100%); background: #fff; border-radius: 20px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25); overflow: hidden;
}
.modal.small { width: min(450px, 100%); }
.modal-header { padding: 20px 20px 0; display: flex; justify-content: space-between; }
.form { padding: 20px; }
.grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
.modal-footer { padding: 16px 20px; border-top: 1px solid #f1f5f9; display: flex; justify-content: flex-end; gap: 10px; }

/* --- Utils --- */
.text-primary { color: #2563eb; }
.text-success { color: #16a34a; }
.empty { padding: 40px; text-align: center; color: #94a3b8; }

/* --- Toast --- */
.toast {
  position: fixed; right: 20px; bottom: 20px; color: #fff;
  padding: 12px 20px; border-radius: 12px; z-index: 100;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}
.toast.ok { background: #16a34a; }
.toast.err { background: #dc2626; }

/* --- Spinner --- */
.spinner {
  display: inline-block; width: 14px; height: 14px;
  border: 2px solid rgba(255,255,255,.3); border-top-color: #fff;
  border-radius: 50%; animation: spin .8s linear infinite;
  margin-right: 8px;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* Animations */
.row-enter-active, .row-leave-active { transition: all .2s ease; }
.row-enter-from, .row-leave-to { opacity: 0; transform: translateX(-10px); }
</style>