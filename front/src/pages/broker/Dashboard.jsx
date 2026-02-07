import React, { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import api from '../../utils/api'

export default function BrokerDashboard() {
  const [summary, setSummary] = useState(null)
  const [requests, setRequests] = useState([])
  const [analytics, setAnalytics] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const load = async () => {
      try {
        const [summaryRes, requestsRes, analyticsRes] = await Promise.all([
          api.get('/brokers/dashboard/summary'),
          api.get('/brokers/dashboard/requests', { params: { pageNumber: 1, pageSize: 5 } }),
          api.get('/brokers/dashboard/analytics'),
        ])
        setSummary(summaryRes?.data?.data || {})
        const reqData = requestsRes?.data?.data
        setRequests(reqData?.requests || reqData?.items || [])
        setAnalytics(analyticsRes?.data?.data || null)
      } catch (err) {
        setSummary({})
        setRequests([])
        setAnalytics(null)
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [])

  const quickActions = useMemo(
    () => [
      { label: 'درخواست‌ها', to: '/dashboard/broker/requests' },
      { label: 'قرار ملاقات‌ها', to: '/dashboard/broker/appointments' },
      { label: 'تکمیل پروفایل', to: '/profile/complete' },
    ],
    []
  )

  return (
    <div className="space-y-4">
      <h2 className="text-lg font-semibold">داشبورد ترخیص‌کار</h2>
      {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}
      {!loading && (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div className="bg-white border rounded p-4">
              <div className="text-sm text-slate-500">درآمد ماه</div>
              <div className="text-xl font-semibold">{summary?.monthlyIncome || 0} تومان</div>
            </div>
            <div className="bg-white border rounded p-4">
              <div className="text-sm text-slate-500">درخواست‌های جدید</div>
              <div className="text-xl font-semibold">{summary?.newRequests || 0}</div>
            </div>
            <div className="bg-white border rounded p-4">
              <div className="text-sm text-slate-500">امتیاز میانگین</div>
              <div className="text-xl font-semibold">{summary?.rating || 0}</div>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-[1.4fr_1fr] gap-4">
            <div className="bg-white border rounded p-4">
              <div className="flex items-center justify-between mb-3">
                <div className="font-semibold">درخواست‌های جدید</div>
                <Link className="text-sm text-slate-500" to="/dashboard/broker/requests">
                  مشاهده همه
                </Link>
              </div>
              <div className="space-y-2 text-sm">
                {requests.map((req) => (
                  <div key={req.id} className="border rounded p-2 flex items-center justify-between">
                    <div>
                      <div className="font-medium">{req.title || 'بدون عنوان'}</div>
                      <div className="text-xs text-slate-500">
                        {req.createdAt ? new Date(req.createdAt).toLocaleDateString('fa-IR') : '-'}
                      </div>
                    </div>
                    <span className="text-xs text-slate-500">{req.status || '-'}</span>
                  </div>
                ))}
                {requests.length === 0 && (
                  <div className="text-slate-500">درخواستی وجود ندارد.</div>
                )}
              </div>
            </div>

            <div className="space-y-4">
              <div className="bg-white border rounded p-4">
                <div className="font-semibold mb-3">نمودار پیشرفت</div>
                <div className="h-36 rounded bg-slate-100 flex items-center justify-center text-slate-500 text-sm">
                  {analytics ? 'نمودار داده‌ها' : 'نمودار (Placeholder)'}
                </div>
              </div>
              <div className="bg-white border rounded p-4">
                <div className="font-semibold mb-3">دسترسی سریع</div>
                <div className="flex flex-wrap gap-2">
                  {quickActions.map((item) => (
                    <Link key={item.to} to={item.to} className="px-3 py-2 rounded border text-sm">
                      {item.label}
                    </Link>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  )
}
