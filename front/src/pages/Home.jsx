import React, { useEffect, useState } from 'react'
import { Award, ShieldCheck, MessageCircle, Clock, Search, Star, MapPin, Sparkles } from 'lucide-react'
import PublicLayout from '../components/PublicLayout'
import RequestModal from '../components/RequestModal'
import api from '../utils/api'

const FEATURES = [
  {
    icon: Award,
    title: 'انتخاب هوشمند ترخیص‌کار',
    desc: 'فیلترهای دقیق بر اساس تخصص، استان، امتیاز و تجربه.',
  },
  {
    icon: Clock,
    title: 'شفافیت قیمت و زمان',
    desc: 'مشاهده بازه هزینه و زمان تحویل قبل از ارسال درخواست.',
  },
  {
    icon: MessageCircle,
    title: 'پیگیری و پیام‌رسانی امن',
    desc: 'چت داخلی، ارسال فایل و تاریخچه کامل مکاتبات.',
  },
  {
    icon: ShieldCheck,
    title: 'اعتبارسنجی و امتیازدهی',
    desc: 'نظرات واقعی کاربران و امتیازدهی چندمعیاره.',
  },
]

const TOP_BROKERS = [
  {
    id: 1,
    name: 'محمد رضایی',
    city: 'تهران',
    rating: 4.9,
    tags: ['واردات', 'کالاهای صنعتی', 'تهران'],
  },
  {
    id: 2,
    name: 'ساناز کاظمی',
    city: 'بندرعباس',
    rating: 4.8,
    tags: ['صادرات', 'مواد غذایی', 'جنوب'],
  },
  {
    id: 3,
    name: 'رضا حسینی',
    city: 'چابهار',
    rating: 4.7,
    tags: ['ترانزیت', 'کالاهای حجیم', 'شرق'],
  },
]

const STATS = [
  { value: '+12,500', label: 'درخواست ثبت‌شده', icon: Sparkles },
  { value: '4.8/5', label: 'میانگین رضایت کاربران', icon: Star },
  { value: '1,200+', label: 'ترخیص‌کار فعال', icon: Award },
  { value: '98%', label: 'نرخ موفقیت', icon: ShieldCheck },
]

const TESTIMONIALS = [
  {
    name: 'مهتاب نوری',
    role: 'صاحب کالا',
    text: 'با فیلترهای دقیق خیلی سریع ترخیص‌کار مناسب پیدا کردم.',
  },
  {
    name: 'کاوه سعیدی',
    role: 'ترخیص‌کار',
    text: 'درخواست‌ها هدفمندتر شده و نرخ تبدیل بالا رفته.',
  },
  {
    name: 'الهام صمدی',
    role: 'مدیر بازرگانی',
    text: 'پیگیری مراحل و پیام‌رسانی داخلی فوق‌العاده است.',
  },
]

