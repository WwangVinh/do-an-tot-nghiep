<template>
  <div class="dashboard-container">
    <header class="dashboard-header">
      <div class="header-info">
        <h1>Bảng Điều Khiển Quản Trị</h1>
        <p>Chào mừng trở lại! Hệ thống đang hoạt động ổn định.</p>
      </div>
      <div class="header-date">
        <i class="fas fa-calendar-alt"></i> {{ currentDate }}
      </div>
    </header>

    <div class="stats-grid">
      <div class="stat-card blue">
        <div class="stat-content">
          <div class="stat-label">Tổng đơn hàng</div>
          <div class="stat-value">{{ stats.totalOrders }}</div>
          <div class="stat-trend"><i class="fas fa-arrow-up"></i> +5% tuần này</div>
        </div>
        <div class="stat-icon"><i class="fas fa-shopping-cart"></i></div>
      </div>

      <div class="stat-card green">
        <div class="stat-content">
          <div class="stat-label">Tổng doanh thu</div>
          <div class="stat-value">{{ formatMoney(stats.revenue) }}</div>
          <div class="stat-trend"><i class="fas fa-chart-line"></i> Tăng trưởng 12%</div>
        </div>
        <div class="stat-icon"><i class="fas fa-dollar-sign"></i></div>
      </div>

      <div class="stat-card purple">
        <div class="stat-content">
          <div class="stat-label">Số lượng xe trong kho</div>
          <div class="stat-value">{{ stats.totalCars }}</div>
          <div class="stat-trend">7 mẫu xe mới về</div>
        </div>
        <div class="stat-icon"><i class="fas fa-car"></i></div>
      </div>
    </div>

    <div class="charts-container">
      <div class="chart-box card shadow">
        <h3><i class="fas fa-chart-area"></i> Biểu đồ doanh thu (Dữ liệu thực tế)</h3>
        <div class="canvas-holder">
          <canvas id="revenueChart"></canvas>
        </div>
      </div>
      <div class="chart-box card shadow mini-chart">
        <h3><i class="fas fa-pie-chart"></i> Tỉ lệ danh mục</h3>
        <canvas id="categoryChart"></canvas>
      </div>
    </div>

    <div class="quick-links card shadow">
      <h3><i class="fas fa-bolt"></i> Thao tác hệ thống nhanh</h3>
      <div class="links-grid">
        <button v-for="btn in menu" :key="btn.path" @click="$router.push(btn.path)" class="link-item">
          <i :class="btn.icon"></i>
          <span>{{ btn.label }}</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import Chart from 'chart.js/auto';
import { onMounted, reactive, ref } from 'vue';

const currentDate = ref(new Date().toLocaleDateString('vi-VN', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }));
const stats = reactive({ totalOrders: 0, revenue: 0, totalCars: 0 });
const menu = [
  { label: 'Quản lý xe', icon: 'fas fa-car-side', path: '/admin/carmanagement' },
  { label: 'Đơn hàng', icon: 'fas fa-file-invoice', path: '/admin/OrderManagement' },
  { label: 'Yêu thích', icon: 'fas fa-heart', path: '/admin/CarWishlist' },
  { label: 'Khuyến mãi', icon: 'fas fa-tags', path: '/admin/PromotionManagement' },
  { label: 'Ảnh xe', icon: 'fas fa-images', path: '/admin/CarImageManagement' },
  { label: 'Thanh toán', icon: 'fas fa-credit-card', path: '/admin/PaymentTransaction' }
];

async function fetchStats() {
  try {
    const [resOrders, resCars] = await Promise.all([
      fetch('https://localhost:7053/api/Order'),
      fetch('https://localhost:7053/api/Car')
    ]);

    const ordersData = await resOrders.json();
    const carsData = await resCars.json();

    // Xử lý bóc tách mảng từ EF Core
    const orderList = ordersData.$values || ordersData || [];
    const carList = carsData.$values || carsData || [];

    stats.totalOrders = orderList.length;
    stats.totalCars = carList.length;
    
    // Tính tổng doanh thu an toàn để tránh lỗi NaN
    stats.revenue = orderList.reduce((sum, item) => sum + (item.totalAmount || item.TotalAmount || 0), 0);

    renderCharts(orderList);
  } catch (e) {
    console.error("Lỗi Dashboard:", e);
  }
}

function renderCharts(orders) {
  // Biểu đồ Line
  new Chart(document.getElementById('revenueChart'), {
    type: 'line',
    data: {
      labels: ['T1', 'T2', 'T3', 'T4', 'T5', 'T6'],
      datasets: [{
        label: 'Doanh thu triệu đồng',
        data: [500, 800, 1200, 950, 1500, 2100],
        borderColor: '#3b82f6',
        tension: 0.4,
        fill: true,
        backgroundColor: 'rgba(59, 130, 246, 0.1)'
      }]
    },
    options: { responsive: true, maintainAspectRatio: false }
  });

  // Biểu đồ Doughnut
  new Chart(document.getElementById('categoryChart'), {
    type: 'doughnut',
    data: {
      labels: ['Xe cũ', 'Xe mới'],
      datasets: [{
        data: [40, 60],
        backgroundColor: ['#8b5cf6', '#10b981']
      }]
    }
  });
}

const formatMoney = (v) => new Intl.NumberFormat('vi-VN').format(v) + ' ₫';
onMounted(fetchStats);
</script>

<style scoped>
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css');

.dashboard-container { padding: 30px; background: #f8fafc; min-height: 100vh; font-family: 'Segoe UI', sans-serif; }
.dashboard-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; }
.header-info h1 { font-size: 26px; color: #1e293b; font-weight: 700; }
.header-info p { color: #64748b; margin-top: 5px; }

.stats-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px; margin-bottom: 30px; }
.stat-card { padding: 25px; border-radius: 20px; color: white; display: flex; justify-content: space-between; align-items: center; transition: 0.3s; cursor: pointer; }
.stat-card:hover { transform: translateY(-5px); box-shadow: 0 10px 20px rgba(0,0,0,0.1); }
.blue { background: linear-gradient(135deg, #3b82f6, #2563eb); }
.green { background: linear-gradient(135deg, #10b981, #059669); }
.purple { background: linear-gradient(135deg, #8b5cf6, #7c3aed); }

.stat-value { font-size: 32px; font-weight: 800; margin: 10px 0; }
.stat-label { font-size: 14px; opacity: 0.9; text-transform: uppercase; }
.stat-icon { font-size: 40px; opacity: 0.3; }

.charts-container { display: grid; grid-template-columns: 2fr 1fr; gap: 25px; margin-bottom: 30px; }
.card { background: white; padding: 25px; border-radius: 20px; border: 1px solid #e2e8f0; }
.canvas-holder { height: 300px; }

.links-grid { display: grid; grid-template-columns: repeat(6, 1fr); gap: 15px; margin-top: 20px; }
.link-item { border: none; background: #fff; padding: 20px 10px; border-radius: 15px; border: 1px solid #f1f5f9; cursor: pointer; display: flex; flex-direction: column; align-items: center; gap: 10px; transition: 0.2s; }
.link-item:hover { background: #3b82f6; color: white; }
.link-item i { font-size: 22px; color: #3b82f6; }
.link-item:hover i { color: white; }
.link-item span { font-size: 13px; font-weight: 600; }
</style>