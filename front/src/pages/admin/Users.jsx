import React, { useEffect, useMemo, useState } from 'react'
import { Plus, User, Search, Pencil, ShieldCheck, KeyRound, Trash2, PauseCircle, PlayCircle } from 'lucide-react'
import api from '../../utils/api'

const ROLE_OPTIONS = [
  { value: '', label: 'همه نقش‌ها' },
  { value: 'ClearanceAgent', label: 'ترخیص‌کار' },
  { value: 'CargoOwner', label: 'صاحب کالا' },
  { value: 'Admin', label: 'ادمین' },
]

const CREATE_ROLE_OPTIONS = [
  { value: 2, label: 'صاحب کالا' },
  { value: 1, label: 'ترخیص‌کار' },
  { value: 3, label: 'ادمین' },
]

const STATUS_OPTIONS = [
  { value: 'all', label: 'همه وضعیت‌ها' },
  { value: 'active', label: 'فعال' },
  { value: 'inactive', label: 'غیرفعال' },
]

const VERIFIED_OPTIONS = [
  { value: 'all', label: 'همه وضعیت تایید' },
  { value: 'verified', label: 'تایید شده' },
  { value: 'unverified', label: 'تایید نشده' },
]

const PAGE_SIZES = [10, 20, 30, 50]

function roleLabel(role) {
  const match = ROLE_OPTIONS.find((r) => r.value === role)
  return match ? match.label : role || '-'
}

