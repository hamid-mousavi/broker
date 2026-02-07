import React, { useEffect, useState } from 'react'
import api from '../utils/api'

const emptyBroker = {
  companyName: '',
  licenseNumber: '',
  city: '',
  province: '',
  address: '',
  website: '',
  yearsOfExperience: 0,
  specializations: '',
  isLegalEntity: false,
  nationalId: '',
  registrationNumber: '',
  economicCode: '',
}

const emptyOwner = {
  companyName: '',
  nationalId: '',
  economicCode: '',
  city: '',
  province: '',
  address: '',
}

export default function ProfileComplete() {
  const [role, setRole] = useState(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [brokerForm, setBrokerForm] = useState(emptyBroker)
  const [ownerForm, setOwnerForm] = useState(emptyOwner)

  useEffect(() => {
    const userInfo = (() => {
      try {
        return JSON.parse(localStorage.getItem('userInfo') || 'null')
      } catch (err) {
        return null
      }
    })()
    setRole(userInfo?.role || null)
  }, [])

  useEffect(() => {
    const load = async () => {
      if (!role) return
      setLoading(true)
      setError('')
      try {
        if (role === 'ClearanceAgent') {
          const res = await api.get('/brokers/profile')
          const data = res?.data?.data
          if (data) {
            setBrokerForm({
              companyName: data.companyName || '',
              licenseNumber: data.licenseNumber || '',
              city: data.city || '',
              province: data.province || '',
              address: data.address || '',
              website: data.website || '',
              yearsOfExperience: data.yearsOfExperience || 0,
              specializations: (data.specializations || []).join(', '),
              isLegalEntity: data.isLegalEntity || false,
              nationalId: data.nationalId || '',
              registrationNumber: data.registrationNumber || '',
              economicCode: data.economicCode || '',
            })
          }
        } else if (role === 'CargoOwner') {
          const res = await api.get('/cargo-owners/profile')
          const data = res?.data?.data
          if (data) {
            setOwnerForm({
              companyName: data.companyName || '',
              nationalId: data.nationalId || '',
              economicCode: data.economicCode || '',
              city: data.city || '',
              province: data.province || '',
              address: data.address || '',
            })
          }
        }
      } catch (err) {
        setError(err?.response?.data?.message || 'خطا در دریافت پروفایل')
      } finally {
        setLoading(false)
      }
    }

    load()
  }, [role])

  const handleBrokerSubmit = async (event) => {
    event.preventDefault()
    setSaving(true)
    setError('')
    setSuccess('')
    try {
      const payload = {
        companyName: brokerForm.companyName,
        licenseNumber: brokerForm.licenseNumber || null,
        city: brokerForm.city || null,
        province: brokerForm.province || null,
        address: brokerForm.address || null,
        website: brokerForm.website || null,
        yearsOfExperience: Number(brokerForm.yearsOfExperience || 0),
        isLegalEntity: brokerForm.isLegalEntity,
        nationalId: brokerForm.isLegalEntity ? null : brokerForm.nationalId || null,
        registrationNumber: brokerForm.isLegalEntity ? brokerForm.registrationNumber || null : null,
        economicCode: brokerForm.isLegalEntity ? brokerForm.economicCode || null : null,
        specializations: brokerForm.specializations
          ? brokerForm.specializations.split(',').map((s) => s.trim()).filter(Boolean)
          : [],
      }
      const res = await api.put('/brokers/profile', payload)
      if (!res?.data?.success) {
        throw new Error(res?.data?.message || 'ذخیره ناموفق بود')
      }
      setSuccess('پروفایل ترخیص‌کار با موفقیت ذخیره شد')
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در ذخیره پروفایل')
    } finally {
      setSaving(false)
    }
  }

  const handleOwnerSubmit = async (event) => {
    event.preventDefault()
    setSaving(true)
    setError('')
    setSuccess('')
    try {
      const payload = {
        companyName: ownerForm.companyName || null,
        nationalId: ownerForm.nationalId || null,
        economicCode: ownerForm.economicCode || null,
        city: ownerForm.city || null,
        province: ownerForm.province || null,
        address: ownerForm.address || null,
      }
      const res = await api.put('/cargo-owners/profile', payload)
      if (!res?.data?.success) {
        throw new Error(res?.data?.message || 'ذخیره ناموفق بود')
      }
      setSuccess('پروفایل صاحب کالا با موفقیت ذخیره شد')
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در ذخیره پروفایل')
    } finally {
      setSaving(false)
    }
  }

  if (!role) {
    return (
      <div className="min-h-screen flex items-center justify-center text-slate-500">
        لطفا ابتدا وارد شوید.
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900 p-4">
      <div className="max-w-3xl mx-auto bg-white rounded-xl border p-6">
        <h1 className="text-xl font-semibold mb-4">تکمیل پروفایل</h1>

        {(error || success) && (
          <div
            className={`mb-4 rounded border px-3 py-2 text-sm ${
              error
                ? 'border-rose-200 bg-rose-50 text-rose-700'
                : 'border-emerald-200 bg-emerald-50 text-emerald-700'
            }`}
          >
            {error || success}
          </div>
        )}

        {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}

        {!loading && role === 'ClearanceAgent' && (
          <form className="space-y-3" onSubmit={handleBrokerSubmit}>
            <input
              className="w-full px-3 py-2 rounded border"
              placeholder="نام شرکت"
              value={brokerForm.companyName}
              onChange={(event) =>
                setBrokerForm((prev) => ({ ...prev, companyName: event.target.value }))
              }
              required
            />
            <div className="flex items-center gap-2 text-sm">
              <input
                id="brokerLegalEntity"
                type="checkbox"
                checked={brokerForm.isLegalEntity}
                onChange={(event) =>
                  setBrokerForm((prev) => ({
                    ...prev,
                    isLegalEntity: event.target.checked,
                    nationalId: event.target.checked ? '' : prev.nationalId,
                    registrationNumber: event.target.checked ? prev.registrationNumber : '',
                    economicCode: event.target.checked ? prev.economicCode : '',
                  }))
                }
              />
              <label htmlFor="brokerLegalEntity">حقوقی هستم</label>
            </div>
            {!brokerForm.isLegalEntity && (
              <input
                className="w-full px-3 py-2 rounded border"
                placeholder="کد ملی (حقیقی)"
                value={brokerForm.nationalId}
                onChange={(event) =>
                  setBrokerForm((prev) => ({ ...prev, nationalId: event.target.value }))
                }
              />
            )}
            {brokerForm.isLegalEntity && (
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                <input
                  className="px-3 py-2 rounded border"
                  placeholder="شماره ثبت"
                  value={brokerForm.registrationNumber}
                  onChange={(event) =>
                    setBrokerForm((prev) => ({ ...prev, registrationNumber: event.target.value }))
                  }
                />
                <input
                  className="px-3 py-2 rounded border"
                  placeholder="کد اقتصادی"
                  value={brokerForm.economicCode}
                  onChange={(event) =>
                    setBrokerForm((prev) => ({ ...prev, economicCode: event.target.value }))
                  }
                />
              </div>
            )}
            <input
              className="w-full px-3 py-2 rounded border"
              placeholder="شماره مجوز"
              value={brokerForm.licenseNumber}
              onChange={(event) =>
                setBrokerForm((prev) => ({ ...prev, licenseNumber: event.target.value }))
              }
            />
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <input
                className="px-3 py-2 rounded border"
                placeholder="استان"
                value={brokerForm.province}
                onChange={(event) =>
                  setBrokerForm((prev) => ({ ...prev, province: event.target.value }))
                }
              />
              <input
                className="px-3 py-2 rounded border"
                placeholder="شهر"
                value={brokerForm.city}
                onChange={(event) =>
                  setBrokerForm((prev) => ({ ...prev, city: event.target.value }))
                }
              />
            </div>
            <input
              className="w-full px-3 py-2 rounded border"
              placeholder="آدرس"
              value={brokerForm.address}
              onChange={(event) =>
                setBrokerForm((prev) => ({ ...prev, address: event.target.value }))
              }
            />
            <input
              className="w-full px-3 py-2 rounded border"
              placeholder="وب‌سایت"
              value={brokerForm.website}
              onChange={(event) =>
                setBrokerForm((prev) => ({ ...prev, website: event.target.value }))
              }
            />
            <input
              className="w-full px-3 py-2 rounded border"
              type="number"
              min="0"
              placeholder="سال‌های تجربه"
              value={brokerForm.yearsOfExperience}
              onChange={(event) =>
                setBrokerForm((prev) => ({
                  ...prev,
                  yearsOfExperience: event.target.value,
                }))
              }
            />
            <input
              className="w-full px-3 py-2 rounded border"
              placeholder="تخصص‌ها (با کاما جدا کنید)"
              value={brokerForm.specializations}
              onChange={(event) =>
                setBrokerForm((prev) => ({ ...prev, specializations: event.target.value }))
              }
            />
            <button
              type="submit"
              className="px-4 py-2 rounded bg-slate-900 text-white"
              disabled={saving}
            >
              {saving ? 'در حال ذخیره...' : 'ذخیره پروفایل'}
            </button>
          </form>
        )}

        {!loading && role === 'CargoOwner' && (
          <form className="space-y-3" onSubmit={handleOwnerSubmit}>
            <input
              className="w-full px-3 py-2 rounded border"
              placeholder="نام شرکت"
              value={ownerForm.companyName}
              onChange={(event) =>
                setOwnerForm((prev) => ({ ...prev, companyName: event.target.value }))
              }
            />
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <input
                className="px-3 py-2 rounded border"
                placeholder="کد ملی/شناسه"
                value={ownerForm.nationalId}
                onChange={(event) =>
                  setOwnerForm((prev) => ({ ...prev, nationalId: event.target.value }))
                }
              />
              <input
                className="px-3 py-2 rounded border"
                placeholder="کد اقتصادی"
                value={ownerForm.economicCode}
                onChange={(event) =>
                  setOwnerForm((prev) => ({ ...prev, economicCode: event.target.value }))
                }
              />
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <input
                className="px-3 py-2 rounded border"
                placeholder="استان"
                value={ownerForm.province}
                onChange={(event) =>
                  setOwnerForm((prev) => ({ ...prev, province: event.target.value }))
                }
              />
              <input
                className="px-3 py-2 rounded border"
                placeholder="شهر"
                value={ownerForm.city}
                onChange={(event) =>
                  setOwnerForm((prev) => ({ ...prev, city: event.target.value }))
                }
              />
            </div>
            <input
              className="w-full px-3 py-2 rounded border"
              placeholder="آدرس"
              value={ownerForm.address}
              onChange={(event) =>
                setOwnerForm((prev) => ({ ...prev, address: event.target.value }))
              }
            />
            <button
              type="submit"
              className="px-4 py-2 rounded bg-slate-900 text-white"
              disabled={saving}
            >
              {saving ? 'در حال ذخیره...' : 'ذخیره پروفایل'}
            </button>
          </form>
        )}

        <div className="mt-6 text-sm text-slate-500 flex flex-wrap gap-2">
          <span>بعد از تکمیل پروفایل می‌توانید وارد داشبورد شوید.</span>
          {role === 'ClearanceAgent' && (
            <a className="text-slate-900 underline" href="/dashboard/broker">ورود به داشبورد ترخیص‌کار</a>
          )}
          {role === 'CargoOwner' && (
            <a className="text-slate-900 underline" href="/dashboard/owner">ورود به داشبورد صاحب کالا</a>
          )}
        </div>
      </div>
    </div>
  )
}
