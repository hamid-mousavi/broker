import React from 'react'
import { NavLink, Outlet, useNavigate } from 'react-router-dom'

export default function UserLayout() {
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
          { to: '/dashboard/broker', label: 'داشبورد' },
          { to: '/dashboard/broker/requests', label: 'درخواست‌ها' },
          { to: '/dashboard/broker/appointments', label: 'قرار ملاقات‌ها' },
          { to: '/profile/complete', label: 'تکمیل پروفایل' },
        ]
      : [
          { to: '/dashboard/owner', label: 'داشبورد' },
          { to: '/dashboard/owner/requests', label: 'درخواست‌ها' },
          { to: '/dashboard/owner/favorites', label: 'ذخیره‌شده‌ها' },
          { to: '/dashboard/owner/notifications', label: 'اعلان‌ها' },
          { to: '/profile/complete', label: 'تکمیل پروفایل' },
        ]

  const handleLogout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('userInfo')
    navigate('/login', { replace: true })
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <div className="flex">
        <aside className="hidden md:block w-64 bg-white border-l p-4">
          <div className="mb-6 text-lg font-semibold">پنل کاربری</div>
          <ul className="space-y-2">
            {menu.map((item) => (
              <li key={item.to}>
                <NavLink
                  to={item.to}
                  className={({ isActive }) =>
                    `block px-3 py-2 rounded ${isActive ? 'bg-slate-900 text-white' : ''}`
                  }
                >
                  {item.label}
                </NavLink>
              </li>
            ))}
          </ul>
        </aside>
        <div className="flex-1 p-4">
          <header className="flex items-center justify-between mb-6">
            <div className="text-sm text-slate-500">
              {userInfo ? `${userInfo.firstName} ${userInfo.lastName}` : 'کاربر'}
            </div>
            <button className="px-3 py-2 rounded border" onClick={handleLogout}>
              خروج
            </button>
          </header>
          <main>
            <Outlet />
          </main>
        </div>
      </div>
    </div>
  )
}
