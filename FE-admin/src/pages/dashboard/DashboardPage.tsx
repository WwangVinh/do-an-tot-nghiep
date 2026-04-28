import { Link } from 'react-router-dom'
import {
  ArrowDownRight,
  ArrowUpRight,
  CalendarDays,
  Car,
  CircleDollarSign,
  Clock,
  Package,
  ShoppingBag,
  Users,
} from 'lucide-react'
import { useQuery } from '@tanstack/react-query'

import { useAuth } from '../../app/auth/useAuth'
import { fetchDashboardSummary } from '../../services/dashboard/dashboard'

type Trend = { value: number; direction: 'up' | 'down' }

function formatCompactVnd(value: number) {
  // Compact, readable for dashboard widgets (e.g. 1.2t, 850tr)
  const abs = Math.abs(value)
  if (abs >= 1_000_000_000_000) return `${(value / 1_000_000_000_000).toFixed(1)}t`
  if (abs >= 1_000_000_000) return `${(value / 1_000_000_000).toFixed(1)}b`
  if (abs >= 1_000_000) return `${(value / 1_000_000).toFixed(1)}tr`
  if (abs >= 1_000) return `${(value / 1_000).toFixed(1)}k`
  return `${value}`
}

function formatPercent(pct: number) {
  const sign = pct > 0 ? '+' : ''
  return `${sign}${pct.toFixed(1)}%`
}

function StatCard({
  title,
  value,
  icon,
  subtitle,
  trend,
  accent,
}: {
  title: string
  value: string
  icon: React.ReactNode
  subtitle?: string
  trend?: Trend
  accent?: 'indigo' | 'emerald' | 'amber' | 'rose'
}) {
  const accentStyles =
    accent === 'emerald'
      ? {
          ring: 'ring-emerald-200/60 dark:ring-emerald-900/30',
          icon: 'text-emerald-700 dark:text-emerald-300',
          badge: 'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200/70 dark:bg-emerald-950/40 dark:text-emerald-200 dark:ring-emerald-900/40',
        }
      : accent === 'amber'
        ? {
            ring: 'ring-amber-200/60 dark:ring-amber-900/30',
            icon: 'text-amber-700 dark:text-amber-300',
            badge: 'bg-amber-50 text-amber-700 ring-1 ring-amber-200/70 dark:bg-amber-950/40 dark:text-amber-200 dark:ring-amber-900/40',
          }
        : accent === 'rose'
          ? {
              ring: 'ring-rose-200/60 dark:ring-rose-900/30',
              icon: 'text-rose-700 dark:text-rose-300',
              badge: 'bg-rose-50 text-rose-700 ring-1 ring-rose-200/70 dark:bg-rose-950/40 dark:text-rose-200 dark:ring-rose-900/40',
            }
          : {
              ring: 'ring-indigo-200/60 dark:ring-indigo-900/30',
              icon: 'text-indigo-700 dark:text-indigo-300',
              badge: 'bg-indigo-50 text-indigo-700 ring-1 ring-indigo-200/70 dark:bg-indigo-950/40 dark:text-indigo-200 dark:ring-indigo-900/40',
            }

  const trendNode = trend ? (
    <span
      className={[
        'inline-flex items-center gap-1 rounded-full px-2 py-1 text-xs font-semibold',
        trend.direction === 'up'
          ? 'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200/70 dark:bg-emerald-950/40 dark:text-emerald-200 dark:ring-emerald-900/40'
          : 'bg-rose-50 text-rose-700 ring-1 ring-rose-200/70 dark:bg-rose-950/40 dark:text-rose-200 dark:ring-rose-900/40',
      ].join(' ')}
      title="So với kỳ trước"
    >
      {trend.direction === 'up' ? <ArrowUpRight size={14} /> : <ArrowDownRight size={14} />}
      {formatPercent(trend.value)}
    </span>
  ) : null

  return (
    <div className="relative overflow-hidden rounded-2xl border border-slate-200 bg-white p-4 shadow-sm ring-1 ring-transparent dark:border-zinc-800 dark:bg-zinc-950">
      <div className="flex items-start justify-between gap-3">
        <div>
          <div className="text-sm font-medium text-slate-500 dark:text-zinc-400">{title}</div>
          <div className="mt-1 text-2xl font-semibold tracking-tight text-slate-900 dark:text-zinc-50">{value}</div>
          {subtitle ? <div className="mt-1 text-xs text-slate-500 dark:text-zinc-400">{subtitle}</div> : null}
        </div>
        <div
          className={[
            'flex h-10 w-10 items-center justify-center rounded-xl bg-slate-50 ring-1',
            accentStyles.ring,
            'dark:bg-zinc-950',
          ].join(' ')}
        >
          <span className={accentStyles.icon}>{icon}</span>
        </div>
      </div>

      <div className="mt-3 flex items-center justify-between gap-3">
        {trendNode ?? <span className={['inline-flex rounded-full px-2 py-1 text-xs font-semibold', accentStyles.badge].join(' ')}>Nổi bật</span>}
        <span className="text-xs text-slate-400 dark:text-zinc-500">Cập nhật: hôm nay</span>
      </div>
    </div>
  )
}

