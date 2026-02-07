import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../utils/api'

export default function RequestModal({ onClose, brokerId, brokerName }) {
  const navigate = useNavigate()
  const userInfo = (() => {
    try {
      return JSON.parse(localStorage.getItem('userInfo') || 'null')
    } catch (err) {
      return null
    }
  })()
  const isLoggedIn = !!localStorage.getItem('token')
  const isOwner = userInfo?.role === 'CargoOwner'

  const [form, setForm] = useState({
    title: '',
    description: '',
    cargoType: '',
    originCountry: '',
    destinationPort: '',
    estimatedValue: '',
    customsCode: '',
    deadline: '',
  })
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  const handleSubmit = async (event) => {
    event.preventDefault()
    if (!isLoggedIn) {
      navigate('/login', { replace: true })
      return
    }
    if (!isOwner) {
      setError('فقط صاحب کالا می‌تواند درخواست ثبت کند')
      return
    }
    setSaving(true)
    setError('')
    try {
      const payload = {
        title: form.title,
        description: form.description,
        cargoType: form.cargoType || null,
        originCountry: form.originCountry || null,
        destinationPort: form.destinationPort || null,
        estimatedValue: form.estimatedValue ? Number(form.estimatedValue) : null,
        customsCode: form.customsCode || null,
        deadline: form.deadline ? new Date(form.deadline).toISOString() : null,
      }

      const res = await api.post('/cargo-owners/requests', payload)
      const request = res?.data?.data
      if (!request?.id) {
        throw new Error(res?.data?.message || 'ثبت درخواست ناموفق بود')
      }

      if (brokerId) {
        await api.post(`/request/${request.id}/assign-agent`, { agentId: brokerId })
      }

      setSuccess('درخواست با موفقیت ثبت شد')
      setTimeout(() => {
        onClose()
        navigate('/dashboard/owner', { replace: true })
      }, 700)
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در ثبت درخواست')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="w-full max-w-2xl rounded bg-white p-6">
        <div className="flex items-center justify-between mb-4">
          <div>
            <div className="text-lg font-semibold">ارسال درخواست</div>
            {brokerName && <div className="text-xs text-slate-500">برای {brokerName}</div>}
          </div>
          <button className="text-slate-500" onClick={onClose}>بستن</button>
        </div>

        {!isLoggedIn && (
          <div className="text-sm text-rose-600 mb-3">
            برای ثبت درخواست باید وارد شوید.
          </div>
        )}
        {isLoggedIn && !isOwner && (
          <div className="text-sm text-rose-600 mb-3">
            فقط نقش صاحب کالا امکان ثبت درخواست دارد.
          </div>
        )}

        {(error || success) && (
          <div
            className={`mb-3 rounded border px-3 py-2 text-sm ${
              error
                ? 'border-rose-200 bg-rose-50 text-rose-700'
                : 'border-emerald-200 bg-emerald-50 text-emerald-700'
            }`}
          >
            {error || success}
          </div>
        )}

        <form className="grid grid-cols-1 md:grid-cols-2 gap-3" onSubmit={handleSubmit}>
          <input
            className="px-3 py-2 rounded border md:col-span-2"
            placeholder="عنوان درخواست"
            value={form.title}
            onChange={(e) => setForm((p) => ({ ...p, title: e.target.value }))}
            required
          />
          <textarea
            className="px-3 py-2 rounded border md:col-span-2"
            placeholder="توضیحات"
            rows={4}
            value={form.description}
            onChange={(e) => setForm((p) => ({ ...p, description: e.target.value }))}
            required
          />
          <input
            className="px-3 py-2 rounded border"
            placeholder="نوع کالا"
            value={form.cargoType}
            onChange={(e) => setForm((p) => ({ ...p, cargoType: e.target.value }))}
          />
          <input
            className="px-3 py-2 rounded border"
            placeholder="کشور مبدا"
            value={form.originCountry}
            onChange={(e) => setForm((p) => ({ ...p, originCountry: e.target.value }))}
          />
          <input
            className="px-3 py-2 rounded border"
            placeholder="بندر مقصد"
            value={form.destinationPort}
            onChange={(e) => setForm((p) => ({ ...p, destinationPort: e.target.value }))}
          />
          <input
            className="px-3 py-2 rounded border"
            placeholder="ارزش تقریبی"
            type="number"
            value={form.estimatedValue}
            onChange={(e) => setForm((p) => ({ ...p, estimatedValue: e.target.value }))}
          />
          <input
            className="px-3 py-2 rounded border"
            placeholder="کد گمرکی"
            value={form.customsCode}
            onChange={(e) => setForm((p) => ({ ...p, customsCode: e.target.value }))}
          />
          <input
            className="px-3 py-2 rounded border"
            type="date"
            value={form.deadline}
            onChange={(e) => setForm((p) => ({ ...p, deadline: e.target.value }))}
          />

          <div className="md:col-span-2 flex items-center justify-end gap-2">
            <button type="button" className="px-3 py-2 rounded border" onClick={onClose}>
              انصراف
            </button>
            <button type="submit" className="px-3 py-2 rounded bg-slate-900 text-white" disabled={saving}>
              {saving ? 'در حال ارسال...' : 'ثبت درخواست'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
