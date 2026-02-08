import React from 'react'
import StatsCard from '../../components/StatsCard'
import { Line } from 'react-chartjs-2'
import { Users, UserPlus, ClipboardList, Wallet } from 'lucide-react'

export default function AdminDashboard(){
  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatsCard title="کاربران فعال" value="1,234" icon={Users} />
        <StatsCard title="کاربران جدید امروز" value="24" icon={UserPlus} />
        <StatsCard title="درخواست‌های در انتظار" value="12" icon={ClipboardList} />
        <StatsCard title="تراکنش‌های اخیر" value="₺ 8,430" icon={Wallet} />
      </div>

      <section className="card p-4">
        <h2 className="text-lg font-semibold mb-3">نمودار رشد سیستم</h2>
        <div className="h-56 flex items-center justify-center text-slate-400">نمودار نمونه (قابلیت اتصال به داده‌ها بعدی)</div>
      </section>
    </div>
  )
}
