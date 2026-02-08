import React from 'react'

export default function StatsCard({ title, value, icon: Icon, children }) {
  return (
    <div className="card p-4">
      <div className="text-sm text-slate-500 flex items-center gap-2">
        {Icon && <Icon size={16} />}
        {title}
      </div>
      <div className="mt-2 text-2xl font-bold">{value}</div>
      {children}
    </div>
  )
}
