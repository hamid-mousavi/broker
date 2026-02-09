import React, { useMemo, useState, useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { Search, SlidersHorizontal, LayoutGrid, List, MapPin } from 'lucide-react'
import BrokerCard from '../components/BrokerCard'
import AdvancedSearchFilters from '../components/AdvancedSearchFilters'
import RequestModal from '../components/RequestModal'
import PublicLayout from '../components/PublicLayout'
import api from '../utils/api'

const DEFAULT_FILTERS = {
  province: '',
  city: '',
  specialties: [],
  rating: 0,
  maxPrice: 30000000,
  serviceTypes: [],
}

export default function SearchBrokers() {
  const location = useLocation()
  const navigate = useNavigate()
  const isLoggedIn = !!localStorage.getItem('token')
  const userInfo = useMemo(() => {
    try {
      return JSON.parse(localStorage.getItem('userInfo') || 'null')
    } catch (err) {
      return null
    }
  }, [])
  const isOwner = userInfo?.role === 'CargoOwner'
  const [filters, setFilters] = useState(() => {
    const saved = localStorage.getItem('brokerFilters')
    return saved ? JSON.parse(saved) : DEFAULT_FILTERS
  })
  const [sortBy, setSortBy] = useState('rating')
  const [view, setView] = useState('grid')
  const [searchTerm, setSearchTerm] = useState('')
  const [brokers, setBrokers] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [requestOpen, setRequestOpen] = useState(false)
  const [selectedBroker, setSelectedBroker] = useState(null)
  const [favoriteIds, setFavoriteIds] = useState([])

  useEffect(() => {
    const params = new URLSearchParams(location.search)
    const term = params.get('search')
    if (term) {
      setSearchTerm(term)
      setPageNumber(1)
    }
  }, [location.search])

  const filtered = useMemo(() => {
    let list = [...brokers]
    if (filters.specialties.length > 0) {
      list = list.filter((b) =>
        filters.specialties.some((s) => (b.tags || []).includes(s))
      )
    }
    return list
  }, [brokers, filters.specialties])

  const sorted = useMemo(() => {
    const list = [...filtered]
    if (sortBy === 'rating') list.sort((a, b) => b.rating - a.rating)
    if (sortBy === 'experience') list.sort((a, b) => b.experience - a.experience)
    if (sortBy === 'price') list.sort((a, b) => a.price - b.price)
    return list
  }, [filtered, sortBy])

  const favoriteSet = useMemo(() => new Set(favoriteIds), [favoriteIds])

  useEffect(() => {
    if (!isLoggedIn || !isOwner) {
      setFavoriteIds([])
      return
    }
    const loadFavorites = async () => {
      try {
        const res = await api.get('/cargo-owners/favorites')
        const items = res?.data?.data || []
        setFavoriteIds(items.map((item) => item?.agent?.id).filter(Boolean))
      } catch (err) {
        setFavoriteIds([])
      }
    }
    loadFavorites()
  }, [isLoggedIn, isOwner])

  const handleToggleFavorite = async (brokerId) => {
    if (!isLoggedIn) {
      navigate('/login')
      return
    }
    if (!isOwner) return
    try {
      if (favoriteSet.has(brokerId)) {
        await api.delete(`/cargo-owners/favorites/${brokerId}`)
        setFavoriteIds((prev) => prev.filter((id) => id !== brokerId))
      } else {
        await api.post(`/cargo-owners/favorites/${brokerId}`)
        setFavoriteIds((prev) => [...prev, brokerId])
      }
    } catch (err) {
      // ignore for now
    }
  }

  const handleSave = () => {
    localStorage.setItem('brokerFilters', JSON.stringify(filters))
  }

  const handleReset = () => {
    setFilters(DEFAULT_FILTERS)
    localStorage.removeItem('brokerFilters')
  }

  useEffect(() => {
    const timeout = setTimeout(async () => {
      setLoading(true)
      setError('')
      try {
        const params = {
          city: filters.city || undefined,
          province: filters.province || undefined,
          specialization: filters.specialties[0] || undefined,
          minRating: filters.rating || undefined,
          searchTerm: searchTerm || undefined,
          pageNumber,
          pageSize: 10,
        }
        const response = await api.get('/brokers', { params })
        const payload = response?.data?.data
        if (!payload) {
          throw new Error(response?.data?.message || 'دریافت اطلاعات ناموفق بود')
        }
          const mapped = (payload.agents || []).map((agent) => ({
            id: agent.id,
            name: agent.companyName,
            city: agent.city || '-',
            province: agent.province || '-',
            rating: Number(agent.averageRating || 0),
            ratingCount: Number(agent.totalRatings || 0),
            experience: agent.yearsOfExperience || 0,
            tags: agent.specializations || [],
            phoneNumber: agent.phoneNumber || '',
            hasVerifiedDocuments: Boolean(agent.hasVerifiedDocuments),
            price: null,
          }))
        setBrokers(mapped)
        setTotalPages(payload.totalPages || 1)
      } catch (err) {
        setError(err?.response?.data?.message || 'خطا در دریافت ترخیص‌کاران')
      } finally {
        setLoading(false)
      }
    }, 300)

    return () => clearTimeout(timeout)
  }, [filters.city, filters.province, filters.specialties, filters.rating, searchTerm, pageNumber])

  return (
    <PublicLayout>
      <div className="max-w-6xl mx-auto px-4 py-8">
        <div className="flex flex-col gap-4 mb-6">
          <div>
            <h1 className="text-2xl font-semibold">جستجوی ترخیص‌کار</h1>
            <p className="text-sm text-slate-500">
              نتایج بر اساس فیلترهای انتخابی نمایش داده می‌شوند.
            </p>
          </div>

          <div className="flex flex-col lg:flex-row lg:items-center gap-2">
            <div className="relative flex-1">
              <Search size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input
                className="w-full px-3 py-2 pr-9 rounded border bg-white"
                placeholder="جستجو نام، تخصص یا شهر"
                value={searchTerm}
                onChange={(event) => {
                  setSearchTerm(event.target.value)
                  setPageNumber(1)
                }}
              />
            </div>
            <div className="flex flex-wrap items-center gap-2">
              <div className="flex items-center gap-2 text-sm text-slate-500">
                <SlidersHorizontal size={16} /> مرتب‌سازی
              </div>
              <select
                className="px-3 py-2 rounded border bg-white"
                value={sortBy}
                onChange={(event) => setSortBy(event.target.value)}
              >
                <option value="rating">امتیاز</option>
                <option value="experience">تجربه</option>
                <option value="price">قیمت</option>
              </select>
              <div className="flex gap-2">
                <button
                  className={`px-3 py-2 rounded border flex items-center gap-2 ${view === 'grid' ? 'bg-slate-900 text-white' : ''}`}
                  onClick={() => setView('grid')}
                >
                  <LayoutGrid size={16} /> گرید
                </button>
                <button
                  className={`px-3 py-2 rounded border flex items-center gap-2 ${view === 'list' ? 'bg-slate-900 text-white' : ''}`}
                  onClick={() => setView('list')}
                >
                  <List size={16} /> لیست
                </button>
              </div>
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-[280px_1fr] gap-6">
          <aside>
            <AdvancedSearchFilters
              filters={filters}
              onChange={setFilters}
              onSave={handleSave}
              onReset={handleReset}
            />
          </aside>

          <section className="space-y-4">
            <div className="card p-4">
              <div className="text-sm font-semibold mb-2 flex items-center gap-2">
                <MapPin size={16} /> نقشه موقعیت‌ها
              </div>
              {isLoggedIn ? (
                <iframe
                  title="map"
                  className="w-full h-48 rounded border"
                  loading="lazy"
                  src={`https://www.google.com/maps?q=${encodeURIComponent(
                    `${filters.city || ''} ${filters.province || ''}`.trim() || 'Iran'
                  )}&output=embed`}
                />
              ) : (
                <div className="h-48 rounded-lg bg-slate-100 flex items-center justify-center text-slate-500 text-sm">
                  برای نمایش نقشه وارد شوید
                </div>
              )}
            </div>

            {error && (
              <div className="rounded border border-rose-200 bg-rose-50 text-rose-700 px-3 py-2 text-sm">
                {error}
              </div>
            )}

            {loading && <div className="text-sm text-slate-500">در حال دریافت...</div>}

            {!loading && (
              <div className={`grid gap-4 ${view === 'grid' ? 'md:grid-cols-2' : ''}`}>
                {sorted.map((broker) => (
                  <BrokerCard
                    key={broker.id}
                    broker={broker}
                    view={view}
                    showActions
                    showVerifiedBadge
                    isFavorite={favoriteSet.has(broker.id)}
                    onToggleFavorite={isOwner ? handleToggleFavorite : null}
                    onRequest={(b) => {
                      setSelectedBroker(b)
                      setRequestOpen(true)
                    }}
                  />
                ))}
                {sorted.length === 0 && (
                  <div className="text-sm text-slate-500">نتیجه‌ای پیدا نشد.</div>
                )}
              </div>
            )}

            <div className="flex items-center justify-between">
              <button
                className="px-3 py-2 rounded border"
                onClick={() => setPageNumber((prev) => Math.max(prev - 1, 1))}
                disabled={pageNumber <= 1}
              >
                قبلی
              </button>
              <div className="text-xs text-slate-500">صفحه {pageNumber} از {totalPages}</div>
              <button
                className="px-3 py-2 rounded border"
                onClick={() => setPageNumber((prev) => Math.min(prev + 1, totalPages))}
                disabled={pageNumber >= totalPages}
              >
                بعدی
              </button>
            </div>
          </section>
        </div>
      </div>
      {requestOpen && (
        <RequestModal
          onClose={() => setRequestOpen(false)}
          brokerId={selectedBroker ? selectedBroker.id : null}
          brokerName={selectedBroker ? selectedBroker.name : ''}
        />
      )}
    </PublicLayout>
  )
}
