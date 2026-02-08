import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Mail, Lock, LogIn } from 'lucide-react'
import api from '../../utils/api'

export default function Login() {
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '' })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (event) => {
    event.preventDefault()
    setLoading(true)
    setError('')
    try {
      const response = await api.post('/auth/login', form)
      const payload = response?.data?.data
      if (!payload?.token) {
        throw new Error(response?.data?.message || 'ورود ناموفق بود')
      }

      localStorage.setItem('token', payload.token)
      if (payload.userInfo) {
        localStorage.setItem('userInfo', JSON.stringify(payload.userInfo))
      }

      navigate('/', { replace: true })
    } catch (err) {
      setError(err?.response?.data?.message || err?.message || 'خطا در ورود')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50 text-slate-900 p-4">
      <div className="w-full max-w-md card p-6">
        <h1 className="text-xl font-semibold mb-4">ورود</h1>
        {error && (
          <div className="mb-3 rounded border border-rose-200 bg-rose-50 text-rose-700 px-3 py-2 text-sm">
            {error}
          </div>
        )}
        <form className="space-y-3" onSubmit={handleSubmit}>
          <div className="relative">
            <Mail size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              className="w-full px-3 py-2 pr-9 rounded border bg-white"
              placeholder="ایمیل"
              type="email"
              value={form.email}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, email: event.target.value }))
              }
              required
            />
          </div>
          <div className="relative">
            <Lock size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              className="w-full px-3 py-2 pr-9 rounded border bg-white"
              placeholder="رمز عبور"
              type="password"
              value={form.password}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, password: event.target.value }))
              }
              required
            />
          </div>
          <button
            type="submit"
            className="w-full px-3 py-2 rounded accent-btn flex items-center justify-center gap-2"
            disabled={loading}
          >
            <LogIn size={16} /> {loading ? 'در حال ورود...' : 'ورود'}
          </button>
        </form>
        <div className="mt-4 text-sm text-slate-500">
          حساب ندارید؟{' '}
          <Link className="text-sky-600" to="/register">
            ثبت‌نام
          </Link>
        </div>
      </div>
    </div>
  )
}
