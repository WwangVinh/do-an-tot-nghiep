// Sử dụng 'import type' để nhập kiểu dữ liệu
import type { RouteRecordRaw } from 'vue-router'; // Sử dụng 'import type' cho kiểu dữ liệu
import { createRouter, createWebHistory } from 'vue-router';
import CarImageManagement from '../pages/CarImageManagement.vue';
import CarManagement from '../pages/CarManagement.vue';
import CarWishlist from '../pages/CarWishlist.vue';
import Dashboard from '../pages/Dashboard.vue';
import OrderItems from '../pages/OrderItems.vue';
import OrderManagement from '../pages/OrderManagement.vue';
import PaymentTransaction from '../pages/PaymentTransaction.vue';
import PromotionManagement from '../pages/PromotionManagement.vue';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'dashboard',
    component: Dashboard
  },
  {
    path: '/admin/carmanagement',
    name: 'carmanagement',
    component: CarManagement
  },
   {
    path: '/admin/OrderManagement',
    name: 'OrderManagement',
    component: OrderManagement
  },
   {
    path: '/admin/OrderItems',
    name: 'OrderItems',
    component: OrderItems
  },
     {
    path: '/admin/PromotionManagement',
    name: 'PromotionManagement',
    component: PromotionManagement
  },
    {
    path: '/admin/CarImageManagement',
    name: 'CarImageManagement',
    component: CarImageManagement
  },
    {
    path: '/admin/PaymentTransaction',
    name: 'PaymentTransaction',
    component: PaymentTransaction
  },
    {
    path: '/admin/CarWishlist',
    name: 'CarWishlist',
    component: CarWishlist
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
