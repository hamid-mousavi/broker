import React, { useEffect, useMemo, useState } from 'react'
import StatsCard from '../../components/StatsCard'
import { Users, UserPlus, ClipboardList, BadgeCheck } from 'lucide-react'
import api from '../../utils/api'

function toNumber(value) {
  return Number(value || 0)
}

export default function AdminDashboard() {
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [overview, setOverview] = useState(null)
  const [usersReport, setUsersReport] = useState(null)
  const [daily, setDaily] = useState([])

  useEffect(() => {
    const load = async () => {
      setLoading(true)
      setError('')
      try {
        const [overviewRes, usersRes, dailyRes] = await Promise.all([
          api.get('/admin/statistics/overview'),
          api.get('/admin/reports/users'),
          api.get('/admin/statistics/daily'),
        ])
        setOverview(overviewRes?.data?.data || null)
        setUsersReport(usersRes?.data?.data || null)
        setDaily(dailyRes?.data?.data || [])
      } catch (err) {
        setError(err?.response?.data?.message || 'خطا در دریافت اطلاعات داشبورد')
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [])

  const todayUsers = useMemo(() => {
    const today = new Date().toISOString().slice(0, 10)
    const todayRow = (daily || []).find((item) => {
      const dateValue = item?.date || item?.Date
      return String(dateValue || '').slice(0, 10) === today
    })
    if (!todayRow) return 0
    return toNumber(todayRow.newUsers ?? todayRow.NewUsers)
  }, [daily])

  const last7Days = useMemo(() => {
    const rows = (daily || []).slice(-7)
    return rows.map((item) => ({
      label: String(item?.date || item?.Date || '').slice(5, 10),
      users: toNumber(item?.newUsers ?? item?.NewUsers),
      requests: toNumber(item?.newRequests ?? item?.NewRequests),
    }))
  }, [daily])

  const maxForBars = useMemo(() => {
    const maxValue = Math.max(
      1,
      ...last7Days.flatMap((item) => [item.users, item.requests])
    )
    return maxValue
  }, [last7Days])

  return (
    <div className="space-y-6">
      {error && (
        <div className="rounded border border-rose-200 bg-rose-50 text-rose-700 px-3 py-2 text-sm">
          {error}
        </div>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatsCard
          title="کاربران فعال"
          value={loading ? '...' : toNumber(usersReport?.activeUsers ?? usersReport?.ActiveUsers).toLocaleString()}
          icon={Users}
        />
        <StatsCard
          title="کاربران جدید امروز"
          value={loading ? '...' : todayUsers.toLocaleString()}
          icon={UserPlus}
        />
        <StatsCard
          title="درخواست‌های در انتظار"
          value={loading ? '...' : toNumber(overview?.pendingRequests ?? overview?.PendingRequests).toLocaleString()}
          icon={ClipboardList}
        />
        <StatsCard
          title="تاییدیه‌های معلق"
          value={loading ? '...' : toNumber(overview?.pendingVerifications ?? overview?.PendingVerifications).toLocaleString()}
          icon={BadgeCheck}
        />
      </div>

      <section className="card p-4">
        <h2 className="text-lg font-semibold mb-1">روند ۷ روز اخیر</h2>
        <p className="text-xs text-slate-500 mb-4">کاربران جدید و درخواست‌های جدید</p>

        {last7Days.length === 0 ? (
          <div className="text-sm text-slate-500">داده‌ای برای نمایش وجود ندارد.</div>
        ) : (
          <div className="space-y-3">
            {last7Days.map((item) => (
              <div key={item.label} className="grid grid-cols-[56px_1fr_1fr] items-center gap-3">
                <div className="text-xs text-slate-500">{item.label}</div>
                <div className="h-3 bg-slate-100 rounded">
                  <div
                    className="h-3 rounded bg-sky-500"
                    style={{ width: `${(item.users / maxForBars) * 100}%` }}
                    title={`کاربر جدید: ${item.users}`}
                  />
                </div>
                <div className="h-3 bg-slate-100 rounded">
                  <div
                    className="h-3 rounded bg-amber-500"
                    style={{ width: `${(item.requests / maxForBars) * 100}%` }}
                    title={`درخواست جدید: ${item.requests}`}
                  />
                </div>
              </div>
            ))}
            <div className="flex items-center gap-4 text-xs text-slate-500 pt-2">
              <span className="inline-flex items-center gap-2">
                <span className="h-2 w-4 bg-sky-500 rounded" /> کاربران جدید
              </span>
              <span className="inline-flex items-center gap-2">
                <span className="h-2 w-4 bg-amber-500 rounded" /> درخواست‌های جدید
              </span>
            </div>
          </div>
        )}
      </section>
    </div>
  )
}
