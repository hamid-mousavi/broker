import React from 'react'
import { Link } from 'react-router-dom'

export default function PublicLayout({ children }) {
  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <header className="sticky top-0 z-20 bg-white/80 backdrop-blur border-b">
        <div className="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between">
          <Link to="/" className="font-semibold text-lg">پلتفرم ترخیص کالا</Link>
          <nav className="hidden md:flex items-center gap-6 text-sm text-slate-600">
            <Link to="/#features" className="hover:text-slate-900">ویژگی‌ها</Link>
            <Link to="/#brokers" className="hover:text-slate-900">ترخیص‌کاران</Link>
            <Link to="/brokers" className="hover:text-slate-900">جستجو</Link>
          </nav>
          <div className="flex items-center gap-2 text-sm">
            <Link to="/login" className="px-3 py-2 rounded border">ورود</Link>
            <Link to="/register" className="px-3 py-2 rounded bg-slate-900 text-white">ثبت‌نام</Link>
          </div>
        </div>
      </header>

      <main>{children}</main>

      <footer className="border-t bg-white mt-10">
        <div className="max-w-6xl mx-auto px-4 py-6 text-sm text-slate-500 flex flex-col md:flex-row gap-3 md:items-center md:justify-between">
          <div>© 2026 پلتفرم ترخیص کالا</div>
          <div className="flex gap-4">
            <Link to="/#features">ویژگی‌ها</Link>
            <Link to="/#brokers">ترخیص‌کاران</Link>
            <Link to="/brokers">جستجو</Link>
          </div>
        </div>
      </footer>
    </div>
  )
}
