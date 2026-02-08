import React, { useMemo } from 'react'

const PROVINCES = ['تهران', 'هرمزگان', 'سیستان و بلوچستان', 'خراسان رضوی', 'اصفهان']
const CITIES = ['تهران', 'بندرعباس', 'چابهار', 'مشهد', 'اصفهان']
const SPECIALTIES = ['واردات', 'صادرات', 'ترانزیت', 'کالاهای صنعتی', 'مواد غذایی', 'کالاهای حجیم']
const SERVICE_TYPES = ['ترخیص کامل', 'مشاوره', 'ترانزیت', 'ترخیص فوری']

export default function AdvancedSearchFilters({
  filters,
  onChange,
  onSave,
  onReset,
}) {
  const selectedSpecialties = useMemo(() => new Set(filters.specialties), [filters.specialties])

  const toggleSpecialty = (value) => {
    const next = new Set(selectedSpecialties)
    if (next.has(value)) next.delete(value)
    else next.add(value)
    onChange({ ...filters, specialties: Array.from(next) })
  }

  return (
    <div className="space-y-4">
      <div className="card p-4">
        <div className="text-sm font-semibold mb-2">منطقه</div>
        <div className="space-y-2">
          <select
            className="w-full px-3 py-2 rounded border"
            value={filters.province}
            onChange={(event) => onChange({ ...filters, province: event.target.value })}
          >
            <option value="">همه استان‌ها</option>
            {PROVINCES.map((item) => (
              <option key={item} value={item}>{item}</option>
            ))}
          </select>
          <select
            className="w-full px-3 py-2 rounded border"
            value={filters.city}
            onChange={(event) => onChange({ ...filters, city: event.target.value })}
          >
            <option value="">همه شهرها</option>
            {CITIES.map((item) => (
              <option key={item} value={item}>{item}</option>
            ))}
          </select>
        </div>
      </div>

      <details className="card p-4" open>
        <summary className="cursor-pointer text-sm font-semibold">تخصص‌ها</summary>
        <div className="mt-3 space-y-2">
          {SPECIALTIES.map((item) => (
            <label key={item} className="flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                checked={selectedSpecialties.has(item)}
                onChange={() => toggleSpecialty(item)}
              />
              {item}
            </label>
          ))}
        </div>
      </details>

      <details className="card p-4" open>
        <summary className="cursor-pointer text-sm font-semibold">امتیاز</summary>
        <div className="mt-3">
          <input
            type="range"
            min="1"
            max="5"
            step="0.1"
            value={filters.rating}
            onChange={(event) => onChange({ ...filters, rating: Number(event.target.value) })}
            className="w-full"
          />
          <div className="text-xs text-slate-500 mt-1">حداقل امتیاز: {filters.rating}</div>
        </div>
      </details>

      <details className="card p-4" open>
        <summary className="cursor-pointer text-sm font-semibold">قیمت</summary>
        <div className="mt-3 space-y-2">
          <input
            type="range"
            min="1000000"
            max="50000000"
            step="500000"
            value={filters.maxPrice}
            onChange={(event) => onChange({ ...filters, maxPrice: Number(event.target.value) })}
            className="w-full"
          />
          <div className="text-xs text-slate-500">حداکثر قیمت: {filters.maxPrice.toLocaleString()} تومان</div>
        </div>
      </details>

      <div className="card p-4">
        <div className="text-sm font-semibold mb-2">نوع خدمات</div>
        <div className="space-y-2">
          {SERVICE_TYPES.map((item) => (
            <label key={item} className="flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                checked={filters.serviceTypes.includes(item)}
                onChange={() => {
                  const next = filters.serviceTypes.includes(item)
                    ? filters.serviceTypes.filter((s) => s !== item)
                    : [...filters.serviceTypes, item]
                  onChange({ ...filters, serviceTypes: next })
                }}
              />
              {item}
            </label>
          ))}
        </div>
      </div>

      <div className="flex gap-2">
        <button onClick={onSave} className="flex-1 px-3 py-2 rounded accent-btn text-sm">ذخیره فیلترها</button>
        <button onClick={onReset} className="flex-1 px-3 py-2 rounded border text-sm">بازنشانی</button>
      </div>
    </div>
  )
}
