import React from 'react'

export default function StatsCard({title, value, children}){
  return (
    <div className="bg-white dark:bg-slate-800 p-4 rounded shadow-sm">
      <div className="text-sm text-slate-500 dark:text-slate-300">{title}</div>
      <div className="mt-2 text-2xl font-bold">{value}</div>
      {children}
    </div>
  )
}
