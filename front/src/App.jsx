import React from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import AdminLayout from './components/AdminLayout'
import AdminDashboard from './pages/admin/Dashboard'
import AdminUsers from './pages/admin/Users'
import AdminVerifications from './pages/admin/Verifications'
import AdminReports from './pages/admin/Reports'
import AdminContent from './pages/admin/Content'
import AdminSettings from './pages/admin/Settings'
import Login from './pages/auth/Login'
import Register from './pages/auth/Register'
import Home from './pages/Home'
import SearchBrokers from './pages/SearchBrokers'
import ProfileComplete from './pages/ProfileComplete'
import BrokerProfile from './pages/BrokerProfile'
import UserLayout from './components/UserLayout'
import BrokerDashboard from './pages/broker/Dashboard'
import BrokerRequests from './pages/broker/Requests'
import BrokerAppointments from './pages/broker/Appointments'
import OwnerDashboard from './pages/owner/Dashboard'
import OwnerRequests from './pages/owner/Requests'
import OwnerFavorites from './pages/owner/Favorites'
import OwnerNotifications from './pages/owner/Notifications'

function RequireAuth({ children, requireAdmin = false }) {
  const token = localStorage.getItem('token') || localStorage.getItem('accessToken')
  const userInfo = (() => {
    try {
      return JSON.parse(localStorage.getItem('userInfo') || 'null')
    } catch (err) {
      return null
    }
  })()

  if (!token) {
    return <Navigate to="/login" replace />
  }
  if (requireAdmin && userInfo?.role !== 'Admin') {
    return <Navigate to="/login" replace />
  }
  return children
}

export default function App(){
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/brokers" element={<SearchBrokers />} />
      <Route path="/brokers/:id" element={<BrokerProfile />} />
      <Route
        path="/profile/complete"
        element={
          <RequireAuth>
            <ProfileComplete />
          </RequireAuth>
        }
      />
      <Route
        path="/dashboard/broker"
        element={
          <RequireAuth>
            <UserLayout />
          </RequireAuth>
        }
      >
        <Route index element={<BrokerDashboard />} />
        <Route path="requests" element={<BrokerRequests />} />
        <Route path="appointments" element={<BrokerAppointments />} />
      </Route>
      <Route
        path="/dashboard/owner"
        element={
          <RequireAuth>
            <UserLayout />
          </RequireAuth>
        }
      >
        <Route index element={<OwnerDashboard />} />
        <Route path="requests" element={<OwnerRequests />} />
        <Route path="favorites" element={<OwnerFavorites />} />
        <Route path="notifications" element={<OwnerNotifications />} />
      </Route>
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route
        path="/dashboard/admin"
        element={
          <RequireAuth requireAdmin>
            <AdminLayout />
          </RequireAuth>
        }
      >
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

