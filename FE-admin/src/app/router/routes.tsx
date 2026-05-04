import type { RouteObject } from 'react-router-dom'

import { AdminLayout } from '../../layouts/AdminLayout'
import { DashboardPage } from '../../pages/dashboard/DashboardPage'
import { LoginPage } from '../../pages/login/LoginPage'
import { CarsNewPage } from '../../pages/cars/CarsNewPage'
import { NotFoundPage } from '../../pages/NotFoundPage'
import { BannersPage } from '../../pages/banners/BannersPage'
import { CarsListPage } from '../../pages/cars/CarsListPage'
import { CarsEditPage } from '../../pages/cars/CarsEditPage'
import { ShowroomsPage } from '../../pages/showrooms/ShowroomsPage'
import { OrdersPage } from '../../pages/orders/OrdersPage'
import { BookingsPage } from '../../pages/bookings/BookingsPage'
import { AdminConsignmentsPage } from '../../pages/consignments/AdminConsignmentsPage' // <-- Thêm import này
import { UsersPage } from '../../pages/users/UsersPage'
import { ArticlesPage } from '../../pages/articles/ArticlesPage'
import { PromotionsPage } from '../../pages/promotions/PromotionsPage'
import { NotificationsPage } from '../../pages/notifications/NotificationsPage'
import { InventoriesPage } from '../../pages/inventories/InventoriesPage'
import { ReviewsPage } from '../../pages/reviews/ReviewsPage'
import { ForbiddenPage } from '../../pages/ForbiddenPage'
import { RequireRole } from './RequireRole'

export const routes: RouteObject[] = [
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/403',
    element: <ForbiddenPage />,
  },
  {
    element: (
      // Cho phép tất cả các role đã định nghĩa vào Layout chung
      <RequireRole allowed={['Admin', 'Manager', 'ShowroomSales', 'Sales', 'Technician', 'Marketing', 'Content']}>
        <AdminLayout />
      </RequireRole>
    ),
    children: [
      { path: '/', element: <DashboardPage /> },
      {
        path: '/banners',
        element: (
          <RequireRole allowed={['Admin','Manager', 'Marketing']}>
            <BannersPage />
          </RequireRole>
        ),
      },
      {
        path: '/cars',
        element: (
          <RequireRole allowed={['Admin', 'Manager', 'ShowroomSales', 'Sales', 'Technician']}>
            <CarsListPage />
          </RequireRole>
        ),
      },
      {
        path: '/cars/new',
        element: (
          <RequireRole allowed={['Admin', 'Manager', 'ShowroomSales', 'Sales']}>
            <CarsNewPage />
          </RequireRole>
        ),
      },
      {
        path: '/cars/:id/edit',
        element: (
          <RequireRole allowed={['Admin', 'Manager', 'ShowroomSales', 'Sales']}>
            <CarsEditPage />
          </RequireRole>
        ),
      },
      {
        path: '/showrooms',
        element: (
          <RequireRole allowed={['Admin', 'Manager']}>
            <ShowroomsPage />
          </RequireRole>
        ),
      },
      {
        path: '/orders',
        element: (
          <RequireRole allowed={['Admin', 'Manager', 'ShowroomSales', 'Sales']}>
            <OrdersPage />
          </RequireRole>
        ),
      },
      {
        path: '/bookings',
        element: (
          <RequireRole allowed={['Admin', 'Manager', 'ShowroomSales', 'Sales', 'Technician']}>
            <BookingsPage />
          </RequireRole>
        ),
      },
      {
        path: '/consignments', // <-- Thêm route ký gửi ở đây
        element: (
          <RequireRole allowed={['Admin', 'Manager', 'ShowroomSales', 'Sales']}>
            <AdminConsignmentsPage />
          </RequireRole>
        ),
      },
      {
        path: '/users',
        element: (
          <RequireRole allowed={['Admin', 'Manager']}>
            <UsersPage />
          </RequireRole>
        ),
      },
      {
        path: '/articles',
        element: (
          <RequireRole allowed={['Admin','Manager', 'Content', 'Marketing']}>
            <ArticlesPage />
          </RequireRole>
        ),
      },
      {
        path: '/promotions',
        element: (
          <RequireRole allowed={['Admin', 'Manager', 'Marketing']}>
            <PromotionsPage />
          </RequireRole>
        ),
      },
      {
        path: '/notifications',
        element: (
          <RequireRole allowed={['Admin', 'Manager', 'Marketing', 'Content', 'ShowroomSales', 'Sales', 'Technician']}>
            <NotificationsPage />
          </RequireRole>
        ),
      },
      {
        path: '/inventories',
        element: (
          <RequireRole allowed={['Admin', 'Manager', 'ShowroomSales', 'Sales', 'Technician']}>
            <InventoriesPage />
          </RequireRole>
        ),
      },
      {
        path: '/reviews',
        element: (
          <RequireRole allowed={['Admin', 'Content']}>
            <ReviewsPage />
          </RequireRole>
        ),
      },
      { path: '*', element: <NotFoundPage /> },
    ],
  },
]