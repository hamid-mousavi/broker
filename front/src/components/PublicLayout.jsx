import React, { useMemo, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Menu, X, Search, Star, LogIn, UserPlus, LayoutDashboard, LogOut } from 'lucide-react'

export default function PublicLayout({ children }) {
  const [open, setOpen] = useState(false)
  const navigate = useNavigate()
  const userInfo = useMemo(() => {
    try {
      return JSON.parse(localStorage.getItem('userInfo') || 'null')
    } catch (err) {
      return null
    }
  }, [])
  const isLoggedIn = !!localStorage.getItem('token')
  const dashboardPath =
    userInfo?.role === 'Admin'
      ? '/dashboard/admin'
      : userInfo?.role === 'ClearanceAgent'
      ? '/dashboard/broker'
      : userInfo?.role === 'CargoOwner'
      ? '/dashboard/owner'
      : '/profile/complete'

  const handleLogout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('userInfo')
    navigate('/', { replace: true })
  }

  return (
    <div className="min-h-screen text-slate-900">
      <header className="sticky top-0 z-20 border-b glass">
        <div className="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between">
          <Link to="/" className="font-semibold text-lg tracking-tight">پلتفرم ترخیص کالا</Link>
          <nav className="hidden md:flex items-center gap-6 text-sm text-slate-600">
            <a href="/#features" className="hover:text-slate-900 flex items-center gap-2">
              <Star size={16} /> ویژگی‌ها
            </a>
            <a href="/#brokers" className="hover:text-slate-900 flex items-center gap-2">
              <Search size={16} /> ترخیص‌کاران
            </a>
            <Link to="/brokers" className="hover:text-slate-900 flex items-center gap-2">
              <Search size={16} /> جستجو
            </Link>
          </nav>
          <div className="hidden md:flex items-center gap-2 text-sm">
            {!isLoggedIn && (
              <>
                <Link to="/login" className="px-3 py-2 rounded border border-slate-200 bg-white/70 flex items-center gap-2">
                  <LogIn size={16} /> ورود
                </Link>
                <Link to="/register" className="px-3 py-2 rounded accent-btn flex items-center gap-2">
                  <UserPlus size={16} /> ثبت‌نام
                </Link>
              </>
            )}
            {isLoggedIn && (
              <>
                <Link to={dashboardPath} className="px-3 py-2 rounded border border-slate-200 bg-white/70 flex items-center gap-2">
                  <LayoutDashboard size={16} /> داشبورد
                </Link>
                <button
                  type="button"
                  onClick={handleLogout}
                  className="px-3 py-2 rounded accent-btn flex items-center gap-2"
                >
                  <LogOut size={16} /> خروج
                </button>
              </>
            )}
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
        {open && (
          <div className="md:hidden border-t bg-white/90">
            <div className="max-w-6xl mx-auto px-4 py-3 flex flex-col gap-2 text-sm text-slate-700">
              <a href="/#features" onClick={() => setOpen(false)} className="flex items-center gap-2">
                <Star size={16} /> ویژگی‌ها
              </a>
              <a href="/#brokers" onClick={() => setOpen(false)} className="flex items-center gap-2">
                <Search size={16} /> ترخیص‌کاران
              </a>
              <Link to="/brokers" onClick={() => setOpen(false)} className="flex items-center gap-2">
                <Search size={16} /> جستجو
              </Link>
              <div className="flex gap-2 pt-2">
                {!isLoggedIn && (
                  <>
                    <Link to="/login" onClick={() => setOpen(false)} className="px-3 py-2 rounded border border-slate-200 bg-white/70 flex items-center gap-2">
                      <LogIn size={16} /> ورود
                    </Link>
                    <Link to="/register" onClick={() => setOpen(false)} className="px-3 py-2 rounded accent-btn flex items-center gap-2">
                      <UserPlus size={16} /> ثبت‌نام
                    </Link>
                  </>
                )}
                {isLoggedIn && (
                  <>
                    <Link to={dashboardPath} onClick={() => setOpen(false)} className="px-3 py-2 rounded border border-slate-200 bg-white/70 flex items-center gap-2">
                      <LayoutDashboard size={16} /> داشبورد
                    </Link>
                    <button
                      type="button"
                      onClick={() => {
                        setOpen(false)
                        handleLogout()
                      }}
                      className="px-3 py-2 rounded accent-btn flex items-center gap-2"
                    >
                      <LogOut size={16} /> خروج
                    </button>
                  </>
                )}
              </div>
            </div>
          </div>
        )}
      </header>

      <main className="page-enter">{children}</main>

      <footer className="border-t bg-white/70 mt-10">
        <div className="max-w-6xl mx-auto px-4 py-6 text-sm text-slate-500 flex flex-col md:flex-row gap-3 md:items-center md:justify-between">
          <div>© 2026 پلتفرم ترخیص کالا</div>
          <div className="flex gap-4">
            <a href="/#features">ویژگی‌ها</a>
            <a href="/#brokers">ترخیص‌کاران</a>
            <Link to="/brokers">جستجو</Link>
          </div>
        </div>
      </footer>
    </div>
  )
}
