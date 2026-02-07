import React, { useEffect, useState } from 'react'
import api from '../../utils/api'

export default function BrokerRequests() {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [status, setStatus] = useState('')

  useEffect(() => {
    const load = async () => {
      try {
        const res = await api.get('/brokers/dashboard/requests', {
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
      <h2 className="text-lg font-semibold">درخواست‌های من</h2>
      <div className="flex items-center gap-2">
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
      {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}
      {!loading && (
        <div className="bg-white border rounded">
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
                  <td className="p-2">{req.status || '-'}</td>
                  <td className="p-2">
                    {req.createdAt ? new Date(req.createdAt).toLocaleDateString('fa-IR') : '-'}
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
      )}
    </div>
  )
}
