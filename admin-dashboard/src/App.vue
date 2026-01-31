<script>
export default {
  name: 'App'
}
</script>

<template>
  <div class="admin-layout">
    <nav class="sidebar">
      <div class="logo">
        <i class="fas fa-steering-wheel"></i> <span>OTO ADMIN</span>
      </div>
      
      <div class="nav-menu">
        <router-link to="/" class="nav-item">
          <i class="fas fa-chart-pie"></i> <span>Dashboard</span>
        </router-link>

        <router-link to="/admin/carmanagement" class="nav-item">
          <i class="fas fa-car-side"></i> <span>Quản lý Xe</span>
        </router-link>

        <router-link to="/admin/OrderManagement" class="nav-item">
          <i class="fas fa-file-invoice-dollar"></i> <span>Quản lý Đơn hàng</span>
        </router-link>

        <router-link to="/admin/OrderItems" class="nav-item">
          <i class="fas fa-list-ul"></i> <span>Chi tiết đơn hàng</span>
        </router-link>

        <router-link to="/admin/PromotionManagement" class="nav-item">
          <i class="fas fa-tags"></i> <span>Mã giảm giá</span>
        </router-link>

        <router-link to="/admin/CarImageManagement" class="nav-item">
          <i class="fas fa-images"></i> <span>Ảnh xe</span>
        </router-link>

        <router-link to="/admin/PaymentTransaction" class="nav-item">
          <i class="fas fa-credit-card"></i> <span>Thanh Toán</span>
        </router-link>

        <router-link to="/admin/CarWishlist" class="nav-item">
          <i class="fas fa-heart"></i> <span>Danh sách yêu thích</span>
        </router-link>
      </div>
    </nav>

    <main class="main-content">
      <router-view></router-view>
    </main>
  </div>
</template>

<style scoped>
/* 1. Lệnh @import PHẢI để trên cùng để tránh lỗi Vite/PostCSS */
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css');

/* 2. Layout chung */
.admin-layout { 
  display: flex; 
  min-height: 100vh; 
  background: #f8fafc; 
}

/* 3. Sidebar cố định */
.sidebar {
  width: 260px;
  background: #1e293b;
  color: white;
  height: 100vh;
  padding: 20px;
  position: fixed; /* Cố định sidebar */
  left: 0;
  top: 0;
  z-index: 1000;
}

/* 4. Nội dung chính bên phải */
.main-content {
  flex: 1;
  margin-left: 260px; /* Đẩy nội dung sang phải để không bị Sidebar che */
  padding: 30px;
  min-height: 100vh;
}

.logo {
  font-size: 22px;
  font-weight: 800;
  color: #3b82f6;
  margin-bottom: 30px;
  display: flex;
  align-items: center;
  gap: 10px;
  padding-left: 10px;
  transition: filter 300ms;
}

.logo:hover {
  filter: drop-shadow(0 0 1em #3b82f6aa);
}

.nav-menu {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 15px;
  color: #94a3b8;
  text-decoration: none;
  border-radius: 10px;
  transition: all 0.3s;
}

.nav-item i {
  width: 20px;
  font-size: 18px;
  text-align: center;
}

.nav-item:hover {
  background: rgba(59, 130, 246, 0.1);
  color: #3b82f6;
}

/* Trạng thái đang chọn trang (Active) */
.router-link-active {
  background: #3b82f6 !important;
  color: white !important;
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
}
</style>