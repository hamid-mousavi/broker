import React, { useState } from 'react'
import { Outlet, NavLink, useNavigate } from 'react-router-dom'
import { Menu, X, LayoutDashboard, Users, BadgeCheck, BarChart3, FileText, Settings, LogOut } from 'lucide-react'

function Sidebar(){
  const items = [
    {to: '/dashboard/admin', label: 'داشبورد', icon: LayoutDashboard},
    {to: '/dashboard/admin/users', label: 'کاربران', icon: Users},
    {to: '/dashboard/admin/verifications', label: 'تایید مدارک', icon: BadgeCheck},
    {to: '/dashboard/admin/reports', label: 'گزارش‌ها', icon: BarChart3},
    {to: '/dashboard/admin/content', label: 'محتوا', icon: FileText},
    {to: '/dashboard/admin/settings', label: 'تنظیمات', icon: Settings}
  ]
  return (
    <nav className="w-64 bg-white border-l h-full p-4">
      <div className="mb-6 text-lg font-semibold">پنل ادمین</div>
      <ul className="space-y-2">
        {items.map(i=> (
          <li key={i.to}>
            <NavLink to={i.to} className={({isActive})=>`flex items-center gap-2 px-3 py-2 rounded ${isActive? 'accent-btn text-white' : 'text-slate-600 hover:text-slate-900'}`}>
              <i.icon size={16} />
              {i.label}
            </NavLink>
          </li>
        ))}
      </ul>
    </nav>
  )
}

export default function AdminLayout(){
  const [open, setOpen] = useState(false)
  const navigate = useNavigate()
  const userInfo = (() => {
    try {
      return JSON.parse(localStorage.getItem('userInfo') || 'null')
    } catch (err) {
      return null
    }
  })()

  const handleLogout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('userInfo')
    navigate('/login', { replace: true })
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <div className="flex">
        <div className="flex-1 p-4">
          <header className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <button
                aria-label="menu"
                className="p-2 rounded-md bg-white/70 border md:hidden"
                onClick={() => setOpen((prev) => !prev)}
              >
                {open ? <X size={18} /> : <Menu size={18} />}
              </button>
              <h1 className="text-xl font-semibold">داشبورد ادمین</h1>
            </div>
            <div className="flex items-center gap-3 text-sm">
              <span className="text-slate-500">
                {userInfo ? `${userInfo.firstName} ${userInfo.lastName}` : 'کاربر'}
              </span>
              <button
                type="button"
                onClick={handleLogout}
                className="px-3 py-2 rounded border flex items-center gap-2"
              >
                <LogOut size={16} /> خروج
              </button>
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
      {open && (
        <div className="md:hidden fixed inset-0 bg-black/30 z-30" onClick={() => setOpen(false)}>
          <div className="absolute right-0 top-0 h-full w-72 bg-white border-l p-4" onClick={(e) => e.stopPropagation()}>
            <Sidebar />
          </div>
        </div>
      )}
    </div>
  )
}

