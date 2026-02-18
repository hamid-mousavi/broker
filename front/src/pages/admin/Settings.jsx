import React, { useEffect, useState } from 'react'
import { Save, RefreshCw, Megaphone, Activity } from 'lucide-react'
import api from '../../utils/api'

const STORAGE_KEY = 'adminSystemSettings'

const defaultSettings = {
  siteName: 'سامانه ترخیص‌کار',
  supportEmail: 'support@broker.ir',
  supportPhone: '09120000000',
  maintenanceMode: false,
  allowRegistration: true,
  defaultLanguage: 'fa',
  timezone: 'Asia/Tehran',
  enforceStrongPassword: true,
  twoFactorForAdmins: false,
  maxLoginAttempts: 5,
  emailNotifications: true,
  smsNotifications: false,
  newRequestAlert: true,
  verificationAlert: true,
  autoBackupEnabled: true,
  backupCron: '0 2 * * *',
  backupRetentionDays: 14,
}

export default function AdminSettings() {
  const [settings, setSettings] = useState(defaultSettings)
  const [loading, setLoading] = useState(false)
  const [logsLoading, setLogsLoading] = useState(false)
  const [logs, setLogs] = useState([])
  const [announceLoading, setAnnounceLoading] = useState(false)
  const [announceForm, setAnnounceForm] = useState({
    title: '',
    content: '',
    type: 'Info',
    isActive: true,
  })
  const [success, setSuccess] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return
    try {
      const parsed = JSON.parse(raw)
      setSettings((prev) => ({ ...prev, ...parsed }))
    } catch (err) {
      // ignore invalid local value
    }
  }, [])

  const loadLogs = async () => {
    setLogsLoading(true)
    try {
      const res = await api.get('/admin/activity-logs', {
        params: { pageNumber: 1, pageSize: 10 },
      })
      setLogs(res?.data?.data?.logs || [])
    } catch (err) {
      setLogs([])
    } finally {
      setLogsLoading(false)
    }
  }

  useEffect(() => {
    loadLogs()
  }, [])

  const handleSaveSettings = async () => {
    setLoading(true)
    setSuccess('')
    setError('')
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(settings))
      setSuccess('تنظیمات ذخیره شد.')
    } catch (err) {
      setError('ذخیره تنظیمات ناموفق بود.')
    } finally {
      setLoading(false)
    }
  }

  const handleCreateAnnouncement = async (event) => {
    event.preventDefault()
    setAnnounceLoading(true)
    setSuccess('')
    setError('')
    try {
      await api.post('/admin/announcements', announceForm)
      setSuccess('اطلاعیه با موفقیت ثبت شد.')
      setAnnounceForm({
        title: '',
        content: '',
        type: 'Info',
        isActive: true,
      })
    } catch (err) {
      setError(err?.response?.data?.message || 'ثبت اطلاعیه ناموفق بود.')
    } finally {
      setAnnounceLoading(false)
    }
  }

  return (
    <div className="space-y-6">
      <h2 className="text-lg font-semibold">تنظیمات سیستم</h2>

      {success && (
        <div className="rounded border border-emerald-200 bg-emerald-50 text-emerald-700 px-3 py-2 text-sm">
          {success}
        </div>
      )}
      {error && (
        <div className="rounded border border-rose-200 bg-rose-50 text-rose-700 px-3 py-2 text-sm">
          {error}
        </div>
      )}

      <section className="card p-4 space-y-4">
        <h3 className="font-semibold">تنظیمات عمومی</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          <input
            className="px-3 py-2 rounded border"
            placeholder="نام سایت"
            value={settings.siteName}
            onChange={(e) => setSettings((p) => ({ ...p, siteName: e.target.value }))}
          />
          <input
            className="px-3 py-2 rounded border"
            placeholder="ایمیل پشتیبانی"
            value={settings.supportEmail}
            onChange={(e) => setSettings((p) => ({ ...p, supportEmail: e.target.value }))}
          />
          <input
            className="px-3 py-2 rounded border"
            placeholder="شماره پشتیبانی"
            value={settings.supportPhone}
            onChange={(e) => setSettings((p) => ({ ...p, supportPhone: e.target.value }))}
          />
          <select
            className="px-3 py-2 rounded border bg-white"
            value={settings.defaultLanguage}
            onChange={(e) => setSettings((p) => ({ ...p, defaultLanguage: e.target.value }))}
          >
            <option value="fa">فارسی</option>
            <option value="en">English</option>
          </select>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm">
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={settings.maintenanceMode}
              onChange={(e) => setSettings((p) => ({ ...p, maintenanceMode: e.target.checked }))}
            />
            حالت تعمیرات
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={settings.allowRegistration}
              onChange={(e) => setSettings((p) => ({ ...p, allowRegistration: e.target.checked }))}
            />
            امکان ثبت‌نام کاربران جدید
          </label>
        </div>
      </section>

      <section className="card p-4 space-y-4">
        <h3 className="font-semibold">امنیت و اعلان‌ها</h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
          <input
            type="number"
            min="1"
            className="px-3 py-2 rounded border"
            placeholder="حداکثر تلاش ورود"
            value={settings.maxLoginAttempts}
            onChange={(e) =>
              setSettings((p) => ({ ...p, maxLoginAttempts: Number(e.target.value || 1) }))
            }
          />
          <input
            className="px-3 py-2 rounded border"
            placeholder="Cron بکاپ"
            value={settings.backupCron}
            onChange={(e) => setSettings((p) => ({ ...p, backupCron: e.target.value }))}
          />
          <input
            type="number"
            min="1"
            className="px-3 py-2 rounded border"
            placeholder="نگهداری بکاپ (روز)"
            value={settings.backupRetentionDays}
            onChange={(e) =>
              setSettings((p) => ({ ...p, backupRetentionDays: Number(e.target.value || 1) }))
            }
          />
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm">
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={settings.enforceStrongPassword}
              onChange={(e) => setSettings((p) => ({ ...p, enforceStrongPassword: e.target.checked }))}
            />
            اجبار رمز قوی
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={settings.twoFactorForAdmins}
              onChange={(e) => setSettings((p) => ({ ...p, twoFactorForAdmins: e.target.checked }))}
            />
            احراز هویت دومرحله‌ای برای ادمین
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={settings.emailNotifications}
              onChange={(e) => setSettings((p) => ({ ...p, emailNotifications: e.target.checked }))}
            />
            اعلان ایمیلی
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={settings.smsNotifications}
              onChange={(e) => setSettings((p) => ({ ...p, smsNotifications: e.target.checked }))}
            />
            اعلان پیامکی
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={settings.newRequestAlert}
              onChange={(e) => setSettings((p) => ({ ...p, newRequestAlert: e.target.checked }))}
            />
            هشدار درخواست جدید
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={settings.verificationAlert}
              onChange={(e) => setSettings((p) => ({ ...p, verificationAlert: e.target.checked }))}
            />
            هشدار تایید مدارک
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={settings.autoBackupEnabled}
              onChange={(e) => setSettings((p) => ({ ...p, autoBackupEnabled: e.target.checked }))}
            />
            بکاپ خودکار
          </label>
        </div>
        <button
          type="button"
          onClick={handleSaveSettings}
          disabled={loading}
          className="px-4 py-2 rounded accent-btn inline-flex items-center gap-2"
        >
          <Save size={16} />
          {loading ? 'در حال ذخیره...' : 'ذخیره تنظیمات'}
        </button>
      </section>

      <section className="card p-4 space-y-4">
        <h3 className="font-semibold inline-flex items-center gap-2">
          <Megaphone size={16} /> اطلاعیه سیستمی
        </h3>
        <form onSubmit={handleCreateAnnouncement} className="space-y-3">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <input
              className="px-3 py-2 rounded border"
              placeholder="عنوان"
              value={announceForm.title}
              onChange={(e) => setAnnounceForm((p) => ({ ...p, title: e.target.value }))}
              required
            />
            <select
              className="px-3 py-2 rounded border bg-white"
              value={announceForm.type}
              onChange={(e) => setAnnounceForm((p) => ({ ...p, type: e.target.value }))}
            >
              <option value="Info">Info</option>
              <option value="Warning">Warning</option>
              <option value="Success">Success</option>
              <option value="Error">Error</option>
            </select>
          </div>
          <textarea
            className="w-full px-3 py-2 rounded border"
            rows={3}
            placeholder="متن اطلاعیه"
            value={announceForm.content}
            onChange={(e) => setAnnounceForm((p) => ({ ...p, content: e.target.value }))}
            required
          />
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={announceForm.isActive}
              onChange={(e) => setAnnounceForm((p) => ({ ...p, isActive: e.target.checked }))}
            />
            فعال باشد
          </label>
          <button
            type="submit"
            disabled={announceLoading}
            className="px-4 py-2 rounded border inline-flex items-center gap-2"
          >
            <Megaphone size={16} />
            {announceLoading ? 'در حال ثبت...' : 'ثبت اطلاعیه'}
          </button>
        </form>
      </section>

      <section className="card p-4 space-y-3">
        <div className="flex items-center justify-between">
          <h3 className="font-semibold inline-flex items-center gap-2">
            <Activity size={16} /> لاگ فعالیت‌ها
          </h3>
          <button
            type="button"
            onClick={loadLogs}
            className="px-3 py-2 rounded border text-sm inline-flex items-center gap-2"
          >
            <RefreshCw size={14} /> بروزرسانی
          </button>
        </div>
        {logsLoading ? (
          <div className="text-sm text-slate-500">در حال دریافت...</div>
        ) : logs.length === 0 ? (
          <div className="text-sm text-slate-500">لاگی ثبت نشده است.</div>
        ) : (
          <div className="overflow-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-slate-500">
                  <th className="text-right py-2">کاربر</th>
                  <th className="text-right py-2">عملیات</th>
                  <th className="text-right py-2">توضیح</th>
                  <th className="text-right py-2">زمان</th>
                </tr>
              </thead>
              <tbody>
                {logs.map((item) => (
                  <tr key={item.id} className="border-t">
                    <td className="py-2">{item.userName || '-'}</td>
                    <td className="py-2">{item.action || '-'}</td>
                    <td className="py-2">{item.description || '-'}</td>
                    <td className="py-2">
                      {item.createdAt ? new Date(item.createdAt).toLocaleString('fa-IR') : '-'}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  )
}