function Filters({
  search,
  onSearch,
  role,
  onRole,
  status,
  onStatus,
  verified,
  onVerified,
  pageSize,
  onPageSize,
  onCreate,
}) {
  return (
    <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
      <div className="flex flex-col gap-2 lg:flex-row lg:items-center">
        <input
          aria-label="search"
          placeholder="جستجو بر اساس نام یا ایمیل"
          className="px-3 py-2 rounded border bg-white dark:bg-slate-900"
          value={search}
          onChange={(event) => onSearch(event.target.value)}
        />
        <select
          className="px-2 py-2 rounded border bg-white dark:bg-slate-900"
          value={role}
          onChange={(event) => onRole(event.target.value)}
        >
          {ROLE_OPTIONS.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
        <select
          className="px-2 py-2 rounded border bg-white dark:bg-slate-900"
          value={status}
          onChange={(event) => onStatus(event.target.value)}
        >
          {STATUS_OPTIONS.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
        <select
          className="px-2 py-2 rounded border bg-white dark:bg-slate-900"
          value={verified}
          onChange={(event) => onVerified(event.target.value)}
        >
          {VERIFIED_OPTIONS.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
      </div>
      <div className="flex flex-col gap-2 sm:flex-row sm:items-center">
        <select
          className="px-2 py-2 rounded border bg-white dark:bg-slate-900"
          value={pageSize}
          onChange={(event) => onPageSize(Number(event.target.value))}
        >
          {PAGE_SIZES.map((size) => (
            <option key={size} value={size}>
              {size} در هر صفحه
            </option>
          ))}
        </select>
        <button
          type="button"
          onClick={onCreate}
          className="px-3 py-2 rounded accent-btn flex items-center gap-2"
        >
          <Plus size={16} /> افزودن کاربر
        </button>
      </div>
    </div>
  )
}

export default function AdminUsers() {
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  const [search, setSearch] = useState('')
  const [roleFilter, setRoleFilter] = useState('')
  const [statusFilter, setStatusFilter] = useState('all')
  const [verifiedFilter, setVerifiedFilter] = useState('all')

  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [totalPages, setTotalPages] = useState(1)
  const [totalCount, setTotalCount] = useState(0)

  const [updatingId, setUpdatingId] = useState(null)
  const [showCreate, setShowCreate] = useState(false)
  const [creating, setCreating] = useState(false)
  const [createForm, setCreateForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    password: '',
    role: 2,
  })

  const [showDetails, setShowDetails] = useState(false)
  const [detailsLoading, setDetailsLoading] = useState(false)
  const [selectedUser, setSelectedUser] = useState(null)
  const [verifyingId, setVerifyingId] = useState(null)

  const [showEdit, setShowEdit] = useState(false)
  const [editingId, setEditingId] = useState(null)
  const [editForm, setEditForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
  })

  const [showPassword, setShowPassword] = useState(false)
  const [passwordUserId, setPasswordUserId] = useState(null)
  const [passwordForm, setPasswordForm] = useState({
    newPassword: '',
    confirmPassword: '',
  })
  const [passwordSaving, setPasswordSaving] = useState(false)
  const [deletingId, setDeletingId] = useState(null)

  const getErrorMessage = (err, fallback) => {
    const status = err?.response?.status
    if (status === 401) return 'دسترسی غیرمجاز است. لطفا وارد شوید.'
    if (status === 403) return 'دسترسی ندارید. لطفا با حساب ادمین وارد شوید.'
    const data = err?.response?.data
    if (data?.errors && Array.isArray(data.errors) && data.errors.length > 0) {
      return data.errors.join(' - ')
    }
    return data?.message || err?.message || fallback || 'خطای نامشخص'
  }

  const filteredUsers = useMemo(() => {
    if (!search.trim()) return users
    const query = search.trim().toLowerCase()
    return users.filter((user) => {
      const fullName = `${user.firstName} ${user.lastName}`.toLowerCase()
      return (
        fullName.includes(query) ||
        user.email?.toLowerCase().includes(query) ||
        user.phoneNumber?.toLowerCase().includes(query)
      )
    })
  }, [users, search])

  useEffect(() => {
    setPageNumber(1)
  }, [roleFilter, statusFilter, verifiedFilter, pageSize])

  useEffect(() => {
    const fetchUsers = async () => {
      setLoading(true)
      setError('')
      try {
        const params = {
          pageNumber,
          pageSize,
        }
        if (roleFilter) params.role = roleFilter
        if (statusFilter === 'active') params.isActive = true
        if (statusFilter === 'inactive') params.isActive = false
        if (verifiedFilter === 'verified') params.isVerified = true
        if (verifiedFilter === 'unverified') params.isVerified = false

        const response = await api.get('/admin/users', { params })
        const payload = response?.data?.data
        if (!payload) {
          throw new Error(response?.data?.message || 'دریافت اطلاعات ناموفق بود')
        }
        setUsers(payload.users || [])
        setTotalPages(payload.totalPages || 1)
        setTotalCount(payload.totalCount || 0)
      } catch (err) {
        setError(getErrorMessage(err, 'خطا در دریافت کاربران'))
      } finally {
        setLoading(false)
      }
    }

    fetchUsers()
  }, [pageNumber, pageSize, roleFilter, statusFilter, verifiedFilter])

  const handleToggleStatus = async (user) => {
    setUpdatingId(user.id)
    setError('')
    setSuccess('')
    try {
      const response = await api.put(`/admin/users/${user.id}/status`, {
        isActive: !user.isActive,
      })
      if (!response?.data?.success) {
        throw new Error(response?.data?.message || 'به‌روزرسانی ناموفق بود')
      }
      setUsers((prev) =>
        prev.map((item) =>
          item.id === user.id ? { ...item, isActive: !item.isActive } : item
        )
      )
      setSuccess('وضعیت کاربر با موفقیت به‌روزرسانی شد')
    } catch (err) {
      setError(getErrorMessage(err, 'خطا در به‌روزرسانی وضعیت کاربر'))
    } finally {
      setUpdatingId(null)
    }
  }

  const handleVerify = async (user) => {
    setVerifyingId(user.id)
    setError('')
    setSuccess('')
    try {
      const response = await api.put(`/admin/users/${user.id}/verify`, {
        isVerified: true,
      })
      if (!response?.data?.success) {
        throw new Error(response?.data?.message || 'به‌روزرسانی ناموفق بود')
      }
      setUsers((prev) =>
        prev.map((item) =>
          item.id === user.id ? { ...item, isVerified: true } : item
        )
      )
      setSuccess('کاربر تایید شد')
    } catch (err) {
      setError(getErrorMessage(err, 'خطا در تایید کاربر'))
    } finally {
      setVerifyingId(null)
    }
  }

  const openEdit = (user) => {
    setEditForm({
      firstName: user.firstName || '',
      lastName: user.lastName || '',
      email: user.email || '',
      phoneNumber: user.phoneNumber || '',
    })
    setEditingId(user.id)
    setShowEdit(true)
  }

  const handleEditSubmit = async (event) => {
    event.preventDefault()
    if (!editingId) return
    setError('')
    setSuccess('')
    try {
      const response = await api.put(`/admin/users/${editingId}`, editForm)
      const payload = response?.data?.data
      if (!payload) {
        throw new Error(response?.data?.message || 'ویرایش ناموفق بود')
      }
      setUsers((prev) =>
        prev.map((item) => (item.id === editingId ? payload : item))
      )
      setShowEdit(false)
      setSuccess('اطلاعات کاربر ویرایش شد')
    } catch (err) {
      setError(getErrorMessage(err, 'خطا در ویرایش کاربر'))
    }
  }

  const openPassword = (user) => {
    setPasswordForm({ newPassword: '', confirmPassword: '' })
    setPasswordUserId(user.id)
    setShowPassword(true)
  }

  const handlePasswordSubmit = async (event) => {
    event.preventDefault()
    if (!passwordUserId) return
    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      setError('رمز عبور و تکرار آن برابر نیستند')
      return
    }
    setPasswordSaving(true)
    setError('')
    setSuccess('')
    try {
      const response = await api.put(`/admin/users/${passwordUserId}/password`, {
        newPassword: passwordForm.newPassword,
      })
      if (!response?.data?.success) {
        throw new Error(response?.data?.message || 'تغییر رمز ناموفق بود')
      }
      setShowPassword(false)
      setSuccess('رمز عبور با موفقیت تغییر کرد')
    } catch (err) {
      setError(getErrorMessage(err, 'خطا در تغییر رمز عبور'))
    } finally {
      setPasswordSaving(false)
    }
  }

  const handleDelete = async (user) => {
    const confirmed = window.confirm(`آیا از حذف کاربر ${user.firstName} ${user.lastName} مطمئن هستید؟`)
    if (!confirmed) return
    setDeletingId(user.id)
    setError('')
    setSuccess('')
    try {
      const response = await api.delete(`/admin/users/${user.id}`)
      if (!response?.data?.success) {
        throw new Error(response?.data?.message || 'حذف ناموفق بود')
      }
      setUsers((prev) => prev.filter((item) => item.id !== user.id))
      setSuccess('کاربر حذف شد')
    } catch (err) {
      setError(getErrorMessage(err, 'خطا در حذف کاربر'))
    } finally {
      setDeletingId(null)
    }
  }

  const openDetails = async (userId) => {
    setShowDetails(true)
    setDetailsLoading(true)
    setSelectedUser(null)
    setError('')
    try {
      const response = await api.get(`/admin/users/${userId}`)
      const payload = response?.data?.data
      if (!payload) {
        throw new Error(response?.data?.message || 'دریافت جزئیات ناموفق بود')
      }
      setSelectedUser(payload)
    } catch (err) {
      setError(getErrorMessage(err, 'خطا در دریافت جزئیات کاربر'))
    } finally {
      setDetailsLoading(false)
    }
  }

  const handleCreate = async (event) => {
    event.preventDefault()
    setCreating(true)
    setError('')
    setSuccess('')
    try {
      const response = await api.post('/auth/register', createForm)
      if (!response?.data?.success) {
        throw new Error(response?.data?.message || 'ایجاد کاربر ناموفق بود')
      }
      setShowCreate(false)
      setCreateForm({
        firstName: '',
        lastName: '',
        email: '',
        phoneNumber: '',
        password: '',
        role: 2,
      })
      setSuccess('کاربر با موفقیت ایجاد شد')
      setPageNumber(1)
      const refresh = await api.get('/admin/users', {
        params: {
          pageNumber: 1,
          pageSize,
          role: roleFilter || undefined,
          isActive:
            statusFilter === 'active'
              ? true
              : statusFilter === 'inactive'
              ? false
              : undefined,
          isVerified:
            verifiedFilter === 'verified'
              ? true
              : verifiedFilter === 'unverified'
              ? false
              : undefined,
        },
      })
      const payload = refresh?.data?.data
      if (payload) {
        setUsers(payload.users || [])
        setTotalPages(payload.totalPages || 1)
        setTotalCount(payload.totalCount || 0)
      }
    } catch (err) {
      setError(getErrorMessage(err, 'خطا در ایجاد کاربر'))
    } finally {
      setCreating(false)
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
        <h2 className="text-lg font-semibold flex items-center gap-2">
          <User size={18} /> مدیریت کاربران
        </h2>
        <Filters
          search={search}
          onSearch={setSearch}
          role={roleFilter}
          onRole={setRoleFilter}
          status={statusFilter}
          onStatus={setStatusFilter}
          verified={verifiedFilter}
          onVerified={setVerifiedFilter}
          pageSize={pageSize}
          onPageSize={setPageSize}
          onCreate={() => setShowCreate(true)}
        />
      </div>

      {(error || success) && (
        <div
          className={`rounded border px-3 py-2 text-sm ${
            error
              ? 'border-rose-200 bg-rose-50 text-rose-700'
              : 'border-emerald-200 bg-emerald-50 text-emerald-700'
          }`}
        >
          {error || success}
        </div>
      )}

      <div className="card p-4">
        <div className="flex items-center justify-between text-sm text-slate-500 mb-3">
          <span>نمایش {filteredUsers.length} کاربر از {totalCount}</span>
          <span>
            صفحه {pageNumber} از {totalPages}
          </span>
        </div>
        <div className="space-y-3 md:hidden">
          {loading && (
            <div className="card p-4 text-center text-slate-400 text-sm">در حال دریافت اطلاعات...</div>
          )}
          {!loading && filteredUsers.length === 0 && (
            <div className="card p-4 text-center text-slate-400 text-sm">موردی یافت نشد.</div>
          )}
          {!loading &&
            filteredUsers.map((user) => (
              <div key={user.id} className="card p-4">
                <div className="font-semibold">
                  {user.firstName} {user.lastName}
                </div>
                <div className="text-xs text-slate-500 mt-1">{user.email}</div>
                <div className="mt-2 text-xs text-slate-600">{roleLabel(user.role)}</div>
                <div className="mt-2 flex flex-wrap gap-2">
                  <span
                    className={`inline-flex items-center rounded-full px-2 py-1 text-xs ${
                      user.isVerified
                        ? 'bg-emerald-100 text-emerald-700'
                        : 'bg-amber-100 text-amber-700'
                    }`}
                  >
                    {user.isVerified ? 'تایید شده' : 'تایید نشده'}
                  </span>
                  <span
                    className={`inline-flex items-center rounded-full px-2 py-1 text-xs ${
                      user.isActive
                        ? 'bg-sky-100 text-sky-700'
                        : 'bg-slate-200 text-slate-700'
                    }`}
                  >
                    {user.isActive ? 'فعال' : 'غیرفعال'}
                  </span>
                </div>
                <div className="mt-3 flex flex-wrap gap-2">
                  <button
                    type="button"
                    className="px-2 py-1 rounded border text-xs flex items-center gap-1"
                    onClick={() => openDetails(user.id)}
                  >
                    <Search size={12} /> جزئیات
                  </button>
                  <button
                    type="button"
                    className="px-2 py-1 rounded border text-xs flex items-center gap-1"
                    onClick={() => openEdit(user)}
                  >
                    <Pencil size={12} /> ویرایش
                  </button>
                  {!user.isVerified && (
                    <button
                      type="button"
                      className="px-2 py-1 rounded text-xs bg-emerald-50 text-emerald-700 border border-emerald-200 flex items-center gap-1"
                      onClick={() => handleVerify(user)}
                      disabled={verifyingId === user.id}
                    >
                      <ShieldCheck size={12} /> تایید
                    </button>
                  )}
                  <button
                    type="button"
                    className={`px-2 py-1 rounded text-xs flex items-center gap-1 ${
                      user.isActive
                        ? 'bg-rose-50 text-rose-700 border border-rose-200'
                        : 'bg-emerald-50 text-emerald-700 border border-emerald-200'
                    }`}
                    onClick={() => handleToggleStatus(user)}
                    disabled={updatingId === user.id}
                  >
                    {user.isActive ? <PauseCircle size={12} /> : <PlayCircle size={12} />}
                    {user.isActive ? 'تعلیق' : 'فعال‌سازی'}
                  </button>
                  <button
                    type="button"
                    className="px-2 py-1 rounded text-xs bg-rose-600 text-white flex items-center gap-1"
                    onClick={() => handleDelete(user)}
                    disabled={deletingId === user.id}
                  >
                    <Trash2 size={12} /> حذف
                  </button>
                </div>
              </div>
            ))}
        </div>
        <div className="overflow-auto">
          <table className="w-full text-sm table-auto hidden md:table">
            <thead>
              <tr className="text-slate-500">
                <th className="text-right p-2">نام</th>
                <th className="text-right p-2">ایمیل</th>
                <th className="text-right p-2">نقش</th>
                <th className="text-right p-2">تایید</th>
                <th className="text-right p-2">وضعیت</th>
                <th className="text-right p-2">عملیات</th>
              </tr>
            </thead>
            <tbody>
              {loading && (
                <tr>
                  <td className="p-4 text-center text-slate-400" colSpan={6}>
                    در حال دریافت اطلاعات...
                  </td>
                </tr>
              )}
              {!loading && filteredUsers.length === 0 && (
                <tr>
                  <td className="p-4 text-center text-slate-400" colSpan={6}>
                    موردی یافت نشد.
                  </td>
                </tr>
              )}
              {!loading &&
                filteredUsers.map((user) => (
                  <tr key={user.id} className="border-t">
                    <td className="p-2">
                      {user.firstName} {user.lastName}
                    </td>
                    <td className="p-2">{user.email}</td>
                    <td className="p-2">{roleLabel(user.role)}</td>
                    <td className="p-2">
                      <span
                        className={`inline-flex items-center rounded-full px-2 py-1 text-xs ${
                          user.isVerified
                            ? 'bg-emerald-100 text-emerald-700'
                            : 'bg-amber-100 text-amber-700'
                        }`}
                      >
                        {user.isVerified ? 'تایید شده' : 'تایید نشده'}
                      </span>
                    </td>
                    <td className="p-2">
                      <span
                        className={`inline-flex items-center rounded-full px-2 py-1 text-xs ${
                          user.isActive
                            ? 'bg-sky-100 text-sky-700'
                            : 'bg-slate-200 text-slate-700'
                        }`}
                      >
                        {user.isActive ? 'فعال' : 'غیرفعال'}
                      </span>
                    </td>
                    <td className="p-2">
                      <div className="flex flex-wrap gap-2">
                        <button
                          type="button"
                          className="px-2 py-1 rounded border text-xs flex items-center gap-1"
                          onClick={() => openDetails(user.id)}
                        >
                          <Search size={12} /> جزئیات
                        </button>
                        <button
                          type="button"
                          className="px-2 py-1 rounded border text-xs flex items-center gap-1"
                          onClick={() => openEdit(user)}
                        >
                          <Pencil size={12} /> ویرایش
                        </button>
                        {!user.isVerified && (
                          <button
                            type="button"
                            className="px-2 py-1 rounded text-xs bg-emerald-50 text-emerald-700 border border-emerald-200 flex items-center gap-1"
                            onClick={() => handleVerify(user)}
                            disabled={verifyingId === user.id}
                          >
                            <ShieldCheck size={12} /> {verifyingId === user.id ? 'در حال تایید...' : 'تایید'}
                          </button>
                        )}
                        <button
                          type="button"
                          className="px-2 py-1 rounded border text-xs flex items-center gap-1"
                          onClick={() => openPassword(user)}
                        >
                          <KeyRound size={12} /> تغییر رمز
                        </button>
                        <button
                          type="button"
                          className={`px-2 py-1 rounded text-xs flex items-center gap-1 ${
                            user.isActive
                              ? 'bg-rose-50 text-rose-700 border border-rose-200'
                              : 'bg-emerald-50 text-emerald-700 border border-emerald-200'
                          }`}
                          onClick={() => handleToggleStatus(user)}
                          disabled={updatingId === user.id}
                        >
                          {updatingId === user.id
                            ? 'در حال تغییر...'
                            : user.isActive
                            ? (<><PauseCircle size={12} /> تعلیق</>)
                            : (<><PlayCircle size={12} /> فعال‌سازی</>)}
                        </button>
                        <button
                          type="button"
                          className="px-2 py-1 rounded text-xs bg-rose-600 text-white flex items-center gap-1"
                          onClick={() => handleDelete(user)}
                          disabled={deletingId === user.id}
                        >
                          <Trash2 size={12} /> {deletingId === user.id ? 'در حال حذف...' : 'حذف'}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
            </tbody>
          </table>
        </div>

        <div className="mt-4 flex items-center justify-between">
          <button
            type="button"
            className="px-3 py-2 rounded border"
            onClick={() => setPageNumber((prev) => Math.max(prev - 1, 1))}
            disabled={pageNumber <= 1 || loading}
          >
            قبلی
          </button>
          <div className="flex gap-2">
            <button
              type="button"
              className="px-3 py-2 rounded border"
              onClick={() => setPageNumber(1)}
              disabled={pageNumber === 1 || loading}
            >
              اول
            </button>
            <button
              type="button"
              className="px-3 py-2 rounded border"
              onClick={() => setPageNumber(totalPages)}
              disabled={pageNumber === totalPages || loading}
            >
              آخر
            </button>
          </div>
          <button
            type="button"
            className="px-3 py-2 rounded border"
            onClick={() =>
              setPageNumber((prev) => Math.min(prev + 1, totalPages))
            }
            disabled={pageNumber >= totalPages || loading}
          >
            بعدی
          </button>
        </div>
      </div>

      {showCreate && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-lg card p-5">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold">افزودن کاربر</h3>
              <button
                type="button"
                onClick={() => setShowCreate(false)}
                className="text-slate-500"
              >
                بستن
              </button>
            </div>
            <form className="space-y-3" onSubmit={handleCreate}>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                <input
                  className="px-3 py-2 rounded border bg-white dark:bg-slate-900"
                  placeholder="نام"
                  value={createForm.firstName}
                  onChange={(event) =>
                    setCreateForm((prev) => ({
                      ...prev,
                      firstName: event.target.value,
                    }))
                  }
                  required
                />
                <input
                  className="px-3 py-2 rounded border bg-white dark:bg-slate-900"
                  placeholder="نام خانوادگی"
                  value={createForm.lastName}
                  onChange={(event) =>
                    setCreateForm((prev) => ({
                      ...prev,
                      lastName: event.target.value,
                    }))
                  }
                  required
                />
              </div>
              <input
                className="px-3 py-2 rounded border bg-white dark:bg-slate-900 w-full"
                placeholder="ایمیل"
                type="email"
                value={createForm.email}
                onChange={(event) =>
                  setCreateForm((prev) => ({
                    ...prev,
                    email: event.target.value,
                  }))
                }
                required
              />
              <input
                className="px-3 py-2 rounded border bg-white dark:bg-slate-900 w-full"
                placeholder="شماره موبایل"
                value={createForm.phoneNumber}
                onChange={(event) =>
                  setCreateForm((prev) => ({
                    ...prev,
                    phoneNumber: event.target.value,
                  }))
                }
                required
              />
              <input
                className="px-3 py-2 rounded border bg-white dark:bg-slate-900 w-full"
                placeholder="رمز عبور"
                type="password"
                value={createForm.password}
                onChange={(event) =>
                  setCreateForm((prev) => ({
                    ...prev,
                    password: event.target.value,
                  }))
                }
                required
                minLength={6}
              />
              <select
                className="px-3 py-2 rounded border bg-white dark:bg-slate-900 w-full"
                value={createForm.role}
                onChange={(event) =>
                  setCreateForm((prev) => ({
                    ...prev,
                    role: Number(event.target.value),
                  }))
                }
              >
                {CREATE_ROLE_OPTIONS.map((opt) => (
                  <option key={opt.value} value={opt.value}>
                    {opt.label}
                  </option>
                ))}
              </select>
              <div className="flex items-center justify-end gap-2">
                <button
                  type="button"
                  onClick={() => setShowCreate(false)}
                  className="px-3 py-2 rounded border"
                >
                  انصراف
                </button>
                <button
                  type="submit"
                  className="px-3 py-2 rounded accent-btn"
                  disabled={creating}
                >
                  {creating ? 'در حال ایجاد...' : 'ایجاد'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showDetails && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-lg card p-5">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold">جزئیات کاربر</h3>
              <button
                type="button"
                onClick={() => setShowDetails(false)}
                className="text-slate-500"
              >
                بستن
              </button>
            </div>
            {detailsLoading && (
              <div className="text-slate-400">در حال دریافت...</div>
            )}
            {!detailsLoading && selectedUser && (
              <div className="space-y-2 text-sm">
                <div>
                  نام: {selectedUser.firstName} {selectedUser.lastName}
                </div>
                <div>ایمیل: {selectedUser.email}</div>
                <div>شماره موبایل: {selectedUser.phoneNumber || '-'}</div>
                <div>نقش: {roleLabel(selectedUser.role)}</div>
                <div>وضعیت: {selectedUser.isActive ? 'فعال' : 'غیرفعال'}</div>
                <div>
                  تایید: {selectedUser.isVerified ? 'تایید شده' : 'تایید نشده'}
                </div>
                <div>
                  تاریخ ایجاد:{' '}
                  {selectedUser.createdAt
                    ? new Date(selectedUser.createdAt).toLocaleDateString('fa-IR')
                    : '-'}
                </div>
                <div>
                  آخرین بروزرسانی:{' '}
                  {selectedUser.updatedAt
                    ? new Date(selectedUser.updatedAt).toLocaleDateString('fa-IR')
                    : '-'}
                </div>
              </div>
            )}
          </div>
        </div>
      )}

      {showEdit && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-lg card p-5">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold">ویرایش کاربر</h3>
              <button
                type="button"
                onClick={() => setShowEdit(false)}
                className="text-slate-500"
              >
                بستن
              </button>
            </div>
            <form className="space-y-3" onSubmit={handleEditSubmit}>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                <input
                  className="px-3 py-2 rounded border bg-white dark:bg-slate-900"
                  placeholder="نام"
                  value={editForm.firstName}
                  onChange={(event) =>
                    setEditForm((prev) => ({
                      ...prev,
                      firstName: event.target.value,
                    }))
                  }
                  required
                />
                <input
                  className="px-3 py-2 rounded border bg-white dark:bg-slate-900"
                  placeholder="نام خانوادگی"
                  value={editForm.lastName}
                  onChange={(event) =>
                    setEditForm((prev) => ({
                      ...prev,
                      lastName: event.target.value,
                    }))
                  }
                  required
                />
              </div>
              <input
                className="px-3 py-2 rounded border bg-white dark:bg-slate-900 w-full"
                placeholder="ایمیل"
                type="email"
                value={editForm.email}
                onChange={(event) =>
                  setEditForm((prev) => ({
                    ...prev,
                    email: event.target.value,
                  }))
                }
                required
              />
              <input
                className="px-3 py-2 rounded border bg-white dark:bg-slate-900 w-full"
                placeholder="شماره موبایل"
                value={editForm.phoneNumber}
                onChange={(event) =>
                  setEditForm((prev) => ({
                    ...prev,
                    phoneNumber: event.target.value,
                  }))
                }
                required
              />
              <div className="flex items-center justify-end gap-2">
                <button
                  type="button"
                  onClick={() => setShowEdit(false)}
                  className="px-3 py-2 rounded border"
                >
                  انصراف
                </button>
                <button
                  type="submit"
                  className="px-3 py-2 rounded accent-btn"
                >
                  ذخیره
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showPassword && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-lg card p-5">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold">تغییر رمز عبور</h3>
              <button
                type="button"
                onClick={() => setShowPassword(false)}
                className="text-slate-500"
              >
                بستن
              </button>
            </div>
            <form className="space-y-3" onSubmit={handlePasswordSubmit}>
              <input
                className="px-3 py-2 rounded border bg-white dark:bg-slate-900 w-full"
                placeholder="رمز جدید"
                type="password"
                value={passwordForm.newPassword}
                onChange={(event) =>
                  setPasswordForm((prev) => ({
                    ...prev,
                    newPassword: event.target.value,
                  }))
                }
                minLength={6}
                required
              />
              <input
                className="px-3 py-2 rounded border bg-white dark:bg-slate-900 w-full"
                placeholder="تکرار رمز جدید"
                type="password"
                value={passwordForm.confirmPassword}
                onChange={(event) =>
                  setPasswordForm((prev) => ({
                    ...prev,
                    confirmPassword: event.target.value,
                  }))
                }
                minLength={6}
                required
              />
              <div className="flex items-center justify-end gap-2">
                <button
                  type="button"
                  onClick={() => setShowPassword(false)}
                  className="px-3 py-2 rounded border"
                >
                  انصراف
                </button>
                <button
                  type="submit"
                  className="px-3 py-2 rounded accent-btn"
                  disabled={passwordSaving}
                >
                  {passwordSaving ? 'در حال ذخیره...' : 'ذخیره'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
