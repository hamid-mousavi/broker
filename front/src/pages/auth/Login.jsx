import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
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

      if (payload.userInfo?.role === 'Admin') {
        navigate('/dashboard/admin', { replace: true })
      } else if (payload.userInfo?.role === 'ClearanceAgent') {
        navigate('/dashboard/broker', { replace: true })
      } else {
        navigate('/dashboard/owner', { replace: true })
      }
    } catch (err) {
      setError(err?.response?.data?.message || err?.message || 'خطا در ورود')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50 dark:bg-slate-900 text-slate-900 dark:text-slate-100 p-4">
      <div className="w-full max-w-md bg-white dark:bg-slate-800 rounded p-6 shadow">
        <h1 className="text-xl font-semibold mb-4">ورود ادمین</h1>
        {error && (
          <div className="mb-3 rounded border border-rose-200 bg-rose-50 text-rose-700 px-3 py-2 text-sm">
            {error}
          </div>
        )}
        <form className="space-y-3" onSubmit={handleSubmit}>
          <input
            className="w-full px-3 py-2 rounded border bg-white dark:bg-slate-900"
            placeholder="ایمیل"
            type="email"
            value={form.email}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, email: event.target.value }))
            }
            required
          />
          <input
            className="w-full px-3 py-2 rounded border bg-white dark:bg-slate-900"
            placeholder="رمز عبور"
            type="password"
            value={form.password}
            onChange={(event) =>
              setForm((prev) => ({ ...prev, password: event.target.value }))
            }
            required
          />
          <button
            type="submit"
            className="w-full px-3 py-2 rounded bg-slate-900 text-white dark:bg-slate-200 dark:text-slate-900"
            disabled={loading}
          >
            {loading ? 'در حال ورود...' : 'ورود'}
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
