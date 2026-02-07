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
  const [reviews, setReviews] = useState(null)
  const [stats, setStats] = useState(null)
  const [loading, setLoading] = useState(true)
  const [requestOpen, setRequestOpen] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    const load = async () => {
      setLoading(true)
      setError('')
      try {
        const [profileRes, reviewsRes, statsRes] = await Promise.all([
          api.get(`/brokers/${id}`),
          api.get(`/brokers/${id}/reviews`, { params: { pageNumber: 1, pageSize: 5 } }),
          api.get(`/brokers/${id}/stats`),
        ])
        setProfile(profileRes?.data?.data || null)
        setReviews(reviewsRes?.data?.data || null)
        setStats(statsRes?.data?.data || null)
      } catch (err) {
        setError(err?.response?.data?.message || 'خطا در دریافت اطلاعات ترخیص‌کار')
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [id])

  const ratingSummary = stats?.summary || stats
  const reviewItems = reviews?.ratings || reviews?.items || reviews?.data || []

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
            <div className="bg-white border rounded-2xl p-6">
              <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
                <div className="flex items-center gap-4">
                  <div className="h-20 w-20 rounded-full bg-slate-200" />
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
                    className="px-4 py-2 rounded bg-slate-900 text-white"
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
              <div className="bg-white border rounded-xl p-4 space-y-3">
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
                <div className="bg-white border rounded-xl p-4 space-y-3">
                  <div className="font-semibold">نظرات کاربران</div>
                  {reviewItems.map((item) => (
                    <div key={item.id} className="border rounded p-3">
                      <div className="text-sm font-semibold">امتیاز: {item.score}</div>
                      <div className="text-sm text-slate-500">{item.comment || 'بدون نظر'}</div>
                    </div>
                  ))}
                  {reviewItems.length === 0 && (
                    <div className="text-sm text-slate-500">نظری ثبت نشده است.</div>
                  )}
                </div>
                <div className="bg-white border rounded-xl p-4">
                  <div className="font-semibold mb-2">خلاصه امتیاز</div>
                  <div className="text-2xl font-semibold">{ratingSummary?.average || profile.averageRating || 0}</div>
                  <div className="text-sm text-slate-500">از {ratingSummary?.totalRatings || profile.totalRatings || 0} نظر</div>
                </div>
              </div>
            )}

            {activeTab === 'portfolio' && (
              <div className="bg-white border rounded-xl p-4">
                <div className="font-semibold mb-2">نمونه کارها</div>
                <div className="text-sm text-slate-500">نمونه کار ثبت نشده است.</div>
              </div>
            )}

            {activeTab === 'contact' && (
              <div className="bg-white border rounded-xl p-4 space-y-2 text-sm">
                <div>آدرس: {profile.address || '-'}</div>
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
