import React, { useEffect, useState } from 'react'
import { ClipboardList, Filter, Calendar } from 'lucide-react'
import api from '../../utils/api'

export default function BrokerRequests() {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [status, setStatus] = useState('')
  const [refreshing, setRefreshing] = useState(false)
  const [updatingId, setUpdatingId] = useState(null)

  const load = async (silent = false) => {
    if (!silent) setLoading(true)
    setRefreshing(true)
    try {
      const res = await api.get('/brokers/dashboard/requests', {
        params: { pageNumber: 1, pageSize: 20, status: status || undefined },
      })
      setData(res?.data?.data || null)
    } catch (err) {
      if (!silent) setData(null)
    } finally {
      setLoading(false)
      setRefreshing(false)
    }
  }

  useEffect(() => {
    load()
  }, [status])

  const items = data?.requests || data?.items || []

  const updateStatus = async (requestId, nextStatus) => {
    setUpdatingId(requestId)
    try {
      await api.put(`/request/${requestId}/status`, { status: Number(nextStatus) })
      setData((prev) => {
        if (!prev) return prev
        const list = prev.requests || prev.items || []
        const updated = list.map((req) =>
          req.id === requestId
            ? { ...req, status: Number(nextStatus), statusName: req.statusName }
            : req
        )
        return { ...prev, requests: updated, items: updated }
      })
    } catch (err) {
      // ignore for now
    } finally {
      setUpdatingId(null)
    }
  }

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
            <option value="1">در انتظار</option>
            <option value="2">در حال انجام</option>
            <option value="3">تکمیل شده</option>
            <option value="4">لغو شده</option>
            <option value="5">رد شده</option>
          </select>
          <button
            type="button"
            className="px-3 py-2 rounded border text-sm"
            onClick={() => load(true)}
            disabled={refreshing}
          >
            {refreshing ? 'در حال بروزرسانی...' : 'بروزرسانی'}
          </button>
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
                    {req.statusName || req.status || '-'}
                  </span>
                </div>
                <div className="mt-3">
                  <select
                    className="w-full px-3 py-2 rounded border bg-white text-sm"
                    value={req.status}
                    onChange={(e) => updateStatus(req.id, e.target.value)}
                    disabled={updatingId === req.id}
                  >
                    <option value="1">در انتظار</option>
                    <option value="2">در حال انجام</option>
                    <option value="3">تکمیل شده</option>
                    <option value="4">لغو شده</option>
                    <option value="5">رد شده</option>
                  </select>
                </div>
                <div className="mt-2 flex gap-2">
                  <button
                    type="button"
                    className="px-3 py-2 rounded border text-sm"
                    onClick={() => updateStatus(req.id, 2)}
                    disabled={updatingId === req.id}
                  >
                    قبول
                  </button>
                  <button
                    type="button"
                    className="px-3 py-2 rounded border text-sm text-rose-600"
                    onClick={() => updateStatus(req.id, 5)}
                    disabled={updatingId === req.id}
                  >
                    رد
                  </button>
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
                  <th className="text-right p-2">اقدام</th>
                </tr>
              </thead>
              <tbody>
                {items.map((req) => (
                  <tr key={req.id} className="border-t">
                    <td className="p-2">{req.title || '-'}</td>
                    <td className="p-2">
                      <span className="inline-flex items-center rounded-full px-2 py-1 text-xs bg-slate-100 text-slate-700">
                        {req.statusName || req.status || '-'}
                      </span>
                    </td>
                    <td className="p-2">
                      <span className="inline-flex items-center gap-1 text-xs text-slate-500">
                        <Calendar size={12} />
                        {req.createdAt ? new Date(req.createdAt).toLocaleDateString('fa-IR') : '-'}
                      </span>
                    </td>
                    <td className="p-2">
                      <select
                        className="px-3 py-2 rounded border bg-white text-sm"
                        value={req.status}
                        onChange={(e) => updateStatus(req.id, e.target.value)}
                        disabled={updatingId === req.id}
                      >
                        <option value="1">در انتظار</option>
                        <option value="2">در حال انجام</option>
                        <option value="3">تکمیل شده</option>
                        <option value="4">لغو شده</option>
                        <option value="5">رد شده</option>
                      </select>
                      <div className="mt-2 flex gap-2">
                        <button
                          type="button"
                          className="px-3 py-1 rounded border text-xs"
                          onClick={() => updateStatus(req.id, 2)}
                          disabled={updatingId === req.id}
                        >
                          قبول
                        </button>
                        <button
                          type="button"
                          className="px-3 py-1 rounded border text-xs text-rose-600"
                          onClick={() => updateStatus(req.id, 5)}
                          disabled={updatingId === req.id}
                        >
                          رد
                        </button>
                      </div>
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
