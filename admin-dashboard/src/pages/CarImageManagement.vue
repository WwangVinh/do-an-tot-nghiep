<template>
  <div class="page">
    <header class="topbar">
      <div class="title-wrap">
        <h1>🖼️ Thư viện Ảnh Xe</h1>
        <span class="muted small">Quản lý hình ảnh đại diện và bộ sưu tập chi tiết</span>
      </div>
      <button class="btn primary" @click="openCreate">+ Thêm ảnh mới</button>
    </header>

    <section class="card search-card">
      <div class="search-grid">
        <div class="field">
          <label>Lọc theo dòng xe</label>
          <select v-model="carFilter" class="custom-select">
            <option :value="null">Tất cả các xe</option>
            <option v-for="c in cars" :key="c.carId" :value="c.carId">{{ c.name }}</option>
          </select>
        </div>
        <div class="field">
          <label>Loại ảnh</label>
          <select v-model="typeFilter" class="custom-select">
            <option value="all">Tất cả loại</option>
            <option value="main">Chỉ ảnh chính</option>
            <option value="sub">Chỉ ảnh phụ</option>
          </select>
        </div>
        <div class="field right">
          <button class="btn" @click="resetFilter">↻ Làm mới</button>
        </div>
      </div>
    </section>

    <section class="card no-padding">
      <div class="table-wrap">
        <table class="table">
          <thead>
            <tr>
              <th>Hình ảnh</th>
              <th>Tên xe</th>
              <th>Trạng thái</th>
              <th>Ngày cập nhật</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="img in paginatedImages" :key="img.carImageId || img.CarImageId" class="row">
              <td>
                <div class="img-container">
                  <img :src="img.imageUrl || img.ImageUrl" class="preview-img" @error="handleImgError" />
                </div>
              </td>
              <td>
                <div class="strong">
                  {{ getCarName(img.carId || img.CarId) }}
                </div>
                <div class="muted x-small">Mã số xe: #{{ img.carId || img.CarId }}</div>
              </td>
              <td>
                <span :class="['status-pill', (img.isMainImage || img.IsMainImage) ? 'active' : 'sub']">
                  {{ (img.isMainImage || img.IsMainImage) ? '🌟 Ảnh chính' : '📷 Ảnh phụ' }}
                </span>
              </td>
              <td class="muted small">{{ formatDate(img.createdAt || img.CreatedAt) }}</td>
              <td>
                <div class="btn-row">
                  <button class="btn sm ghost" @click="openEdit(img)">Sửa</button>
                  <button class="btn sm danger-ghost" @click="confirmDelete(img.carImageId || img.CarImageId)">Xóa</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div v-if="filteredImages.length === 0" class="empty-state">
        📭 Không tìm thấy hình ảnh nào phù hợp.
      </div>

      <div class="pagination-container" v-if="totalPages > 1">
        <div class="pagination-info">
          Hiển thị <b>{{ paginatedImages.length }}</b> / {{ filteredImages.length }} ảnh
        </div>
        <div class="pagination-btns">
          <button class="btn page-num" :disabled="currentPage === 1" @click="currentPage--">◀</button>
          <button 
            v-for="p in totalPages" :key="p" 
            :class="['btn page-num', currentPage === p ? 'active' : '']"
            @click="currentPage = p"
          >
            {{ p }}
          </button>
          <button class="btn page-num" :disabled="currentPage === totalPages" @click="currentPage++">▶</button>
        </div>
      </div>
    </section>

    <div v-if="modal.open" class="modal-backdrop" @mousedown.self="modal.open = false">
      <div class="modal">
        <div class="modal-header">
          <h3>{{ modal.mode === 'create' ? 'Thêm ảnh mới' : 'Cập nhật ảnh' }}</h3>
        </div>
        <form @submit.prevent="handleSubmit" class="modal-body">
          <div class="field">
            <label>Chọn xe mục tiêu</label>
            <select v-model.number="form.carId" class="custom-select" required>
              <option :value="null" disabled>-- Chọn một chiếc xe --</option>
              <option v-for="c in cars" :key="c.carId" :value="c.carId">
                {{ c.name }}
              </option>
            </select>
          </div>
          <div class="field">
            <label>Đường dẫn ảnh (URL)</label>
            <input v-model="form.imageUrl" type="url" placeholder="Dán link ảnh tại đây..." required />
          </div>
          <div class="field toggle-field">
            <label class="switch-wrap">
              <input type="checkbox" v-model="form.isMain" />
              <span class="slider"></span>
            </label>
            <span>Đặt làm ảnh đại diện chính</span>
          </div>

          <div v-if="form.imageUrl" class="url-preview">
            <label>Xem trước ảnh:</label>
            <img :src="form.imageUrl" @error="handleImgError" />
          </div>

          <div class="modal-footer">
            <button type="button" class="btn" @click="modal.open = false">Hủy</button>
            <button type="submit" class="btn primary" :disabled="modal.loading">
              {{ modal.loading ? 'Đang lưu...' : 'Lưu dữ liệu' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue';

const API_IMG = "https://localhost:7053/api/CarImage";
const API_CAR = "https://localhost:7053/api/Car";

const carImages = ref([]);
const cars = ref([]);
const carFilter = ref(null);
const typeFilter = ref("all");
const currentPage = ref(1);
const itemsPerPage = 5;
const modal = reactive({ open: false, mode: 'create', editId: null, loading: false });
const form = reactive({ carId: null, imageUrl: '', isMain: false });

// Tải dữ liệu
const loadData = async () => {
  try {
    const [imgRes, carRes] = await Promise.all([fetch(API_IMG), fetch(API_CAR)]);
    const imgData = await imgRes.json();
    const carData = await carRes.json();

    // Chuẩn hóa dữ liệu ảnh
    carImages.value = imgData.$values || imgData;
    
    // Chuẩn hóa dữ liệu xe (Ép kiểu Number cho carId)
    const rawCars = carData.$values || carData;
    cars.value = rawCars.map(c => ({
      carId: Number(c.carId || c.CarId), 
      name: c.name || c.Name
    }));
  } catch (e) {
    console.error("Lỗi tải dữ liệu:", e);
  }
};

// Hàm lấy tên xe dựa trên ID
const getCarName = (id) => {
  const car = cars.value.find(c => c.carId === Number(id));
  return car ? car.name : 'Đang tải...';
};

// Logic Lọc & Phân trang
const filteredImages = computed(() => {
  return carImages.value.filter(img => {
    const id = img.carId || img.CarId;
    const isMain = img.isMainImage || img.IsMainImage;
    const matchCar = carFilter.value ? id === carFilter.value : true;
    const matchType = typeFilter.value === 'all' ? true : 
                     (typeFilter.value === 'main' ? isMain : !isMain);
    return matchCar && matchType;
  });
});

const totalPages = computed(() => Math.ceil(filteredImages.value.length / itemsPerPage));
const paginatedImages = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage;
  return filteredImages.value.slice(start, start + itemsPerPage);
});

