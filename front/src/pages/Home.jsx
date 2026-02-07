import React, { useState } from 'react'
import PublicLayout from '../components/PublicLayout'
import RequestModal from '../components/RequestModal'

const FEATURES = [
  {
    title: 'انتخاب هوشمند ترخیص‌کار',
    desc: 'فیلترهای دقیق بر اساس تخصص، استان، امتیاز و تجربه.',
  },
  {
    title: 'شفافیت قیمت و زمان',
    desc: 'مشاهده بازه هزینه و زمان تحویل قبل از ارسال درخواست.',
  },
  {
    title: 'پیگیری و پیام‌رسانی امن',
    desc: 'چت داخلی، ارسال فایل و تاریخچه کامل مکاتبات.',
  },
  {
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
  { value: '+12,500', label: 'درخواست ثبت‌شده' },
  { value: '4.8/5', label: 'میانگین رضایت کاربران' },
  { value: '1,200+', label: 'ترخیص‌کار فعال' },
  { value: '98%', label: 'نرخ موفقیت' },
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

  const openRequest = (broker) => {
    setSelectedBroker(broker)
    setRequestOpen(true)
  }

  return (
    <PublicLayout>

      <section className="max-w-6xl mx-auto px-4 py-12 md:py-16">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-10 items-center">
          <div className="space-y-5">
            <h1 className="text-3xl md:text-4xl font-bold leading-tight">
              ترخیص کالا سریع‌تر، شفاف‌تر و با اطمینان بیشتر
            </h1>
            <p className="text-slate-600">
              بهترین ترخیص‌کارها را بر اساس تخصص، منطقه و امتیاز مقایسه کن و با چند کلیک درخواستت را ثبت کن.
            </p>
            <div className="flex flex-wrap gap-3">
              <a href="#quick-search" className="px-5 py-3 rounded bg-slate-900 text-white">
                جستجوی سریع ترخیص‌کار
              </a>
              <a href="#brokers" className="px-5 py-3 rounded border">
                مشاهده برترین‌ها
              </a>
            </div>
          </div>
          <div className="bg-white rounded-2xl p-6 shadow">
            <div className="text-sm text-slate-500 mb-2">در یک نگاه</div>
            <div className="grid grid-cols-2 gap-4">
              {STATS.map((item) => (
                <div key={item.label} className="border rounded-xl p-4">
                  <div className="text-xl font-semibold">{item.value}</div>
                  <div className="text-xs text-slate-500">{item.label}</div>
                </div>
              ))}
            </div>
            <div className="mt-4 text-xs text-slate-400">به‌روزرسانی روزانه</div>
          </div>
        </div>
      </section>

      <section id="quick-search" className="max-w-6xl mx-auto px-4 pb-10">
        <div className="bg-white rounded-2xl p-6 shadow">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <h2 className="text-lg font-semibold">جستجوی سریع ترخیص‌کار</h2>
              <p className="text-sm text-slate-500">نام، تخصص یا شهر را وارد کنید.</p>
            </div>
            <div className="flex flex-col sm:flex-row gap-2 w-full md:w-auto">
              <input
                className="px-3 py-2 rounded border w-full sm:w-64"
                placeholder="مثلا: ترخیص‌کار تهران"
              />
              <button className="px-4 py-2 rounded bg-slate-900 text-white">جستجو</button>
            </div>
          </div>
        </div>
      </section>

      <section id="features" className="max-w-6xl mx-auto px-4 py-10">
        <h2 className="text-xl font-semibold mb-6">ویژگی‌های پلتفرم</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {FEATURES.map((item) => (
            <div key={item.title} className="bg-white rounded-xl p-4 shadow-sm border">
              <div className="font-semibold mb-2">{item.title}</div>
              <div className="text-sm text-slate-600">{item.desc}</div>
            </div>
          ))}
        </div>
      </section>

      <section id="brokers" className="max-w-6xl mx-auto px-4 py-10">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-xl font-semibold">ترخیص‌کاران برتر</h2>
          <a className="text-sm text-slate-500 hover:text-slate-900" href="#quick-search">
            مشاهده همه
          </a>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {TOP_BROKERS.map((broker) => (
            <div key={broker.name} className="bg-white rounded-xl p-4 border shadow-sm">
              <div className="flex items-center justify-between">
                <div className="font-semibold">{broker.name}</div>
                <div className="text-sm text-amber-500">★ {broker.rating}</div>
              </div>
              <div className="text-xs text-slate-500 mt-1">{broker.city}</div>
              <div className="mt-3 flex flex-wrap gap-2">
                {broker.tags.map((tag) => (
                  <span key={tag} className="text-xs px-2 py-1 rounded-full bg-slate-100">
                    {tag}
                  </span>
                ))}
              </div>
              <button
                className="mt-4 w-full px-3 py-2 rounded bg-slate-900 text-white text-sm"
                onClick={() => openRequest(broker)}
              >
                ارسال درخواست
              </button>
            </div>
          ))}
        </div>
      </section>

      <section id="stats" className="max-w-6xl mx-auto px-4 py-10">
        <div className="bg-slate-900 text-white rounded-2xl p-8 grid grid-cols-1 md:grid-cols-4 gap-6">
          {STATS.map((item) => (
            <div key={item.label}>
              <div className="text-2xl font-semibold">{item.value}</div>
              <div className="text-sm text-slate-300">{item.label}</div>
            </div>
          ))}
        </div>
      </section>

      <section id="testimonials" className="max-w-6xl mx-auto px-4 py-10">
        <h2 className="text-xl font-semibold mb-6">نظرات کاربران</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {TESTIMONIALS.map((item) => (
            <div key={item.name} className="bg-white rounded-xl p-4 border shadow-sm">
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
