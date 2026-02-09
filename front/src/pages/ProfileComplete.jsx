import React, { useEffect, useMemo, useState } from 'react'
import api from '../utils/api'

const PROVINCES = [
  'تهران',
  'البرز',
  'اصفهان',
  'خراسان رضوی',
  'فارس',
  'آذربایجان شرقی',
  'خوزستان',
  'گیلان',
  'مازندران',
  'کرمان',
  'هرمزگان',
]

const CITIES_BY_PROVINCE = {
  تهران: ['تهران', 'ری', 'شمیرانات', 'اسلام‌شهر'],
  البرز: ['کرج', 'فردیس', 'نظرآباد'],
  اصفهان: ['اصفهان', 'کاشان', 'خمینی‌شهر'],
  'خراسان رضوی': ['مشهد', 'نیشابور', 'سبزوار'],
  فارس: ['شیراز', 'مرودشت', 'کازرون'],
  'آذربایجان شرقی': ['تبریز', 'مراغه', 'مرند'],
  خوزستان: ['اهواز', 'آبادان', 'خرمشهر'],
  گیلان: ['رشت', 'انزلی', 'لاهیجان'],
  مازندران: ['ساری', 'بابل', 'آمل'],
  کرمان: ['کرمان', 'رفسنجان', 'سیرجان'],
  هرمزگان: ['بندرعباس', 'قشم', 'کیش'],
}

