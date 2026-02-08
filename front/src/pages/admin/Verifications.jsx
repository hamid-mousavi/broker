import React, { useEffect, useState } from 'react'
import api from '../../utils/api'

export default function AdminVerifications() {
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [documents, setDocuments] = useState([])
  const [requests, setRequests] = useState([])

  const resolveMediaUrl = (path) => {
    if (!path) return ''
    if (path.startsWith('http')) return path
    const baseUrl = api.defaults.baseURL || ''
    return `${baseUrl.replace('/api', '')}${path}`
  }

  const loadData = async () => {
    setLoading(true)
    setError('')
    try {
      const [docsRes, reqRes] = await Promise.all([
        api.get('/admin/documents/pending'),
        api.get('/admin/verifications/pending'),
      ])
      setDocuments(docsRes?.data?.data || [])
      setRequests(reqRes?.data?.data || [])
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در دریافت اطلاعات تایید')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadData()
  }, [])

  const handleApproveDocument = async (id) => {
    try {
      await api.post(`/admin/documents/${id}/approve`)
      setDocuments((prev) => prev.filter((item) => item.id !== id))
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در تایید مدرک')
    }
  }

  const handleRejectDocument = async (id) => {
    try {
      await api.post(`/admin/documents/${id}/reject`)
      setDocuments((prev) => prev.filter((item) => item.id !== id))
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در رد مدرک')
    }
  }

  const handleApproveRequest = async (id) => {
    try {
      await api.post(`/admin/verifications/${id}/approve`, { adminNotes: '' })
      setRequests((prev) => prev.filter((item) => item.id !== id))
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در تایید درخواست')
    }
  }

  const handleRejectRequest = async (id) => {
    try {
      await api.post(`/admin/verifications/${id}/reject`, { adminNotes: 'رد شد' })
      setRequests((prev) => prev.filter((item) => item.id !== id))
    } catch (err) {
      setError(err?.response?.data?.message || 'خطا در رد درخواست')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">تایید مدارک</h2>
        <button className="px-3 py-2 rounded border" onClick={loadData}>
          بروزرسانی
        </button>
      </div>

      {error && (
        <div className="rounded border border-rose-200 bg-rose-50 text-rose-700 px-3 py-2 text-sm">
          {error}
        </div>
      )}

      {loading && <div className="text-sm text-slate-500">در حال بارگذاری...</div>}

      {!loading && (
        <>
          <section className="card p-4 space-y-3">
            <div className="font-semibold">مدارک در انتظار تایید</div>
            {documents.length === 0 && (
              <div className="text-sm text-slate-500">مدرکی برای بررسی وجود ندارد.</div>
            )}
            {documents.map((doc) => (
              <div key={doc.id} className="border rounded p-3 flex flex-col gap-2">
                <div className="flex flex-wrap items-center justify-between gap-2">
                  <div>
                    <div className="font-medium">
                      {doc.userName} - {doc.documentType}
                    </div>
                    <div className="text-sm text-slate-500">
                      {doc.userEmail} {doc.userPhoneNumber ? `• ${doc.userPhoneNumber}` : ''} • {doc.userRole}
                    </div>
                  </div>
                  <div className="flex flex-wrap gap-2">
                    {doc.filePath && (
                      <a
                        className="px-3 py-1 rounded border text-sm"
                        href={resolveMediaUrl(doc.filePath)}
                        target="_blank"
                        rel="noreferrer"
                      >
                        مشاهده فایل
                      </a>
                    )}
                    <button
                      className="px-3 py-1 rounded bg-emerald-600 text-white text-sm"
                      onClick={() => handleApproveDocument(doc.id)}
                    >
                      تایید
                    </button>
                    <button
                      className="px-3 py-1 rounded bg-rose-600 text-white text-sm"
                      onClick={() => handleRejectDocument(doc.id)}
                    >
                      رد
                    </button>
                  </div>
                </div>
                {doc.description && <div className="text-sm text-slate-500">{doc.description}</div>}
              </div>
            ))}
          </section>

          <section className="card p-4 space-y-3">
            <div className="font-semibold">درخواست‌های تایید هویت</div>
            {requests.length === 0 && (
              <div className="text-sm text-slate-500">درخواستی برای بررسی وجود ندارد.</div>
            )}
            {requests.map((req) => (
              <div key={req.id} className="border rounded p-3 flex items-center justify-between gap-3 flex-wrap">
                <div>
                  <div className="font-medium">{req.agentCompanyName}</div>
                  <div className="text-sm text-slate-500">{req.agentEmail}</div>
                </div>
                <div className="flex gap-2">
                  <button
                    className="px-3 py-1 rounded bg-emerald-600 text-white text-sm"
                    onClick={() => handleApproveRequest(req.id)}
                  >
                    تایید
                  </button>
                  <button
                    className="px-3 py-1 rounded bg-rose-600 text-white text-sm"
                    onClick={() => handleRejectRequest(req.id)}
                  >
                    رد
                  </button>
                </div>
              </div>
            ))}
          </section>
        </>
      )}
    </div>
  )
}