watch([carFilter, typeFilter], () => currentPage.value = 1);

// Gửi dữ liệu (Sửa lỗi 400 Bad Request)
const handleSubmit = async () => {
  if (form.carId === null) return alert("Vui lòng chọn xe mục tiêu!");

  modal.loading = true;
  const fd = new FormData();
  fd.append("carId", Number(form.carId)); 
  fd.append("imageUrl", form.imageUrl);
  // SỬA LỖI 400: Gửi số 1/0 thay vì chuỗi "true/false"
  fd.append("isMainImage", form.isMain ? 1 : 0); 

  const isCreate = modal.mode === 'create';
  const url = isCreate ? API_IMG : `${API_IMG}/${modal.editId}`;
  
  try {
    const res = await fetch(url, { method: isCreate ? 'POST' : 'PUT', body: fd });

    if (res.ok) {
      modal.open = false;
      await loadData();
    } else {
      const err = await res.json();
      console.error("Lỗi:", err);
      alert("Lỗi: Server từ chối dữ liệu (400 Bad Request).");
    }
  } catch (e) {
    alert("Lỗi kết nối!");
  } finally {
    modal.loading = false;
  }
};

const openCreate = () => {
  modal.mode = 'create';
  form.carId = cars.value.length > 0 ? cars.value[0].carId : null;
  form.imageUrl = '';
  form.isMain = false;
  modal.open = true;
};

