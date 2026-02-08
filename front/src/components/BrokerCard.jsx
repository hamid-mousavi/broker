import React from 'react'
import { useNavigate } from 'react-router-dom'
import { Star, MapPin, ArrowUpRight } from 'lucide-react'

function Stars({ value, count }) {
  if (value == null) {
    return <div className="text-sm text-slate-500">بدون امتیاز</div>
  }
  if (value === 0 && count === 0) {
    return <div className="text-sm text-slate-500">بدون امتیاز</div>
  }
  const full = Math.floor(value)
  const half = value - full >= 0.5
  const stars = Array.from({ length: 5 }).map((_, idx) => {
    if (idx < full) return '★'
    if (idx === full && half) return '☆'
    return '☆'
  })
  return (
    <div className="text-amber-500 text-sm flex items-center gap-2">
      <Star size={14} />
      {stars.join(' ')} <span className="text-slate-500">({Number(value).toFixed(1)})</span>
    </div>
  )
}

export default function BrokerCard({ broker, view = 'grid', onRequest }) {
  const navigate = useNavigate()
  return (
    <div
      className={`card p-4 ${
        view === 'list' ? 'flex flex-col md:flex-row md:items-center md:gap-6' : ''
      }`}
    >
      <div className="flex items-center gap-3">
        <div className="h-12 w-12 rounded-full bg-slate-200" />
        <div>
          <div className="font-semibold">{broker.name}</div>
          <div className="text-xs text-slate-500 flex items-center gap-1">
            <MapPin size={12} /> {broker.city} • {broker.experience} سال تجربه
          </div>
        </div>
      </div>
      <div className={`mt-3 ${view === 'list' ? 'md:mt-0 md:flex-1' : ''}`}>
        <Stars value={broker.rating} count={broker.ratingCount} />
        <div className="mt-2 flex flex-wrap gap-2">
          {broker.tags.map((tag) => (
            <span key={tag} className="text-xs px-2 py-1 rounded-full bg-slate-100">
              {tag}
            </span>
          ))}
        </div>
      </div>
      <div className={`mt-4 ${view === 'list' ? 'md:mt-0' : ''}`}>
        <div className="text-sm text-slate-500">شروع قیمت</div>
        <div className="text-lg font-semibold">
          {typeof broker.price === 'number' ? `${broker.price.toLocaleString()} تومان` : 'توافقی'}
        </div>
        <div className="mt-3 flex gap-2">
          <button
            className="px-3 py-2 rounded accent-btn text-sm flex items-center gap-2"
            onClick={() => onRequest && onRequest(broker)}
          >
            ارسال درخواست
          </button>
          <button
            className="px-3 py-2 rounded border text-sm flex items-center gap-2"
            onClick={() => navigate(`/brokers/${broker.id}`)}
          >
            جزئیات <ArrowUpRight size={14} />
          </button>
        </div>
      </div>
    </div>
  )
}
