import React from 'react'
import { useNavigate } from 'react-router-dom'
import { Star, MapPin, ArrowUpRight, Phone, Heart, Share2, BadgeCheck } from 'lucide-react'

function Stars({ value, count }) {
  const safeValue = Number.isFinite(Number(value)) ? Number(value) : 0
  const full = Math.round(safeValue)
  return (
    <div className="text-amber-500 text-sm flex items-center gap-2">
      <div className="flex items-center gap-1">
        {Array.from({ length: 5 }).map((_, idx) => (
          <Star
            key={idx}
            size={14}
            className={
              idx < full ? 'text-amber-500 fill-amber-400' : 'text-amber-300'
            }
          />
        ))}
      </div>
      <span className="text-slate-500">({safeValue.toFixed(1)})</span>
    </div>
  )
}

export default function BrokerCard({
  broker,
  view = 'grid',
  onRequest,
  showActions = false,
  isFavorite = false,
  onToggleFavorite,
  onShare,
  showVerifiedBadge = false,
}) {
  const navigate = useNavigate()
  const tags = broker?.tags || []
  const handleShare = async () => {
    if (!broker?.id) return
    const url = `${window.location.origin}/brokers/${broker.id}`
    if (onShare) {
      onShare(url, broker)
      return
    }
    if (navigator.share) {
      try {
        await navigator.share({ title: broker.name, url })
        return
      } catch (err) {
        // fall back to clipboard
      }
    }
    if (navigator.clipboard?.writeText) {
      await navigator.clipboard.writeText(url)
    }
  }

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
            <MapPin size={12} /> {broker.city || '-'} • {broker.experience || 0} سال تجربه
          </div>
          {showVerifiedBadge && broker.hasVerifiedDocuments && (
            <div className="mt-1 inline-flex items-center gap-1 text-xs text-emerald-600">
              <BadgeCheck size={12} /> مدارک تایید شده
            </div>
          )}
        </div>
      </div>
      <div className={`mt-3 ${view === 'list' ? 'md:mt-0 md:flex-1' : ''}`}>
        <Stars value={broker.rating} count={broker.ratingCount} />
        <div className="mt-2 flex flex-wrap gap-2">
          {tags.map((tag) => (
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
        <div className="mt-3 flex flex-wrap gap-2 items-center">
          <button
            className="px-3 py-2 rounded accent-btn text-sm flex items-center gap-2"
            onClick={() => onRequest && onRequest(broker)}
          >
            ارسال درخواست
          </button>
          <button
            className="px-3 py-2 rounded border text-sm flex items-center gap-2"
            onClick={() => {
              if (broker.phoneNumber) {
                window.location.href = `tel:${broker.phoneNumber}`
              }
            }}
            disabled={!broker.phoneNumber}
            title="تماس"
          >
            <Phone size={14} />
          </button>
          <button
            className="px-3 py-2 rounded border text-sm flex items-center gap-2"
            onClick={() => navigate(`/brokers/${broker.id}`)}
            title="جزئیات"
          >
            <ArrowUpRight size={14} />
          </button>
          {showActions && (
            <>
              {(onToggleFavorite || isFavorite) && (
                <button
                  className={`px-3 py-2 rounded border text-sm flex items-center gap-2 ${
                    onToggleFavorite ? '' : 'opacity-50 cursor-not-allowed'
                  }`}
                  onClick={() => onToggleFavorite && onToggleFavorite(broker.id)}
                  disabled={!onToggleFavorite}
                  title="ذخیره در علاقه‌مندی‌ها"
                >
                  <Heart
                    size={14}
                    className={isFavorite ? 'text-rose-600 fill-rose-500' : ''}
                  />
                </button>
              )}
              <button
                className="px-3 py-2 rounded border text-sm flex items-center gap-2"
                onClick={handleShare}
                title="اشتراک‌گذاری"
              >
                <Share2 size={14} />
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  )
}
