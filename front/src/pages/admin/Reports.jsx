import React from 'react'

export default function AdminReports(){
  return (
    <div className="space-y-4">
      <h2 className="text-lg font-semibold">گزارش‌گیری</h2>
      <div className="bg-white dark:bg-slate-800 p-4 rounded">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="p-4 border rounded">گزارش کاربران (CSV/PDF)</div>
          <div className="p-4 border rounded">گزارش درخواست‌ها</div>
          <div className="p-4 border rounded">گزارش مالی</div>
          <div className="p-4 border rounded">گزارش فعالیت‌ها</div>
        </div>
      </div>
    </div>
  )
}
