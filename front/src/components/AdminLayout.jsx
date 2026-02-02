import React from 'react'
import { Outlet, NavLink } from 'react-router-dom'
import { Menu } from 'lucide-react'

function Sidebar(){
  const items = [
    {to: '/dashboard/admin', label: 'داشبورد'},
    {to: '/dashboard/admin/users', label: 'کاربران'},
    {to: '/dashboard/admin/verifications', label: 'تایید مدارک'},
    {to: '/dashboard/admin/reports', label: 'گزارش‌ها'},
    {to: '/dashboard/admin/content', label: 'محتوا'},
    {to: '/dashboard/admin/settings', label: 'تنظیمات'}
  ]
  return (
    <nav className="w-64 bg-white dark:bg-slate-800 border-l h-full p-4">
      <div className="mb-6 text-lg font-semibold">پنل ادمین</div>
      <ul className="space-y-2">
        {items.map(i=> (
          <li key={i.to}>
            <NavLink to={i.to} className={({isActive})=>`block px-3 py-2 rounded ${isActive? 'bg-sky-100 dark:bg-sky-900 font-medium':''}`}>
              {i.label}
            </NavLink>
          </li>
        ))}
      </ul>
    </nav>
  )
}

export default function AdminLayout(){
  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-900 text-slate-900 dark:text-slate-100">
      <div className="flex">
        <div className="flex-1 p-4">
          <header className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <button aria-label="menu" className="p-2 rounded-md bg-white/50 dark:bg-slate-700"><Menu /></button>
              <h1 className="text-xl font-semibold">داشبورد ادمین</h1>
            </div>
          </header>
          <main>
            <Outlet />
          </main>
        </div>
        <aside className="hidden md:block">
          <Sidebar />
        </aside>
      </div>
    </div>
  )
}
