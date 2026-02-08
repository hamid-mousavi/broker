import React, { useState } from 'react'
import { NavLink, Outlet, Link, useNavigate } from 'react-router-dom'
import { Menu, X, LayoutDashboard, ClipboardList, Calendar, UserCircle, Heart, Bell, LogOut } from 'lucide-react'

export default function UserLayout() {
  const [open, setOpen] = useState(false)
  const navigate = useNavigate()
  const userInfo = (() => {
    try {
      return JSON.parse(localStorage.getItem('userInfo') || 'null')
    } catch (err) {
      return null
    }
  })()

  const role = userInfo?.role

  const menu =
    role === 'ClearanceAgent'
      ? [
          { to: '/dashboard/broker', label: 'داشبورد', icon: LayoutDashboard },
          { to: '/dashboard/broker/requests', label: 'درخواست‌ها', icon: ClipboardList },
          { to: '/dashboard/broker/appointments', label: 'قرار ملاقات‌ها', icon: Calendar },
          { to: '/profile/complete', label: 'تکمیل پروفایل', icon: UserCircle },
        ]
      : [
          { to: '/dashboard/owner', label: 'داشبورد', icon: LayoutDashboard },
          { to: '/dashboard/owner/requests', label: 'درخواست‌ها', icon: ClipboardList },
          { to: '/dashboard/owner/favorites', label: 'ذخیره‌شده‌ها', icon: Heart },
          { to: '/dashboard/owner/notifications', label: 'اعلان‌ها', icon: Bell },
          { to: '/profile/complete', label: 'تکمیل پروفایل', icon: UserCircle },
        ]

  const handleLogout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('userInfo')
    navigate('/login', { replace: true })
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <header className="sticky top-0 z-20 border-b glass">
        <div className="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between">
          <Link to="/" className="font-semibold text-lg tracking-tight">پلتفرم ترخیص کالا</Link>
          <nav className="hidden md:flex items-center gap-6 text-sm text-slate-600">
            <Link to="/" className="hover:text-slate-900">خانه</Link>
            <Link to="/brokers" className="hover:text-slate-900">جستجو</Link>
            <Link to="/profile/complete" className="hover:text-slate-900">پروفایل</Link>
          </nav>
          <div className="hidden md:flex items-center gap-2 text-sm">
            <button className="px-3 py-2 rounded border" onClick={handleLogout}>
              <LogOut size={16} /> خروج
            </button>
          </div>
          <button
            type="button"
            className="md:hidden p-2 rounded border bg-white/70"
            onClick={() => setOpen((prev) => !prev)}
            aria-label="menu"
          >
            {open ? <X size={18} /> : <Menu size={18} />}
          </button>
        </div>
      </header>
      <div className="flex">
        <aside className="hidden md:block w-64 bg-white border-l p-4">
          <div className="mb-6 text-lg font-semibold">پنل کاربری</div>
          <ul className="space-y-2">
            {menu.map((item) => (
              <li key={item.to}>
                <NavLink
                  to={item.to}
                  className={({ isActive }) =>
                    `flex items-center gap-2 px-3 py-2 rounded ${isActive ? 'accent-btn text-white' : 'text-slate-600 hover:text-slate-900'}`
                  }
                >
                  <item.icon size={16} />
                  {item.label}
                </NavLink>
              </li>
            ))}
          </ul>
        </aside>
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
              <div className="text-sm text-slate-500">
                {userInfo ? `${userInfo.firstName} ${userInfo.lastName}` : 'کاربر'}
              </div>
            </div>
            <button className="px-3 py-2 rounded border flex items-center gap-2" onClick={handleLogout}>
              <LogOut size={16} /> خروج
            </button>
          </header>
          <main>
            <Outlet />
          </main>
        </div>
      </div>
      {open && (
        <div className="md:hidden fixed inset-0 bg-black/30 z-30" onClick={() => setOpen(false)}>
          <div className="absolute right-0 top-0 h-full w-72 bg-white border-l p-4" onClick={(e) => e.stopPropagation()}>
            <div className="mb-6 text-lg font-semibold">پنل کاربری</div>
            <ul className="space-y-2">
              {menu.map((item) => (
                <li key={item.to}>
                  <NavLink
                    to={item.to}
                    onClick={() => setOpen(false)}
                    className={({ isActive }) =>
                      `flex items-center gap-2 px-3 py-2 rounded ${isActive ? 'accent-btn text-white' : 'text-slate-600 hover:text-slate-900'}`
                    }
                  >
                    <item.icon size={16} />
                    {item.label}
                  </NavLink>
                </li>
              ))}
            </ul>
          </div>
        </div>
      )}
    </div>
  )
}
