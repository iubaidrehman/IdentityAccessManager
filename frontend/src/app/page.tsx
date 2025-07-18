'use client'

import { useEffect, useState } from 'react'
import { LoginForm } from '../components/auth/LoginForm'
import { Dashboard } from '../components/dashboard/Dashboard'
import { useAuth } from '../hooks/useAuth'

export default function HomePage() {
  const { user, isLoading } = useAuth()

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-primary"></div>
      </div>
    )
  }

  if (!user) {
    return <LoginForm />
  }

  return <Dashboard />
} 