import React, { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ClipboardList, Clock, Heart, ArrowUpRight } from 'lucide-react'
import api from '../../utils/api'

export default function OwnerDashboard() {
  const [summary, setSummary] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const load = async () => {
      try {
        const res = await api.get('/cargo-owners/dashboard')
        setSummary(res?.data?.data || {})
      } catch (err) {
        setSummary({})
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [])

  return (
    <div className="space-y-4">
      <h2 className="text-lg font-semibold">داشبورد صاحب کالا</h2>
      {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}
      {!loading && (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div className="card p-4">
              <div className="text-sm text-slate-500 flex items-center gap-2">
                <ClipboardList size={16} /> کل درخواست‌ها
              </div>
              <div className="text-xl font-semibold">{summary?.totalRequests || 0}</div>
            </div>
            <div className="card p-4">
              <div className="text-sm text-slate-500 flex items-center gap-2">
                <Clock size={16} /> در انتظار
              </div>
              <div className="text-xl font-semibold">{summary?.pendingRequests || 0}</div>
            </div>
            <div className="card p-4">
              <div className="text-sm text-slate-500 flex items-center gap-2">
                <Heart size={16} /> ذخیره‌شده‌ها
              </div>
              <div className="text-xl font-semibold">{summary?.favoritesCount || 0}</div>
            </div>
          </div>
          <div className="card p-4">
            <div className="font-semibold mb-2 flex items-center gap-2">
              <ArrowUpRight size={16} /> دسترسی سریع
            </div>
            <div className="flex flex-wrap gap-2 text-sm">
              <Link className="px-3 py-2 rounded border" to="/dashboard/owner/requests">
                پیگیری درخواست‌ها
              </Link>
              <Link className="px-3 py-2 rounded border" to="/dashboard/owner/favorites">
                ترخیص‌کاران ذخیره‌شده
              </Link>
              <Link className="px-3 py-2 rounded border" to="/dashboard/owner/notifications">
                تنظیمات اعلان
              </Link>
            </div>
          </div>
        </>
      )}
    </div>
  )
}
