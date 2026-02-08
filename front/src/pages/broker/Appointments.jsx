import React, { useEffect, useState } from 'react'
import { Calendar, MapPin, ClipboardList } from 'lucide-react'
import api from '../../utils/api'

export default function BrokerAppointments() {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const load = async () => {
      try {
        const res = await api.get('/brokers/dashboard/appointments')
        setItems(res?.data?.data || [])
      } catch (err) {
        setItems([])
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [])

  return (
    <div className="space-y-4">
      <h2 className="text-lg font-semibold flex items-center gap-2">
        <Calendar size={18} /> قرار ملاقات‌ها
      </h2>
      {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}
      {!loading && (
        <>
          <div className="space-y-3 md:hidden">
            {items.map((app) => (
              <div key={app.id} className="card p-4">
                <div className="text-xs text-slate-500 flex items-center gap-2">
                  <Calendar size={12} />
                  {app.appointmentDate
                    ? new Date(app.appointmentDate).toLocaleDateString('fa-IR')
                    : '-'}
                </div>
                <div className="mt-2 text-sm flex items-center gap-2 text-slate-600">
                  <MapPin size={12} />
                  {app.location || '-'}
                </div>
                <div className="mt-2">
                  <span className="inline-flex items-center gap-1 text-xs text-slate-700 bg-slate-100 rounded-full px-2 py-1">
                    <ClipboardList size={12} />
                    {app.status || '-'}
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
                  <th className="text-right p-2">تاریخ</th>
                  <th className="text-right p-2">مکان</th>
                  <th className="text-right p-2">وضعیت</th>
                </tr>
              </thead>
              <tbody>
                {items.map((app) => (
                  <tr key={app.id} className="border-t">
                    <td className="p-2">
                      <span className="inline-flex items-center gap-1 text-xs text-slate-500">
                        <Calendar size={12} />
                        {app.appointmentDate
                          ? new Date(app.appointmentDate).toLocaleDateString('fa-IR')
                          : '-'}
                      </span>
                    </td>
                    <td className="p-2">
                      <span className="inline-flex items-center gap-1 text-xs text-slate-500">
                        <MapPin size={12} />
                        {app.location || '-'}
                      </span>
                    </td>
                    <td className="p-2">
                      <span className="inline-flex items-center gap-1 text-xs text-slate-700 bg-slate-100 rounded-full px-2 py-1">
                        <ClipboardList size={12} />
                        {app.status || '-'}
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
