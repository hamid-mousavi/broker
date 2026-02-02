import React from 'react'

export default function AdminVerifications(){
  return (
    <div className="space-y-4">
      <h2 className="text-lg font-semibold">تایید مدارک</h2>
      <div className="bg-white dark:bg-slate-800 p-4 rounded">
        <ul className="space-y-3">
          <li className="p-3 border rounded flex items-center justify-between">
            <div>
              <div className="font-medium">علی رضایی</div>
              <div className="text-sm text-slate-500">گواهی شرکت - بارگذاری شده: 2026-02-01</div>
            </div>
            <div className="flex gap-2">
              <button className="px-3 py-1 rounded bg-emerald-600 text-white">تایید</button>
              <button className="px-3 py-1 rounded bg-rose-600 text-white">رد</button>
            </div>
          </li>
        </ul>
      </div>
    </div>
  )
}
