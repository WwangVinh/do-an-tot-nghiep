<template>
  <div class="payment-container">
    <div class="header-section">
      <h2><i class="fas fa-credit-card"></i> Quản lý Giao dịch Thanh toán</h2>
      <div class="actions">
        <button @click="openAddModal" class="btn btn-primary">+ Thêm Giao dịch</button>
        <button @click="fetchTransactions" class="btn btn-refresh">↻ Tải lại</button>
      </div>
    </div>

    <div class="card-table">
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Mã Đơn</th>
            <th>Số tiền</th>
            <th>Phương thức</th>
            <th>Ngày GD</th>
            <th>Trạng thái</th>
            <th class="text-center">Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in transactions" :key="item.TransactionId || item.transactionId">
            <td>#{{ item.TransactionId || item.transactionId }}</td>
            <td class="order-id">#{{ item.OrderId || item.orderId }}</td>
            <td class="amount-cell">{{ formatPrice(item.Amount || item.amount) }}</td>
            <td>
              <span class="method-tag">{{ item.PaymentMethod || item.paymentMethod }}</span>
            </td>
            <td class="date-cell">{{ formatDate(item.TransactionDate || item.transactionDate) }}</td>
            <td>
              <span :class="['status-badge', (item.Status || item.status || '').toLowerCase()]">
                {{ item.Status || item.status }}
              </span>
            </td>
            <td class="text-center">
              <button @click="openEditModal(item)" class="btn-icon edit" title="Sửa trạng thái">✎</button>
              <button @click="deleteTransaction(item.TransactionId || item.transactionId)" class="btn-icon delete" title="Xóa">✕</button>
            </td>
          </tr>
          <tr v-if="transactions.length === 0">
            <td colspan="7" class="empty-state">Không có dữ liệu giao dịch.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="modal.open" class="modal-overlay">
      <div class="modal-box">
        <div class="modal-header">
          <h3>{{ modal.mode === 'add' ? 'Thêm Giao dịch' : 'Cập nhật Trạng thái' }}</h3>
          <button @click="modal.open = false" class="close-btn">&times;</button>
        </div>
        
        <div class="modal-body">
          <div v-if="modal.mode === 'add'" class="form-row">
            <div class="form-group">
              <label>Mã Đơn hàng (OrderId)</label>
              <input v-model="form.orderId" type="number" placeholder="Nhập ID đơn hàng...">
            </div>
            <div class="form-group">
              <label>Số tiền (VND)</label>
              <input v-model="form.amount" type="number" placeholder="0">
            </div>
          </div>

          <div class="form-group">
            <label>Phương thức thanh toán</label>
            <select v-model="form.paymentMethod" :disabled="modal.mode === 'edit'">
              <option value="Tiền mặt">💵 Tiền mặt</option>
              <option value="Thẻ">💳 Thẻ ngân hàng</option>
              <option value="Chuyển khoản">🏦 Chuyển khoản</option>
            </select>
          </div>

          <div class="form-group">
            <label>Trạng thái giao dịch</label>
            <select v-model="form.status">
              <option value="Pending">🕒 Chờ xử lý</option>
              <option value="Completed">✅ Hoàn tất</option>
              <option value="Failed">❌ Thất bại</option>
            </select>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="modal.open = false" class="btn-cancel">Hủy bỏ</button>
          <button @click="saveData" class="btn-save" :disabled="loading">
            {{ loading ? 'Đang lưu...' : (modal.mode === 'add' ? 'Tạo giao dịch' : 'Cập nhật') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { onMounted, reactive, ref } from 'vue';

const API_URL = "https://localhost:7053/api/PaymentTransaction";
const transactions = ref([]);
const loading = ref(false);

const modal = reactive({
  open: false,
  mode: 'add', // 'add' hoặc 'edit'
  currentId: null
});

const form = reactive({
  orderId: '',
  amount: 0,
  paymentMethod: 'Tiền mặt',
  status: 'Completed'
});

// 1. Tải dữ liệu
const fetchTransactions = async () => {
  try {
    const res = await fetch(API_URL);
    transactions.value = await res.json();
  } catch (e) {
    console.error("Lỗi fetch:", e);
  }
};

// 2. Mở Modal
const openAddModal = () => {
  modal.mode = 'add';
  form.orderId = '';
  form.amount = 0;
  form.status = 'Completed';
  modal.open = true;
};

const openEditModal = (item) => {
  modal.mode = 'edit';
  modal.currentId = item.TransactionId || item.transactionId;
  form.orderId = item.OrderId || item.orderId;
  form.amount = item.Amount || item.amount;
  form.paymentMethod = item.PaymentMethod || item.paymentMethod;
  form.status = item.Status || item.status;
  modal.open = true;
};

// 3. Lưu dữ liệu (Hỗ trợ cả POST và PUT)
const saveData = async () => {
  loading.value = true;
  try {
    const fd = new FormData();
    // Chế độ Edit chỉ cần gửi Status theo Controller của bạn
    if (modal.mode === 'edit') {
      fd.append("status", form.status);
    } else {
      fd.append("orderId", form.orderId);
      fd.append("amount", form.amount);
      fd.append("paymentMethod", form.paymentMethod);
      fd.append("status", form.status);
    }

    const url = modal.mode === 'edit' ? `${API_URL}/${modal.currentId}` : API_URL;
    const method = modal.mode === 'edit' ? "PUT" : "POST";

    const res = await fetch(url, { method: method, body: fd });

    if (!res.ok) throw new Error(await res.text());

    modal.open = false;
    fetchTransactions();
    alert(modal.mode === 'add' ? "Đã thêm!" : "Đã cập nhật!");
  } catch (e) {
    alert("Lỗi: " + e.message);
  } finally {
    loading.value = false;
  }
};

// 4. Xóa
const deleteTransaction = async (id) => {
  if (!confirm("Bạn có chắc chắn muốn xóa giao dịch này?")) return;
  try {
    const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    if (res.ok) fetchTransactions();
  } catch (e) { alert("Lỗi xóa"); }
};

// Helpers
const formatPrice = (v) => new Intl.NumberFormat('vi-VN').format(v) + ' ₫';
const formatDate = (s) => s ? new Date(s).toLocaleString('vi-VN') : '---';

onMounted(fetchTransactions);
</script>

<style scoped>
.payment-container { padding: 20px; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f8f9fa; min-height: 100vh; }
.header-section { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; }
.card-table { background: white; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); overflow: hidden; }

table { width: 100%; border-collapse: collapse; }
th { background: #f1f3f5; padding: 15px; text-align: left; font-size: 13px; text-transform: uppercase; color: #495057; }
td { padding: 15px; border-bottom: 1px solid #f1f3f5; font-size: 14px; }

.amount-cell { font-weight: 700; color: #2e7d32; }
.status-badge { padding: 5px 12px; border-radius: 20px; font-size: 12px; font-weight: 600; }
.completed { background: #e8f5e9; color: #2e7d32; }
.pending { background: #fff3e0; color: #ef6c00; }
.failed { background: #ffebee; color: #c62828; }

.btn-icon { border: none; background: none; cursor: pointer; padding: 5px 10px; border-radius: 4px; transition: 0.2s; }
.btn-icon.edit { color: #1976d2; }
.btn-icon.edit:hover { background: #e3f2fd; }
.btn-icon.delete { color: #d32f2f; }
.btn-icon.delete:hover { background: #ffebee; }

/* Modal Styles */
.modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 1000; }
.modal-box { background: white; width: 450px; border-radius: 15px; padding: 0; overflow: hidden; animation: slideDown 0.3s; }
.modal-header { padding: 20px; border-bottom: 1px solid #eee; display: flex; justify-content: space-between; align-items: center; }
.modal-body { padding: 20px; }
.form-group { margin-bottom: 15px; }
.form-group label { display: block; margin-bottom: 6px; font-weight: 600; color: #333; }
.form-group input, .form-group select { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 8px; }
.modal-footer { padding: 15px 20px; background: #f8f9fa; text-align: right; }
.btn-save { background: #1976d2; color: white; border: none; padding: 10px 25px; border-radius: 8px; cursor: pointer; }
.btn-cancel { background: none; border: none; color: #666; margin-right: 15px; cursor: pointer; }

@keyframes slideDown { from { transform: translateY(-20px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
</style>