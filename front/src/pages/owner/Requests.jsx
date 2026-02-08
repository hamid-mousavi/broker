import React, { useEffect, useState } from 'react'
import { ClipboardList, Filter, Calendar } from 'lucide-react'
import api from '../../utils/api'

export default function OwnerRequests() {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [status, setStatus] = useState('')

  useEffect(() => {
    const load = async () => {
      try {
        const res = await api.get('/cargo-owners/requests', {
          params: { pageNumber: 1, pageSize: 20, status: status || undefined },
        })
        setData(res?.data?.data || null)
      } catch (err) {
        setData(null)
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [status])

  const items = data?.requests || data?.items || []

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <h2 className="text-lg font-semibold flex items-center gap-2">
          <ClipboardList size={18} /> درخواست‌های من
        </h2>
        <div className="flex items-center gap-2">
          <Filter size={16} className="text-slate-400" />
          <select
            className="px-3 py-2 rounded border bg-white text-sm"
            value={status}
            onChange={(event) => setStatus(event.target.value)}
          >
            <option value="">همه وضعیت‌ها</option>
            <option value="0">در انتظار</option>
            <option value="1">در حال انجام</option>
            <option value="2">تکمیل شده</option>
            <option value="3">لغو شده</option>
            <option value="4">رد شده</option>
          </select>
        </div>
      </div>
      {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}
      {!loading && (
        <>
          <div className="space-y-3 md:hidden">
            {items.map((req) => (
              <div key={req.id} className="card p-4">
                <div className="font-semibold">{req.title || '-'}</div>
                <div className="mt-2 flex items-center gap-2 text-xs text-slate-500">
                  <Calendar size={12} />
                  {req.createdAt ? new Date(req.createdAt).toLocaleDateString('fa-IR') : '-'}
                </div>
                <div className="mt-2">
                  <span className="inline-flex items-center rounded-full px-2 py-1 text-xs bg-slate-100 text-slate-700">
                    {req.status || '-'}
                  </span>
                </div>
              </div>
            ))}
            {items.length === 0 && (
              <div className="card p-4 text-center text-slate-500 text-sm">موردی یافت نشد</div>
            )}
          </div>
          <div className="card p-0 hidden md:block">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-slate-500">
                  <th className="text-right p-2">عنوان</th>
                  <th className="text-right p-2">وضعیت</th>
                  <th className="text-right p-2">تاریخ</th>
                </tr>
              </thead>
              <tbody>
                {items.map((req) => (
                  <tr key={req.id} className="border-t">
                    <td className="p-2">{req.title || '-'}</td>
                    <td className="p-2">
                      <span className="inline-flex items-center rounded-full px-2 py-1 text-xs bg-slate-100 text-slate-700">
                        {req.status || '-'}
                      </span>
                    </td>
                    <td className="p-2">
                      <span className="inline-flex items-center gap-1 text-xs text-slate-500">
                        <Calendar size={12} />
                        {req.createdAt ? new Date(req.createdAt).toLocaleDateString('fa-IR') : '-'}
                      </span>
                    </td>
                  </tr>
                ))}
                {items.length === 0 && (
                  <tr>
                    <td className="p-3 text-center text-slate-500" colSpan={3}>
                      موردی یافت نشد
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  )
}
