import React, { useEffect, useState } from 'react'
import { Heart, Star, ClipboardList } from 'lucide-react'
import api from '../../utils/api'

export default function OwnerFavorites() {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const load = async () => {
      try {
        const res = await api.get('/cargo-owners/favorites')
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
        <Heart size={18} /> ترخیص‌کاران ذخیره‌شده
      </h2>
      {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}
      {!loading && (
        <>
          <div className="space-y-3 md:hidden">
            {items.map((fav) => (
              <div key={fav.id} className="card p-4">
                <div className="font-semibold">{fav.agent?.companyName || '-'}</div>
                <div className="mt-2 flex items-center gap-2 text-xs text-amber-600">
                  <Star size={12} /> {fav.agent?.averageRating ?? '-'}
                </div>
                <div className="mt-2 flex items-center gap-2 text-xs text-slate-600">
                  <ClipboardList size={12} /> {fav.agent?.completedRequests ?? '-'}
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
                  <th className="text-right p-2">نام</th>
                  <th className="text-right p-2">امتیاز</th>
                  <th className="text-right p-2">درخواست‌ها</th>
                </tr>
              </thead>
              <tbody>
                {items.map((fav) => (
                  <tr key={fav.id} className="border-t">
                    <td className="p-2">{fav.agent?.companyName || '-'}</td>
                    <td className="p-2">
                      <span className="inline-flex items-center gap-1 text-xs text-amber-600">
                        <Star size={12} /> {fav.agent?.averageRating ?? '-'}
                      </span>
                    </td>
                    <td className="p-2">
                      <span className="inline-flex items-center gap-1 text-xs text-slate-600">
                        <ClipboardList size={12} /> {fav.agent?.completedRequests ?? '-'}
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
