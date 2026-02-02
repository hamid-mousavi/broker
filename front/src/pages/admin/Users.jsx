import React from 'react'

function Filters(){
  return (
    <div className="flex gap-2 items-center">
      <input aria-label="search" placeholder="جستجو بر اساس نام یا ایمیل" className="px-3 py-2 rounded border" />
      <select className="px-2 py-2 rounded border">
        <option>همه نقش‌ها</option>
        <option>ترخیص‌کار</option>
        <option>صاحب کالا</option>
        <option>ادمین</option>
      </select>
    </div>
  )
}

export default function AdminUsers(){
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">مدیریت کاربران</h2>
        <Filters />
      </div>

      <div className="bg-white dark:bg-slate-800 p-4 rounded">
        <table className="w-full text-sm table-auto">
          <thead>
            <tr className="text-slate-500">
              <th className="text-right p-2">نام</th>
              <th className="text-right p-2">ایمیل</th>
              <th className="text-right p-2">نقش</th>
              <th className="text-right p-2">وضعیت</th>
              <th className="text-right p-2">عملیات</th>
            </tr>
          </thead>
          <tbody>
            <tr className="border-t">
              <td className="p-2">علی رضایی</td>
              <td className="p-2">ali@example.com</td>
              <td className="p-2">ترخیص‌کار</td>
              <td className="p-2">فعال</td>
              <td className="p-2">جزئیات | تعلیق</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  )
}
