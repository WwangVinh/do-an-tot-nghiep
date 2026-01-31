<template>
  <div class="page">
    <header class="topbar">
      <div class="title-wrap">
        <h1>🎁 Quản lý Khuyến mãi</h1>
        <span class="muted small">Thiết lập mã giảm giá và chương trình ưu đãi</span>
      </div>
      <div class="actions">
        <button class="btn primary" @click="openCreate">+ Tạo mã mới</button>
      </div>
    </header>

    <section class="card search-card">
      <div class="search-grid">
        <div class="field">
          <label>Tìm theo mã (Code)</label>
          <input v-model.trim="keyword" type="text" placeholder="Ví dụ: SUMMER2026..." />
        </div>
        <div class="field">
          <label>Trạng thái</label>
          <select v-model="statusFilter" class="custom-select">
            <option value="">Tất cả</option>
            <option value="Active">Đang hoạt động</option>
            <option value="Inactive">Tạm dừng</option>
          </select>
        </div>
        <div class="field right">
          <label>&nbsp;</label>
          <button class="btn" @click="reload">↻ Làm mới</button>
        </div>
      </div>
    </section>

    <section class="card">
      <div class="table-wrap">
        <table class="table big">
          <thead>
            <tr>
              <th>Mã Code</th>
              <th>% Giảm</th>
              <th>Mô tả</th>
              <th>Thời gian áp dụng</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in filteredPromotions" :key="p.promotionId || p.PromotionId" class="row">
              <td><span class="code-badge">{{ p.code || p.Code }}</span></td>
              <td class="strong text-success">{{ p.discountPercentage || p.DiscountPercentage }}%</td>
              <td class="muted small">{{ p.description || p.Description || '---' }}</td>
              <td class="small">
                {{ formatDate(p.startDate || p.StartDate) }} → <br/>
                {{ formatDate(p.endDate || p.EndDate) }}
              </td>
              <td>
                <span :class="['status-pill', (p.status || p.Status) === 'Active' ? 'active' : 'inactive']">
                  {{ (p.status || p.Status) === 'Active' ? 'Đang chạy' : 'Tạm dừng' }}
                </span>
              </td>
              <td>
                <div class="btn-row">
                  <button class="btn sm" @click="openEdit(p)">Sửa</button>
                  <button class="btn sm danger" @click="confirmDelete(p)">Xóa</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-if="filteredPromotions.length === 0" class="empty">📭 Không có mã giảm giá nào.</div>
    </section>

    <transition name="modal">
      <div v-if="modal.open" class="modal-backdrop" @mousedown.self="modal.open = false">
        <div class="modal">
          <div class="modal-header">
            <h2>{{ modal.mode === 'create' ? '➕ Tạo mã khuyến mãi' : '📝 Cập nhật mã' }}</h2>
            <button class="btn ghost" @click="modal.open = false">✕</button>
          </div>
          <form @submit.prevent="handleSubmit" class="form">
            <div class="grid">
              <div class="field">
                <label>Mã khuyến mãi (Code)</label>
                <input v-model="form.code" type="text" placeholder="Ví dụ: GIAM30" required :disabled="modal.mode === 'edit'" />
              </div>
              <div class="field">
                <label>% Giảm giá</label>
                <input v-model.number="form.discountPercentage" type="number" min="1" max="100" required />
              </div>
              <div class="field">
                <label>Ngày bắt đầu</label>
                <input v-model="form.startDate" type="date" required />
              </div>
              <div class="field">
                <label>Ngày kết thúc</label>
                <input v-model="form.endDate" type="date" required />
              </div>
              <div class="field full">
                <label>Mô tả chương trình</label>
                <textarea v-model="form.description" rows="2" placeholder="Nhập chi tiết ưu đãi..."></textarea>
              </div>
              <div class="field">
                <label>Kích hoạt ngay?</label>
                <div class="toggle-wrap">
                  <input type="checkbox" v-model="form.isActive" id="status-check" />
                  <label for="status-check">{{ form.isActive ? 'Đang chạy (Active)' : 'Tạm tắt (Inactive)' }}</label>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn" @click="modal.open = false">Hủy</button>
              <button type="submit" class="btn primary" :disabled="modal.saving">
                {{ modal.saving ? 'Đang lưu...' : 'Lưu dữ liệu' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue';

const API_BASE = "https://localhost:7053/api/Promotion";

// States
const promotions = ref([]);
const keyword = ref("");
const statusFilter = ref("");
const modal = reactive({ open: false, mode: 'create', saving: false, editId: null });
const form = reactive({ code: '', discountPercentage: 0, startDate: '', endDate: '', description: '', isActive: true });

// Helpers
const formatDate = (d) => d ? new Date(d).toLocaleDateString('vi-VN') : '---';
const normalize = (data) => data?.$values || (Array.isArray(data) ? data : []);

// Fetch dữ liệu
const fetchList = async () => {
  try {
    const res = await fetch(API_BASE);
    promotions.value = normalize(await res.json());
  } catch (e) { alert("Không thể kết nối API"); }
};

const handleSubmit = async () => {
  modal.saving = true;
  try {
    const isEdit = modal.mode === 'edit';
    const url = isEdit ? `${API_BASE}/${modal.editId}` : API_BASE;
    
    const fd = new FormData();
    fd.append("code", form.code);
    fd.append("discountPercentage", form.discountPercentage);
    fd.append("startDate", form.startDate);
    fd.append("endDate", form.endDate);
    fd.append("description", form.description || "");
    fd.append("status", form.isActive ? 1 : 0); // Gửi 1/0 sang Backend xử lý thành Active/Inactive

    const res = await fetch(url, { method: isEdit ? 'PUT' : 'POST', body: fd });
    if (!res.ok) {
        const errText = await res.text();
        throw new Error(errText || "Lỗi xử lý dữ liệu");
    }

    modal.open = false;
    fetchList();
    alert(isEdit ? "Đã cập nhật" : "Đã tạo thành công");
  } catch (e) {
    alert(e.message);
  } finally {
    modal.saving = false;
  }
};

const openCreate = () => {
  modal.mode = 'create'; modal.open = true;
  Object.assign(form, { code: '', discountPercentage: 0, startDate: '', endDate: '', description: '', isActive: true });
};

const openEdit = (p) => {
  modal.mode = 'edit'; modal.open = true;
  modal.editId = p.promotionId || p.PromotionId;
  Object.assign(form, {
    code: p.code || p.Code,
    discountPercentage: p.discountPercentage || p.DiscountPercentage,
    startDate: (p.startDate || p.StartDate).split('T')[0],
    endDate: (p.endDate || p.EndDate).split('T')[0],
    description: p.description || p.Description,
    isActive: (p.status || p.Status) === 'Active'
  });
};

const reload = () => { keyword.value = ""; statusFilter.value = ""; fetchList(); };

const filteredPromotions = computed(() => {
  return promotions.value.filter(p => {
    const matchK = (p.code || p.Code).toLowerCase().includes(keyword.value.toLowerCase());
    const matchS = statusFilter.value ? (p.status || p.Status) === statusFilter.value : true;
    return matchK && matchS;
  });
});

onMounted(fetchList);
</script>

<style scoped>
.page {
  padding: 24px;
  background-color: #f8fafc;
  min-height: 100vh;
  font-family: 'Inter', sans-serif;
}

.topbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.title-wrap h1 {
  font-size: 24px;
  color: #0f172a;
  margin: 0;
}

.muted { color: #64748b; }
.small { font-size: 13px; }

/* --- Thẻ Card & Tìm Kiếm --- */
.card {
  background: white;
  border-radius: 12px;
  border: 1px solid rgba(15, 23, 42, 0.08);
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);
  margin-bottom: 24px;
  padding: 20px;
}

.search-grid {
  display: grid;
  grid-template-columns: 1fr 1fr auto;
  gap: 20px;
  align-items: flex-end;
}

/* --- Bảng Dữ Liệu --- */
.table-wrap {
  overflow-x: auto;
}

.table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

.table th {
  padding: 12px 16px;
  background: #f1f5f9;
  color: #475569;
  font-weight: 600;
  font-size: 13px;
  text-transform: uppercase;
}

.table td {
  padding: 16px;
  border-bottom: 1px solid #f1f5f9;
  vertical-align: middle;
}

.row:hover { background-color: #f8fafc; }

/* --- Badges & Status --- */
.code-badge {
  background: #e2e8f0;
  padding: 6px 10px;
  border-radius: 6px;
  font-family: 'JetBrains Mono', monospace;
  font-weight: 700;
  color: #1e293b;
  border: 1px solid #cbd5e1;
}

.status-pill {
  display: inline-flex;
  align-items: center;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}

.status-pill.active {
  background: #dcfce7;
  color: #15803d;
}

.status-pill.inactive {
  background: #fee2e2;
  color: #b91c1c;
}

/* --- Form & Input --- */
.field {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.field label {
  font-weight: 600;
  font-size: 13px;
  color: #334155;
}

input, select, textarea {
  padding: 10px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  font-size: 14px;
  transition: all 0.2s;
}

input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.toggle-wrap {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  background: #f1f5f9;
  border-radius: 8px;
  cursor: pointer;
}

/* --- Buttons --- */
.btn {
  padding: 10px 18px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  border: 1px solid #cbd5e1;
  background: white;
  transition: 0.2s;
}

.btn.primary {
  background: #3b82f6;
  color: white;
  border: none;
}

.btn.primary:hover { background: #2563eb; }

.btn.danger {
  color: #dc2626;
  border-color: #fecaca;
}

.btn.danger:hover {
  background: #dc2626;
  color: white;
}

.btn.sm {
  padding: 6px 12px;
  font-size: 12px;
}

.btn-row {
  display: flex;
  gap: 8px;
}

/* --- Modal --- */
.modal-backdrop {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(15, 23, 42, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  backdrop-filter: blur(4px);
}

.modal {
  background: white;
  width: 100%;
  max-width: 600px;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}

.modal-header {
  padding: 20px;
  border-bottom: 1px solid #f1f5f9;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-footer {
  padding: 20px;
  background: #f8fafc;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  padding: 20px;
}

.field.full { grid-column: span 2; }
.code-badge {
  background: #e2e8f0;
  padding: 4px 8px;
  border-radius: 6px;
  font-family: monospace;
  font-weight: bold;
  color: #1e293b;
}

.status-pill {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}

.status-pill.active { background: #dcfce7; color: #15803d; }
.status-pill.inactive { background: #fee2e2; color: #b91c1c; }

.toggle-wrap {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px;
  background: #f8fafc;
  border-radius: 8px;
}
/* Các style khác kế thừa từ giao diện Admin của bạn */
</style>