const emptyBroker = {
  companyName: '',
  licenseNumber: '',
  city: '',
  province: '',
  address: '',
  personalAddress: '',
  legalAddress: '',
  postalCode: '',
  website: '',
  yearsOfExperience: 0,
  specializations: [],
  otherSpecialization: '',
  description: '',
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
  const [userForm, setUserForm] = useState({ phoneNumber: '' })
  const [profileImageUrl, setProfileImageUrl] = useState('')
  const [imageUploading, setImageUploading] = useState(false)
  const [documents, setDocuments] = useState([])
  const [documentTypes, setDocumentTypes] = useState([])
  const [docUpload, setDocUpload] = useState({ type: '', description: '', file: null })
  const [docUploading, setDocUploading] = useState(false)

  const resolveMediaUrl = (path) => {
    if (!path) return ''
    if (path.startsWith('http')) return path
    const baseUrl = api.defaults.baseURL || ''
    return `${baseUrl.replace('/api', '')}${path}`
  }

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
        const profileRes = await api.get('/auth/profile')
        const profile = profileRes?.data?.data
        if (profile) {
          setUserForm({
            phoneNumber: profile.phoneNumber || '',
          })
        }

        if (role === 'ClearanceAgent') {
          const [agentRes, docsRes, typesRes] = await Promise.all([
            api.get('/brokers/profile'),
            api.get('/documents'),
            api.get('/documents/types'),
          ])

          const data = agentRes?.data?.data
          if (data) {
            const specs = data.specializations || []
            const otherSpec = specs.find((s) => s.startsWith('سایر:'))
            const normalizedSpecs = Array.from(
              new Set(specs.map((s) => (s.startsWith('سایر:') ? 'سایر' : s)))
            )
            setBrokerForm({
              companyName: data.companyName || '',
              licenseNumber: data.licenseNumber || '',
              city: data.city || '',
              province: data.province || '',
              address: data.address || '',
              personalAddress: data.personalAddress || '',
              legalAddress: data.legalAddress || '',
              postalCode: data.postalCode || '',
              website: data.website || '',
              yearsOfExperience: data.yearsOfExperience || 0,
              specializations: normalizedSpecs,
              otherSpecialization: otherSpec ? otherSpec.replace('سایر:', '').trim() : '',
              description: data.description || '',
              isLegalEntity: data.isLegalEntity || false,
              nationalId: data.nationalId || '',
              registrationNumber: data.registrationNumber || '',
              economicCode: data.economicCode || '',
            })
          }

          setDocuments(docsRes?.data?.data || [])
          setDocumentTypes(typesRes?.data?.data || [])
        }

        if (role === 'CargoOwner') {
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

  const cities = useMemo(() => {
    if (!brokerForm.province) return []
    return CITIES_BY_PROVINCE[brokerForm.province] || []
  }, [brokerForm.province])

  const ownerCities = useMemo(() => {
    if (!ownerForm.province) return []
    return CITIES_BY_PROVINCE[ownerForm.province] || []
  }, [ownerForm.province])

  const handleProfileImageUpload = async (event) => {
    const file = event.target.files?.[0]
    if (!file) return
    setImageUploading(true)
    setError('')
    try {
      const formData = new FormData()
      formData.append('file', file)
      const res = await api.post('/auth/upload-profile-image', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
      const path = res?.data?.data || ''
      if (path) {
        setProfileImageUrl(path)
      }
      setSuccess('عکس پروفایل با موفقیت آپلود شد')
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در آپلود عکس پروفایل')
    } finally {
      setImageUploading(false)
    }
  }

  const handleDocumentUpload = async (event) => {
    event.preventDefault()
    if (!docUpload.file || !docUpload.type) {
      setError('نوع مدرک و فایل الزامی است')
      return
    }
    setDocUploading(true)
    setError('')
    setSuccess('')
    try {
      const formData = new FormData()
      formData.append('file', docUpload.file)
      formData.append('documentType', docUpload.type)
      if (docUpload.description) {
        formData.append('description', docUpload.description)
      }
      const res = await api.post('/documents/upload', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
      const newDoc = res?.data?.data
      if (newDoc) {
        setDocuments((prev) => [newDoc, ...prev])
      }
      setDocUpload({ type: '', description: '', file: null })
      setSuccess('مدرک با موفقیت آپلود شد')
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در آپلود مدرک')
    } finally {
      setDocUploading(false)
    }
  }

  const handleDeleteDocument = async (docId) => {
    setError('')
    try {
      await api.delete(`/documents/${docId}`)
      setDocuments((prev) => prev.filter((item) => item.id !== docId))
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در حذف مدرک')
    }
  }

  const updateUserProfile = async () => {
    await api.put('/auth/profile', {
      phoneNumber: userForm.phoneNumber || null,
    })
  }

  const handleBrokerSubmit = async (event) => {
    event.preventDefault()
    setSaving(true)
    setError('')
    setSuccess('')
    try {
      await updateUserProfile()
      const payload = {
        companyName: brokerForm.companyName,
        licenseNumber: brokerForm.licenseNumber || null,
        city: brokerForm.city || null,
        province: brokerForm.province || null,
        address: brokerForm.address || null,
        personalAddress: brokerForm.personalAddress || null,
        legalAddress: brokerForm.legalAddress || null,
        postalCode: brokerForm.postalCode || null,
        website: brokerForm.website || null,
        yearsOfExperience: Number(brokerForm.yearsOfExperience || 0),
        description: brokerForm.description || null,
        isLegalEntity: brokerForm.isLegalEntity,
        nationalId: brokerForm.isLegalEntity ? null : brokerForm.nationalId || null,
        registrationNumber: brokerForm.isLegalEntity ? brokerForm.registrationNumber || null : null,
        economicCode: brokerForm.isLegalEntity ? brokerForm.economicCode || null : null,
        specializations:
          brokerForm.otherSpecialization && brokerForm.specializations.includes('سایر')
            ? [
                ...brokerForm.specializations.filter((s) => s !== 'سایر'),
                'سایر',
                `سایر: ${brokerForm.otherSpecialization}`,
              ]
            : brokerForm.specializations,
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
      await updateUserProfile()
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
      <div className="max-w-4xl mx-auto card p-6">
        <div className="flex items-center justify-between gap-4 flex-wrap">
          <h1 className="text-xl font-semibold">تکمیل پروفایل</h1>
          {profileImageUrl && (
            <img
              src={profileImageUrl}
              alt="profile"
              className="h-16 w-16 rounded-full object-cover border"
            />
          )}
        </div>

        {(error || success) && (
          <div
            className={`mt-4 rounded border px-3 py-2 text-sm ${
              error
                ? 'border-rose-200 bg-rose-50 text-rose-700'
                : 'border-emerald-200 bg-emerald-50 text-emerald-700'
            }`}
          >
            {error || success}
          </div>
        )}

        {loading && <div className="mt-4 text-sm text-slate-500">در حال بارگذاری...</div>}

        {!loading && (
          <div className="mt-6 space-y-8">
            <section className="card p-4">
              <div className="font-semibold mb-3">اطلاعات حساب</div>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                <input
                  className="px-3 py-2 rounded border"
                  placeholder="شماره موبایل"
                  value={userForm.phoneNumber}
                  onChange={(event) =>
                    setUserForm((prev) => ({ ...prev, phoneNumber: event.target.value }))
                  }
                />
                <label className="flex items-center gap-3 text-sm text-slate-600">
                  <input
                    type="file"
                    accept="image/*"
                    onChange={handleProfileImageUpload}
                    disabled={imageUploading}
                  />
                  {imageUploading ? 'در حال آپلود عکس...' : 'آپلود عکس پروفایل'}
                </label>
              </div>
            </section>

            {role === 'ClearanceAgent' && (
              <>
                <form className="card p-4 space-y-3" onSubmit={handleBrokerSubmit}>
                  <div className="font-semibold">اطلاعات ترخیص‌کار</div>
                  <input
                    className="w-full px-3 py-2 rounded border"
                    placeholder="نام شرکت"
                    value={brokerForm.companyName}
                    onChange={(event) =>
                      setBrokerForm((prev) => ({ ...prev, companyName: event.target.value }))
                    }
                    required
                  />
                  <textarea
                    className="w-full px-3 py-2 rounded border"
                    placeholder="توضیحات کوتاه"
                    rows="3"
                    value={brokerForm.description}
                    onChange={(event) =>
                      setBrokerForm((prev) => ({ ...prev, description: event.target.value }))
                    }
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
                          setBrokerForm((prev) => ({
                            ...prev,
                            registrationNumber: event.target.value,
                          }))
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
                    <select
                      className="px-3 py-2 rounded border bg-white"
                      value={brokerForm.province}
                      onChange={(event) =>
                        setBrokerForm((prev) => ({
                          ...prev,
                          province: event.target.value,
                          city: '',
                        }))
                      }
                    >
                      <option value="">انتخاب استان</option>
                      {PROVINCES.map((item) => (
                        <option key={item} value={item}>
                          {item}
                        </option>
                      ))}
                    </select>
                    <select
                      className="px-3 py-2 rounded border bg-white"
                      value={brokerForm.city}
                      onChange={(event) =>
                        setBrokerForm((prev) => ({ ...prev, city: event.target.value }))
                      }
                      disabled={!brokerForm.province}
                    >
                      <option value="">انتخاب شهر</option>
                      {cities.map((item) => (
                        <option key={item} value={item}>
                          {item}
                        </option>
                      ))}
                    </select>
                  </div>
                  <input
                    className="w-full px-3 py-2 rounded border"
                    placeholder="کد پستی"
                    value={brokerForm.postalCode}
                    onChange={(event) =>
                      setBrokerForm((prev) => ({ ...prev, postalCode: event.target.value }))
                    }
                  />
                  <input
                    className="w-full px-3 py-2 rounded border"
                    placeholder="آدرس دفتر/مرکز"
                    value={brokerForm.address}
                    onChange={(event) =>
                      setBrokerForm((prev) => ({ ...prev, address: event.target.value }))
                    }
                  />
                  {!brokerForm.isLegalEntity && (
                    <input
                      className="w-full px-3 py-2 rounded border"
                      placeholder="آدرس حقیقی"
                      value={brokerForm.personalAddress}
                      onChange={(event) =>
                        setBrokerForm((prev) => ({
                          ...prev,
                          personalAddress: event.target.value,
                        }))
                      }
                    />
                  )}
                  {brokerForm.isLegalEntity && (
                    <input
                      className="w-full px-3 py-2 rounded border"
                      placeholder="آدرس حقوقی"
                      value={brokerForm.legalAddress}
                      onChange={(event) =>
                        setBrokerForm((prev) => ({
                          ...prev,
                          legalAddress: event.target.value,
                        }))
                      }
                    />
                  )}
                  {/* <input
                    className="w-full px-3 py-2 rounded border"
                    placeholder="وب‌سایت"
                    value={brokerForm.website}
                    onChange={(event) =>
                      setBrokerForm((prev) => ({ ...prev, website: event.target.value }))
                    }
                  /> */}
                  
                 
                  {/* <input
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
                  /> */}
                  <div className="card p-4 space-y-2">
                    <div className="text-sm font-semibold">تخصص‌ها</div>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 text-sm">
                      {[
                        'واردات',
                        'صادرات',
                        'ترانزیت',
                        'کالای ملوانی',
                        'سایر',
                      ].map((item) => (
                        <label key={item} className="flex items-center gap-2">
                          <input
                            type="checkbox"
                            checked={brokerForm.specializations.includes(item)}
                            onChange={() =>
                              setBrokerForm((prev) => {
                                const next = prev.specializations.includes(item)
                                  ? prev.specializations.filter((s) => s !== item)
                                  : [...prev.specializations, item]
                                return { ...prev, specializations: next }
                              })
                            }
                          />
                          {item}
                        </label>
                      ))}
                    </div>
                    {brokerForm.specializations.includes('سایر') && (
                      <input
                        className="w-full px-3 py-2 rounded border"
                        placeholder="توضیحات سایر تخصص‌ها"
                        value={brokerForm.otherSpecialization}
                        onChange={(event) =>
                          setBrokerForm((prev) => ({ ...prev, otherSpecialization: event.target.value }))
                        }
                      />
                    )}
                  </div>
                  <button
                    type="submit"
                    className="px-4 py-2 rounded accent-btn"
                    disabled={saving}
                  >
                    {saving ? 'در حال ذخیره...' : 'ذخیره پروفایل'}
                  </button>
                </form>

                <section className="card p-4 space-y-3">
                  <div className="font-semibold">مدارک و اسناد</div>
                  <form className="grid grid-cols-1 sm:grid-cols-3 gap-3" onSubmit={handleDocumentUpload}>
                    <select
                      className="px-3 py-2 rounded border bg-white"
                      value={docUpload.type}
                      onChange={(event) =>
                        setDocUpload((prev) => ({ ...prev, type: event.target.value }))
                      }
                    >
                      <option value="">انتخاب نوع مدرک</option>
                      {documentTypes.map((item) => (
                        <option key={item.type} value={item.type}>
                          {item.description || item.type}
                        </option>
                      ))}
                    </select>
                    <input
                      className="px-3 py-2 rounded border"
                      placeholder="توضیحات"
                      value={docUpload.description}
                      onChange={(event) =>
                        setDocUpload((prev) => ({ ...prev, description: event.target.value }))
                      }
                    />
                    <input
                      className="px-3 py-2 rounded border"
                      type="file"
                      onChange={(event) =>
                        setDocUpload((prev) => ({ ...prev, file: event.target.files?.[0] || null }))
                      }
                    />
                    <button
                      type="submit"
                      className="px-4 py-2 rounded accent-btn"
                      disabled={docUploading}
                    >
                      {docUploading ? 'در حال آپلود...' : 'آپلود مدرک'}
                    </button>
                  </form>

                  <div className="space-y-2">
                    {documents.map((doc) => (
                      <div key={doc.id} className="border rounded p-3 flex flex-wrap items-center gap-2 text-sm">
                        <span className="font-semibold">{doc.documentType}</span>
                        <span className="text-slate-500">{doc.description || doc.fileName}</span>
                        <span
                          className={`text-xs px-2 py-1 rounded-full ${
                            doc.status === 'Approved' || doc.isVerified
                              ? 'bg-emerald-100 text-emerald-700'
                              : doc.status === 'Rejected'
                              ? 'bg-rose-100 text-rose-700'
                              : 'bg-amber-100 text-amber-700'
                          }`}
                        >
                          {doc.status === 'Approved' || doc.isVerified
                            ? 'تایید شده'
                            : doc.status === 'Rejected'
                            ? 'رد شده'
                            : 'در انتظار تایید'}
                        </span>
                        {doc.filePath && (
                          <a
                            className="text-slate-900 text-xs underline"
                            href={resolveMediaUrl(doc.filePath)}
                            target="_blank"
                            rel="noreferrer"
                          >
                            مشاهده/دانلود
                          </a>
                        )}
                        <button
                          className="text-rose-600 text-xs underline"
                          type="button"
                          onClick={() => handleDeleteDocument(doc.id)}
                        >
                          حذف
                        </button>
                      </div>
                    ))}
                    {documents.length === 0 && (
                      <div className="text-sm text-slate-500">مدرکی آپلود نشده است.</div>
                    )}
                  </div>
                </section>
              </>
            )}

            {role === 'CargoOwner' && (
              <form className="card p-4 space-y-3" onSubmit={handleOwnerSubmit}>
                <div className="font-semibold">اطلاعات صاحب کالا</div>
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
                  <select
                    className="px-3 py-2 rounded border bg-white"
                    value={ownerForm.province}
                    onChange={(event) =>
                      setOwnerForm((prev) => ({
                        ...prev,
                        province: event.target.value,
                        city: '',
                      }))
                    }
                  >
                    <option value="">انتخاب استان</option>
                    {PROVINCES.map((item) => (
                      <option key={item} value={item}>
                        {item}
                      </option>
                    ))}
                  </select>
                  <select
                    className="px-3 py-2 rounded border bg-white"
                    value={ownerForm.city}
                    onChange={(event) =>
                      setOwnerForm((prev) => ({ ...prev, city: event.target.value }))
                    }
                    disabled={!ownerForm.province}
                  >
                    <option value="">انتخاب شهر</option>
                    {ownerCities.map((item) => (
                      <option key={item} value={item}>
                        {item}
                      </option>
                    ))}
                  </select>
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
                  className="px-4 py-2 rounded accent-btn"
                  disabled={saving}
                >
                  {saving ? 'در حال ذخیره...' : 'ذخیره پروفایل'}
                </button>
              </form>
            )}
          </div>
        )}

        <div className="mt-6 text-sm text-slate-500 flex flex-wrap gap-2">
          <span>بعد از تکمیل پروفایل می‌توانید وارد داشبورد شوید.</span>
          {role === 'ClearanceAgent' && (
            <a className="text-slate-900 underline" href="/dashboard/broker">
              ورود به داشبورد ترخیص‌کار
            </a>
          )}
          {role === 'CargoOwner' && (
            <a className="text-slate-900 underline" href="/dashboard/owner">
              ورود به داشبورد صاحب کالا
            </a>
          )}
        </div>
      </div>
    </div>
  )
}
