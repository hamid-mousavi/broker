import React, { useEffect, useState } from 'react'
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
      <h2 className="text-lg font-semibold">ترخیص‌کاران ذخیره‌شده</h2>
      {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}
      {!loading && (
        <div className="bg-white border rounded">
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
                  <td className="p-2">{fav.agent?.averageRating ?? '-'}</td>
                  <td className="p-2">{fav.agent?.completedRequests ?? '-'}</td>
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
