import type { ReactNode } from 'react'
import { NavLink, useNavigate } from 'react-router-dom'
import {
  Bell, CalendarDays, Gift, Image, LayoutDashboard,
  LogOut, Newspaper, Package, ShoppingBag, Star,
  Store, Users, Wrench,
} from 'lucide-react'

import { clearAuth } from '../../app/auth/authStore'
import { useAuth } from '../../app/auth/useAuth'
import { isInRole } from '../../app/auth/roles'

function NavItem({
  to,
  label,
  icon,
  end = true,
  collapsed = false,
}: {
  to: string
  label: string
  icon: ReactNode
  end?: boolean
  collapsed?: boolean
}) {
  return (
    <NavLink
      to={to}
      title={collapsed ? label : undefined}
      className={({ isActive }) =>
        [
          'group flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm transition-all duration-200',
          'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500/30',
          collapsed ? 'justify-center px-2' : '',
          isActive
            ? 'bg-white text-indigo-700 shadow-sm ring-1 ring-indigo-100 dark:bg-zinc-950 dark:text-indigo-200 dark:ring-indigo-500/10'
            : 'text-slate-700 hover:bg-white/70 hover:shadow-sm hover:ring-1 hover:ring-slate-200/70 dark:text-zinc-200 dark:hover:bg-zinc-950/60 dark:hover:ring-zinc-800/70',
        ].join(' ')
      }
      end={end}
    >
      {({ isActive }) => (
        <>
          <span className={[
            'flex-shrink-0 transition-colors',
            isActive
              ? 'text-indigo-600 dark:text-indigo-300'
              : 'text-slate-400 group-hover:text-slate-500 dark:text-zinc-400 dark:group-hover:text-zinc-300',
          ].join(' ')}>
            {icon}
          </span>
          {!collapsed && <span className="truncate font-medium">{label}</span>}
        </>
      )}
    </NavLink>
  )
}

export function AdminSidebar({
  onNavigate,
  collapsed = false,
}: {
  onNavigate?: () => void
  collapsed?: boolean
}) {
  const navigate = useNavigate()
  const auth = useAuth()
  const userRole = auth.user?.role

  const canSeeCars        = isInRole(userRole, ['Admin', 'Manager', 'ShowroomSales', 'Sales', 'Technician'])
  const canSeeInventories = isInRole(userRole, ['Admin', 'Manager', 'ShowroomSales', 'Sales', 'Technician'])
  const canSeeOrders      = isInRole(userRole, ['Admin', 'Manager', 'ShowroomSales', 'Sales'])
  const canSeeBookings    = isInRole(userRole, ['Admin', 'Manager', 'ShowroomSales', 'Sales', 'Technician'])
  const canSeeShowrooms   = isInRole(userRole, ['Admin', 'Manager'])
  const canSeeUsers       = isInRole(userRole, ['Admin', 'Manager'])
  const canSeeReviews     = isInRole(userRole, ['Admin', 'Manager', 'Marketing'])
  const canSeeNotifications = isInRole(userRole, ['Admin', 'Manager'])
  const canSeeBanners     = isInRole(userRole, ['Admin', 'Manager', 'Marketing', 'Content'])
  const canSeeArticles    = isInRole(userRole, ['Admin', 'Manager', 'Marketing', 'Content'])
  const canSeePromotions  = isInRole(userRole, ['Admin', 'Manager', 'Marketing'])

  return (
    <div className="flex h-full flex-col">
      {/* Header label — ẩn khi collapsed */}
      {!collapsed && (
        <div className="px-3 pt-3">
          <div className="mb-3 flex items-center gap-3">
            <div className="h-px flex-1 bg-slate-200/70 dark:bg-zinc-800/70" />
            <div className="text-xs font-semibold uppercase tracking-wide text-slate-400 dark:text-zinc-500">
              Menu
            </div>
            <div className="h-px flex-1 bg-slate-200/70 dark:bg-zinc-800/70" />
          </div>
        </div>
      )}

      {/* Nav items */}
      <div
        className={['flex-1 overflow-y-auto px-2 pb-3', collapsed ? 'pt-3' : ''].join(' ')}
        onClick={onNavigate}
      >
        <div className="space-y-1">
          <NavItem to="/" label="Tổng quan"   icon={<LayoutDashboard size={16} />} collapsed={collapsed} />
          {canSeeBanners     && <NavItem to="/banners"       label="Banner"      icon={<Image size={16} />}        collapsed={collapsed} />}
          {canSeeCars        && <NavItem to="/cars"          label="Xe"          icon={<Package size={16} />}      end={false} collapsed={collapsed} />}
          {canSeeShowrooms   && <NavItem to="/showrooms"     label="Showroom"    icon={<Store size={16} />}        collapsed={collapsed} />}
          {canSeeOrders      && <NavItem to="/orders"        label="Đơn hàng"    icon={<ShoppingBag size={16} />}  collapsed={collapsed} />}
          {canSeeBookings    && <NavItem to="/bookings"      label="Đặt lịch"    icon={<CalendarDays size={16} />} collapsed={collapsed} />}
          {canSeeUsers       && <NavItem to="/users"         label="Người dùng"  icon={<Users size={16} />}        collapsed={collapsed} />}
          {canSeeInventories && <NavItem to="/inventories"   label="Tồn kho"     icon={<Wrench size={16} />}       collapsed={collapsed} />}
          {canSeeArticles    && <NavItem to="/articles"      label="Bài viết"    icon={<Newspaper size={16} />}    collapsed={collapsed} />}
          {canSeePromotions  && <NavItem to="/promotions"    label="Khuyến mãi"  icon={<Gift size={16} />}         collapsed={collapsed} />}
          {canSeeNotifications && <NavItem to="/notifications" label="Thông báo" icon={<Bell size={16} />}        collapsed={collapsed} />}
          {canSeeReviews     && <NavItem to="/reviews"       label="Đánh giá"    icon={<Star size={16} />}         collapsed={collapsed} />}
        </div>
      </div>

      {/* Logout */}
      <div className="border-t border-slate-200/70 px-2 py-3 dark:border-zinc-800/70">
        <button
          type="button"
          title={collapsed ? 'Đăng xuất' : undefined}
          className={[
            'flex w-full items-center gap-2 rounded-xl px-3 py-2 text-sm font-medium transition-colors',
            collapsed ? 'justify-center px-2' : 'justify-center',
            'border border-slate-200 bg-white text-slate-700 hover:bg-slate-50',
            'dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 dark:hover:bg-zinc-900',
            'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500/30',
          ].join(' ')}
          onClick={() => { clearAuth(); navigate('/login') }}
        >
          <LogOut size={16} className="flex-shrink-0" />
          {!collapsed && <span>Đăng xuất</span>}
        </button>
      </div>
    </div>
  )
}