export default function Home() {
  const [requestOpen, setRequestOpen] = useState(false)
  const [selectedBroker, setSelectedBroker] = useState(null)
  const [topBrokers, setTopBrokers] = useState(TOP_BROKERS)
  const [stats, setStats] = useState(STATS)
  const [testimonials, setTestimonials] = useState(TESTIMONIALS)

  const openRequest = (broker) => {
    setSelectedBroker(broker)
    setRequestOpen(true)
  }

  useEffect(() => {
    const load = async () => {
      try {
        const res = await api.get('/brokers', { params: { pageNumber: 1, pageSize: 6 } })
        const payload = res?.data?.data
        const agents = payload?.agents || []
        if (agents.length > 0) {
          const mapped = agents.map((agent) => ({
            id: agent.id,
            name: agent.companyName,
            city: agent.city || '-',
            rating: Number(agent.averageRating || 0),
            tags: agent.specializations || [],
            completedRequests: Number(agent.completedRequests || 0),
          }))
          const sorted = [...mapped].sort((a, b) => b.rating - a.rating).slice(0, 3)
          setTopBrokers(sorted)

          const totalAgents = Number(payload?.totalCount || agents.length)
          const avgRating =
            agents.length > 0
              ? (agents.reduce((sum, a) => sum + Number(a.averageRating || 0), 0) / agents.length).toFixed(1)
              : '0.0'
          const totalCompleted = agents.reduce((sum, a) => sum + Number(a.completedRequests || 0), 0)

          setStats([
            { value: totalCompleted.toLocaleString(), label: 'درخواست ثبت‌شده', icon: Sparkles },
            { value: `${avgRating}/5`, label: 'میانگین رضایت کاربران', icon: Star },
            { value: totalAgents.toLocaleString(), label: 'ترخیص‌کار فعال', icon: Award },
            { value: totalAgents ? '—' : '—', label: 'نرخ موفقیت', icon: ShieldCheck },
          ])

          try {
            const first = sorted[0]
            if (first?.id) {
              const reviewsRes = await api.get(`/brokers/${first.id}/reviews`, { params: { pageNumber: 1, pageSize: 3 } })
              const reviewPayload = reviewsRes?.data?.data
              const reviewItems = reviewPayload?.ratings || reviewPayload?.items || reviewPayload?.data || []
              if (reviewItems.length > 0) {
                setTestimonials(
                  reviewItems.map((item) => ({
                    name: item.userName || 'کاربر',
                    role: 'صاحب کالا',
                    text: item.comment || 'بدون نظر',
                  }))
                )
              }
            }
          } catch (err) {
            // keep fallback testimonials
          }
        }
      } catch (err) {
        // keep fallback data
      }
    }
    load()
  }, [])

  return (
    <PublicLayout>
      <section className="max-w-6xl mx-auto px-4 py-10 md:py-14">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-10 items-center">
          <div className="space-y-5">
            <h1 className="text-3xl md:text-4xl font-bold leading-tight">
              ترخیص کالا سریع‌تر، شفاف‌تر و با اطمینان بیشتر
            </h1>
            <p className="text-slate-600">
              بهترین ترخیص‌کارها را بر اساس تخصص، منطقه و امتیاز مقایسه کن و با چند کلیک درخواستت را ثبت کن.
            </p>
            <div className="flex flex-wrap gap-3">
              <a href="#quick-search" className="px-5 py-3 rounded accent-btn shadow flex items-center gap-2">
                <Search size={18} /> جستجوی سریع ترخیص‌کار
              </a>
              <a href="#brokers" className="px-5 py-3 rounded border flex items-center gap-2">
                <Star size={18} /> مشاهده برترین‌ها
              </a>
            </div>
          </div>
          <div className="card p-6">
            <div className="text-sm text-slate-500 mb-2">در یک نگاه</div>
            <div className="grid grid-cols-2 gap-4">
              {stats.map((item) => (
                <div key={item.label} className="border rounded-xl p-4">
                  <div className="flex items-center justify-between">
                    <div className="text-xl font-semibold">{item.value}</div>
                    <item.icon size={18} className="text-slate-400" />
                  </div>
                  <div className="text-xs text-slate-500 mt-1">{item.label}</div>
                </div>
              ))}
            </div>
            <div className="mt-4 text-xs text-slate-400">به‌روزرسانی روزانه</div>
          </div>
        </div>
      </section>

      <section id="quick-search" className="max-w-6xl mx-auto px-4 pb-10">
        <div className="card p-6">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <h2 className="text-lg font-semibold">جستجوی سریع ترخیص‌کار</h2>
              <p className="text-sm text-slate-500">نام، تخصص یا شهر را وارد کنید.</p>
            </div>
            <div className="flex flex-col sm:flex-row gap-2 w-full md:w-auto">
              <div className="relative w-full sm:w-64">
                <Search size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
                <input
                  className="px-3 py-2 pr-9 rounded border w-full"
                  placeholder="مثلا: ترخیص‌کار تهران"
                />
              </div>
              <button className="px-4 py-2 rounded accent-btn flex items-center gap-2">
                <Search size={16} /> جستجو
              </button>
            </div>
          </div>
        </div>
      </section>

      <section id="features" className="max-w-6xl mx-auto px-4 py-10">
        <h2 className="text-xl font-semibold mb-6">ویژگی‌های پلتفرم</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {FEATURES.map((item) => (
            <div key={item.title} className="card p-4">
              <div className="h-10 w-10 rounded-full bg-slate-100 flex items-center justify-center mb-3">
                <item.icon size={18} className="text-slate-700" />
              </div>
              <div className="font-semibold mb-2">{item.title}</div>
              <div className="text-sm text-slate-600">{item.desc}</div>
            </div>
          ))}
        </div>
      </section>

      <section id="brokers" className="max-w-6xl mx-auto px-4 py-10">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-xl font-semibold">ترخیص‌کاران </h2>
          <a className="text-sm text-slate-500 hover:text-slate-900" href="/brokers">
            مشاهده همه
          </a>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {topBrokers.map((broker) => (
            <div key={broker.name} className="card p-4">
              <div className="flex items-center justify-between">
                <div className="font-semibold">{broker.name}</div>
                <div className="text-sm text-amber-500 flex items-center gap-1">
                  <Star size={14} /> {broker.rating}
                </div>
              </div>
              <div className="text-xs text-slate-500 mt-1 flex items-center gap-1">
                <MapPin size={12} /> {broker.city}
              </div>
              <div className="mt-3 flex flex-wrap gap-2">
                {broker.tags.map((tag) => (
                  <span key={tag} className="text-xs px-2 py-1 rounded-full bg-slate-100">
                    {tag}
                  </span>
                ))}
              </div>
              <button
                className="mt-4 w-full px-3 py-2 rounded accent-btn text-sm"
                onClick={() => openRequest(broker)}
              >
                ارسال درخواست
              </button>
            </div>
          ))}
        </div>
      </section>

      <section id="stats" className="max-w-6xl mx-auto px-4 py-10">
        <div
          className="text-white rounded-2xl p-8 grid grid-cols-1 md:grid-cols-4 gap-6"
          style={{ background: 'linear-gradient(135deg, rgb(14 116 144), rgb(217 119 6))' }}
        >
          {stats.map((item) => (
            <div key={item.label}>
              <div className="flex items-center gap-2 text-2xl font-semibold">
                <item.icon size={20} /> {item.value}
              </div>
              <div className="text-sm text-slate-300">{item.label}</div>
            </div>
          ))}
        </div>
      </section>

      <section id="testimonials" className="max-w-6xl mx-auto px-4 py-10">
        <h2 className="text-xl font-semibold mb-6">نظرات کاربران</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {testimonials.map((item) => (
            <div key={item.name} className="card p-4">
              <div className="text-sm text-slate-600 leading-relaxed">"{item.text}"</div>
              <div className="mt-3 text-sm font-semibold">{item.name}</div>
              <div className="text-xs text-slate-500">{item.role}</div>
            </div>
          ))}
        </div>
      </section>

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
