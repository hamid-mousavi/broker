import React from 'react'
import StatsCard from '../../components/StatsCard'
import { Line } from 'react-chartjs-2'

export default function AdminDashboard(){
  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatsCard title="کاربران فعال" value="1,234" />
        <StatsCard title="کاربران جدید امروز" value="24" />
        <StatsCard title="درخواست‌های در انتظار" value="12" />
        <StatsCard title="تراکنش‌های اخیر" value="₫ 8,430" />
      </div>

      <section className="bg-white dark:bg-slate-800 p-4 rounded">
        <h2 className="text-lg font-semibold mb-3">نمودار رشد سیستم</h2>
        <div className="h-56 flex items-center justify-center text-slate-400">نمودار نمونه (قابلیت اتصال به داده‌ها بعدی)</div>
      </section>
    </div>
  )
}