const openEdit = (img) => {
  modal.mode = 'edit';
  modal.editId = img.carImageId || img.CarImageId;
  form.carId = img.carId || img.CarId;
  form.imageUrl = img.imageUrl || img.ImageUrl;
  form.isMain = !!(img.isMainImage || img.IsMainImage);
  modal.open = true;
};

const confirmDelete = async (id) => {
  if (confirm("Xóa ảnh này vĩnh viễn?")) {
    await fetch(`${API_IMG}/${id}`, { method: 'DELETE' });
    await loadData();
  }
};

const resetFilter = () => { carFilter.value = null; typeFilter.value = 'all'; };
const formatDate = (d) => d ? new Date(d).toLocaleDateString('vi-VN') : '---';

const handleImgError = (e) => {
  // Thay thế bằng link ảnh xe sang trọng nếu link gốc bị Google chặn
  e.target.src = 'https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=400&q=80';
};

onMounted(loadData);
</script>

<style scoped>
/* Code CSS giữ nguyên như của bạn nhưng đã tối ưu class pagination-container */
.page { padding: 24px; background-color: #f8fafc; min-height: 100vh; font-family: sans-serif; }
.topbar { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
.card { background: white; border-radius: 12px; box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1); padding: 20px; margin-bottom: 20px; border: 1px solid #e2e8f0; }
.search-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px; align-items: flex-end; }
.table-wrap { overflow-x: auto; }
.table { width: 100%; border-collapse: collapse; }
.table th { text-align: left; padding: 12px; background: #f1f5f9; color: #475569; font-size: 13px; text-transform: uppercase; }
.table td { padding: 12px; border-bottom: 1px solid #f1f5f9; }
.img-container { width: 120px; height: 75px; overflow: hidden; border-radius: 8px; background: #e2e8f0; border: 1px solid #eee; }
.preview-img { width: 100%; height: 100%; object-fit: cover; }
.status-pill { padding: 4px 12px; border-radius: 99px; font-size: 12px; font-weight: 600; display: inline-block; }
.status-pill.active { background: #dcfce7; color: #15803d; }
.status-pill.sub { background: #f1f5f9; color: #64748b; }
.modal-backdrop { position: fixed; inset: 0; background: rgba(0, 0, 0, 0.5); display: flex; align-items: center; justify-content: center; z-index: 1000; backdrop-filter: blur(4px); }
.modal { background: white; width: 100%; max-width: 500px; border-radius: 12px; overflow: hidden; }
.modal-body { padding: 20px; }
.modal-footer { padding: 16px 20px; background: #f8fafc; display: flex; justify-content: flex-end; gap: 12px; }
.field { display: flex; flex-direction: column; gap: 6px; margin-bottom: 16px; }
.custom-select, input { padding: 8px 12px; border: 1px solid #cbd5e1; border-radius: 6px; }
.btn { padding: 8px 16px; border-radius: 6px; font-weight: 600; cursor: pointer; border: 1px solid #cbd5e1; background: white; }
.btn.primary { background: #3b82f6; color: white; border: none; }
.btn.sm { padding: 4px 10px; font-size: 12px; margin-right: 5px; }
.btn.page-num.active { background: #3b82f6; color: white; }
.pagination-container { display: flex; justify-content: space-between; align-items: center; padding: 15px; background: #fff; border-top: 1px solid #eee; }
.url-preview img { width: 100%; max-height: 120px; object-fit: contain; margin-top: 10px; border-radius: 4px; }
</style>