import React from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import AdminLayout from './components/AdminLayout'
import AdminDashboard from './pages/admin/Dashboard'
import AdminUsers from './pages/admin/Users'
import AdminVerifications from './pages/admin/Verifications'
import AdminReports from './pages/admin/Reports'
import AdminContent from './pages/admin/Content'
import AdminSettings from './pages/admin/Settings'

export default function App(){
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/dashboard/admin" replace />} />
      <Route path="/dashboard/admin" element={<AdminLayout />}>
        <Route index element={<AdminDashboard />} />
        <Route path="users" element={<AdminUsers />} />
        <Route path="verifications" element={<AdminVerifications />} />
        <Route path="reports" element={<AdminReports />} />
        <Route path="content" element={<AdminContent />} />
        <Route path="settings" element={<AdminSettings />} />
      </Route>
      <Route path="*" element={<div className="p-8">صفحه مورد نظر یافت نشد</div>} />
    </Routes>
  )
}
