import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import PublicLayout from '../components/PublicLayout'
import RequestModal from '../components/RequestModal'
import api from '../utils/api'

const TABS = [
  { key: 'services', label: 'خدمات و قیمت‌ها' },
  { key: 'reviews', label: 'نظرات و امتیازات' },
  { key: 'portfolio', label: 'نمونه کارها' },
  { key: 'contact', label: 'اطلاعات تماس' },
]

export default function BrokerProfile() {
  const { id } = useParams()
  const [activeTab, setActiveTab] = useState('services')
  const [profile, setProfile] = useState(null)
  const [userProfile, setUserProfile] = useState(null)
  const [reviews, setReviews] = useState(null)
  const [stats, setStats] = useState(null)
  const [loading, setLoading] = useState(true)
  const [requestOpen, setRequestOpen] = useState(false)
  const [error, setError] = useState('')
  const [ratingForm, setRatingForm] = useState({ score: 0, comment: '' })
  const [ratingSaving, setRatingSaving] = useState(false)
  const [ratingError, setRatingError] = useState('')
  const [ratingSuccess, setRatingSuccess] = useState('')
  const [myRating, setMyRating] = useState(null)
  const [editMode, setEditMode] = useState(false)
  const [hoverScore, setHoverScore] = useState(0)

  const userInfo = (() => {
    try {
      return JSON.parse(localStorage.getItem('userInfo') || 'null')
    } catch (err) {
      return null
    }
  })()
  const isLoggedIn = !!localStorage.getItem('token')
  const isOwner = userInfo?.role === 'CargoOwner'

  const resolveMediaUrl = (path) => {
    if (!path) return ''
    if (path.startsWith('http')) return path
    const baseUrl = api.defaults.baseURL || ''
    return `${baseUrl.replace('/api', '')}${path}`
  }

  useEffect(() => {
    const load = async () => {
      setLoading(true)
      setError('')
      try {
        const [profileRes, reviewsRes, statsRes, userRes] = await Promise.all([
          api.get(`/brokers/${id}`),
          api.get(`/brokers/${id}/reviews`, { params: { pageNumber: 1, pageSize: 5 } }),
          api.get(`/brokers/${id}/stats`),
          api.get('/auth/profile').catch(() => null),
        ])
        setProfile(profileRes?.data?.data || null)
        setReviews(reviewsRes?.data?.data || null)
        setStats(statsRes?.data?.data || null)
        setUserProfile(userRes?.data?.data || null)

        if (userInfo?.role === 'CargoOwner') {
          const list = Array.isArray(reviewsRes?.data?.data)
            ? reviewsRes?.data?.data
            : reviewsRes?.data?.data?.ratings || reviewsRes?.data?.data?.items || []
          const mine = list.find((r) => r.raterId === userInfo?.id)
          if (mine) {
            setMyRating(mine)
            setRatingForm({ score: mine.score || 0, comment: mine.comment || '' })
          }
        }
      } catch (err) {
        setError(err?.response?.data?.message || 'خطا در دریافت اطلاعات ترخیص‌کار')
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [id])

  const ratingSummary = stats?.summary || stats
  const reviewItems = Array.isArray(reviews)
    ? reviews
    : reviews?.ratings || reviews?.items || reviews?.data || []

  const submitRating = async (event) => {
    event.preventDefault()
    setRatingError('')
    setRatingSuccess('')
    if (!isLoggedIn) {
      setRatingError('برای ثبت نظر باید وارد شوید.')
      return
    }
    if (!isOwner) {
      setRatingError('فقط صاحب کالا می‌تواند نظر ثبت کند.')
      return
    }
    if (!ratingForm.score) {
      setRatingError('امتیاز را انتخاب کنید.')
      return
    }
    setRatingSaving(true)
    try {
      let saved = null
      if (editMode && myRating?.id) {
        const res = await api.put(`/reviews/${myRating.id}`, {
          score: Number(ratingForm.score),
          comment: ratingForm.comment || null,
        })
        saved = res?.data?.data
        setRatingSuccess('نظر شما ویرایش شد.')
      } else {
        const payload = {
          agentId: Number(id),
          score: Number(ratingForm.score),
          comment: ratingForm.comment || null,
        }
        const res = await api.post('/reviews', payload)
        saved = res?.data?.data
        if (!saved) {
          throw new Error(res?.data?.message || 'ثبت نظر ناموفق بود')
        }
        setRatingSuccess('نظر شما ثبت شد.')
      }

      if (saved) {
        setMyRating(saved)
        setEditMode(false)
        setReviews((prev) => ({
          ...(prev || {}),
          ratings: [
            saved,
            ...(prev?.ratings || prev?.items || []).filter((r) => r.id !== saved.id),
          ],
        }))
      }

      const summaryRes = await api.get(`/brokers/${id}/stats`)
      setStats(summaryRes?.data?.data || stats)
    } catch (err) {
      setRatingError(err?.response?.data?.message || 'خطا در ثبت نظر')
    } finally {
      setRatingSaving(false)
    }
  }

  const deleteRating = async () => {
    if (!myRating?.id) return
    setRatingSaving(true)
    setRatingError('')
    try {
      await api.delete(`/reviews/${myRating.id}`)
      setMyRating(null)
      setRatingForm({ score: 0, comment: '' })
      setEditMode(false)
      setRatingSuccess('نظر شما حذف شد.')
      const summaryRes = await api.get(`/brokers/${id}/stats`)
      setStats(summaryRes?.data?.data || stats)
    } catch (err) {
      setRatingError(err?.response?.data?.message || 'خطا در حذف نظر')
    } finally {
      setRatingSaving(false)
    }
  }

  return (
    <PublicLayout>
      <div className="max-w-6xl mx-auto px-4 py-8">
        {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}
        {error && (
          <div className="rounded border border-rose-200 bg-rose-50 text-rose-700 px-3 py-2 text-sm">
            {error}
          </div>
        )}

        {!loading && profile && (
          <div className="space-y-6">
            <div className="card p-6">
              <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
                <div className="flex items-center gap-4">
                  {userProfile?.profileImagePath ? (
                    <img
                      className="h-20 w-20 rounded-full object-cover border"
                      src={resolveMediaUrl(userProfile.profileImagePath)}
                      alt={profile.companyName}
                    />
                  ) : (
                    <div className="h-20 w-20 rounded-full bg-slate-200" />
                  )}
                  <div>
                    <div className="text-xl font-semibold">{profile.companyName}</div>
                    <div className="text-sm text-slate-500">
                      {profile.city || '-'} • {profile.province || '-'} • {profile.yearsOfExperience || 0} سال تجربه
                    </div>
                    <div className="mt-2 text-sm text-amber-600">
                      امتیاز: {profile.averageRating?.toFixed(1) || '0.0'} ({profile.totalRatings || 0})
                    </div>
                  </div>
                </div>
                <div className="flex flex-wrap gap-2">
                  <button
                    className="px-4 py-2 rounded accent-btn"
                    onClick={() => setRequestOpen(true)}
                  >
                    ارسال درخواست
                  </button>
                  <button className="px-4 py-2 rounded border">ذخیره در علاقه‌مندی‌ها</button>
                  <button className="px-4 py-2 rounded border">اشتراک‌گذاری</button>
                </div>
              </div>
            </div>

            <div className="flex flex-wrap gap-2">
                  {TABS.map((tab) => (
                    <button
                      key={tab.key}
                      onClick={() => setActiveTab(tab.key)}
                      className={`px-3 py-2 rounded border text-sm ${
                        activeTab === tab.key ? 'bg-slate-900 text-white' : ''
                      }`}
                    >
                      {tab.label}
                    </button>
                  ))}
            </div>

            {activeTab === 'services' && (
              <div className="card p-4 space-y-3">
                <div className="font-semibold">خدمات و تخصص‌ها</div>
                <div className="flex flex-wrap gap-2">
                  {(profile.specializations || []).map((item) => (
                    <span key={item} className="text-xs px-2 py-1 rounded-full bg-slate-100">
                      {item}
                    </span>
                  ))}
                  {(profile.specializations || []).length === 0 && (
                    <div className="text-sm text-slate-500">تخصصی ثبت نشده است.</div>
                  )}
                </div>
                <div className="text-sm text-slate-500">قیمت‌ها بعدا از سرویس‌ها اضافه می‌شود.</div>
              </div>
            )}

            {activeTab === 'reviews' && (
              <div className="grid grid-cols-1 lg:grid-cols-[1fr_260px] gap-4">
                <div className="card p-4 space-y-3">
                  <div className="font-semibold">نظرات کاربران</div>
                  <form className="card p-4 space-y-3" onSubmit={submitRating}>
                    <div className="text-sm font-semibold">ثبت نظر شما</div>
                    {(ratingError || ratingSuccess) && (
                      <div
                        className={`rounded border px-3 py-2 text-sm ${
                          ratingError
                            ? 'border-rose-200 bg-rose-50 text-rose-700'
                            : 'border-emerald-200 bg-emerald-50 text-emerald-700'
                        }`}
                      >
                        {ratingError || ratingSuccess}
                      </div>
                    )}
                    <div className="flex items-center gap-1 text-2xl">
                      {[1, 2, 3, 4, 5].map((n) => (
                        <button
                          type="button"
                          key={n}
                          onClick={() => setRatingForm((p) => ({ ...p, score: n }))}
                          onMouseEnter={() => setHoverScore(n)}
                          onMouseLeave={() => setHoverScore(0)}
                          className={`px-1 ${
                            (hoverScore || ratingForm.score) >= n ? 'text-amber-500' : 'text-slate-300'
                          }`}
                          aria-label={`امتیاز ${n}`}
                        >
                          ★
                        </button>
                      ))}
                    </div>
                    <textarea
                      className="w-full px-3 py-2 rounded border"
                      rows={3}
                      placeholder="نظر شما"
                      value={ratingForm.comment}
                      onChange={(e) => setRatingForm((p) => ({ ...p, comment: e.target.value }))}
                    />
                    <button
                      type="submit"
                      className="px-3 py-2 rounded accent-btn"
                      disabled={ratingSaving}
                    >
                      {ratingSaving ? 'در حال ثبت...' : editMode ? 'ویرایش نظر' : 'ثبت نظر'}
                    </button>
                    {myRating && (
                      <div className="flex gap-2">
                        <button
                          type="button"
                          className="px-3 py-2 rounded border text-sm"
                          onClick={() => setEditMode((p) => !p)}
                        >
                          {editMode ? 'انصراف از ویرایش' : 'ویرایش نظر من'}
                        </button>
                        <button
                          type="button"
                          className="px-3 py-2 rounded border text-sm text-rose-600"
                          onClick={deleteRating}
                        >
                          حذف نظر
                        </button>
                      </div>
                    )}
                  </form>
                  {reviewItems.map((item) => (
                    <div key={item.id} className="border rounded p-3">
                      <div className="text-sm font-semibold">
                        {'★'.repeat(item.score)} {'☆'.repeat(5 - item.score)}
                      </div>
                      <div className="text-sm text-slate-500">{item.comment || 'بدون نظر'}</div>
                    </div>
                  ))}
                  {reviewItems.length === 0 && (
                    <div className="text-sm text-slate-500">نظری ثبت نشده است.</div>
                  )}
                </div>
                <div className="card p-4">
                  <div className="font-semibold mb-2">خلاصه امتیاز</div>
                  <div className="text-2xl font-semibold">{ratingSummary?.average || profile.averageRating || 0}</div>
                  <div className="text-sm text-slate-500">از {ratingSummary?.totalRatings || profile.totalRatings || 0} نظر</div>
                </div>
              </div>
            )}

            {activeTab === 'portfolio' && (
              <div className="card p-4">
                <div className="font-semibold mb-2">نمونه کارها</div>
                <div className="text-sm text-slate-500">نمونه کار ثبت نشده است.</div>
              </div>
            )}

            {activeTab === 'contact' && (
              <div className="card p-4 space-y-2 text-sm">
                <div>استان: {profile.province || '-'}</div>
                <div>شهر: {profile.city || '-'}</div>
                <div>آدرس دفتر: {profile.address || '-'}</div>
                {profile.isLegalEntity ? (
                  <div>آدرس حقوقی: {profile.legalAddress || '-'}</div>
                ) : (
                  <div>آدرس حقیقی: {profile.personalAddress || '-'}</div>
                )}
                <div>شماره موبایل: {userProfile?.phoneNumber || '-'}</div>
                <div>وب‌سایت: {profile.website || '-'}</div>
                <div>شماره مجوز: {profile.licenseNumber || '-'}</div>
              </div>
            )}
          </div>
        )}
      </div>

      {requestOpen && (
        <RequestModal
          onClose={() => setRequestOpen(false)}
          brokerId={profile ? profile.id : null}
          brokerName={profile ? profile.companyName : ''}
        />
      )}
    </PublicLayout>
  )
}
