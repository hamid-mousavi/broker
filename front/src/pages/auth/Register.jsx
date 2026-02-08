import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { User, Mail, Phone, Lock, UserPlus, Building2, BadgeCheck } from 'lucide-react'
import api from '../../utils/api'

const ROLE_OPTIONS = [
  { value: 2, label: 'صاحب کالا' },
  { value: 1, label: 'ترخیص‌کار' },
  { value: 3, label: 'ادمین' },
]

export default function Register() {
  const navigate = useNavigate()
  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    password: '',
    role: 2,
    isLegalEntity: false,
    nationalId: '',
    registrationNumber: '',
    economicCode: '',
  })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  const getErrorMessage = (err, fallback) => {
    const data = err?.response?.data
    if (data?.errors && Array.isArray(data.errors) && data.errors.length > 0) {
      return data.errors.join(' - ')
    }
    return data?.message || err?.message || fallback || 'خطای نامشخص'
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setLoading(true)
    setError('')
    setSuccess('')
    try {
      const response = await api.post('/auth/register', form)
      const payload = response?.data?.data
      if (!payload?.token) {
        throw new Error(response?.data?.message || 'ثبت‌نام ناموفق بود')
      }
      localStorage.setItem('token', payload.token)
      if (payload.userInfo) {
        localStorage.setItem('userInfo', JSON.stringify(payload.userInfo))
      }
      setSuccess('ثبت‌نام انجام شد. در حال انتقال...')
      setTimeout(() => {
        navigate('/', { replace: true })
      }, 600)
    } catch (err) {
      setError(getErrorMessage(err, 'خطا در ثبت‌نام'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50 text-slate-900 p-4">
      <div className="w-full max-w-xl card p-6">
        <h1 className="text-xl font-semibold mb-4">ثبت‌نام</h1>
        {error && (
          <div className="mb-3 rounded border border-rose-200 bg-rose-50 text-rose-700 px-3 py-2 text-sm">
            {error}
          </div>
        )}
        {success && (
          <div className="mb-3 rounded border border-emerald-200 bg-emerald-50 text-emerald-700 px-3 py-2 text-sm">
            {success}
          </div>
        )}
        <form className="space-y-3" onSubmit={handleSubmit}>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div className="relative">
              <User size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input
                className="px-3 py-2 pr-9 rounded border bg-white w-full"
                placeholder="نام"
                value={form.firstName}
                onChange={(event) =>
                  setForm((prev) => ({
                    ...prev,
                    firstName: event.target.value,
                  }))
                }
                required
              />
            </div>
            <div className="relative">
              <User size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input
                className="px-3 py-2 pr-9 rounded border bg-white w-full"
                placeholder="نام خانوادگی"
                value={form.lastName}
                onChange={(event) =>
                  setForm((prev) => ({
                    ...prev,
                    lastName: event.target.value,
                  }))
                }
                required
              />
            </div>
          </div>
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
            <Phone size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              className="w-full px-3 py-2 pr-9 rounded border bg-white"
              placeholder="شماره موبایل"
              value={form.phoneNumber}
              onChange={(event) =>
                setForm((prev) => ({ ...prev, phoneNumber: event.target.value }))
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
              minLength={6}
              required
            />
          </div>
          <select
            className="w-full px-3 py-2 rounded border bg-white"
            value={form.role}
            onChange={(event) =>
              setForm((prev) => {
                const nextRole = Number(event.target.value)
                return {
                  ...prev,
                  role: nextRole,
                  ...(nextRole !== 1
                    ? {
                        isLegalEntity: false,
                        nationalId: '',
                        registrationNumber: '',
                        economicCode: '',
                      }
                    : {}),
                }
              })
            }
          >
            {ROLE_OPTIONS.map((opt) => (
              <option key={opt.value} value={opt.value}>
                {opt.label}
              </option>
            ))}
          </select>
          {form.role === 1 && (
            <div className="rounded border bg-slate-50 p-4 space-y-3">
              <div className="flex items-center gap-2 text-sm">
                <input
                  id="isLegalEntity"
                  type="checkbox"
                  checked={form.isLegalEntity}
                  onChange={(event) =>
                    setForm((prev) => ({
                      ...prev,
                      isLegalEntity: event.target.checked,
                      nationalId: event.target.checked ? '' : prev.nationalId,
                      registrationNumber: event.target.checked ? prev.registrationNumber : '',
                      economicCode: event.target.checked ? prev.economicCode : '',
                    }))
                  }
                />
                <label htmlFor="isLegalEntity">حقوقی هستم</label>
              </div>

              {!form.isLegalEntity && (
                <div className="relative">
                  <BadgeCheck size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
                  <input
                    className="w-full px-3 py-2 pr-9 rounded border bg-white"
                    placeholder="کد ملی (حقیقی)"
                    value={form.nationalId}
                    onChange={(event) =>
                      setForm((prev) => ({ ...prev, nationalId: event.target.value }))
                    }
                  />
                </div>
              )}

              {form.isLegalEntity && (
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                  <div className="relative">
                    <Building2 size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
                    <input
                      className="px-3 py-2 pr-9 rounded border bg-white w-full"
                      placeholder="شماره ثبت"
                      value={form.registrationNumber}
                      onChange={(event) =>
                        setForm((prev) => ({
                          ...prev,
                          registrationNumber: event.target.value,
                        }))
                      }
                    />
                  </div>
                  <div className="relative">
                    <Building2 size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
                    <input
                      className="px-3 py-2 pr-9 rounded border bg-white w-full"
                      placeholder="کد اقتصادی"
                      value={form.economicCode}
                      onChange={(event) =>
                        setForm((prev) => ({
                          ...prev,
                          economicCode: event.target.value,
                        }))
                      }
                    />
                  </div>
                </div>
              )}
            </div>
          )}
          <button
            type="submit"
            className="w-full px-3 py-2 rounded accent-btn flex items-center justify-center gap-2"
            disabled={loading}
          >
            <UserPlus size={16} /> {loading ? 'در حال ثبت‌نام...' : 'ثبت‌نام'}
          </button>
        </form>
        <div className="mt-4 text-sm text-slate-500">
          حساب دارید؟{' '}
          <Link className="text-sky-600" to="/login">
            ورود
          </Link>
        </div>
      </div>
    </div>
  )
}