function Sparkline({
  points,
  strokeClassName = 'stroke-indigo-500',
  fillClassName = 'fill-indigo-500/15',
}: {
  points: number[]
  strokeClassName?: string
  fillClassName?: string
}) {
  const w = 180
  const h = 52
  const pad = 4
  const min = Math.min(...points)
  const max = Math.max(...points)
  const range = Math.max(1e-9, max - min)

  const toXY = (v: number, i: number) => {
    const x = pad + (i * (w - pad * 2)) / Math.max(1, points.length - 1)
    const y = pad + (1 - (v - min) / range) * (h - pad * 2)
    return { x, y }
  }

  const lineD = points
    .map((v, i) => {
      const { x, y } = toXY(v, i)
      return `${i === 0 ? 'M' : 'L'} ${x.toFixed(2)} ${y.toFixed(2)}`
    })
    .join(' ')

  const areaD = `${lineD} L ${(w - pad).toFixed(2)} ${(h - pad).toFixed(2)} L ${pad.toFixed(2)} ${(h - pad).toFixed(2)} Z`

  return (
    <svg viewBox={`0 0 ${w} ${h}`} className="h-14 w-full">
      <path d={areaD} className={fillClassName} />
      <path d={lineD} className={['fill-none', strokeClassName].join(' ')} strokeWidth="2.5" strokeLinejoin="round" strokeLinecap="round" />
    </svg>
  )
}

function Bars({
  values,
  barClassName = 'bg-indigo-500',
}: {
  values: number[]
  barClassName?: string
}) {
  const max = Math.max(...values, 1)
  return (
    <div className="flex h-16 items-end gap-1.5">
      {values.map((v, idx) => (
        <div
          key={idx}
          className={['w-full rounded-md', barClassName].join(' ')}
          style={{ height: `${Math.max(6, (v / max) * 100)}%`, opacity: 0.35 + (idx / Math.max(1, values.length - 1)) * 0.6 }}
          title={`${v}`}
        />
      ))}
    </div>
  )
}

