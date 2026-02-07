import React, { useState } from 'react'

export default function OwnerNotifications() {
  const [settings, setSettings] = useState({
    email: true,
    sms: false,
    push: true,
    newRequest: true,
    statusChange: true,
    message: true,
  })

  const toggle = (key) => {
    setSettings((prev) => ({ ...prev, [key]: !prev[key] }))
  }

  return (
    <div className="space-y-4">
      <h2 className="text-lg font-semibold">تنظیمات اعلان</h2>
      <div className="bg-white border rounded p-4 space-y-3">
        <div className="font-semibold text-sm">کانال‌های دریافت</div>
        <label className="flex items-center gap-2 text-sm">
          <input type="checkbox" checked={settings.email} onChange={() => toggle('email')} />
          ایمیل
        </label>
        <label className="flex items-center gap-2 text-sm">
          <input type="checkbox" checked={settings.sms} onChange={() => toggle('sms')} />
          پیامک
        </label>
        <label className="flex items-center gap-2 text-sm">
          <input type="checkbox" checked={settings.push} onChange={() => toggle('push')} />
          اعلان درون‌برنامه
        </label>
      </div>
      <div className="bg-white border rounded p-4 space-y-3">
        <div className="font-semibold text-sm">رویدادها</div>
        <label className="flex items-center gap-2 text-sm">
          <input type="checkbox" checked={settings.newRequest} onChange={() => toggle('newRequest')} />
          ثبت درخواست جدید
        </label>
        <label className="flex items-center gap-2 text-sm">
          <input type="checkbox" checked={settings.statusChange} onChange={() => toggle('statusChange')} />
          تغییر وضعیت درخواست
        </label>
        <label className="flex items-center gap-2 text-sm">
          <input type="checkbox" checked={settings.message} onChange={() => toggle('message')} />
          پیام جدید
        </label>
      </div>
      <div className="text-xs text-slate-500">
        این تنظیمات فعلا در سمت کلاینت ذخیره می‌شوند و بعدا به API متصل می‌شود.
      </div>
    </div>
  )
}