function Donut({
  segments,
}: {
  segments: { label: string; value: number; className: string }[]
}) {
  const total = segments.reduce((s, x) => s + x.value, 0) || 1
  const size = 132
  const stroke = 12
  const r = (size - stroke) / 2
  const c = 2 * Math.PI * r
  let offset = 0

  return (
    <div className="flex items-center gap-4">
      <svg width={size} height={size} viewBox={`0 0 ${size} ${size}`} className="shrink-0">
        <circle cx={size / 2} cy={size / 2} r={r} strokeWidth={stroke} className="fill-none stroke-slate-200 dark:stroke-zinc-800" />
        {segments.map((seg) => {
          const frac = seg.value / total
          const dash = frac * c
          const el = (
            <circle
              key={seg.label}
              cx={size / 2}
              cy={size / 2}
              r={r}
              strokeWidth={stroke}
              className={['fill-none', seg.className].join(' ')}
              strokeDasharray={`${dash} ${c - dash}`}
              strokeDashoffset={-offset}
              strokeLinecap="round"
              transform={`rotate(-90 ${size / 2} ${size / 2})`}
            />
          )
          offset += dash
          return el
        })}
        <text x="50%" y="50%" dominantBaseline="middle" textAnchor="middle" className="fill-slate-900 text-[16px] font-semibold dark:fill-zinc-50">
          {Math.round((segments[0]?.value ?? 0) / total * 100)}%
        </text>
        <text x="50%" y="62%" dominantBaseline="middle" textAnchor="middle" className="fill-slate-500 text-[11px] dark:fill-zinc-400">
          Hoàn tất
        </text>
      </svg>

      <div className="min-w-0">
        <div className="text-sm font-semibold text-slate-900 dark:text-zinc-50">Trạng thái đặt lịch</div>
        <div className="mt-2 space-y-1.5">
          {segments.map((seg) => (
            <div key={seg.label} className="flex items-center justify-between gap-3 text-sm">
              <div className="flex min-w-0 items-center gap-2">
                <span className={['h-2.5 w-2.5 rounded-full', seg.className.replace('stroke-', 'bg-')].join(' ')} />
                <span className="truncate text-slate-600 dark:text-zinc-300">{seg.label}</span>
              </div>
              <span className="shrink-0 font-medium text-slate-900 dark:text-zinc-100">{seg.value}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

export function DashboardPage() {
  const auth = useAuth()

  const days = 30
  const dashboardQuery = useQuery({
    queryKey: ['dashboard-summary', days],
    queryFn: () => fetchDashboardSummary(days),
    enabled: auth.isAuthenticated,
    staleTime: 30_000,
  })

  const summary = dashboardQuery.data
  const revenueSeries = summary?.revenueByMonth?.map((p) => p.value) ?? []
  const ordersSeries = summary?.ordersByWeek?.map((p) => p.value) ?? []

  const bookingSegments = (summary?.bookingByStatus ?? [])
    .slice(0, 3)
    .map((x, idx) => ({
      label: x.status,
      value: x.count,
      className: idx === 0 ? 'stroke-emerald-500' : idx === 1 ? 'stroke-amber-500' : 'stroke-rose-500',
    }))

  const recent = (summary?.recentOrders ?? []).slice(0, 4).map((o) => ({
    id: o.orderCode || `#${o.orderId}`,
    customer: o.customerName || '(Không rõ)',
    status: o.paymentStatus || o.status || 'Unknown',
    amount: o.amount,
    time: o.orderDate ? new Date(o.orderDate).toLocaleString() : '',
  }))

  return (
    <div>
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold">Tổng quan</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-zinc-300">
            Theo dõi nhanh hiệu suất và hoạt động gần đây của hệ thống.
          </p>
        </div>
        <div className="flex items-center gap-2">
          <div className="hidden items-center gap-2 rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 shadow-sm dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-200 md:flex">
            <Clock size={16} className="text-slate-400 dark:text-zinc-500" />
            30 ngày gần nhất
          </div>
          <Link
            to="/cars/new"
            className="rounded-xl bg-slate-900 px-3 py-2 text-sm font-semibold text-white hover:bg-slate-800 dark:bg-white dark:text-zinc-900 dark:hover:bg-zinc-100"
          >
            + Thêm xe
          </Link>
        </div>
      </div>

      {/* KPI */}
      <div className="mt-6 grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
        <StatCard
          title="Doanh thu"
          value={`${formatCompactVnd(summary?.revenue ?? 0)} đ`}
          subtitle={`Tổng ${days} ngày`}
          accent="emerald"
          icon={<CircleDollarSign size={18} />}
        />
        <StatCard
          title="Đơn hàng"
          value={`${summary?.orders ?? 0}`}
          subtitle={`Tổng ${days} ngày`}
          accent="indigo"
          icon={<ShoppingBag size={18} />}
        />
        <StatCard
          title="Đặt lịch"
          value={`${summary?.bookings ?? 0}`}
          subtitle={`Tổng ${days} ngày`}
          accent="amber"
          icon={<CalendarDays size={18} />}
        />
        <StatCard
          title="Người dùng"
          value={`${summary?.activeUsers ?? 0}`}
          subtitle="Đang hoạt động"
          accent="indigo"
          icon={<Users size={18} />}
        />
        <StatCard
          title="Số xe"
          value={`${summary?.activeCars ?? 0}`}
          subtitle="Đang hiển thị"
          accent="emerald"
          icon={<Car size={18} />}
        />
        <StatCard
          title="Tồn kho"
          value={`${summary?.inventoryQuantity ?? 0}`}
          subtitle="Tổng số lượng"
          accent="rose"
          icon={<Package size={18} />}
        />
      </div>

      {/* Charts + highlights */}
      <div className="mt-6 grid grid-cols-1 gap-4 xl:grid-cols-3">
        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm dark:border-zinc-800 dark:bg-zinc-950 xl:col-span-2">
          <div className="flex items-start justify-between gap-4">
            <div>
              <div className="text-sm font-semibold text-slate-900 dark:text-zinc-50">Doanh thu theo tháng</div>
              <div className="mt-1 text-xs text-slate-500 dark:text-zinc-400">Xu hướng 12 tháng (demo)</div>
            </div>
            <div className="rounded-full bg-slate-50 px-2.5 py-1 text-xs font-semibold text-slate-700 ring-1 ring-slate-200/70 dark:bg-zinc-950 dark:text-zinc-200 dark:ring-zinc-800">
              {formatCompactVnd(summary?.revenue ?? 0)} đ / {days} ngày
            </div>
          </div>
          <div className="mt-3">
            <Sparkline
              points={revenueSeries.length ? revenueSeries : [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]}
              strokeClassName="stroke-emerald-500"
              fillClassName="fill-emerald-500/15"
            />
          </div>
          <div className="mt-3 grid grid-cols-2 gap-3 md:grid-cols-4">
            {[
              { label: 'Đơn mới', value: '48', cls: 'bg-indigo-50 text-indigo-700 ring-indigo-200/70 dark:bg-indigo-950/40 dark:text-indigo-200 dark:ring-indigo-900/40' },
              { label: 'Chờ xử lý', value: '31', cls: 'bg-amber-50 text-amber-700 ring-amber-200/70 dark:bg-amber-950/40 dark:text-amber-200 dark:ring-amber-900/40' },
              { label: 'Hoàn tất', value: '162', cls: 'bg-emerald-50 text-emerald-700 ring-emerald-200/70 dark:bg-emerald-950/40 dark:text-emerald-200 dark:ring-emerald-900/40' },
              { label: 'Hoàn tiền', value: '2', cls: 'bg-rose-50 text-rose-700 ring-rose-200/70 dark:bg-rose-950/40 dark:text-rose-200 dark:ring-rose-900/40' },
            ].map((x) => (
              <div key={x.label} className={['rounded-xl px-3 py-2 ring-1', x.cls].join(' ')}>
                <div className="text-[11px] font-semibold opacity-80">{x.label}</div>
                <div className="mt-0.5 text-lg font-semibold">{x.value}</div>
              </div>
            ))}
          </div>
        </div>

        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm dark:border-zinc-800 dark:bg-zinc-950">
          <Donut segments={bookingSegments} />
          <div className="mt-4 border-t border-slate-200/70 pt-4 dark:border-zinc-800/70">
            <div className="flex items-start justify-between gap-3">
              <div>
                <div className="text-sm font-semibold text-slate-900 dark:text-zinc-50">Đơn hàng theo tuần</div>
                <div className="mt-1 text-xs text-slate-500 dark:text-zinc-400">So sánh nhanh (demo)</div>
              </div>
              <div className="rounded-full bg-slate-50 px-2.5 py-1 text-xs font-semibold text-slate-700 ring-1 ring-slate-200/70 dark:bg-zinc-950 dark:text-zinc-200 dark:ring-zinc-800">
              {summary?.orders ?? 0} đơn
              </div>
            </div>
            <div className="mt-3">
            <Bars values={(ordersSeries.length ? ordersSeries : [0, 0, 0, 0, 0, 0, 0, 0]).slice(-8)} barClassName="bg-indigo-500" />
            </div>
          </div>
        </div>
      </div>

      {/* System info + recent activity */}
      <div className="mt-6 grid grid-cols-1 gap-4 xl:grid-cols-3">
        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm dark:border-zinc-800 dark:bg-zinc-950">
          <div className="text-sm font-semibold text-slate-900 dark:text-zinc-50">Tình trạng hệ thống</div>
          <div className="mt-3 space-y-3">
            <div className="flex items-center justify-between gap-3">
              <span className="text-sm text-slate-600 dark:text-zinc-300">Đăng nhập</span>
              <span className="rounded-full bg-emerald-50 px-2 py-1 text-xs font-semibold text-emerald-700 ring-1 ring-emerald-200/70 dark:bg-emerald-950/40 dark:text-emerald-200 dark:ring-emerald-900/40">
                {auth.isAuthenticated ? 'OK' : 'Chưa có'}
              </span>
            </div>
            <div className="flex items-center justify-between gap-3">
              <span className="text-sm text-slate-600 dark:text-zinc-300">API base</span>
              <span className="max-w-[60%] truncate text-sm font-medium text-slate-900 dark:text-zinc-100" title={import.meta.env.VITE_API_BASE_URL ?? '(default)'}>
                {import.meta.env.VITE_API_BASE_URL ?? '(default)'}
              </span>
            </div>
            <div className="flex items-center justify-between gap-3">
              <span className="text-sm text-slate-600 dark:text-zinc-300">Phiên bản</span>
              <span className="text-sm font-medium text-slate-900 dark:text-zinc-100">Admin v0.1</span>
            </div>
          </div>
        </div>

        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm dark:border-zinc-800 dark:bg-zinc-950 xl:col-span-2">
          <div className="flex items-start justify-between gap-3">
            <div>
              <div className="text-sm font-semibold text-slate-900 dark:text-zinc-50">Hoạt động gần đây</div>
              <div className="mt-1 text-xs text-slate-500 dark:text-zinc-400">4 giao dịch mới nhất (demo)</div>
            </div>
            <Link
              to="/orders"
              className="rounded-xl bg-slate-50 px-3 py-2 text-sm font-semibold text-slate-800 ring-1 ring-slate-200/70 hover:bg-slate-100 dark:bg-zinc-950 dark:text-zinc-200 dark:ring-zinc-800 dark:hover:bg-zinc-900"
            >
              Xem đơn hàng
            </Link>
          </div>

          <div className="mt-3 overflow-hidden rounded-xl border border-slate-200 dark:border-zinc-800">
            <div className="grid grid-cols-[1fr_140px_120px] gap-3 bg-slate-50 px-3 py-2 text-xs font-semibold text-slate-600 dark:bg-zinc-950 dark:text-zinc-400">
              <div>Mã / Khách</div>
              <div className="text-right">Số tiền</div>
              <div className="text-right">Thời gian</div>
            </div>
            <div className="divide-y divide-slate-200 dark:divide-zinc-800">
              {dashboardQuery.isLoading ? (
                <div className="px-3 py-6 text-sm text-slate-500 dark:text-zinc-400">Đang tải dữ liệu…</div>
              ) : dashboardQuery.isError ? (
                <div className="px-3 py-6 text-sm text-rose-700 dark:text-rose-200">
                  Không tải được dữ liệu dashboard. Vui lòng kiểm tra API và token.
                </div>
              ) : recent.length === 0 ? (
                <div className="px-3 py-6 text-sm text-slate-500 dark:text-zinc-400">Chưa có dữ liệu.</div>
              ) : (
                recent.map((row) => (
                <div key={row.id} className="grid grid-cols-[1fr_140px_120px] items-center gap-3 px-3 py-3">
                  <div className="min-w-0">
                    <div className="flex items-center gap-2">
                      <span className="rounded-md bg-slate-50 px-2 py-0.5 text-xs font-semibold text-slate-700 ring-1 ring-slate-200/70 dark:bg-zinc-950 dark:text-zinc-200 dark:ring-zinc-800">
                        {row.id}
                      </span>
                      <span
                        className={[
                          'rounded-full px-2 py-0.5 text-xs font-semibold ring-1',
                          row.status === 'Đã thanh toán' || row.status === 'Hoàn tất'
                            ? 'bg-emerald-50 text-emerald-700 ring-emerald-200/70 dark:bg-emerald-950/40 dark:text-emerald-200 dark:ring-emerald-900/40'
                            : row.status === 'Chờ xác nhận'
                              ? 'bg-amber-50 text-amber-700 ring-amber-200/70 dark:bg-amber-950/40 dark:text-amber-200 dark:ring-amber-900/40'
                              : 'bg-indigo-50 text-indigo-700 ring-indigo-200/70 dark:bg-indigo-950/40 dark:text-indigo-200 dark:ring-indigo-900/40',
                        ].join(' ')}
                      >
                        {row.status}
                      </span>
                    </div>
                    <div className="mt-1 truncate text-sm font-medium text-slate-900 dark:text-zinc-100">{row.customer}</div>
                  </div>
                  <div className="text-right text-sm font-semibold text-slate-900 dark:text-zinc-100">
                    {formatCompactVnd(row.amount)} đ
                  </div>
                  <div className="text-right text-sm text-slate-500 dark:text-zinc-400">{row.time}</div>
                </div>
                ))
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

